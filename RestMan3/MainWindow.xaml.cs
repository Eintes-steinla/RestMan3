using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using RestMan3.Views;
using RestMan3.Views.Goods;
using RestMan3.Helpers;

namespace RestMan3;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Dictionary<string, object> _pageCache = new Dictionary<string, object>();

    public MainWindow()
    {
        InitializeComponent();

        // 1. Quản lý tập trung các Popup để tránh lặp code
        PopupManager.Instance.RegisterPopups(
                HangHoaPopup, PhongBanPopup, GiaoDichPopup, DoiTacPopup,
                NhanVienPopup, BanOnlinePopup, BaoCaoPopup, ThueKeToanPopup,
                ThongTinPopup, CaiDatPopup
        );

        // 2. Đăng ký Logic chuyển trang cho Manager
        // Khi Manager gọi OnNavigate, hàm này sẽ chạy
        PopupManager.Instance.OnNavigate = (menuItem) =>
        {
            NavigateByMenuName(menuItem);
        };

        // 3. Đăng ký các sự kiện hệ thống
        MainScrollViewer.ScrollChanged += MainScrollViewer_ScrollChanged;
        this.PreviewMouseDown += Window_PreviewMouseDown;
        MainScrollViewer.PreviewMouseMove += MainArea_PreviewMouseMove;

        // 4. Load trang mặc định
        LoadPage(new DashBoardView());
    }

    #region Navigation & Page Management

    /// <summary>
    /// Load trang vào vùng nội dung chính, sử dụng Cache để tiết kiệm tài nguyên
    /// </summary>
    private void LoadPage(object page)
    {
        if (page == null) return;

        string pageKey = page.GetType().FullName ?? page.GetType().Name;

        if (!_pageCache.ContainsKey(pageKey))
        {
            _pageCache[pageKey] = page;
        }

        MainScrollViewer.Content = _pageCache[pageKey];
        MainScrollViewer.ScrollToTop();
    }

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Content != null)
        {
            string content = button.Content.ToString()!;
            PopupManager.Instance.CloseAll();

            switch (content)
            {
                case "Tổng quan":
                    LoadPage(new DashBoardView());
                    break;
                case "Sổ quỹ":
                    // LoadPage(new SoQuyView());
                    MessageBox.Show("Trang Sổ quỹ đang được phát triển", "Thông báo");
                    break;
                default:
                    MessageBox.Show($"Trang {content} đang được phát triển", "Thông báo");
                    break;
            }
        }
    }

    // Gom logic switch-case vào một hàm riêng cho sạch
    private void NavigateByMenuName(string menuItem)
    {
        switch (menuItem)
        {
            case "Danh mục": LoadPage(new CategoryView()); break;
            case "Tổng quan": LoadPage(new DashBoardView()); break;
            case "Đăng xuất":
                if (MessageBox.Show("Đăng xuất?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    Application.Current.Shutdown();
                break;
            default:
                MessageBox.Show($"Trang {menuItem} đang phát triển");
                break;
        }
    }

    // Hàm Click bây giờ cực kỳ ngắn gọn
    private void SubMenuItem_Click(object sender, RoutedEventArgs e)
    {
        PopupManager.Instance.HandleSubMenuItemClick(sender);
    }

    #endregion

    #region Popup Management

    private void MenuContainer_MouseEnter(object sender, MouseEventArgs e)
    {
        PopupManager.Instance.StopCloseTimer();

        if (sender is Grid menuGrid)
        {
            string targetName = menuGrid.Name.Replace("Menu", "Popup");
            var targetPopup = FindName(targetName) as Popup;
            PopupManager.Instance.OpenPopup(targetPopup);
        }
    }

    private void MenuContainer_MouseLeave(object sender, MouseEventArgs e)
    {
        if (PopupManager.Instance.IsAnyOpen())
        {
            PopupManager.Instance.StartCloseTimer();
        }
    }

    private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (PopupManager.Instance.IsAnyOpen() &&
            !PopupManager.Instance.IsMouseOverAnyPopup())
        {
            PopupManager.Instance.CloseAll();
        }
    }

    #endregion

    #region Utility Events

    private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        BackToTopButton.Visibility = (e.VerticalOffset > 100) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Kiểm tra nếu người dùng đang lăn chuột
        if (sender is ScrollViewer scv)
        {
            // Tính toán vị trí cuộn mới dựa trên độ mạnh nhẹ của cú lăn chuột (e.Delta)
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);

            // Đánh dấu sự kiện đã được xử lý để tránh các Control con tranh chấp
            e.Handled = true;
        }
    }

    private void BackToTopButton_Click(object sender, RoutedEventArgs e)
    {
        MainScrollViewer.ScrollToTop();
    }

    private void MainArea_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (PopupManager.Instance.IsAnyOpen()) PopupManager.Instance.StartCloseTimer();
        //if (PopupManager.Instance.IsAnyOpen()) closeTimer.Start();
    }

    private void SupportButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Tính năng hỗ trợ", "Hỗ trợ", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    #endregion

}