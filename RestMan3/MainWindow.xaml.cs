using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using RestMan3.Views;

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

        // Load Dashboard (Tổng quan) mặc định khi khởi động
        LoadPage(new DashBoardView());
    }

    #region Scroll Events

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

    #endregion

    #region Support Button

    private void SupportButton_Click(object sender, RoutedEventArgs e)
    {
        // Handle support button click
        MessageBox.Show("Tính năng hỗ trợ", "Hỗ trợ", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    #endregion

    #region Navigation

    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        if (button == null) return;

        string content = button.Content.ToString();

        // Đóng tất cả popup trước khi chuyển trang
        CloseAllPopups();

        switch (content)
        {
            case "Tổng quan":
                LoadPage(new DashBoardView());
                break;
            case "Sổ quỹ":
                // LoadPage(new SoQuyView());
                MessageBox.Show("Trang Sổ quỹ đang được phát triển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                break;
            default:
                MessageBox.Show($"Trang {content} đang được phát triển", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                break;
        }
    }

    private void SubMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        if (button == null) return;

        string menuItem = button.Content.ToString();

        // Đóng tất cả popup
        CloseAllPopups();

        // TODO: Load trang tương ứng theo menu item
        MessageBox.Show($"Đang mở trang: {menuItem}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

        // Ví dụ navigation cho từng menu item:
        /*
        switch (menuItem)
        {
            // Menu Hàng hóa
            case "Danh mục":
                LoadPage(new DanhMucView());
                break;
            case "Thiết lập giá":
                LoadPage(new ThietLapGiaView());
                break;
            case "Kiểm kho":
                LoadPage(new KiemKhoView());
                break;
            
            // Menu Phòng/Bàn
            case "Danh sách phòng bàn":
                LoadPage(new PhongBanView());
                break;
            case "Gọi món qua mã QR":
                LoadPage(new GoiMonQRView());
                break;
            
            // Menu Giao dịch
            case "Hóa đơn":
                LoadPage(new HoaDonView());
                break;
            case "Trả hàng":
                LoadPage(new TraHangView());
                break;
            case "Nhập hàng":
                LoadPage(new NhapHangView());
                break;
            case "Trả hàng nhập":
                LoadPage(new TraHangNhapView());
                break;
            case "Xuất hủy":
                LoadPage(new XuatHuyView());
                break;
            
            // Menu Đối tác
            case "Khách Hàng":
                LoadPage(new KhachHangView());
                break;
            case "Tương tác":
                LoadPage(new TuongTacView());
                break;
            case "Nhà cung cấp":
                LoadPage(new NhaCungCapView());
                break;
            case "Đối tác giao hàng":
                LoadPage(new DoiTacGiaoHangView());
                break;
            
            // Menu Nhân viên
            case "Danh sách nhân viên":
                LoadPage(new NhanVienView());
                break;
            case "Lịch làm việc":
                LoadPage(new LichLamViecView());
                break;
            case "Bảng chấm công":
                LoadPage(new ChamCongView());
                break;
            case "Bảng lương":
                LoadPage(new BangLuongView());
                break;
            case "Bảng hoa hồng":
                LoadPage(new HoaHongView());
                break;
            case "Thiết lập nhân viên":
                LoadPage(new ThietLapNhanVienView());
                break;
            
            // Menu Bán Online
            case "Bán hàng Zalo":
                LoadPage(new BanHangZaloView());
                break;
            case "Bán hàng Facebook":
                LoadPage(new BanHangFacebookView());
                break;
            case "Website bán hàng":
                LoadPage(new WebsiteBanHangView());
                break;
            
            // Menu Báo cáo
            case "Cuối Ngày":
                LoadPage(new BaoCaoCuoiNgayView());
                break;
            case "Bán hàng":
                LoadPage(new BaoCaoBanHangView());
                break;
            case "Hàng hóa":
                LoadPage(new BaoCaoHangHoaView());
                break;
            case "Khách hàng":
                LoadPage(new BaoCaoKhachHangView());
                break;
            case "Nhà cung cấp":
                LoadPage(new BaoCaoNhaCungCapView());
                break;
            case "Nhân viên":
                LoadPage(new BaoCaoNhanVienView());
                break;
            case "Kênh bán hàng":
                LoadPage(new BaoCaoKenhBanHangView());
                break;
            case "Tài chính":
                LoadPage(new BaoCaoTaiChinhView());
                break;
            
            // Menu Thuế & Kế toán
            case "Thuế & kế toán":
                LoadPage(new ThueKeToanView());
                break;
            case "Hóa đơn điện tử":
                LoadPage(new HoaDonDienTuView());
                break;
            
            // Menu Cài đặt
            case "Thiết lập cửa hàng":
                LoadPage(new ThietLapCuaHangView());
                break;
            case "Quản lý mẫu in":
                LoadPage(new QuanLyMauInView());
                break;
            case "Quản lý người dùng":
                LoadPage(new QuanLyNguoiDungView());
                break;
            case "Quản lý chi nhánh":
                LoadPage(new QuanLyChiNhanhView());
                break;
            case "Quản lý lý do hủy món":
                LoadPage(new QuanLyLyDoHuyMonView());
                break;
            case "Quản lý ghi chú món":
                LoadPage(new QuanLyGhiChuMonView());
                break;
            case "Lịch sử thao tác":
                LoadPage(new LichSuThaoTacView());
                break;
            case "Xóa dữ liệu dùng thử":
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa tất cả dữ liệu dùng thử?", 
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // Xử lý xóa dữ liệu
                    MessageBox.Show("Đã xóa dữ liệu dùng thử", "Thông báo");
                }
                break;
            
            // Menu Thông tin
            case "Tài khoản":
                LoadPage(new TaiKhoanView());
                break;
            case "Thông tin gian hàng":
                LoadPage(new ThongTinGianHangView());
                break;
            case "Đăng xuất":
                if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", 
                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Xử lý đăng xuất
                    // this.Close();
                    // Mở LoginWindow
                }
                break;
            
            default:
                MessageBox.Show($"Trang {menuItem} đang được phát triển", "Thông báo");
                break;
        }
        */
    }

    /// <summary>
    /// Load trang mới vào MainScrollViewer
    /// </summary>
    private void LoadPage(object page)
    {
        if (page == null) return;

        // Clear nội dung cũ và load trang mới
        MainScrollViewer.Content = page;

        // Scroll về đầu trang
        MainScrollViewer.ScrollToTop();
    }

    #endregion

    #region Popup Management

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
        else if (menuGrid.Name == "ThongTinMenu")
        {
            ThongTinPopup.IsOpen = true;
        }
        else if (menuGrid.Name == "CaiDatMenu")
        {
            CaiDatPopup.IsOpen = true;
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
               IsMouseOverElement(ThongTinMenu, mousePosition) ||
               IsMouseOverElement(CaiDatMenu, mousePosition) ||
               IsMouseOverPopup(HangHoaPopup) ||
               IsMouseOverPopup(PhongBanPopup) ||
               IsMouseOverPopup(GiaoDichPopup) ||
               IsMouseOverPopup(DoiTacPopup) ||
               IsMouseOverPopup(NhanVienPopup) ||
               IsMouseOverPopup(BanOnlinePopup) ||
               IsMouseOverPopup(BaoCaoPopup) ||
               IsMouseOverPopup(ThueKeToanPopup) ||
               IsMouseOverPopup(ThongTinPopup) ||
               IsMouseOverPopup(CaiDatPopup);
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
               ThueKeToanPopup.IsOpen ||
               ThongTinPopup.IsOpen ||
               CaiDatPopup.IsOpen;
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
        ThongTinPopup.IsOpen = false;
        CaiDatPopup.IsOpen = false;
    }

    public CustomPopupPlacement[] PlacePopupRightAligned(Size popupSize, Size targetSize, Point offset)
    {
        // popupSize: Kích thước của menu thả xuống
        // targetSize: Kích thước của cái Button

        // Tính toán để cạnh phải trùng nhau: 
        // Tọa độ X = Chiều rộng Button - Chiều rộng Popup
        double x = targetSize.Width - popupSize.Width;
        double y = targetSize.Height;

        return new[] { new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None) };
    }

    #endregion

    #region Legacy Method (Backward Compatibility)

    /// <summary>
    /// Method để load content (giữ lại cho tương thích với code cũ)
    /// </summary>
    [Obsolete("Sử dụng LoadPage() thay thế")]
    public void LoadContent(UIElement content)
    {
        var grid = MainScrollViewer.Content as Grid;
        if (grid != null)
        {
            grid.Children.Clear();
            grid.Children.Add(content);
        }
    }

    #endregion
}