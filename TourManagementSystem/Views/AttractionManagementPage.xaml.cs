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
    public partial class AttractionManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _attractionsView;
        private List<Attraction> _allAttractions;

        public AttractionManagementPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);

            LoadAttractions();
            LoadLocationFilter();
            SetupEventHandlers();
        }

        private void LoadAttractions()
        {
            try
            {
                _allAttractions = _context.Attractions.ToList();

                _attractionsView = CollectionViewSource.GetDefaultView(_allAttractions);
                AttractionsDataGrid.ItemsSource = _attractionsView;

                // Apply initial filter
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attractions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLocationFilter()
        {
            try
            {
                var locations = _context.Attractions
                    .Select(a => a.Location)
                    .Distinct()
                    .OrderBy(l => l)
                    .ToList();

                LocationFilterComboBox.Items.Clear();
                LocationFilterComboBox.Items.Add(new ComboBoxItem { Content = "All Locations", IsSelected = true });

                foreach (var location in locations)
                {
                    LocationFilterComboBox.Items.Add(new ComboBoxItem { Content = location });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading location filter: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupEventHandlers()
        {
            // Setup sorting
            AttractionsDataGrid.Sorting += AttractionsDataGrid_Sorting;
        }

        private void ApplyFilters()
        {
            if (_attractionsView == null) return;

            _attractionsView.Filter = attraction =>
            {
                if (attraction is not Attraction attractionObj) return false;

                // Search filter
                var searchText = SearchTextBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = attractionObj.AttractionName?.ToLower().Contains(searchText) == true ||
                                       attractionObj.Location?.ToLower().Contains(searchText) == true ||
                                       attractionObj.Description?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // Location filter
                var selectedLocation = (LocationFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedLocation) && selectedLocation != "All Locations")
                {
                    if (attractionObj.Location != selectedLocation) return false;
                }

                return true;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void LocationFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void AttractionsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void AttractionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void AddAttractionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addAttractionWindow = new AddAttractionWindow(_activityLogService);
                if (addAttractionWindow.ShowDialog() == true)
                {
                    LoadAttractions(); // Refresh data
                    LoadLocationFilter(); // Refresh location filter
                    MessageBox.Show("Attraction added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add Attraction window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditAttraction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int attractionId)
                {
                    var attraction = _allAttractions.FirstOrDefault(a => a.AttractionId == attractionId);
                    if (attraction != null)
                    {
                        var editAttractionWindow = new EditAttractionWindow(attraction, _context, _activityLogService);
                        if (editAttractionWindow.ShowDialog() == true)
                        {
                            LoadAttractions(); // Refresh data
                            LoadLocationFilter(); // Refresh location filter
                            MessageBox.Show("Attraction updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing attraction: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteAttraction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int attractionId)
                {
                    var attraction = _allAttractions.FirstOrDefault(a => a.AttractionId == attractionId);
                    if (attraction != null)
                    {
                        // Check if attraction is used in any tour
                        var isUsedInTour = _context.TourAttractions.Any(ta => ta.AttractionId == attractionId);
                        if (isUsedInTour)
                        {
                            MessageBox.Show("Cannot delete this attraction because it is used in one or more tours.",
                                "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        var result = MessageBox.Show(
                            $"Are you sure you want to delete this attraction?\n\nName: {attraction.AttractionName}\nLocation: {attraction.Location}",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            _context.Attractions.Remove(attraction);
                            _context.SaveChanges();

                            // Log activity
                            _activityLogService.LogActivity(1, "DELETE", "Attraction", attractionId,
                                $"Deleted attraction: {attraction.AttractionName}");

                            LoadAttractions(); // Refresh the list
                            LoadLocationFilter(); // Refresh location filter
                            MessageBox.Show("Attraction deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting attraction: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}