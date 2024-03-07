using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DonatePayStatus
    {
        /// <summary>
        /// 繳款狀態 1 2
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        /// 繳款狀態 名稱 1待付款2已付款
        /// </summary>
        [StringLength(10)]
        public string Name { get; set; }
    }
}