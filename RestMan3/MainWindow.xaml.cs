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
    private DispatcherTimer closeTimer;
    private List<Popup> _allPopups;
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

        // 2. Khởi tạo Timer để xử lý đóng Popup mượt mà
        //closeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        //closeTimer.Tick += CloseTimer_Tick;

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

    private void SubMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Content != null)
        {
            string menuItem = button.Content.ToString()!;
            PopupManager.Instance.CloseAll();

            // Xử lý chuyển trang dựa vào tên menu
            switch (menuItem)
            {
                // Menu Hàng hóa
                case "Danh mục":
                    LoadPage(new CategoryView());
                    break;
                case "Thiết lập giá":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Kiểm kho":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;

                // Menu Phòng/Bàn
                case "Danh sách phòng bàn":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Gọi món qua mã QR":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;

                // Menu Giao dịch
                case "Hóa đơn":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Trả hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Nhập hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Trả hàng nhập":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Xuất hủy":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;

                // Menu Đối tác
                case "Khách Hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Tương tác":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Nhà cung cấp":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Đối tác giao hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;

                // Menu Nhân viên
                case "Danh sách nhân viên":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Lịch làm việc":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Bảng chấm công":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Bảng lương":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Bảng hoa hồng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Thiết lập nhân viên":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;

                // Menu Bán Online
                case "Bán hàng Zalo":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Bán hàng Facebook":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Website bán hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;

                // Menu Báo cáo
                case "Cuối Ngày":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Bán hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Hàng hóa":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Khách hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Nhân viên":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Kênh bán hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Tài chính":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;

                // Menu Thuế & Kế toán
                case "Thuế & kế toán":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Hóa đơn điện tử":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;

                // Menu Cài đặt
                case "Thiết lập cửa hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Quản lý mẫu in":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Quản lý người dùng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Quản lý chi nhánh":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Quản lý lý do hủy món":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Quản lý ghi chú món":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Lịch sử thao tác":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Xóa dữ liệu dùng thử":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;

                // Menu Thông tin
                case "Tài khoản":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Thông tin gian hàng":
                    MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                    break;
                case "Đăng xuất":
                    var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Xử lý đăng xuất
                        Application.Current.Shutdown();
                    }
                    break;

                default:
                    MessageBox.Show($"Đang mở trang: {menuItem}", "Thông báo");
                    break;
            }
        }
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

    #region Popup Custom Placement

    public CustomPopupPlacement[] PlacePopupRightAligned(Size popupSize, Size targetSize, Point offset)
    {
        // Tính toán để cạnh phải của Popup khớp với cạnh phải của Button
        double x = targetSize.Width - popupSize.Width;
        double y = targetSize.Height;

        return new[] { new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None) };
    }

    #endregion
}