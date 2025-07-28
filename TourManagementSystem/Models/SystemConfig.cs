using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class SystemConfig
{
    public int ConfigId { get; set; }

    public string ConfigKey { get; set; } = null!;

    public string ConfigValue { get; set; } = null!;

    public string? Description { get; set; }

    public string Category { get; set; } = null!;

    public bool? IsActive { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
