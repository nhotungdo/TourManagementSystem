using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class Attraction
{
    public int AttractionId { get; set; }

    public string AttractionName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<TourAttraction> TourAttractions { get; set; } = new List<TourAttraction>();
}
