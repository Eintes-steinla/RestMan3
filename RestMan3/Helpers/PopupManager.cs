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
            _popups.Clear();
            _popups.AddRange(popups);
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

        private bool IsMouseOverPopup(Popup popup)
        {
            if (popup?.IsOpen != true || popup.Child is not FrameworkElement element) return false;

            try
            {
                Point mousePos = Mouse.GetPosition(element);
                return mousePos.X >= 0 && mousePos.X <= element.ActualWidth &&
                       mousePos.Y >= -10 && mousePos.Y <= element.ActualHeight;
            }
            catch { return false; }
        }
        public bool IsMouseOverAnyPopup()
        {
            // 1. Kiểm tra chuột trên các Menu Grid
            bool overMenu = _menuGrids.Any(m => {
                if (!m.IsVisible) return false;
                try
                {
                    Point pos = Mouse.GetPosition(m);
                    return pos.X >= 0 && pos.X <= m.ActualWidth && pos.Y >= 0 && pos.Y <= m.ActualHeight;
                }
                catch { return false; }
            });

            // 2. Kiểm tra chuột trên các Popups
            bool overPopup = _popups.Any(p => {
                if (p?.IsOpen != true || p.Child is not FrameworkElement element) return false;
                try
                {
                    Point mousePos = Mouse.GetPosition(element);
                    // Dùng biên độ -20 như logic cũ của bạn để bù đắp khoảng hở
                    return mousePos.X >= -5 && mousePos.X <= element.ActualWidth + 5 &&
                           mousePos.Y >= -20 && mousePos.Y <= element.ActualHeight + 5;
                }
                catch { return false; }
            });

            return overMenu || overPopup;
        }
    }
}

