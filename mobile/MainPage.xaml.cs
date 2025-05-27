using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using CogtiveDevAssignment.Models;
using CogtiveDevAssignment.Services;

namespace CogtiveDevAssignment
{
    public partial class MainPage : ContentPage
    {
        private const string ApiBaseUrl = "http://10.0.2.2:5000"; // Special IP for Android emulator to reach host
        private readonly HttpClient _httpClient;
        private readonly IApiService _apiService;
        private readonly ILocalStorageService _localStorageService;
        private List<Machine> _machines;
        private Machine _selectedMachine;

        public MainPage(IApiService apiService, ILocalStorageService localStorageService)
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _apiService = apiService;
            _localStorageService = localStorageService;
            Task.Run(async () => await LoadMachines());
            Task.Run(async () => await UpdateSyncStatus());
        }

        private async Task LoadMachines()
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                if (IsOnline())
                {
                    _machines = await FetchMachines();
                }
                else
                {
                    // Fallback for offline mode
                    _machines = new List<Machine>()
                    {
                        new Machine { Id = 1, Name = "CNC Machine Alpha (Offline Cache)", SerialNumber = "CNC-2023-001", Type = "CNC", IsActive = true },
                        new Machine { Id = 2, Name = "Injection Molder Beta (Offline Cache)", SerialNumber = "INJ-2022-042", Type = "Injection", IsActive = true },
                    };

                    await DisplayAlert("Offline Mode", "You are currently working offline. Data will be synchronized when a connection is available.", "OK");
                }

                // Setup the picker
                var machineNames = new List<string>();
                foreach (var machine in _machines)
                {
                    machineNames.Add(machine.Name);
                }

                MachinePicker.ItemsSource = machineNames;
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
            catch (Exception ex)
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                ErrorMessage.Text = $"Error loading machines: {ex.Message}";
                ErrorMessage.IsVisible = true;
            }
        }

        private async Task<List<Machine>> FetchMachines()
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/api/machines");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Machine>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private async Task<List<ProductionData>> FetchMachineProductionData(int machineId)
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/api/machines/{machineId}/production-data");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductionData>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private async Task<ProductionData> PostProductionData(ProductionData data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{ApiBaseUrl}/api/production-data", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductionData>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private void OnMachineSelected(object sender, EventArgs e)
        {
            int selectedIndex = MachinePicker.SelectedIndex;
            if (selectedIndex == -1) return;

            _selectedMachine = _machines[selectedIndex];

            // Update the machine status section
            MachineName.Text = _selectedMachine.Name;
            MachineSerial.Text = _selectedMachine.SerialNumber;
            MachineType.Text = _selectedMachine.Type;
            MachineStatus.Text = _selectedMachine.IsActive ? "Active" : "Inactive";

            // Show the machine status and data entry sections
            MachineStatusFrame.IsVisible = true;
            DataEntryFrame.IsVisible = _selectedMachine.IsActive; // Only allow data entry for active machines
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            if (_selectedMachine == null) return;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(EfficiencyEntry.Text) ||
                string.IsNullOrWhiteSpace(UnitsProducedEntry.Text) ||
                string.IsNullOrWhiteSpace(DowntimeEntry.Text))
            {
                await DisplayAlert("Validation Error", "All fields are required", "OK");
                return;
            }

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                // Create production data object
                var productionData = new ProductionData
                {
                    MachineId = _selectedMachine.Id,
                    Timestamp = DateTime.UtcNow,
                    Efficiency = decimal.Parse(EfficiencyEntry.Text),
                    UnitsProduced = int.Parse(UnitsProducedEntry.Text),
                    Downtime = int.Parse(DowntimeEntry.Text)
                };

                if (IsOnline())
                {
                    // Send directly to API
                    await PostProductionData(productionData);
                    await DisplayAlert("Success", "Production data submitted successfully", "OK");
                }
                else
                {
                    // For offline mode, we'd store locally
                    // Simple simulation for this challenge
                    int pendingCount = await GetPendingDataCount();
                    await SavePendingData(pendingCount + 1);
                    await DisplayAlert("Offline Mode", "Data saved for later synchronization", "OK");
                }

                // Clear form
                EfficiencyEntry.Text = string.Empty;
                UnitsProducedEntry.Text = string.Empty;
                DowntimeEntry.Text = string.Empty;

                await UpdateSyncStatus();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to submit data: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async Task UpdateSyncStatus()
        {
            int pendingCount = await GetPendingDataCount();
            PendingUploadsLabel.Text = pendingCount.ToString();
            SyncButton.IsEnabled = pendingCount > 0 && IsOnline();
        }

        private async void OnSyncClicked(object sender, EventArgs e)
        {
            if (!IsOnline())
            {
                await DisplayAlert("Offline", "Cannot sync while offline", "OK");
                return;
            }

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                int pendingCount = await GetPendingDataCount();
                
                // In a real app, we'd retrieve and sync the stored data
                // For this challenge, we'll just simulate it
                await SavePendingData(0);
                await UpdateSyncStatus();

                await DisplayAlert("Sync Complete", $"Successfully synchronized {pendingCount} records", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Sync Error", $"Failed to synchronize: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async void OnScanQRClicked(object sender, EventArgs e)
        {
            // In a real app, this would launch the scanner
            // For this challenge, we'll simulate scanning a QR code
            await DisplayAlert("QR Scanner", "This is a simulated QR code scanner. In a full implementation, this would open the device camera to scan equipment QR codes.", "OK");
        }

        // Helper methods for offline mode simulation
        private bool IsOnline()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
        }

        private async Task<int> GetPendingDataCount()
        {
            string pendingDataStr = await SecureStorage.GetAsync("pending_data_count");
            if (int.TryParse(pendingDataStr, out int count))
            {
                return count;
            }
            return 0;
        }

        private async Task SavePendingData(int count)
        {
            await SecureStorage.SetAsync("pending_data_count", count.ToString());
        }
    }
}