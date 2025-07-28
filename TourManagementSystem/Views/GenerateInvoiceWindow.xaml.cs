using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class GenerateInvoiceWindow : Window
    {
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;
        private Booking _selectedBooking;

        public GenerateInvoiceWindow(ActivityLogService activityLogService)
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
                // Load all bookings for invoice generation (including those that might already have invoices for testing)
                var bookings = _context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Schedule)
                        .ThenInclude(s => s.Tour)
                    .Include(b => b.Invoices)
                    .Where(b => b.Status == "Confirmed" || b.Status == "confirmed" || b.Status == "pending")
                    .OrderBy(b => b.BookingDate)
                    .ToList();

                // For testing purposes, also include bookings that already have invoices
                // In production, you would want to filter out bookings that already have invoices
                // var bookings = _context.Bookings
                //     .Include(b => b.Customer)
                //     .Include(b => b.Schedule)
                //         .ThenInclude(s => s.Tour)
                //     .Include(b => b.Invoices)
                //     .Where(b => (b.Status == "Confirmed" || b.Status == "confirmed") && !b.Invoices.Any())
                //     .OrderBy(b => b.BookingDate)
                //     .ToList();

                // Create display items for bookings
                var bookingItems = bookings.Select(b => new
                {
                    BookingId = b.BookingId,
                    DisplayName = $"Booking #{b.BookingId} - {b.Customer.FullName} - {b.Schedule.Tour.TourName} - {b.TotalPrice:N0} VND - Status: {b.Status} - Has Invoice: {(b.Invoices.Any() ? "Yes" : "No")}",
                    Booking = b
                }).ToList();

                BookingComboBox.ItemsSource = bookingItems;
                BookingComboBox.DisplayMemberPath = "DisplayName";
                BookingComboBox.SelectedValuePath = "BookingId";

                // Debug: Show how many bookings were loaded
                if (!bookingItems.Any())
                {
                    MessageBox.Show("No bookings available for invoice generation. Please ensure there are confirmed bookings without existing invoices.", "No Bookings", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Set default dates
                IssueDatePicker.SelectedDate = DateTime.Today;
                DueDatePicker.SelectedDate = DateTime.Today.AddDays(7);

                // Auto-generate invoice number
                InvoiceNumberTextBox.Text = GenerateInvoiceNumber();

                // Clear customer and tour info initially
                CustomerTextBox.Text = "";
                TourTextBox.Text = "";
                AmountTextBox.Text = "";
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
                AmountTextBox.Text = $"{_selectedBooking.TotalPrice:N0} VND";
            }
            else
            {
                CustomerTextBox.Text = "";
                TourTextBox.Text = "";
                AmountTextBox.Text = "";
            }
        }

        private string GenerateInvoiceNumber()
        {
            var currentYear = DateTime.Now.Year;
            var maxInvoiceNumber = _context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith($"INV-{currentYear}-"))
                .Select(i => i.InvoiceNumber)
                .ToList()
                .Select(inv => int.TryParse(inv.Split('-').Last(), out int num) ? num : 0)
                .DefaultIfEmpty(0)
                .Max();

            return $"INV-{currentYear}-{(maxInvoiceNumber + 1):D3}";
        }

        private void GenerateInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var invoice = CreateInvoiceFromForm();

                    // Add to database
                    _context.Invoices.Add(invoice);
                    _context.SaveChanges();

                    // Log activity
                    _activityLogService.LogActivity(1, "CREATE", "Invoice", invoice.InvoiceId,
                        $"Generated invoice {invoice.InvoiceNumber} for booking #{_selectedBooking.BookingId}");

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            // Issue date validation
            if (IssueDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select an issue date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                IssueDatePicker.Focus();
                return false;
            }

            if (IssueDatePicker.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Issue date cannot be in the future.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                IssueDatePicker.Focus();
                return false;
            }

            // Due date validation
            if (DueDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select a due date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DueDatePicker.Focus();
                return false;
            }

            if (DueDatePicker.SelectedDate <= IssueDatePicker.SelectedDate)
            {
                MessageBox.Show("Due date must be after issue date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DueDatePicker.Focus();
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

        private Invoice CreateInvoiceFromForm()
        {
            var invoice = new Invoice();

            // Get next InvoiceId
            var maxInvoiceId = _context.Invoices.Max(i => (int?)i.InvoiceId) ?? 0;
            invoice.InvoiceId = maxInvoiceId + 1;

            // Set booking
            invoice.BookingId = _selectedBooking.BookingId;

            // Set invoice number
            invoice.InvoiceNumber = InvoiceNumberTextBox.Text;

            // Set amount
            invoice.TotalAmount = _selectedBooking.TotalPrice;
            invoice.FinalAmount = _selectedBooking.TotalPrice; // For now, final amount equals total amount

            // Set dates
            invoice.IssueDate = IssueDatePicker.SelectedDate ?? DateTime.Today;
            invoice.DueDate = DueDatePicker.SelectedDate ?? DateTime.Today.AddDays(7);

            // Set terms and conditions
            invoice.PaymentTerms = TermsTextBox.Text.Trim();

            // Set status
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedStatus)
            {
                invoice.Status = selectedStatus.Content.ToString();
            }

            // Set created by (admin user ID)
            invoice.CreatedBy = 1;

            return invoice;
        }
    }
}