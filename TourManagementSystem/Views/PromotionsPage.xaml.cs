using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Models;

namespace TourManagementSystem.Views
{
    public partial class PromotionsPage : Page
    {
        private readonly TourManagementContext _context;
        private readonly User _currentUser;

        public PromotionsPage(User user)
        {
            InitializeComponent();
            _context = new TourManagementContext();
            _currentUser = user;

            LoadPromotions();
        }

        private void LoadPromotions()
        {
            try
            {
                var promotions = _context.Promotions
                    .Where(p => p.IsActive == true && p.EndDate > DateTime.Today)
                    .OrderBy(p => p.EndDate)
                    .ToList();

                DisplayPromotions(promotions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading promotions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayPromotions(System.Collections.Generic.List<Promotion> promotions)
        {
            try
            {
                PromotionsStackPanel.Children.Clear();

                if (!promotions.Any())
                {
                    var noPromotionsText = new TextBlock
                    {
                        Text = "No active promotions available.",
                        FontSize = 16,
                        Foreground = System.Windows.Media.Brushes.Gray,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 50, 0, 0)
                    };
                    PromotionsStackPanel.Children.Add(noPromotionsText);
                    return;
                }

                foreach (var promotion in promotions)
                {
                    var promotionCard = CreatePromotionCard(promotion);
                    PromotionsStackPanel.Children.Add(promotionCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying promotions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreatePromotionCard(Promotion promotion)
        {
            var card = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var stackPanel = new StackPanel();

            var title = new TextBlock
            {
                Text = promotion.PromotionName,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var code = new TextBlock
            {
                Text = $"Code: {promotion.PromotionCode}",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.Blue,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var discount = new TextBlock
            {
                Text = $"Discount: {promotion.DiscountPercentage}%",
                FontSize = 16,
                Foreground = System.Windows.Media.Brushes.Green,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var description = new TextBlock
            {
                Text = promotion.Description,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var expiry = new TextBlock
            {
                Text = $"Expires: {promotion.EndDate:dd/MM/yyyy}",
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Gray
            };

            stackPanel.Children.Add(title);
            stackPanel.Children.Add(code);
            stackPanel.Children.Add(discount);
            stackPanel.Children.Add(description);
            stackPanel.Children.Add(expiry);

            card.Child = stackPanel;
            return card;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}