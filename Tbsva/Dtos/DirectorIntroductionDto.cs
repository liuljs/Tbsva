using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class DirectorIntroductionDto
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 編號另設 ID 唯一索引用
        /// </summary>
        public Guid directorIntroductionId { get; set; }
        /// <summary>
        /// 縮圖位置+image_name
        /// </summary>
        public string imageURL { get; set; }
        public string name { get; set; }
        public string subtitle { get; set; }
        public string brief { get; set; }
        public string content { get; set; }
        public byte first { get; set; }
        public int sort { get; set; }
        public byte enabled { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string creationDate { get; set; }
        //public DateTime? updatedDate { get; set; }
    }
}