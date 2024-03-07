using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public enum PreserveStatus
    {
        General, //一般時程
        Long //較長時程
    }

    public enum ProductStatus
    {
        BrandNew,  //全新
        SecondHand //二手
    }

    public class Spu
    {
        /// <summary>
        /// 產品ID，修改時為必填
        /// </summary>
        public int Id { get; set; }

        public string Title { get; set; }

        public byte Enabled { get; set; }

        public OpenStatus Recommend { get; set; }

        public OpenStatus Sell_Stop { get; set; }

        public OpenStatus View_Stock { get; set; }

        public OpenStatus View_Sell_Num { get; set; }

        public OpenStatus Lead_Time { get; set; }

        public PreserveStatus Preserve_Status { get; set; }

        public ProductStatus Product_Status { get; set; }

        public DateTime Starts_At { get; set; }

        public DateTime Ends_At { get; set; }

        public string Product_Cover { get; set; }

        public string Spec { get; set; }

        public string Describe { get; set; }

        public string Product01 { get; set; }

        public string Product02 { get; set; }

        public string Product03 { get; set; }

        public string Product04 { get; set; }

        public string Product05 { get; set; }

        public string Product06 { get; set; }

        public decimal Price { get; set; }

        public string Marketing_Title { get; set; }

        public DateTime Marketing_Starts_At { get; set; }

        public DateTime Marketing_Ends_At { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }

        /// <summary>
        /// 產品介紹
        /// </summary>
        public SpuDetail Detail { get; set; }
        /// <summary>
        /// 產品分類
        /// </summary>
        public SpuCategory ProductCategory { get; set; }
        /// <summary>
        /// 運費資料
        /// </summary>
        public List<SpuLogistics> Logistics { get; set; }
        /// <summary>
        /// 產品規格
        /// </summary>
        public List<Sku> SellInfos { get; set; }
        /// <summary>
        /// 產品介紹圖片
        /// </summary>
        public List<SpuIntroductionImage> IntroductionImage { get; set; }

        /// <summary>
        /// 產品庫存量(所有規格庫存量加總)
        /// 取得產品列表時提供
        /// </summary>
        public int stock_qty { get; set; }

        /// <summary>
        /// 產品銷售量(所有規格銷售量加總)
        /// 取得產品列表時提供
        /// </summary>
        public int sell_qty { get; set; }

        ///// <summary>
        ///// 格式化日期
        ///// </summary>
        ///// <param name="date"></param>
        ///// <returns></returns>
        //public string FormatDate(DateTime date) {
        //    if (date == DateTime.MinValue)
        //        return "";
        //    return date.ToString("yyyy/MM/dd HH:mm");
        //}
    }

    public class SpuGetDto
    {
        /// <summary>
        /// 產品ID，修改時為必填
        /// </summary>
        public int Id { get; set; }

        public string Title { get; set; }

        public byte Enabled { get; set; }

        public OpenStatus Recommend { get; set; }

        public OpenStatus Sell_Stop { get; set; }

        public OpenStatus View_Stock { get; set; }

        public OpenStatus View_Sell_Num { get; set; }

        public OpenStatus Lead_Time { get; set; }

        public PreserveStatus Preserve_Status { get; set; }

        public ProductStatus Product_Status { get; set; }

        public string Starts_At { get; set; }

        public string Ends_At { get; set; }

        public string Product_Cover { get; set; }

        public string Spec { get; set; }

        public string Describe { get; set; }

        public string Product01 { get; set; }

        public string Product02 { get; set; }

        public string Product03 { get; set; }

        public string Product04 { get; set; }

        public string Product05 { get; set; }

        public string Product06 { get; set; }

        public decimal Price { get; set; }

        public string Marketing_Title { get; set; }

        public string Marketing_Starts_At { get; set; }

        public string Marketing_Ends_At { get; set; }

        public string Created_At { get; set; }

        public string Updated_At { get; set; }

        /// <summary>
        /// 產品介紹
        /// </summary>
        public SpuDetail Detail { get; set; }
        /// <summary>
        /// 產品分類
        /// </summary>
        public SpuCategory ProductCategory { get; set; }
        /// <summary>
        /// 運費資料
        /// </summary>
        public List<SpuLogistics> Logistics { get; set; }
        /// <summary>
        /// 產品規格
        /// </summary>
        public List<Sku> SellInfos { get; set; }
        /// <summary>
        /// 產品介紹圖片
        /// </summary>
        public List<SpuIntroductionImage> IntroductionImage { get; set; }

        /// <summary>
        /// 產品庫存量(所有規格庫存量加總)
        /// 取得產品列表時提供
        /// </summary>
        public int stock_qty { get; set; }

        /// <summary>
        /// 產品銷售量(所有規格銷售量加總)
        /// 取得產品列表時提供
        /// </summary>
        public int sell_qty { get; set; }

    }

}