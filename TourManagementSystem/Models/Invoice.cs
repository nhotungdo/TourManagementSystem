using System;
using System.Collections.Generic;

namespace TourManagementSystem.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public int BookingId { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public string? Status { get; set; }

    public string? PaymentTerms { get; set; }

    public string? Notes { get; set; }

    public int CreatedBy { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;
}
