using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class CustomerDashboardPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentCustomer;

        public CustomerDashboardPage(User customer)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _currentCustomer = customer;

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                LoadStatistics();
                LoadRecentBookings();
                LoadNotifications();
                LoadUpcomingTours();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStatistics()
        {
            try
            {
                // Total Bookings
                var totalBookings = _context.Bookings.Count(b => b.CustomerId == _currentCustomer.UserId);
                TotalBookingsText.Text = totalBookings.ToString();

                // Upcoming Tours (tours that are scheduled in the future)
                var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
                var upcomingTours = _context.Bookings
                    .Include(b => b.Schedule)
                    .Where(b => b.CustomerId == _currentCustomer.UserId &&
                               b.Schedule.DepartureDate >= todayDateOnly)
                    .Count();
                UpcomingToursText.Text = upcomingTours.ToString();

                // Pending Payments
                var pendingPayments = _context.Payments
                    .Count(p => p.Booking.CustomerId == _currentCustomer.UserId &&
                               p.Status == "pending");
                PendingPaymentsText.Text = pendingPayments.ToString();

                // My Reviews
                var myReviews = _context.Reviews
                    .Count(r => r.Booking.CustomerId == _currentCustomer.UserId);
                MyReviewsText.Text = myReviews.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecentBookings()
        {
            try
            {
                RecentBookingsPanel.Children.Clear();

                var recentBookings = _context.Bookings
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Tour)
                    .Where(b => b.CustomerId == _currentCustomer.UserId)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(5)
                    .ToList();

                foreach (var booking in recentBookings)
                {
                    var bookingCard = CreateBookingCard(booking);
                    RecentBookingsPanel.Children.Add(bookingCard);
                }

                if (recentBookings.Count == 0)
                {
                    var noBookingsText = new TextBlock
                    {
                        Text = "No bookings found",
                        Foreground = System.Windows.Media.Brushes.Gray,
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    RecentBookingsPanel.Children.Add(noBookingsText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent bookings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateBookingCard(Booking booking)
        {
            var card = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var stackPanel = new StackPanel();

            var tourNameText = new TextBlock
            {
                Text = booking.Schedule.Tour.TourName,
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Black,
                FontWeight = FontWeights.SemiBold
            };

            var dateText = new TextBlock
            {
                Text = $"Departure: {booking.Schedule.DepartureDate:dd/MM/yyyy}",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 2, 0, 0)
            };

            var statusText = new TextBlock
            {
                Text = $"Status: {booking.Status}",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.DarkGray,
                Margin = new Thickness(0, 2, 0, 0)
            };

            var priceText = new TextBlock
            {
                Text = $"Price: {booking.TotalPrice:C}",
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.DarkGray,
                Margin = new Thickness(0, 2, 0, 0)
            };

            stackPanel.Children.Add(tourNameText);
            stackPanel.Children.Add(dateText);
            stackPanel.Children.Add(statusText);
            stackPanel.Children.Add(priceText);

            card.Child = stackPanel;
            return card;
        }

        private void LoadNotifications()
        {
            try
            {
                NotificationsPanel.Children.Clear();

                var notifications = new List<string>();

                // Check for upcoming tours (within 7 days)
                var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
                var weekFromNow = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
                var upcomingTours = _context.Bookings
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Tour)
                    .Where(b => b.CustomerId == _currentCustomer.UserId &&
                               b.Schedule.DepartureDate >= todayDateOnly &&
                               b.Schedule.DepartureDate <= weekFromNow)
                    .ToList();

                foreach (var booking in upcomingTours)
                {
                    var daysUntilDeparture = (booking.Schedule.DepartureDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;
                    notifications.Add($"🗺️ Tour '{booking.Schedule.Tour.TourName}' departs in {daysUntilDeparture} days");
                }

                // Check for pending payments
                var pendingPayments = _context.Payments
                    .Include(p => p.Booking)
                    .Include(p => p.Booking.Schedule)
                    .Include(p => p.Booking.Schedule.Tour)
                    .Where(p => p.Booking.CustomerId == _currentCustomer.UserId &&
                               p.Status == "pending")
                    .ToList();

                foreach (var payment in pendingPayments)
                {
                    notifications.Add($"💳 Payment pending for '{payment.Booking.Schedule.Tour.TourName}' - {payment.Amount:C}");
                }

                // Check for tours starting today
                var todayTours = _context.Bookings
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Tour)
                    .Count(b => b.CustomerId == _currentCustomer.UserId &&
                               b.Schedule.DepartureDate == todayDateOnly);
                if (todayTours > 0)
                {
                    notifications.Add($"🎉 {todayTours} tour(s) departing today! Have a great trip!");
                }

                // Check for completed tours without reviews
                var completedToursWithoutReviews = _context.Bookings
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Tour)
                    .Where(b => b.CustomerId == _currentCustomer.UserId &&
                               b.Schedule.ReturnDate < todayDateOnly &&
                               b.Status == "completed" &&
                               !b.Reviews.Any())
                    .Take(3)
                    .ToList();

                foreach (var booking in completedToursWithoutReviews)
                {
                    notifications.Add($"⭐ Don't forget to review your experience with '{booking.Schedule.Tour.TourName}'");
                }

                // Display notifications
                foreach (var notification in notifications)
                {
                    var notificationCard = CreateNotificationCard(notification);
                    NotificationsPanel.Children.Add(notificationCard);
                }

                if (notifications.Count == 0)
                {
                    var noNotificationsText = new TextBlock
                    {
                        Text = "No notifications at this time",
                        Foreground = System.Windows.Media.Brushes.Green,
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    NotificationsPanel.Children.Add(noNotificationsText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading notifications: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateNotificationCard(string notificationMessage)
        {
            var card = new Border
            {
                Background = System.Windows.Media.Brushes.LightBlue,
                BorderBrush = System.Windows.Media.Brushes.Blue,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var textBlock = new TextBlock
            {
                Text = notificationMessage,
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.DarkBlue,
                TextWrapping = TextWrapping.Wrap,
                FontWeight = FontWeights.SemiBold
            };

            card.Child = textBlock;
            return card;
        }

        private void LoadUpcomingTours()
        {
            try
            {
                var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
                var upcomingTours = _context.Bookings
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Tour)
                    .Include(b => b.Schedule.Guide)
                    .Where(b => b.CustomerId == _currentCustomer.UserId &&
                               b.Schedule.DepartureDate >= todayDateOnly)
                    .OrderBy(b => b.Schedule.DepartureDate)
                    .Select(b => b.Schedule)
                    .ToList();

                UpcomingToursDataGrid.ItemsSource = upcomingTours;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading upcoming tours: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseToursButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Browse Tours functionality will be implemented soon.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navigate to browse tours page
                // var browseToursPage = new BrowseToursPage();
                // NavigationService.Navigate(browseToursPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening browse tours: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("New Booking functionality will be implemented soon.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navigate to new booking page
                // var newBookingPage = new NewBookingPage();
                // NavigationService.Navigate(newBookingPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening new booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MakePaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Make Payment functionality will be implemented soon.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navigate to payment page
                // var paymentPage = new PaymentPage();
                // NavigationService.Navigate(paymentPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WriteReviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Write Review functionality will be implemented soon.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navigate to review page
                // var reviewPage = new ReviewPage();
                // NavigationService.Navigate(reviewPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}