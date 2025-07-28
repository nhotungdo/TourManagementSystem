using System.Windows;
using System.Windows.Controls;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class CustomerMainWindow : Window
    {
        private User _currentUser;

        public CustomerMainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            UserNameText.Text = $"Welcome, {user.FullName}";

            // Load dashboard by default
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            var dashboardPage = new CustomerDashboardPage(_currentUser);
            MainFrame.Navigate(dashboardPage);
            SetActiveButton(DashboardButton);
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            var dashboardPage = new CustomerDashboardPage(_currentUser);
            MainFrame.Navigate(dashboardPage);
            SetActiveButton(DashboardButton);
        }

        private void AccountManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var accountManagementPage = new AccountManagementPage(_currentUser);
            MainFrame.Navigate(accountManagementPage);
            SetActiveButton(AccountManagementButton);
        }

        private void BrowseToursButton_Click(object sender, RoutedEventArgs e)
        {
            var browseToursPage = new BrowseToursPage(_currentUser);
            MainFrame.Navigate(browseToursPage);
            SetActiveButton(BrowseToursButton);
        }

        private void BookTourButton_Click(object sender, RoutedEventArgs e)
        {
            var bookTourPage = new BookTourPage(_currentUser);
            MainFrame.Navigate(bookTourPage);
            SetActiveButton(BookTourButton);
        }

        private void MyBookingsButton_Click(object sender, RoutedEventArgs e)
        {
            var myBookingsPage = new MyBookingsPage(_currentUser);
            MainFrame.Navigate(myBookingsPage);
            SetActiveButton(MyBookingsButton);
        }

        private void PaymentManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var paymentManagementPage = new PaymentManagementPage(_currentUser);
            MainFrame.Navigate(paymentManagementPage);
            SetActiveButton(PaymentManagementButton);
        }

        private void BookingHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var bookingHistoryPage = new BookingHistoryPage(_currentUser);
            MainFrame.Navigate(bookingHistoryPage);
            SetActiveButton(BookingHistoryButton);
        }

        private void MyReviewsButton_Click(object sender, RoutedEventArgs e)
        {
            var myReviewsPage = new MyReviewsPage(_currentUser);
            MainFrame.Navigate(myReviewsPage);
            SetActiveButton(MyReviewsButton);
        }

        private void PromotionsButton_Click(object sender, RoutedEventArgs e)
        {
            var promotionsPage = new PromotionsPage(_currentUser);
            MainFrame.Navigate(promotionsPage);
            SetActiveButton(PromotionsButton);
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
            AccountManagementButton.Style = FindResource("MenuButton") as Style;
            BrowseToursButton.Style = FindResource("MenuButton") as Style;
            BookTourButton.Style = FindResource("MenuButton") as Style;
            MyBookingsButton.Style = FindResource("MenuButton") as Style;
            PaymentManagementButton.Style = FindResource("MenuButton") as Style;
            BookingHistoryButton.Style = FindResource("MenuButton") as Style;
            MyReviewsButton.Style = FindResource("MenuButton") as Style;
            PromotionsButton.Style = FindResource("MenuButton") as Style;

            // Set active button style
            activeButton.Style = FindResource("ActiveMenuButton") as Style;
        }
    }
}