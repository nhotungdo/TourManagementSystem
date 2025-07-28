using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int TourId { get; set; }

    public int CustomerId { get; set; }

    public int BookingId { get; set; }

    public byte Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewDate { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User Customer { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
