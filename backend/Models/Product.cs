namespace Cogtive.DevAssignment.Api.Models;

public class Machine
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class ProductionData
{
    public int Id { get; set; }
    public int MachineId { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Efficiency { get; set; }
    public int UnitsProduced { get; set; }
    public int Downtime { get; set; } // In minutes
}