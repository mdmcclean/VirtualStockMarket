using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone;

namespace Capstone
{
    public class AvailableStocks
    {
        public List<Stock> _stocks = new List<Stock>();
        public AvailableStocks(List<Stock> stocks)
        {
            _stocks = stocks;
        }

    }
}
