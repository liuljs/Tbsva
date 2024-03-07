using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DonateType
    {
        /// <summary>
        ///捐款方式
        /// </summary>
        public byte Id { get; set; }

        /// <summary>
        ///  1一般捐款 2結緣捐贈
        /// </summary>
        [StringLength(10)]
        public string Name { get; set; }
    }
}