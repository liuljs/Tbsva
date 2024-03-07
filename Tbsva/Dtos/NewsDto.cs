using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class NewsDto
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 活動編號 活動類別另設 ID 唯一索引用
        /// </summary>
        public Guid newsId { get; set; }

        /// <summary>
        /// 縮圖位置+image_name
        /// </summary>
        public string cover { get; set; }
        public string name { get; set; }
        public string video { get; set; }
        public string brief { get; set; }
        public string content { get; set; }
        public byte first { get; set; }
        public int sort { get; set; }
        public byte enabled { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string createDate { get; set; }
        //public DateTime? updateDate { get; set; }
        public string imageURL01 { get; set; }
        public string imageURL02 { get; set; }
        public string imageURL03 { get; set; }
        public string imageURL04 { get; set; }
        public string imageURL05 { get; set; }
        public string imageURL06 { get; set; }
        public string imageURL07 { get; set; }
        public string imageURL08 { get; set; }
    }
}