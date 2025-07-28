using System;
using System.Windows;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class AddTourWindow : Window
    {
        private readonly TourService _tourService;
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public AddTourWindow(TourService tourService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _tourService = tourService;
            _activityLogService = activityLogService;
            _context = new TourManagementContext();
        }

        private void AddTourButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var tour = CreateTourFromForm();

                    _context.Tours.Add(tour);
                    _context.SaveChanges();
                    
                    // Log activity
                    _activityLogService.LogActivity(1, "CREATE", "Tour", tour.TourId, $"Created tour: {tour.TourName}");
                    
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Tour name validation
            if (string.IsNullOrWhiteSpace(TourNameTextBox.Text))
            {
                MessageBox.Show("Tour name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TourNameTextBox.Focus();
                return false;
            }

            // Description validation
            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                MessageBox.Show("Description is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DescriptionTextBox.Focus();
                return false;
            }

            // Duration validation
            if (string.IsNullOrWhiteSpace(DurationTextBox.Text))
            {
                MessageBox.Show("Duration is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DurationTextBox.Focus();
                return false;
            }

            if (!int.TryParse(DurationTextBox.Text, out int duration) || duration <= 0)
            {
                MessageBox.Show("Duration must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DurationTextBox.Focus();
                return false;
            }

            // Price validation
            if (string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                MessageBox.Show("Price is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return false;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Price must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return false;
            }

            // Destination validation
            if (string.IsNullOrWhiteSpace(DestinationTextBox.Text))
            {
                MessageBox.Show("Destination is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DestinationTextBox.Focus();
                return false;
            }

            return true;
        }

        private Tour CreateTourFromForm()
        {
            var tour = new Tour
            {
                TourName = TourNameTextBox.Text.Trim(),
                Description = DescriptionTextBox.Text.Trim(),
                DurationDays = int.Parse(DurationTextBox.Text),
                DurationNights = int.Parse(DurationTextBox.Text) - 1, // Assuming nights = days - 1
                DepartureLocation = "TP.HCM", // Default departure location
                Destination = string.IsNullOrWhiteSpace(DestinationTextBox.Text) ? "Unknown" : DestinationTextBox.Text.Trim(),
                BasePrice = decimal.Parse(PriceTextBox.Text),
                CreatedBy = 1, // Admin ID
                IsActive = (StatusComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() == "Active",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return tour;
        }
    }
}