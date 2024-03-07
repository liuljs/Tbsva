﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    //[MetadataType(typeof(ActivityItemAttributes))]       // [MetadataType(typeof(對應的 類別名稱))]
    //可將驗證抽離出來放在ActivityItemAttributes，Activity就可只放欄位屬性就好
    public class Activity
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 活動編號 活動類別另設 ID 唯一索引用
        /// </summary>
        [Key]    // 主索引鍵（P.K.）
        public Guid activity_id { get; set; }

        /// <summary>
        /// 活動隸屬目錄 ID
        /// </summary>
        public Guid activity_category_id { get; set; }

        /// <summary>
        /// 活動隸屬目錄目錄名稱
        /// </summary>
        public string categoryName { get; set; }

        /// <summary>
        /// 圖片名稱
        /// </summary>
        [Required]
        [StringLength(50)]
        public string image_name { get; set; }

        [Required]
        [StringLength(40)]
        public string name { get; set; }

        public string video { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "{0}，至少要有{2}個字（最長允許 {1} 個字）")]
        // {0} 為 [Display(Name=....)]     
        // {1} 為 [StringLength(100,
        // {2} 為 [StringLength(...MinimumLength = 1,  .... 以此類推。請看圖片
        [Display(Name = "活動簡述（brief，必填）")]
        public string brief { get; set; }
        //活動簡述（brief，必填），至少要有1個字（最長允許 100 個字）

        [Required]
        [DataType(DataType.MultilineText)]
        public string content { get; set; }

        public bool first { get; set; }

        [RegularExpression(@"^([0-9]{6})$")]  //只能輸入0~9共6位數字 ^在該行的開頭開始比對 $在行尾結束比對
        public int sort { get; set; }

        public bool enabled { get; set; }
 
        /// <summary>
        /// 設定DateTime時若沒有收到值Start_Date=0001/1/1 上午 12:00:00
        /// 設定DateTime? 時若沒有收到值Start_Date=null
        /// </summary>
        public DateTime? start_date { get; set; }
 
        public DateTime? end_date { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd HH:mm")]
        [MyValidateDateRangeAttribute1(MyStartDate = "1950-1-1", MyEndDate = "2050-12-31", ErrorMessage = "日期區間，只能在1950年以後~2050年之前")]
        public DateTime creation_date { get; set; }

        public DateTime? updated_date { get; set; }

        [StringLength(50)]
        public string navPics01 { get; set; }

        [StringLength(50)]
        public string navPics02 { get; set; }

        [StringLength(50)]
        public string navPics03 { get; set; }

        [StringLength(50)]
        public string navPics04 { get; set; }

        [StringLength(50)]
        public string navPics05 { get; set; }

        [StringLength(50)]
        public string navPics06 { get; set; }

        [StringLength(50)]
        public string navPics07 { get; set; }

        [StringLength(50)]
        public string navPics08 { get; set; }

        private class MyValidateDateRangeAttribute1 : ValidationAttribute
        {
            // ****** 請自己修改 **************************************** (start)
            // 日期區間，允許修改、輸入「起迄日」
            public string MyStartDate { get; set; }    // string 改成 DateTime會出錯
            public string MyEndDate { get; set; }
            // ****** 請自己修改 **************************************** (end)
            //protected override <=這裏只要打這樣後面就會自動出現ValidationResult IsValid(object value, ValidationContext validationContext)
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {   //        ********** 覆寫，請自己實作

                // https://msdn.microsoft.com/zh-tw/library/dd730022(v=vs.110).aspx
                // 參數 (1) value : System.Object  要驗證的值。 (2) validationContext : 驗證作業的相關內容資訊。
                // 傳回值 :  ValidationResult 類別的執行個體。

                // ****** 請自己修改 **************************************** (start)
                DateTime dt = (DateTime)value;
                // 日期區間（起迄日）
                if (value != null && dt >= Convert.ToDateTime(MyStartDate) && dt <= Convert.ToDateTime(MyEndDate))
                {
                    return ValidationResult.Success;   // 驗證成功
                }
                else
                {   // 第一種作法，驗證失敗會出現這一句錯誤訊息。
                    //return new ValidationResult("[自訂驗證 的 錯誤訊息] 抱歉～日期區間，不符合或超出範圍");

                    //第二種作法，這裡使用空字串（""）。驗證失敗就會使用 UserTable.cs裡面的 [ ErrorMessage=""] 錯誤訊息
                    return new ValidationResult("");
                }
            }
        }

    }
}