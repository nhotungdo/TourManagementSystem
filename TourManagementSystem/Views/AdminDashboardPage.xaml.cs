using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class AdminDashboardPage : Page
    {
        private readonly User _currentUser;
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;

        public AdminDashboardPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                // Load statistics
                LoadStatistics();

                // Load recent activity
                LoadRecentActivity();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStatistics()
        {
            // Total Users
            var totalUsers = _context.Users.Count();
            TotalUsersText.Text = totalUsers.ToString();

            // Active Tours
            var activeTours = _context.Tours.Count(t => t.IsActive == true);
            TotalToursText.Text = activeTours.ToString();

            // Total Bookings
            var totalBookings = _context.Bookings.Count();
            TotalBookingsText.Text = totalBookings.ToString();

            // Total Revenue
            var totalRevenue = _context.Bookings
                .Where(b => b.Status == "completed" || b.Status == "confirmed")
                .Sum(b => b.FinalPrice ?? b.TotalPrice);
            TotalRevenueText.Text = $"${totalRevenue:N0}";
        }

        private void LoadRecentActivity()
        {
            var recentActivities = _activityLogService.GetRecentActivityLogs(20);
            var activityItems = new List<ActivityItem>();

            foreach (var activity in recentActivities)
            {
                var icon = GetActivityIcon(activity.Action);
                var timeAgo = GetTimeAgo(activity.CreatedAt);

                activityItems.Add(new ActivityItem
                {
                    Icon = icon,
                    Description = activity.Description,
                    UserName = activity.User?.FullName ?? "Unknown",
                    TimeAgo = timeAgo
                });
            }

            ActivityList.ItemsSource = activityItems;
        }

        private string GetActivityIcon(string action)
        {
            return action.ToUpper() switch
            {
                "CREATE" => "➕",
                "UPDATE" => "✏️",
                "DELETE" => "🗑️",
                "LOGIN" => "🔐",
                "PAYMENT" => "💳",
                "BOOKING" => "📋",
                "STATUS_UPDATE" => "🔄",
                "ROLE_CHANGE" => "👤",
                _ => "📝"
            };
        }

        private string GetTimeAgo(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return "Unknown";

            var timeSpan = DateTime.Now - dateTime.Value;

            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays}d ago";
            else if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours}h ago";
            else if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes}m ago";
            else
                return "Just now";
        }

        // Quick Action Button Handlers
        private void AddTourButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = new Models.TourManagementContext();
                var activityLogService = new Services.ActivityLogService(context);
                var tourService = new Services.TourService(context, activityLogService);

                var addTourWindow = new AddTourWindow(tourService, activityLogService);
                if (addTourWindow.ShowDialog() == true)
                {
                    LoadDashboardData(); // Refresh data
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add Tour window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = new Models.TourManagementContext();
                var activityLogService = new Services.ActivityLogService(context);
                var userService = new Services.UserService(context, activityLogService);

                var addUserWindow = new AddUserWindow(userService, activityLogService);
                if (addUserWindow.ShowDialog() == true)
                {
                    LoadDashboardData(); // Refresh data
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add User window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = new Models.TourManagementContext();
                var activityLogService = new Services.ActivityLogService(context);
                var tourScheduleService = new Services.TourScheduleService(context, activityLogService);

                var addScheduleWindow = new AddScheduleWindow(tourScheduleService, activityLogService);
                if (addScheduleWindow.ShowDialog() == true)
                {
                    LoadDashboardData(); // Refresh data
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Create Schedule window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddPromotionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var context = new Models.TourManagementContext();
                var activityLogService = new Services.ActivityLogService(context);
                var promotionService = new Services.PromotionService(context, activityLogService);
                
                var addPromotionWindow = new AddPromotionWindow(promotionService, activityLogService);
                if (addPromotionWindow.ShowDialog() == true)
                {
                    LoadDashboardData(); // Refresh data
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add Promotion window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewReportsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("View Reports functionality will be implemented here.", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class ActivityItem
    {
        public string Icon { get; set; } = "";
        public string Description { get; set; } = "";
        public string UserName { get; set; } = "";
        public string TimeAgo { get; set; } = "";
    }
}