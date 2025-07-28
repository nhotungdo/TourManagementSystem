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
    public partial class InvoiceManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly InvoiceService _invoiceService;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _invoicesView;
        private List<Invoice> _allInvoices;

        public InvoiceManagementPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);
            _invoiceService = new InvoiceService(_context, _activityLogService);

            LoadInvoices();
        }

        private void LoadInvoices()
        {
            try
            {
                _allInvoices = _context.Invoices
                    .Include(i => i.Booking)
                    .Include(i => i.Booking.Customer)
                    .Include(i => i.Booking.Schedule)
                    .Include(i => i.Booking.Schedule.Tour)
                    .ToList();

                _invoicesView = CollectionViewSource.GetDefaultView(_allInvoices);
                InvoicesDataGrid.ItemsSource = _invoicesView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading invoices: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_invoicesView == null) return;

            _invoicesView.Filter = item =>
            {
                if (item is not Invoice invoice) return false;

                // Search filter
                var searchText = SearchTextBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = invoice.InvoiceNumber?.ToLower().Contains(searchText) == true ||
                                       invoice.Booking?.Customer?.FullName?.ToLower().Contains(searchText) == true ||
                                       invoice.Status?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // Status filter
                var selectedStatus = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All Status")
                {
                    if (invoice.Status != selectedStatus) return false;
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

        private void InvoicesDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void InvoicesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void GenerateInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var generateInvoiceWindow = new GenerateInvoiceWindow(_activityLogService);
                if (generateInvoiceWindow.ShowDialog() == true)
                {
                    LoadInvoices(); // Refresh data
                    MessageBox.Show("Invoice generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Generate Invoice window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int invoiceId)
                {
                    var invoice = _allInvoices.FirstOrDefault(i => i.InvoiceId == invoiceId);
                    if (invoice != null)
                    {
                        var editInvoiceWindow = new EditInvoiceWindow(invoice, _context, _activityLogService);
                        if (editInvoiceWindow.ShowDialog() == true)
                        {
                            LoadInvoices(); // Refresh data
                            MessageBox.Show("Invoice updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int invoiceId)
                {
                    var invoice = _allInvoices.FirstOrDefault(i => i.InvoiceId == invoiceId);
                    if (invoice != null)
                    {
                        var result = MessageBox.Show(
                            $"Are you sure you want to delete the invoice '{invoice.InvoiceNumber}'?",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                _context.Invoices.Remove(invoice);
                                _context.SaveChanges();

                                // Log activity
                                _activityLogService.LogActivity(1, "DELETE", "Invoice", invoiceId,
                                    $"Deleted invoice {invoice.InvoiceNumber}");

                                LoadInvoices(); // Refresh data
                                MessageBox.Show("Invoice deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to delete invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}