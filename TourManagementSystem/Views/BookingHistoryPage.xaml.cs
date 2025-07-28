using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class BookingHistoryPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentUser;
        private List<Booking> _allBookings;

        public BookingHistoryPage(User user)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _currentUser = user;

            LoadBookingHistory();
        }

        private void LoadBookingHistory()
        {
            try
            {
                _allBookings = _context.Bookings
                    .Include(b => b.Schedule)
                    .Include(b => b.Schedule.Tour)
                    .Include(b => b.Schedule.Guide)
                    .Include(b => b.Payments)
                    .Include(b => b.Reviews)
                    .Where(b => b.CustomerId == _currentUser.UserId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToList();

                InitializeFilters();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading booking history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _allBookings = new List<Booking>();
            }
        }

        private void InitializeFilters()
        {
            try
            {
                // Initialize status filter if empty
                if (StatusFilterComboBox?.Items.Count == 0)
                {
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "All Bookings" });
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "Pending" });
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "Confirmed" });
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "Cancelled" });
                    StatusFilterComboBox.Items.Add(new ComboBoxItem { Content = "Completed" });
                    StatusFilterComboBox.SelectedIndex = 0;
                }

                // Initialize year filter if empty
                if (YearFilterComboBox?.Items.Count == 0)
                {
                    YearFilterComboBox.Items.Add(new ComboBoxItem { Content = "All Years" });
                    var currentYear = DateTime.Now.Year;
                    for (int year = currentYear; year >= currentYear - 5; year--)
                    {
                        YearFilterComboBox.Items.Add(new ComboBoxItem { Content = year.ToString() });
                    }
                    YearFilterComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (_allBookings == null)
                {
                    DisplayBookingHistory(new List<Booking>());
                    return;
                }

                var filteredBookings = _allBookings.AsQueryable();

                // Apply status filter
                if (StatusFilterComboBox?.SelectedItem is ComboBoxItem statusItem)
                {
                    var statusText = statusItem.Content?.ToString();
                    if (!string.IsNullOrEmpty(statusText) && statusText != "All Bookings")
                    {
                        filteredBookings = filteredBookings.Where(b => (b.Status ?? "").ToLower() == statusText.ToLower());
                    }
                }

                // Apply year filter
                if (YearFilterComboBox?.SelectedItem is ComboBoxItem yearItem)
                {
                    var yearText = yearItem.Content?.ToString();
                    if (!string.IsNullOrEmpty(yearText) && yearText != "All Years" && int.TryParse(yearText, out int year))
                    {
                        filteredBookings = filteredBookings.Where(b => b.BookingDate.HasValue && b.BookingDate.Value.Year == year);
                    }
                }

                DisplayBookingHistory(filteredBookings.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayBookingHistory(List<Booking> bookings)
        {
            try
            {
                if (HistoryStackPanel == null)
                {
                    return;
                }

                HistoryStackPanel.Children.Clear();

                if (!bookings.Any())
                {
                    var noBookingsText = new TextBlock
                    {
                        Text = "No booking history found matching your criteria.",
                        FontSize = 16,
                        Foreground = Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 50, 0, 0)
                    };
                    HistoryStackPanel.Children.Add(noBookingsText);
                    return;
                }

                foreach (var booking in bookings)
                {
                    var historyCard = CreateHistoryCard(booking);
                    HistoryStackPanel.Children.Add(historyCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying booking history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateHistoryCard(Booking booking)
        {
            var card = new Border
            {
                Style = FindResource("HistoryCard") as Style
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Left side - Tour and Schedule info
            var leftPanel = new StackPanel();

            var tourName = new TextBlock
            {
                Text = booking.Schedule.Tour.TourName,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var destination = new TextBlock
            {
                Text = $"📍 {booking.Schedule.Tour.Destination}",
                FontSize = 14,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var schedule = new TextBlock
            {
                Text = $"📅 {booking.Schedule.DepartureDate:dd/MM/yyyy} - {booking.Schedule.ReturnDate:dd/MM/yyyy}",
                FontSize = 14,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var bookingDate = new TextBlock
            {
                Text = $"📋 Booked on: {booking.BookingDate:dd/MM/yyyy}",
                FontSize = 14,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var people = new TextBlock
            {
                Text = $"👥 {booking.NumAdults} adults, {booking.NumChildren} children",
                FontSize = 14,
                Foreground = Brushes.Gray
            };

            leftPanel.Children.Add(tourName);
            leftPanel.Children.Add(destination);
            leftPanel.Children.Add(schedule);
            leftPanel.Children.Add(bookingDate);
            leftPanel.Children.Add(people);

            // Center - Status and Payment info
            var centerPanel = new StackPanel();

            // Status
            var statusColor = GetStatusColor(booking.Status);
            var statusBorder = new Border
            {
                Background = statusColor,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var statusText = new TextBlock
            {
                Text = booking.Status.ToUpper(),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            statusBorder.Child = statusText;

            // Payment info
            var totalPaid = booking.Payments?.Sum(p => p.Amount) ?? 0;
            var paymentStatus = totalPaid >= booking.TotalPrice ? "Paid" :
                               totalPaid > 0 ? "Partial" : "Unpaid";

            var paymentText = new TextBlock
            {
                Text = $"💳 {paymentStatus}",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = GetPaymentStatusColor(paymentStatus),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var amountText = new TextBlock
            {
                Text = $"💰 {booking.TotalPrice:C}",
                FontSize = 14,
                Foreground = Brushes.Black,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // Review status
            var hasReview = booking.Reviews?.Any() == true;
            var reviewText = new TextBlock
            {
                Text = hasReview ? "⭐ Reviewed" : "📝 Not Reviewed",
                FontSize = 14,
                Foreground = hasReview ? Brushes.Green : Brushes.Orange
            };

            centerPanel.Children.Add(statusBorder);
            centerPanel.Children.Add(paymentText);
            centerPanel.Children.Add(amountText);
            centerPanel.Children.Add(reviewText);

            // Right side - Action buttons
            var rightPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };

            var viewDetailsButton = new Button
            {
                Content = "👁️ View Details",
                Style = FindResource("ActionButton") as Style,
                Margin = new Thickness(0, 0, 0, 5),
                Tag = booking
            };
            viewDetailsButton.Click += ViewDetailsButton_Click;

            var reviewButton = new Button
            {
                Content = hasReview ? "✏️ Edit Review" : "⭐ Write Review",
                Style = FindResource("ActionButton") as Style,
                Background = hasReview ? Brushes.Orange : Brushes.Green,
                Tag = booking
            };
            reviewButton.Click += ReviewButton_Click;

            // Only show review button for completed tours
            if (booking.Status.ToLower() == "completed")
            {
                rightPanel.Children.Add(reviewButton);
            }

            rightPanel.Children.Add(viewDetailsButton);

            Grid.SetColumn(leftPanel, 0);
            Grid.SetColumn(centerPanel, 1);
            Grid.SetColumn(rightPanel, 2);

            grid.Children.Add(leftPanel);
            grid.Children.Add(centerPanel);
            grid.Children.Add(rightPanel);

            card.Child = grid;
            return card;
        }

        private Brush GetStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "pending" => Brushes.Orange,
                "confirmed" => Brushes.Green,
                "cancelled" => Brushes.Red,
                "completed" => Brushes.Blue,
                _ => Brushes.Gray
            };
        }

        private Brush GetPaymentStatusColor(string paymentStatus)
        {
            return paymentStatus switch
            {
                "Paid" => Brushes.Green,
                "Partial" => Brushes.Orange,
                "Unpaid" => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is Booking booking)
                {
                    var detailsWindow = new BookingHistoryDetailsWindow(booking);
                    detailsWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing booking details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is Booking booking)
                {
                    var reviewWindow = new ReviewWindow(booking, _currentUser);
                    reviewWindow.ShowDialog();

                    // Refresh to show updated review status
                    LoadBookingHistory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening review window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void YearFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBookingHistory();
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }

    // Booking History Details Window
    public class BookingHistoryDetailsWindow : Window
    {
        public BookingHistoryDetailsWindow(Booking booking)
        {
            Title = $"Booking History Details - {booking.Schedule.Tour.TourName}";
            Width = 600;
            Height = 500;
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
                Background = Brushes.Blue,
                Margin = new Thickness(20)
            };

            var title = new TextBlock
            {
                Text = booking.Schedule.Tour.TourName,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };

            header.Children.Add(title);

            // Content
            var scrollViewer = new ScrollViewer();
            var content = new StackPanel { Margin = new Thickness(20) };

            // Booking Info
            var bookingInfo = new TextBlock
            {
                Text = $"Booking ID: {booking.BookingId}\n" +
                  $"Booking Date: {booking.BookingDate:dd/MM/yyyy HH:mm}\n" +
                  $"Status: {booking.Status}\n" +
                  $"Total Price: {booking.TotalPrice:C}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Tour Info
            var tourInfo = new TextBlock
            {
                Text = $"Tour: {booking.Schedule.Tour.TourName}\n" +
                      $"Destination: {booking.Schedule.Tour.Destination}\n" +
                      $"Duration: {booking.Schedule.Tour.DurationDays} days\n" +
                      $"Description: {booking.Schedule.Tour.Description}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Schedule Info
            var scheduleInfo = new TextBlock
            {
                Text = $"Departure: {booking.Schedule.DepartureDate:dd/MM/yyyy}\n" +
                  $"Return: {booking.Schedule.ReturnDate:dd/MM/yyyy}\n" +
                  $"Guide: {booking.Schedule.Guide?.FullName ?? "TBD"}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Payment History
            var totalPaid = booking.Payments?.Sum(p => p.Amount) ?? 0;
            var paymentInfo = new TextBlock
            {
                Text = $"Total Paid: {totalPaid:C}\n" +
                  $"Remaining: {(booking.TotalPrice - totalPaid):C}",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 20)
            };

            // Review Info
            if (booking.Reviews?.Any() == true)
            {
                var review = booking.Reviews.First();
                var reviewInfo = new TextBlock
                {
                    Text = $"Review Rating: {review.Rating}/5 stars\n" +
                          $"Review Date: {review.ReviewDate:dd/MM/yyyy}\n" +
                          $"Comment: {review.Comment}",
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                content.Children.Add(reviewInfo);
            }

            // Notes
            if (!string.IsNullOrWhiteSpace(booking.Notes))
            {
                var notesInfo = new TextBlock
                {
                    Text = $"Notes: {booking.Notes}",
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                content.Children.Add(notesInfo);
            }

            content.Children.Add(bookingInfo);
            content.Children.Add(tourInfo);
            content.Children.Add(scheduleInfo);
            content.Children.Add(paymentInfo);

            scrollViewer.Content = content;

            Grid.SetRow(header, 0);
            Grid.SetRow(scrollViewer, 1);
            grid.Children.Add(header);
            grid.Children.Add(scrollViewer);

            Content = grid;
        }
    }

    // Review Window
    public class ReviewWindow : Window
    {
        public ReviewWindow(Booking booking, User currentUser)
        {
            Title = $"Review - {booking.Schedule.Tour.TourName}";
            Width = 500;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResize;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header
            var header = new StackPanel
            {
                Background = Brushes.Green,
                Margin = new Thickness(20)
            };

            var title = new TextBlock
            {
                Text = "Write Review",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White
            };

            header.Children.Add(title);

            // Form
            var scrollViewer = new ScrollViewer();
            var form = new StackPanel { Margin = new Thickness(20) };

            // Rating
            var ratingLabel = new TextBlock { Text = "Rating:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var ratingComboBox = new ComboBox { Margin = new Thickness(0, 0, 0, 15) };
            for (int i = 1; i <= 5; i++)
            {
                ratingComboBox.Items.Add($"{i} star{(i > 1 ? "s" : "")}");
            }
            ratingComboBox.SelectedIndex = 4; // Default to 5 stars

            // Comment
            var commentLabel = new TextBlock { Text = "Review Comment:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) };
            var commentTextBox = new TextBox
            {
                Height = 100,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Load existing review if any
            var existingReview = booking.Reviews?.FirstOrDefault();
            if (existingReview != null)
            {
                ratingComboBox.SelectedIndex = existingReview.Rating - 1;
                commentTextBox.Text = existingReview.Comment;
            }

            form.Children.Add(ratingLabel);
            form.Children.Add(ratingComboBox);
            form.Children.Add(commentLabel);
            form.Children.Add(commentTextBox);

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

            var submitButton = new Button
            {
                Content = existingReview != null ? "Update Review" : "Submit Review",
                Width = 100,
                Height = 30,
                Background = Brushes.Green,
                Foreground = Brushes.White
            };
            submitButton.Click += (s, e) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(commentTextBox.Text))
                    {
                        MessageBox.Show("Please enter a review comment.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var rating = ratingComboBox.SelectedIndex + 1;
                    var comment = commentTextBox.Text.Trim();

                    using (var context = new TourManagementContext())
                    {
                        if (existingReview != null)
                        {
                            // Update existing review
                            var reviewToUpdate = context.Reviews.FirstOrDefault(r => r.ReviewId == existingReview.ReviewId);
                            if (reviewToUpdate != null)
                            {
                                reviewToUpdate.Rating = (byte)rating;
                                reviewToUpdate.Comment = comment;
                                reviewToUpdate.ReviewDate = DateTime.Now;
                            }
                        }
                        else
                        {
                            // Create new review
                            var newReview = new Review
                            {
                                BookingId = booking.BookingId,
                                CustomerId = currentUser.UserId,
                                TourId = booking.Schedule.TourId,
                                Rating = (byte)rating,
                                Comment = comment,
                                ReviewDate = DateTime.Now
                            };
                            context.Reviews.Add(newReview);
                        }

                        context.SaveChanges();
                    }

                    MessageBox.Show("Review submitted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error submitting review: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(submitButton);

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