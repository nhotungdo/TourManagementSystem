using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int CustomerId { get; set; }

    public int ScheduleId { get; set; }

    public DateTime? BookingDate { get; set; }

    public int NumAdults { get; set; }

    public int? NumChildren { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Status { get; set; }

    public string? PaymentStatus { get; set; }

    public string? Notes { get; set; }

    public int? PromotionId { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? FinalPrice { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Promotion? Promotion { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual TourSchedule Schedule { get; set; } = null!;
}
