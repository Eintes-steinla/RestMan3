using RestMan3.Helpers;
using RestMan3.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace RestMan3.Views.Goods
{
    public partial class CategoryView : UserControl
    {
        private CategoryViewModel _viewModel;

        public CategoryView()
        {
            InitializeComponent();
            _viewModel = new CategoryViewModel();
            this.DataContext = _viewModel;

            // Bind RecordsPerPageComboBox
            RecordsPerPageComboBox.SelectionChanged += RecordsPerPageComboBox_SelectionChanged;

            // 1. Quản lý tập trung các Popup để tránh lặp code
            PopupManager.Instance.RegisterPopups(
                    ThemMoiPopup
            );
            PopupManager.Instance.RegisterMenuGrids(ThemMoiMenu);

            this.PreviewMouseDown += Window_PreviewMouseDown;
        }

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

        // Hàm này sẽ nhận sự kiện từ các nút bấm trong Popup
        private void SubMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string command = btn.Content.ToString();
                switch (command)
                {
                    case "Thêm hàng hóa":
                        // Mở cửa sổ/box Thêm hàng hóa tại đây
                        MoBoxThemHangHoa();
                        break;
                }
                PopupManager.Instance.CloseAll(); // Đóng popup sau khi chọn
            }
        }

        private void MoBoxThemHangHoa()
        {
            // Tạo instance của cửa sổ nhập liệu
            var addWindow = new AddGoodsWindow();

            // Hiển thị dạng Dialog (người dùng phải xong việc mới quay lại được màn hình chính)
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                // Refresh lại danh sách hàng hóa sau khi thêm thành công
                _viewModel.LoadData();
                MessageBox.Show("Đã thêm hàng hóa thành công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Hoặc nếu muốn mở form dạng non-modal (không chặn window chính):
        //private void MoBoxThemHangHoaNonModal()
        //{
        //    var addWindow = new AddGoodsWindow();
        //    addWindow.Owner = Window.GetWindow(this); // Set owner để window con luôn ở trên window cha
        //    addWindow.Show();
        //}

        #endregion

        private void RecordsPerPageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && RecordsPerPageComboBox.SelectedItem is ComboBoxItem item)
            {
                _viewModel.PageSize = int.Parse(item.Content.ToString());
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //if (sender is TextBox textBox)
            //{
            //    if (textBox.Text == "Theo mã, tên hàng" || textBox.Text == "Tìm kiếm nhóm hàng")
            //    {
            //        textBox.Text = "";
            //        textBox.Foreground = System.Windows.Media.Brushes.Black;
            //    }
            //}
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (sender is TextBox textBox)
            //{
            //    if (string.IsNullOrWhiteSpace(textBox.Text))
            //    {
            //        if (textBox.Name == "SearchTextBox")
            //        {
            //            textBox.Text = "Theo mã, tên hàng";
            //        }
            //        else if (textBox.Name == "SearchCategoryTextBox")
            //        {
            //            textBox.Text = "Tìm kiếm nhóm hàng";
            //        }

            //        textBox.Foreground = new System.Windows.Media.SolidColorBrush(
            //            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#666666")
            //        );
            //    }
            //}
        }
    }
}