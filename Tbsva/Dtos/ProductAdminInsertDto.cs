using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 基本資料
    /// </summary>
    public class ProductAdminDto
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }
        [Required]
        public byte Enabled { get; set; }

        [Required]
        [RegularExpression("[0]|[1]")]
        public int Recommend { get; set; }

        [Required]
        [RegularExpression("[0]|[1]")]
        public int SellStop { get; set; }

        [Required]
        [RegularExpression("[0]|[1]")]
        public int ViewStock { get; set; }

        [Required]
        [RegularExpression("[0]|[1]")]
        public int ViewSellNum { get; set; }

        public int LeadTime { get; set; }

        [Required]
        [RegularExpression("[0]|[1]")]
        public int PreserveStatus { get; set; }

        [Required]
        [RegularExpression("[0]|[1]")]
        public int ProductStatus { get; set; }

        public string StartsAt { get; set; }

        public string EndsAt { get; set; }

        public string Spec { get; set; }

        public string Describe { get; set; }

        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// 行銷標語
        /// </summary>
        public string Marketing_Title { get; set; }

        /// <summary>
        /// 行銷上架日期
        /// </summary>
        public string Marketing_Starts_At { get; set; }

        /// <summary>
        /// 行銷下架日期
        /// </summary>
        public string Marketing_Ends_At { get; set; }

        //public List<MarketingInsertDto> MarketingInserts { get; set; } = new List<MarketingInsertDto>();
        /// <summary>
        /// 運費資料
        /// </summary>
        public List<LogisticsInsertDto> Logistics { get; set; } = new List<LogisticsInsertDto>();
        /// <summary>
        /// 規格表
        /// </summary>
        public List<SellInfoInsertDto> SellInfos { get; set; } = new List<SellInfoInsertDto>();
        /// <summary>
        /// 產品介紹
        /// </summary>
        public DetailInsertDto Detail { get; set; }
        /// <summary>
        /// 產品分類
        /// </summary>
        public ProductCategoryInsertDto ProductCategory { get; set; }
    }

    /// <summary>
    /// 產品分類
    /// </summary>
    public class ProductCategoryInsertDto
    {
        public int Cid1 { get; set; }

        public int Cid2 { get; set; }

        public int Cid3 { get; set; }
    }

    /// <summary>
    /// 運費資訊
    /// </summary>
    public class LogisticsInsertDto
    {
        public string Code { get; set; }

        public decimal ShippingFee { get; set; }

        public byte Enabled { get; set; }
    }

    /// <summary>
    /// 規格表
    /// </summary>
    public class SellInfoInsertDto
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        //[Required]
        //public decimal Price { get; set; }

        [Required]
        public decimal SellPrice { get; set; }

        [Required]
        public byte Enabled { get; set; }
        
        [Required]
        public int StockQty { get; set; }

       [Required]
        public int SafetyStockQty { get; set; }

        [Required]
        public int DiscountPercent { get; set; }

        [Required]
        public decimal DiscountPrice { get; set; }
    }

    //行銷資訊
    //public class MarketingInsertDto
    //{
    //    public decimal SellPrice { get; set; }
       
    //    public decimal DiscountPercent { get; set; }
    //}

    /// <summary>
    /// 產品介紹
    /// </summary>
    public class DetailInsertDto
    {
        public int Id { get; set; }

        public int Spu_Id { get; set; }

        public string Title1 { get; set; }

        public string Title2 { get; set; }

        public string Introduction1 { get; set; }

        public string Introduction2 { get; set; }
    }

    public class SendProductAddImageDto
    {
        /// <summary>
        /// 圖片路徑連結位置
        /// </summary>
        public string Image_Link { get; set; }
    }

}