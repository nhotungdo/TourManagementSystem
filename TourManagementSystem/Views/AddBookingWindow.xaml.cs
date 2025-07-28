using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class AddBookingWindow : Window
    {
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;
        private TourSchedule _selectedSchedule;
        private decimal _basePrice;

        public AddBookingWindow(ActivityLogService activityLogService)
        {
            InitializeComponent();
            _activityLogService = activityLogService;
            _context = new TourManagementContext();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load customers (users with role "customer")
                var customers = _context.Users
                    .Where(u => u.Role.ToLower() == "customer" && u.IsActive == true)
                    .OrderBy(u => u.FullName)
                    .ToList();
                CustomerComboBox.ItemsSource = customers;

                // Load tour schedules (for testing, include all schedules)
                var schedules = _context.TourSchedules
                    .Include(ts => ts.Tour)
                    .Include(ts => ts.Guide)
                    .Where(ts => ts.Status == "scheduled" || ts.Status == "ongoing")
                    .OrderBy(ts => ts.DepartureDate)
                    .ToList();

                // In production, you would want to filter for future schedules only:
                // var today = DateOnly.FromDateTime(DateTime.Today);
                // var schedules = _context.TourSchedules
                //     .Include(ts => ts.Tour)
                //     .Include(ts => ts.Guide)
                //     .Where(ts => ts.DepartureDate >= today && (ts.Status == "scheduled" || ts.Status == "ongoing"))
                //     .OrderBy(ts => ts.DepartureDate)
                //     .ToList();

                // Create display items for schedules
                var scheduleItems = schedules.Select(s => new
                {
                    ScheduleId = s.ScheduleId,
                    DisplayName = $"{s.Tour.TourName} - {s.DepartureDate:dd/MM/yyyy} - Guide: {s.Guide?.FullName ?? "TBD"} - Available: {s.MaxCapacity - (s.CurrentBookings ?? 0)}",
                    Schedule = s
                }).ToList();

                ScheduleComboBox.ItemsSource = scheduleItems;
                ScheduleComboBox.DisplayMemberPath = "DisplayName";
                ScheduleComboBox.SelectedValuePath = "ScheduleId";

                // Debug: Show how many schedules were loaded
                if (!scheduleItems.Any())
                {
                    MessageBox.Show("No tour schedules available for booking. Please ensure there are scheduled tours with available capacity.", "No Schedules", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Set default date to today
                BookingDatePicker.SelectedDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle customer selection if needed
        }

        private void ScheduleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ScheduleComboBox.SelectedItem != null)
                {
                    var selectedItem = ScheduleComboBox.SelectedItem;
                    var scheduleProperty = selectedItem.GetType().GetProperty("Schedule");
                    if (scheduleProperty != null)
                    {
                        _selectedSchedule = (TourSchedule)scheduleProperty.GetValue(selectedItem);
                        _basePrice = _selectedSchedule.Tour.BasePrice;
                        UpdateTotalAmount();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating total amount: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTotalAmount()
        {
            try
            {
                if (int.TryParse(NumberOfPeopleTextBox.Text, out int numberOfPeople) && _selectedSchedule != null)
                {
                    var totalAmount = _basePrice * numberOfPeople;
                    TotalAmountTextBox.Text = $"{totalAmount:N0} VND";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating total amount: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NumberOfPeopleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTotalAmount();
        }

        private void CreateBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var booking = CreateBookingFromForm();

                    // Check capacity
                    var totalPeople = booking.NumAdults + (booking.NumChildren ?? 0);
                    var currentBookings = _selectedSchedule.CurrentBookings ?? 0;
                    if (currentBookings + totalPeople > _selectedSchedule.MaxCapacity)
                    {
                        MessageBox.Show($"Not enough capacity. Available: {_selectedSchedule.MaxCapacity - currentBookings}, Requested: {totalPeople}",
                            "Capacity Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Add to database
                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    // Log activity
                    _activityLogService.LogActivity(1, "CREATE", "Booking", booking.BookingId,
                        $"Created booking for {totalPeople} people on tour: {_selectedSchedule.Tour.TourName}");

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Customer validation
            if (CustomerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CustomerComboBox.Focus();
                return false;
            }

            // Schedule validation
            if (ScheduleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a tour schedule.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ScheduleComboBox.Focus();
                return false;
            }

            // Number of people validation
            if (!int.TryParse(NumberOfPeopleTextBox.Text, out int numberOfPeople) || numberOfPeople <= 0)
            {
                MessageBox.Show("Please enter a valid number of people (greater than 0).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NumberOfPeopleTextBox.Focus();
                return false;
            }

            if (numberOfPeople > 20)
            {
                MessageBox.Show("Maximum 20 people per booking.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                NumberOfPeopleTextBox.Focus();
                return false;
            }

            // Booking date validation
            if (BookingDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select a booking date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                BookingDatePicker.Focus();
                return false;
            }

            if (BookingDatePicker.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Booking date cannot be in the future.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                BookingDatePicker.Focus();
                return false;
            }

            // Status validation
            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                StatusComboBox.Focus();
                return false;
            }

            return true;
        }

        private Booking CreateBookingFromForm()
        {
            var booking = new Booking();

            // Get next BookingId
            var maxBookingId = _context.Bookings.Max(b => (int?)b.BookingId) ?? 0;
            booking.BookingId = maxBookingId + 1;

            // Set customer
            if (CustomerComboBox.SelectedItem is User selectedCustomer)
            {
                booking.CustomerId = selectedCustomer.UserId;
            }

            // Set schedule
            booking.ScheduleId = _selectedSchedule.ScheduleId;

            // Set number of people (split into adults and children)
            var totalPeople = int.Parse(NumberOfPeopleTextBox.Text);
            booking.NumAdults = totalPeople;
            booking.NumChildren = 0; // For simplicity, all people are adults

            // Set total price
            booking.TotalPrice = _basePrice * totalPeople;

            // Set booking date
            booking.BookingDate = BookingDatePicker.SelectedDate ?? DateTime.Today;

            // Set notes (special requests)
            booking.Notes = string.IsNullOrWhiteSpace(SpecialRequestsTextBox.Text) ? null : SpecialRequestsTextBox.Text.Trim();

            // Set status
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedStatus)
            {
                booking.Status = selectedStatus.Content.ToString();
            }

            return booking;
        }
    }
}