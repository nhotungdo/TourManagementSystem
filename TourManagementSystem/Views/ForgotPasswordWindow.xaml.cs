using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly TourManagementContext _context;

        public ForgotPasswordWindow()
        {
            InitializeComponent();
            _context = new TourManagementContext();

            // Pre-fill with a test email for convenience
            EmailTextBox.Text = "customer1@gmail.com";

            // Test database connection on startup
            TestDatabaseConnection();
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (!ValidateInput())
                    return;

                // Show loading
                ShowLoading(true);
                HideMessages();

                // Find user by email
                var email = EmailTextBox.Text.Trim();

                // Debug: Check if database connection works
                var allUsers = _context.Users.ToList();
                var user = allUsers.FirstOrDefault(u =>
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    var availableEmails = string.Join(", ", allUsers.Select(u => u.Email));
                    ShowError($"Không tìm thấy tài khoản với email: {email}\n\nCác email có sẵn: {availableEmails}");
                    ShowLoading(false);
                    return;
                }

                // Set new password to "123456" - đây là mục đích chính
                var newPassword = "123456";

                // Update user's password in database
                user.Password = HashPassword(newPassword);
                user.UpdatedAt = DateTime.Now;

                // Save changes to database
                var result = _context.SaveChanges();

                if (result > 0)
                {
                    // Verify the password was saved correctly
                    _context.Entry(user).Reload();
                    var savedUser = _context.Users.FirstOrDefault(u => u.Email == email);

                    if (savedUser != null && VerifyPassword(newPassword, savedUser.Password))
                    {
                        // Show success message with new password - đây là mục đích chính
                        ShowSuccess(newPassword);

                        // Clear form after showing success
                        EmailTextBox.Clear();

                        // Log the successful password reset
                        System.Diagnostics.Debug.WriteLine($"Password reset successful for {email}. New password: {newPassword}");
                    }
                    else
                    {
                        ShowError("Password đã được cập nhật nhưng có lỗi xác minh. Vui lòng thử lại.");
                        ShowLoading(false);
                    }
                }
                else
                {
                    ShowError("Không thể cập nhật password. Vui lòng thử lại.");
                    ShowLoading(false);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi khi reset password: {ex.Message}\n\nChi tiết: {ex.StackTrace}");
                ShowLoading(false);
            }
        }

        private bool ValidateInput()
        {
            // Clear previous error
            HideMessages();

            // Check required fields
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowError("Vui lòng nhập email address.");
                return false;
            }

            // Validate email format
            if (!IsValidEmail(EmailTextBox.Text))
            {
                ShowError("Vui lòng nhập email hợp lệ.");
                return false;
            }

            return true;
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

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // Ensure password has at least one uppercase, one lowercase, one digit, and one special character
            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(c => "!@#$%^&*".Contains(c));

            if (!hasUpper || !hasLower || !hasDigit || !hasSpecial)
            {
                // Regenerate if requirements not met
                return GenerateRandomPassword();
            }

            return password;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            var hashedInput = HashPassword(plainPassword);
            return hashedInput.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
        }

        private void ShowLoading(bool show)
        {
            LoadingProgressBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ShowSuccess(string newPassword)
        {
            try
            {
                // Set the password text - đây là mục đích chính
                NewPasswordText.Text = $"Password mới của bạn là: {newPassword}";

                // Hide loading first
                ShowLoading(false);

                // Hide error message if any
                ErrorMessage.Visibility = Visibility.Collapsed;

                // Show success message - đây là giao diện thông báo chính
                SuccessMessage.Visibility = Visibility.Visible;

                // Disable form elements when showing success
                EmailTextBox.IsEnabled = false;
                var resetButton = FindName("ResetPasswordButton") as Button;
                if (resetButton != null)
                {
                    resetButton.IsEnabled = false;
                }

                // Force UI update to ensure message is visible
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SuccessMessage.UpdateLayout();
                    NewPasswordText.UpdateLayout();
                });

                // Log success for debugging
                System.Diagnostics.Debug.WriteLine($"Password reset successful for email: {EmailTextBox.Text.Trim()}");

                // Test login with new password
                var loginTest = TestLoginWithNewPassword(EmailTextBox.Text.Trim(), newPassword);
                System.Diagnostics.Debug.WriteLine($"Login test after reset: {(loginTest ? "SUCCESS" : "FAILED")}");

                // Show a simple message box as backup with login instructions
                var message = $"Password mới của bạn là: {newPassword}\n\nBây giờ bạn có thể đăng nhập với password mới này!\n\nTest đăng nhập: {(loginTest ? "✅ Thành công" : "❌ Thất bại")}";
                MessageBox.Show(message, "Reset Password Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi hiển thị thông báo: {ex.Message}");
                // Show backup message box
                var message = $"Password mới của bạn là: {newPassword}\n\nBây giờ bạn có thể đăng nhập với password mới này!";
                MessageBox.Show(message, "Reset Password Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;

            // Also show in message box for important errors
            if (message.Contains("Không tìm thấy") || message.Contains("Lỗi"))
            {
                MessageBox.Show(message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HideMessages()
        {
            SuccessMessage.Visibility = Visibility.Collapsed;
            ErrorMessage.Visibility = Visibility.Collapsed;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void SignInText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void TryAnotherEmailButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset form
            EmailTextBox.Clear();
            EmailTextBox.IsEnabled = true;
            var resetButton = FindName("ResetPasswordButton") as Button;
            if (resetButton != null)
            {
                resetButton.IsEnabled = true;
            }
            HideMessages();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Method to create test user if needed (can be called from external code)
        public void CreateTestUserIfNeeded()
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == "customer1@gmail.com");
                if (existingUser == null)
                {
                    var testUser = new User
                    {
                        Username = "customer1",
                        Email = "customer1@gmail.com",
                        Password = HashPassword("oldpassword"),
                        FullName = "Customer One",
                        Role = "customer",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Users.Add(testUser);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Silently handle error
            }
        }

        // Test database connection
        private void TestDatabaseConnection()
        {
            try
            {
                var userCount = _context.Users.Count();
                System.Diagnostics.Debug.WriteLine($"Database connection successful. Found {userCount} users.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection failed: {ex.Message}");
                MessageBox.Show($"Lỗi kết nối database: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Test login with new password
        public bool TestLoginWithNewPassword(string email, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                if (user != null)
                {
                    var hashedPassword = HashPassword(password);
                    var isValid = user.Password.Equals(hashedPassword, StringComparison.OrdinalIgnoreCase);
                    System.Diagnostics.Debug.WriteLine($"Login test for {email}: {(isValid ? "SUCCESS" : "FAILED")}");
                    return isValid;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login test failed: {ex.Message}");
                return false;
            }
        }

        // Clean up resources when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            _context?.Dispose();
        }
    }
}