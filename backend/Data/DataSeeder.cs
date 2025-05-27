using Cogtive.DevAssignment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Cogtive.DevAssignment.Api.Data;

public static class DataSeeder
{
    public static async Task SeedData(AppDbContext context)
    {
        if (!context.Machines.Any())
        {
            // Seed machines
            context.Machines.AddRange(
            new Machine
            {
                Id = 1,
                Name = "CNC Machine Alpha",
                SerialNumber = "CNC-2023",
                InstallationDate = DateTime.UtcNow.AddYears(-1),
                Type = "CNC",
                IsActive = true,
                Description = "High precision CNC machine for metal parts"
            },
            new Machine
            {
                Id = 2,
                Name = "Injection Molder Beta",
                SerialNumber = "INJ-2022",
                Type = "Injection",
                InstallationDate = DateTime.UtcNow.AddMonths(-6),
                IsActive = true,
                Description = "Plastic injection molding machine"
            },
            new Machine
            {
                Id = 3,
                Name = "Assembly Line Gamma",
                SerialNumber = "ASM-2021",
                Type = "Assembly",
                InstallationDate = DateTime.UtcNow.AddYears(-2),
                IsActive = false,
                Description = "Automated assembly line for electronics"
            }
        );
            await context.SaveChangesAsync();

            // Seed production data
            var now = DateTime.UtcNow;
            context.ProductionData.AddRange(
                new ProductionData
                {
                    MachineId = 1,
                    Timestamp = now.AddHours(-4),
                    Efficiency = 92.7m,
                    UnitsProduced = 427,
                    Downtime = 24
                },
                new ProductionData
                {
                    MachineId = 2,
                    Timestamp = now.AddHours(-3),
                    Efficiency = 88.3m,
                    UnitsProduced = 195,
                    Downtime = 32
                },
                new ProductionData
                {
                    MachineId = 1,
                    Timestamp = now.AddHours(-2),
                    Efficiency = 95.1m,
                    UnitsProduced = 512,
                    Downtime = 15
                }
            );
            await context.SaveChangesAsync();
        }
    }
}