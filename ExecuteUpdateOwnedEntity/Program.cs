using Microsoft.EntityFrameworkCore;

var contextOptions = new DbContextOptionsBuilder()
    .UseInMemoryDatabase("Test").Options;

var context = new EntityContext(contextOptions);
context.Entities.ExecuteUpdate(
    updates => updates.SetProperty(e => e.Name, "Test"));

public class Entity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public SubEntity Owned { get; set; }
}
public class SubEntity
{
    public string Value { get; set; }
}

class EntityContext : DbContext
{
    public EntityContext(DbContextOptions options)
        : base(options)
    { }

    public DbSet<Entity> Entities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var entityModelBuilder = modelBuilder.Entity<Entity>();
        entityModelBuilder
            .HasKey(e => e.Id);
        entityModelBuilder.OwnsOne(e => e.Owned);
    }
}
