using System.Configuration;
using System.Data;
using System.Windows;
using TourManagementSystem.Models;
using TourManagementSystem.Scripts;
using TourManagementSystem.Views;

namespace TourManagementSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Initialize database and seed data
                using (var context = new TourManagementContext())
                {
                    context.Database.EnsureCreated();
                    SeedData.Initialize(context);
                }

                // Show LoginWindow as the main window
                var loginWindow = new Views.LoginWindow();
                loginWindow.Show();
                MainWindow = loginWindow;
                
                // Set shutdown mode to close when main window closes
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                
                // Debug: Show message to confirm LoginWindow is being shown
                System.Diagnostics.Debug.WriteLine("LoginWindow should be displayed now");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}\n\nPlease check your database connection and try again.", 
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
