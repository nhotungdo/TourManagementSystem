using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditBookingWindow : Window
    {
        private readonly TourManagementContext _context;
        private readonly BookingService _bookingService;
        private readonly ActivityLogService _activityLogService;
        private readonly Booking _booking;
        private Tour _selectedTour;
        private TourSchedule _selectedSchedule;
        private User _selectedCustomer;

        public EditBookingWindow(Booking booking, BookingService bookingService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _bookingService = bookingService;
            _activityLogService = activityLogService;
            _booking = booking;

            LoadData();
            PopulateForm();
        }

        private void LoadData()
        {
            try
            {
                // Load customers
                var customers = _context.Users
                    .Where(u => u.Role.ToLower() == "customer" && u.IsActive == true)
                    .OrderBy(u => u.FullName)
                    .ToList();
                CustomerComboBox.ItemsSource = customers;

                // Load tours
                var tours = _context.Tours
                    .Where(t => t.IsActive == true)
                    .OrderBy(t => t.TourName)
                    .ToList();
                TourComboBox.ItemsSource = tours;

                // Load schedules for the current tour
                if (_booking.Schedule?.Tour != null)
                {
                    LoadSchedulesForTour(_booking.Schedule.Tour);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSchedulesForTour(Tour tour)
        {
            try
            {
                var schedules = _context.TourSchedules
                    .Include(ts => ts.Guide)
                    .Where(ts => ts.TourId == tour.TourId &&
                                (ts.Status == "scheduled" || ts.Status == "ongoing"))
                    .OrderBy(ts => ts.DepartureDate)
                    .ToList();

                var scheduleItems = schedules.Select(s => new
                {
                    ScheduleId = s.ScheduleId,
                    DisplayName = $"Schedule #{s.ScheduleId} - {s.DepartureDate:dd/MM/yyyy} to {s.ReturnDate:dd/MM/yyyy} - Guide: {s.Guide?.FullName ?? "TBD"} - Status: {s.Status}",
                    Schedule = s
                }).ToList();

                ScheduleComboBox.ItemsSource = scheduleItems;
                ScheduleComboBox.DisplayMemberPath = "DisplayName";
                ScheduleComboBox.SelectedValuePath = "ScheduleId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateForm()
        {
            try
            {
                // Set customer
                if (_booking.Customer != null)
                {
                    CustomerComboBox.SelectedItem = _booking.Customer;
                    CustomerEmailTextBox.Text = _booking.Customer.Email;
                }

                // Set tour
                if (_booking.Schedule?.Tour != null)
                {
                    TourComboBox.SelectedItem = _booking.Schedule.Tour;
                    _selectedTour = _booking.Schedule.Tour;
                }

                // Set schedule
                if (_booking.Schedule != null)
                {
                    ScheduleComboBox.SelectedValue = _booking.ScheduleId;
                    _selectedSchedule = _booking.Schedule;
                    GuideTextBox.Text = _booking.Schedule.Guide?.FullName ?? "TBD";
                }

                // Set booking details
                AdultsTextBox.Text = _booking.NumAdults.ToString();
                ChildrenTextBox.Text = _booking.NumChildren.ToString();
                TotalPriceTextBox.Text = _booking.TotalPrice.ToString("C");

                // Set status
                foreach (ComboBoxItem item in StatusComboBox.Items)
                {
                    if (item.Content.ToString().ToLower() == (_booking.Status ?? "").ToLower())
                    {
                        StatusComboBox.SelectedItem = item;
                        break;
                    }
                }

                // Set payment status
                foreach (ComboBoxItem item in PaymentStatusComboBox.Items)
                {
                    if (item.Content.ToString().ToLower() == (_booking.PaymentStatus ?? "").ToLower())
                    {
                        PaymentStatusComboBox.SelectedItem = item;
                        break;
                    }
                }

                // Set notes
                NotesTextBox.Text = _booking.Notes ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating form: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CustomerComboBox.SelectedItem is User customer)
                {
                    _selectedCustomer = customer;
                    CustomerEmailTextBox.Text = customer.Email;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (TourComboBox.SelectedItem is Tour tour)
                {
                    _selectedTour = tour;
                    LoadSchedulesForTour(tour);
                    ScheduleComboBox.SelectedIndex = -1;
                    GuideTextBox.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdultsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePrice();
        }

        private void ChildrenTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePrice();
        }

        private void CalculatePrice()
        {
            try
            {
                if (_selectedTour == null) return;

                if (int.TryParse(AdultsTextBox.Text, out int adults) &&
                    int.TryParse(ChildrenTextBox.Text, out int children))
                {
                    var totalPeople = adults + children;
                    var basePrice = _selectedTour.BasePrice * totalPeople;
                    TotalPriceTextBox.Text = basePrice.ToString("C");
                }
            }
            catch (Exception ex)
            {
                // Silently handle calculation errors
                System.Diagnostics.Debug.WriteLine($"Error calculating price: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateForm())
                {
                    return;
                }

                // Update booking
                UpdateBookingFromForm();

                // Save to database
                _context.SaveChanges();

                // Log activity
                _activityLogService.LogActivity(1, "UPDATE", "BOOKING", _booking.BookingId,
                    $"Updated booking #{_booking.BookingId} for customer {_booking.Customer?.FullName}");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            // Validate customer
            if (CustomerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CustomerComboBox.Focus();
                return false;
            }

            // Validate tour
            if (TourComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a tour.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TourComboBox.Focus();
                return false;
            }

            // Validate schedule
            if (ScheduleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a schedule.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ScheduleComboBox.Focus();
                return false;
            }

            // Validate adults
            if (!int.TryParse(AdultsTextBox.Text, out int adults) || adults < 0)
            {
                MessageBox.Show("Please enter a valid number of adults (must be 0 or greater).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AdultsTextBox.Focus();
                return false;
            }

            // Validate children
            if (!int.TryParse(ChildrenTextBox.Text, out int children) || children < 0)
            {
                MessageBox.Show("Please enter a valid number of children (must be 0 or greater).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ChildrenTextBox.Focus();
                return false;
            }

            // Validate total people
            if (adults + children == 0)
            {
                MessageBox.Show("Total number of people must be at least 1.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AdultsTextBox.Focus();
                return false;
            }

            // Validate status
            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                StatusComboBox.Focus();
                return false;
            }

            // Validate payment status
            if (PaymentStatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PaymentStatusComboBox.Focus();
                return false;
            }

            return true;
        }

        private void UpdateBookingFromForm()
        {
            // Update customer
            if (CustomerComboBox.SelectedItem is User customer)
            {
                _booking.CustomerId = customer.UserId;
            }

            // Update schedule
            if (ScheduleComboBox.SelectedItem != null)
            {
                var selectedScheduleItem = ScheduleComboBox.SelectedItem;
                var scheduleIdProperty = selectedScheduleItem.GetType().GetProperty("ScheduleId");
                if (scheduleIdProperty != null)
                {
                    var scheduleId = (int)scheduleIdProperty.GetValue(selectedScheduleItem);
                    _booking.ScheduleId = scheduleId;
                }
            }

            // Update booking details
            _booking.NumAdults = int.Parse(AdultsTextBox.Text);
            _booking.NumChildren = int.Parse(ChildrenTextBox.Text);
            _booking.TotalPrice = decimal.Parse(TotalPriceTextBox.Text.Replace("$", "").Replace(",", ""));

            // Update status
            if (StatusComboBox.SelectedItem is ComboBoxItem statusItem)
            {
                _booking.Status = statusItem.Content.ToString().ToLower();
            }

            // Update payment status
            if (PaymentStatusComboBox.SelectedItem is ComboBoxItem paymentStatusItem)
            {
                _booking.PaymentStatus = paymentStatusItem.Content.ToString().ToLower();
            }

            // Update notes
            _booking.Notes = NotesTextBox.Text.Trim();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}