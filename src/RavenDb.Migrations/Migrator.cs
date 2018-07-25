using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Raven.Client.Documents;
using RavenDb.Migrations.Documents;

namespace RavenDb.Migrations
{
    public class Migrator
    {
        private readonly IDocumentStore store;

        public Migrator(IDocumentStore store)
        {
            this.store = store;
        }

        public void Run(Assembly migrations)
        {
            var migrationClasses = migrations.GetLoadableTypes()
                .Where(p => typeof(DatabaseMigration).IsAssignableFrom(p) &&
                            p.IsAbstract == false &&
                            p.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(o => o.Name)
                .ToList();

            IDictionary<string, Migration> executedMigrations;

            using (var session = this.store.OpenSession())
            {
                executedMigrations = session.Query<Migration>().ToList().ToDictionary(k => k.FullName);
            }

            foreach (var migrationClass in migrationClasses)
            {
                if (executedMigrations.ContainsKey(migrationClass.FullName))
                {
                    continue;
                }

                var executedMigration = new Migration
                {
                    Id = $"Migrations/{migrationClass.Name.Substring(1)}",
                    FullName = migrationClass.FullName,
                    Start = DateTime.UtcNow
                };

                using (var session = this.store.OpenSession())
                {
                    session.Store(executedMigration);
                    session.SaveChanges();
                }

                var instance = (DatabaseMigration)Activator.CreateInstance(migrationClass);

                instance.Up(store);

                using (var session = this.store.OpenSession())
                {
                    executedMigration.End = DateTime.UtcNow;

                    session.Store(executedMigration);
                    session.SaveChanges();
                }
            }
        }
    }
}
