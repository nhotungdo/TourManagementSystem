using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using TourManagementSystem.Models;
using TourManagementSystem.Services;

namespace TourManagementSystem.Views
{
    public partial class ReportsPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly ActivityLogService _activityLogService;

        // Control references
        private TextBlock? _totalToursText;
        private TextBlock? _totalBookingsText;
        private TextBlock? _totalRevenueText;
        private TextBlock? _activeUsersText;
        private DataGrid? _recentBookingsDataGrid;
        private DataGrid? _popularToursDataGrid;

        public ReportsPage()
        {
            // Initialize without XAML compilation dependency
            InitializePageWithoutXaml();

            _context = new TourManagementContext();
            _activityLogService = new ActivityLogService(_context);

            // Initialize control references
            InitializeControls();

            LoadStatistics();
            LoadRecentBookings();
            LoadPopularTours();
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
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header
            var headerPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 20) };
            var titleText = new TextBlock
            {
                Text = "📊 Reports and Analytics",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };
            var subtitleText = new TextBlock
            {
                Text = "Comprehensive reports and data analytics for tour management",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray
            };
            headerPanel.Children.Add(titleText);
            headerPanel.Children.Add(subtitleText);
            Grid.SetRow(headerPanel, 0);
            mainGrid.Children.Add(headerPanel);

            // Action buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 20)
            };

            var refreshButton = new Button
            {
                Content = "🔄 Refresh Data",
                Margin = new Thickness(5),
                Padding = new Thickness(15, 8, 15, 8),
                Background = System.Windows.Media.Brushes.DodgerBlue,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0)
            };
            refreshButton.Click += RefreshButton_Click;

            var generateButton = new Button
            {
                Content = "📄 Generate Report",
                Margin = new Thickness(5),
                Padding = new Thickness(15, 8, 15, 8),
                Background = System.Windows.Media.Brushes.DodgerBlue,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0)
            };
            generateButton.Click += GenerateReportButton_Click;

            var exportButton = new Button
            {
                Content = "📊 Export CSV",
                Margin = new Thickness(5),
                Padding = new Thickness(15, 8, 15, 8),
                Background = System.Windows.Media.Brushes.DodgerBlue,
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0)
            };
            exportButton.Click += ExportDataButton_Click;

            buttonPanel.Children.Add(refreshButton);
            buttonPanel.Children.Add(generateButton);
            buttonPanel.Children.Add(exportButton);
            Grid.SetRow(buttonPanel, 1);
            mainGrid.Children.Add(buttonPanel);

            // Statistics cards
            var statsGrid = new Grid { Margin = new Thickness(0, 0, 0, 20) };
            statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Total Tours
            var toursPanel = new StackPanel { Margin = new Thickness(10) };
            var toursLabel = new TextBlock
            {
                Text = "🗺️ Total Tours",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.Gray
            };
            _totalToursText = new TextBlock
            {
                Text = "0",
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.DodgerBlue
            };
            toursPanel.Children.Add(toursLabel);
            toursPanel.Children.Add(_totalToursText);
            Grid.SetColumn(toursPanel, 0);
            statsGrid.Children.Add(toursPanel);

            // Total Bookings
            var bookingsPanel = new StackPanel { Margin = new Thickness(10) };
            var bookingsLabel = new TextBlock
            {
                Text = "📋 Total Bookings",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.Gray
            };
            _totalBookingsText = new TextBlock
            {
                Text = "0",
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.Green
            };
            bookingsPanel.Children.Add(bookingsLabel);
            bookingsPanel.Children.Add(_totalBookingsText);
            Grid.SetColumn(bookingsPanel, 1);
            statsGrid.Children.Add(bookingsPanel);

            // Total Revenue
            var revenuePanel = new StackPanel { Margin = new Thickness(10) };
            var revenueLabel = new TextBlock
            {
                Text = "💰 Total Revenue",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.Gray
            };
            _totalRevenueText = new TextBlock
            {
                Text = "$0",
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.Orange
            };
            revenuePanel.Children.Add(revenueLabel);
            revenuePanel.Children.Add(_totalRevenueText);
            Grid.SetColumn(revenuePanel, 2);
            statsGrid.Children.Add(revenuePanel);

            // Active Users
            var usersPanel = new StackPanel { Margin = new Thickness(10) };
            var usersLabel = new TextBlock
            {
                Text = "👥 Active Users",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.Gray
            };
            _activeUsersText = new TextBlock
            {
                Text = "0",
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.Purple
            };
            usersPanel.Children.Add(usersLabel);
            usersPanel.Children.Add(_activeUsersText);
            Grid.SetColumn(usersPanel, 3);
            statsGrid.Children.Add(usersPanel);

            Grid.SetRow(statsGrid, 2);
            mainGrid.Children.Add(statsGrid);

            // Data grids
            var dataGridsGrid = new Grid { Margin = new Thickness(0, 0, 0, 20) };
            dataGridsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            dataGridsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Recent Bookings
            var recentBookingsPanel = new StackPanel { Margin = new Thickness(0, 0, 10, 0) };
            var recentBookingsLabel = new TextBlock
            {
                Text = "📋 Recent Bookings",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            };
            _recentBookingsDataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                MaxHeight = 300
            };
            _recentBookingsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Customer", Width = 120 });
            _recentBookingsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Tour", Width = 150 });
            _recentBookingsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Date", Width = 100 });
            _recentBookingsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Status", Width = 80 });

            recentBookingsPanel.Children.Add(recentBookingsLabel);
            recentBookingsPanel.Children.Add(_recentBookingsDataGrid);
            Grid.SetColumn(recentBookingsPanel, 0);
            dataGridsGrid.Children.Add(recentBookingsPanel);

            // Popular Tours
            var popularToursPanel = new StackPanel { Margin = new Thickness(10, 0, 0, 0) };
            var popularToursLabel = new TextBlock
            {
                Text = "🏆 Popular Tours",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            };
            _popularToursDataGrid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                MaxHeight = 300
            };
            _popularToursDataGrid.Columns.Add(new DataGridTextColumn { Header = "Tour Name", Width = 150 });
            _popularToursDataGrid.Columns.Add(new DataGridTextColumn { Header = "Bookings", Width = 80 });
            _popularToursDataGrid.Columns.Add(new DataGridTextColumn { Header = "Revenue", Width = 100 });
            _popularToursDataGrid.Columns.Add(new DataGridTextColumn { Header = "Rating", Width = 80 });

            popularToursPanel.Children.Add(popularToursLabel);
            popularToursPanel.Children.Add(_popularToursDataGrid);
            Grid.SetColumn(popularToursPanel, 1);
            dataGridsGrid.Children.Add(popularToursPanel);

            Grid.SetRow(dataGridsGrid, 3);
            mainGrid.Children.Add(dataGridsGrid);

            // Info section
            var infoPanel = new StackPanel { Margin = new Thickness(10) };
            var infoLabel = new TextBlock
            {
                Text = "ℹ️ Report Information",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            };
            var infoText1 = new TextBlock
            {
                Text = "• Revenue calculations include only confirmed bookings",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };
            var infoText2 = new TextBlock
            {
                Text = "• Popular tours are ranked by number of confirmed bookings",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };
            var infoText3 = new TextBlock
            {
                Text = "• Reports can be exported as text files or CSV data",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 5)
            };
            var infoText4 = new TextBlock
            {
                Text = "• Data is refreshed in real-time when you click Refresh",
                FontSize = 14,
                Foreground = System.Windows.Media.Brushes.Gray
            };

            infoPanel.Children.Add(infoLabel);
            infoPanel.Children.Add(infoText1);
            infoPanel.Children.Add(infoText2);
            infoPanel.Children.Add(infoText3);
            infoPanel.Children.Add(infoText4);
            Grid.SetRow(infoPanel, 4);
            mainGrid.Children.Add(infoPanel);

            this.Content = mainGrid;
        }

        private void InitializeControls()
        {
            try
            {
                // If controls were not created in CreateBasicLayout, try to find them
                if (_totalToursText == null)
                    _totalToursText = FindName("TotalToursText") as TextBlock;
                if (_totalBookingsText == null)
                    _totalBookingsText = FindName("TotalBookingsText") as TextBlock;
                if (_totalRevenueText == null)
                    _totalRevenueText = FindName("TotalRevenueText") as TextBlock;
                if (_activeUsersText == null)
                    _activeUsersText = FindName("ActiveUsersText") as TextBlock;
                if (_recentBookingsDataGrid == null)
                    _recentBookingsDataGrid = FindName("RecentBookingsDataGrid") as DataGrid;
                if (_popularToursDataGrid == null)
                    _popularToursDataGrid = FindName("PopularToursDataGrid") as DataGrid;

                // Log if any controls are not found
                if (_totalToursText == null || _totalBookingsText == null || _totalRevenueText == null ||
                    _activeUsersText == null || _recentBookingsDataGrid == null || _popularToursDataGrid == null)
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

        private void LoadStatistics()
        {
            try
            {
                // Total Tours
                var totalTours = _context.Tours.Count(t => t.IsActive == true);
                if (_totalToursText != null)
                    _totalToursText.Text = totalTours.ToString();

                // Total Bookings (all statuses)
                var totalBookings = _context.Bookings.Count();
                if (_totalBookingsText != null)
                    _totalBookingsText.Text = totalBookings.ToString();

                // Total Revenue (all time)
                var totalRevenue = _context.Bookings
                    .Where(b => b.Status == "confirmed" || b.Status == "completed")
                    .Sum(b => b.TotalPrice);
                if (_totalRevenueText != null)
                    _totalRevenueText.Text = totalRevenue.ToString("C");

                // Active Users
                var activeUsers = _context.Users.Count(u => u.IsActive == true);
                if (_activeUsersText != null)
                    _activeUsersText.Text = activeUsers.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecentBookings()
        {
            try
            {
                var recentBookings = _context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Schedule)
                    .ThenInclude(s => s.Tour)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(10)
                    .ToList();

                if (_recentBookingsDataGrid != null)
                    _recentBookingsDataGrid.ItemsSource = recentBookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading recent bookings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPopularTours()
        {
            try
            {
                var popularTours = _context.Tours
                    .Where(t => t.IsActive == true)
                    .Select(t => new
                    {
                        TourName = t.TourName,
                        BookingCount = t.TourSchedules
                            .SelectMany(ts => ts.Bookings)
                            .Count(b => b.Status == "confirmed"),
                        TotalRevenue = t.TourSchedules
                            .SelectMany(ts => ts.Bookings)
                            .Where(b => b.Status == "confirmed")
                            .Sum(b => b.TotalPrice),
                        AverageRating = t.TourSchedules
                            .SelectMany(ts => ts.Bookings)
                            .SelectMany(b => b.Reviews)
                            .Any() ? t.TourSchedules
                                .SelectMany(ts => ts.Bookings)
                                .SelectMany(b => b.Reviews)
                                .Average(r => r.Rating) : 0.0
                    })
                    .OrderByDescending(t => t.BookingCount)
                    .Take(10)
                    .ToList();

                if (_popularToursDataGrid != null)
                    _popularToursDataGrid.ItemsSource = popularTours;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading popular tours: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show report type selection dialog
                var reportType = MessageBox.Show(
                    "Select report type:\n\n" +
                    "Yes = Comprehensive Report (Detailed analysis)\n" +
                    "No = Financial Summary (Revenue and booking focus)",
                    "Report Type Selection",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                string report;
                string fileName;

                if (reportType == MessageBoxResult.Yes)
                {
                    report = GenerateComprehensiveReport();
                    fileName = $"Tour_Comprehensive_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                }
                else
                {
                    report = GenerateFinancialSummary();
                    fileName = $"Tour_Financial_Summary_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FileName = fileName
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, report);
                    MessageBox.Show("Report generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    FileName = $"Tour_Data_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var csvData = GenerateCSVData();
                    System.IO.File.WriteAllText(saveFileDialog.FileName, csvData);
                    MessageBox.Show("Data exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadStatistics();
                LoadRecentBookings();
                LoadPopularTours();

                // Log activity
                _activityLogService.LogActivity(1, "REFRESH", "Reports", 0, "Refreshed reports and analytics");

                MessageBox.Show("Data refreshed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateComprehensiveReport()
        {
            var report = new System.Text.StringBuilder();

            report.AppendLine("TOUR MANAGEMENT SYSTEM - COMPREHENSIVE REPORT");
            report.AppendLine("=".PadRight(60, '='));
            report.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();

            // Statistics Summary
            report.AppendLine("STATISTICS SUMMARY");
            report.AppendLine("-".PadRight(40, '-'));
            report.AppendLine($"Total Tours: {_context.Tours.Count(t => t.IsActive == true)}");
            report.AppendLine($"Total Bookings: {_context.Bookings.Count()}");
            report.AppendLine($"Confirmed Bookings: {_context.Bookings.Count(b => b.Status == "confirmed")}");
            report.AppendLine($"Pending Bookings: {_context.Bookings.Count(b => b.Status == "pending")}");
            report.AppendLine($"Total Revenue: {_context.Bookings.Where(b => b.Status == "confirmed" || b.Status == "completed").Sum(b => b.TotalPrice):C}");
            report.AppendLine($"Active Users: {_context.Users.Count(u => u.IsActive == true)}");
            report.AppendLine($"Total Users: {_context.Users.Count()}");
            report.AppendLine();

            // Booking Status Breakdown
            report.AppendLine("BOOKING STATUS BREAKDOWN");
            report.AppendLine("-".PadRight(40, '-'));
            var bookingStatuses = _context.Bookings
                .GroupBy(b => b.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count);

            foreach (var status in bookingStatuses)
            {
                report.AppendLine($"{status.Status}: {status.Count} bookings");
            }
            report.AppendLine();

            // Top Tours
            report.AppendLine("TOP TOURS BY BOOKINGS");
            report.AppendLine("-".PadRight(40, '-'));
            var topTours = _context.Tours
                .Where(t => t.IsActive == true)
                .Select(t => new
                {
                    t.TourName,
                    t.Destination,
                    BookingCount = t.TourSchedules.SelectMany(ts => ts.Bookings).Count(b => b.Status == "confirmed"),
                    Revenue = t.TourSchedules.SelectMany(ts => ts.Bookings).Where(b => b.Status == "confirmed" || b.Status == "completed").Sum(b => b.TotalPrice),
                    AverageRating = t.TourSchedules
                        .SelectMany(ts => ts.Bookings)
                        .SelectMany(b => b.Reviews)
                        .Any() ? t.TourSchedules
                            .SelectMany(ts => ts.Bookings)
                            .SelectMany(b => b.Reviews)
                            .Average(r => r.Rating) : 0.0
                })
                .OrderByDescending(t => t.BookingCount)
                .Take(10);

            foreach (var tour in topTours)
            {
                report.AppendLine($"{tour.TourName} ({tour.Destination}): {tour.BookingCount} bookings, {tour.Revenue:C}, Rating: {tour.AverageRating:F1}/5");
            }
            report.AppendLine();

            // Revenue by Month (Last 6 months)
            report.AppendLine("REVENUE BY MONTH (LAST 6 MONTHS)");
            report.AppendLine("-".PadRight(40, '-'));
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var monthlyRevenue = _context.Bookings
                .Where(b => (b.Status == "confirmed" || b.Status == "completed") && b.BookingDate >= sixMonthsAgo)
                .GroupBy(b => new { b.BookingDate.Value.Year, b.BookingDate.Value.Month })
                .Select(g => new { 
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}", 
                    Revenue = g.Sum(b => b.TotalPrice),
                    Bookings = g.Count()
                })
                .OrderBy(x => x.Month);

            foreach (var month in monthlyRevenue)
            {
                report.AppendLine($"{month.Month}: {month.Revenue:C} ({month.Bookings} bookings)");
            }
            report.AppendLine();

            // User Statistics
            report.AppendLine("USER STATISTICS");
            report.AppendLine("-".PadRight(40, '-'));
            var userStats = _context.Users
                .GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count(), Active = g.Count(u => u.IsActive == true) })
                .OrderByDescending(x => x.Count);

            foreach (var role in userStats)
            {
                report.AppendLine($"{role.Role}: {role.Count} total, {role.Active} active");
            }
            report.AppendLine();

            // Recent Activity
            report.AppendLine("RECENT ACTIVITY (LAST 20 ACTIONS)");
            report.AppendLine("-".PadRight(40, '-'));
            var recentActivity = _context.ActivityLogs
                .Include(al => al.User)
                .OrderByDescending(al => al.CreatedAt)
                .Take(20);

            foreach (var activity in recentActivity)
            {
                var userName = activity.User?.FullName ?? "Unknown User";
                report.AppendLine($"{activity.CreatedAt:yyyy-MM-dd HH:mm}: {userName} - {activity.Action} {activity.EntityType}");
            }

            return report.ToString();
        }

        private string GenerateFinancialSummary()
        {
            var report = new System.Text.StringBuilder();

            report.AppendLine("TOUR MANAGEMENT SYSTEM - FINANCIAL SUMMARY");
            report.AppendLine("=".PadRight(50, '='));
            report.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine();

            // Financial Overview
            report.AppendLine("FINANCIAL OVERVIEW");
            report.AppendLine("-".PadRight(30, '-'));
            var totalRevenue = _context.Bookings.Where(b => b.Status == "confirmed" || b.Status == "completed").Sum(b => b.TotalPrice);
            var pendingRevenue = _context.Bookings.Where(b => b.Status == "pending").Sum(b => b.TotalPrice);
            var totalBookings = _context.Bookings.Count();
            var confirmedBookings = _context.Bookings.Count(b => b.Status == "confirmed" || b.Status == "completed");
            var pendingBookings = _context.Bookings.Count(b => b.Status == "pending");

            report.AppendLine($"Total Revenue (Confirmed): {totalRevenue:C}");
            report.AppendLine($"Pending Revenue: {pendingRevenue:C}");
            report.AppendLine($"Total Bookings: {totalBookings}");
            report.AppendLine($"Confirmed Bookings: {confirmedBookings}");
            report.AppendLine($"Pending Bookings: {pendingBookings}");
            report.AppendLine($"Average Booking Value: {(confirmedBookings > 0 ? totalRevenue / confirmedBookings : 0):C}");
            report.AppendLine();

            // Top Revenue Generating Tours
            report.AppendLine("TOP REVENUE GENERATING TOURS");
            report.AppendLine("-".PadRight(30, '-'));
            var topRevenueTours = _context.Tours
                .Where(t => t.IsActive == true)
                .Select(t => new
                {
                    t.TourName,
                    Revenue = t.TourSchedules.SelectMany(ts => ts.Bookings).Where(b => b.Status == "confirmed" || b.Status == "completed").Sum(b => b.TotalPrice),
                    Bookings = t.TourSchedules.SelectMany(ts => ts.Bookings).Count(b => b.Status == "confirmed" || b.Status == "completed")
                })
                .OrderByDescending(t => t.Revenue)
                .Take(10);

            foreach (var tour in topRevenueTours)
            {
                report.AppendLine($"{tour.TourName}: {tour.Revenue:C} ({tour.Bookings} bookings)");
            }
            report.AppendLine();

            // Monthly Revenue Trend (Last 12 months)
            report.AppendLine("MONTHLY REVENUE TREND (LAST 12 MONTHS)");
            report.AppendLine("-".PadRight(30, '-'));
            var twelveMonthsAgo = DateTime.Now.AddMonths(-12);
            var monthlyRevenue = _context.Bookings
                .Where(b => (b.Status == "confirmed" || b.Status == "completed") && b.BookingDate >= twelveMonthsAgo)
                .GroupBy(b => new { b.BookingDate.Value.Year, b.BookingDate.Value.Month })
                .Select(g => new { 
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}", 
                    Revenue = g.Sum(b => b.TotalPrice),
                    Bookings = g.Count()
                })
                .OrderBy(x => x.Month);

            foreach (var month in monthlyRevenue)
            {
                report.AppendLine($"{month.Month}: {month.Revenue:C} ({month.Bookings} bookings)");
            }

            return report.ToString();
        }

        private string GenerateCSVData()
        {
            var csv = new System.Text.StringBuilder();

            // Tour Performance Data
            csv.AppendLine("=== TOUR PERFORMANCE DATA ===");
            csv.AppendLine("Tour Name,Destination,Duration (Days),Base Price,Total Bookings,Confirmed Bookings,Revenue,Average Rating");

            var tourData = _context.Tours
                .Where(t => t.IsActive == true)
                .Select(t => new
                {
                    TourName = t.TourName,
                    Destination = t.Destination,
                    DurationDays = t.DurationDays,
                    BasePrice = t.BasePrice,
                    TotalBookings = t.TourSchedules.SelectMany(ts => ts.Bookings).Count(),
                    ConfirmedBookings = t.TourSchedules.SelectMany(ts => ts.Bookings).Count(b => b.Status == "confirmed"),
                    Revenue = t.TourSchedules.SelectMany(ts => ts.Bookings).Where(b => b.Status == "confirmed" || b.Status == "completed").Sum(b => b.TotalPrice),
                    AverageRating = t.TourSchedules
                        .SelectMany(ts => ts.Bookings)
                        .SelectMany(b => b.Reviews)
                        .Any() ? t.TourSchedules
                            .SelectMany(ts => ts.Bookings)
                            .SelectMany(b => b.Reviews)
                            .Average(r => r.Rating) : 0.0
                })
                .OrderByDescending(t => t.ConfirmedBookings);

            foreach (var tour in tourData)
            {
                csv.AppendLine($"{tour.TourName},{tour.Destination},{tour.DurationDays},{tour.BasePrice:F2},{tour.TotalBookings},{tour.ConfirmedBookings},{tour.Revenue:F2},{tour.AverageRating:F1}");
            }

            csv.AppendLine();
            csv.AppendLine("=== BOOKING STATUS BREAKDOWN ===");
            csv.AppendLine("Status,Count");

            var bookingStatuses = _context.Bookings
                .GroupBy(b => b.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count);

            foreach (var status in bookingStatuses)
            {
                csv.AppendLine($"{status.Status},{status.Count}");
            }

            csv.AppendLine();
            csv.AppendLine("=== USER STATISTICS ===");
            csv.AppendLine("Role,Total Count,Active Count");

            var userStats = _context.Users
                .GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count(), Active = g.Count(u => u.IsActive == true) })
                .OrderByDescending(x => x.Count);

            foreach (var role in userStats)
            {
                csv.AppendLine($"{role.Role},{role.Count},{role.Active}");
            }

            return csv.ToString();
        }

        // Clean up resources when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}