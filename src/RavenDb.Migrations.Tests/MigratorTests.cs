using System.Linq;
using Raven.TestDriver;
using RavenDb.Migrations.Documents;
using RavenDb.Migrations.Tests.Infrastructure;
using RavenDb.Migrations.Tests.Migrations;
using Shouldly;
using Xunit;

namespace RavenDb.Migrations.Tests
{
    public class MigratorTests : RavenTestDriver<RavenDBLocator>
    {
        [Fact]
        public void Executing_Migration_Creates_Journal_Entry()
        {
            var store = GetDocumentStore();

            var migrator = new Migrator(store);

            migrator.Run(typeof(MigratorTests).Assembly);

            WaitForIndexing(store);

            using (var session = store.OpenSession())
            {
                var migrations = session.Query<Migration>().ToList();

                migrations.Count.ShouldBe(2);

                migrations.ShouldContain(p => p.FullName == typeof(_01_Test_Migration).FullName);
                migrations.ShouldContain(p => p.FullName == typeof(_02_Test_Migration).FullName);

                var migration1 = migrations.First(p => p.FullName == typeof(_01_Test_Migration).FullName);
                var migration2 = migrations.First(p => p.FullName == typeof(_02_Test_Migration).FullName);

                migration1.Id.ShouldNotBeNull();
                migration1.Start.ShouldNotBeNull();
                migration1.End.ShouldNotBeNull();

                migration2.Id.ShouldNotBeNull();
                migration2.Start.ShouldNotBeNull();
                migration2.End.ShouldNotBeNull();
            }
        }

        [Fact]
        public void Executed_Migration_Does_Not_Run_Twice()
        {
            var store = GetDocumentStore();

            var migrator = new Migrator(store);

            migrator.Run(typeof(MigratorTests).Assembly);
            migrator.Run(typeof(MigratorTests).Assembly);

            WaitForIndexing(store);

            using (var session = store.OpenSession())
            {
                var migrations = session.Query<Migration>().ToList();

                migrations.Count.ShouldBe(2);
            }
        }

        [Fact]
        public void Migrations_Executed_In_Order()
        {
            var store = GetDocumentStore();

            var migrator = new Migrator(store);

            migrator.Run(typeof(MigratorTests).Assembly);

            WaitForIndexing(store);

            using (var session = store.OpenSession())
            {
                var migrations = session.Query<Migration>().ToList();

                migrations.Count.ShouldBe(2);

                var migration1 = migrations.First(p => p.FullName == typeof(_01_Test_Migration).FullName);
                var migration2 = migrations.First(p => p.FullName == typeof(_02_Test_Migration).FullName);

                migration1.Start.ShouldNotBeNull();
                migration2.Start.ShouldNotBeNull();

                migration1.Start.Value.ShouldBeLessThan(migration2.Start.Value);
            }
        }
    }
}
