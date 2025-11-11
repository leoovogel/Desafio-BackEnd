using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vogel.Rentals.Domain.Entities;

namespace Vogel.Rentals.Infrastructure.Contexts;

public class RentalsDbContext(DbContextOptions<RentalsDbContext> options) : DbContext(options)
{
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
    public DbSet<Courier> Couriers => Set<Courier>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<MotorcycleNotification> MotorcycleNotifications => Set<MotorcycleNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureMotorcycle(modelBuilder);
        ConfigureCourier(modelBuilder);
        ConfigureRental(modelBuilder);
        ConfigureMotorcycleNotification(modelBuilder);
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
    
    private static void ConfigureRental(ModelBuilder modelBuilder)
    {
        var r = modelBuilder.Entity<Rental>();
    
        r.ToTable("rentals");
    
        r.HasKey(x => x.Identifier);
    
        r.Property(x => x.Identifier)
            .HasColumnName("identifier")
            .IsRequired();
    
        r.Property(x => x.CourierId)
            .HasColumnName("courier_id")
            .HasMaxLength(50)
            .IsRequired();
    
        r.Property(x => x.MotorcycleId)
            .HasColumnName("motorcycle_id")
            .HasMaxLength(50)
            .IsRequired();
    
        r.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .IsRequired();
    
        r.Property(x => x.EndDate)
            .HasColumnName("end_date")
            .IsRequired();
    
        r.Property(x => x.ExpectedEndDate)
            .HasColumnName("expected_end_date")
            .IsRequired();
        
        r.Property(x => x.ReturnDate)
            .HasColumnName("return_date");
    
        r.Property(x => x.Plan)
            .HasColumnName("plan")
            .IsRequired();
    
        r.Property(x => x.DailyRate)
            .HasColumnName("daily_rate")
            .HasColumnType("decimal(10,2)")
            .IsRequired();
    
        r.HasOne(x => x.Courier)
            .WithMany()
            .HasForeignKey(rental => rental.CourierId)
            .HasPrincipalKey(courier => courier.Identifier);
        
        r.HasOne(x => x.Motorcycle)
            .WithMany(motorrcycle => motorrcycle.Rentals)
            .HasForeignKey(rental => rental.MotorcycleId)
            .HasPrincipalKey(motorcycle => motorcycle.Identifier);
    }
    
    private static void ConfigureMotorcycleNotification(ModelBuilder modelBuilder)
    {
        var mn = modelBuilder.Entity<MotorcycleNotification>();
        
        mn.ToTable("motorcycle_notifications");
        
        mn.HasKey(x => x.Identifier);

        mn.Property(x => x.Identifier)
            .HasColumnName("identifier")
            .HasMaxLength(50)
            .IsRequired();

        mn.Property(x => x.Year)
            .HasColumnName("year")
            .IsRequired();

        mn.Property(x => x.Model)
            .HasColumnName("model")
            .HasMaxLength(100)
            .IsRequired();

        mn.Property(x => x.Plate)
            .HasColumnName("plate")
            .HasMaxLength(10)
            .IsRequired();

        mn.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}
