using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class DonateRelatedItemDto
    {
        public int id { get; set; }
        public Guid donateRelatedItemId { get; set; }
        public string primary { get; set; }
        public string secondary { get; set; }
        public string title { get; set; }
        public string imageURL { get; set; }
        public string content1 { get; set; }
        public string content2 { get; set; }
        public string content3 { get; set; }
        public int amount { get; set; }
        public string notes { get; set; }
        public int sort { get; set; }
        public byte first { get; set; }
        public byte enabled { get; set; }
        public string creationDate { get; set; }
    }
}