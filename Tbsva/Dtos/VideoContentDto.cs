using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class VideoContentDto
    {
        public Guid Id { get; set; }
        public int VideoCategory_id { get; set; }              
        public string Image_Url { get; set; }
        public string Title { get; set; }
        public string Brief { get; set; }
        public string Content { get; set; }             
        public string Video_url { get; set; }
        public string Creation_Date { get; set; }
        //public DateTime? Updated_Date { get; set; }
        public byte Enabled { get; set; }
        public int Sort { get; set; }
        public string First { get; set; }

        /// <summary>
        /// 活動影片分類
        /// </summary>
        public string VideoCategoryName { get; set; }
        //public virtual VideoCategory VideoCategory { get; set; }
    }
}