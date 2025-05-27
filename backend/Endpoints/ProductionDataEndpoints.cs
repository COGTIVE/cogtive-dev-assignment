using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Cogtive.DevAssignment.Api.Models;
using Cogtive.DevAssignment.Api.Data;

namespace Cogtive.DevAssignment.Api.Endpoints;

public static class ProductionDataEndpoints
{
    public static void MapProductionDataEndpoints(this WebApplication app)
    {
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
    }
} 