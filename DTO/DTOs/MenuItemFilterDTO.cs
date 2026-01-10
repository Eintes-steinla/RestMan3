using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOs
{
    /// <summary>
    /// DTO chứa các điều kiện lọc
    /// </summary>
    public class MenuItemFilterDTO
    {
        public string SearchText { get; set; }
        public int? CategoryID { get; set; }
        public bool? IsActive { get; set; }
        public string StockStatus { get; set; } // "All", "LowStock", "OutOfStock"
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
