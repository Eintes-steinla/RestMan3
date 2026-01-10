using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Entities;

namespace DAL.Repositories
{
    public class MenuCategoryRepository
    {
        /// <summary>
        /// Lấy tất cả danh mục
        /// </summary>
        public List<MenuCategory> GetAll()
        {
            string sql = "SELECT * FROM MenuCategory ORDER BY CategoryName";
            return DapperHelper.Query<MenuCategory>(sql);
        }

        /// <summary>
        /// Lấy danh mục theo Store
        /// </summary>
        public List<MenuCategory> GetByStore(int? storeId)
        {
            string sql = @"
                SELECT * FROM MenuCategory 
                WHERE StoreID = @StoreId OR StoreID IS NULL
                ORDER BY CategoryName
            ";
            return DapperHelper.Query<MenuCategory>(sql, new { StoreId = storeId });
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        public MenuCategory GetById(int categoryId)
        {
            string sql = "SELECT * FROM MenuCategory WHERE CategoryID = @CategoryId";
            return DapperHelper.QueryFirstOrDefault<MenuCategory>(sql, new { CategoryId = categoryId });
        }

        /// <summary>
        /// Thêm danh mục mới
        /// </summary>
        public int Insert(MenuCategory category)
        {
            string sql = @"
                INSERT INTO MenuCategory (StoreID, CategoryName, Description)
                VALUES (@StoreID, @CategoryName, @Description);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return DapperHelper.ExecuteScalar<int>(sql, category);
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        public int Update(MenuCategory category)
        {
            string sql = @"
                UPDATE MenuCategory
                SET CategoryName = @CategoryName,
                    Description = @Description
                WHERE CategoryID = @CategoryID
            ";

            return DapperHelper.Execute(sql, category);
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        public int Delete(int categoryId)
        {
            string sql = "DELETE FROM MenuCategory WHERE CategoryID = @CategoryId";
            return DapperHelper.Execute(sql, new { CategoryId = categoryId });
        }
    }
}
