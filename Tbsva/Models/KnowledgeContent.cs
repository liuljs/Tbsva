using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Knowledge_content
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Guid 唯一索引用
        /// </summary>
        [Key]
        public Guid knowledgetId { get; set; }

        [Required]
        [StringLength(50)]
        public string imageName { get; set; }

        [Required]
        [StringLength(40)]
        public string title { get; set; }

        [Required]
        [StringLength(40)]
        public string category { get; set; }

        [StringLength(40)]
        public string number { get; set; }

        [StringLength(100)]
        public string brief { get; set; }
        public string content { get; set; }

        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool first { get; set; }

        [RegularExpression(@"^([0-9]{6})$")]  //只能輸入0~9共6位數字 ^在該行的開頭開始比對 $在行尾結束比對
        public int sort { get; set; }

        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool enabled { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime? updatedDate { get; set; }

        //public virtual ICollection<Knowledge_image> Knowledge_image { get; set; }
    }
}