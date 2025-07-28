using System.Windows.Controls;

namespace TourManagementSystem.Views
{
    public partial class PlaceholderPage : Page
    {
        public PlaceholderPage(string title, string description)
        {
            InitializeComponent();
            TitleText.Text = title;
            DescriptionText.Text = description;
        }
    }
}