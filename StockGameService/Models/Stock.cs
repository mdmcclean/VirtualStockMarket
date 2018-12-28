using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone
{
    public class Stock
    {
        public int StockID { get; set; }
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public double CurrentPrice { get; set; }
        public int AvailableShares { get; set; }
    }
}