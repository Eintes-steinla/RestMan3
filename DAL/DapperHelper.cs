// ============================================================================
// File: RestMan3.DAL/DapperHelper.cs
// Mô tả: Helper class sử dụng Dapper ORM để truy vấn database
// Lợi ích: Code ngắn gọn hơn, mapping tự động từ DB sang Object
// ============================================================================

using DAL;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    /// <summary>
    /// Helper class sử dụng Dapper để thao tác với Database
    /// </summary>
    public class DapperHelper
    {
        #region Query Methods

        /// <summary>
        /// Query và trả về danh sách object
        /// </summary>
        /// <typeparam name="T">Kiểu object cần map</typeparam>
        /// <param name="sql">Câu SQL query</param>
        /// <param name="param">Parameters (anonymous object)</param>
        /// <returns>List of T</returns>
        public static List<T> Query<T>(string sql, object param = null)
        {
            try
            {
                using (var connection = DataProvider.Instance.GetConnection())
                {
                    return connection.Query<T>(sql, param).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi query: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Query và trả về object đầu tiên (hoặc null)
        /// </summary>
        /// <typeparam name="T">Kiểu object</typeparam>
        /// <param name="sql">Câu SQL query</param>
        /// <param name="param">Parameters</param>
        /// <returns>Object hoặc null</returns>
        public static T QueryFirstOrDefault<T>(string sql, object param = null)
        {
            try
            {
                using (var connection = DataProvider.Instance.GetConnection())
                {
                    return connection.QueryFirstOrDefault<T>(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi query: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Query và trả về single object (throw exception nếu không có hoặc nhiều hơn 1)
        /// </summary>
        /// <typeparam name="T">Kiểu object</typeparam>
        /// <param name="sql">Câu SQL query</param>
        /// <param name="param">Parameters</param>
        /// <returns>Object duy nhất</returns>
        public static T QuerySingle<T>(string sql, object param = null)
        {
            try
            {
                using (var connection = DataProvider.Instance.GetConnection())
                {
                    return connection.QuerySingle<T>(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi query: {ex.Message}", ex);
            }
        }

        #endregion

        #region Execute Methods

        /// <summary>
        /// Thực thi câu lệnh không trả về dữ liệu (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="sql">Câu SQL</param>
        /// <param name="param">Parameters</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public static int Execute(string sql, object param = null)
        {
            try
            {
                using (var connection = DataProvider.Instance.GetConnection())
                {
                    return connection.Execute(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi execute: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thực thi và trả về giá trị scalar (COUNT, SUM, MAX...)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
        /// <param name="sql">Câu SQL</param>
        /// <param name="param">Parameters</param>
        /// <returns>Giá trị scalar</returns>
        public static T ExecuteScalar<T>(string sql, object param = null)
        {
            try
            {
                using (var connection = DataProvider.Instance.GetConnection())
                {
                    return connection.ExecuteScalar<T>(sql, param);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi execute scalar: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Insert và trả về ID vừa tạo (dùng SCOPE_IDENTITY)
        /// </summary>
        /// <param name="sql">Câu SQL INSERT</param>
        /// <param name="param">Parameters</param>
        /// <returns>ID vừa insert</returns>
        public static int InsertAndGetId(string sql, object param = null)
        {
            try
            {
                // Thêm SELECT SCOPE_IDENTITY() để lấy ID
                string sqlWithIdentity = sql + "; SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var connection = DataProvider.Instance.GetConnection())
                {
                    return connection.ExecuteScalar<int>(sqlWithIdentity, param);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi insert: {ex.Message}", ex);
            }
        }

        #endregion

        #region Transaction Methods

        /// <summary>
        /// Thực thi nhiều câu lệnh trong một transaction
        /// </summary>
        /// <param name="action">Action chứa các câu lệnh cần execute</param>
        /// <returns>True nếu thành công</returns>
        public static bool ExecuteTransaction(Action<SqlConnection, SqlTransaction> action)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;

            try
            {
                connection = DataProvider.Instance.GetConnection();
                transaction = connection.BeginTransaction();

                // Thực thi action
                action(connection, transaction);

                // Commit nếu không có lỗi
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Rollback nếu có lỗi
                transaction?.Rollback();
                throw new Exception($"Lỗi transaction: {ex.Message}", ex);
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        #endregion

        #region Stored Procedure Methods

        /// <summary>
        /// Thực thi stored procedure và trả về danh sách
        /// </summary>
        /// <typeparam name="T">Kiểu object</typeparam>
        /// <param name="storedProcedure">Tên stored procedure</param>
        /// <param name="param">Parameters</param>
        /// <returns>List of T</returns>
        public static List<T> ExecuteStoredProcedure<T>(string storedProcedure, object param = null)
        {
            try
            {
                using (var connection = DataProvider.Instance.GetConnection())
                {
                    return connection.Query<T>(
                        storedProcedure,
                        param,
                        commandType: CommandType.StoredProcedure
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi execute SP: {ex.Message}", ex);
            }
        }

        #endregion

        #region Bulk Operations

        /// <summary>
        /// Insert nhiều records cùng lúc
        /// </summary>
        /// <typeparam name="T">Kiểu object</typeparam>
        /// <param name="sql">Câu SQL INSERT</param>
        /// <param name="entities">Danh sách objects</param>
        /// <returns>Số dòng được insert</returns>
        public static int BulkInsert<T>(string sql, IEnumerable<T> entities)
        {
            try
            {
                using (var connection = DataProvider.Instance.GetConnection())
                {
                    return connection.Execute(sql, entities);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi bulk insert: {ex.Message}", ex);
            }
        }

        #endregion
    }
}

// ============================================================================
// EXAMPLES - Ví dụ sử dụng DapperHelper
// ============================================================================

/*
// --- VÍ DỤ 1: Query danh sách ---
var users = DapperHelper.Query<UserAccount>("SELECT * FROM UserAccount WHERE IsActive = 1");

// --- VÍ DỤ 2: Query với parameters ---
var user = DapperHelper.QueryFirstOrDefault<UserAccount>(
    "SELECT * FROM UserAccount WHERE Username = @Username",
    new { Username = "admin" }
);

// --- VÍ DỤ 3: Insert ---
string insertSql = @"
    INSERT INTO Customer (FullName, Phone, Email, MembershipLevel, Points)
    VALUES (@FullName, @Phone, @Email, @MembershipLevel, @Points)
";
int rowsAffected = DapperHelper.Execute(insertSql, new
{
    FullName = "Nguyễn Văn A",
    Phone = "0901234567",
    Email = "a@email.com",
    MembershipLevel = "Normal",
    Points = 0
});

// --- VÍ DỤ 4: Insert và lấy ID ---
int newCustomerId = DapperHelper.InsertAndGetId(insertSql, new
{
    FullName = "Nguyễn Văn B",
    Phone = "0901234568",
    Email = "b@email.com",
    MembershipLevel = "Normal",
    Points = 0
});

// --- VÍ DỤ 5: Update ---
string updateSql = @"
    UPDATE Customer 
    SET Points = Points + @PointsToAdd 
    WHERE CustomerID = @CustomerId
";
DapperHelper.Execute(updateSql, new { PointsToAdd = 100, CustomerId = 1 });

// --- VÍ DỤ 6: Delete (Soft Delete) ---
string deleteSql = "UPDATE UserAccount SET IsDelete = 1 WHERE UserID = @UserId";
DapperHelper.Execute(deleteSql, new { UserId = 5 });

// --- VÍ DỤ 7: Count ---
int totalUsers = DapperHelper.ExecuteScalar<int>("SELECT COUNT(*) FROM UserAccount WHERE IsActive = 1");

// --- VÍ DỤ 8: Transaction ---
bool success = DapperHelper.ExecuteTransaction((connection, transaction) =>
{
    // Tạo Order
    string insertOrderSql = @"
        INSERT INTO [Order] (BranchID, CustomerID, EmployeeID, Status)
        VALUES (@BranchID, @CustomerID, @EmployeeID, 'Pending');
        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";
    int orderId = connection.ExecuteScalar<int>(insertOrderSql, new
    {
        BranchID = 1,
        CustomerID = 1,
        EmployeeID = 2
    }, transaction);

    // Thêm OrderDetail
    string insertDetailSql = @"
        INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitPrice)
        VALUES (@OrderID, @ItemID, @Quantity, @UnitPrice)
    ";
    connection.Execute(insertDetailSql, new
    {
        OrderID = orderId,
        ItemID = 1,
        Quantity = 2,
        UnitPrice = 65000
    }, transaction);
});

// --- VÍ DỤ 9: Query phức tạp với JOIN ---
string joinSql = @"
    SELECT 
        ua.UserID,
        ua.Username,
        e.FullName,
        e.Salary,
        r.RoleName
    FROM UserAccount ua
    INNER JOIN AccountProfileLink apl ON ua.UserID = apl.UserID
    INNER JOIN Employee e ON apl.EmployeeID = e.EmployeeID
    INNER JOIN Role r ON ua.RoleID = r.RoleID
    WHERE ua.IsActive = 1
";

// Sử dụng anonymous type hoặc tạo DTO
var employeeInfo = DapperHelper.Query<dynamic>(joinSql);
foreach (var emp in employeeInfo)
{
    Console.WriteLine($"{emp.Username} - {emp.FullName} - {emp.RoleName}");
}

// --- VÍ DỤ 10: Bulk Insert ---
var customers = new List<Customer>
{
    new Customer { FullName = "A", Phone = "111", Email = "a@test.com" },
    new Customer { FullName = "B", Phone = "222", Email = "b@test.com" },
    new Customer { FullName = "C", Phone = "333", Email = "c@test.com" }
};

string bulkSql = @"
    INSERT INTO Customer (FullName, Phone, Email, MembershipLevel, Points)
    VALUES (@FullName, @Phone, @Email, 'Normal', 0)
";
int insertedRows = DapperHelper.BulkInsert(bulkSql, customers);
*/

// ============================================================================
// SO SÁNH: ADO.NET vs Dapper
// ============================================================================

/*
// --- SỬ DỤNG ADO.NET (Cách cũ - dài dòng) ---
using (var connection = DataProvider.Instance.GetConnection())
using (var command = new SqlCommand("SELECT * FROM UserAccount WHERE UserID = @UserId", connection))
{
    command.Parameters.AddWithValue("@UserId", 1);
    using (var reader = command.ExecuteReader())
    {
        if (reader.Read())
        {
            var user = new UserAccount
            {
                UserID = (int)reader["UserID"],
                Username = reader["Username"].ToString(),
                Email = reader["Email"].ToString(),
                // ... mapping thủ công từng field
            };
        }
    }
}

// --- SỬ DỤNG DAPPER (Cách mới - ngắn gọn) ---
var user = DapperHelper.QueryFirstOrDefault<UserAccount>(
    "SELECT * FROM UserAccount WHERE UserID = @UserId",
    new { UserId = 1 }
);
// ✅ Tự động mapping vào object!
// ✅ Code ngắn hơn 80%!
// ✅ An toàn với SQL Injection!
*/

// ============================================================================
// TIPS & BEST PRACTICES
// ============================================================================

/*
1. LUÔN DÙNG PARAMETERS - Tránh SQL Injection:
   ✅ ĐÚNG: DapperHelper.Query<T>("SELECT * FROM Users WHERE Id = @Id", new { Id = 1 });
   ❌ SAI:  DapperHelper.Query<T>($"SELECT * FROM Users WHERE Id = {id}");

2. SỬ DỤNG TRANSACTION cho multiple operations:
   - Tạo Order + OrderDetail
   - Update Inventory + Create Issue
   - Insert User + Employee + AccountProfileLink

3. QUERY chỉ những cột cần thiết:
   ✅ ĐÚNG: SELECT UserID, Username, Email FROM UserAccount
   ❌ SAI:  SELECT * FROM UserAccount (nặng hơn)

4. SỬ DỤNG ASYNC cho app lớn (nâng cao):
   var users = await connection.QueryAsync<UserAccount>(sql);

5. NAMING CONVENTION:
   - Tên parameter phải match với tên property (@Username = Username)
   - Hoặc dùng exact column name từ DB
*/