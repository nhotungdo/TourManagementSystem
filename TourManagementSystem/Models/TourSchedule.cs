using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class TourSchedule
{
    public int ScheduleId { get; set; }

    public int TourId { get; set; }

    public DateOnly DepartureDate { get; set; }

    public DateOnly ReturnDate { get; set; }

    public int MaxCapacity { get; set; }

    public int? CurrentBookings { get; set; }

    public decimal? PriceAdjustment { get; set; }

    public int? GuideId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User? Guide { get; set; }

    public virtual Tour Tour { get; set; } = null!;
}
