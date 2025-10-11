using Microsoft.EntityFrameworkCore;

namespace component.template.api.domain.Models.Repository.Contexts;

public class CosmosContext : DbContext
{
    // public DbSet<UserRepository> User { get; set; }

    public CosmosContext(DbContextOptions<CosmosContext> options) : base(options)
    {
    }

    public CosmosContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.Entity<UserRepository>().HasNoDiscriminator().ToContainer("User").Property(p => p.Id).ToJsonProperty("id");
    }
}
