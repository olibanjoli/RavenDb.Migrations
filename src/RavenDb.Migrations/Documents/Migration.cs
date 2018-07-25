using System;

namespace RavenDb.Migrations.Documents
{
    public class Migration
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
