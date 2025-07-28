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
    public partial class ActivityLogPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _logsView;
        private List<ActivityLog> _allLogs;

        public ActivityLogPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);

            LoadActivityLogs();
            LoadFilters();
            SetupEventHandlers();
        }

        private void LoadActivityLogs()
        {
            try
            {
                _allLogs = _context.ActivityLogs
                    .Include(al => al.User)
                    .OrderByDescending(al => al.CreatedAt)
                    .ToList();

                _logsView = CollectionViewSource.GetDefaultView(_allLogs);
                ActivityLogsDataGrid.ItemsSource = _logsView;

                // Apply initial filter
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading activity logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFilters()
        {
            try
            {
                // Load user filter
                var users = _context.Users.Where(u => u.IsActive == true).ToList();
                UserFilterComboBox.Items.Clear();
                UserFilterComboBox.Items.Add(new ComboBoxItem { Content = "All Users", IsSelected = true });

                foreach (var user in users)
                {
                    UserFilterComboBox.Items.Add(new ComboBoxItem { Content = user.FullName, Tag = user.UserId });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupEventHandlers()
        {
            // Setup sorting
            ActivityLogsDataGrid.Sorting += ActivityLogsDataGrid_Sorting;
        }

        private void ApplyFilters()
        {
            if (_logsView == null) return;

            _logsView.Filter = log =>
            {
                if (log is not ActivityLog logObj) return false;

                // Search filter
                var searchText = SearchTextBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = logObj.User?.FullName?.ToLower().Contains(searchText) == true ||
                                       logObj.Action?.ToLower().Contains(searchText) == true ||
                                       logObj.EntityType?.ToLower().Contains(searchText) == true ||
                                       logObj.Description?.ToLower().Contains(searchText) == true ||
                                       logObj.IpAddress?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // User filter
                var selectedUser = (UserFilterComboBox.SelectedItem as ComboBoxItem)?.Tag;
                if (selectedUser != null && selectedUser is int userId)
                {
                    if (logObj.UserId != userId) return false;
                }

                // Action filter
                var selectedAction = (ActionFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedAction) && selectedAction != "All Actions")
                {
                    if (logObj.Action != selectedAction) return false;
                }

                // Level filter
                var selectedLevel = (LevelFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedLevel) && selectedLevel != "All Levels")
                {
                    if (logObj.LogLevel != selectedLevel) return false;
                }

                return true;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void UserFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ActionFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void LevelFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ActivityLogsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void ActivityLogsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void ClearLogsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Are you sure you want to clear all activity logs? This action cannot be undone.",
                    "Confirm Clear Logs",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Keep only logs from the last 30 days
                    var cutoffDate = DateTime.Now.AddDays(-30);
                    var logsToDelete = _context.ActivityLogs
                        .Where(al => al.CreatedAt < cutoffDate)
                        .ToList();

                    _context.ActivityLogs.RemoveRange(logsToDelete);
                    _context.SaveChanges();

                    // Log the clear action
                    _activityLogService.LogActivity(1, "CLEAR", "ActivityLog", 0,
                        $"Cleared {logsToDelete.Count} old activity logs");

                    LoadActivityLogs(); // Refresh the list
                    MessageBox.Show($"Cleared {logsToDelete.Count} old activity logs successfully!",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int logId)
                {
                    var log = _allLogs.FirstOrDefault(l => l.LogId == logId);
                    if (log != null)
                    {
                        var result = MessageBox.Show(
                            $"Are you sure you want to delete this log entry?\n\nUser: {log.User?.FullName}\nAction: {log.Action}\nDescription: {log.Description}",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            _context.ActivityLogs.Remove(log);
                            _context.SaveChanges();

                            // Log the delete action
                            _activityLogService.LogActivity(1, "DELETE", "ActivityLog", logId,
                                $"Deleted activity log: {log.Action} by {log.User?.FullName}");

                            LoadActivityLogs(); // Refresh the list
                            MessageBox.Show("Log entry deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting log entry: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}