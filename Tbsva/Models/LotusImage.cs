using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class LotusImage
    {
        public int id { get; set; }

        [Key]    // 主索引鍵（P.K.）
        public Guid lotus_image_id { get; set; }

        [Required]
        [StringLength(50)]
        public string image_name { get; set; }

        public Guid? lotus_id { get; set; }

        public DateTime createDate { get; set; }
    }
}