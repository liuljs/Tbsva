using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DonateAffiliatedArea
    {
        /// <summary>
        /// 隸屬地區
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        ///  0無1北部2中部3南部4海外
        /// </summary>
        [StringLength(10)]
        public string Name { get; set; }
    }
}