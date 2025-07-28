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
    public partial class ReviewManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _reviewsView;
        private List<Review> _allReviews;

        public ReviewManagementPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);

            LoadReviews();
        }

        private void LoadReviews()
        {
            try
            {
                _allReviews = _context.Reviews
                    .Include(r => r.Customer)
                    .Include(r => r.Tour)
                    .ToList();

                _reviewsView = CollectionViewSource.GetDefaultView(_allReviews);
                ReviewsDataGrid.ItemsSource = _reviewsView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading reviews: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_reviewsView == null) return;

            _reviewsView.Filter = item =>
            {
                if (item is not Review review) return false;

                // Search filter
                var searchText = SearchTextBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = review.Customer?.FullName?.ToLower().Contains(searchText) == true ||
                                       review.Tour?.TourName?.ToLower().Contains(searchText) == true ||
                                       review.Comment?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // Rating filter
                var selectedRating = (RatingFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedRating) && selectedRating != "All Ratings")
                {
                    var rating = selectedRating.Replace(" Stars", "").Replace(" Star", "");
                    if (int.TryParse(rating, out int filterRating))
                    {
                        if (review.Rating != filterRating) return false;
                    }
                }

                return true;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void RatingFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ReviewsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void ReviewsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void AddReviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addReviewWindow = new AddReviewWindow(_activityLogService);
                if (addReviewWindow.ShowDialog() == true)
                {
                    LoadReviews(); // Refresh data
                    MessageBox.Show("Review added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add Review window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditReview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int reviewId)
                {
                    var review = _allReviews.FirstOrDefault(r => r.ReviewId == reviewId);
                    if (review != null)
                    {
                        var editReviewWindow = new EditReviewWindow(review, _activityLogService);
                        if (editReviewWindow.ShowDialog() == true)
                        {
                            LoadReviews(); // Refresh data
                            MessageBox.Show("Review updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteReview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int reviewId)
                {
                    var review = _allReviews.FirstOrDefault(r => r.ReviewId == reviewId);
                    if (review != null)
                    {
                        var result = MessageBox.Show(
                            $"Are you sure you want to delete the review from '{review.Customer?.FullName}'?",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            _context.Reviews.Remove(review);
                            _context.SaveChanges();

                            // Log activity
                            _activityLogService.LogActivity(1, "DELETE", "Review", reviewId, $"Deleted review from {review.Customer?.FullName}");

                            LoadReviews(); // Refresh data
                            MessageBox.Show("Review deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}