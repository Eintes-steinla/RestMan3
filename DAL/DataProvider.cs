// ============================================================================
// File: RestMan3.DAL/DataProvider.cs
// Mô tả: Singleton class để quản lý kết nối Database
// Pattern: Singleton
// ============================================================================

using System;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    /// <summary>
    /// Singleton class quản lý kết nối đến SQL Server Database
    /// </summary>
    public class DataProvider
    {
        #region Singleton Instance

        private static DataProvider _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Lấy instance duy nhất của DataProvider (Thread-safe)
        /// </summary>
        public static DataProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DataProvider();
                        }
                    }
                }
                return _instance;
            }
        }

        // Private constructor để ngăn việc tạo instance từ bên ngoài
        private DataProvider() { }

        #endregion

        #region Connection String

        // Connection String đến SQL Server
        // TODO: Thay đổi theo môi trường của bạn
        private readonly string _connectionString = @"
            Data Source=localhost\\SQLEXPRESS;
            Initial Catalog=RestMan3;
            Integrated Security=True;
            Connect Timeout=30;
            Encrypt=False;
            TrustServerCertificate=True;
            ApplicationIntent=ReadWrite;
            MultiSubnetFailover=False
        ";

        /// <summary>
        /// Lấy Connection String
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        #endregion

        #region Connection Methods

        /// <summary>
        /// Tạo và mở một kết nối mới đến Database
        /// </summary>
        /// <returns>SqlConnection đã được mở</returns>
        public SqlConnection GetConnection()
        {
            try
            {
                var connection = new SqlConnection(_connectionString);
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                return connection;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Không thể kết nối đến Database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra kết nối Database có hoạt động không
        /// </summary>
        /// <returns>True nếu kết nối thành công, False nếu thất bại</returns>
        public bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    return connection.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy thông tin Database (tên server, database name)
        /// </summary>
        /// <returns>Thông tin database dưới dạng string</returns>
        public string GetDatabaseInfo()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    return $"Server: {connection.DataSource}, Database: {connection.Database}";
                }
            }
            catch (Exception ex)
            {
                return $"Lỗi: {ex.Message}";
            }
        }

        #endregion

        #region Execute Methods (Sử dụng ADO.NET)

        /// <summary>
        /// Thực thi câu lệnh SQL không trả về dữ liệu (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Parameters (tùy chọn)</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi khi thực thi câu lệnh: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thực thi câu lệnh SQL trả về một giá trị duy nhất (COUNT, SUM, MAX...)
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Parameters (tùy chọn)</param>
        /// <returns>Giá trị scalar</returns>
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi khi thực thi câu lệnh: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thực thi câu lệnh SQL trả về DataTable (SELECT)
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Parameters (tùy chọn)</param>
        /// <returns>DataTable chứa kết quả</returns>
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    var adapter = new SqlDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    return dataTable;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi khi truy vấn dữ liệu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thực thi Stored Procedure
        /// </summary>
        /// <param name="storedProcedureName">Tên stored procedure</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>DataTable chứa kết quả</returns>
        public DataTable ExecuteStoredProcedure(string storedProcedureName, SqlParameter[] parameters = null)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    var adapter = new SqlDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    return dataTable;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi khi thực thi Stored Procedure: {ex.Message}", ex);
            }
        }

        #endregion

        #region Transaction Support

        /// <summary>
        /// Bắt đầu một transaction
        /// </summary>
        /// <returns>SqlConnection và SqlTransaction</returns>
        public (SqlConnection connection, SqlTransaction transaction) BeginTransaction()
        {
            var connection = GetConnection();
            var transaction = connection.BeginTransaction();
            return (connection, transaction);
        }

        #endregion
    }
}

// ============================================================================
// File: RestMan3.DAL/App.config (hoặc appsettings.json)
// Mô tả: Cấu hình Connection String (Khuyến nghị)
// ============================================================================

/*
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="RestMan3DB" 
         connectionString="Data Source=.;Initial Catalog=RestMan3;Integrated Security=True" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
*/

