﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class VideoImage2
    {
        public int id { get; set; }

        [Key]    // 主索引鍵（P.K.）
        public Guid videoImage2Id { get; set; }

        [Required]
        [StringLength(50)]
        public string imageName { get; set; }

        public Guid? video2Id { get; set; }

        public DateTime creationDate { get; set; }
    }
}