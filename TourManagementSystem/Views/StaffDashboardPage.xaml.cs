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
    public partial class StaffDashboardPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;

        public StaffDashboardPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                LoadStatistics();
                LoadRecentActivities();
                LoadAlerts();
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
                var today = DateTime.Today;
                var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);

                // Today's Bookings
                var todayBookings = _context.Bookings.Count(b => b.BookingDate >= today);
                TodayBookingsText.Text = todayBookings.ToString();

                // Pending Payments
                var pendingPayments = _context.Payments.Count(p => p.Status == "pending");
                PendingPaymentsText.Text = pendingPayments.ToString();

                // Active Tours (tours that are currently running)
                var activeTours = _context.TourSchedules.Count(ts =>
                    ts.DepartureDate <= todayDateOnly &&
                    ts.ReturnDate >= todayDateOnly &&
                    ts.Status == "active");
                ActiveToursText.Text = activeTours.ToString();

                // New Reviews (reviews from last 7 days)
                var newReviews = _context.Reviews.Count(r =>
                    r.ReviewDate >= DateTime.Now.AddDays(-7));
                NewReviewsText.Text = newReviews.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecentActivities()
        {
            try
            {
                RecentActivitiesPanel.Children.Clear();

                var recentActivities = _context.ActivityLogs
                    .Include(al => al.User)
                    .OrderByDescending(al => al.CreatedAt)
                    .Take(10)
                    .ToList();

                foreach (var activity in recentActivities)
                {
                    var activityCard = CreateActivityCard(activity);
                    RecentActivitiesPanel.Children.Add(activityCard);
                }

                if (recentActivities.Count == 0)
                {
                    var noActivityText = new TextBlock
                    {
                        Text = "No recent activities",
                        Foreground = System.Windows.Media.Brushes.Gray,
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    RecentActivitiesPanel.Children.Add(noActivityText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent activities: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateActivityCard(ActivityLog activity)
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

            var timeText = new TextBlock
            {
                Text = activity.CreatedAt?.ToString("HH:mm") ?? "N/A",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Gray,
                FontWeight = FontWeights.SemiBold
            };

            var actionText = new TextBlock
            {
                Text = $"{activity.User?.FullName ?? "Unknown"} - {activity.Action} {activity.EntityType}",
                FontSize = 13,
                Foreground = System.Windows.Media.Brushes.Black,
                TextWrapping = TextWrapping.Wrap
            };

            var descriptionText = new TextBlock
            {
                Text = activity.Description,
                FontSize = 11,
                Foreground = System.Windows.Media.Brushes.DarkGray,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 2, 0, 0)
            };

            stackPanel.Children.Add(timeText);
            stackPanel.Children.Add(actionText);
            stackPanel.Children.Add(descriptionText);

            card.Child = stackPanel;
            return card;
        }

        private void LoadAlerts()
        {
            try
            {
                AlertsPanel.Children.Clear();

                var alerts = new List<string>();

                // Check for tours with low capacity
                var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
                var weekFromNow = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
                var lowCapacityTours = _context.TourSchedules
                    .Include(ts => ts.Tour)
                    .Where(ts => ts.DepartureDate >= todayDateOnly &&
                                ts.DepartureDate <= weekFromNow &&
                                ts.CurrentBookings >= ts.MaxCapacity * 0.8)
                    .ToList();

                foreach (var tour in lowCapacityTours)
                {
                    alerts.Add($"⚠️ Tour '{tour.Tour.TourName}' is nearly full ({tour.CurrentBookings}/{tour.MaxCapacity})");
                }

                // Check for pending payments
                var pendingPaymentCount = _context.Payments.Count(p => p.Status == "pending");
                if (pendingPaymentCount > 0)
                {
                    alerts.Add($"💳 {pendingPaymentCount} pending payments require attention");
                }

                // Check for new reviews
                var newReviewCount = _context.Reviews.Count(r => r.ReviewDate >= DateTime.Now.AddDays(-1));
                if (newReviewCount > 0)
                {
                    alerts.Add($"⭐ {newReviewCount} new reviews received in the last 24 hours");
                }

                // Check for tours starting today
                var todayTours = _context.TourSchedules
                    .Include(ts => ts.Tour)
                    .Count(ts => ts.DepartureDate == todayDateOnly);
                if (todayTours > 0)
                {
                    alerts.Add($"🗺️ {todayTours} tours are departing today");
                }

                // Display alerts
                foreach (var alert in alerts)
                {
                    var alertCard = CreateAlertCard(alert);
                    AlertsPanel.Children.Add(alertCard);
                }

                if (alerts.Count == 0)
                {
                    var noAlertsText = new TextBlock
                    {
                        Text = "No important alerts at this time",
                        Foreground = System.Windows.Media.Brushes.Green,
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    AlertsPanel.Children.Add(noAlertsText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading alerts: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateAlertCard(string alertMessage)
        {
            var card = new Border
            {
                Background = System.Windows.Media.Brushes.Orange,
                BorderBrush = System.Windows.Media.Brushes.DarkOrange,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var textBlock = new TextBlock
            {
                Text = alertMessage,
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.White,
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
                var weekFromNow = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
                var upcomingTours = _context.TourSchedules
                    .Include(ts => ts.Tour)
                    .Include(ts => ts.Guide)
                    .Where(ts => ts.DepartureDate >= todayDateOnly &&
                                ts.DepartureDate <= weekFromNow)
                    .OrderBy(ts => ts.DepartureDate)
                    .ToList();

                UpcomingToursDataGrid.ItemsSource = upcomingTours;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading upcoming tours: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addBookingWindow = new AddBookingWindow(_activityLogService);
                if (addBookingWindow.ShowDialog() == true)
                {
                    // Refresh dashboard data
                    LoadDashboardData();
                    MessageBox.Show("Booking created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening new booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var processPaymentWindow = new ProcessPaymentWindow(_activityLogService);
                if (processPaymentWindow.ShowDialog() == true)
                {
                    // Refresh dashboard data
                    LoadDashboardData();
                    MessageBox.Show("Payment processed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateInvoiceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var generateInvoiceWindow = new GenerateInvoiceWindow(_activityLogService);
                if (generateInvoiceWindow.ShowDialog() == true)
                {
                    // Refresh dashboard data
                    LoadDashboardData();
                    MessageBox.Show("Invoice generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReviewResponseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Review Response functionality will be implemented soon.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navigate to review management page
                // var reviewPage = new ReviewManagementPage();
                // NavigationService.Navigate(reviewPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error responding to review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}