using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class DirectorIntroductionImageDto
    {
        public int id { get; set; }
        /// <summary>
        /// public Guid directorIntroductionImageId
        /// </summary>
        public Guid imageId { get; set; }
        public string imageURL { get; set; }
    }
}