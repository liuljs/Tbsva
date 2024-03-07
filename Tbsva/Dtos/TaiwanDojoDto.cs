using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class TaiwanDojoDto
    {
        public int id { get; set; }
        public Guid categoryId { get; set; }
        public Guid taiwanDojoId { get; set; }
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string brief { get; set; }
        public string area { get; set; }
        public string address { get; set; }
        public string video { get; set; }
        public string content { get; set; }
        public byte icon { get; set; }
        public byte first { get; set; }
        public int sort { get; set; }
        public byte enabled { get; set; }
        public string creationDate { get; set; }
        //public DateTime? updatedDate { get; set; }
        public string categoryName { get; set; }
    }
}