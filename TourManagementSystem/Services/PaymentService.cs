using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class PaymentService
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;

        public PaymentService(TourManagementContext context, ActivityLogService activityLogService)
        {
            _context = context;
            _activityLogService = activityLogService;
        }

        public List<Payment> GetAllPayments()
        {
            return _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                .Include(p => p.ProcessedByNavigation)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        public List<Payment> GetPaymentsByBooking(int bookingId)
        {
            return _context.Payments
                .Include(p => p.ProcessedByNavigation)
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        public Payment? GetPaymentById(int paymentId)
        {
            return _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                .Include(p => p.ProcessedByNavigation)
                .FirstOrDefault(p => p.PaymentId == paymentId);
        }

        public bool CreatePayment(Payment payment)
        {
            try
            {
                payment.PaymentDate = DateTime.Now;
                payment.Status = "pending";
                _context.Payments.Add(payment);
                _context.SaveChanges();

                // Log activity only if ProcessedBy is not null
                if (payment.ProcessedBy.HasValue)
                {
                    _activityLogService.LogActivity(payment.ProcessedBy.Value, "CREATE", "Payment", payment.PaymentId,
                        $"Created payment of {payment.Amount:C} for booking {payment.BookingId}");
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdatePaymentStatus(int paymentId, string newStatus, int updatedBy)
        {
            try
            {
                var payment = _context.Payments.Find(paymentId);
                if (payment == null) return false;

                var oldStatus = payment.Status;
                payment.Status = newStatus;
                _context.SaveChanges();

                _activityLogService.LogActivity(updatedBy, "STATUS_UPDATE", "Payment", paymentId,
                    $"Changed payment status from {oldStatus} to {newStatus}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessRefund(int paymentId, decimal refundAmount, int processedBy)
        {
            try
            {
                var payment = _context.Payments.Find(paymentId);
                if (payment == null) return false;

                // Create refund payment record
                var refundPayment = new Payment
                {
                    BookingId = payment.BookingId,
                    Amount = -refundAmount, // Negative amount for refund
                    PaymentDate = DateTime.Now,
                    PaymentMethod = payment.PaymentMethod,
                    Status = "refunded",
                    ProcessedBy = processedBy
                };

                _context.Payments.Add(refundPayment);
                _context.SaveChanges();

                _activityLogService.LogActivity(processedBy, "REFUND", "Payment", paymentId,
                    $"Processed refund of {refundAmount:C}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Payment> GetPaymentsByStatus(string status)
        {
            return _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(p => p.ProcessedByNavigation)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        public List<Payment> GetPaymentsByMethod(string paymentMethod)
        {
            return _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(p => p.ProcessedByNavigation)
                .Where(p => p.PaymentMethod == paymentMethod)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();
        }

        public decimal GetTotalPayments(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Payments.Where(p => p.Status == "completed" && p.PaymentDate.HasValue);

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= toDate.Value);

            return query.Sum(p => p.Amount);
        }

        public decimal GetTotalRefunds(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Payments.Where(p => p.Status == "refunded" && p.PaymentDate.HasValue);

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= toDate.Value);

            return Math.Abs(query.Sum(p => p.Amount));
        }
    }
}