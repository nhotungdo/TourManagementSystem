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
    public partial class SystemConfigPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;
        private ICollectionView _configsView;
        private List<SystemConfig> _allConfigs;

        public SystemConfigPage()
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);

            LoadSystemConfigs();
            SetupEventHandlers();
        }

        private void LoadSystemConfigs()
        {
            try
            {
                _allConfigs = _context.SystemConfigs
                    .Include(sc => sc.UpdatedByNavigation)
                    .OrderBy(sc => sc.Category)
                    .ThenBy(sc => sc.ConfigKey)
                    .ToList();

                _configsView = CollectionViewSource.GetDefaultView(_allConfigs);
                SystemConfigsDataGrid.ItemsSource = _configsView;

                // Apply initial filter
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading system configurations: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupEventHandlers()
        {
            // Setup sorting
            SystemConfigsDataGrid.Sorting += SystemConfigsDataGrid_Sorting;
        }

        private void ApplyFilters()
        {
            if (_configsView == null) return;

            _configsView.Filter = config =>
            {
                if (config is not SystemConfig configObj) return false;

                // Search filter
                var searchText = SearchTextBox.Text?.ToLower() ?? "";
                if (!string.IsNullOrEmpty(searchText))
                {
                    var matchesSearch = configObj.ConfigKey?.ToLower().Contains(searchText) == true ||
                                       configObj.ConfigValue?.ToLower().Contains(searchText) == true ||
                                       configObj.Description?.ToLower().Contains(searchText) == true ||
                                       configObj.Category?.ToLower().Contains(searchText) == true;
                    if (!matchesSearch) return false;
                }

                // Category filter
                var selectedCategory = (CategoryFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "All Categories")
                {
                    if (configObj.Category != selectedCategory) return false;
                }

                return true;
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SystemConfigsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Handle sorting if needed
        }

        private void SystemConfigsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        private void AddConfigButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Add system config functionality will be implemented soon.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                // var addConfigWindow = new AddSystemConfigWindow(_context, _activityLogService);
                // if (addConfigWindow.ShowDialog() == true)
                // {
                //     LoadSystemConfigs(); // Refresh the list
                //     MessageBox.Show("System configuration added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding system configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int configId)
                {
                    var config = _allConfigs.FirstOrDefault(c => c.ConfigId == configId);
                    if (config != null)
                    {
                        var editConfigWindow = new EditSystemConfigWindow(config, _context, _activityLogService);
                        if (editConfigWindow.ShowDialog() == true)
                        {
                            LoadSystemConfigs(); // Refresh the list
                            MessageBox.Show("System configuration updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing system configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is int configId)
                {
                    var config = _allConfigs.FirstOrDefault(c => c.ConfigId == configId);
                    if (config != null)
                    {
                        var result = MessageBox.Show(
                            $"Are you sure you want to delete this configuration?\n\nKey: {config.ConfigKey}\nValue: {config.ConfigValue}\nCategory: {config.Category}",
                            "Confirm Delete",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            _context.SystemConfigs.Remove(config);
                            _context.SaveChanges();

                            // Log activity
                            _activityLogService.LogActivity(1, "DELETE", "SystemConfig", configId,
                                $"Deleted system configuration: {config.ConfigKey}");

                            LoadSystemConfigs(); // Refresh the list
                            MessageBox.Show("System configuration deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting system configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}