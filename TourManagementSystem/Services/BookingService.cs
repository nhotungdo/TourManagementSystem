using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class BookingService
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;
        private readonly PaymentService _paymentService;
        private readonly InvoiceService _invoiceService;

        public BookingService(TourManagementContext context, ActivityLogService activityLogService,
            PaymentService paymentService, InvoiceService invoiceService)
        {
            _context = context;
            _activityLogService = activityLogService;
            _paymentService = paymentService;
            _invoiceService = invoiceService;
        }

        public List<Booking> GetAllBookings()
        {
            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Tour)
                .Include(b => b.Promotion)
                .Include(b => b.Payments)
                .Include(b => b.Invoices)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        public List<Booking> GetUserBookings(int customerId)
        {
            return _context.Bookings
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Tour)
                .Include(b => b.Promotion)
                .Include(b => b.Payments)
                .Include(b => b.Invoices)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        public Booking? GetBookingById(int bookingId)
        {
            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Tour)
                .Include(b => b.Promotion)
                .Include(b => b.Payments)
                .Include(b => b.Invoices)
                .Include(b => b.Reviews)
                .FirstOrDefault(b => b.BookingId == bookingId);
        }

        public bool CreateBooking(Booking booking)
        {
            try
            {
                // Validate schedule availability
                var schedule = _context.TourSchedules
                    .Include(s => s.Tour)
                    .FirstOrDefault(s => s.ScheduleId == booking.ScheduleId);

                if (schedule == null) return false;

                // Calculate total price
                booking.TotalPrice = CalculateTotalPrice(schedule.Tour.BasePrice, booking.NumAdults, booking.NumChildren ?? 0);

                // Apply promotion if exists
                if (booking.PromotionId.HasValue)
                {
                    var promotion = _context.Promotions.Find(booking.PromotionId.Value);
                    if (promotion != null && promotion.IsActive == true)
                    {
                        booking.DiscountAmount = booking.TotalPrice * (promotion.DiscountPercentage / 100m);
                        booking.FinalPrice = booking.TotalPrice - booking.DiscountAmount;
                    }
                }
                else
                {
                    booking.FinalPrice = booking.TotalPrice;
                }

                booking.BookingDate = DateTime.Now;
                booking.Status = "pending";
                booking.PaymentStatus = "unpaid";

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                // Create invoice
                _invoiceService.CreateInvoiceForBooking(booking.BookingId);

                _activityLogService.LogActivity(booking.CustomerId, "CREATE", "Booking", booking.BookingId,
                    $"Created booking for tour: {schedule.Tour.TourName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateBookingStatus(int bookingId, string newStatus, int updatedBy)
        {
            try
            {
                var booking = _context.Bookings.Find(bookingId);
                if (booking == null) return false;

                var oldStatus = booking.Status;
                booking.Status = newStatus;
                _context.SaveChanges();

                _activityLogService.LogActivity(updatedBy, "STATUS_UPDATE", "Booking", bookingId,
                    $"Changed booking status from {oldStatus} to {newStatus}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CancelBooking(int bookingId, int customerId)
        {
            try
            {
                var booking = _context.Bookings
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                    .FirstOrDefault(b => b.BookingId == bookingId && b.CustomerId == customerId);

                if (booking == null) return false;

                booking.Status = "cancelled";
                _context.SaveChanges();

                _activityLogService.LogActivity(customerId, "CANCEL", "Booking", bookingId,
                    $"Cancelled booking for tour: {booking.Schedule.Tour.TourName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessPayment(int bookingId, decimal amount, string paymentMethod, int processedBy)
        {
            try
            {
                var booking = _context.Bookings.Find(bookingId);
                if (booking == null) return false;

                // Create payment record
                var payment = new Payment
                {
                    BookingId = bookingId,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = paymentMethod,
                    Status = "completed",
                    ProcessedBy = processedBy
                };

                _context.Payments.Add(payment);

                // Update booking payment status
                var totalPaid = _context.Payments
                    .Where(p => p.BookingId == bookingId && p.Status == "completed")
                    .Sum(p => p.Amount);

                if (totalPaid >= booking.FinalPrice)
                {
                    booking.PaymentStatus = "paid";
                    booking.Status = "confirmed";
                }
                else if (totalPaid > 0)
                {
                    booking.PaymentStatus = "partial";
                }

                _context.SaveChanges();

                _activityLogService.LogActivity(processedBy, "PAYMENT", "Booking", bookingId,
                    $"Processed payment of {amount:C} for booking");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Booking> GetBookingsByStatus(string status)
        {
            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Tour)
                .Where(b => b.Status == status)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        public List<Booking> GetBookingsByPaymentStatus(string paymentStatus)
        {
            return _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Tour)
                .Where(b => b.PaymentStatus == paymentStatus)
                .OrderByDescending(b => b.BookingDate)
                .ToList();
        }

        public decimal GetTotalRevenue(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Bookings.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(b => b.BookingDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(b => b.BookingDate <= toDate.Value);

            return query.Sum(b => b.FinalPrice ?? b.TotalPrice);
        }

        public bool DeleteBooking(int bookingId, int deletedBy)
        {
            try
            {
                var booking = _context.Bookings
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null) return false;

                // Check if booking can be deleted (not confirmed or completed)
                if (booking.Status == "confirmed" || booking.Status == "completed")
                {
                    return false; // Cannot delete confirmed or completed bookings
                }

                _context.Bookings.Remove(booking);
                _context.SaveChanges();

                _activityLogService.LogActivity(deletedBy, "DELETE", "Booking", bookingId,
                    $"Deleted booking for tour: {booking.Schedule.Tour.TourName}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private decimal CalculateTotalPrice(decimal basePrice, int numAdults, int numChildren)
        {
            return (basePrice * numAdults) + (basePrice * 0.7m * numChildren); // Children get 30% discount
        }
    }
}