using Raven.Client.Documents;

namespace RavenDb.Migrations.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var store = new DocumentStore
            {
                Urls = new[] {"http://localhost:8081"},
                Database = "Playground"
            };

            store.Initialize();

            SampleData.Provide(store);

            var migrator = new Migrator(store);
            migrator.Run(typeof(Program).Assembly);
        }
    }
}
