using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditItineraryWindow : Window
    {
        private readonly TourAttraction _itinerary;
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public EditItineraryWindow(TourAttraction itinerary, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _itinerary = itinerary;
            _activityLogService = activityLogService;
            _context = new TourManagementContext();

            LoadItineraryData();
        }

        private void LoadItineraryData()
        {
            try
            {
                // Load tour and attraction names
                var tour = _context.Tours.FirstOrDefault(t => t.TourId == _itinerary.TourId);
                var attraction = _context.Attractions.FirstOrDefault(a => a.AttractionId == _itinerary.AttractionId);

                TourTextBox.Text = tour?.TourName ?? "Unknown Tour";
                AttractionTextBox.Text = attraction?.AttractionName ?? "Unknown Attraction";

                // Load current values
                VisitDayTextBox.Text = _itinerary.VisitDay.ToString();
                VisitOrderTextBox.Text = _itinerary.VisitOrder.ToString();
                DescriptionTextBox.Text = _itinerary.Description ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading itinerary data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateItineraryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    UpdateItineraryFromForm();

                    // Save changes to database
                    _context.SaveChanges();

                    // Log activity
                    _activityLogService.LogActivity(1, "UPDATE", "TourAttraction", _itinerary.TourId,
                        $"Updated itinerary: {_itinerary.Description} for tour ID {_itinerary.TourId}");

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating itinerary: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            // Check for duplicate itinerary (same tour, attraction, day, and order) - excluding current itinerary
            var existingItinerary = _context.TourAttractions
                .FirstOrDefault(ta => ta.TourId == _itinerary.TourId &&
                                    ta.AttractionId == _itinerary.AttractionId &&
                                    ta.VisitDay == visitDay &&
                                    ta.VisitOrder == visitOrder &&
                                    !(ta.TourId == _itinerary.TourId &&
                                      ta.AttractionId == _itinerary.AttractionId &&
                                      ta.VisitDay == _itinerary.VisitDay &&
                                      ta.VisitOrder == _itinerary.VisitOrder));

            if (existingItinerary != null)
            {
                MessageBox.Show($"An itinerary already exists for this tour, attraction, day {visitDay}, and order {visitOrder}.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void UpdateItineraryFromForm()
        {
            // Update visit day and order
            _itinerary.VisitDay = int.Parse(VisitDayTextBox.Text);
            _itinerary.VisitOrder = int.Parse(VisitOrderTextBox.Text);

            // Update description
            _itinerary.Description = DescriptionTextBox.Text.Trim();
        }
    }
}