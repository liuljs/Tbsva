using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class ProductSpec
    {
        public int Id { get; set; }

        public int Sku_Id { get; set; }

        public string Spec_Name { get; set; }
    }
}