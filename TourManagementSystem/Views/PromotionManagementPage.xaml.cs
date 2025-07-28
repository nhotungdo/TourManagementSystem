using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class PromotionManagementPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly PromotionService _promotionService;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _promotionsView;
        private List<Promotion> _allPromotions;

        public PromotionManagementPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);
            _promotionService = new PromotionService(_context, _activityLogService);

            LoadPromotions();
        }

        private void LoadPromotions()
        {
            try
            {
                _allPromotions = _context.Promotions.ToList();
                _promotionsView = CollectionViewSource.GetDefaultView(_allPromotions);
                PromotionsDataGrid.ItemsSource = _promotionsView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading promotions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (_promotionsView == null) return;

            _promotionsView.Filter = item =>
            {
                if (item is not Promotion promotion) return false;

                // Search filter
                var searchText = SearchTextBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = promotion.PromotionCode?.ToLower().Contains(searchText) == true ||
                                       promotion.PromotionName?.ToLower().Contains(searchText) == true ||
                                       promotion.Description?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // Status filter
                var selectedStatus = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All Status")
                {
                    if (selectedStatus == "Active" && promotion.IsActive != true) return false;
                    if (selectedStatus == "Inactive" && promotion.IsActive != false) return false;
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

        private void PromotionsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void PromotionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void AddPromotionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addPromotionWindow = new AddPromotionWindow(_promotionService, _activityLogService);
                if (addPromotionWindow.ShowDialog() == true)
                {
                    LoadPromotions(); // Refresh data
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add Promotion window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPromotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int promotionId)
                {
                    var promotion = _allPromotions.FirstOrDefault(p => p.PromotionId == promotionId);
                    if (promotion != null)
                    {
                        var editPromotionWindow = new EditPromotionWindow(promotion, _promotionService, _activityLogService);
                        if (editPromotionWindow.ShowDialog() == true)
                        {
                            LoadPromotions(); // Refresh data
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing promotion: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeletePromotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int promotionId)
                {
                    var promotion = _allPromotions.FirstOrDefault(p => p.PromotionId == promotionId);
                    if (promotion != null)
                    {
                        var result = MessageBox.Show(
                            $"Are you sure you want to delete the promotion '{promotion.PromotionCode}'?",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            if (_promotionService.DeletePromotion(promotionId, 1)) // 1 is admin ID
                            {
                                LoadPromotions(); // Refresh data
                                MessageBox.Show("Promotion deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete promotion.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting promotion: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}