using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Knowledge_image
    {
        public int id { get; set; }

        [Key]    // 主索引鍵（P.K.）
        public Guid knowledgeImageId { get; set; }

        [StringLength(50)]
        public string imageName { get; set; }
        public Guid? knowledgetId { get; set; }
        public DateTime creationDate { get; set; }

        //public virtual Knowledge_content Knowledge_content { get; set; }
    }
}