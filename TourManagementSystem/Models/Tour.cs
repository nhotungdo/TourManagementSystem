using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class Tour
{
    public int TourId { get; set; }

    public string TourName { get; set; } = null!;

    public string? Description { get; set; }

    public int DurationDays { get; set; }

    public int DurationNights { get; set; }

    public string DepartureLocation { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public decimal BasePrice { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<TourAttraction> TourAttractions { get; set; } = new List<TourAttraction>();

    public virtual ICollection<TourPromotion> TourPromotions { get; set; } = new List<TourPromotion>();

    public virtual ICollection<TourSchedule> TourSchedules { get; set; } = new List<TourSchedule>();
}