// ============================================================================
// File: RestMan3.DAL/DataProvider.Enhanced.cs
// Mô tả: Version nâng cao với đọc Connection String từ config
// ============================================================================

/*
using System.Configuration;

namespace RestMan3.DAL
{
    public class DataProvider
    {
        // ... (code như trên)
        
        // Đọc connection string từ App.config
        private readonly string _connectionString = 
            ConfigurationManager.ConnectionStrings["RestMan3DB"].ConnectionString;
            
        // Hoặc cho phép override
        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
*/

// ============================================================================
// File: RestMan3.DAL.Tests/DataProviderTest.cs
// Mô tả: Unit Test để kiểm tra kết nối
// ============================================================================

/*
using System;
using RestMan3.DAL;

namespace RestMan3.DAL.Tests
{
    public class DataProviderTest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Test DataProvider ===\n");

            // Test 1: Singleton Instance
            Console.WriteLine("Test 1: Singleton Pattern");
            var instance1 = DataProvider.Instance;
            var instance2 = DataProvider.Instance;
            Console.WriteLine($"instance1 == instance2: {instance1 == instance2}"); // True
            Console.WriteLine();

            // Test 2: Test Connection
            Console.WriteLine("Test 2: Test Connection");
            bool isConnected = DataProvider.Instance.TestConnection();
            Console.WriteLine($"Kết nối Database: {(isConnected ? "Thành công ✓" : "Thất bại ✗")}");
            Console.WriteLine();

            // Test 3: Database Info
            Console.WriteLine("Test 3: Database Info");
            string dbInfo = DataProvider.Instance.GetDatabaseInfo();
            Console.WriteLine(dbInfo);
            Console.WriteLine();

            // Test 4: Execute Scalar
            Console.WriteLine("Test 4: Count Tables");
            try
            {
                string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                object result = DataProvider.Instance.ExecuteScalar(query);
                Console.WriteLine($"Số bảng trong database: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
            Console.WriteLine();

            // Test 5: Execute Query
            Console.WriteLine("Test 5: Get All Tables");
            try
            {
                string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                var dt = DataProvider.Instance.ExecuteQuery(query);
                Console.WriteLine($"Danh sách {dt.Rows.Count} bảng:");
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    Console.WriteLine($"  - {row["TABLE_NAME"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }

            Console.WriteLine("\n=== Test hoàn tất ===");
            Console.ReadKey();
        }
    }
}
*/

// ============================================================================
// HƯỚNG DẪN SỬ DỤNG
// ============================================================================

/*
1. THAY ĐỔI CONNECTION STRING:
   - Mở file DataProvider.cs
   - Sửa _connectionString theo môi trường của bạn:
   
   Ví dụ:
   - Local SQL Server:     "Data Source=.;Initial Catalog=RestMan3;Integrated Security=True"
   - Named Instance:       "Data Source=.\SQLEXPRESS;Initial Catalog=RestMan3;..."
   - Remote Server:        "Data Source=192.168.1.100;Initial Catalog=RestMan3;User ID=sa;Password=xxx"
   - SQL Authentication:   "Data Source=.;Initial Catalog=RestMan3;User ID=sa;Password=123456"

2. TEST KẾT NỐI:
   - Tạo Console App tạm để test
   - Copy đoạn code trong DataProviderTest
   - Run và kiểm tra output

3. SỬ DỤNG TRONG REPOSITORY:
   ```csharp
   using (var connection = DataProvider.Instance.GetConnection())
   {
       // Sử dụng connection với Dapper
       var users = connection.Query<UserAccount>("SELECT * FROM UserAccount");
   }
   ```

4. XỬ LÝ LỖI:
   - Nếu không kết nối được, check:
     ✓ SQL Server có đang chạy không
     ✓ Database RestMan3 đã tạo chưa
     ✓ Connection String đúng chưa
     ✓ Firewall có block không
     ✓ SQL Server Authentication mode

5. BẢO MẬT:
   - KHÔNG commit connection string có password vào Git
   - Dùng User Secrets hoặc Environment Variables cho production
   - Encrypt connection string trong config file
*/