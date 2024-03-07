﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class Knowledge_image_Dto
    {
        public int id { get; set; }
        public Guid imageId { get; set; }
        public string imageURL { get; set; }
    }
}