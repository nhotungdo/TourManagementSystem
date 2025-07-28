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
    public partial class EditInvoiceWindow : Window
    {
        private readonly Invoice _invoice;
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public EditInvoiceWindow(Invoice invoice, TourManagementContext context, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _invoice = invoice;
            _context = context;
            _activityLogService = activityLogService;
            LoadInvoiceData();
        }

        private void LoadInvoiceData()
        {
            try
            {
                // Load invoice with related data
                var invoiceWithData = _context.Invoices
                    .Include(i => i.Booking)
                        .ThenInclude(b => b.Customer)
                    .Include(i => i.Booking)
                        .ThenInclude(b => b.Schedule)
                            .ThenInclude(s => s.Tour)
                    .FirstOrDefault(i => i.InvoiceId == _invoice.InvoiceId);

                if (invoiceWithData != null)
                {
                    // Set read-only fields
                    InvoiceNumberTextBox.Text = invoiceWithData.InvoiceNumber;
                    CustomerTextBox.Text = invoiceWithData.Booking.Customer.FullName;
                    TourTextBox.Text = $"{invoiceWithData.Booking.Schedule.Tour.TourName} - {invoiceWithData.Booking.Schedule.DepartureDate:dd/MM/yyyy}";
                    TotalAmountTextBox.Text = $"{invoiceWithData.TotalAmount:N0} VND";

                    // Set editable fields
                    TaxAmountTextBox.Text = (invoiceWithData.TaxAmount ?? 0).ToString();
                    DiscountAmountTextBox.Text = (invoiceWithData.DiscountAmount ?? 0).ToString();
                    FinalAmountTextBox.Text = $"{invoiceWithData.FinalAmount:N0} VND";

                    // Set dates
                    IssueDatePicker.SelectedDate = invoiceWithData.IssueDate ?? DateTime.Today;
                    DueDatePicker.SelectedDate = invoiceWithData.DueDate;

                    // Set payment terms and notes
                    PaymentTermsTextBox.Text = invoiceWithData.PaymentTerms ?? "Payment is due within 7 days of invoice date. Late payments may incur additional charges.";
                    NotesTextBox.Text = invoiceWithData.Notes ?? "";

                    // Set status
                    foreach (ComboBoxItem item in StatusComboBox.Items)
                    {
                        if (item.Content.ToString() == invoiceWithData.Status)
                        {
                            StatusComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading invoice data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    UpdateInvoiceFromForm();

                    // Save to database
                    _context.SaveChanges();

                    // Log activity
                    _activityLogService.LogActivity(1, "UPDATE", "Invoice", _invoice.InvoiceId,
                        $"Updated invoice {_invoice.InvoiceNumber}");

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private bool ValidateForm()
        {
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

            // Tax amount validation
            if (!decimal.TryParse(TaxAmountTextBox.Text, out decimal taxAmount) || taxAmount < 0)
            {
                MessageBox.Show("Please enter a valid tax amount (must be 0 or greater).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TaxAmountTextBox.Focus();
                return false;
            }

            // Discount amount validation
            if (!decimal.TryParse(DiscountAmountTextBox.Text, out decimal discountAmount) || discountAmount < 0)
            {
                MessageBox.Show("Please enter a valid discount amount (must be 0 or greater).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DiscountAmountTextBox.Focus();
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

        private void UpdateInvoiceFromForm()
        {
            // Update tax and discount amounts
            _invoice.TaxAmount = decimal.Parse(TaxAmountTextBox.Text);
            _invoice.DiscountAmount = decimal.Parse(DiscountAmountTextBox.Text);

            // Recalculate final amount
            _invoice.FinalAmount = _invoice.TotalAmount + (_invoice.TaxAmount ?? 0) - (_invoice.DiscountAmount ?? 0);

            // Update dates
            _invoice.IssueDate = IssueDatePicker.SelectedDate ?? DateTime.Today;
            _invoice.DueDate = DueDatePicker.SelectedDate ?? DateTime.Today.AddDays(7);

            // Update payment terms and notes
            _invoice.PaymentTerms = PaymentTermsTextBox.Text.Trim();
            _invoice.Notes = string.IsNullOrWhiteSpace(NotesTextBox.Text) ? null : NotesTextBox.Text.Trim();

            // Update status
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedStatus)
            {
                _invoice.Status = selectedStatus.Content.ToString();
            }

            // Update final amount display
            FinalAmountTextBox.Text = $"{_invoice.FinalAmount:N0} VND";
        }
    }
}