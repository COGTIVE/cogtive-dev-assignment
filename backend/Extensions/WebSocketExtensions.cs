using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Cogtive.DevAssignment.Api.Models;
using Microsoft.Extensions.Logging;

namespace Cogtive.DevAssignment.Api.Extensions;

public static class WebSocketExtensions
{
    public static async Task HandleWebSocketConnection(this WebSocket webSocket, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Conexão WebSocket estabelecida. Iniciando envio de dados...");
            
            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var productionData = GenerateProductionData();
                await SendProductionData(webSocket, productionData, logger);
                await Task.Delay(10000, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro na conexão WebSocket: {Message}", ex.Message);
        }
        finally
        {
            logger.LogInformation("Conexão WebSocket encerrada");
        }
    }

    private static ProductionData GenerateProductionData()
    {
        var random = new Random();
        var efficiency = 70m + (decimal)(random.NextDouble() * 30);
        var machineId = random.Next(1, 4); // Garante que o machineId seja sempre um número entre 1 e 3
        
        return new ProductionData
        {
            MachineId = machineId,
            Timestamp = DateTime.UtcNow,
            Efficiency = Math.Round(efficiency, 1),
            UnitsProduced = random.Next(100, 600),
            Downtime = random.Next(0, 60)
        };
    }

    private static async Task SendProductionData(WebSocket webSocket, ProductionData data, ILogger logger)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var bytes = Encoding.UTF8.GetBytes(json);
            
            await webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            logger.LogInformation(
                "Dados enviados via WebSocket - Máquina: {MachineId}, Eficiência: {Efficiency}%, " +
                "Unidades: {UnitsProduced}, Downtime: {Downtime}min",
                data.MachineId,
                data.Efficiency,
                data.UnitsProduced,
                data.Downtime);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao enviar dados via WebSocket: {Message}", ex.Message);
        }
    }
} 