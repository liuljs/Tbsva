using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class VideoCategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        //public DateTime creation_date { get; set; }

        //public DateTime? updated_date { get; set; }

        public byte Enabled { get; set; }

        public int Sort { get; set; }
    }
}