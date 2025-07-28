using System;
using System.Text.RegularExpressions;
using System.Windows;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditAttractionWindow : Window
    {
        private readonly Attraction _attraction;
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public EditAttractionWindow(Attraction attraction, TourManagementContext context, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _attraction = attraction;
            _context = context;
            _activityLogService = activityLogService;

            LoadAttractionData();
        }

        private void LoadAttractionData()
        {
            try
            {
                // Load attraction data into form
                AttractionNameTextBox.Text = _attraction.AttractionName;
                LocationTextBox.Text = _attraction.Location;
                DescriptionTextBox.Text = _attraction.Description;
                ImageUrlTextBox.Text = _attraction.ImageUrl ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attraction data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateAttractionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    UpdateAttractionFromForm();

                    // Update in database
                    var attractionToUpdate = _context.Attractions.Find(_attraction.AttractionId);
                    if (attractionToUpdate != null)
                    {
                        attractionToUpdate.AttractionName = _attraction.AttractionName;
                        attractionToUpdate.Location = _attraction.Location;
                        attractionToUpdate.Description = _attraction.Description;
                        attractionToUpdate.ImageUrl = _attraction.ImageUrl;
                        _context.SaveChanges();

                        // Log activity
                        _activityLogService.LogActivity(1, "UPDATE", "Attraction", _attraction.AttractionId,
                            $"Updated attraction: {_attraction.AttractionName}");

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Attraction not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating attraction: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Attraction name validation
            if (string.IsNullOrWhiteSpace(AttractionNameTextBox.Text))
            {
                MessageBox.Show("Attraction name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AttractionNameTextBox.Focus();
                return false;
            }

            if (AttractionNameTextBox.Text.Length < 3)
            {
                MessageBox.Show("Attraction name must be at least 3 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AttractionNameTextBox.Focus();
                return false;
            }

            // Location validation
            if (string.IsNullOrWhiteSpace(LocationTextBox.Text))
            {
                MessageBox.Show("Location is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LocationTextBox.Focus();
                return false;
            }

            if (LocationTextBox.Text.Length < 2)
            {
                MessageBox.Show("Location must be at least 2 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LocationTextBox.Focus();
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

            // Image URL validation (optional but if provided, must be valid)
            if (!string.IsNullOrWhiteSpace(ImageUrlTextBox.Text))
            {
                if (!IsValidUrl(ImageUrlTextBox.Text))
                {
                    MessageBox.Show("Please enter a valid URL for the image.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ImageUrlTextBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private void UpdateAttractionFromForm()
        {
            // Update attraction properties
            _attraction.AttractionName = AttractionNameTextBox.Text.Trim();
            _attraction.Location = LocationTextBox.Text.Trim();
            _attraction.Description = DescriptionTextBox.Text.Trim();
            _attraction.ImageUrl = string.IsNullOrWhiteSpace(ImageUrlTextBox.Text) ? null : ImageUrlTextBox.Text.Trim();
        }

        private bool IsValidUrl(string url)
        {
            try
            {
                var regex = new Regex(@"^https?://.*$", RegexOptions.IgnoreCase);
                return regex.IsMatch(url);
            }
            catch
            {
                return false;
            }
        }
    }
}