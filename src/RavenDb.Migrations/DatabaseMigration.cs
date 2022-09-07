using System.Threading.Tasks;
using Raven.Client.Documents;

namespace Migrations.For.RavenDb
{
    public abstract class DatabaseMigration
    {
        public virtual void Up(IDocumentStore store)
        {
        }

        public virtual Task UpAsync(IDocumentStore store)
        {
            return Task.CompletedTask;
        }
    }
}