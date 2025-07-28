using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class UserService
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;

        public UserService(TourManagementContext context, ActivityLogService activityLogService)
        {
            _context = context;
            _activityLogService = activityLogService;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.ActivityLogs)
                .Include(u => u.Bookings)
                .OrderByDescending(u => u.CreatedAt)
                .ToList();
        }

        public User? GetUserById(int userId)
        {
            return _context.Users
                .Include(u => u.ActivityLogs)
                .Include(u => u.Bookings)
                .FirstOrDefault(u => u.UserId == userId);
        }

        public bool CreateUser(User user, int adminId)
        {
            try
            {
                // Check if username already exists
                if (_context.Users.Any(u => u.Username.ToLower() == user.Username.ToLower()))
                {
                    return false; // Username already exists
                }

                // Check if email already exists
                if (_context.Users.Any(u => u.Email.ToLower() == user.Email.ToLower()))
                {
                    return false; // Email already exists
                }

                // Generate next UserId
                user.UserId = GetNextUserId();

                // Hash the password before saving
                user.Password = HashPassword(user.Password);
                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                _context.Users.Add(user);
                _context.SaveChanges();

                _activityLogService.LogActivity(adminId, "CREATE", "User", user.UserId,
                    $"Created user: {user.Username} with role: {user.Role}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUser(User user, int adminId)
        {
            try
            {
                user.UpdatedAt = DateTime.Now;
                _context.Users.Update(user);
                _context.SaveChanges();

                _activityLogService.LogActivity(adminId, "UPDATE", "User", user.UserId,
                    $"Updated user: {user.Username}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteUser(int userId, int adminId)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.ActivityLogs)
                    .Include(u => u.Bookings)
                    .Include(u => u.Invoices)
                    .Include(u => u.Payments)
                    .Include(u => u.Reviews)
                    .Include(u => u.Promotions)
                    .Include(u => u.SystemConfigs)
                    .Include(u => u.TourSchedules)
                    .Include(u => u.Tours)
                    .FirstOrDefault(u => u.UserId == userId);

                if (user == null) return false;

                // Check if user has any related data that would prevent deletion
                if (user.Bookings.Any() || user.Invoices.Any() || user.Payments.Any() ||
                    user.Reviews.Any() || user.Promotions.Any() || user.TourSchedules.Any() ||
                    user.Tours.Any())
                {
                    return false; // User has related data, cannot delete
                }

                // Check for tour-related data that might prevent deletion
                var userTourAttractions = _context.TourAttractions.Where(ta => ta.Tour.CreatedBy == userId).ToList();
                var userTourPromotions = _context.TourPromotions.Where(tp => tp.Tour.CreatedBy == userId).ToList();

                if (userTourAttractions.Any() || userTourPromotions.Any())
                {
                    return false; // User has tour-related data, cannot delete
                }

                // Remove related system configs if any
                if (user.SystemConfigs.Any())
                {
                    _context.SystemConfigs.RemoveRange(user.SystemConfigs);
                }

                // Remove related notifications if any
                var userNotifications = _context.Notifications.Where(n => n.UserId == userId || n.CreatedBy == userId).ToList();
                if (userNotifications.Any())
                {
                    _context.Notifications.RemoveRange(userNotifications);
                }

                // Remove related tour attractions and promotions if any
                var userTourAttractionsToDelete = _context.TourAttractions.Where(ta => ta.Tour.CreatedBy == userId).ToList();
                var userTourPromotionsToDelete = _context.TourPromotions.Where(tp => tp.Tour.CreatedBy == userId).ToList();

                if (userTourAttractionsToDelete.Any())
                {
                    _context.TourAttractions.RemoveRange(userTourAttractionsToDelete);
                }

                if (userTourPromotionsToDelete.Any())
                {
                    _context.TourPromotions.RemoveRange(userTourPromotionsToDelete);
                }

                // Remove related activity logs first (to avoid foreign key constraint issues)
                if (user.ActivityLogs.Any())
                {
                    _context.ActivityLogs.RemoveRange(user.ActivityLogs);
                }

                // Remove the user
                _context.Users.Remove(user);
                _context.SaveChanges();

                _activityLogService.LogActivity(adminId, "DELETE", "User", userId,
                    $"Deleted user: {user.Username}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ToggleUserStatus(int userId, int adminId)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user == null) return false;

                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.Now;
                _context.SaveChanges();

                var action = user.IsActive == true ? "ACTIVATED" : "DEACTIVATED";
                _activityLogService.LogActivity(adminId, action, "User", userId,
                    $"{action} user: {user.Username}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ChangeUserRole(int userId, string newRole, int adminId)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user == null) return false;

                var oldRole = user.Role;
                user.Role = newRole;
                user.UpdatedAt = DateTime.Now;
                _context.SaveChanges();

                _activityLogService.LogActivity(adminId, "ROLE_CHANGE", "User", userId,
                    $"Changed role for {user.Username} from {oldRole} to {newRole}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<User> GetUsersByRole(string role)
        {
            return _context.Users
                .Where(u => u.Role.ToLower() == role.ToLower() && u.IsActive == true)
                .OrderBy(u => u.FullName)
                .ToList();
        }

        public List<User> SearchUsers(string searchTerm)
        {
            return _context.Users
                .Where(u => u.Username.Contains(searchTerm) ||
                           u.FullName.Contains(searchTerm) ||
                           u.Email.Contains(searchTerm))
                .OrderBy(u => u.FullName)
                .ToList();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private int GetNextUserId()
        {
            var maxUserId = _context.Users.Max(u => (int?)u.UserId) ?? 0;
            return maxUserId + 1;
        }
    }
}