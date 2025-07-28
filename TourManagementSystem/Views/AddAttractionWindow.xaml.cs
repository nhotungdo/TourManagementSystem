using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class AddAttractionWindow : Window
    {
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public AddAttractionWindow(ActivityLogService activityLogService)
        {
            InitializeComponent();
            _activityLogService = activityLogService;
            _context = new TourManagementContext();
        }

        private void AddAttractionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var attraction = CreateAttractionFromForm();

                    // Add to database
                    _context.Attractions.Add(attraction);
                    _context.SaveChanges();

                    // Log activity
                    _activityLogService.LogActivity(1, "CREATE", "Attraction", attraction.AttractionId,
                        $"Added new attraction: {attraction.AttractionName}");

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding attraction: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private bool ValidateForm()
        {
            // Attraction name validation
            if (string.IsNullOrWhiteSpace(AttractionNameTextBox.Text))
            {
                MessageBox.Show("Please enter an attraction name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AttractionNameTextBox.Focus();
                return false;
            }

            if (AttractionNameTextBox.Text.Length < 3)
            {
                MessageBox.Show("Attraction name must be at least 3 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AttractionNameTextBox.Focus();
                return false;
            }

            // Check for duplicate attraction name
            var existingAttraction = _context.Attractions
                .FirstOrDefault(a => a.AttractionName.ToLower() == AttractionNameTextBox.Text.Trim().ToLower());
            if (existingAttraction != null)
            {
                MessageBox.Show("An attraction with this name already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AttractionNameTextBox.Focus();
                return false;
            }

            // Location validation
            if (string.IsNullOrWhiteSpace(LocationComboBox.Text))
            {
                MessageBox.Show("Please enter a location.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LocationComboBox.Focus();
                return false;
            }

            // Description validation
            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                MessageBox.Show("Please enter a description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show("Please enter a valid image URL.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ImageUrlTextBox.Focus();
                    return false;
                }
            }

            // Entry fee validation (optional field)
            if (!string.IsNullOrWhiteSpace(EntryFeeTextBox.Text))
            {
                if (!decimal.TryParse(EntryFeeTextBox.Text, out decimal entryFee) || entryFee < 0)
                {
                    MessageBox.Show("Please enter a valid entry fee (must be 0 or greater).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EntryFeeTextBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private Attraction CreateAttractionFromForm()
        {
            var attraction = new Attraction();

            // Get next AttractionId
            var maxAttractionId = _context.Attractions.Max(a => (int?)a.AttractionId) ?? 0;
            attraction.AttractionId = maxAttractionId + 1;

            // Set basic properties
            attraction.AttractionName = AttractionNameTextBox.Text.Trim();
            attraction.Location = LocationComboBox.Text.Trim();
            attraction.Description = DescriptionTextBox.Text.Trim();

            // Set optional properties
            attraction.ImageUrl = string.IsNullOrWhiteSpace(ImageUrlTextBox.Text) ? null : ImageUrlTextBox.Text.Trim();

            return attraction;
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}