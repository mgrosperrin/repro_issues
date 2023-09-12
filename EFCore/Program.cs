using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

var contextOptions = new DbContextOptionsBuilder()
    .UseSqlServer("Server=localhost;Database=filteredinclude;User Id=sa;Password=yourStrong(!)Password;Encrypt=false;MultipleActiveResultSets=true;").Options;

var widgetId = Guid.NewGuid();
var dashboardId = Guid.NewGuid();
var context = new EntityContext(contextOptions);
var widgets = context.Dashboards
        .Include(x => x.Widgets.Where(w => w.WidgetId == widgetId))
        .Where(d => d.DashboardId == dashboardId)
        .SelectMany(d => d.Widgets);

Console.WriteLine(widgets.ToQueryString());

public  class Dashboard
{
    public Guid DashboardId { get; set; }
    public IEnumerable<Widget> Widgets { get; set; } = null!;
    public string Name { get; set; } = null!;
}

public class Widget
{
    public Guid WidgetId { get; set; }
    public Guid DashboardId { get; set; }
    public string Name { get; set; } = null!;
}
class EntityContext : DbContext
{
    public DbSet<Dashboard> Dashboards { get; set; }
    public EntityContext(DbContextOptions options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new DashboardMapping())
                    .ApplyConfiguration(new WidgetMapping())
                    ;
    }
    class DashboardMapping : IEntityTypeConfiguration<Dashboard>
    {
        public virtual void Configure(EntityTypeBuilder<Dashboard> builder)
        {
            builder.HasKey(e => e.DashboardId);
            builder.Property(e => e.DashboardId).HasColumnName("DashboardId").IsRequired();
            builder.HasMany(e => e.Widgets).WithOne().HasForeignKey(w => w.DashboardId);
            builder.Property(e => e.Name).HasColumnName("Name").IsRequired();
        }
    }
    class WidgetMapping : IEntityTypeConfiguration<Widget>
    {
        public virtual void Configure(EntityTypeBuilder<Widget> builder)
        {
            builder.HasKey(x => x.WidgetId);
            builder.Property(x => x.WidgetId).HasColumnName("WidgetId");
            builder.Property(x => x.DashboardId).HasColumnName("DashboardId");
            builder.Property(e => e.Name).HasColumnName("Name").IsRequired();
        }
    }
}
