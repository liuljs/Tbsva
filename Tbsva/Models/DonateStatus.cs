using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DonateStatus
    {
        /// <summary>
        /// 狀態 1 2
        /// </summary>
        public byte id { get; set; }

        /// <summary>
        /// 狀態 名稱 1未結案2已結案
        /// </summary>
        [StringLength(10)]
        public string name { get; set; }
    }
}