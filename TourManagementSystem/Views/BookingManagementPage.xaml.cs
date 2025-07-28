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
    public partial class BookingManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly BookingService _bookingService;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _bookingsView;
        private List<Booking> _allBookings;

        public BookingManagementPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                // Fallback: Create basic UI programmatically if XAML fails
                CreateBasicLayout();
            }

            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);
            var paymentService = new PaymentService(_context, _activityLogService);
            var invoiceService = new InvoiceService(_context, _activityLogService);
            _bookingService = new BookingService(_context, _activityLogService, paymentService, invoiceService);

            LoadBookings();
        }

        private void CreateBasicLayout()
        {
            // Create a basic layout if XAML initialization fails
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Header
            var header = new StackPanel
            {
                Background = System.Windows.Media.Brushes.Orange,
                Margin = new Thickness(20)
            };

            var title = new TextBlock
            {
                Text = "Booking Management",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White
            };

            header.Children.Add(title);

            // Content
            var content = new StackPanel { Margin = new Thickness(20) };
            var message = new TextBlock
            {
                Text = "Booking management interface loaded successfully.",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            content.Children.Add(message);

            Grid.SetRow(header, 0);
            Grid.SetRow(content, 1);
            grid.Children.Add(header);
            grid.Children.Add(content);

            Content = grid;
        }

        private void LoadBookings()
        {
            try
            {
                _allBookings = _context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Tour)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                _bookingsView = CollectionViewSource.GetDefaultView(_allBookings);

                // Safely set DataGrid ItemsSource
                var dataGrid = FindName("BookingsDataGrid") as DataGrid;
                if (dataGrid != null)
                {
                    dataGrid.ItemsSource = _bookingsView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bookings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_bookingsView == null) return;

            _bookingsView.Filter = item =>
            {
                if (item is not Booking booking) return false;

                // Search filter
                var searchTextBox = FindName("SearchTextBox") as TextBox;
                var searchText = searchTextBox?.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = (booking.Customer?.FullName?.ToLower().Contains(searchText) ?? false) ||
                                       (booking.Schedule?.Tour?.TourName?.ToLower().Contains(searchText) ?? false) ||
                                       (booking.Status?.ToLower().Contains(searchText) ?? false);
                    if (!matchesSearch) return false;
                }

                // Status filter
                var statusComboBox = FindName("StatusFilterComboBox") as ComboBox;
                var selectedStatus = (statusComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All Status")
                {
                    if (booking.Status != selectedStatus) return false;
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

        private void BookingsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Let the DataGrid handle sorting automatically
            // The CollectionView will handle the sorting based on the column clicked
        }

        private void BookingsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
            // This can be used to enable/disable buttons based on selection
            var dataGrid = FindName("BookingsDataGrid") as DataGrid;
            var selectedBooking = dataGrid?.SelectedItem as Booking;
            // Add any selection-based logic here if needed
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }

        private void AddBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addBookingWindow = new AddBookingWindow(_activityLogService);
                if (addBookingWindow.ShowDialog() == true)
                {
                    LoadBookings(); // Refresh data
                    MessageBox.Show("Booking created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add Booking window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditBooking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int bookingId)
                {
                    var booking = _allBookings.FirstOrDefault(b => b.BookingId == bookingId);
                    if (booking != null)
                    {
                        var editBookingWindow = new EditBookingWindow(booking, _bookingService, _activityLogService);
                        if (editBookingWindow.ShowDialog() == true)
                        {
                            LoadBookings(); // Refresh data
                            MessageBox.Show("Booking updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBooking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int bookingId)
                {
                    var booking = _allBookings.FirstOrDefault(b => b.BookingId == bookingId);
                    if (booking != null)
                    {
                        // Check if booking can be deleted
                        if (booking.Status == "confirmed" || booking.Status == "completed")
                        {
                            MessageBox.Show(
                                $"Cannot delete booking with status '{booking.Status}'. Only pending or cancelled bookings can be deleted.",
                                "Cannot Delete",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            return;
                        }

                        var result = MessageBox.Show(
                            $"Are you sure you want to delete the booking for '{booking.Customer?.FullName}'?",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            if (_bookingService.DeleteBooking(bookingId, 1)) // 1 is admin ID
                            {
                                LoadBookings(); // Refresh data
                                MessageBox.Show("Booking deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete booking. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}