using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class ActivityCategoryDto
    {
        public int id { get; set; }
        public string categoryId { get; set; }
        public string name { get; set; }
        public string berif { get; set; }
        public int sort { get; set; }
        public byte enabled { get; set; }
        public string createDate { get; set; }        
        //public DateTime? updated_date { get; set; }
    }
}