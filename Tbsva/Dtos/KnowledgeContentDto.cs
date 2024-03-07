using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class Knowledge_content_Dto
    {
        public int id { get; set; }
        public Guid knowledgetId { get; set; }
        public string imageURL { get; set; }
        public string title { get; set; }
        public string category { get; set; }
        public string number { get; set; }
        public string brief { get; set; }
        public string content { get; set; }
        public byte first { get; set; }
        public int sort { get; set; }
        public byte enabled { get; set; }
        public string creationDate { get; set; }
    }
}