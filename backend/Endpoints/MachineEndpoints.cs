using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Cogtive.DevAssignment.Api.Models;
using Cogtive.DevAssignment.Api.Data;

namespace Cogtive.DevAssignment.Api.Endpoints;

public static class MachineEndpoints
{
    public static void MapMachineEndpoints(this WebApplication app)
    {
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
    }
} 