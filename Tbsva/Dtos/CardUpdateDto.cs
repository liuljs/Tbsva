using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 修改購物車數量
    /// </summary>
    public class CardUpdateDto
    {
        /// <summary>
        /// 購物車品項Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public int quantity { get; set; }

    }
}