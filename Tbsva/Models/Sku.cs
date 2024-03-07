using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Sku
    {
        /// <summary>
        /// 規格Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 產品ID
        /// </summary>
        public int Spu_Id { get; set; }

        /// <summary>
        /// 規格名稱
        /// </summary>
        public string Title { get; set; }

        //public decimal Price { get; set; }

        /// <summary>
        /// 規格售價
        /// </summary>
        public decimal Sell_Price { get; set; }

        /// <summary>
        /// 是否開啟
        /// </summary>
        public byte Enabled { get; set; }

        /// <summary>
        /// 目前庫存量
        /// </summary>
        public int Stock_Qty { get; set; }

        /// <summary>
        /// 起始庫存量
        /// </summary>
        public int Start_Stock_Qty { get; set; }

        /// <summary>
        /// 銷售量
        /// </summary>
        public int Sell_Qty { get; set; }

        /// <summary>
        /// 安全庫存量
        /// </summary>
        public int Safety_Stock_Qty { get; set; }

        /// <summary>
        /// 打折%數
        /// </summary>
        public int Discount_Percent { get; set; }

        /// <summary>
        /// 折扣後價格
        /// </summary>
        public decimal Discount_Price { get; set; }


        //public SkuMarketing SkuMarketing { get; set; }

        //public List<ProductSpec> Specs { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime Created_At { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime Updated_At { get; set; }
    }
}