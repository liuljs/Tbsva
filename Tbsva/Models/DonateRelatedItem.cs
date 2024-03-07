using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DonateRelatedItem
    {
        /// <summary>
        /// 訂購明細項目流水編號
        /// </summary>         
        public int Id { get; set; }

        /// <summary>
        /// 訂購明細項目的ID
        /// </summary>
        public Guid donateRelatedItemId { get; set; }

        /// <summary>
        /// 主類別
        /// </summary>
        [StringLength(1)]
        public string primary { get; set; }

        /// <summary>
        /// 次類別
        /// </summary>
        [StringLength(1)]
        public string secondary { get; set; }

        /// <summary>
        /// 品名標題
        /// </summary>
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(50)]
        public string ImageName { get; set; }

        /// <summary>
        /// 內容1
        /// </summary>
        [StringLength(100)]
        public string Content1 { get; set; }

        /// <summary>
        /// 內容2
        /// </summary>
        [Column(TypeName = "ntext")]
        public string Content2 { get; set; }

        /// <summary>
        /// 內容3
        /// </summary>
        [Column(TypeName = "ntext")]
        public string content3 { get; set; }

        /// <summary>
        /// 項目金額
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 注意事項
        /// </summary>
        [StringLength(100)]
        public string Notes { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [RegularExpression(@"^([0-9]{6})$")]  //只能輸入0~9共6位數字 ^在該行的開頭開始比對 $在行尾結束比對
        public int Sort { get; set; }

        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool first { get; set; }

        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool enabled { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? updatedDate { get; set; }
    }
}