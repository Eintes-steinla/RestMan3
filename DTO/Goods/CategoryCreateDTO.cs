using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Goods
{
    internal class CategoryCreateDTO
    {
        public int ItemID { get; set; }
        public int StoreID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [MaxLength(200)]
        public string ItemName { get; set; }
        public int CategoryID { get; set; }
        public string Unit { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public float Price { get; set; }
        public float CostPrice { get; set; }
    }
}
