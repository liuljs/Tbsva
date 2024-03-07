using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class PictureList
    {
        public int id { get; set; }

        [StringLength(500)]
        public string title { get; set; }

        [StringLength(2000)]
        public string more_pic_url { get; set; }

        [StringLength(255)]
        public string Picture01 { get; set; }

        [StringLength(255)]
        public string Picture02 { get; set; }

        [StringLength(255)]
        public string Picture03 { get; set; }

        [StringLength(255)]
        public string Picture04 { get; set; }

        [StringLength(255)]
        public string Picture05 { get; set; }

        [StringLength(255)]
        public string Picture06 { get; set; }

        [StringLength(255)]
        public string Picture07 { get; set; }

        [StringLength(255)]
        public string Picture08 { get; set; }

        [StringLength(255)]
        public string Picture09 { get; set; }

        [StringLength(255)]
        public string Picture10 { get; set; }
       
        public DateTime? updated_date { get; set; }
    }
}