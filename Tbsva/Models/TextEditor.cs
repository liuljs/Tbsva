using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class TextEditor
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 內容類別另設 ID 唯一索引用
        /// </summary>
        [Key]
        public Guid textEditorId { get; set; }

        [StringLength(40)]
        public string title { get; set; }

        [StringLength(40)]
        public string subTitle { get; set; }

        [Column(TypeName = "ntext")]
        public string other { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string content { get; set; }
        public DateTime creationDate { get; set; }
    }
}