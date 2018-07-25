using System;
using System.IO;
using Raven.TestDriver;

namespace RavenDb.Migrations.Tests.Infrastructure
{
    public class RavenDBLocator : RavenServerLocator
    {
        private string serverPath;
        private string command = "dotnet";
        private const string RavenServerName = "Raven.Server";
        private string arguments;

        public override string ServerPath
        {
            get
            {
                if (string.IsNullOrEmpty(serverPath) == false)
                {
                    return serverPath;
                }
                var path = Environment.GetEnvironmentVariable("Raven_Server_Test_Path");
                if (string.IsNullOrEmpty(path) == false)
                {
                    if (InitializeFromPath(path))
                        return serverPath;
                }
                
                throw new FileNotFoundException($"Please setup Environment variable 'Raven_Server_Test_Path' pointing to ravendb server eg 'C:\\RavenDB-4.0.6-patch-40047'");
            }
        }

        private bool InitializeFromPath(string path)
        {
            var ext = Path.GetExtension(path);

            if (ext == ".dll")
            {
                serverPath = path;
                arguments = serverPath;

                return true;
            }

            if (ext == ".exe")
            {
                serverPath = path;
                command = serverPath;
                arguments = string.Empty;

                return true;
            }
            return false;
        }

        public override string Command => command;
        public override string CommandArguments => arguments;
    }
}