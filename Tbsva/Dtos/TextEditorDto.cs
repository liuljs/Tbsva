using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class TextEditorDto
    {
        public int id { get; set; }
        public Guid textEditorId { get; set; }
        public string title { get; set; }
        public string subTitle { get; set; }
        public string other { get; set; }
        public string content { get; set; }
        public string creationDate { get; set; }
    }
}