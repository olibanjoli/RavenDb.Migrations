using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Migrations.For.RavenDb.Documents;
using Migrations.For.RavenDb.Logging;
using Raven.Client.Documents;

namespace Migrations.For.RavenDb
{
    public class Migrator
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly IDocumentStore store;

        public Migrator(IDocumentStore store)
        {
            this.store = store;
        }

        public void Run(Assembly migrations)
        {
            Log.Info("starting migration");

            Log.Debug($"discovering migrations from assembly {migrations.FullName}");

            var migrationClasses = migrations.GetLoadableTypes()
                .Where(p => typeof(DatabaseMigration).IsAssignableFrom(p) &&
                            p.IsAbstract == false &&
                            p.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(o => o.Name)
                .ToList();

            migrationClasses.ForEach(m => Log.Debug($" - {m.FullName}"));
            Log.Debug($" => {migrationClasses.Count} migrations found");

            IDictionary<string, Migration> executedMigrations;

            using (var session = this.store.OpenSession())
            {
                var executedMigrationsList = session.Query<Migration>().OrderBy(o => o.FullName).ToList();
                executedMigrations = executedMigrationsList.ToDictionary(k => k.FullName);

                Log.Debug($"the following migrations are already executed on the target:");
                executedMigrationsList.ForEach(m => Log.Debug($" - {m.FullName}"));
                Log.Debug($" => {executedMigrationsList.Count} executed migrations found");
            }

            foreach (var migrationClass in migrationClasses)
            {
                if (executedMigrations.ContainsKey(migrationClass.FullName))
                {
                    var migration = executedMigrations[migrationClass.FullName];

                    if (migration.End == null)
                    {
                        Log.Error($"unfinished migration detected in journal. will not continue to migrate. manual intervention required. 1) set and end value for the migration in order to skip the migration and continue with the next one. or 2) delete the migration document to retry the migration. (document id: '{migration.Id}')");
                        Environment.Exit(1);
                    }
                    
                    Log.Debug($"skipping migration {migrationClass.Name} - already executed");
                    continue;
                }

                var executedMigration = new Migration
                {
                    Id = $"Migrations/{migrationClass.Name.Substring(1)}",
                    FullName = migrationClass.FullName,
                    Start = DateTime.UtcNow
                };

                Log.Info($"executing migration {executedMigration.Id}");

                using (var session = this.store.OpenSession())
                {
                    session.Store(executedMigration);
                    session.SaveChanges();
                }

                var instance = (DatabaseMigration)Activator.CreateInstance(migrationClass);

                instance.Up(store);

                Log.Info($"migration {executedMigration.Id} finished, writing end date to journal entry...");

                using (var session = this.store.OpenSession())
                {
                    executedMigration.End = DateTime.UtcNow;

                    session.Store(executedMigration);
                    session.SaveChanges();
                }

                Log.Info($"migration {executedMigration.Id} done.");
            }
        }
    }
}
