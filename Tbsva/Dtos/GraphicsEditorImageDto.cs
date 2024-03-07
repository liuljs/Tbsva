using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 先作廢
    /// </summary>
    public class GraphicsEditorImageDto
    {
        public int id { get; set; }
        public Guid imageId { get; set; }
        public string imageURL { get; set; }
    }
}