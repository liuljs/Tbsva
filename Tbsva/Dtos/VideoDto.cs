using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class VideoDto
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 活動編號 活動類別另設 ID 唯一索引用
        /// </summary>
        public Guid videoId { get; set; }

        /// <summary>
        /// 活動影片目錄名稱(活動隸屬目錄), 名稱由前端畫面自定
        /// </summary>
        public string videoCategory { get; set; }

        /// <summary>
        /// 1.上傳的封面檔
        /// 2.video=>cover 自動縮圖網址
        /// </summary>
        public string imageURL { get; set; }

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
    }
}