using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Entities;
using DTO.DTOs;

namespace DAL.Repositories
{
    public class MenuItemRepository
    {
        /// <summary>
        /// Lấy tất cả MenuItem (chưa xóa)
        /// </summary>
        public List<MenuItem> GetAll()
        {
            string sql = @"
                SELECT 
                    mi.*,
                    mc.CategoryName
                FROM MenuItem mi
                LEFT JOIN MenuCategory mc ON mi.CategoryID = mc.CategoryID
                WHERE mi.IsDelete = 0
                ORDER BY mi.ItemName
            ";

            return DapperHelper.Query<MenuItem>(sql);
        }

        /// <summary>
        /// Lấy MenuItem theo ID
        /// </summary>
        public MenuItem GetById(int itemId)
        {
            string sql = @"
                SELECT 
                    mi.*,
                    mc.CategoryName
                FROM MenuItem mi
                LEFT JOIN MenuCategory mc ON mi.CategoryID = mc.CategoryID
                WHERE mi.ItemID = @ItemId AND mi.IsDelete = 0
            ";

            return DapperHelper.QueryFirstOrDefault<MenuItem>(sql, new { ItemId = itemId });
        }

        /// <summary>
        /// Lấy MenuItem với filter và phân trang
        /// </summary>
        public PagedResultDTO<MenuItemDTO> GetPaged(MenuItemFilterDTO filter)
        {
            // Build WHERE clause dynamically
            var conditions = new List<string> { "mi.IsDelete = 0" };

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                conditions.Add("(mi.ItemName LIKE @SearchText OR CAST(mi.ItemID AS NVARCHAR) LIKE @SearchText)");
            }

            if (filter.CategoryID.HasValue)
            {
                conditions.Add("mi.CategoryID = @CategoryID");
            }

            if (filter.IsActive.HasValue)
            {
                conditions.Add("mi.IsActive = @IsActive");
            }

            string whereClause = string.Join(" AND ", conditions);

            // Count total
            string countSql = $@"
                SELECT COUNT(*) 
                FROM MenuItem mi 
                WHERE {whereClause}
            ";

            int totalCount = DapperHelper.ExecuteScalar<int>(countSql, new
            {
                SearchText = "%" + filter.SearchText + "%",
                CategoryID = filter.CategoryID,
                IsActive = filter.IsActive
            });

            // Get paged data
            string dataSql = $@"
                SELECT 
                    mi.ItemID,
                    CAST(mi.ItemID AS NVARCHAR) AS Code,
                    mi.ItemName AS Name,
                    mc.CategoryName AS MenuType,
                    mi.Price,
                    mi.CostPrice AS Cost,
                    ISNULL(ii.CurrentStock, 0) AS Stock,
                    mi.Unit,
                    mi.IsActive
                FROM MenuItem mi
                LEFT JOIN MenuCategory mc ON mi.CategoryID = mc.CategoryID
                LEFT JOIN InventoryItem ii ON mi.ItemName = ii.ItemName
                WHERE {whereClause}
                ORDER BY mi.ItemName
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY
            ";

            var items = DapperHelper.Query<MenuItemDTO>(dataSql, new
            {
                SearchText = "%" + filter.SearchText + "%",
                CategoryID = filter.CategoryID,
                IsActive = filter.IsActive,
                Offset = (filter.PageNumber - 1) * filter.PageSize,
                PageSize = filter.PageSize
            });

            return new PagedResultDTO<MenuItemDTO>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        /// <summary>
        /// Thêm MenuItem mới
        /// </summary>
        public int Insert(MenuItem item)
        {
            string sql = @"
                INSERT INTO MenuItem (StoreID, ItemName, CategoryID, Unit, Price, CostPrice, IsActive, IsDelete)
                VALUES (@StoreID, @ItemName, @CategoryID, @Unit, @Price, @CostPrice, @IsActive, 0);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return DapperHelper.ExecuteScalar<int>(sql, item);
        }

        /// <summary>
        /// Cập nhật MenuItem
        /// </summary>
        public int Update(MenuItem item)
        {
            string sql = @"
                UPDATE MenuItem
                SET ItemName = @ItemName,
                    CategoryID = @CategoryID,
                    Unit = @Unit,
                    Price = @Price,
                    CostPrice = @CostPrice,
                    IsActive = @IsActive
                WHERE ItemID = @ItemID
            ";

            return DapperHelper.Execute(sql, item);
        }

        /// <summary>
        /// Xóa mềm MenuItem
        /// </summary>
        public int Delete(int itemId)
        {
            string sql = "UPDATE MenuItem SET IsDelete = 1 WHERE ItemID = @ItemId";
            return DapperHelper.Execute(sql, new { ItemId = itemId });
        }

        /// <summary>
        /// Xóa cứng MenuItem (cẩn thận!)
        /// </summary>
        public int HardDelete(int itemId)
        {
            string sql = "DELETE FROM MenuItem WHERE ItemID = @ItemId";
            return DapperHelper.Execute(sql, new { ItemId = itemId });
        }
    }
}
