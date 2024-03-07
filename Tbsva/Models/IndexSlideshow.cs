using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class IndexSlideshow
    {
        /// <summary>
        /// 自動流水號編號
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        /// <summary>
        /// 輪播編號 另設 ID 唯一索引用
        /// </summary>
        [Key]  // 主索引鍵（P.K.）
        public Guid slideId { get; set; }

        /// <summary>
        /// 輪播圖片名稱
        /// </summary>
        [StringLength(50)]
        public string name { get; set; }

        /// <summary>
        /// 滿版圖片檔案imageURL01
        /// </summary>
        [Required]
        [StringLength(50)]
        public string  fullImage { get; set; }

        /// <summary>
        /// 平板圖片檔案
        /// </summary>
        [StringLength(50)]
        public string tabletImage { get; set; }

        /// <summary>
        /// 手機圖片檔案
        /// </summary>
        [StringLength(50)]
        public string smPhoneImage { get; set; }

        /// <summary>
        /// 連結網址
        /// </summary>
        [StringLength(255)]
        public string hyperlink { get; set; }

        /// <summary>
        /// 置頂狀態（0 關閉；1 開啟）
        /// </summary>
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool first { get; set; }

        /// <summary>
        /// 排序，預設為流水號編號
        /// </summary>
        [RegularExpression(@"^([0-9]{6})$")]  //只能輸入0~9共6位數字 ^在該行的開頭開始比對 $在行尾結束比對
        public int sort { get; set; }

        /// <summary>
        /// 啟用狀態（0 關閉；1 開啟）
        /// </summary>
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool enabled { get; set; }


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "yyyy-MM-dd HH:mm")]
        [MyValidateDateRangeAttribute1(MyStartDate = "1950-1-1", MyEndDate = "2050-12-31", ErrorMessage = "日期區間，只能在1950年以後~2050年之前")]
        public DateTime creationDate { get; set; }

        public DateTime? updatedDate { get; set; }

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