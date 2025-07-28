using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class TourAttraction
{
    public int TourId { get; set; }

    public int AttractionId { get; set; }

    public int VisitDay { get; set; }

    public int VisitOrder { get; set; }

    public string? Description { get; set; }

    public virtual Attraction Attraction { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
