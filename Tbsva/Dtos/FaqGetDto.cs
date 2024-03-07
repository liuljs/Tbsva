using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class FaqGetDto
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public string Asked { get; set; }

        public int Sort { get; set; }

        public byte Enabled { get; set; }
    }
}