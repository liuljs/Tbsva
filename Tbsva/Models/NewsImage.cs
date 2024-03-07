using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class NewsImage
    {
        public int id { get; set; }

        [Key]    // 主索引鍵（P.K.）
        public Guid news_image_id { get; set; }

        [Required]
        [StringLength(50)]
        public string image_name { get; set; }

        public Guid? news_id { get; set; }

        public DateTime creation_date { get; set; }
    }
}