using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class GraphicsEditorDto
    {
        public int id { get; set; }
        public Guid graphicsEditorId { get; set; }
        public string title { get; set; }
        public string subTitle { get; set; }
        public string other { get; set; }
        public string content1 { get; set; }
        public string content2 { get; set; }
        public string content3 { get; set; }
        public string content4 { get; set; }
        public string content5 { get; set; }
        public string content6 { get; set; }
        public string content7 { get; set; }
        public string content8 { get; set; }
        //// 導覽圖片1~8
        public string imageURL01 { get; set; }
        public string imageURL02 { get; set; }
        public string imageURL03 { get; set; }
        public string imageURL04 { get; set; }
        public string imageURL05 { get; set; }
        public string imageURL06 { get; set; }
        public string imageURL07 { get; set; }
        public string imageURL08 { get; set; }
        public string creationDate { get; set; }
    }
}