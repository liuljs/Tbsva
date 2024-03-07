using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class SubCategory2
    {
        public int Id { get; set; }

        public int Parent_id { get; set; }

        public string Name { get; set; }

        public OpenStatus Enable { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }
    }
}