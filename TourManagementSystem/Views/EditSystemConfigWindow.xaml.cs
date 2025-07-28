using System;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class EditSystemConfigWindow : Window
    {
        private readonly SystemConfig _config;
        private readonly ActivityLogService _activityLogService;
        private readonly TourManagementContext _context;

        public EditSystemConfigWindow(SystemConfig config, TourManagementContext context, ActivityLogService activityLogService)
        {
            InitializeComponent();
            _config = config;
            _context = context;
            _activityLogService = activityLogService;

            LoadConfigData();
        }

        private void LoadConfigData()
        {
            try
            {
                // Load configuration data into form
                ConfigKeyTextBox.Text = _config.ConfigKey;
                ValueTextBox.Text = _config.ConfigValue;
                DescriptionTextBox.Text = _config.Description ?? "";
                CategoryTextBox.Text = _config.Category;
                UpdatedByTextBox.Text = _config.UpdatedBy?.ToString() ?? "Admin System";
                UpdatedAtTextBox.Text = _config.UpdatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

                // Set status
                if (_config.IsActive == true)
                {
                    StatusComboBox.SelectedIndex = 0; // Active
                }
                else
                {
                    StatusComboBox.SelectedIndex = 1; // Inactive
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateConfigButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    UpdateConfigFromForm();

                    // Update in database
                    var configToUpdate = _context.SystemConfigs.Find(_config.ConfigId);
                    if (configToUpdate != null)
                    {
                        configToUpdate.ConfigValue = _config.ConfigValue;
                        configToUpdate.Description = _config.Description;
                        configToUpdate.IsActive = _config.IsActive;
                        configToUpdate.UpdatedBy = 1; // Admin user ID
                        configToUpdate.UpdatedAt = DateTime.Now;
                        _context.SaveChanges();

                        // Log activity
                        _activityLogService.LogActivity(1, "UPDATE", "SystemConfig", _config.ConfigId,
                            $"Updated system configuration: {_config.ConfigKey}");

                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Configuration not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Value validation
            if (string.IsNullOrWhiteSpace(ValueTextBox.Text))
            {
                MessageBox.Show("Configuration value is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ValueTextBox.Focus();
                return false;
            }

            // Description validation (optional but if provided, check length)
            if (!string.IsNullOrWhiteSpace(DescriptionTextBox.Text) && DescriptionTextBox.Text.Length > 200)
            {
                MessageBox.Show("Description must not exceed 200 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DescriptionTextBox.Focus();
                return false;
            }

            // Status validation
            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                StatusComboBox.Focus();
                return false;
            }

            return true;
        }

        private void UpdateConfigFromForm()
        {
            // Update configuration properties
            _config.ConfigValue = ValueTextBox.Text.Trim();
            _config.Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim();

            // Set status based on ComboBox selection
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                _config.IsActive = selectedItem.Content.ToString() == "Active";
            }
        }
    }
}