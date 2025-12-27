using System.Windows;
using RestMan3.WPF.Common;
using RestMan3.WPF.Views.Dashboard;

namespace RestMan3.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NavigationHelper.MainContent = MainContent;

            ApplyAuthorization();

            NavigationHelper.Navigate(new DashboardView());
        }

        private void ApplyAuthorization()
        {
            var user = SessionContext.CurrentUser;

            if (user.RoleName != "Admin")
            {
                btnUser.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationHelper.Navigate(new DashboardView());
        }

        private void BtnCategory_Click(object sender, RoutedEventArgs e)
        {
            // sẽ làm ở phase 3
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Views.Auth.LoginView();
            login.Show();
            this.Close();
        }
    }
}
