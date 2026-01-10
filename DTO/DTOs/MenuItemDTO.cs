using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOs
{
    /// <summary>
    /// DTO dùng để hiển thị trên DataGrid
    /// </summary>
    public class MenuItemDTO
    {
        public int ItemID { get; set; }
        public string Code { get; set; }          // Mã hàng hóa
        public string Name { get; set; }          // Tên hàng
        public string MenuType { get; set; }      // Loại thực đơn (CategoryName)
        public decimal Price { get; set; }        // Giá bán
        public decimal? Cost { get; set; }        // Giá vốn
        public decimal Stock { get; set; }        // Tồn kho
        public string Unit { get; set; }          // Đơn vị
        public bool IsActive { get; set; }        // Trạng thái

        // Display formatting
        public string PriceDisplay => Price.ToString("N0") + " đ";
        public string CostDisplay => (Cost ?? 0).ToString("N0") + " đ";
        public string StockDisplay => Stock.ToString("N0") + " " + Unit;
    }
}
