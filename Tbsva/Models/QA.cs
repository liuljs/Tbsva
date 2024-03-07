using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class QA
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string Question { get; set; }

        [Column(TypeName = "ntext")]
        public string Answer { get; set; }

        public DateTime creation_date { get; set; }

        public DateTime? updated_date { get; set; }

        public bool Enabled { get; set; }

        public int Sort { get; set; }


    }
}