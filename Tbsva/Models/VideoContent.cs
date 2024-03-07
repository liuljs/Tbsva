using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class VideoContent
    {
        public Guid id { get; set; }

        public int VideoCategory_id { get; set; }

        [Required]
        [StringLength(50)]
        public string image_name { get; set; }

        [Required]
        [StringLength(255)]
        public string title { get; set; }

        [Required]
        [StringLength(800)]
        public string brief { get; set; }

        public string content { get; set; }

        [StringLength(2000)]

        public string Video_url { get; set; }

        public DateTime creation_date { get; set; }

        public DateTime? updated_date { get; set; }

        public bool Enabled { get; set; }

        public int Sort { get; set; }

        [Required]
        [StringLength(1)]
        [RegularExpression(@"[Y][N]")]
        public string first { get; set; }

        /// <summary>
        /// 活動影片分類
        /// </summary>
        public string VideoCategoryName { get; set; }
        //public virtual VideoCategory VideoCategory { get; set; }
    }
}