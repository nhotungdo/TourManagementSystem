using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class BookTourPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentUser;
        private List<Tour> _availableTours;
        private Tour _selectedTour;
        private Promotion _appliedPromotion;

        public BookTourPage(User user)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _currentUser = user;

            LoadAvailableTours();
            LoadContactInfo();
        }

        private void LoadAvailableTours()
        {
            try
            {
                _availableTours = _context.Tours
                    .Where(t => t.IsActive == true)
                    .ToList();

                // Populate tour dropdown
                if (TourComboBox != null)
                {
                    TourComboBox.Items.Clear();
                    TourComboBox.Items.Add(new ComboBoxItem { Content = "Select a tour", Tag = null });

                    foreach (var tour in _availableTours)
                    {
                        TourComboBox.Items.Add(new ComboBoxItem
                        {
                            Content = $"{tour.TourName} - {tour.Destination} ({tour.DurationDays} days)",
                            Tag = tour
                        });
                    }

                    TourComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tours: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadContactInfo()
        {
            try
            {
                if (ContactPhoneTextBox != null)
                {
                    ContactPhoneTextBox.Text = _currentUser.Phone ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading contact info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (TourComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is Tour tour)
                {
                    _selectedTour = tour;
                    UpdateTourDetails();
                    CalculatePrice();
                }
                else
                {
                    _selectedTour = null;
                    TourDetailsBorder.Visibility = Visibility.Collapsed;
                    ClearPriceCalculation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void UpdateTourDetails()
        {
            try
            {
                if (_selectedTour != null)
                {
                    if (PriceTextBlock != null)
                        PriceTextBlock.Text = _selectedTour.BasePrice.ToString("C");
                    if (DurationTextBlock != null)
                        DurationTextBlock.Text = $"{_selectedTour.DurationDays} days";
                    if (DestinationTextBlock != null)
                        DestinationTextBlock.Text = _selectedTour.Destination;
                    if (TourDetailsBorder != null)
                        TourDetailsBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    if (TourDetailsBorder != null)
                        TourDetailsBorder.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating tour details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculatePrice()
        {
            try
            {
                if (_selectedTour == null)
                {
                    ClearPriceCalculation();
                    return;
                }

                // Get number of people
                int adults = 0;
                int children = 0;

                if (AdultsTextBox != null)
                {
                    if (!int.TryParse(AdultsTextBox.Text, out adults) || adults < 0)
                    {
                        adults = 0;
                        AdultsTextBox.Text = "0";
                    }
                }

                if (ChildrenTextBox != null)
                {
                    if (!int.TryParse(ChildrenTextBox.Text, out children) || children < 0)
                    {
                        children = 0;
                        ChildrenTextBox.Text = "0";
                    }
                }

                var totalPeople = adults + children;
                if (TotalPeopleTextBlock != null)
                    TotalPeopleTextBlock.Text = totalPeople.ToString();

                if (totalPeople == 0)
                {
                    ClearPriceCalculation();
                    return;
                }

                // Calculate base price with 75% discount for children
                var adultPrice = _selectedTour.BasePrice * adults;
                var childrenPrice = _selectedTour.BasePrice * children * 0.25m; 
                var basePrice = adultPrice + childrenPrice;

                if (BasePriceTextBlock != null)
                    BasePriceTextBlock.Text = basePrice.ToString("C");







                // Apply promotion if any
                var discount = 0m;
                if (PromotionCodeTextBox != null && !string.IsNullOrWhiteSpace(PromotionCodeTextBox.Text))
                {
                    _appliedPromotion = _context.Promotions
                        .FirstOrDefault(p => p.PromotionCode == PromotionCodeTextBox.Text.Trim() &&
                                           p.IsActive == true &&
                                           p.EndDate > DateTime.Today);


                    if (_appliedPromotion != null)
                    {
                        discount = basePrice * (_appliedPromotion.DiscountPercentage / 100m);
                    }
                }
                else
                {
                    _appliedPromotion = null;
                }

                if (DiscountTextBlock != null)
                    DiscountTextBlock.Text = discount.ToString("C");

                // Calculate final price
                var finalPrice = basePrice - discount;
                if (FinalPriceTextBlock != null)
                    FinalPriceTextBlock.Text = finalPrice.ToString("C");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating price: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AdultsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePrice();
        }

        private void ChildrenTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePrice();
        }

        private void PromotionCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculatePrice();
        }

        private void ClearPriceCalculation()
        {
            try
            {
                if (BasePriceTextBlock != null)
                    BasePriceTextBlock.Text = "$0.00";
                if (DiscountTextBlock != null)
                    DiscountTextBlock.Text = "$0.00";
                if (TotalPeopleTextBlock != null)
                    TotalPeopleTextBlock.Text = "0";
                if (FinalPriceTextBlock != null)
                    FinalPriceTextBlock.Text = "$0.00";
            }
            catch (Exception ex)
            {
                // Silently handle the error as controls might not be initialized yet
                System.Diagnostics.Debug.WriteLine($"Error in ClearPriceCalculation: {ex.Message}");
            }
        }

        private void ResetFormButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TourComboBox != null)
                    TourComboBox.SelectedIndex = 0;
                if (AdultsTextBox != null)
                    AdultsTextBox.Text = "1";
                if (ChildrenTextBox != null)
                    ChildrenTextBox.Text = "0";
                if (PromotionCodeTextBox != null)
                    PromotionCodeTextBox.Text = "";
                if (NotesTextBox != null)
                    NotesTextBox.Text = "";
                LoadContactInfo();
                if (TourDetailsBorder != null)
                    TourDetailsBorder.Visibility = Visibility.Collapsed;
                ClearPriceCalculation();
                _selectedTour = null;
                _appliedPromotion = null;

                MessageBox.Show("Form has been reset.", "Reset", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting form: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BookTourButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate form
                if (_selectedTour == null)
                {
                    MessageBox.Show("Please select a tour.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }



                int adults = 0;
                int children = 0;

                if (AdultsTextBox == null || !int.TryParse(AdultsTextBox.Text, out adults) || adults < 0)
                {
                    MessageBox.Show("Please enter a valid number of adults.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ChildrenTextBox == null || !int.TryParse(ChildrenTextBox.Text, out children) || children < 0)
                {
                    MessageBox.Show("Please enter a valid number of children.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var totalPeople = adults + children;
                if (totalPeople == 0)
                {
                    MessageBox.Show("Please enter at least 1 person.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Calculate final price
                var basePrice = _selectedTour.BasePrice * totalPeople;
                var discount = _appliedPromotion != null ? basePrice * (_appliedPromotion.DiscountPercentage / 100m) : 0m;
                var finalPrice = basePrice - discount;

                // Create a booking with automatic schedule creation
                try
                {
                    // Find an available schedule for this tour
                    var availableSchedule = _context.TourSchedules
                        .Where(ts => ts.TourId == _selectedTour.TourId &&
                                   (ts.Status == "scheduled" || ts.Status == "ongoing") &&
                                   ts.DepartureDate > DateOnly.FromDateTime(DateTime.Today) &&
                                   (ts.MaxCapacity - (ts.CurrentBookings ?? 0)) >= totalPeople)
                        .OrderBy(ts => ts.DepartureDate)
                        .FirstOrDefault();

                    // If no available schedule, create a new one
                    if (availableSchedule == null)
                    {
                        // Find a staff member to assign as guide
                        var availableGuide = _context.Users
                            .Where(u => u.Role.ToLower() == "staff" && u.IsActive == true)
                            .FirstOrDefault();

                        if (availableGuide == null)
                        {
                            MessageBox.Show("No available guides found. Please contact customer service for assistance.", "No Available Guides", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Create a new schedule starting from tomorrow
                        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
                        var returnDate = tomorrow.AddDays(_selectedTour.DurationDays - 1);

                        // Get next schedule ID
                        var maxScheduleId = _context.TourSchedules.Any() ? _context.TourSchedules.Max(ts => ts.ScheduleId) : 0;
                        var nextScheduleId = maxScheduleId + 1;

                        availableSchedule = new TourSchedule
                        {
                            ScheduleId = nextScheduleId,
                            TourId = _selectedTour.TourId,
                            DepartureDate = tomorrow,
                            ReturnDate = returnDate,
                            MaxCapacity = 20, // Default capacity
                            CurrentBookings = 0,
                            GuideId = availableGuide.UserId,
                            Status = "scheduled"
                        };

                        _context.TourSchedules.Add(availableSchedule);
                        _context.SaveChanges();

                        // Load the guide information for the new schedule
                        availableSchedule.Guide = availableGuide;
                    }

                    // Get next booking ID
                    var maxBookingId = _context.Bookings.Any() ? _context.Bookings.Max(b => b.BookingId) : 0;
                    var nextBookingId = maxBookingId + 1;

                    // Create booking
                    var booking = new Booking
                    {
                        BookingId = nextBookingId,
                        CustomerId = _currentUser.UserId,
                        ScheduleId = availableSchedule.ScheduleId,
                        NumAdults = adults,
                        NumChildren = children,
                        TotalPrice = finalPrice,
                        Status = "pending",
                        PaymentStatus = "unpaid",
                        BookingDate = DateTime.Now,
                        Notes = NotesTextBox?.Text?.Trim() ?? ""
                    };

                    // Update schedule capacity
                    availableSchedule.CurrentBookings = (availableSchedule.CurrentBookings ?? 0) + totalPeople;

                    // Save to database
                    _context.Bookings.Add(booking);
                    _context.SaveChanges();

                    // Show success message
                    var result = MessageBox.Show(
                        $"Tour booked successfully!\n\n" +
                            $"Booking ID: {booking.BookingId}\n" +
                        $"Tour: {_selectedTour.TourName}\n" +
                            $"Schedule: {availableSchedule.DepartureDate:dd/MM/yyyy} - {availableSchedule.ReturnDate:dd/MM/yyyy}\n" +
                            $"Guide: {availableSchedule.Guide?.FullName ?? "TBD"}\n" +
                        $"Total Price: {finalPrice:C}\n\n" +
                        $"Would you like to proceed to payment now?",
                        "Booking Confirmed",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Navigate to payment page or show payment dialog
                        MessageBox.Show("Payment functionality will be implemented soon. Please check 'My Bookings' for your booking details.", "Payment", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    // Reset form after successful booking
                    ResetFormButton_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating booking: {ex.Message}", "Booking Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error booking tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}