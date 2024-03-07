using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DonateBuyerType
    {
        /// <summary>
        /// 捐款單位1,2
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        /// 捐款單位名稱1個人2公司
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Name { get; set; }
    }
}