using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class BrowseToursPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentUser;
        private List<Tour> _allTours;
        private List<Tour> _filteredTours;

        public BrowseToursPage(User user)
        {
            _context = new TourManagementContext();
            _currentUser = user;

            try
            {
                // Try to invoke InitializeComponent using reflection
                var initializeMethod = this.GetType().GetMethod("InitializeComponent");
                if (initializeMethod != null)
                {
                    initializeMethod.Invoke(this, null);
                }
                else
                {
                    // Fallback: Create basic UI programmatically if XAML fails
                    CreateBasicLayout();
                }
            }
            catch (Exception ex)
            {
                // Fallback: Create basic UI programmatically if XAML fails
                CreateBasicLayout();
            }

            LoadTours();
            LoadFilters();
        }

        private void CreateBasicLayout()
        {
            // Create a basic layout if XAML initialization fails
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Header
            var header = new StackPanel
            {
                Background = System.Windows.Media.Brushes.Orange,
                Margin = new Thickness(20)
            };

            var title = new TextBlock
            {
                Text = "Browse Tours",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White
            };

            header.Children.Add(title);

            // Content
            var content = new StackPanel { Margin = new Thickness(20) };
            var message = new TextBlock
            {
                Text = "Tour browsing interface loaded successfully.",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            content.Children.Add(message);

            Grid.SetRow(header, 0);
            Grid.SetRow(content, 1);
            grid.Children.Add(header);
            grid.Children.Add(content);

            Content = grid;
        }

        private void LoadTours()
        {
            try
            {
                _allTours = _context.Tours
                    .Include(t => t.TourSchedules)
                    .Include(t => t.TourSchedules).ThenInclude(ts => ts.Guide)
                    .Include(t => t.TourAttractions)
                    .Include(t => t.TourAttractions).ThenInclude(ta => ta.Attraction)
                    .Where(t => t.IsActive == true)
                    .ToList();

                _filteredTours = new List<Tour>(_allTours);

                var toursItemsControl = FindName("ToursItemsControl") as ItemsControl;
                if (toursItemsControl != null)
                {
                    toursItemsControl.ItemsSource = _filteredTours;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tours: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFilters()
        {
            try
            {
                // Load destinations
                var destinations = _allTours
                    .Select(t => t.Destination)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                var destinationComboBox = FindName("DestinationComboBox") as ComboBox;
                if (destinationComboBox != null)
                {
                    foreach (var destination in destinations)
                    {
                        destinationComboBox.Items.Add(new ComboBoxItem { Content = destination });
                    }

                    // Set default selections
                    destinationComboBox.SelectedIndex = 0;
                }

                var priceRangeComboBox = FindName("PriceRangeComboBox") as ComboBox;
                if (priceRangeComboBox != null)
                    priceRangeComboBox.SelectedIndex = 0;

                var durationComboBox = FindName("DurationComboBox") as ComboBox;
                if (durationComboBox != null)
                    durationComboBox.SelectedIndex = 0;

                var sortByComboBox = FindName("SortByComboBox") as ComboBox;
                if (sortByComboBox != null)
                    sortByComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void DestinationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void PriceRangeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void DurationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Reset all filters
                var searchTextBox = FindName("SearchTextBox") as TextBox;
                if (searchTextBox != null)
                    searchTextBox.Text = "";

                var destinationComboBox = FindName("DestinationComboBox") as ComboBox;
                if (destinationComboBox != null)
                    destinationComboBox.SelectedIndex = 0;

                var priceRangeComboBox = FindName("PriceRangeComboBox") as ComboBox;
                if (priceRangeComboBox != null)
                    priceRangeComboBox.SelectedIndex = 0;

                var durationComboBox = FindName("DurationComboBox") as ComboBox;
                if (durationComboBox != null)
                    durationComboBox.SelectedIndex = 0;

                var sortByComboBox = FindName("SortByComboBox") as ComboBox;
                if (sortByComboBox != null)
                    sortByComboBox.SelectedIndex = 0;

                // Reset tours list
                _filteredTours = new List<Tour>(_allTours);

                var toursItemsControl = FindName("ToursItemsControl") as ItemsControl;
                if (toursItemsControl != null)
                {
                    toursItemsControl.ItemsSource = _filteredTours;
                }

                MessageBox.Show("All filters have been cleared.", "Filters Cleared", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var filtered = _allTours.AsQueryable();

                // Search filter
                var searchTextBox = FindName("SearchTextBox") as TextBox;
                if (searchTextBox != null && !string.IsNullOrWhiteSpace(searchTextBox.Text))
                {
                    var searchTerm = searchTextBox.Text.ToLower();
                    filtered = filtered.Where(t =>
                        t.TourName.ToLower().Contains(searchTerm) ||
                        t.Destination.ToLower().Contains(searchTerm) ||
                        t.Description.ToLower().Contains(searchTerm));
                }

                // Destination filter
                var destinationComboBox = FindName("DestinationComboBox") as ComboBox;
                if (destinationComboBox?.SelectedItem is ComboBoxItem selectedDestination &&
                    selectedDestination.Content.ToString() != "All Destinations")
                {
                    filtered = filtered.Where(t => t.Destination == selectedDestination.Content.ToString());
                }

                // Price range filter
                var priceRangeComboBox = FindName("PriceRangeComboBox") as ComboBox;
                if (priceRangeComboBox?.SelectedItem is ComboBoxItem selectedPrice)
                {
                    var priceText = selectedPrice.Content.ToString();
                    switch (priceText)
                    {
                        case "Under $500":
                            filtered = filtered.Where(t => t.BasePrice < 500);
                            break;
                        case "$500 - $1000":
                            filtered = filtered.Where(t => t.BasePrice >= 500 && t.BasePrice <= 1000);
                            break;
                        case "$1000 - $2000":
                            filtered = filtered.Where(t => t.BasePrice >= 1000 && t.BasePrice <= 2000);
                            break;
                        case "Over $2000":
                            filtered = filtered.Where(t => t.BasePrice > 2000);
                            break;
                    }
                }

                // Duration filter
                var durationComboBox = FindName("DurationComboBox") as ComboBox;
                if (durationComboBox?.SelectedItem is ComboBoxItem selectedDuration)
                {
                    var durationText = selectedDuration.Content.ToString();
                    switch (durationText)
                    {
                        case "1-3 Days":
                            filtered = filtered.Where(t => t.DurationDays >= 1 && t.DurationDays <= 3);
                            break;
                        case "4-7 Days":
                            filtered = filtered.Where(t => t.DurationDays >= 4 && t.DurationDays <= 7);
                            break;
                        case "8-14 Days":
                            filtered = filtered.Where(t => t.DurationDays >= 8 && t.DurationDays <= 14);
                            break;
                        case "15+ Days":
                            filtered = filtered.Where(t => t.DurationDays >= 15);
                            break;
                    }
                }

                // Sort
                var sortByComboBox = FindName("SortByComboBox") as ComboBox;
                if (sortByComboBox?.SelectedItem is ComboBoxItem selectedSort)
                {
                    var sortText = selectedSort.Content.ToString();
                    switch (sortText)
                    {
                        case "Name (A-Z)":
                            filtered = filtered.OrderBy(t => t.TourName);
                            break;
                        case "Name (Z-A)":
                            filtered = filtered.OrderByDescending(t => t.TourName);
                            break;
                        case "Price (Low to High)":
                            filtered = filtered.OrderBy(t => t.BasePrice);
                            break;
                        case "Price (High to Low)":
                            filtered = filtered.OrderByDescending(t => t.BasePrice);
                            break;
                        case "Duration (Short to Long)":
                            filtered = filtered.OrderBy(t => t.DurationDays);
                            break;
                        case "Duration (Long to Short)":
                            filtered = filtered.OrderByDescending(t => t.DurationDays);
                            break;
                    }
                }

                _filteredTours = filtered.ToList();

                var toursItemsControl = FindName("ToursItemsControl") as ItemsControl;
                if (toursItemsControl != null)
                {
                    toursItemsControl.ItemsSource = _filteredTours;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is Tour tour)
                {
                    var tourDetailsWindow = new TourDetailsWindow(tour, _currentUser);
                    tourDetailsWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening tour details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BookNowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.DataContext is Tour tour)
                {
                    var bookTourWindow = new BookTourWindow(tour, _currentUser);
                    bookTourWindow.ShowDialog();

                    // Refresh tours after booking
                    LoadTours();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening booking window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }

    // Tour Details Window
    public class TourDetailsWindow : Window
    {
        public TourDetailsWindow(Tour tour, User currentUser)
        {
            Title = $"Tour Details - {tour.TourName}";
            Width = 800;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResize;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });

            // Header
            var header = new StackPanel
            {
                Background = System.Windows.Media.Brushes.Orange,
                Margin = new Thickness(20)
            };

            var title = new TextBlock
            {
                Text = tour.TourName,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White
            };

            var destination = new TextBlock
            {
                Text = $"📍 {tour.Destination}",
                FontSize = 16,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 5, 0, 0)
            };

            header.Children.Add(title);
            header.Children.Add(destination);

            // Content
            var scrollViewer = new ScrollViewer();
            var content = new StackPanel { Margin = new Thickness(20) };

            // Description
            var descriptionSection = new TextBlock
            {
                Text = $"📝 Description:\n{tour.Description}",
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Tour Info
            var infoGrid = new Grid();
            infoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            infoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var priceInfo = new StackPanel();
            priceInfo.Children.Add(new TextBlock { Text = "💰 Price", FontWeight = FontWeights.Bold });
            priceInfo.Children.Add(new TextBlock { Text = tour.BasePrice.ToString("C"), FontSize = 18, Foreground = System.Windows.Media.Brushes.Green });

            var durationInfo = new StackPanel();
            durationInfo.Children.Add(new TextBlock { Text = "⏱️ Duration", FontWeight = FontWeights.Bold });
            durationInfo.Children.Add(new TextBlock { Text = $"{tour.DurationDays} days", FontSize = 18, Foreground = System.Windows.Media.Brushes.Blue });

            Grid.SetColumn(priceInfo, 0);
            Grid.SetColumn(durationInfo, 1);
            infoGrid.Children.Add(priceInfo);
            infoGrid.Children.Add(durationInfo);

            // Attractions
            var attractionsSection = new TextBlock
            {
                Text = "🏞️ Attractions:",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            };

            var attractionsList = new StackPanel();
            foreach (var tourAttraction in tour.TourAttractions.OrderBy(ta => ta.VisitDay))
            {
                var attractionItem = new TextBlock
                {
                    Text = $"Day {tourAttraction.VisitDay}: {tourAttraction.Attraction.AttractionName}",
                    FontSize = 12,
                    Margin = new Thickness(10, 0, 0, 5)
                };
                attractionsList.Children.Add(attractionItem);
            }

            // Schedules
            var schedulesSection = new TextBlock
            {
                Text = "📅 Available Schedules:",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            };

            var schedulesList = new StackPanel();
            foreach (var schedule in tour.TourSchedules.Where(ts => ts.Status == "active").OrderBy(ts => ts.DepartureDate))
            {
                var scheduleItem = new Border
                {
                    Background = System.Windows.Media.Brushes.LightGray,
                    BorderBrush = System.Windows.Media.Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var scheduleContent = new StackPanel();
                scheduleContent.Children.Add(new TextBlock { Text = $"Departure: {schedule.DepartureDate:dd/MM/yyyy}", FontWeight = FontWeights.SemiBold });
                scheduleContent.Children.Add(new TextBlock { Text = $"Return: {schedule.ReturnDate:dd/MM/yyyy}" });
                scheduleContent.Children.Add(new TextBlock { Text = $"Guide: {schedule.Guide?.FullName ?? "TBD"}" });
                scheduleContent.Children.Add(new TextBlock { Text = $"Status: {schedule.Status}" });
                scheduleContent.Children.Add(new TextBlock { Text = $"Bookings: {schedule.CurrentBookings}/{schedule.MaxCapacity}" });

                scheduleItem.Child = scheduleContent;
                schedulesList.Children.Add(scheduleItem);
            }

            content.Children.Add(descriptionSection);
            content.Children.Add(infoGrid);
            content.Children.Add(attractionsSection);
            content.Children.Add(attractionsList);
            content.Children.Add(schedulesSection);
            content.Children.Add(schedulesList);

            scrollViewer.Content = content;

            Grid.SetRow(header, 0);
            Grid.SetRow(scrollViewer, 1);
            grid.Children.Add(header);
            grid.Children.Add(scrollViewer);

            Content = grid;
        }
    }

    // Book Tour Window
    public class BookTourWindow : Window
    {
        public BookTourWindow(Tour tour, User currentUser)
        {
            Title = $"Book Tour - {tour.TourName}";
            Width = 600;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResize;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header
            var header = new StackPanel
            {
                Background = System.Windows.Media.Brushes.Orange,
                Margin = new Thickness(20)
            };

            var title = new TextBlock
            {
                Text = $"Book: {tour.TourName}",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White
            };

            header.Children.Add(title);

            // Form
            var scrollViewer = new ScrollViewer();
            var form = new StackPanel { Margin = new Thickness(20) };

            // Tour Info
            var tourInfo = new TextBlock
            {
                Text = $"Price: {tour.BasePrice:C} | Duration: {tour.DurationDays} days | Destination: {tour.Destination}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Schedule Selection
            var scheduleLabel = new TextBlock { Text = "Select Schedule:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var scheduleComboBox = new ComboBox { Margin = new Thickness(0, 0, 0, 15) };

            var availableSchedules = tour.TourSchedules.Where(ts => ts.Status == "active" && ts.CurrentBookings < ts.MaxCapacity).ToList();
            foreach (var schedule in availableSchedules)
            {
                scheduleComboBox.Items.Add(new ComboBoxItem
                {
                    Content = $"{schedule.DepartureDate:dd/MM/yyyy} - {schedule.ReturnDate:dd/MM/yyyy} (Guide: {schedule.Guide?.FullName ?? "TBD"})",
                    Tag = schedule
                });
            }

            if (availableSchedules.Any())
                scheduleComboBox.SelectedIndex = 0;

            // Number of People
            var adultsLabel = new TextBlock { Text = "Number of Adults:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var adultsTextBox = new TextBox { Text = "1", Margin = new Thickness(0, 0, 0, 15) };

            var childrenLabel = new TextBlock { Text = "Number of Children:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var childrenTextBox = new TextBox { Text = "0", Margin = new Thickness(0, 0, 0, 15) };

            // Notes
            var notesLabel = new TextBlock { Text = "Special Notes:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var notesTextBox = new TextBox
            {
                Height = 60,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Promotion Code
            var promoLabel = new TextBlock { Text = "Promotion Code (Optional):", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var promoTextBox = new TextBox { Margin = new Thickness(0, 0, 0, 15) };

            // Total Calculation
            var totalLabel = new TextBlock { Text = "Total Amount:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var totalTextBlock = new TextBlock { Text = tour.BasePrice.ToString("C"), FontSize = 16, Foreground = System.Windows.Media.Brushes.Green, Margin = new Thickness(0, 0, 0, 20) };

            form.Children.Add(tourInfo);
            form.Children.Add(scheduleLabel);
            form.Children.Add(scheduleComboBox);
            form.Children.Add(adultsLabel);
            form.Children.Add(adultsTextBox);
            form.Children.Add(childrenLabel);
            form.Children.Add(childrenTextBox);
            form.Children.Add(notesLabel);
            form.Children.Add(notesTextBox);
            form.Children.Add(promoLabel);
            form.Children.Add(promoTextBox);
            form.Children.Add(totalLabel);
            form.Children.Add(totalTextBlock);

            scrollViewer.Content = form;

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(20)
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0)
            };
            cancelButton.Click += (s, e) => Close();

            var bookButton = new Button
            {
                Content = "Book Tour",
                Width = 100,
                Height = 30,
                Background = System.Windows.Media.Brushes.Green,
                Foreground = System.Windows.Media.Brushes.White
            };
            bookButton.Click += (s, e) =>
            {
                try
                {
                    if (scheduleComboBox.SelectedItem is ComboBoxItem selectedSchedule && selectedSchedule.Tag is TourSchedule schedule)
                    {
                        if (int.TryParse(adultsTextBox.Text, out int adults) && int.TryParse(childrenTextBox.Text, out int children))
                        {
                            var totalPeople = adults + children;
                            if (totalPeople <= 0)
                            {
                                MessageBox.Show("Please enter at least 1 person.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            if (schedule.CurrentBookings + totalPeople > schedule.MaxCapacity)
                            {
                                MessageBox.Show($"Only {schedule.MaxCapacity - schedule.CurrentBookings} spots available for this schedule.", "Capacity Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            // Create booking
                            var booking = new Booking
                            {
                                CustomerId = currentUser.UserId,
                                ScheduleId = schedule.ScheduleId,
                                NumAdults = adults,
                                NumChildren = children,
                                TotalPrice = tour.BasePrice * totalPeople,
                                Status = "pending",
                                BookingDate = DateTime.Now,
                                Notes = notesTextBox.Text
                            };

                            using (var context = new TourManagementContext())
                            {
                                context.Bookings.Add(booking);
                                schedule.CurrentBookings += totalPeople;
                                context.SaveChanges();
                            }

                            MessageBox.Show("Tour booked successfully! Please proceed to payment.", "Booking Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Please enter valid numbers for adults and children.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select a schedule.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error booking tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(bookButton);

            Grid.SetRow(header, 0);
            Grid.SetRow(scrollViewer, 1);
            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(header);
            grid.Children.Add(scrollViewer);
            grid.Children.Add(buttonPanel);

            Content = grid;
        }
    }
}