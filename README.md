# Introduction
Migration framework for RavenDB 4. This library is currently work in progress. It implements a journaling mechanism which ensures that migrations are applied in the correct order and only once. In future helpers will be added like scripted updates etc.

### Implementing Migrations
```c#
    public class _0001_First_Migration : DatabaseMigration
    {
        public override void Up(IDocumentStore store)
        {
            // do your migration using store
        }
    }
```

### Run Migrations
```c#
  class Program
  {
    static void Main(string[] args)
    {
      var store = new DocumentStore
      {
          Urls = new[] {"http://localhost:8081"},
          Database = "Playground"
      };

      var migrator = new Migrator(store);
      migrator.Run(typeof(Program).Assembly); // assembly where migrations are implemented
    }
  }
```

### Ordering of Migrations
Migrations are order by class name (string ordering). Use number prefixes to control the order as seen in the examples.
