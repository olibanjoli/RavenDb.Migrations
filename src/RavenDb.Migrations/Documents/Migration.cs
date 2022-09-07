using System;

namespace Migrations.For.RavenDb.Documents
{
    public class Migration
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        
        public Exception Exception { get; set; }
        public DateTime? ExceptionTime { get; set; }
        
        public double DurationInSeconds { get; set; }
        public double DurationInMinutes { get; set; }
    }
}
