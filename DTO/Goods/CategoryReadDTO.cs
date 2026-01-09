using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Goods
{
    public record CategoryReadDTO(
        int ItemID,
        int StoreID,
        string ItemName,
        int CategoryID,
        string Unit,
        float Price,
        float CostPrice
    );
}
