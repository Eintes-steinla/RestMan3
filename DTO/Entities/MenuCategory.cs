using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entities
{
    public class MenuCategory
    {
        public int CategoryID { get; set; }
        public int? StoreID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }
}
