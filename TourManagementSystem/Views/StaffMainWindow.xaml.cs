using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class StaffMainWindow : Window
    {
        private User _currentUser;

        public StaffMainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            UserNameText.Text = $"Welcome, {user.FullName}";

            // Load dashboard by default
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            var dashboardPage = new StaffDashboardPage();
            MainFrame.Navigate(dashboardPage);
            SetActiveButton(DashboardButton);
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            var dashboardPage = new StaffDashboardPage();
            MainFrame.Navigate(dashboardPage);
            SetActiveButton(DashboardButton);
        }

        private void TourManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var tourManagementPage = new TourManagementPage();
            MainFrame.Navigate(tourManagementPage);
            SetActiveButton(TourManagementButton);
        }

        private void ItineraryManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var itineraryManagementPage = new ItineraryManagementPage();
            MainFrame.Navigate(itineraryManagementPage);
            SetActiveButton(ItineraryManagementButton);
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
            TourManagementButton.Style = FindResource("MenuButton") as Style;
            ItineraryManagementButton.Style = FindResource("MenuButton") as Style;
            ScheduleManagementButton.Style = FindResource("MenuButton") as Style;
            BookingManagementButton.Style = FindResource("MenuButton") as Style;
            PaymentManagementButton.Style = FindResource("MenuButton") as Style;
            InvoiceManagementButton.Style = FindResource("MenuButton") as Style;
            ReviewManagementButton.Style = FindResource("MenuButton") as Style;
            AttractionManagementButton.Style = FindResource("MenuButton") as Style;

            // Set active button style
            activeButton.Style = FindResource("ActiveMenuButton") as Style;
        }
    }
}