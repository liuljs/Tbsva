using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class GraphicsEditor
    {
        /// <summary>
        /// 流水號編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 內容類別另設 ID 唯一索引用
        /// </summary>
        [Key]
        public Guid graphicsEditorId { get; set; }

        [StringLength(40)]
        public string title { get; set; }

        [StringLength(40)]
        public string subTitle { get; set; }

        [Column(TypeName = "ntext")]
        public string other { get; set; }

        [Column(TypeName = "ntext")]
        public string content1 { get; set; }

        [Column(TypeName = "ntext")]
        public string content2 { get; set; }

        [Column(TypeName = "ntext")]
        public string content3 { get; set; }

        [Column(TypeName = "ntext")]
        public string content4 { get; set; }

        [Column(TypeName = "ntext")]
        public string content5 { get; set; }

        [Column(TypeName = "ntext")]
        public string content6 { get; set; }

        [Column(TypeName = "ntext")]
        public string content7 { get; set; }

        [Column(TypeName = "ntext")]
        public string content8 { get; set; }

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

        public DateTime creationDate { get; set; }

        public DateTime updateDate { get; set; }

    }
}