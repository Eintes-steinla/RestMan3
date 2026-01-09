using System;
using System.Windows;
using DAL;

namespace RestMan3.Views
{
    public partial class Test : Window
    {
        public Test()
        {
            InitializeComponent();
            AppendLog("=== Test Database Connection ===\n");
            AppendLog("Nhấn các nút bên dưới để test kết nối\n\n");
        }

        private void AppendLog(string message)
        {
            txtResults.Text += message + "\n";
        }

        private void BtnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppendLog("===== TEST 1: Kiểm tra kết nối =====");

                bool isConnected = DataProvider.Instance.TestConnection();

                if (isConnected)
                {
                    AppendLog("✅ Kết nối Database THÀNH CÔNG!");

                    // Lấy thông tin DB
                    string dbInfo = DataProvider.Instance.GetDatabaseInfo();
                    txtConnectionInfo.Text = dbInfo;
                    AppendLog($"📌 {dbInfo}");
                }
                else
                {
                    AppendLog("❌ Kết nối Database THẤT BẠI!");
                    txtConnectionInfo.Text = "Không thể kết nối";
                }

                AppendLog("");
            }
            catch (Exception ex)
            {
                AppendLog($"❌ LỖI: {ex.Message}");
                AppendLog("");
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTestQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppendLog("===== TEST 2: Kiểm tra Query =====");

                // Test 1: Đếm số bảng
                string query1 = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                int tableCount = DapperHelper.ExecuteScalar<int>(query1);
                AppendLog($"✅ Số bảng trong database: {tableCount}");

                // Test 2: Lấy tên các bảng
                string query2 = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                var tables = DapperHelper.Query<string>(query2);
                AppendLog($"✅ Danh sách {tables.Count} bảng:");
                foreach (var table in tables)
                {
                    AppendLog($"   - {table}");
                }

                AppendLog("");
            }
            catch (Exception ex)
            {
                AppendLog($"❌ LỖI: {ex.Message}");
                AppendLog("");
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTestDapper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppendLog("===== TEST 3: Kiểm tra Dapper ORM =====");

                // Test query một bảng cụ thể (ví dụ: Role)
                AppendLog("📊 Test query bảng Role:");
                string query = "SELECT TOP 5 * FROM Role";
                var roles = DapperHelper.Query<dynamic>(query);

                AppendLog($"✅ Tìm thấy {roles.Count} roles:");
                foreach (var role in roles)
                {
                    AppendLog($"   - RoleID: {role.RoleID}, RoleName: {role.RoleName}");
                }

                // Test query bảng MenuCategory
                AppendLog("\n📊 Test query bảng MenuCategory:");
                query = "SELECT TOP 5 * FROM MenuCategory";
                var categories = DapperHelper.Query<dynamic>(query);

                AppendLog($"✅ Tìm thấy {categories.Count} categories:");
                foreach (var cat in categories)
                {
                    AppendLog($"   - CategoryID: {cat.CategoryID}, CategoryName: {cat.CategoryName}");
                }

                // Test count
                AppendLog("\n📊 Test count records:");
                int userCount = DapperHelper.ExecuteScalar<int>("SELECT COUNT(*) FROM UserAccount");
                int employeeCount = DapperHelper.ExecuteScalar<int>("SELECT COUNT(*) FROM Employee");
                int customerCount = DapperHelper.ExecuteScalar<int>("SELECT COUNT(*) FROM Customer");

                AppendLog($"✅ Tổng số UserAccount: {userCount}");
                AppendLog($"✅ Tổng số Employee: {employeeCount}");
                AppendLog($"✅ Tổng số Customer: {customerCount}");

                AppendLog("");
            }
            catch (Exception ex)
            {
                AppendLog($"❌ LỖI: {ex.Message}");
                AppendLog("");
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtResults.Text = "";
            AppendLog("=== Test Database Connection ===\n");
            AppendLog("Nhấn các nút bên dưới để test kết nối\n\n");
        }
    }
}