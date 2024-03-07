using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using WebShopping.Models;
using WebShopping.Dtos;

namespace WebShopping.Profiles
{
    public class LightingCategoryListProfile : Profile
    {
        public LightingCategoryListProfile()
        {
            CreateMap<Lighting_category, Lighting_category_Dto>().ReverseMap();
        }
    }
}