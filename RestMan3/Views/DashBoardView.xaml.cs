using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RestMan3.Views
{
    /// <summary>
    /// Interaction logic for DashBoardView.xaml
    /// </summary>
    public partial class DashBoardView : UserControl
    {
        public DashBoardView()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            // TODO: Load dữ liệu thống kê từ database hoặc service
            // Ví dụ:
            // - Tổng doanh thu hôm nay
            // - Số đơn hàng đã xong
            // - Số khách hàng mới
            // - Dữ liệu biểu đồ doanh số theo giờ
            // - Top 10 hàng hóa bán chạy
            // - Lịch sử hoạt động gần đây
        }
    }
}
