using System;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditPromotionWindow : Window
    {
        private readonly Promotion _promotion;
        private readonly PromotionService _promotionService;
        private readonly ActivityLogService _activityLogService;

        public EditPromotionWindow(Promotion promotion, PromotionService promotionService, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _promotion = promotion;
            _promotionService = promotionService;
            _activityLogService = activityLogService;

            LoadPromotionData();
        }

        private void LoadPromotionData()
        {
            try
            {
                // Load promotion code (read-only)
                CodeTextBox.Text = _promotion.PromotionCode;

                // Load description
                DescriptionTextBox.Text = _promotion.Description;

                // Load discount percentage
                DiscountPercentageTextBox.Text = _promotion.DiscountPercentage.ToString();

                // Load discount amount
                MaxDiscountAmountTextBox.Text = _promotion.DiscountAmount?.ToString() ?? "0";

                // Load minimum amount
                MinimumAmountTextBox.Text = _promotion.MinBookingAmount?.ToString() ?? "0";

                // Load dates
                StartDatePicker.SelectedDate = _promotion.StartDate;
                EndDatePicker.SelectedDate = _promotion.EndDate;

                // Load status
                foreach (ComboBoxItem item in StatusComboBox.Items)
                {
                    if (item.Content.ToString() == (_promotion.IsActive == true ? "Active" : "Inactive"))
                    {
                        StatusComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading promotion data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePromotionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    UpdatePromotionFromForm();

                    if (_promotionService.UpdatePromotion(_promotion, 1)) // 1 is admin ID
                    {
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update promotion. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating promotion: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
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

        private void UpdatePromotionFromForm()
        {
            _promotion.Description = DescriptionTextBox.Text.Trim();
            _promotion.DiscountPercentage = decimal.Parse(DiscountPercentageTextBox.Text);
            _promotion.DiscountAmount = decimal.Parse(MaxDiscountAmountTextBox.Text);
            _promotion.MinBookingAmount = decimal.Parse(MinimumAmountTextBox.Text);
            _promotion.StartDate = StartDatePicker.SelectedDate.Value;
            _promotion.EndDate = EndDatePicker.SelectedDate.Value;
            _promotion.IsActive = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Active";
            _promotion.UpdatedAt = DateTime.Now;
        }
    }
}