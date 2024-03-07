using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class article_category
    {
        public int id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public string content { get; set; }

        public DateTime creation_date { get; set; }

        public DateTime? updated_date { get; set; }

        /// <summary>
        /// 狀態(上下架)
        /// </summary>
        public bool Enabled { get; set; }

        public int Sort { get; set; }

        //public virtual ICollection<article_content> article_content { get; set; }
    }
}