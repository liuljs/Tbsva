using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class QueryProduct
    {
        /// <summary>
        /// 產品編號
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 分類1
        /// </summary>
        public int? cid1 { get; set; }
        /// <summary>
        /// 分類2
        /// </summary>
        public int? cid2 { get; set; }
        /// <summary>
        /// 分類3
        /// </summary>
        public int? cid3 { get; set; }
        /// <summary>
        /// 是否推薦
        /// </summary>
        public  bool? recommend { get; set; }
        /// <summary>
        /// 關鍵字查詢
        /// </summary>
        public string keyword { get; set; }
        /// <summary>
        /// 一頁幾筆
        /// </summary>
        public int? count { get; set; }
        /// <summary>
        /// 第幾頁
        /// </summary>
        public int? page { get; set; }

        /// <summary>
        /// 1.已上架,2.已售完,3.停售,4.未上架
        /// </summary>
        public string cond { get; set; }
    }
}