using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Cogtive.DevAssignment.Api.Models;
using Cogtive.DevAssignment.Api.Data;
using Cogtive.DevAssignment.Api.Endpoints;

namespace Cogtive.DevAssignment.Api;

public static class ApiEndpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapMachineEndpoints();
        app.MapProductionDataEndpoints();
    }
} 