﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class article_image_Dto
    {
        public int id { get; set; }
        public Guid imageId { get; set; }
        public string imageURL { get; set; }
    }
}