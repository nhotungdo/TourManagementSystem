using System;
using System.Windows;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditTourWindow : Window
    {
        private readonly Tour _tour;
        private readonly TourService _tourService;
        private readonly ActivityLogService _activityLogService;

        public EditTourWindow(Tour tour, TourService tourService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _tour = tour;
            _tourService = tourService;
            _activityLogService = activityLogService;

            LoadTourData();
        }

        private void LoadTourData()
        {
            try
            {
                // Load tour data into form
                TourNameTextBox.Text = _tour.TourName;
                DescriptionTextBox.Text = _tour.Description;
                DurationTextBox.Text = _tour.DurationDays.ToString();
                PriceTextBox.Text = _tour.BasePrice.ToString("N0");
                DestinationTextBox.Text = _tour.Destination;

                // Set status
                if (_tour.IsActive == true)
                {
                    StatusComboBox.SelectedIndex = 0; // Active
                }
                else
                {
                    StatusComboBox.SelectedIndex = 1; // Inactive
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tour data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTourButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    UpdateTourFromForm();

                    if (_tourService.UpdateTour(_tour, 1)) // 1 is admin ID
                    {
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update tour. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (TourNameTextBox.Text.Length < 3)
            {
                MessageBox.Show("Tour name must be at least 3 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            if (DescriptionTextBox.Text.Length < 10)
            {
                MessageBox.Show("Description must be at least 10 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            if (duration > 30)
            {
                MessageBox.Show("Duration cannot exceed 30 days.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            // Remove commas and spaces from price
            var cleanPrice = PriceTextBox.Text.Replace(",", "").Replace(" ", "");
            if (!decimal.TryParse(cleanPrice, out decimal price) || price <= 0)
            {
                MessageBox.Show("Price must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PriceTextBox.Focus();
                return false;
            }

            if (price > 100000000) // 100 million VND
            {
                MessageBox.Show("Price cannot exceed 100,000,000 VND.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void UpdateTourFromForm()
        {
            // Update tour properties
            _tour.TourName = TourNameTextBox.Text.Trim();
            _tour.Description = DescriptionTextBox.Text.Trim();
            _tour.DurationDays = int.Parse(DurationTextBox.Text);
            _tour.DurationNights = int.Parse(DurationTextBox.Text) - 1; // Assuming nights = days - 1

            // Parse price (remove commas and spaces)
            var cleanPrice = PriceTextBox.Text.Replace(",", "").Replace(" ", "");
            _tour.BasePrice = decimal.Parse(cleanPrice);

            _tour.Destination = DestinationTextBox.Text.Trim();
            _tour.IsActive = (StatusComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() == "Active";
            _tour.UpdatedAt = DateTime.Now;
        }
    }
}