using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace Catalog.API.Data;

public class MyDbContext: DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options): base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<EquipmentCategory> EquipmentCategories { get; set; }
    public DbSet<EquipmentType> EquipmentTypes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<InventoryBooking>  InventoryBookings { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>()
            .Property(r => r.Condition)
            .HasConversion<string>(); // 存储为字符串
        modelBuilder.Entity<Location>()
            .HasIndex(i=>i.Name)
            .IsUnique(); // unique constraint
        modelBuilder.Entity<Equipment>()
            .HasOne(e => e.LocationDetail)
            .WithMany()
            .HasForeignKey(e => e.Location) // Equipment.Location 是 FK
            .HasPrincipalKey(l => l.Name);  // Location.Name 是目标主键（不是 Id）
        modelBuilder.Entity<User>()
            .Property(u => u.Roles)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<UserRole>>(v, (JsonSerializerOptions)null));
    }
}

