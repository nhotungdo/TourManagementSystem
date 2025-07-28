using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class AddScheduleWindow : Window
    {
        private readonly TourScheduleService _tourScheduleService;
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public AddScheduleWindow(TourScheduleService tourScheduleService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _tourScheduleService = tourScheduleService;
            _activityLogService = activityLogService;
            _context = new TourManagementContext();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load tours
                var tours = _context.Tours.Where(t => t.IsActive == true).ToList();
                TourComboBox.ItemsSource = tours;
                TourComboBox.DisplayMemberPath = "TourName";
                TourComboBox.SelectedValuePath = "TourId";

                // Load guides (users with role "staff")
                var guides = _context.Users.Where(u => u.Role.ToLower() == "staff" && u.IsActive == true).ToList();
                GuideComboBox.ItemsSource = guides;
                GuideComboBox.DisplayMemberPath = "FullName";
                GuideComboBox.SelectedValuePath = "UserId";

                // Debug: Show how many tours and guides were loaded
                if (!tours.Any())
                {
                    MessageBox.Show("No active tours available. Please ensure there are active tours in the system.", "No Tours", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (!guides.Any())
                {
                    MessageBox.Show("No staff guides available. Please ensure there are staff users in the system.", "No Guides", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TourComboBox.SelectedItem is Tour selectedTour)
            {
                // Set default max capacity based on tour duration
                MaxCapacityTextBox.Text = "20"; // Default capacity
            }
        }

        private void CreateScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var schedule = CreateScheduleFromForm();

                    if (_tourScheduleService.CreateSchedule(schedule, 1)) // 1 is admin ID
                    {
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create schedule. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating schedule: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Tour validation
            if (TourComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a tour.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TourComboBox.Focus();
                return false;
            }

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

            // Debug: Show selected dates
            var departureDate = DepartureDatePicker.SelectedDate.Value.Date;
            var returnDate = ReturnDatePicker.SelectedDate.Value.Date;
            var today = DateTime.Now.Date;
            Console.WriteLine($"Selected departure date: {departureDate:yyyy-MM-dd}");
            Console.WriteLine($"Selected return date: {returnDate:yyyy-MM-dd}");
            Console.WriteLine($"Today: {today:yyyy-MM-dd}");

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

        private TourSchedule CreateScheduleFromForm()
        {
            var selectedTour = TourComboBox.SelectedItem as Tour;
            var selectedGuide = GuideComboBox.SelectedItem as User;

            var schedule = new TourSchedule
            {
                TourId = selectedTour.TourId,
                DepartureDate = DateOnly.FromDateTime(DepartureDatePicker.SelectedDate.Value),
                ReturnDate = DateOnly.FromDateTime(ReturnDatePicker.SelectedDate.Value),
                MaxCapacity = int.Parse(MaxCapacityTextBox.Text),
                CurrentBookings = 0,
                Status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Scheduled",
                GuideId = selectedGuide.UserId
            };

            return schedule;
        }
    }
}