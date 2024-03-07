using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Faq
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public string Asked { get; set; }

        public int Sort { get; set; }

        public byte Enabled { get; set; }
    }
}