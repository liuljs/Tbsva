using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Stock
    {
        public int Id { get; set; }

        public int Sku_Id { get; set; }

        public int Stock_Qty { get; set; }

        public int Start_Stock_Qty { get; set; }

        public int Sell_Qty { get; set; }

        public int Safety_Stock_Qty { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }
    }
}