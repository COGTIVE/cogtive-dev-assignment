using System.Net.Http;
using System.Text;
using System.Text.Json;
using CogtiveDevAssignment.Models;

namespace CogtiveDevAssignment.Services;

public interface IApiService
{
    Task<List<Machine>> GetMachinesAsync();
    Task<Machine> GetMachineByIdAsync(int id);
    Task<List<ProductionData>> GetProductionDataAsync();
    Task<List<ProductionData>> GetMachineProductionDataAsync(int machineId);
    Task<ProductionData> PostProductionDataAsync(ProductionData data);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    
    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Special IP for Android emulator to access host machine
        _baseUrl = "http://10.0.2.2:5211/api";
        
        // For iOS simulator, uncomment this:
        // _baseUrl = "http://localhost:5000/api";
    }
    
    public async Task<List<Machine>> GetMachinesAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/machines");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Machine>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<Machine>();
    }
    
    public async Task<Machine> GetMachineByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/machines/{id}");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Machine>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new Machine();
    }
    
    public async Task<List<ProductionData>> GetProductionDataAsync()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/production-data");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ProductionData>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<ProductionData>();
    }
    
    public async Task<List<ProductionData>> GetMachineProductionDataAsync(int machineId)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/machines/{machineId}/production-data");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ProductionData>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<ProductionData>();
    }
    
    public async Task<ProductionData> PostProductionDataAsync(ProductionData data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{_baseUrl}/production-data", content);
        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ProductionData>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new ProductionData();
    }
}
