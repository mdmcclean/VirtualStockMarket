using Capstone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockGameService.Models
{
    public class UserCash
    {
        public double CurrentCash { get; set; }
        public double TotalCash { get; set; }
        public double StockWorth {
            get
            {
                return TotalCash - CurrentCash;
            }
        }
        public int IdOfUser { get; set; }
        public UserItem UserInfo { get; set; }
        public int? OwnedStocks { get; set; }
    }
}
