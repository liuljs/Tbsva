using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class LotusImageDto
    {
        public int id { get; set; }
        public Guid imageId { get; set; }
        public string imageURL { get; set; }
    }
}