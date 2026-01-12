using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Windows.Controls;

namespace RestMan3.Helpers
{
    /// <summary>
    /// Singleton để quản lý Popup tập trung
    /// </summary>
    public class PopupManager
    {
        private static PopupManager _instance;
        public static PopupManager Instance => _instance ??= new PopupManager();

        private List<Popup> _popups = new List<Popup>();
        private List<FrameworkElement> _menuGrids = new List<FrameworkElement>();
        private DispatcherTimer _closeTimer;

        public Action<string> OnNavigate { get; set; }

        private PopupManager()
        {
            _closeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _closeTimer.Tick += CloseTimer_Tick;
        }

        /// <summary>
        /// Đăng ký danh sách Popup để quản lý
        /// </summary>
        public void RegisterPopups(params Popup[] popups)
        {
            foreach (var p in popups)
            {
                if (!_popups.Contains(p)) // Chỉ thêm nếu chưa có
                {
                    _popups.Add(p);
                }
            }
        }

        /// <summary>
        /// Đóng tất cả Popup
        /// </summary>
        public void CloseAll()
        {
            _popups.ForEach(p => p.IsOpen = false);
        }

        /// <summary>
        /// Kiểm tra có Popup nào đang mở
        /// </summary>
        public bool IsAnyOpen() => _popups.Any(p => p.IsOpen);

        /// <summary>
        /// Mở Popup cụ thể và đóng các popup khác
        /// </summary>
        public void OpenPopup(Popup popup)
        {
            CloseAll();
            if (popup != null)
            {
                popup.IsOpen = true;
            }
        }

        /// <summary>
        /// Start close timer
        /// </summary>
        public void StartCloseTimer()
        {
            _closeTimer.Start();
        }

        /// <summary>
        /// Stop close timer
        /// </summary>
        public void StopCloseTimer()
        {
            _closeTimer.Stop();
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            // Tự lấy vị trí chuột hiện tại so với màn hình
            if (!IsMouseOverAnyPopup())
            {
                CloseAll();
            }
            _closeTimer.Stop();
        }

        /// <summary>
        /// Kiểm tra chuột có đang ở trên Popup
        /// </summary>

        public void RegisterMenuGrids(params FrameworkElement[] grids)
        {
            foreach (var grid in grids)
            {
                if (!_menuGrids.Contains(grid))
                {
                    _menuGrids.Add(grid);
                }
            }
        }
        public bool IsMouseOverAnyPopup()
        {
            // 1. Kiểm tra chuột trên các Menu Grid
            bool overMenu = _menuGrids.Any(m =>
            {
                if (!m.IsVisible) return false;
                try
                {
                    Point pos = Mouse.GetPosition(m);
                    // Mở rộng vùng kiểm tra để bao gồm cả các phần tử con
                    return pos.X >= -5 && pos.X <= m.ActualWidth + 5 &&
                           pos.Y >= -5 && pos.Y <= m.ActualHeight + 5;
                }
                catch { return false; }
            });

            // 2. Kiểm tra chuột trên các Popups
            bool overPopup = _popups.Any(p =>
            {
                if (p?.IsOpen != true || p.Child is not FrameworkElement element) return false;
                try
                {
                    Point mousePos = Mouse.GetPosition(element);
                    return mousePos.X >= -5 && mousePos.X <= element.ActualWidth + 5 &&
                           mousePos.Y >= -20 && mousePos.Y <= element.ActualHeight + 5;
                }
                catch { return false; }
            });

            return overMenu || overPopup;
        }

        #region Popup Custom Placement

        public CustomPopupPlacement[] PlacePopupRightAligned(Size popupSize, Size targetSize, Point offset)
        {
            // Tính toán để cạnh phải của Popup khớp với cạnh phải của Button
            double x = targetSize.Width - popupSize.Width;
            double y = targetSize.Height;

            return new[] { new CustomPopupPlacement(new Point(x, y), PopupPrimaryAxis.None) };
        }


        public static readonly DependencyProperty UseRightAlignedPlacementProperty =
                               DependencyProperty.RegisterAttached("UseRightAlignedPlacement", typeof(bool), typeof(PopupManager),
                               new PropertyMetadata(false, OnUseRightAlignedPlacementChanged));

        public static void SetUseRightAlignedPlacement(DependencyObject element, bool value) => element.SetValue(UseRightAlignedPlacementProperty, value);
        public static bool GetUseRightAlignedPlacement(DependencyObject element) => (bool)element.GetValue(UseRightAlignedPlacementProperty);

        private static void OnUseRightAlignedPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Popup popup && (bool)e.NewValue)
            {
                // Tự động gán hàm callback từ Singleton Instance
                popup.CustomPopupPlacementCallback = Instance.PlacePopupRightAligned;
            }
        }
        #endregion

        /// <summary>
        /// Xử lý Click tập trung cho tất cả SubMenu Items
        /// </summary>
        public void HandleSubMenuItemClick(object sender)
        {
            if (sender is Button button && button.Content != null)
            {
                string menuItem = button.Content.ToString()!;

                // 1. Đóng tất cả popup ngay lập tức
                CloseAll();

                // 2. Gọi lệnh chuyển trang (MainWindow sẽ thực hiện)
                OnNavigate?.Invoke(menuItem);
            }
        }
    }
}

