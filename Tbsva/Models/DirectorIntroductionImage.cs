using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DirectorIntroductionImage
    {
        public int id { get; set; }

        [Key]    // 主索引鍵（P.K.）
        public Guid directorIntroductionImageId { get; set; }

        [Required]
        [StringLength(50)]
        public string imageName { get; set; }

        public Guid? directorIntroductionId { get; set; }

        public DateTime creationDate { get; set; }
    }
}