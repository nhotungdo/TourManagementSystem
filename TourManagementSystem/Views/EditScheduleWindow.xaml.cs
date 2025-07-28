using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditScheduleWindow : Window
    {
        private readonly TourSchedule _schedule;
        private readonly TourScheduleService _tourScheduleService;
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public EditScheduleWindow(TourSchedule schedule, TourScheduleService tourScheduleService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _schedule = schedule;
            _tourScheduleService = tourScheduleService;
            _activityLogService = activityLogService;
            _context = new TourManagementContext();

            LoadData();
            LoadScheduleData();
        }

        private void LoadData()
        {
            try
            {
                // Load guides (users with role "staff")
                var guides = _context.Users.Where(u => u.Role.ToLower() == "staff" && u.IsActive == true).ToList();
                GuideComboBox.ItemsSource = guides;
                GuideComboBox.DisplayMemberPath = "FullName";
                GuideComboBox.SelectedValuePath = "UserId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadScheduleData()
        {
            try
            {
                // Load tour name
                var tour = _context.Tours.FirstOrDefault(t => t.TourId == _schedule.TourId);
                TourTextBox.Text = tour?.TourName ?? "Unknown Tour";

                // Load dates
                DepartureDatePicker.SelectedDate = _schedule.DepartureDate.ToDateTime(TimeOnly.MinValue);
                ReturnDatePicker.SelectedDate = _schedule.ReturnDate.ToDateTime(TimeOnly.MinValue);

                // Load capacity
                MaxCapacityTextBox.Text = _schedule.MaxCapacity.ToString();

                // Load guide
                var guide = _context.Users.FirstOrDefault(u => u.UserId == _schedule.GuideId);
                if (guide != null)
                {
                    GuideComboBox.SelectedValue = guide.UserId;
                }

                // Load status
                foreach (ComboBoxItem item in StatusComboBox.Items)
                {
                    if (item.Content.ToString() == _schedule.Status)
                    {
                        StatusComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedule data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    UpdateScheduleFromForm();

                    if (_tourScheduleService.UpdateSchedule(_schedule, 1)) // 1 is admin ID
                    {
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update schedule. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating schedule: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Departure date validation
            if (!DepartureDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Departure date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DepartureDatePicker.Focus();
                return false;
            }

            // For testing purposes, allow past dates
            // In production, you would want to validate that departure date is not in the past
            // if (DepartureDatePicker.SelectedDate.Value.Date < DateTime.Now.Date)
            // {
            //     MessageBox.Show("Departure date cannot be in the past.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            //     DepartureDatePicker.Focus();
            //     return false;
            // }

            // Return date validation
            if (!ReturnDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Return date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ReturnDatePicker.Focus();
                return false;
            }

            if (ReturnDatePicker.SelectedDate.Value.Date <= DepartureDatePicker.SelectedDate.Value.Date)
            {
                MessageBox.Show("Return date must be after departure date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ReturnDatePicker.Focus();
                return false;
            }

            // Max capacity validation
            if (string.IsNullOrWhiteSpace(MaxCapacityTextBox.Text))
            {
                MessageBox.Show("Max capacity is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MaxCapacityTextBox.Focus();
                return false;
            }

            if (!int.TryParse(MaxCapacityTextBox.Text, out int maxCapacity) || maxCapacity <= 0)
            {
                MessageBox.Show("Max capacity must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MaxCapacityTextBox.Focus();
                return false;
            }

            // Guide validation
            if (GuideComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a guide.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                GuideComboBox.Focus();
                return false;
            }

            return true;
        }

        private void UpdateScheduleFromForm()
        {
            _schedule.DepartureDate = DateOnly.FromDateTime(DepartureDatePicker.SelectedDate.Value);
            _schedule.ReturnDate = DateOnly.FromDateTime(ReturnDatePicker.SelectedDate.Value);
            _schedule.MaxCapacity = int.Parse(MaxCapacityTextBox.Text);
            _schedule.Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Scheduled";

            if (GuideComboBox.SelectedValue != null && int.TryParse(GuideComboBox.SelectedValue.ToString(), out int guideId))
            {
                _schedule.GuideId = guideId;
            }
        }
    }
}