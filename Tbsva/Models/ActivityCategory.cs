using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class ActivityCategory
    {
        /// <summary>
        /// 流水號，資料庫排序、索引用
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 目錄類別另設 ID 唯一索引用
        /// </summary>
        [Key]    // 主索引鍵（P.K.）
        public Guid category_id { get; set; }

        [Required]
        [StringLength(20)]
        public string name { get; set; }

        [StringLength(40)]
        public string berif { get; set; }

        public bool enabled { get; set; }

        public int sort { get; set; }

        public DateTime creation_date { get; set; }

        public DateTime? updated_date { get; set; }

        //public virtual ICollection<Activity> activity { get; set; }
    }
}