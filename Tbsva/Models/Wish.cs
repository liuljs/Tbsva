using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class WishReceiveDto
    {
        /// <summary>
        /// 會員Id
        /// </summary>
        public Guid Member_Id { get; set; }
        /// <summary>
        /// 規格編號
        /// </summary>
        public int Sku_Id { get; set; }

    }

    public class WishReturnDto
    {
        /// <summary>
        /// 會員Id
        /// </summary>
        public Guid Member_Id { get; set; }
        
        /// <summary>
        /// 產品編號
        /// </summary>
        public int Spu_Id { get; set; }

        /// <summary>
        /// 規格編號
        /// </summary>
        public int Sku_Id { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string SPU_Title { get; set; }

        /// <summary>
        /// 規格名稱
        /// </summary>
        public string SKU_Title { get; set; }

        /// <summary>
        /// 產品封面圖
        /// </summary>
        public string Product_cover { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public int Sell_price { get; set; }

        /// <summary>
        /// 庫存量
        /// </summary>
        public int Stock_qty { get; set; }

        /// <summary>
        /// 折價後金額
        /// </summary>
        public int discount_price { get; set; }

        /// <summary>
        /// 折價%數
        /// </summary>
        public int discount_percent { get; set; }


    }
}