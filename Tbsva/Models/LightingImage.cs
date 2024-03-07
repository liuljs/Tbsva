using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Lighting_image
    {
        public Guid id { get; set; }

        [Required]
        [StringLength(50)]
        public string image_name { get; set; }

        public Guid? Lighting_content_id { get; set; }

        public DateTime creation_date { get; set; }

        //public virtual Lighting_content Lighting_content { get; set; }
    }
}