using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Queries;
using RavenDb.Migrations.Playground.Documents;

namespace RavenDb.Migrations.Playground
{
    public static class SampleData
    {
        public static void Provide(IDocumentStore store)
        {
            Clear(store);

            using (var session = store.OpenSession())
            {
                session.Store(new Car { Make = "Mitsubishi", Model = "Lancer Evolution IX" });
                session.Store(new Car { Make = "Mitsubishi", Model = "Eclipse" });
                session.Store(new Car { Make = "Toyota", Model = "Supra" });

                session.SaveChanges();
            }
        }

        private static void Clear(IDocumentStore store)
        {
            Task.WaitAll(store.Operations.Send(new DeleteByQueryOperation(new IndexQuery { Query = "from Cars" })).WaitForCompletionAsync());
        }
    }
}
