using Microsoft.EntityFrameworkCore;

namespace RENAPER.Dominio.Data;

public class RenaperDbContext : DbContext
{
    public RenaperDbContext(DbContextOptions<RenaperDbContext> options) : base(options)
    {
    }

    public DbSet<Persona> Personas { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CUIL).IsUnique();
            entity.HasIndex(e => e.DNI).IsUnique();
        });

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.HasIndex(e => e.Mail).IsUnique();
        });
    }
}

