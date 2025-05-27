using System;

namespace CogtiveDevAssignment.Models;

public class Machine
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string SerialNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime InstallationDate { get; set; }
    public string? Description { get; set; }
}
