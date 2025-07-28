using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class TourScheduleService
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;

        public TourScheduleService(TourManagementContext context, ActivityLogService activityLogService)
        {
            _context = context;
            _activityLogService = activityLogService;
        }

        public List<TourSchedule> GetAllSchedules()
        {
            return _context.TourSchedules
                .Include(s => s.Tour)
                .Include(s => s.Guide)
                .OrderByDescending(s => s.DepartureDate)
                .ToList();
        }

        public List<TourSchedule> GetSchedulesByTour(int tourId)
        {
            return _context.TourSchedules
                .Include(s => s.Tour)
                .Include(s => s.Guide)
                .Where(s => s.TourId == tourId)
                .OrderBy(s => s.DepartureDate)
                .ToList();
        }

        public List<TourSchedule> GetActiveSchedules()
        {
            var now = DateOnly.FromDateTime(DateTime.Now);
            return _context.TourSchedules
                .Include(s => s.Tour)
                .Include(s => s.Guide)
                .Where(s => s.DepartureDate > now && s.Status == "scheduled")
                .OrderBy(s => s.DepartureDate)
                .ToList();
        }

        public TourSchedule? GetScheduleById(int scheduleId)
        {
            return _context.TourSchedules
                .Include(s => s.Tour)
                .Include(s => s.Guide)
                .FirstOrDefault(s => s.ScheduleId == scheduleId);
        }

        public bool CreateSchedule(TourSchedule schedule, int createdByUserId)
        {
            try
            {
                schedule.Status = "scheduled";
                schedule.CurrentBookings = 0;

                _context.TourSchedules.Add(schedule);
                _context.SaveChanges();

                _activityLogService.LogActivity(createdByUserId, "CREATE", "SCHEDULE", schedule.ScheduleId,
                    $"Created schedule for tour '{schedule.Tour?.TourName}' on {schedule.DepartureDate:dd/MM/yyyy}");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateSchedule(TourSchedule schedule, int updatedByUserId)
        {
            try
            {
                var existingSchedule = _context.TourSchedules.Find(schedule.ScheduleId);
                if (existingSchedule == null) return false;

                existingSchedule.DepartureDate = schedule.DepartureDate;
                existingSchedule.ReturnDate = schedule.ReturnDate;
                existingSchedule.GuideId = schedule.GuideId;
                existingSchedule.MaxCapacity = schedule.MaxCapacity;
                existingSchedule.Status = schedule.Status;

                _context.SaveChanges();

                _activityLogService.LogActivity(updatedByUserId, "UPDATE", "SCHEDULE", schedule.ScheduleId,
                    $"Updated schedule for tour '{existingSchedule.Tour?.TourName}' on {existingSchedule.DepartureDate:dd/MM/yyyy}");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteSchedule(int scheduleId, int deletedByUserId)
        {
            try
            {
                var schedule = _context.TourSchedules
                    .Include(s => s.Tour)
                    .FirstOrDefault(s => s.ScheduleId == scheduleId);

                if (schedule == null)
                {
                    throw new InvalidOperationException("Schedule not found");
                }

                // Check if there are any bookings for this schedule
                var hasBookings = _context.Bookings.Any(b => b.ScheduleId == scheduleId);
                if (hasBookings)
                {
                    // For now, allow deletion but log a warning
                    // In production, you might want to prevent deletion or require confirmation
                    // throw new InvalidOperationException("Cannot delete schedule with existing bookings");

                    // Delete related bookings first
                    var relatedBookings = _context.Bookings.Where(b => b.ScheduleId == scheduleId).ToList();
                    _context.Bookings.RemoveRange(relatedBookings);
                }

                _context.TourSchedules.Remove(schedule);
                _context.SaveChanges();

                _activityLogService.LogActivity(deletedByUserId, "DELETE", "SCHEDULE", scheduleId,
                    $"Deleted schedule for tour '{schedule.Tour?.TourName}' on {schedule.DepartureDate:dd/MM/yyyy}");

                return true;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw InvalidOperationException
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete schedule: {ex.Message}");
            }
        }

        public bool UpdateScheduleStatus(int scheduleId, string status, int updatedByUserId)
        {
            try
            {
                var schedule = _context.TourSchedules
                    .Include(s => s.Tour)
                    .FirstOrDefault(s => s.ScheduleId == scheduleId);

                if (schedule == null) return false;

                schedule.Status = status;

                _context.SaveChanges();

                _activityLogService.LogActivity(updatedByUserId, "STATUS_UPDATE", "SCHEDULE", scheduleId,
                    $"Updated schedule status to '{status}' for tour '{schedule.Tour?.TourName}'");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetAvailableSlots(int scheduleId)
        {
            var schedule = _context.TourSchedules.Find(scheduleId);
            if (schedule == null) return 0;

            return schedule.MaxCapacity - (schedule.CurrentBookings ?? 0);
        }

        public bool IncrementBookings(int scheduleId)
        {
            try
            {
                var schedule = _context.TourSchedules.Find(scheduleId);
                if (schedule == null) return false;

                if (schedule.CurrentBookings >= schedule.MaxCapacity)
                    return false;

                schedule.CurrentBookings = (schedule.CurrentBookings ?? 0) + 1;
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DecrementBookings(int scheduleId)
        {
            try
            {
                var schedule = _context.TourSchedules.Find(scheduleId);
                if (schedule == null) return false;

                if (schedule.CurrentBookings <= 0)
                    return false;

                schedule.CurrentBookings = (schedule.CurrentBookings ?? 0) - 1;
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<TourSchedule> GetSchedulesByStatus(string status)
        {
            return _context.TourSchedules
                .Include(s => s.Tour)
                .Include(s => s.Guide)
                .Where(s => s.Status == status)
                .OrderBy(s => s.DepartureDate)
                .ToList();
        }

        public List<TourSchedule> GetSchedulesByDateRange(DateTime startDate, DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            return _context.TourSchedules
                .Include(s => s.Tour)
                .Include(s => s.Guide)
                .Where(s => s.DepartureDate >= startDateOnly && s.DepartureDate <= endDateOnly)
                .OrderBy(s => s.DepartureDate)
                .ToList();
        }
    }
}