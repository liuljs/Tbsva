using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DonateRelatedItemRecord
    {
        /// <summary>
        /// 訂購明細記錄 
        /// </summary>
         
        public int Id { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        [StringLength(11)]
        public string OrderId { get; set; }

        /// <summary>
        /// 相關請購品名標題
        /// </summary>
        [StringLength(50)]
        public string Title { get; set; }

        /// <summary>
        /// 單筆金額
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        public int Qty { get; set; }
    }
}