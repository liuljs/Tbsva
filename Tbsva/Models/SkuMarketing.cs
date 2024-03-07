using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class SkuMarketing
    {
        public int Id { get; set; }

        public int Sku_Id { get; set; }

        public string Title { get; set; }

        public decimal Discount_Percent { get; set; }

        public DateTime Starts_At { get; set; }

        public DateTime Ends_At { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }
    }
}