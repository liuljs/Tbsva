using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DirectorIntroduction
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 時光走廊 Guid 唯一索引用
        /// </summary>
        [Key]    // 主索引鍵（P.K.）
        public Guid directorIntroductionId { get; set; }

        [StringLength(50)]
        public string imageName { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        [Required]
        [StringLength(40)]
        public string name { get; set; }

        /// <summary>
        /// 副標題
        /// </summary>
        [Required]
        [StringLength(40)]
        public string subtitle { get; set; }

        /// <summary>
        /// 簡述
        /// </summary>
        [Required]
        [StringLength(150)]
        public string brief { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string content { get; set; }

        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool first { get; set; }

        [RegularExpression(@"^([0-9]{6})$")]  //只能輸入0~9共6位數字 ^在該行的開頭開始比對 $在行尾結束比對
        public int sort { get; set; }

        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool enabled { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public DateTime creationDate { get; set; }

        public DateTime? updatedDate { get; set; }
    }
}