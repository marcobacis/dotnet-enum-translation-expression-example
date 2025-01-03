using Microsoft.EntityFrameworkCore;

namespace EnumTranslator;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<MyEntity> MyEntities { get; set; }
}
