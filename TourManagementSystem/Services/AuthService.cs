using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Services
{
    public class AuthService
    {
        private readonly TourManagementContext _context;

        public AuthService(TourManagementContext context)
        {
            _context = context;
        }

        public User? AuthenticateUser(string username, string password)
        {
            try
            {
                // Test database connection first
                if (!_context.Database.CanConnect())
                {
                    throw new Exception("Cannot connect to database. Please check your connection string.");
                }

                var hashedPassword = HashPassword(password);

                // Try to find user with hashed password first (for new users)
                var user = _context.Users
                    .FirstOrDefault(u => u.Username == username &&
                                       u.Password == hashedPassword &&
                                       u.IsActive == true);

                // If not found, try with plain text password (for existing database users)
                if (user == null)
                {
                    user = _context.Users
                        .FirstOrDefault(u => u.Username == username &&
                                           u.Password == password &&
                                           u.IsActive == true);

                    // If found with plain text, update to hashed password
                    if (user != null)
                    {
                        user.Password = hashedPassword;
                        user.UpdatedAt = DateTime.Now;
                        _context.SaveChanges();
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Authentication error: {ex.Message}");
            }
        }

        public bool RegisterUser(User user)
        {
            try
            {
                user.Password = HashPassword(user.Password);
                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                user.Role = "customer"; // Default role

                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return false;

            var hashedOldPassword = HashPassword(oldPassword);
            if (user.Password != hashedOldPassword) return false;

            user.Password = HashPassword(newPassword);
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return true;
        }

        public bool UpdateUserProfile(User user)
        {
            try
            {
                user.UpdatedAt = DateTime.Now;
                _context.Users.Update(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsAdmin(int userId)
        {
            var user = _context.Users.Find(userId);
            return user?.Role?.ToLower() == "admin";
        }

        public bool IsStaff(int userId)
        {
            var user = _context.Users.Find(userId);
            return user?.Role?.ToLower() == "staff" || user?.Role?.ToLower() == "admin";
        }

        public bool IsCustomer(int userId)
        {
            var user = _context.Users.Find(userId);
            return user?.Role?.ToLower() == "customer";
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool ResetPassword(string username, string email, string newPassword)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (user == null) return false;

                // Update password
                user.Password = HashPassword(newPassword);
                user.UpdatedAt = DateTime.Now;
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}