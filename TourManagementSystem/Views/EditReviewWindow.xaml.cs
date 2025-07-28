using System;
using System.Windows;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditReviewWindow : Window
    {
        private readonly Review _review;
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public EditReviewWindow(Review review, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _review = review;
            _activityLogService = activityLogService;
            _context = new TourManagementContext();

            LoadReviewData();
        }

        private void LoadReviewData()
        {
            try
            {
                // Load review data into form
                CustomerTextBox.Text = _review.Customer?.FullName ?? "Unknown Customer";
                TourTextBox.Text = _review.Tour?.TourName ?? "Unknown Tour";
                CommentTextBox.Text = _review.Comment;
                ReviewDateTextBox.Text = _review.ReviewDate?.ToString("dd/MM/yyyy HH:mm") ?? "Unknown";

                // Set rating
                RatingComboBox.SelectedIndex = _review.Rating - 1; // Rating is 1-5, index is 0-4
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading review data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateReviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    UpdateReviewFromForm();

                    // Update in database
                    var reviewToUpdate = _context.Reviews.Find(_review.ReviewId);
                    if (reviewToUpdate != null)
                    {
                        reviewToUpdate.Rating = _review.Rating;
                        reviewToUpdate.Comment = _review.Comment;
                        reviewToUpdate.ReviewDate = DateTime.Now; // Update review date
                        _context.SaveChanges();

                        // Log activity
                        _activityLogService.LogActivity(1, "UPDATE", "Review", _review.ReviewId,
                            $"Updated review for tour: {_review.Tour?.TourName}");

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Review not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Rating validation
            if (RatingComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a rating.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                RatingComboBox.Focus();
                return false;
            }

            // Comment validation
            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))
            {
                MessageBox.Show("Comment is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CommentTextBox.Focus();
                return false;
            }

            if (CommentTextBox.Text.Length < 10)
            {
                MessageBox.Show("Comment must be at least 10 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CommentTextBox.Focus();
                return false;
            }

            if (CommentTextBox.Text.Length > 1000)
            {
                MessageBox.Show("Comment cannot exceed 1000 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CommentTextBox.Focus();
                return false;
            }

            return true;
        }

        private void UpdateReviewFromForm()
        {
            // Update review properties
            var selectedRating = RatingComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem;
            if (selectedRating?.Tag != null)
            {
                _review.Rating = byte.Parse(selectedRating.Tag.ToString());
            }

            _review.Comment = CommentTextBox.Text.Trim();
            _review.ReviewDate = DateTime.Now;
        }
    }
}