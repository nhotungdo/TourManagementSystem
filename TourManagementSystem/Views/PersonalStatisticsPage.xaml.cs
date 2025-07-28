using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class PersonalStatisticsPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentUser;

        public PersonalStatisticsPage(User user)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _currentUser = user;

            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                // Load basic statistics
                var totalBookings = _context.Bookings.Count(b => b.CustomerId == _currentUser.UserId);
                var totalSpent = _context.Bookings
                    .Where(b => b.CustomerId == _currentUser.UserId)
                    .Sum(b => b.TotalPrice);
                var reviewsWritten = _context.Reviews.Count(r => r.CustomerId == _currentUser.UserId);
                var promotionsUsed = _context.Bookings
                    .Where(b => b.CustomerId == _currentUser.UserId)
                    .Count(b => b.TotalPrice < b.Schedule.Tour.BasePrice * (b.NumAdults + b.NumChildren));

                // Update UI
                TotalBookingsText.Text = totalBookings.ToString();
                TotalSpentText.Text = totalSpent.ToString("C");
                ReviewsWrittenText.Text = reviewsWritten.ToString();
                PromotionsUsedText.Text = promotionsUsed.ToString();

                // Load detailed statistics
                LoadDetailedStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDetailedStatistics()
        {
            try
            {
                DetailsStackPanel.Children.Clear();

                // Recent bookings
                var recentBookings = _context.Bookings
                    .Include(b => b.Schedule.Tour)
                    .Where(b => b.CustomerId == _currentUser.UserId)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(5)
                    .ToList();

                if (recentBookings.Any())
                {
                    var recentBookingsHeader = new TextBlock
                    {
                        Text = "Recent Bookings",
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 0, 15)
                    };
                    DetailsStackPanel.Children.Add(recentBookingsHeader);

                    foreach (var booking in recentBookings)
                    {
                        var bookingCard = CreateBookingCard(booking);
                        DetailsStackPanel.Children.Add(bookingCard);
                    }
                }

                // Favorite destinations
                var favoriteDestinations = _context.Bookings
                    .Include(b => b.Schedule.Tour)
                    .Where(b => b.CustomerId == _currentUser.UserId)
                    .GroupBy(b => b.Schedule.Tour.Destination)
                    .Select(g => new { Destination = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(3)
                    .ToList();

                if (favoriteDestinations.Any())
                {
                    var destinationsHeader = new TextBlock
                    {
                        Text = "Favorite Destinations",
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 30, 0, 15)
                    };
                    DetailsStackPanel.Children.Add(destinationsHeader);

                    foreach (var dest in favoriteDestinations)
                    {
                        var destCard = CreateDestinationCard(dest.Destination, dest.Count);
                        DetailsStackPanel.Children.Add(destCard);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading detailed statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateBookingCard(Booking booking)
        {
            var card = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var stackPanel = new StackPanel();

            var tourName = new TextBlock
            {
                Text = booking.Schedule.Tour.TourName,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var details = new TextBlock
            {
                Text = $"{booking.Schedule.DepartureDate:dd/MM/yyyy} - {booking.Schedule.ReturnDate:dd/MM/yyyy} | {booking.TotalPrice:C}",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray
            };

            stackPanel.Children.Add(tourName);
            stackPanel.Children.Add(details);

            card.Child = stackPanel;
            return card;
        }

        private Border CreateDestinationCard(string destination, int count)
        {
            var card = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var stackPanel = new StackPanel();

            var destName = new TextBlock
            {
                Text = destination,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var visitCount = new TextBlock
            {
                Text = $"Visited {count} time{(count > 1 ? "s" : "")}",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray
            };

            stackPanel.Children.Add(destName);
            stackPanel.Children.Add(visitCount);

            card.Child = stackPanel;
            return card;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}