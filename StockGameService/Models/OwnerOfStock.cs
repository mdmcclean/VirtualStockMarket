using Capstone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockGameService.Models
{
    public class OwnerOfStock
    {
        public UserItem Owner { get; set; }
        public Stock StockOwned { get; set; }
    }
}
