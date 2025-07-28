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
    public partial class PaymentManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly PaymentService _paymentService;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _paymentsView;
        private List<Payment> _allPayments;

        private readonly User _currentUser;

        public PaymentManagementPage(User user = null)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);
            _paymentService = new PaymentService(_context, _activityLogService);
            _currentUser = user;

            LoadPayments();
        }

        private void LoadPayments()
        {
            try
            {
                IQueryable<Payment> query = _context.Payments
                    .Include(p => p.Booking)
                    .Include(p => p.Booking.Customer);

                // If current user is provided, filter payments for that user only
                if (_currentUser != null)
                {
                    query = query.Where(p => p.Booking.CustomerId == _currentUser.UserId);
                }

                _allPayments = query.ToList();

                var paymentsDataGrid = FindName("PaymentsDataGrid") as DataGrid;
                if (paymentsDataGrid != null)
                {
                    _paymentsView = CollectionViewSource.GetDefaultView(_allPayments);
                    paymentsDataGrid.ItemsSource = _paymentsView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_paymentsView == null) return;

            _paymentsView.Filter = item =>
            {
                if (item is not Payment payment) return false;

                // Search filter
                var searchTextBox = FindName("SearchTextBox") as TextBox;
                var searchText = searchTextBox?.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = payment.Booking?.Customer?.FullName?.ToLower().Contains(searchText) == true ||
                                       payment.TransactionId?.ToLower().Contains(searchText) == true ||
                                       payment.Status?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // Status filter
                var statusComboBox = FindName("StatusFilterComboBox") as ComboBox;
                var selectedStatus = (statusComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All Status")
                {
                    if (payment.Status != selectedStatus) return false;
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

        private void PaymentsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void PaymentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void ProcessPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var processPaymentWindow = new ProcessPaymentWindow(_activityLogService);
                if (processPaymentWindow.ShowDialog() == true)
                {
                    LoadPayments(); // Refresh data
                    MessageBox.Show("Payment processed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Process Payment window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int paymentId)
                {
                    var payment = _allPayments.FirstOrDefault(p => p.PaymentId == paymentId);
                    if (payment != null)
                    {
                        var editPaymentWindow = new EditPaymentWindow(payment, _paymentService, _activityLogService);
                        if (editPaymentWindow.ShowDialog() == true)
                        {
                            LoadPayments(); // Refresh data
                            MessageBox.Show("Payment updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeletePayment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int paymentId)
                {
                    var payment = _allPayments.FirstOrDefault(p => p.PaymentId == paymentId);
                    if (payment != null)
                    {
                        var result = MessageBox.Show(
                            $"Are you sure you want to delete the payment for '{payment.Booking?.Customer?.FullName}'?",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            try
                            {
                                _context.Payments.Remove(payment);
                                _context.SaveChanges();

                                // Log activity
                                _activityLogService.LogActivity(1, "DELETE", "Payment", paymentId,
                                    $"Deleted payment for {payment.Booking?.Customer?.FullName}");

                                LoadPayments(); // Refresh data
                                MessageBox.Show("Payment deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to delete payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}