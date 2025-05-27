namespace Cogtive.DevAssignment.Api.Models;

public class ProductionData
{
    public int Id { get; set; }
    public int MachineId { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Efficiency { get; set; }
    public int UnitsProduced { get; set; }
    public int Downtime { get; set; } // In minutes
}