namespace Cogtive.DevAssignment.Api.Data;

using Microsoft.EntityFrameworkCore;
using Cogtive.DevAssignment.Api.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Machine> Machines { get; set; } = null!;
    public DbSet<ProductionData> ProductionData { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductionData>()
            .HasIndex(p => p.MachineId);

        // Configure relationship
        modelBuilder.Entity<ProductionData>()
            .HasOne<Machine>()
            .WithMany(m => m.ProductionData)
            .HasForeignKey(p => p.MachineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}