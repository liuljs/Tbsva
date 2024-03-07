using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class AddTbsva
    {
        /// <summary>
        /// 0.T0000001T加流水號編
        /// </summary>
        public string tId { 
            get {
                    return @"T" + $"{id.ToString().PadLeft(7, '0')}";
                    //return "T" + id.ToString().PadLeft(8, '0');
            }
        }
        /// <summary>
        /// 1.流水號編號索引
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        /// <summary>
        /// 2.主索引鍵（P.K.）
        /// </summary>
        [Key]
        public Guid addtbsvaId { get; set; }

        /// <summary>
        /// 3.會員姓名
        /// </summary>
        [Required]
        [StringLength(50)]
        public string namez { get; set; }

        /// <summary>
        /// 4.性別
        /// </summary>
        [RegularExpression(@"^([0-2]{1})$")] //只能輸入012共1位數
        public byte gender { get; set; }

        /// <summary>
        /// 5.出生日期
        /// </summary>
        [Column(TypeName = "date")]
        [Display(Name = "生日（birthz）")]  //屬性指定要顯示的欄位名稱
        [DataType(DataType.Date)]    // 只有日期 - 「年月日」。如果是 DateTime就是「日期與時間」屬性指定資料的類型 (Date)，因此不會顯示儲存在欄位中的時間資訊
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]   // 定義顯示格式 設定日期為 yyyy/MM/dd 格式
        public DateTime birthz { get; set; }

        /// <summary>
        /// 6.隸屬地區 0無1北部2中部3南部4海外
        /// </summary>
       [RegularExpression(@"^([0-4]{1})$")] //只能輸入1~12共1位數
        public byte affiliatedAreaz { get; set; }

        /// <summary>
        /// 7.聯絡地址
        /// </summary>
        [Required]
        [StringLength(250)]
        public string contactAddress { get; set; }

        /// <summary>
        /// 8.聯絡電話
        /// </summary>
        [Required]
        [StringLength(20)]
        public string contactNumber { get; set; }

        /// <summary>
        /// 9.手機電話
        /// </summary>
        [StringLength(20, ErrorMessage = "*** 自訂訊息 *** 不得超過15個數字 ***")]
        [Display(Name = "手機號碼（UserMobilePhone）")]
        [RegularExpression(@"^(09)([0-9]{8})$")]   // 前兩個數字必須是09，後面跟著八個數字（必須是0~9的數字）。
        // 正規運算、正規表達（regular expression）。  ^符號 代表開始，$符號 代表結束。
        public string moblieNumber { get; set; }

        /// <summary>
        /// 10.電子信箱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string email { get; set; }

        /// <summary>
        /// 11.自我介紹（顯示於後台）前後台都有輸入欄
        /// </summary>
        [Column(TypeName = "ntext")]
        public string contentz { get; set; }

        /// <summary>
        /// 12.備註內容（顯示於前台）
        /// </summary>
        [Column(TypeName = "ntext")]
        public string notez { get; set; }

        /// <summary>
        /// 13.狀態(1顯示，0不顯示)
        /// </summary>
        [StringLength(1)]
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool enabled { get; set; }

        /// <summary>
        /// 14.審核(0未通過, 1通過)
        /// </summary>
        [StringLength(1)]
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool audit { get; set; }

        public DateTime creationDate { get; set; }

        public DateTime? updatedDate { get; set; }
    }
}