using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class MyBookingsPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentUser;
        private List<Booking> _allBookings;

        public MyBookingsPage(User user)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _currentUser = user;

            LoadBookings();
        }

        private void LoadBookings()
        {
            try
            {
                _allBookings = _context.Bookings
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Tour)
                    .Include(b => b.Schedule.Guide)
                    .Include(b => b.Payments)
                    .Where(b => b.CustomerId == _currentUser.UserId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                // Initialize filters if they haven't been set up yet
                InitializeFilters();

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bookings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _allBookings = new List<Booking>();
            }
        }

        private void InitializeFilters()
        {
            try
            {
                // Initialize status filter
                if (StatusFilterComboBox != null && StatusFilterComboBox.Items.Count == 0)
                {
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "All Bookings" });
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "Pending" });
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "Confirmed" });
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "Cancelled" });
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "Completed" });
                    StatusFilterComboBox.SelectedIndex = 0;
                }

                // Initialize payment filter
                if (PaymentFilterComboBox != null && PaymentFilterComboBox.Items.Count == 0)
                {
                    PaymentFilterComboBox.Items.Add(new ComboBoxItem { Content = "All Payments" });
                    PaymentFilterComboBox.Items.Add(new ComboBoxItem { Content = "Unpaid" });
                    PaymentFilterComboBox.Items.Add(new ComboBoxItem { Content = "Partial" });
                    PaymentFilterComboBox.Items.Add(new ComboBoxItem { Content = "Paid" });
                    PaymentFilterComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing filters: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (_allBookings == null)
                {
                    DisplayBookings(new List<Booking>());
                    return;
                }

                var filteredBookings = _allBookings.AsQueryable();

                // Apply status filter
                if (StatusFilterComboBox?.SelectedItem is ComboBoxItem statusItem)
                {
                    var statusText = statusItem.Content?.ToString();
                    if (!string.IsNullOrEmpty(statusText) && statusText != "All Bookings")
                    {
                        filteredBookings = filteredBookings.Where(b => (b.Status ?? "").ToLower() == statusText.ToLower());
                    }
                }

                // Apply payment filter
                if (PaymentFilterComboBox?.SelectedItem is ComboBoxItem paymentItem)
                {
                    var paymentText = paymentItem.Content?.ToString();
                    if (!string.IsNullOrEmpty(paymentText) && paymentText != "All Payments")
                    {
                        switch (paymentText)
                        {
                            case "Unpaid":
                                filteredBookings = filteredBookings.Where(b => !b.Payments.Any() || b.Payments.Sum(p => p.Amount) == 0);
                                break;
                            case "Partial":
                                filteredBookings = filteredBookings.Where(b => b.Payments.Any() && b.Payments.Sum(p => p.Amount) > 0 && b.Payments.Sum(p => p.Amount) < b.TotalPrice);
                                break;
                            case "Paid":
                                filteredBookings = filteredBookings.Where(b => b.Payments.Sum(p => p.Amount) >= b.TotalPrice);
                                break;
                        }
                    }
                }

                DisplayBookings(filteredBookings.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayBookings(List<Booking> bookings)
        {
            try
            {
                if (BookingsStackPanel == null)
                {
                    return;
                }

                BookingsStackPanel.Children.Clear();

                if (!bookings.Any())
                {
                    var noBookingsText = new TextBlock
                    {
                        Text = "No bookings found matching your criteria.",
                        FontSize = 16,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 50, 0, 0)
                    };
                    BookingsStackPanel.Children.Add(noBookingsText);
                    return;
                }

                foreach (var booking in bookings)
                {
                    var bookingCard = CreateBookingCard(booking);
                    BookingsStackPanel.Children.Add(bookingCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying bookings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateBookingCard(Booking booking)
        {
            var card = new Border
            {
                Style = FindResource("BookingCard") as Style
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Left side - Tour and Schedule info
            var leftPanel = new StackPanel();

            var tourName = new TextBlock
            {
                Text = booking.Schedule.Tour.TourName,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var destination = new TextBlock
            {
                Text = $"📍 {booking.Schedule.Tour.Destination}",
                FontSize = 14,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var schedule = new TextBlock
            {
                Text = $"📅 {booking.Schedule.DepartureDate:dd/MM/yyyy} - {booking.Schedule.ReturnDate:dd/MM/yyyy}",
                FontSize = 14,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var guide = new TextBlock
            {
                Text = $"👤 Guide: {booking.Schedule.Guide?.FullName ?? "TBD"}",
                FontSize = 14,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var people = new TextBlock
            {
                Text = $"👥 {booking.NumAdults} adults, {booking.NumChildren} children",
                FontSize = 14,
                Foreground = Brushes.Gray
            };

            leftPanel.Children.Add(tourName);
            leftPanel.Children.Add(destination);
            leftPanel.Children.Add(schedule);
            leftPanel.Children.Add(guide);
            leftPanel.Children.Add(people);

            // Center - Status and Payment info
            var centerPanel = new StackPanel();

            // Status badge
            var statusBadge = new Border
            {
                Style = FindResource("StatusBadge") as Style,
                Background = GetStatusColor(booking.Status),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var statusText = new TextBlock
            {
                Text = booking.Status.ToUpper(),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            statusBadge.Child = statusText;

            // Payment info
            var totalPaid = booking.Payments?.Sum(p => p.Amount) ?? 0;
            var paymentStatus = totalPaid >= booking.TotalPrice ? "Paid" :
                               totalPaid > 0 ? "Partial" : "Unpaid";

            var paymentText = new TextBlock
            {
                Text = $"💳 Payment: {paymentStatus}",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = GetPaymentStatusColor(paymentStatus),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var amountText = new TextBlock
            {
                Text = $"💰 Total: {booking.TotalPrice:C}",
                FontSize = 14,
                Foreground = Brushes.Black,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var paidText = new TextBlock
            {
                Text = $"💵 Paid: {totalPaid:C}",
                FontSize = 14,
                Foreground = Brushes.Green
            };

            centerPanel.Children.Add(statusBadge);
            centerPanel.Children.Add(paymentText);
            centerPanel.Children.Add(amountText);
            centerPanel.Children.Add(paidText);

            // Right side - Action buttons
            var rightPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            var viewDetailsButton = new Button
            {
                Content = "👁️ View Details",
                Style = FindResource("ActionButton") as Style,
                Margin = new Thickness(0, 0, 0, 5),
                Tag = booking
            };
            viewDetailsButton.Click += ViewDetailsButton_Click;

            var payButton = new Button
            {
                Content = "💳 Pay Now",
                Style = FindResource("ActionButton") as Style,
                Background = Brushes.Green,
                Margin = new Thickness(0, 0, 0, 5),
                Tag = booking
            };
            payButton.Click += PayButton_Click;

            // Only show pay button if not fully paid
            if (totalPaid < booking.TotalPrice)
            {
                rightPanel.Children.Add(payButton);
            }

            var cancelButton = new Button
            {
                Content = "❌ Cancel",
                Style = FindResource("ActionButton") as Style,
                Background = Brushes.Red,
                Tag = booking
            };
            cancelButton.Click += CancelButton_Click;

            // Only show cancel button if booking is pending
            if (booking.Status.ToLower() == "pending")
            {
                rightPanel.Children.Add(cancelButton);
            }

            rightPanel.Children.Add(viewDetailsButton);

            Grid.SetColumn(leftPanel, 0);
            Grid.SetColumn(centerPanel, 1);
            Grid.SetColumn(rightPanel, 2);

            grid.Children.Add(leftPanel);
            grid.Children.Add(centerPanel);
            grid.Children.Add(rightPanel);

            card.Child = grid;
            return card;
        }

        private Brush GetStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "pending" => Brushes.Orange,
                "confirmed" => Brushes.Green,
                "cancelled" => Brushes.Red,
                "completed" => Brushes.Blue,
                _ => Brushes.Gray
            };
        }

        private Brush GetPaymentStatusColor(string paymentStatus)
        {
            return paymentStatus switch
            {
                "Paid" => Brushes.Green,
                "Partial" => Brushes.Orange,
                "Unpaid" => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is Booking booking)
                {
                    var detailsWindow = new BookingDetailsWindow(booking);
                    detailsWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing booking details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is Booking booking)
                {
                    var paymentWindow = new PaymentWindow(booking, _currentUser);
                    paymentWindow.ShowDialog();

                    // Refresh bookings after payment
                    LoadBookings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is Booking booking)
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to cancel this booking?\n\n" +
                        $"Tour: {booking.Schedule.Tour.TourName}\n" +
                        $"Schedule: {booking.Schedule.DepartureDate:dd/MM/yyyy} - {booking.Schedule.ReturnDate:dd/MM/yyyy}",
                        "Cancel Booking",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        booking.Status = "cancelled";
                        _context.SaveChanges();

                        MessageBox.Show("Booking cancelled successfully.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadBookings();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cancelling booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void PaymentFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBookings();
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }

    // Booking Details Window
    public class BookingDetailsWindow : Window
    {
        public BookingDetailsWindow(Booking booking)
        {
            Title = $"Booking Details - {booking.Schedule.Tour.TourName}";
            Width = 600;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResize;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Header
            var header = new StackPanel
            {
                Background = Brushes.Orange,
                Margin = new Thickness(20)
            };

            var title = new TextBlock
            {
                Text = booking.Schedule.Tour.TourName,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };

            header.Children.Add(title);

            // Content
            var scrollViewer = new ScrollViewer();
            var content = new StackPanel { Margin = new Thickness(20) };

            // Booking Info
            var bookingInfo = new TextBlock
            {
                Text = $"Booking ID: {booking.BookingId}\n" +
                      $"Booking Date: {booking.BookingDate:dd/MM/yyyy HH:mm}\n" +
                      $"Status: {booking.Status}\n" +
                      $"Total Price: {booking.TotalPrice:C}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Tour Info
            var tourInfo = new TextBlock
            {
                Text = $"Tour: {booking.Schedule.Tour.TourName}\n" +
                      $"Destination: {booking.Schedule.Tour.Destination}\n" +
                      $"Duration: {booking.Schedule.Tour.DurationDays} days\n" +
                      $"Description: {booking.Schedule.Tour.Description}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Schedule Info
            var scheduleInfo = new TextBlock
            {
                Text = $"Departure: {booking.Schedule.DepartureDate:dd/MM/yyyy}\n" +
                      $"Return: {booking.Schedule.ReturnDate:dd/MM/yyyy}\n" +
                      $"Guide: {booking.Schedule.Guide?.FullName ?? "TBD"}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // People Info
            var peopleInfo = new TextBlock
            {
                Text = $"Adults: {booking.NumAdults}\n" +
                      $"Children: {booking.NumChildren}\n" +
                      $"Total People: {booking.NumAdults + booking.NumChildren}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Notes
            if (!string.IsNullOrWhiteSpace(booking.Notes))
            {
                var notesInfo = new TextBlock
                {
                    Text = $"Notes: {booking.Notes}",
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                content.Children.Add(notesInfo);
            }

            content.Children.Add(bookingInfo);
            content.Children.Add(tourInfo);
            content.Children.Add(scheduleInfo);
            content.Children.Add(peopleInfo);

            scrollViewer.Content = content;

            Grid.SetRow(header, 0);
            Grid.SetRow(scrollViewer, 1);
            grid.Children.Add(header);
            grid.Children.Add(scrollViewer);

            Content = grid;
        }
    }

    // Payment Window
    public class PaymentWindow : Window
    {
        public PaymentWindow(Booking booking, User currentUser)
        {
            Title = $"Payment - {booking.Schedule.Tour.TourName}";
            Width = 500;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResize;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header
            var header = new StackPanel
            {
                Background = Brushes.Green,
                Margin = new Thickness(20)
            };

            var title = new TextBlock
            {
                Text = "Payment",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };

            header.Children.Add(title);

            // Form
            var scrollViewer = new ScrollViewer();
            var form = new StackPanel { Margin = new Thickness(20) };

            // Payment Info
            var totalPaid = booking.Payments?.Sum(p => p.Amount) ?? 0;
            var remainingAmount = booking.TotalPrice - totalPaid;

            var paymentInfo = new TextBlock
            {
                Text = $"Total Price: {booking.TotalPrice:C}\n" +
                      $"Amount Paid: {totalPaid:C}\n" +
                      $"Remaining: {remainingAmount:C}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Payment Method
            var methodLabel = new TextBlock { Text = "Payment Method:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var methodComboBox = new ComboBox { Margin = new Thickness(0, 0, 0, 15) };
            methodComboBox.Items.Add("Cash");
            methodComboBox.Items.Add("Credit Card");
            methodComboBox.Items.Add("Bank Transfer");
            methodComboBox.Items.Add("E-Wallet");
            methodComboBox.SelectedIndex = 0;

            // Amount
            var amountLabel = new TextBlock { Text = "Amount to Pay:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var amountTextBox = new TextBox { Text = remainingAmount.ToString("F2"), Margin = new Thickness(0, 0, 0, 15) };

            form.Children.Add(paymentInfo);
            form.Children.Add(methodLabel);
            form.Children.Add(methodComboBox);
            form.Children.Add(amountLabel);
            form.Children.Add(amountTextBox);

            scrollViewer.Content = form;

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20)
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0)
            };
            cancelButton.Click += (s, e) => Close();

            var payButton = new Button
            {
                Content = "Pay",
                Width = 80,
                Height = 30,
                Background = Brushes.Green,
                Foreground = Brushes.White
            };
            payButton.Click += (s, e) =>
            {
                try
                {
                    if (decimal.TryParse(amountTextBox.Text, out decimal amount) && amount > 0)
                    {
                        using (var context = new TourManagementContext())
                        {
                            // Check if payment amount exceeds remaining amount
                            var totalPaid = context.Payments
                                .Where(p => p.BookingId == booking.BookingId)
                                .Sum(p => p.Amount);
                            var remainingAmount = booking.TotalPrice - totalPaid;

                            if (amount > remainingAmount)
                            {
                                MessageBox.Show($"Payment amount cannot exceed remaining amount: {remainingAmount:C}", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            // Get next payment ID
                            var maxPaymentId = context.Payments.Any() ? context.Payments.Max(p => p.PaymentId) : 0;
                            var nextPaymentId = maxPaymentId + 1;

                            var payment = new Payment
                            {
                                PaymentId = nextPaymentId,
                                BookingId = booking.BookingId,
                                Amount = amount,
                                PaymentMethod = methodComboBox.SelectedItem.ToString().ToLower().Replace(" ", "_"),
                                PaymentDate = DateTime.Now,
                                Status = "completed",
                                TransactionId = $"TXN{DateTime.Now:yyyyMMddHHmmss}",
                                ProcessedBy = currentUser.UserId
                            };

                            context.Payments.Add(payment);
                            context.SaveChanges();

                            // Update booking payment status if fully paid
                            if (totalPaid + amount >= booking.TotalPrice)
                            {
                                booking.PaymentStatus = "paid";
                                context.SaveChanges();
                            }
                        }

                        MessageBox.Show("Payment processed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid amount.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(payButton);

            Grid.SetRow(header, 0);
            Grid.SetRow(scrollViewer, 1);
            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(header);
            grid.Children.Add(scrollViewer);
            grid.Children.Add(buttonPanel);

            Content = grid;
        }
    }
}