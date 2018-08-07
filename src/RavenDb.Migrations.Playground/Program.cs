using System;
using Migrations.For.RavenDb;
using NLog;
using NLog.Config;
using NLog.Targets;
using Raven.Client.Documents;

namespace RavenDb.Migrations.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));
            LogManager.Configuration = config;

            var store = new DocumentStore
            {
                Urls = new[] {"http://localhost:8081"},
                Database = "Playground"
            };

            store.Initialize();

            SampleData.Provide(store);

            var migrator = new Migrator(store);
            migrator.Run(typeof(Program).Assembly);

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
