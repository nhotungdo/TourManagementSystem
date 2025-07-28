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
    public partial class ItineraryManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _itinerariesView;
        private List<TourAttraction> _allItineraries;

        // Control references
        private DataGrid? _itinerariesDataGrid;
        private ComboBox? _tourFilterComboBox;
        private TextBox? _searchTextBox;

        public ItineraryManagementPage()
        {
            // Initialize without XAML compilation dependency
            InitializePageWithoutXaml();

            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);

            // Initialize control references
            InitializeControls();

            LoadItineraries();
            LoadTourFilter();
            SetupEventHandlers();
        }

        private void InitializePageWithoutXaml()
        {
            try
            {
                // Try to initialize XAML components if available
                var initializeComponentMethod = this.GetType().GetMethod("InitializeComponent");
                if (initializeComponentMethod != null)
                {
                    initializeComponentMethod.Invoke(this, null);
                }
                else
                {
                    // Create a basic layout if XAML compilation failed
                    CreateBasicLayout();
                }
            }
            catch (Exception ex)
            {
                // Create a basic layout if XAML initialization fails
                CreateBasicLayout();
                MessageBox.Show($"Error initializing XAML components: {ex.Message}\nUsing basic layout instead.",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CreateBasicLayout()
        {
            // Create a basic layout programmatically
            this.Background = System.Windows.Media.Brushes.White;

            var mainGrid = new Grid();
            mainGrid.Margin = new Thickness(20);

            // Add row definitions
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header
            var headerPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };
            var titleText = new TextBlock
            {
                Text = "🗺️ Itinerary Management",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.DarkSlateGray
            };
            var subtitleText = new TextBlock
            {
                Text = "Manage tour itineraries and attractions",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 5, 0, 0)
            };
            headerPanel.Children.Add(titleText);
            headerPanel.Children.Add(subtitleText);
            Grid.SetRow(headerPanel, 0);
            mainGrid.Children.Add(headerPanel);

            // Search and Filter Bar
            var filterGrid = new Grid { Margin = new Thickness(0, 0, 0, 20) };
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Search Box
            _searchTextBox = new TextBox
            {
                Margin = new Thickness(0, 0, 10, 0),
                Padding = new Thickness(10, 8, 10, 8),
                FontSize = 14,
                BorderThickness = new Thickness(1),
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            _searchTextBox.TextChanged += SearchTextBox_TextChanged;
            Grid.SetColumn(_searchTextBox, 0);
            filterGrid.Children.Add(_searchTextBox);

            // Tour Filter
            _tourFilterComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 10, 0),
                Padding = new Thickness(10, 8, 10, 8),
                FontSize = 14,
                Width = 200
            };
            _tourFilterComboBox.SelectionChanged += TourFilterComboBox_SelectionChanged;
            Grid.SetColumn(_tourFilterComboBox, 1);
            filterGrid.Children.Add(_tourFilterComboBox);

            // Add Itinerary Button
            var addButton = new Button
            {
                Content = "➕ Add Itinerary",
                Background = System.Windows.Media.Brushes.DodgerBlue,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(15, 8, 15, 8),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,

            };
            addButton.Click += AddItineraryButton_Click;
            Grid.SetColumn(addButton, 2);
            filterGrid.Children.Add(addButton);

            Grid.SetRow(filterGrid, 1);
            mainGrid.Children.Add(filterGrid);

            // DataGrid
            _itinerariesDataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserReorderColumns = true,
                CanUserResizeColumns = true,
                CanUserResizeRows = false,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                SelectionMode = DataGridSelectionMode.Single
            };
            _itinerariesDataGrid.Sorting += ItinerariesDataGrid_Sorting;
            _itinerariesDataGrid.SelectionChanged += ItinerariesDataGrid_SelectionChanged;

            // Add columns
            _itinerariesDataGrid.Columns.Add(new DataGridTextColumn { Header = "Tour", Width = 200 });
            _itinerariesDataGrid.Columns.Add(new DataGridTextColumn { Header = "Attraction", Width = 200 });
            _itinerariesDataGrid.Columns.Add(new DataGridTextColumn { Header = "Location", Width = 150 });
            _itinerariesDataGrid.Columns.Add(new DataGridTextColumn { Header = "Visit Day", Width = 80 });
            _itinerariesDataGrid.Columns.Add(new DataGridTextColumn { Header = "Visit Order", Width = 80 });
            _itinerariesDataGrid.Columns.Add(new DataGridTextColumn { Header = "Description", Width = 250 });

            // Actions column
            var actionsColumn = new DataGridTemplateColumn { Header = "Actions", Width = 150 };
            var actionsTemplate = new DataTemplate();
            var actionsFactory = new FrameworkElementFactory(typeof(StackPanel));
            actionsFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            // Edit button factory
            var editButtonFactory = new FrameworkElementFactory(typeof(Button));
            editButtonFactory.SetValue(Button.ContentProperty, "✏️ Edit");
            editButtonFactory.SetValue(Button.MarginProperty, new Thickness(2));
            editButtonFactory.SetValue(Button.PaddingProperty, new Thickness(8, 4, 8, 4));
            editButtonFactory.SetValue(Button.FontSizeProperty, 12.0);
            editButtonFactory.SetValue(Button.BackgroundProperty, System.Windows.Media.Brushes.Orange);
            editButtonFactory.SetValue(Button.ForegroundProperty, System.Windows.Media.Brushes.White);
            editButtonFactory.SetValue(Button.BorderThicknessProperty, new Thickness(0));
            editButtonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(EditItinerary_Click));

            // Delete button factory
            var deleteButtonFactory = new FrameworkElementFactory(typeof(Button));
            deleteButtonFactory.SetValue(Button.ContentProperty, "🗑️ Delete");
            deleteButtonFactory.SetValue(Button.MarginProperty, new Thickness(2));
            deleteButtonFactory.SetValue(Button.PaddingProperty, new Thickness(8, 4, 8, 4));
            deleteButtonFactory.SetValue(Button.FontSizeProperty, 12.0);
            deleteButtonFactory.SetValue(Button.BackgroundProperty, System.Windows.Media.Brushes.Red);
            deleteButtonFactory.SetValue(Button.ForegroundProperty, System.Windows.Media.Brushes.White);
            deleteButtonFactory.SetValue(Button.BorderThicknessProperty, new Thickness(0));
            deleteButtonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(DeleteItinerary_Click));

            actionsFactory.AppendChild(editButtonFactory);
            actionsFactory.AppendChild(deleteButtonFactory);
            actionsTemplate.VisualTree = actionsFactory;
            actionsColumn.CellTemplate = actionsTemplate;

            _itinerariesDataGrid.Columns.Add(actionsColumn);

            Grid.SetRow(_itinerariesDataGrid, 2);
            mainGrid.Children.Add(_itinerariesDataGrid);

            this.Content = mainGrid;
        }

        private void InitializeControls()
        {
            try
            {
                // If controls were not created in CreateBasicLayout, try to find them
                if (_itinerariesDataGrid == null)
                    _itinerariesDataGrid = FindName("ItinerariesDataGrid") as DataGrid;
                if (_tourFilterComboBox == null)
                    _tourFilterComboBox = FindName("TourFilterComboBox") as ComboBox;
                if (_searchTextBox == null)
                    _searchTextBox = FindName("SearchTextBox") as TextBox;

                // Log if any controls are not found
                if (_itinerariesDataGrid == null || _tourFilterComboBox == null || _searchTextBox == null)
                {
                    MessageBox.Show("Some UI controls could not be found. The page may not display correctly.",
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing controls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadItineraries()
        {
            try
            {
                _allItineraries = _context.TourAttractions
                    .Include(ta => ta.Tour)
                    .Include(ta => ta.Attraction)
                    .OrderBy(ta => ta.Tour.TourName)
                    .ThenBy(ta => ta.VisitDay)
                    .ThenBy(ta => ta.VisitOrder)
                    .AsNoTracking() // Use AsNoTracking for better performance since we're just displaying
                    .ToList();

                _itinerariesView = CollectionViewSource.GetDefaultView(_allItineraries);

                if (_itinerariesDataGrid != null)
                    _itinerariesDataGrid.ItemsSource = _itinerariesView;

                // Apply initial filter
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading itineraries: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTourFilter()
        {
            try
            {
                var tours = _context.Tours.Where(t => t.IsActive == true).ToList();

                if (_tourFilterComboBox != null)
                {
                    _tourFilterComboBox.Items.Clear();
                    _tourFilterComboBox.Items.Add(new ComboBoxItem { Content = "All Tours", IsSelected = true });

                    foreach (var tour in tours)
                    {
                        _tourFilterComboBox.Items.Add(new ComboBoxItem { Content = tour.TourName, Tag = tour.TourId });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tour filter: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupEventHandlers()
        {
            // Setup sorting
            if (_itinerariesDataGrid != null)
                _itinerariesDataGrid.Sorting += ItinerariesDataGrid_Sorting;
        }

        private void ApplyFilters()
        {
            if (_itinerariesView == null) return;

            _itinerariesView.Filter = itinerary =>
            {
                if (itinerary is not TourAttraction itineraryObj) return false;

                // Search filter
                var searchText = _searchTextBox?.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = (itineraryObj.Tour?.TourName?.ToLower().Contains(searchText) ?? false) ||
                                       (itineraryObj.Attraction?.AttractionName?.ToLower().Contains(searchText) ?? false) ||
                                       (itineraryObj.Attraction?.Location?.ToLower().Contains(searchText) ?? false) ||
                                       (itineraryObj.Description?.ToLower().Contains(searchText) ?? false);
                    if (!matchesSearch) return false;
                }

                // Tour filter
                var selectedTour = (_tourFilterComboBox?.SelectedItem as ComboBoxItem)?.Tag;
                if (selectedTour != null && selectedTour is int tourId)
                {
                    if (itineraryObj.TourId != tourId) return false;
                }

                return true;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void TourFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ItinerariesDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Let the DataGrid handle sorting automatically
            // The CollectionView will handle the sorting based on the column clicked
        }

        private void ItinerariesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
            // This can be used to enable/disable buttons based on selection
            var selectedItinerary = _itinerariesDataGrid?.SelectedItem as TourAttraction;
            // Add any selection-based logic here if needed
        }

        private void AddItineraryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addItineraryWindow = new AddItineraryWindow(_activityLogService);
                if (addItineraryWindow.ShowDialog() == true)
                {
                    LoadItineraries(); // Refresh data
                    MessageBox.Show("Itinerary added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add Itinerary window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditItinerary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is TourAttraction itinerary)
                {
                    if (itinerary != null)
                    {
                        var editItineraryWindow = new EditItineraryWindow(itinerary, _activityLogService);
                        if (editItineraryWindow.ShowDialog() == true)
                        {
                            LoadItineraries(); // Refresh the list
                            MessageBox.Show("Itinerary updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing itinerary: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteItinerary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is TourAttraction itinerary)
                {
                    if (itinerary != null)
                    {
                        var tourName = itinerary.Tour?.TourName ?? "Unknown Tour";
                        var attractionName = itinerary.Attraction?.AttractionName ?? "Unknown Attraction";

                        var result = MessageBox.Show(
                            $"Are you sure you want to delete this itinerary?\n\nTour: {tourName}\nAttraction: {attractionName}\nVisit Day: {itinerary.VisitDay}",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            // For composite key entities, we need to find the entity in the context first
                            var entityToDelete = _context.TourAttractions
                                .FirstOrDefault(ta => ta.TourId == itinerary.TourId &&
                                                    ta.AttractionId == itinerary.AttractionId &&
                                                    ta.VisitDay == itinerary.VisitDay);

                            if (entityToDelete != null)
                            {
                                _context.TourAttractions.Remove(entityToDelete);
                                _context.SaveChanges();

                                // Log activity
                                _activityLogService.LogActivity(1, "DELETE", "TourAttraction", itinerary.TourId,
                                    $"Deleted itinerary: {tourName} - {attractionName} (Day {itinerary.VisitDay})");

                                LoadItineraries(); // Refresh the list
                                MessageBox.Show("Itinerary deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Itinerary not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting itinerary: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}
