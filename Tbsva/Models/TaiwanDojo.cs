using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class TaiwanDojo
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 隸屬目錄 ID
        /// </summary>
        public Guid categoryId { get; set; }

        /// <summary>
        /// Guid 唯一索引用
        /// </summary>
        [Key]
        public Guid taiwanDojoId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Required]
        [StringLength(40)]
        public string name { get; set; }

        /// <summary>
        /// 經度
        /// </summary>
        [Required]
        [StringLength(100)]
        public string latitude { get; set; }

        /// <summary>
        /// 緯度
        /// </summary>
        [Required]
        [StringLength(100)]
        public string longitude { get; set; }

        /// <summary>
        /// 簡述
        /// </summary>
        [StringLength(100)]
        public string brief { get; set; }

        /// <summary>
        /// 地區
        /// </summary>
        [StringLength(100)]
        public string area { get; set; }

        [StringLength(100)]
        public string address { get; set; }

        /// <summary>
        /// 地圖
        /// </summary>
        public string video { get; set; }

        public string content { get; set; }

        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool icon { get; set; }

        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool first { get; set; }

        [RegularExpression(@"^([0-9]{6})$")]  //只能輸入0~9共6位數字 ^在該行的開頭開始比對 $在行尾結束比對
        public int sort { get; set; }

        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool enabled { get; set; }

        public DateTime creationDate { get; set; }

        public DateTime? updatedDate { get; set; }

        public string categoryName { get; set; }
    }
}