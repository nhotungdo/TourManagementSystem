using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class MyReviewsPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentUser;

        public MyReviewsPage(User user)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _currentUser = user;

            LoadReviews();
        }

        private void LoadReviews()
        {
            try
            {
                var reviews = _context.Reviews
                    .Include(r => r.Tour)
                    .Include(r => r.Booking)
                    .Where(r => r.CustomerId == _currentUser.UserId)
                    .OrderByDescending(r => r.ReviewDate)
                    .ToList();

                DisplayReviews(reviews);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading reviews: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayReviews(System.Collections.Generic.List<Review> reviews)
        {
            try
            {
                ReviewsStackPanel.Children.Clear();

                if (!reviews.Any())
                {
                    var noReviewsText = new TextBlock
                    {
                        Text = "You haven't written any reviews yet.",
                        FontSize = 16,
                        Foreground = System.Windows.Media.Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 50, 0, 0)
                    };
                    ReviewsStackPanel.Children.Add(noReviewsText);
                    return;
                }

                foreach (var review in reviews)
                {
                    var reviewCard = CreateReviewCard(review);
                    ReviewsStackPanel.Children.Add(reviewCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying reviews: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateReviewCard(Review review)
        {
            var card = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var stackPanel = new StackPanel();

            var tourName = new TextBlock
            {
                Text = review.Tour.TourName,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var rating = new TextBlock
            {
                Text = $"Rating: {new string('⭐', review.Rating)}",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var comment = new TextBlock
            {
                Text = review.Comment,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var date = new TextBlock
            {
                Text = $"Reviewed on: {review.ReviewDate:dd/MM/yyyy}",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Gray
            };

            stackPanel.Children.Add(tourName);
            stackPanel.Children.Add(rating);
            stackPanel.Children.Add(comment);
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