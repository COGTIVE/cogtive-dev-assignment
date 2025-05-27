using System;

namespace CogtiveDevAssignment.Models;

public class Machine
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SerialNumber { get; set; }
    public string Type { get; set; }
    public bool IsActive { get; set; }
}
