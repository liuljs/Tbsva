using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class article_content_Dto
    {
        public Guid id { get; set; }
        public int articleCategoryId { get; set; }
        public string title { get; set; }
        public string subtitle { get; set; }
        public string imageURL { get; set; }
        public string brief { get; set; }
        public string content { get; set; }
        public string creationDate { get; set; }
        //public DateTime creation_date { get; set; }
        /// <summary>
        /// 狀態(上下架)
        /// </summary>
        public byte enabled { get; set; }
        public int sort { get; set; }
        //public string first { get; set; }
        public byte first { get; set; }
        public string articleCategoryName { get; set; }
        /// <summary>
        /// 文章分類
        /// </summary>
        //public article_category_Dto Article_Category_Collection { get; set; }
    }
}