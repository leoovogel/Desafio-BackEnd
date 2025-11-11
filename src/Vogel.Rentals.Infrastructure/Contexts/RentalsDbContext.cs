using Microsoft.EntityFrameworkCore;
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Infrastructure.Contexts;

public class RentalsDbContext(DbContextOptions<RentalsDbContext> options) : DbContext(options)
{
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
    public DbSet<Courier> Couriers => Set<Courier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureMotorcycle(modelBuilder);
        ConfigureCourier(modelBuilder);
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
    
    private static void ConfigureCourier(ModelBuilder modelBuilder)
    {
        var c = modelBuilder.Entity<Courier>();

        c.ToTable("couriers");

        c.HasKey(x => x.Identifier);

        c.Property(x => x.Identifier)
            .HasColumnName("identifier")
            .HasMaxLength(50)
            .IsRequired();

        c.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        c.Property(x => x.Cnpj)
            .HasColumnName("cnpj")
            .HasMaxLength(14)
            .IsRequired();

        c.Property(x => x.BirthDate)
            .HasColumnName("birth_date")
            .IsRequired();

        c.Property(x => x.CnhNumber)
            .HasColumnName("cnh_number")
            .HasMaxLength(20)
            .IsRequired();

        c.Property(x => x.CnhType)
            .HasColumnName("cnh_type")
            .IsRequired();

        c.Property(x => x.CnhImage)
            .HasColumnName("cnh_image");

        c.HasIndex(x => x.Cnpj)
            .IsUnique();

        c.HasIndex(x => x.CnhNumber)
            .IsUnique();
    }
}