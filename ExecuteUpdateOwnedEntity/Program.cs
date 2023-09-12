using Microsoft.EntityFrameworkCore;

var contextOptions = new DbContextOptionsBuilder()
    .UseSqlServer("Server=localhost;Database=test;User Id=sa;Password=yourStrong(!)Password;Encrypt=false;MultipleActiveResultSets=true;").Options;

var context = new EntityContext(contextOptions);

string? newName = null;
string? newDisplayName = "new display name";

context.Entities.ExecuteUpdate(
    updates => updates
            .SetProperty(e => e.Name, e => newName ?? e.Name)
            .SetProperty(e => e.DisplayName, e => newDisplayName ?? e.DisplayName)
);

public class Entity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public SubEntity Owned { get; set; } = null!;
}
public class SubEntity
{
    public string Value { get; set; } = null!;
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
        entityModelBuilder.Property(e => e.Name).HasColumnName("Name").IsRequired();
        entityModelBuilder.Property(e => e.DisplayName).HasColumnName("DisplayName").IsRequired();
        entityModelBuilder.OwnsOne(e => e.Owned);
    }
}
