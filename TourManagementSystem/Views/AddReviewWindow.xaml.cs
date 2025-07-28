using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class AddReviewWindow : Window
    {
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public AddReviewWindow(ActivityLogService activityLogService)
        {
            InitializeComponent();
            _activityLogService = activityLogService;
            _context = new TourManagementContext();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Load customers (users with role "customer")
                var customers = _context.Users
                    .Where(u => u.Role.ToLower() == "customer" && u.IsActive == true)
                    .OrderBy(u => u.FullName)
                    .ToList();
                CustomerComboBox.ItemsSource = customers;

                // Load tours (active tours)
                var tours = _context.Tours
                    .Where(t => t.IsActive == true)
                    .OrderBy(t => t.TourName)
                    .ToList();
                TourComboBox.ItemsSource = tours;

                // Set default date to today
                ReviewDatePicker.SelectedDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddReviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var review = CreateReviewFromForm();

                    // Add to database
                    _context.Reviews.Add(review);
                    _context.SaveChanges();

                    // Log activity
                    _activityLogService.LogActivity(1, "CREATE", "Review", review.ReviewId,
                        $"Added review for tour: {review.Tour?.TourName} by customer: {review.Customer?.FullName}");

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Customer validation
            if (CustomerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CustomerComboBox.Focus();
                return false;
            }

            // Tour validation
            if (TourComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a tour.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TourComboBox.Focus();
                return false;
            }

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

            // Review date validation
            if (ReviewDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select a review date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ReviewDatePicker.Focus();
                return false;
            }

            if (ReviewDatePicker.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("Review date cannot be in the future.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ReviewDatePicker.Focus();
                return false;
            }

            return true;
        }

        private Review CreateReviewFromForm()
        {
            var review = new Review();

            // Get next ReviewId
            var maxReviewId = _context.Reviews.Max(r => (int?)r.ReviewId) ?? 0;
            review.ReviewId = maxReviewId + 1;

            // Set customer
            if (CustomerComboBox.SelectedItem is User selectedCustomer)
            {
                review.CustomerId = selectedCustomer.UserId;
            }

            // Set tour
            if (TourComboBox.SelectedItem is Tour selectedTour)
            {
                review.TourId = selectedTour.TourId;
            }

            // Set booking (we'll use a default booking or find one for this customer and tour)
            // For now, we'll set it to 1 as a placeholder
            review.BookingId = 1;

            // Set rating
            if (RatingComboBox.SelectedItem is ComboBoxItem selectedRating)
            {
                var ratingText = selectedRating.Content.ToString();
                review.Rating = (byte)int.Parse(ratingText.Split(' ')[0]); // Extract number from "5 Stars"
            }

            // Set comment
            review.Comment = CommentTextBox.Text.Trim();

            // Set review date
            review.ReviewDate = ReviewDatePicker.SelectedDate ?? DateTime.Today;

            return review;
        }
    }
}