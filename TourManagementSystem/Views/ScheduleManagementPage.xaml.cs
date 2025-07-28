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
    public partial class ScheduleManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly TourScheduleService _tourScheduleService;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _schedulesView;
        private List<TourSchedule> _allSchedules;

        public ScheduleManagementPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);
            _tourScheduleService = new TourScheduleService(_context, _activityLogService);

            LoadSchedules();
        }

        private void LoadSchedules()
        {
            try
            {
                _allSchedules = _context.TourSchedules
                    .Include(ts => ts.Tour)
                    .Include(ts => ts.Guide)
                    .ToList();

                _schedulesView = CollectionViewSource.GetDefaultView(_allSchedules);
                SchedulesDataGrid.ItemsSource = _schedulesView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_schedulesView == null) return;

            _schedulesView.Filter = item =>
            {
                if (item is not TourSchedule schedule) return false;

                // Search filter
                var searchText = SearchTextBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = schedule.Tour?.TourName?.ToLower().Contains(searchText) == true ||
                                       schedule.Guide?.FullName?.ToLower().Contains(searchText) == true ||
                                       schedule.Status?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // Status filter
                var selectedStatus = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All Status")
                {
                    if (schedule.Status != selectedStatus) return false;
                }

                return true;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SchedulesDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void SchedulesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void AddScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addScheduleWindow = new AddScheduleWindow(_tourScheduleService, _activityLogService);
                if (addScheduleWindow.ShowDialog() == true)
                {
                    LoadSchedules(); // Refresh data
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add Schedule window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int scheduleId)
                {
                    var schedule = _allSchedules.FirstOrDefault(s => s.ScheduleId == scheduleId);
                    if (schedule != null)
                    {
                        var editScheduleWindow = new EditScheduleWindow(schedule, _tourScheduleService, _activityLogService);
                        if (editScheduleWindow.ShowDialog() == true)
                        {
                            LoadSchedules(); // Refresh data
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing schedule: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int scheduleId)
                {
                    var schedule = _allSchedules.FirstOrDefault(s => s.ScheduleId == scheduleId);
                    if (schedule != null)
                    {
                        // Check if schedule has bookings
                        var hasBookings = _context.Bookings.Any(b => b.ScheduleId == scheduleId);
                        string message = $"Are you sure you want to delete the schedule for '{schedule.Tour?.TourName}'?";

                        if (hasBookings)
                        {
                            message += "\n\n⚠️ WARNING: This schedule has existing bookings. Deleting the schedule will also delete all related bookings.";
                        }

                        var result = MessageBox.Show(
                            message,
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            hasBookings ? MessageBoxImage.Warning : MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                            if (_tourScheduleService.DeleteSchedule(scheduleId, 1)) // 1 is admin ID
                            {
                                LoadSchedules(); // Refresh data
                                    string successMessage = "Schedule deleted successfully.";
                                    if (hasBookings)
                                    {
                                        successMessage += "\n\nNote: Related bookings have also been deleted.";
                                    }
                                    MessageBox.Show(successMessage, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                    MessageBox.Show("Failed to delete schedule. The schedule may have existing bookings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            catch (InvalidOperationException ex)
                            {
                                MessageBox.Show($"Cannot delete schedule: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting schedule: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}