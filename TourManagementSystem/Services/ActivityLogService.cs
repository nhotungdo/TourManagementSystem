using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class ActivityLogService
    {
        private readonly TourManagementContext _context;

        public ActivityLogService(TourManagementContext context)
        {
            _context = context;
        }

        public void LogActivity(int userId, string action, string entityType, int entityId, string description,
            string logLevel = "info", string ipAddress = "", string userAgent = "")
        {
            try
            {
                var log = new ActivityLog
                {
                    UserId = userId,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    Description = description,
                    LogLevel = logLevel,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    CreatedAt = DateTime.Now
                };

                _context.ActivityLogs.Add(log);
                _context.SaveChanges();
            }
            catch
            {
                // Log error silently to avoid breaking main functionality
            }
        }

        public List<ActivityLog> GetActivityLogs(int? userId = null, string? entityType = null,
            string? logLevel = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.ActivityLogs
                .Include(a => a.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(a => a.EntityType == entityType);

            if (!string.IsNullOrEmpty(logLevel))
                query = query.Where(a => a.LogLevel == logLevel);

            if (fromDate.HasValue)
                query = query.Where(a => a.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.CreatedAt <= toDate.Value);

            return query.OrderByDescending(a => a.CreatedAt).ToList();
        }

        public List<ActivityLog> GetUserActivityLogs(int userId, int page = 1, int pageSize = 20)
        {
            return _context.ActivityLogs
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<ActivityLog> GetRecentActivityLogs(int count = 50)
        {
            return _context.ActivityLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .ToList();
        }

        public void CleanOldLogs(int daysToKeep = 90)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
            var oldLogs = _context.ActivityLogs
                .Where(a => a.CreatedAt < cutoffDate)
                .ToList();

            _context.ActivityLogs.RemoveRange(oldLogs);
            _context.SaveChanges();
        }
    }
}