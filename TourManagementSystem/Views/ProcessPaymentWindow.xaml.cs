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
    public partial class ProcessPaymentWindow : Window
    {
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;
        private Booking _selectedBooking;

        public ProcessPaymentWindow(ActivityLogService activityLogService)
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
                // Load bookings that need payment (pending or unpaid status)
                var bookings = _context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                    .Where(b => b.Status == "Confirmed" && b.PaymentStatus == "unpaid")
                    .OrderBy(b => b.BookingDate)
                    .ToList();

                // Create display items for bookings
                var bookingItems = bookings.Select(b => new
                {
                    BookingId = b.BookingId,
                    DisplayName = $"Booking #{b.BookingId} - {b.Customer.FullName} - {b.Schedule.Tour.TourName} - {b.TotalPrice:N0} VND",
                    Booking = b
                }).ToList();

                BookingComboBox.ItemsSource = bookingItems;
                BookingComboBox.DisplayMemberPath = "DisplayName";
                BookingComboBox.SelectedValuePath = "BookingId";

                // Set default date to today
                PaymentDatePicker.SelectedDate = DateTime.Today;

                // Auto-generate transaction ID
                TransactionIdTextBox.Text = GenerateTransactionId();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BookingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (BookingComboBox.SelectedItem != null)
                {
                    var selectedItem = BookingComboBox.SelectedItem;
                    var bookingProperty = selectedItem.GetType().GetProperty("Booking");
                    if (bookingProperty != null)
                    {
                        _selectedBooking = (Booking)bookingProperty.GetValue(selectedItem);
                        UpdateBookingInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating booking info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateBookingInfo()
        {
            if (_selectedBooking != null)
            {
                CustomerTextBox.Text = _selectedBooking.Customer.FullName;
                TourTextBox.Text = $"{_selectedBooking.Schedule.Tour.TourName} - {_selectedBooking.Schedule.DepartureDate:dd/MM/yyyy}";
                AmountDueTextBox.Text = $"{_selectedBooking.TotalPrice:N0} VND";
                PaymentAmountTextBox.Text = _selectedBooking.TotalPrice.ToString();
            }
        }

        private string GenerateTransactionId()
        {
            var random = new Random();
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var randomPart = random.Next(1000, 9999);
            return $"TRX{timestamp}{randomPart}";
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ProcessPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var payment = CreatePaymentFromForm();

                    // Add to database
                    _context.Payments.Add(payment);

                    // Update booking payment status
                    _selectedBooking.PaymentStatus = "paid";
                    _context.SaveChanges();

                    // Log activity
                    _activityLogService.LogActivity(1, "CREATE", "Payment", payment.PaymentId,
                        $"Processed payment of {payment.Amount:N0} VND for booking #{_selectedBooking.BookingId}");

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Booking validation
            if (BookingComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a booking.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                BookingComboBox.Focus();
                return false;
            }

            // Payment amount validation
            if (!decimal.TryParse(PaymentAmountTextBox.Text, out decimal paymentAmount) || paymentAmount <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount (greater than 0).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PaymentAmountTextBox.Focus();
                return false;
            }

            // Payment method validation
            if (PaymentMethodComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment method.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PaymentMethodComboBox.Focus();
                return false;
            }

            // Payment date validation
            if (PaymentDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select a payment date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PaymentDatePicker.Focus();
                return false;
            }

            if (PaymentDatePicker.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Payment date cannot be in the future.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PaymentDatePicker.Focus();
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

        private Payment CreatePaymentFromForm()
        {
            var payment = new Payment();

            // Get next PaymentId
            var maxPaymentId = _context.Payments.Max(p => (int?)p.PaymentId) ?? 0;
            payment.PaymentId = maxPaymentId + 1;

            // Set booking
            payment.BookingId = _selectedBooking.BookingId;

            // Set amount
            payment.Amount = decimal.Parse(PaymentAmountTextBox.Text);

            // Set payment method
            if (PaymentMethodComboBox.SelectedItem is ComboBoxItem selectedMethod)
            {
                payment.PaymentMethod = selectedMethod.Content.ToString();
            }

            // Set transaction ID
            payment.TransactionId = string.IsNullOrWhiteSpace(TransactionIdTextBox.Text) ? GenerateTransactionId() : TransactionIdTextBox.Text.Trim();

            // Set payment date
            payment.PaymentDate = PaymentDatePicker.SelectedDate ?? DateTime.Today;

            // Set status
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedStatus)
            {
                payment.Status = selectedStatus.Content.ToString();
            }

            return payment;
        }
    }
}