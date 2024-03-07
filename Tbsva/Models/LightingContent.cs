using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Lighting_content
    {
        //public Lighting_content()
        //{
        //    Lighting_image = new HashSet<Lighting_image>();
        //}
        public Guid id { get; set; }
        public int Lighting_category_id { get; set; }
        [Required]
        [StringLength(50)]
        public string image_name { get; set; }
        [Required]
        [StringLength(255)]
        public string title { get; set; }
        [StringLength(800)]
        public string brief { get; set; }
        public string content { get; set; }
        [StringLength(2000)]
        public string more_pic_url { get; set; }
        public DateTime creation_date { get; set; }
        public DateTime? updated_date { get; set; }
        public bool Enabled { get; set; }
        public int Sort { get; set; }

        [Required]
        [StringLength(1)]
        [RegularExpression(@"[Y]|[N]")]
        public string first { get; set; }
        /// <summary>
        /// 點燈目錄
        /// </summary>
        public string Lighting_Category_Name { get; set; }

        //public virtual Lighting_category Lighting_category { get; set; }
        //public virtual ICollection<Lighting_image> Lighting_image { get; set; }
    }
}