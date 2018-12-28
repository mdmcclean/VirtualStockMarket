using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone
{
    public class UserHoldings
    {
        public List<UserStockItem> _userStocks = new List<UserStockItem>();
        public UserHoldings(List<UserStockItem> userStocks)
        {
            _userStocks = userStocks;
        }

    }
}
