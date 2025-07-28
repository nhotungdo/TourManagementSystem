using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class NotificationsPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentUser;

        public NotificationsPage(User user)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _currentUser = user;

            LoadNotifications();
        }

        private void LoadNotifications()
        {
            try
            {
                var notifications = _context.Notifications
                    .Where(n => n.UserId == _currentUser.UserId)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList();

                DisplayNotifications(notifications);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading notifications: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayNotifications(System.Collections.Generic.List<Notification> notifications)
        {
            try
            {
                NotificationsStackPanel.Children.Clear();

                if (!notifications.Any())
                {
                    var noNotificationsText = new TextBlock
                    {
                        Text = "No notifications found.",
                        FontSize = 16,
                        Foreground = System.Windows.Media.Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 50, 0, 0)
                    };
                    NotificationsStackPanel.Children.Add(noNotificationsText);
                    return;
                }

                foreach (var notification in notifications)
                {
                    var notificationCard = CreateNotificationCard(notification);
                    NotificationsStackPanel.Children.Add(notificationCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying notifications: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateNotificationCard(Notification notification)
        {
            var card = new Border
            {
                Background = notification.IsRead == true ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.LightBlue,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var stackPanel = new StackPanel();

            var title = new TextBlock
            {
                Text = notification.Title,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var message = new TextBlock
            {
                Text = notification.Message,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var date = new TextBlock
            {
                Text = $"Date: {notification.CreatedAt:dd/MM/yyyy HH:mm}",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Gray
            };

            stackPanel.Children.Add(title);
            stackPanel.Children.Add(message);
            stackPanel.Children.Add(date);

            card.Child = stackPanel;
            return card;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}