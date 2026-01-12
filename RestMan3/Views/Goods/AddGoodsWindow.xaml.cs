using System.Windows;
using System.Windows.Input;

namespace RestMan3.Views.Goods
{
    public partial class AddGoodsWindow : Window
    {
        private string _currentTab = "ThongTin";

        public AddGoodsWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LuuButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic lưu dữ liệu
            if (ValidateInput())
            {
                SaveData();
                this.DialogResult = true;
                this.Close();
            }
        }

        private void LuuVaThemMoiButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic lưu và reset form để thêm mới
            if (ValidateInput())
            {
                SaveData();
                ResetForm();
                MessageBox.Show("Đã lưu thành công! Bạn có thể tiếp tục thêm hàng hóa mới.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BoQuaButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn bỏ qua? Dữ liệu chưa lưu sẽ bị mất.",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private bool ValidateInput()
        {
            // Kiểm tra tên hàng
            if (string.IsNullOrWhiteSpace(TenHangTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập tên hàng!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TenHangTextBox.Focus();
                return false;
            }

            // Kiểm tra giá bán
            if (!decimal.TryParse(GiaBanTextBox.Text.Replace(",", ""), out decimal giaBan) || giaBan < 0)
            {
                MessageBox.Show("Giá bán không hợp lệ!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                GiaBanTextBox.Focus();
                return false;
            }

            return true;
        }

        private void SaveData()
        {
            // TODO: Implement save logic
            // Lấy dữ liệu từ các control và lưu vào database

            var maHangHoa = MaHangHoaTextBox.Text;
            var tenHang = TenHangTextBox.Text;
            var loaiThucDon = (LoaiThucDonComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            var viTri = ViTriTextBox.Text;
            var trongLuong = TrongLuongTextBox.Text;
            var donViTinh = DonViTinhTextBox.Text;
            var moTa = MoTaTextBox.Text;

            decimal.TryParse(GiaVonTextBox.Text.Replace(",", ""), out decimal giaVon);
            decimal.TryParse(GiaBanTextBox.Text.Replace(",", ""), out decimal giaBan);

            int.TryParse(TonKhoTextBox.Text.Replace(",", ""), out int tonKho);
            int.TryParse(ItNhatTextBox.Text.Replace(",", ""), out int itNhat);
            int.TryParse(NhieuNhatTextBox.Text.Replace(",", ""), out int nhieuNhat);

            var banTrucTiep = BanTrucTiepCheckBox.IsChecked ?? false;
            var laMonThem = LaMonThemCheckBox.IsChecked ?? false;
            var quanLyTonKho = QuanLyTonKhoToggle.IsChecked ?? false;

            // Gọi service để lưu dữ liệu vào database
            // await _menuItemService.CreateAsync(newItem);
        }

        private void ResetForm()
        {
            // Reset tất cả các field về giá trị mặc định
            MaHangHoaTextBox.Text = "Mã hàng tự động";
            TenHangTextBox.Clear();
            LoaiThucDonComboBox.SelectedIndex = 0;
            ViTriTextBox.Clear();
            TrongLuongTextBox.Clear();
            NhomHangComboBox.SelectedIndex = 0;
            DonViTinhTextBox.Clear();
            MoTaTextBox.Clear();

            GiaVonTextBox.Text = "0";
            GiaBanTextBox.Text = "0";

            TonKhoTextBox.Text = "0";
            ItNhatTextBox.Text = "0";
            NhieuNhatTextBox.Text = "999,999,999";

            BanTrucTiepCheckBox.IsChecked = true;
            LaMonThemCheckBox.IsChecked = false;
            QuanLyTonKhoToggle.IsChecked = true;

            // Reset về tab Thông tin
            SelectTab("ThongTin");

            // Focus vào tên hàng
            TenHangTextBox.Focus();
        }

        private void Tab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = e.OriginalSource as System.Windows.Controls.TextBlock;
            if (textBlock?.Tag is string tabName)
            {
                SelectTab(tabName);
            }
        }

        private void SelectTab(string tabName)
        {
            if (_currentTab == tabName)
                return;

            // 1. Ẩn tất cả nội dung (Giữ nguyên code cũ của bạn)
            ThongTinContent.Visibility = Visibility.Collapsed;
            MoTaContent.Visibility = Visibility.Collapsed;
            ThanhPhanContent.Visibility = Visibility.Collapsed;
            MonThemContent.Visibility = Visibility.Collapsed;
            ChiNhanhContent.Visibility = Visibility.Collapsed;

            // 2. RESET TRẠNG THÁI TẤT CẢ CÁC TAB (Xóa sạch hiệu ứng nổi)
            var tabs = new[] { ThongTinTab, MoTaTab, ThanhPhanTab, MonThemTab, ChiNhanhTab };
            foreach (var tab in tabs)
            {
                tab.Style = null; // Xóa Style gây ra đổ bóng
                tab.Background = System.Windows.Media.Brushes.Transparent; // Đặt nền trong suốt
                tab.BorderThickness = new Thickness(0); // Xóa viền

                // Reset chữ về màu xám
                if (tab.Child is System.Windows.Controls.TextBlock tb)
                {
                    tb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x66, 0x66, 0x66));
                }
            }

            // 3. KÍCH HOẠT TAB ĐƯỢC CHỌN
            switch (tabName)
            {
                case "ThongTin":
                    ThongTinContent.Visibility = Visibility.Visible;
                    ActivateTab(ThongTinTab);
                    break;
                case "MoTa":
                    MoTaContent.Visibility = Visibility.Visible;
                    ActivateTab(MoTaTab);
                    break;
                case "ThanhPhan":
                    ThanhPhanContent.Visibility = Visibility.Visible;
                    ActivateTab(ThanhPhanTab);
                    break;
                case "MonThem":
                    MonThemContent.Visibility = Visibility.Visible;
                    ActivateTab(MonThemTab);
                    break;
                case "ChiNhanh":
                    ChiNhanhContent.Visibility = Visibility.Visible;
                    ActivateTab(ChiNhanhTab);
                    break;
            }

            _currentTab = tabName;
        }

        // Hàm ActivateTab tối giản (chỉ đổi màu chữ, không làm nổi khối)
        private void ActivateTab(System.Windows.Controls.Border tabBorder)
        {
            if (tabBorder.Child is System.Windows.Controls.TextBlock textBlock)
            {
                textBlock.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x00, 0xB1, 0x4F));
                //Nếu bạn muốn có gạch chân xanh thay vì nổi khối trắng:
                tabBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x00, 0xB1, 0x4F));
                tabBorder.BorderThickness = new Thickness(0, 0, 0, 2);
            }
        }
    }
}