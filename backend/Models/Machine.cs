using System.ComponentModel.DataAnnotations;

namespace Cogtive.DevAssignment.Api.Models;

public class Machine
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    public DateTime InstallationDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }

    public virtual ICollection<ProductionData> ProductionData { get; set; } = new List<ProductionData>();
} 