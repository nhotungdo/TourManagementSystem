using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? UserId { get; set; }

    public string TargetRole { get; set; } = null!;

    public bool? IsRead { get; set; }

    public bool? IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? User { get; set; }
}
