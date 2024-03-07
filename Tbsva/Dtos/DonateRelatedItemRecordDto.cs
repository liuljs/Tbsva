using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class DonateRelatedItemRecordDto
    {
        public int id { get; set; }
        public string orderId { get; set; }
        public int amount { get; set; }
        public string title { get; set; }
        public int qty { get; set; }
    }
}