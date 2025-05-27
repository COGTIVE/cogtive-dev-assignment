using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CogtiveDevAssignment.Models;

namespace CogtiveDevAssignment.Services;

public interface ILocalStorageService
{
    Task SavePendingProductionDataAsync(ProductionData data);
    Task<List<ProductionData>> GetPendingProductionDataAsync();
    Task ClearPendingProductionDataAsync();
    Task<int> GetPendingProductionDataCountAsync();
    bool IsOnline();
}

public class LocalStorageService : ILocalStorageService
{
    private const string PendingDataKey = "pending_production_data";
    private readonly string _localStoragePath;
    
    public LocalStorageService()
    {
        _localStoragePath = Path.Combine(FileSystem.AppDataDirectory, "offlineData.json");
    }
    
    public async Task SavePendingProductionDataAsync(ProductionData data)
    {
        var pendingData = await GetPendingProductionDataAsync();
        pendingData.Add(data);
        
        var json = JsonSerializer.Serialize(pendingData);
        await SecureStorage.SetAsync(PendingDataKey, json);
    }
    
    public async Task<List<ProductionData>> GetPendingProductionDataAsync()
    {
        try
        {
            var json = await SecureStorage.GetAsync(PendingDataKey);
            
            if (string.IsNullOrEmpty(json))
            {
                return new List<ProductionData>();
            }
            
            return JsonSerializer.Deserialize<List<ProductionData>>(json) ?? new List<ProductionData>();
        }
        catch
        {
            return new List<ProductionData>();
        }
    }
    
    public async Task ClearPendingProductionDataAsync()
    {
        await SecureStorage.SetAsync(PendingDataKey, JsonSerializer.Serialize(new List<ProductionData>()));
    }
    
    public async Task<int> GetPendingProductionDataCountAsync()
    {
        var pendingData = await GetPendingProductionDataAsync();
        return pendingData.Count;
    }
    
    // Method to check if we're online
    public bool IsOnline()
    {
        return Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
