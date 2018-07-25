using Raven.Client.Documents;

namespace RavenDb.Migrations
{
    public abstract class DatabaseMigration
    {
        public abstract void Up(IDocumentStore store);
    }
}
