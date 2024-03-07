using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class article_image
    {
        public int id { get; set; }

        public Guid article_image_id { get; set; }

        public Guid article_content_id { get; set; }

        [StringLength(50)]
        public string image_name { get; set; }

        public DateTime? creation_date { get; set; }

        //public virtual article_content article_content { get; set; }
    }
}