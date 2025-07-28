using System;
using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;
using TourManagementSystem.Views;

namespace TourManagementSystem.Views
{
    public partial class AdminMainWindow : Window
    {
        private User _currentUser;

        public AdminMainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            UserNameText.Text = $"Welcome, {user.FullName}";
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            var dashboardPage = new AdminDashboardPage(_currentUser);
            MainFrame.Navigate(dashboardPage);
            SetActiveButton(DashboardButton);
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            var dashboardPage = new AdminDashboardPage(_currentUser);
            MainFrame.Navigate(dashboardPage);
            SetActiveButton(DashboardButton);
        }

        private void UserManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var userManagementPage = new UserManagementPage();
            MainFrame.Navigate(userManagementPage);
            SetActiveButton(UserManagementButton);
        }

        private void TourManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var tourManagementPage = new TourManagementPage();
            MainFrame.Navigate(tourManagementPage);
            SetActiveButton(TourManagementButton);
        }

        private void ScheduleManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var scheduleManagementPage = new ScheduleManagementPage();
            MainFrame.Navigate(scheduleManagementPage);
            SetActiveButton(ScheduleManagementButton);
        }

        private void BookingManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var bookingManagementPage = new BookingManagementPage();
            MainFrame.Navigate(bookingManagementPage);
            SetActiveButton(BookingManagementButton);
        }

        private void PaymentManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var paymentManagementPage = new PaymentManagementPage();
            MainFrame.Navigate(paymentManagementPage);
            SetActiveButton(PaymentManagementButton);
        }

        private void InvoiceManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var invoiceManagementPage = new InvoiceManagementPage();
            MainFrame.Navigate(invoiceManagementPage);
            SetActiveButton(InvoiceManagementButton);
        }

        private void PromotionManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var promotionManagementPage = new PromotionManagementPage();
            MainFrame.Navigate(promotionManagementPage);
            SetActiveButton(PromotionManagementButton);
        }

        private void ReviewManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var reviewManagementPage = new ReviewManagementPage();
            MainFrame.Navigate(reviewManagementPage);
            SetActiveButton(ReviewManagementButton);
        }

        private void AttractionManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var attractionManagementPage = new AttractionManagementPage();
            MainFrame.Navigate(attractionManagementPage);
            SetActiveButton(AttractionManagementButton);
        }

        private void ActivityLogsButton_Click(object sender, RoutedEventArgs e)
        {
            var activityLogsPage = new ActivityLogPage();
            MainFrame.Navigate(activityLogsPage);
            SetActiveButton(ActivityLogsButton);
        }

        private void SystemConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var systemConfigPage = new SystemConfigPage();
            MainFrame.Navigate(systemConfigPage);
            SetActiveButton(SystemConfigButton);
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            var reportsPage = new ReportsPage();
            MainFrame.Navigate(reportsPage);
            SetActiveButton(ReportsButton);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Close();
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            // Reset all buttons to default style
            DashboardButton.Style = FindResource("MenuButton") as Style;
            UserManagementButton.Style = FindResource("MenuButton") as Style;
            TourManagementButton.Style = FindResource("MenuButton") as Style;
            ScheduleManagementButton.Style = FindResource("MenuButton") as Style;
            BookingManagementButton.Style = FindResource("MenuButton") as Style;
            PaymentManagementButton.Style = FindResource("MenuButton") as Style;
            InvoiceManagementButton.Style = FindResource("MenuButton") as Style;
            PromotionManagementButton.Style = FindResource("MenuButton") as Style;
            ReviewManagementButton.Style = FindResource("MenuButton") as Style;
            AttractionManagementButton.Style = FindResource("MenuButton") as Style;
            ActivityLogsButton.Style = FindResource("MenuButton") as Style;
            SystemConfigButton.Style = FindResource("MenuButton") as Style;
            ReportsButton.Style = FindResource("MenuButton") as Style;

            // Set active button style
            activeButton.Style = FindResource("ActiveMenuButton") as Style;
        }
    }
}