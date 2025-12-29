using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace RestMan3;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DispatcherTimer closeTimer;

    public MainWindow()
    {
        InitializeComponent();

        // Subscribe to ScrollViewer scroll event
        MainScrollViewer.ScrollChanged += MainScrollViewer_ScrollChanged;

        // Initialize close timer for popups
        closeTimer = new DispatcherTimer();
        closeTimer.Interval = TimeSpan.FromMilliseconds(100);
        closeTimer.Tick += CloseTimer_Tick;

        // Add event handlers for closing popups when clicking outside
        this.PreviewMouseDown += Window_PreviewMouseDown;
        MainScrollViewer.PreviewMouseMove += MainArea_PreviewMouseMove;
    }

    private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        // Show/hide back to top button based on scroll position
        if (e.VerticalOffset > 300)
        {
            BackToTopButton.Visibility = Visibility.Visible;
        }
        else
        {
            BackToTopButton.Visibility = Visibility.Collapsed;
        }
    }

    private void BackToTopButton_Click(object sender, RoutedEventArgs e)
    {
        // Scroll to top smoothly
        MainScrollViewer.ScrollToTop();
    }

    private void SupportButton_Click(object sender, RoutedEventArgs e)
    {
        // Handle support button click
        MessageBox.Show("Tính năng hỗ trợ", "Hỗ trợ", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // Close popups when clicking anywhere
    private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (IsAnyPopupOpen())
        {
            if (!IsMouseOverAnyMenu(e.GetPosition(this)))
            {
                CloseAllPopups();
            }
        }
    }

    // Close popups when mouse moves to main content area
    private void MainArea_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (IsAnyPopupOpen())
        {
            closeTimer.Start();
        }
    }

    // Menu container mouse handlers
    private void MenuContainer_MouseEnter(object sender, MouseEventArgs e)
    {
        closeTimer.Stop();

        Grid menuGrid = sender as Grid;
        if (menuGrid == null) return;

        // Close all other popups first
        CloseAllPopups();

        // Open the corresponding popup
        if (menuGrid.Name == "HangHoaMenu")
        {
            HangHoaPopup.IsOpen = true;
        }
        else if (menuGrid.Name == "PhongBanMenu")
        {
            PhongBanPopup.IsOpen = true;
        }
        else if (menuGrid.Name == "GiaoDichMenu")
        {
            GiaoDichPopup.IsOpen = true;
        }
        else if (menuGrid.Name == "DoiTacMenu")
        {
            DoiTacPopup.IsOpen = true;
        }
        else if (menuGrid.Name == "NhanVienMenu")
        {
            NhanVienPopup.IsOpen = true;
        }
        else if (menuGrid.Name == "BanOnlineMenu")
        {
            BanOnlinePopup.IsOpen = true;
        }
        else if (menuGrid.Name == "BaoCaoMenu")
        {
            BaoCaoPopup.IsOpen = true;
        }
        else if (menuGrid.Name == "ThueKeToanMenu")
        {
            ThueKeToanPopup.IsOpen = true;
        }
    }

    private void MenuContainer_MouseLeave(object sender, MouseEventArgs e)
    {
        // Start timer to close popup after a delay
        if (IsAnyPopupOpen())
        {
            closeTimer.Start();
        }
    }

    private bool IsMouseOverPopup(Popup popup)
    {
        if (popup == null || !popup.IsOpen || popup.Child == null)
            return false;

        try
        {
            Point mousePos = Mouse.GetPosition(popup.Child);
            Border popupBorder = popup.Child as Border;

            if (popupBorder != null)
            {
                return mousePos.X >= 0 && mousePos.X <= popupBorder.ActualWidth &&
                       mousePos.Y >= -10 && mousePos.Y <= popupBorder.ActualHeight;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private bool IsMouseOverAnyMenu(Point mousePosition)
    {
        // Check if mouse is over any menu container or popup
        return IsMouseOverElement(HangHoaMenu, mousePosition) ||
               IsMouseOverElement(PhongBanMenu, mousePosition) ||
               IsMouseOverElement(GiaoDichMenu, mousePosition) ||
               IsMouseOverElement(DoiTacMenu, mousePosition) ||
               IsMouseOverElement(NhanVienMenu, mousePosition) ||
               IsMouseOverElement(BanOnlineMenu, mousePosition) ||
               IsMouseOverElement(BaoCaoMenu, mousePosition) ||
               IsMouseOverElement(ThueKeToanMenu, mousePosition) ||
               IsMouseOverPopup(HangHoaPopup) ||
               IsMouseOverPopup(PhongBanPopup) ||
               IsMouseOverPopup(GiaoDichPopup) ||
               IsMouseOverPopup(DoiTacPopup) ||
               IsMouseOverPopup(NhanVienPopup) ||
               IsMouseOverPopup(BanOnlinePopup) ||
               IsMouseOverPopup(BaoCaoPopup) ||
               IsMouseOverPopup(ThueKeToanPopup);
    }

    private bool IsMouseOverElement(UIElement element, Point mousePosition)
    {
        if (element == null) return false;

        try
        {
            Point pos = element.PointFromScreen(PointToScreen(mousePosition));
            return pos.X >= 0 && pos.X <= ((FrameworkElement)element).ActualWidth &&
                   pos.Y >= 0 && pos.Y <= ((FrameworkElement)element).ActualHeight;
        }
        catch
        {
            return false;
        }
    }

    private bool IsAnyPopupOpen()
    {
        return HangHoaPopup.IsOpen || 
               PhongBanPopup.IsOpen || 
               GiaoDichPopup.IsOpen || 
               DoiTacPopup.IsOpen || 
               NhanVienPopup.IsOpen || 
               BanOnlinePopup.IsOpen || 
               BaoCaoPopup.IsOpen || 
               ThueKeToanPopup.IsOpen;
    }

    private void CloseTimer_Tick(object sender, EventArgs e)
    {
        // Check if mouse is still over any menu area
        Point mousePos = Mouse.GetPosition(this);

        if (!IsMouseOverAnyMenu(mousePos))
        {
            CloseAllPopups();
        }

        closeTimer.Stop();
    }

    private void CloseAllPopups()
    {
        HangHoaPopup.IsOpen = false;
        PhongBanPopup.IsOpen = false;
        GiaoDichPopup.IsOpen = false;
        DoiTacPopup.IsOpen = false;
        NhanVienPopup.IsOpen = false;
        BanOnlinePopup.IsOpen = false;
        BaoCaoPopup.IsOpen = false;
        ThueKeToanPopup.IsOpen = false;
    }

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        if (button != null)
        {
            MessageBox.Show($"Clicked: {button.Content}", "Navigation", MessageBoxButton.OK);
        }
    }

    private void SubMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        if (button != null)
        {
            CloseAllPopups();
            MessageBox.Show($"Clicked submenu: {button.Content}", "Submenu", MessageBoxButton.OK);
        }
    }

    // Method to load content dynamically into the main area
    public void LoadContent(UIElement content)
    {
        var grid = MainScrollViewer.Content as Grid;
        if (grid != null)
        {
            grid.Children.Clear();
            grid.Children.Add(content);
        }
    }
}