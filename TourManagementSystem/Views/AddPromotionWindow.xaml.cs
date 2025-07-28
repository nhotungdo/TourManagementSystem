using System;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class AddPromotionWindow : Window
    {
        private readonly PromotionService _promotionService;
        private readonly ActivityLogService _activityLogService;

        public AddPromotionWindow(PromotionService promotionService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _promotionService = promotionService;
            _activityLogService = activityLogService;
        }

        private void AddPromotionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    var promotion = CreatePromotionFromForm();

                    if (_promotionService.CreatePromotion(promotion, 1)) // 1 is admin ID
                    {
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create promotion. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating promotion: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Code validation
            if (string.IsNullOrWhiteSpace(CodeTextBox.Text))
            {
                MessageBox.Show("Promotion code is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CodeTextBox.Focus();
                return false;
            }

            if (CodeTextBox.Text.Length < 3)
            {
                MessageBox.Show("Promotion code must be at least 3 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CodeTextBox.Focus();
                return false;
            }

            // Description validation
            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                MessageBox.Show("Description is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DescriptionTextBox.Focus();
                return false;
            }

            // Discount percentage validation
            if (string.IsNullOrWhiteSpace(DiscountPercentageTextBox.Text))
            {
                MessageBox.Show("Discount percentage is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DiscountPercentageTextBox.Focus();
                return false;
            }

            if (!decimal.TryParse(DiscountPercentageTextBox.Text, out decimal discountPercentage) || discountPercentage <= 0 || discountPercentage > 100)
            {
                MessageBox.Show("Discount percentage must be between 0 and 100.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DiscountPercentageTextBox.Focus();
                return false;
            }

            // Max discount amount validation
            if (string.IsNullOrWhiteSpace(MaxDiscountAmountTextBox.Text))
            {
                MessageBox.Show("Max discount amount is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MaxDiscountAmountTextBox.Focus();
                return false;
            }

            if (!decimal.TryParse(MaxDiscountAmountTextBox.Text, out decimal maxDiscountAmount) || maxDiscountAmount <= 0)
            {
                MessageBox.Show("Max discount amount must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MaxDiscountAmountTextBox.Focus();
                return false;
            }

            // Minimum amount validation
            if (string.IsNullOrWhiteSpace(MinimumAmountTextBox.Text))
            {
                MessageBox.Show("Minimum amount is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MinimumAmountTextBox.Focus();
                return false;
            }

            if (!decimal.TryParse(MinimumAmountTextBox.Text, out decimal minimumAmount) || minimumAmount <= 0)
            {
                MessageBox.Show("Minimum amount must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MinimumAmountTextBox.Focus();
                return false;
            }

            // Start date validation
            if (!StartDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Start date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                StartDatePicker.Focus();
                return false;
            }

            if (StartDatePicker.SelectedDate.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Start date cannot be in the past.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                StartDatePicker.Focus();
                return false;
            }

            // End date validation
            if (!EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("End date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EndDatePicker.Focus();
                return false;
            }

            if (EndDatePicker.SelectedDate.Value.Date <= StartDatePicker.SelectedDate.Value.Date)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EndDatePicker.Focus();
                return false;
            }

            return true;
        }

        private Promotion CreatePromotionFromForm()
        {
            var promotion = new Promotion
            {
                PromotionCode = CodeTextBox.Text.Trim().ToUpper(),
                PromotionName = CodeTextBox.Text.Trim().ToUpper(), // Using code as name for now
                Description = DescriptionTextBox.Text.Trim(),
                DiscountPercentage = decimal.Parse(DiscountPercentageTextBox.Text),
                DiscountAmount = decimal.Parse(MaxDiscountAmountTextBox.Text),
                MinBookingAmount = decimal.Parse(MinimumAmountTextBox.Text),
                StartDate = StartDatePicker.SelectedDate.Value,
                EndDate = EndDatePicker.SelectedDate.Value,
                IsActive = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Active",
                CreatedBy = 1, // Admin ID
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return promotion;
        }
    }
}