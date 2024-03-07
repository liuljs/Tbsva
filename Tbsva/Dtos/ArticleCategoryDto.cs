using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class article_category_Dto
    {
        public int id { get; set; }

        public string name { get; set; }

        public string content { get; set; }

        //public DateTime creation_date { get; set; }

        /// <summary>
        /// 狀態(上下架)
        /// </summary>
        public byte enabled { get; set; }

        public int sort { get; set; }
    }
}