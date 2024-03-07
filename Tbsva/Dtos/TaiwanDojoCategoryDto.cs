using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class TaiwanDojoCategoryDto
    {
        public int id { get; set; }
        public Guid categoryId { get; set; }
        public string name { get; set; }
        public string berif { get; set; }
        public int sort { get; set; }
        public byte enabled { get; set; }
        public string creationDate { get; set; }
        //public DateTime? updatedDate { get; set; }
    }
}