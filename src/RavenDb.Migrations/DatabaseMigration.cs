using Raven.Client.Documents;

namespace Migrations.For.RavenDb
{
    public abstract class DatabaseMigration
    {
        public abstract void Up(IDocumentStore store);
    }
}
