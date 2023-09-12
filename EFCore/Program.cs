using Microsoft.EntityFrameworkCore;

var contextOptions = new DbContextOptionsBuilder()
    .UseSqlServer("Server=localhost;Database=test;User Id=sa;Password=yourStrong(!)Password;Encrypt=false;MultipleActiveResultSets=true;").Options;

var context = new EntityContext(contextOptions);



class EntityContext : DbContext
{
    public EntityContext(DbContextOptions options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
