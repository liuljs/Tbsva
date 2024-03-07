using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class CategoryGetDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public OpenStatus Enable { get; set; }

        public List<SubCategory1Dto> SubCategories { get; set; } = new List<SubCategory1Dto>();
    }

    public class SubCategory1Dto
    {
        public int Id { get; set; }

        public int Parent_id { get; set; }

        public string Name { get; set; }

        public OpenStatus Enable { get; set; }

        public List<SubCategory2Dto> SubCategories { get; set; } = new List<SubCategory2Dto>();
    }

    public class SubCategory2Dto
    {
        public int Id { get; set; }

        public int Parent_id { get; set; }

        public string Name { get; set; }

        public OpenStatus Enable { get; set; }       
    }
}