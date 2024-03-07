using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class ConvertForms
    {
        /// <summary>
        /// 0.C0000001 C加流水號編7位
        /// </summary>
        public string cId
        {
            get
            {
                return @"C" + $"{id.ToString().PadLeft(7, '0')}";
                //return "C" + id.ToString().PadLeft(8, '0');
            }
        }
        /// <summary>
        /// 1.流水號編號索引
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 2.另設 ID 唯一索引用
        /// </summary>
        [Key]    // 主索引鍵（P.K.）
        public Guid convertFormsId { get; set; }

        /// <summary>
        /// 3.中文姓名
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
        /// 5.英文姓名
        /// </summary>
        [StringLength(50)]
        public string enNamez { get; set; }

        /// <summary>
        /// 6.皈依者 1本人2家人3祖先4亡者5纏身靈6怨親債主7水子靈8土地公9地基主10地靈11地神12寵物
        /// </summary>
        [RegularExpression(@"^([1-12]{1})$")] //只能輸入1~12共1位數
        public byte convertz { get; set; }

        /// <summary>
        /// 7.出生日期
        /// </summary>
        public DateTime birthz { get; set; }

        /// <summary>
        /// 8.隸屬地區 0無1北部2中部3南部4海外
        /// </summary>
        [RegularExpression(@"^([0-4]{1})$")] //只能輸入1~12共1位數
        public byte affiliatedAreaz { get; set; }

        /// <summary>
        /// 9.戶籍地址
        /// </summary>
        [Required]
        [StringLength(250)]
        public string residenceAddress { get; set; }

        /// <summary>
        /// 10.聯絡地址
        /// </summary>
        [Required]
        [StringLength(250)]
        public string contactAddress { get; set; }

        /// <summary>
        /// 11.聯絡電話
        /// </summary>
        [Required]
        [StringLength(20)]
        public string contactNumber { get; set; }

        /// <summary>
        /// 12.手機電話
        /// </summary>
        [StringLength(20)]
        public string moblieNumber { get; set; }

        /// <summary>
        /// 13.電子信箱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string email { get; set; }

        /// <summary>
        /// 14.是否寄發證書必填1是0否
        /// </summary>
        [RegularExpression("[0]|[1]")]
        public bool sendCertificate { get; set; }

        /// <summary>
        /// 15.寄件地址
        /// </summary>
        [StringLength(250)]
        public string sendAddress { get; set; }

        /// <summary>
        /// 16.備註內容
        /// </summary>
        [StringLength(255)]
        public string remarks { get; set; }

        /// <summary>
        /// 17.狀態(1顯示，0不顯示)
        /// </summary>
        [StringLength(1)]
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool enabled { get; set; }

        /// <summary>
        /// 18.審核(0未通過, 1通過)
        /// </summary>
        [StringLength(1)]
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool audit { get; set; }

        /// <summary>
        /// 19.新增時間
        /// </summary>
        public DateTime creationDate { get; set; }

        /// <summary>
        /// 20.修改時間
        /// </summary>
        public DateTime? updatedDate { get; set; }
    }
}