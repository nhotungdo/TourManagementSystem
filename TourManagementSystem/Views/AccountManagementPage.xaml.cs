using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class AccountManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentUser;

        public AccountManagementPage(User user)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _currentUser = user;

            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                // Load current user data from database
                var user = _context.Users.FirstOrDefault(u => u.UserId == _currentUser.UserId);
                if (user != null)
                {
                    FullNameTextBox.Text = user.FullName ?? "";
                    EmailTextBox.Text = user.Email ?? "";
                    PhoneTextBox.Text = user.Phone ?? "";
                    AddressTextBox.Text = user.Address ?? "";

                    // Note: DateOfBirth and Gender are not available in the User model
                    // These fields will be left empty or with default values
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SavePersonalInfoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
                {
                    MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    MessageBox.Show("Email is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate email format
                if (!IsValidEmail(EmailTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if email is already taken by another user
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == EmailTextBox.Text && u.UserId != _currentUser.UserId);
                if (existingUser != null)
                {
                    MessageBox.Show("This email address is already registered by another user.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update user information
                var user = _context.Users.FirstOrDefault(u => u.UserId == _currentUser.UserId);
                if (user != null)
                {
                    user.FullName = FullNameTextBox.Text.Trim();
                    user.Email = EmailTextBox.Text.Trim();
                    user.Phone = PhoneTextBox.Text.Trim();
                    user.Address = AddressTextBox.Text.Trim();
                    // Note: DateOfBirth and Gender are not available in the User model

                    _context.SaveChanges();

                    // Update current user object
                    _currentUser.FullName = user.FullName;
                    _currentUser.Email = user.Email;
                    _currentUser.Phone = user.Phone;
                    _currentUser.Address = user.Address;

                    MessageBox.Show("Personal information updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving personal information: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetPersonalInfoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadUserData();
                MessageBox.Show("Form has been reset to original values.", "Reset", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting form: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate current password
                if (string.IsNullOrEmpty(CurrentPasswordBox.Password))
                {
                    MessageBox.Show("Please enter your current password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate new password
                if (string.IsNullOrEmpty(NewPasswordBox.Password))
                {
                    MessageBox.Show("Please enter a new password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(ConfirmPasswordBox.Password))
                {
                    MessageBox.Show("Please confirm your new password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Check if passwords match
                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("New password and confirmation password do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate password strength
                if (!IsValidPassword(NewPasswordBox.Password))
                {
                    MessageBox.Show("Password does not meet security requirements:\n• Minimum 8 characters\n• At least one uppercase letter\n• At least one number\n• At least one special character",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Verify current password
                var user = _context.Users.FirstOrDefault(u => u.UserId == _currentUser.UserId);
                if (user != null)
                {
                    var hashedCurrentPassword = HashPassword(CurrentPasswordBox.Password);
                    if (user.Password != hashedCurrentPassword)
                    {
                        MessageBox.Show("Current password is incorrect.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Update password
                    user.Password = HashPassword(NewPasswordBox.Password);
                    _context.SaveChanges();

                    // Clear password fields
                    CurrentPasswordBox.Password = "";
                    NewPasswordBox.Password = "";
                    ConfirmPasswordBox.Password = "";

                    MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            // Check minimum length
            if (password.Length < 8)
                return false;

            // Check for uppercase letter
            if (!password.Any(char.IsUpper))
                return false;

            // Check for number
            if (!password.Any(char.IsDigit))
                return false;

            // Check for special character
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return false;

            return true;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}