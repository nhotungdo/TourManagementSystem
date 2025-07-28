using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class InvoiceService
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;

        public InvoiceService(TourManagementContext context, ActivityLogService activityLogService)
        {
            _context = context;
            _activityLogService = activityLogService;
        }

        public List<Invoice> GetAllInvoices()
        {
            return _context.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                .Include(i => i.CreatedByNavigation)
                .OrderByDescending(i => i.IssueDate)
                .ToList();
        }

        public List<Invoice> GetUserInvoices(int customerId)
        {
            return _context.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                .Where(i => i.Booking.CustomerId == customerId)
                .OrderByDescending(i => i.IssueDate)
                .ToList();
        }

        public Invoice? GetInvoiceById(int invoiceId)
        {
            return _context.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                .Include(i => i.CreatedByNavigation)
                .FirstOrDefault(i => i.InvoiceId == invoiceId);
        }

        public bool CreateInvoiceForBooking(int bookingId)
        {
            try
            {
                var booking = _context.Bookings
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null) return false;

                var invoice = new Invoice
                {
                    BookingId = bookingId,
                    InvoiceNumber = GenerateInvoiceNumber(),
                    IssueDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(7), // 7 days to pay
                    TotalAmount = booking.TotalPrice,
                    DiscountAmount = booking.DiscountAmount ?? 0,
                    TaxAmount = CalculateTax(booking.TotalPrice),
                    FinalAmount = booking.FinalPrice ?? booking.TotalPrice,
                    Status = "draft",
                    CreatedBy = booking.CustomerId
                };

                _context.Invoices.Add(invoice);
                _context.SaveChanges();

                _activityLogService.LogActivity(booking.CustomerId, "CREATE", "Invoice", invoice.InvoiceId,
                    $"Created invoice {invoice.InvoiceNumber} for booking {bookingId}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateInvoiceStatus(int invoiceId, string newStatus, int updatedBy)
        {
            try
            {
                var invoice = _context.Invoices.Find(invoiceId);
                if (invoice == null) return false;

                var oldStatus = invoice.Status;
                invoice.Status = newStatus;
                _context.SaveChanges();

                _activityLogService.LogActivity(updatedBy, "STATUS_UPDATE", "Invoice", invoiceId,
                    $"Changed invoice status from {oldStatus} to {newStatus}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IssueInvoice(int invoiceId, int issuedBy)
        {
            try
            {
                var invoice = _context.Invoices.Find(invoiceId);
                if (invoice == null) return false;

                invoice.Status = "issued";
                invoice.IssueDate = DateTime.Now;
                _context.SaveChanges();

                _activityLogService.LogActivity(issuedBy, "ISSUE", "Invoice", invoiceId,
                    $"Issued invoice {invoice.InvoiceNumber}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool MarkInvoiceAsPaid(int invoiceId, int updatedBy)
        {
            try
            {
                var invoice = _context.Invoices.Find(invoiceId);
                if (invoice == null) return false;

                invoice.Status = "paid";
                _context.SaveChanges();

                _activityLogService.LogActivity(updatedBy, "PAYMENT", "Invoice", invoiceId,
                    $"Marked invoice {invoice.InvoiceNumber} as paid");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Invoice> GetInvoicesByStatus(string status)
        {
            return _context.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(i => i.CreatedByNavigation)
                .Where(i => i.Status == status)
                .OrderByDescending(i => i.IssueDate)
                .ToList();
        }

        public List<Invoice> GetOverdueInvoices()
        {
            return _context.Invoices
                .Include(i => i.Booking)
                    .ThenInclude(b => b.Customer)
                .Where(i => i.Status == "issued" && i.DueDate < DateTime.Now)
                .OrderBy(i => i.DueDate)
                .ToList();
        }

        public decimal GetTotalRevenue(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Invoices.Where(i => i.Status == "paid");

            if (fromDate.HasValue)
                query = query.Where(i => i.IssueDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(i => i.IssueDate <= toDate.Value);

            return query.Sum(i => i.TotalAmount);
        }

        public decimal GetOutstandingAmount()
        {
            return _context.Invoices
                .Where(i => i.Status == "issued" && i.DueDate >= DateTime.Now)
                .Sum(i => i.TotalAmount);
        }

        private string GenerateInvoiceNumber()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month.ToString("D2");
            var count = _context.Invoices.Count(i => i.IssueDate.HasValue && i.IssueDate.Value.Year == year && i.IssueDate.Value.Month == DateTime.Now.Month) + 1;
            return $"INV-{year}{month}-{count:D4}";
        }

        private decimal CalculateTax(decimal subtotal)
        {
            return subtotal * 0.1m; // 10% tax
        }
    }
}