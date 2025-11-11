using Microsoft.EntityFrameworkCore;
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Infrastructure.Contexts;

public class RentalsDbContext(DbContextOptions<RentalsDbContext> options) : DbContext(options)
{
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureMotorcycle(modelBuilder);
    }

    private static void ConfigureMotorcycle(ModelBuilder modelBuilder)
    {
        var m = modelBuilder.Entity<Motorcycle>();

        m.ToTable("motorcycles");

        m.HasKey(x => x.Identifier);

        m.Property(x => x.Identifier)
            .HasColumnName("identifier")
            .HasMaxLength(50)
            .IsRequired();

        m.Property(x => x.Year)
            .HasColumnName("year")
            .IsRequired();

        m.Property(x => x.Model)
            .HasColumnName("model")
            .HasMaxLength(100)
            .IsRequired();

        m.Property(x => x.Plate)
            .HasColumnName("plate")
            .HasMaxLength(10)
            .IsRequired();

        m.HasIndex(x => x.Plate)
            .IsUnique();
    }
}