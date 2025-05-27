using System;
using Microsoft.EntityFrameworkCore;
using Cogtive.DevAssignment.Api.Data;
using Cogtive.DevAssignment.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
var connectionString = dbProvider == "Postgres" 
    ? builder.Configuration.GetConnectionString("PostgresConnection")
    : builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (dbProvider == "Postgres")
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp",
        builder => builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowWebApp");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    InitSeedData(context);
}

// Define API endpoints
app.MapGet("/api/machines", async (AppDbContext context) =>
    await context.Machines.ToListAsync())
    .WithName("GetMachines")
    .Produces<List<Machine>>(StatusCodes.Status200OK);

app.MapGet("/api/machines/{id}", async (int id, AppDbContext context) =>
{
    var machine = await context.Machines.FindAsync(id);
    return machine is null ? Results.NotFound() : Results.Ok(machine);
})
.WithName("GetMachine")
.Produces<Machine>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapGet("/api/production-data", async (AppDbContext context) =>
    await context.ProductionData.ToListAsync())
    .WithName("GetProductionData")
    .Produces<List<ProductionData>>(StatusCodes.Status200OK);

app.MapGet("/api/machines/{id}/production-data", async (int id, AppDbContext context) =>
{
    var data = await context.ProductionData
        .Where(p => p.MachineId == id)
        .OrderByDescending(p => p.Timestamp)
        .ToListAsync();

    return data.Count == 0 ? Results.NotFound() : Results.Ok(data);
})
.WithName("GetMachineProductionData")
.Produces<List<ProductionData>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/production-data", async (ProductionData data, AppDbContext context) =>
{
    context.ProductionData.Add(data);
    await context.SaveChangesAsync();
    return Results.Created($"/api/production-data/{data.Id}", data);
})
.WithName("AddProductionData")
.Produces<ProductionData>(StatusCodes.Status201Created);

app.Run();

// Helper method to seed initial data
void InitSeedData(AppDbContext context)
{
    if (context.Machines.Any()) return;

    // Seed machines
    context.Machines.AddRange(
        new Machine { 
            Id = 1, 
            Name = "CNC Machine Alpha", 
            SerialNumber = "CNC-2023-001", 
            Type = "CNC", 
            IsActive = true 
        },
        new Machine { 
            Id = 2, 
            Name = "Injection Molder Beta", 
            SerialNumber = "INJ-2022-042", 
            Type = "Injection", 
            IsActive = true 
        },
        new Machine { 
            Id = 3, 
            Name = "Assembly Line Gamma", 
            SerialNumber = "ASM-2021-007", 
            Type = "Assembly", 
            IsActive = false 
        }
    );
    context.SaveChanges();
    
    // Seed production data
    var now = DateTime.UtcNow;
    context.ProductionData.AddRange(
        new ProductionData { 
            MachineId = 1, 
            Timestamp = now.AddHours(-4), 
            Efficiency = 92.7m,
            UnitsProduced = 427, 
            Downtime = 24 
        },
        new ProductionData { 
            MachineId = 2, 
            Timestamp = now.AddHours(-3), 
            Efficiency = 88.3m, 
            UnitsProduced = 195, 
            Downtime = 32 
        },
        new ProductionData { 
            MachineId = 1, 
            Timestamp = now.AddHours(-2), 
            Efficiency = 95.1m, 
            UnitsProduced = 512, 
            Downtime = 15 
        }
    );
    context.SaveChanges();
}