using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entities
{
    public class MenuItem
    {
        public int ItemID { get; set; }
        public int? StoreID { get; set; }
        public string ItemName { get; set; }
        public int? CategoryID { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal? CostPrice { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public System.DateTime CreatedAt { get; set; }

        // Navigation properties (optional)
        public string CategoryName { get; set; }
        public decimal? Stock { get; set; } // Từ InventoryItem nếu cần
    }
}
