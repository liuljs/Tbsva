using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class TaiwanDojoImage
    {
        public int id { get; set; }

        [Key]    // 主索引鍵（P.K.）
        public Guid taiwanDojoImageId { get; set; }

        [Required]
        [StringLength(50)]
        public string imageName { get; set; }

        /// <summary>
        /// 關聯taiwanDojoId
        /// </summary>
        public Guid? taiwanDojoId { get; set; }

        public DateTime creationDate { get; set; }
    }
}