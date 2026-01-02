using System.Windows;
using System.Windows.Controls;

namespace RestMan3.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox? pb = sender as PasswordBox;
            if (pb != null)
            {
                // Nếu độ dài mật khẩu > 0 thì ẩn Hint, ngược lại thì hiện
                if (pb.Password.Length > 0)
                    HintPassword.Visibility = Visibility.Collapsed;
                else
                    HintPassword.Visibility = Visibility.Visible;
            }
        }
    }
}
