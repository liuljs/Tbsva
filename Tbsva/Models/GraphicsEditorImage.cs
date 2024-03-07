using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    /// <summary>
    /// 先作廢
    /// </summary>
    public class GraphicsEditorImage
    {
        public int id { get; set; }

        [Key]    // 主索引鍵（P.K.）
        public Guid graphics_editor_image_id { get; set; }

        [Required]
        [StringLength(50)]
        public string image_name { get; set; }

        /// <summary>
        /// 關聯taiwanDojoId
        /// </summary>
        public Guid? graphicsEditorId { get; set; }

        public DateTime creationDate { get; set; }
    }
}