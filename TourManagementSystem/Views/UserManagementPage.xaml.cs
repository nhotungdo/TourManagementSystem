using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class UserManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly UserService _userService;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _usersView;
        private List<User> _allUsers;

        public UserManagementPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);
            _userService = new UserService(_context, _activityLogService);

            LoadUsers();
            SetupEventHandlers();
        }

        private void LoadUsers()
        {
            try
            {
                _allUsers = _userService.GetAllUsers().ToList();
                _usersView = CollectionViewSource.GetDefaultView(_allUsers);
                UsersDataGrid.ItemsSource = _usersView;

                // Apply initial filter
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupEventHandlers()
        {
            // Setup sorting
            UsersDataGrid.Sorting += UsersDataGrid_Sorting;
        }

        private void ApplyFilters()
        {
            if (_usersView == null) return;

            _usersView.Filter = user =>
            {
                if (user is not User userObj) return false;

                // Search filter
                var searchText = SearchTextBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = userObj.Username?.ToLower().Contains(searchText) == true ||
                                       userObj.FullName?.ToLower().Contains(searchText) == true ||
                                       userObj.Email?.ToLower().Contains(searchText) == true ||
                                       userObj.Phone?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // Role filter
                var selectedRole = (RoleFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedRole) && selectedRole != "All Roles")
                {
                    if (userObj.Role != selectedRole) return false;
                }

                return true;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void RoleFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void UsersDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addUserWindow = new AddUserWindow(_userService, _activityLogService);
                if (addUserWindow.ShowDialog() == true)
                {
                    LoadUsers(); // Refresh the list
                    MessageBox.Show("User added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int userId)
                {
                    var user = _allUsers.FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        var editUserWindow = new EditUserWindow(user, _userService, _activityLogService);
                        if (editUserWindow.ShowDialog() == true)
                        {
                            LoadUsers(); // Refresh the list
                            MessageBox.Show("User updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleUserStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int userId)
                {
                    var user = _allUsers.FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        // Prevent deactivation of main admin user
                        if (user.UserId == 1 && user.Role.ToLower() == "admin" && user.IsActive == true)
                        {
                            MessageBox.Show(
                                "Cannot deactivate the main administrator account for security reasons.",
                                "Cannot Deactivate Admin",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            return;
                        }

                        var currentStatus = user.IsActive == true ? "Active" : "Inactive";
                        var newStatus = user.IsActive == true ? "Inactive" : "Active";

                        var result = MessageBox.Show(
                            $"Are you sure you want to change user '{user.Username}' status from {currentStatus} to {newStatus}?",
                            "Confirm Status Change",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            if (_userService.ToggleUserStatus(userId, 1)) // 1 is admin ID
                            {
                                LoadUsers(); // Refresh the list
                                MessageBox.Show($"User status changed to {newStatus} successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Failed to change user status. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing user status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int userId)
                {
                    var user = _allUsers.FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        // Prevent deletion of main admin user
                        if (user.UserId == 1 && user.Role.ToLower() == "admin")
                        {
                            MessageBox.Show(
                                "Cannot delete the main administrator account for security reasons.",
                                "Cannot Delete Admin",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            return;
                        }

                        var result = MessageBox.Show(
                            $"Are you sure you want to delete user '{user.Username}'?\n\nThis action cannot be undone.",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            if (_userService.DeleteUser(userId, 1)) // 1 is admin ID
                            {
                                // Log the activity - use admin ID (1) instead of userId
                                _activityLogService.LogActivity(
                                    1, // Admin ID who performed the action
                                    "DELETE_USER",
                                    "User",
                                    userId,
                                    $"Deleted user '{user.Username}'");

                                LoadUsers(); // Refresh the list
                                MessageBox.Show("User deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                // Check if user has related data
                                var userWithRelations = _context.Users
                                    .Include(u => u.Bookings)
                                    .Include(u => u.Invoices)
                                    .Include(u => u.Payments)
                                    .Include(u => u.Reviews)
                                    .Include(u => u.Promotions)
                                    .Include(u => u.TourSchedules)
                                    .Include(u => u.Tours)
                                    .FirstOrDefault(u => u.UserId == userId);

                                if (userWithRelations != null &&
    (userWithRelations.Bookings.Any() || userWithRelations.Invoices.Any() ||
     userWithRelations.Payments.Any() || userWithRelations.Reviews.Any() ||
     userWithRelations.Promotions.Any() || userWithRelations.TourSchedules.Any() ||
     userWithRelations.Tours.Any()))
                                {
                                    // Build detailed message about related data
                                    var relatedData = new List<string>();
                                    if (userWithRelations.Bookings.Any()) relatedData.Add($"{userWithRelations.Bookings.Count} booking(s)");
                                    if (userWithRelations.Invoices.Any()) relatedData.Add($"{userWithRelations.Invoices.Count} invoice(s)");
                                    if (userWithRelations.Payments.Any()) relatedData.Add($"{userWithRelations.Payments.Count} payment(s)");
                                    if (userWithRelations.Reviews.Any()) relatedData.Add($"{userWithRelations.Reviews.Count} review(s)");
                                    if (userWithRelations.Promotions.Any()) relatedData.Add($"{userWithRelations.Promotions.Count} promotion(s)");
                                    if (userWithRelations.TourSchedules.Any()) relatedData.Add($"{userWithRelations.TourSchedules.Count} tour schedule(s)");
                                    if (userWithRelations.Tours.Any()) relatedData.Add($"{userWithRelations.Tours.Count} tour(s)");

                                    // Check for additional tour-related data
                                    var userTourAttractions = _context.TourAttractions.Where(ta => ta.Tour.CreatedBy == userId).Count();
                                    var userTourPromotions = _context.TourPromotions.Where(tp => tp.Tour.CreatedBy == userId).Count();

                                    if (userTourAttractions > 0) relatedData.Add($"{userTourAttractions} tour attraction(s)");
                                    if (userTourPromotions > 0) relatedData.Add($"{userTourPromotions} tour promotion(s)");

                                    // Check for notifications
                                    var userNotifications = _context.Notifications.Where(n => n.UserId == userId || n.CreatedBy == userId).Count();
                                    if (userNotifications > 0) relatedData.Add($"{userNotifications} notification(s)");

                                    // Check for system configs
                                    if (userWithRelations.SystemConfigs.Any()) relatedData.Add($"{userWithRelations.SystemConfigs.Count} system config(s)");

                                    // Check for activity logs
                                    if (userWithRelations.ActivityLogs.Any()) relatedData.Add($"{userWithRelations.ActivityLogs.Count} activity log(s)");

                                    var relatedDataText = string.Join(", ", relatedData);
                                    var deactivateResult = MessageBox.Show(
                                        $"Cannot delete user '{user.Username}' because they have related data: {relatedDataText}.\n\nWould you like to deactivate the user instead?",
                                        "Cannot Delete User",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Warning);

                                    if (deactivateResult == MessageBoxResult.Yes)
                                    {
                                        // Automatically toggle user status to inactive
                                        if (_userService.ToggleUserStatus(userId, 1))
                                        {
                                            LoadUsers(); // Refresh the list
                                            MessageBox.Show($"User '{user.Username}' has been deactivated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                        }
                                        else
                                        {
                                            MessageBox.Show("Failed to deactivate user. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Failed to delete user. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}