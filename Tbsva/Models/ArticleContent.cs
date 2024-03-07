using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebShopping.Dtos;

namespace WebShopping.Models
{
    public class article_content
    {
        public Guid id { get; set; }

        public int article_category_id { get; set; }

        [StringLength(255)]
        public string title { get; set; }

        [StringLength(40)]
        public string subtitle { get; set; }

        [StringLength(50)]
        public string image_name { get; set; }

        [StringLength(800)]
        public string brief { get; set; }

        public string content { get; set; }

        public DateTime creation_date { get; set; }

        public DateTime? updated_date { get; set; }

        public bool Enabled { get; set; }

        public int Sort { get; set; }

        //[Required]
        //[StringLength(1)]
        //[RegularExpression(@"[Y]|[N]")]
        //public string first { get; set; }
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool first { get; set; }

        public string Article_Category_Name { get; set; }

        //public DefaultValue DefaultValue { get; set; } = new DefaultValue();//雖然開了另一個類別,但若沒有new他,則無法用article_content類別來直接.DefaultValue類別下的屬性
        //_article_content.Sort = _article_content.DefaultValue.Sort_Default;

        //public virtual article_category article_category { get; set; }
        //public virtual ICollection<article_image> article_image { get; set; }

        /// <summary>
        /// 文章分類
        /// </summary>
       // public article_category Article_Category_Collection { get; set; }
    }

}