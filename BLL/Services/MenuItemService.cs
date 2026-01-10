using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories;
using DTO.Entities;
using DTO.DTOs;

namespace BLL.Services
{
    public class MenuItemService
    {
        private readonly MenuItemRepository _menuItemRepo;
        private readonly MenuCategoryRepository _categoryRepo;

        public MenuItemService()
        {
            _menuItemRepo = new MenuItemRepository();
            _categoryRepo = new MenuCategoryRepository();
        }

        /// <summary>
        /// Lấy danh sách MenuItem có phân trang và filter
        /// </summary>
        public PagedResultDTO<MenuItemDTO> GetMenuItems(MenuItemFilterDTO filter)
        {
            try
            {
                return _menuItemRepo.GetPaged(filter);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách hàng hóa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy tất cả danh mục
        /// </summary>
        public List<MenuCategory> GetAllCategories()
        {
            try
            {
                return _categoryRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách danh mục: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Thêm MenuItem mới
        /// </summary>
        public int AddMenuItem(MenuItem item)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(item.ItemName))
                    throw new Exception("Tên hàng hóa không được để trống");

                if (item.Price < 0)
                    throw new Exception("Giá bán phải >= 0");

                item.IsActive = true;
                item.IsDelete = false;
                item.CreatedAt = DateTime.Now;

                return _menuItemRepo.Insert(item);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm hàng hóa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Cập nhật MenuItem
        /// </summary>
        public bool UpdateMenuItem(MenuItem item)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(item.ItemName))
                    throw new Exception("Tên hàng hóa không được để trống");

                if (item.Price < 0)
                    throw new Exception("Giá bán phải >= 0");

                int result = _menuItemRepo.Update(item);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật hàng hóa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Xóa MenuItem
        /// </summary>
        public bool DeleteMenuItem(int itemId)
        {
            try
            {
                int result = _menuItemRepo.Delete(itemId);
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa hàng hóa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Lấy MenuItem theo ID
        /// </summary>
        public MenuItem GetMenuItemById(int itemId)
        {
            try
            {
                return _menuItemRepo.GetById(itemId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin hàng hóa: {ex.Message}", ex);
            }
        }
    }
}
