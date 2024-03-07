using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebShopping.Dtos
{
    public class FaqInsertDto
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public string Asked { get; set; }

        public int Sort { get; set; }

        public byte Enabled { get; set; }

    }
}