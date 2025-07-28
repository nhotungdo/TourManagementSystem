using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class AddItineraryWindow : Window
    {
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public AddItineraryWindow(ActivityLogService activityLogService)
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
                // Load tours
                var tours = _context.Tours.Where(t => t.IsActive == true).ToList();
                TourComboBox.ItemsSource = tours;

                // Load attractions
                var attractions = _context.Attractions.OrderBy(a => a.AttractionName).ToList();
                AttractionComboBox.ItemsSource = attractions;

                // Debug: Show how many tours and attractions were loaded
                if (!tours.Any())
                {
                    MessageBox.Show("No active tours available. Please ensure there are active tours in the system.", "No Tours", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (!attractions.Any())
                {
                    MessageBox.Show("No attractions available. Please ensure there are attractions in the system.", "No Attractions", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle tour selection if needed
        }

        private void AddItineraryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var itinerary = CreateItineraryFromForm();

                    // Add to database
                    _context.TourAttractions.Add(itinerary);
                    _context.SaveChanges();

                    // Log activity
                    _activityLogService.LogActivity(1, "CREATE", "TourAttraction", itinerary.TourId,
                        $"Added itinerary: {itinerary.Description} for tour ID {itinerary.TourId}");

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding itinerary: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            // Tour validation
            if (TourComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a tour.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TourComboBox.Focus();
                return false;
            }

            // Attraction validation
            if (AttractionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an attraction.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                AttractionComboBox.Focus();
                return false;
            }

            // Visit day validation
            if (string.IsNullOrWhiteSpace(VisitDayTextBox.Text))
            {
                MessageBox.Show("Please enter a visit day.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                VisitDayTextBox.Focus();
                return false;
            }

            if (!int.TryParse(VisitDayTextBox.Text, out int visitDay) || visitDay <= 0)
            {
                MessageBox.Show("Visit day must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                VisitDayTextBox.Focus();
                return false;
            }

            // Visit order validation
            if (string.IsNullOrWhiteSpace(VisitOrderTextBox.Text))
            {
                MessageBox.Show("Please enter a visit order.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                VisitOrderTextBox.Focus();
                return false;
            }

            if (!int.TryParse(VisitOrderTextBox.Text, out int visitOrder) || visitOrder <= 0)
            {
                MessageBox.Show("Visit order must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                VisitOrderTextBox.Focus();
                return false;
            }

            // Description validation
            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                MessageBox.Show("Please enter a description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DescriptionTextBox.Focus();
                return false;
            }

            if (DescriptionTextBox.Text.Length < 5)
            {
                MessageBox.Show("Description must be at least 5 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DescriptionTextBox.Focus();
                return false;
            }

            // Check for duplicate itinerary (same tour, attraction, day, and order)
            var selectedTour = TourComboBox.SelectedItem as Tour;
            var selectedAttraction = AttractionComboBox.SelectedItem as Attraction;

            if (selectedTour != null && selectedAttraction != null)
            {
                var existingItinerary = _context.TourAttractions
                    .FirstOrDefault(ta => ta.TourId == selectedTour.TourId &&
                                        ta.AttractionId == selectedAttraction.AttractionId &&
                                        ta.VisitDay == visitDay &&
                                        ta.VisitOrder == visitOrder);

                if (existingItinerary != null)
                {
                    MessageBox.Show($"An itinerary already exists for this tour, attraction, day {visitDay}, and order {visitOrder}.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private TourAttraction CreateItineraryFromForm()
        {
            var selectedTour = TourComboBox.SelectedItem as Tour;
            var selectedAttraction = AttractionComboBox.SelectedItem as Attraction;

            var itinerary = new TourAttraction();

            // Set tour and attraction
            itinerary.TourId = selectedTour.TourId;
            itinerary.AttractionId = selectedAttraction.AttractionId;

            // Set visit day and order
            itinerary.VisitDay = int.Parse(VisitDayTextBox.Text);
            itinerary.VisitOrder = int.Parse(VisitOrderTextBox.Text);

            // Set description
            itinerary.Description = DescriptionTextBox.Text.Trim();

            return itinerary;
        }
    }
}