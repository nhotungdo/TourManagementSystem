using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string PromotionName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal DiscountPercentage { get; set; }

    public decimal? DiscountAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string PromotionCode { get; set; } = null!;

    public int? MaxUsage { get; set; }

    public int? CurrentUsage { get; set; }

    public decimal? MinBookingAmount { get; set; }

    public bool? IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<TourPromotion> TourPromotions { get; set; } = new List<TourPromotion>();
}
