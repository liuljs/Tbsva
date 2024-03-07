using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class Lighting_content_Dto
    {
        public Guid Id { get; set; }
        public int Lighting_Category_Id { get; set; }
        public string Image_Url { get; set; }
        public string Title { get; set; }
        public string Brief { get; set; }
        public string Content { get; set; }
        public string More_Pic_Url { get; set; }
        public string Creation_Date { get; set; }
        //public DateTime? updated_date { get; set; }
        /// <summary>
        /// bool改byte 狀態(上下架)
        /// </summary>
        public byte Enabled { get; set; }
        public int Sort { get; set; }
        public string First { get; set; }
        /// <summary>
        /// 點燈目錄
        /// </summary>
        public string Lighting_Category_Name { get; set; }
    }
}