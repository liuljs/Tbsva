using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Lighting_category
    {
        // public Lighting_category()
        //{
        //    Lighting_content = new HashSet<Lighting_content>();
        //}

        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        public string content { get; set; }

        public DateTime creation_date { get; set; }

        public DateTime? updated_date { get; set; }

        public bool Enabled { get; set; }

        public int Sort { get; set; }

        //public virtual ICollection<Lighting_content> Lighting_content { get; set; }
    }
}