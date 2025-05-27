using Microsoft.EntityFrameworkCore;
using Cogtive.DevAssignment.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;

namespace Cogtive.DevAssignment.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureWebSocket(this WebApplication app)
    {
        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromSeconds(120)
        });

        app.Map("/ws/production-data", async context =>
        {
            app.Logger.LogInformation("Nova requisição WebSocket recebida");
            
            if (context.WebSockets.IsWebSocketRequest)
            {
                app.Logger.LogInformation("Requisição WebSocket aceita");
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await webSocket.HandleWebSocketConnection(app.Logger, CancellationToken.None);
            }
            else
            {
                app.Logger.LogWarning("Requisição WebSocket rejeitada - Não é uma requisição WebSocket válida");
                context.Response.StatusCode = 400;
            }
        });
    }

    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var dbProvider = builder.Configuration["DatabaseProvider"] ?? "Postgres";
        var connectionString = dbProvider == "Postgres" 
            ? builder.Configuration.GetConnectionString("DefaultConnection")
            : builder.Configuration.GetConnectionString("SqliteConnection");

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
    }

    public static void ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowWebApp",
                builder => builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
    }
} 