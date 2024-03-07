using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class PaymentMailing
    {
        public Guid id { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string content { get; set; }

        public DateTime creation_date { get; set; }
    }
}