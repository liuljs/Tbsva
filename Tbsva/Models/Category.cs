using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public OpenStatus Enable { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }

        public List<SubCategory1> SubCategories { get; set; } = new List<SubCategory1>();
    }
}