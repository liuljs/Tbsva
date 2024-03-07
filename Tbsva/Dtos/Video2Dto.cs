using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class Video2Dto
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 活動編號 活動類別另設 ID 唯一索引用
        /// </summary>
        public Guid video2Id { get; set; }

        /// <summary>
        /// 1.上傳的封面檔Guid+.png
        /// 2.video=>cover 自動縮圖網址https://img.youtube.com/vi/e73xGcXNPsQ/sddefault.jpg
        /// </summary>
        public string imageURL { get; set; }
        public string name { get; set; }
        public string video { get; set; }
        public string brief { get; set; }
        public string content { get; set; }
        public byte first { get; set; }
        public int sort { get; set; }
        public byte enabled { get; set; }
        public string createDate { get; set; }
        //public DateTime? updateDate { get; set; }
    }
}