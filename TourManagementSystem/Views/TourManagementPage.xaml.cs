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
    public partial class TourManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly TourService _tourService;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _toursView;
        private List<Tour> _allTours;

        public TourManagementPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);
            _tourService = new TourService(_context, _activityLogService);

            LoadTours();
            SetupEventHandlers();
        }

        private void LoadTours()
        {
            try
            {
                _allTours = _context.Tours
                    .Include(t => t.CreatedByNavigation)
                    .ToList();
                _toursView = CollectionViewSource.GetDefaultView(_allTours);
                ToursDataGrid.ItemsSource = _toursView;

                // Apply initial filter
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tours: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupEventHandlers()
        {
            // Setup sorting
            ToursDataGrid.Sorting += ToursDataGrid_Sorting;
        }

        private void ApplyFilters()
        {
            if (_toursView == null) return;

            _toursView.Filter = tour =>
            {
                if (tour is not Tour tourObj) return false;

                // Search filter
                var searchText = SearchTextBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = tourObj.TourName?.ToLower().Contains(searchText) == true ||
                                       tourObj.Description?.ToLower().Contains(searchText) == true ||
                                       tourObj.Destination?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // Status filter
                var selectedStatus = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All Status")
                {
                    if (selectedStatus == "Active" && tourObj.IsActive != true) return false;
                    if (selectedStatus == "Inactive" && tourObj.IsActive != false) return false;
                }

                return true;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ToursDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void ToursDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void AddTourButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addTourWindow = new AddTourWindow(_tourService, _activityLogService);
                if (addTourWindow.ShowDialog() == true)
                {
                    LoadTours(); // Refresh the list
                    MessageBox.Show("Tour added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditTour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int tourId)
                {
                    var tour = _allTours.FirstOrDefault(t => t.TourId == tourId);
                    if (tour != null)
                    {
                        var editTourWindow = new EditTourWindow(tour, _tourService, _activityLogService);
                        if (editTourWindow.ShowDialog() == true)
                        {
                            LoadTours(); // Refresh the list
                            MessageBox.Show("Tour updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteTour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int tourId)
                {
                    var tour = _allTours.FirstOrDefault(t => t.TourId == tourId);
                    if (tour != null)
                    {
                        var result = MessageBox.Show(
                            $"Are you sure you want to delete tour '{tour.TourName}'?\n\nThis action cannot be undone.",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            if (_tourService.DeleteTour(tourId, 1)) // 1 is admin ID
                            {
                                LoadTours(); // Refresh the list
                                MessageBox.Show("Tour deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete tour. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}