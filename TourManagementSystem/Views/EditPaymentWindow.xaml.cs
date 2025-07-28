using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditPaymentWindow : Window
    {
        private readonly TourManagementContext _context;
        private readonly PaymentService _paymentService;
        private readonly ActivityLogService _activityLogService;
        private readonly Payment _payment;
        private readonly decimal _bookingTotal;

        public EditPaymentWindow(Payment payment, PaymentService paymentService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _paymentService = paymentService;
            _activityLogService = activityLogService;
            _payment = payment;

            // Load booking total
            var booking = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Schedule.Tour)
                .FirstOrDefault(b => b.BookingId == payment.BookingId);
            _bookingTotal = booking?.TotalPrice ?? 0;

            LoadData();
            PopulateForm();
        }

        private void LoadData()
        {
            try
            {
                // Load booking information
                var booking = _context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Schedule.Tour)
                    .FirstOrDefault(b => b.BookingId == _payment.BookingId);

                if (booking != null)
                {
                    CustomerTextBox.Text = booking.Customer?.FullName ?? "Unknown";
                    TourTextBox.Text = booking.Schedule?.Tour?.TourName ?? "Unknown";
                    BookingTotalTextBox.Text = booking.TotalPrice.ToString("C");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateForm()
        {
            try
            {
                // Set amount
                AmountTextBox.Text = _payment.Amount.ToString();

                // Set payment method
                foreach (ComboBoxItem item in PaymentMethodComboBox.Items)
                {
                    var methodName = item.Content.ToString().ToLower().Replace(" ", "_");
                    if (methodName == (_payment.PaymentMethod ?? "").ToLower())
                    {
                        PaymentMethodComboBox.SelectedItem = item;
                        break;
                    }
                }

                // Set transaction ID
                TransactionIdTextBox.Text = _payment.TransactionId ?? "";

                // Set status
                foreach (ComboBoxItem item in StatusComboBox.Items)
                {
                    if (item.Content.ToString().ToLower() == (_payment.Status ?? "").ToLower())
                    {
                        StatusComboBox.SelectedItem = item;
                        break;
                    }
                }

                // Set payment date
                PaymentDatePicker.SelectedDate = _payment.PaymentDate;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating form: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (decimal.TryParse(AmountTextBox.Text, out decimal amount))
                {
                    // Check if amount exceeds booking total
                    if (amount > _bookingTotal)
                    {
                        AmountTextBox.Background = System.Windows.Media.Brushes.LightPink;
                    }
                    else
                    {
                        AmountTextBox.Background = System.Windows.Media.Brushes.White;
                    }
                }
            }
            catch (Exception ex)
            {
                // Silently handle calculation errors
                System.Diagnostics.Debug.WriteLine($"Error validating amount: {ex.Message}");
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

                // Update payment
                UpdatePaymentFromForm();

                // Save to database
                _context.SaveChanges();

                // Log activity
                _activityLogService.LogActivity(1, "UPDATE", "PAYMENT", _payment.PaymentId,
                    $"Updated payment #{_payment.PaymentId} for {_payment.Amount:C}");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            // Validate amount
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount (must be greater than 0).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AmountTextBox.Focus();
                return false;
            }

            // Check if amount exceeds booking total
            if (amount > _bookingTotal)
            {
                var result = MessageBox.Show(
                    $"Payment amount (${amount:N0}) exceeds booking total (${_bookingTotal:N0}). Do you want to continue?",
                    "Amount Exceeds Total",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    AmountTextBox.Focus();
                    return false;
                }
            }

            // Validate payment method
            if (PaymentMethodComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment method.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PaymentMethodComboBox.Focus();
                return false;
            }

            // Validate transaction ID
            if (string.IsNullOrWhiteSpace(TransactionIdTextBox.Text))
            {
                MessageBox.Show("Please enter a transaction ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TransactionIdTextBox.Focus();
                return false;
            }

            // Validate status
            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                StatusComboBox.Focus();
                return false;
            }

            // Validate payment date
            if (PaymentDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select a payment date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PaymentDatePicker.Focus();
                return false;
            }

            return true;
        }

        private void UpdatePaymentFromForm()
        {
            // Update amount
            _payment.Amount = decimal.Parse(AmountTextBox.Text);

            // Update payment method
            if (PaymentMethodComboBox.SelectedItem is ComboBoxItem methodItem)
            {
                _payment.PaymentMethod = methodItem.Content.ToString().ToLower().Replace(" ", "_");
            }

            // Update transaction ID
            _payment.TransactionId = TransactionIdTextBox.Text.Trim();

            // Update status
            if (StatusComboBox.SelectedItem is ComboBoxItem statusItem)
            {
                _payment.Status = statusItem.Content.ToString().ToLower();
            }

            // Update payment date
            _payment.PaymentDate = PaymentDatePicker.SelectedDate ?? DateTime.Now;

            // Update processed by (admin)
            _payment.ProcessedBy = 1;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}