using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using WebShopping.Models;
using WebShopping.Dtos;

namespace WebShopping.Profiles
{
    public class LightingContentListProfile : Profile
    {
        public LightingContentListProfile()
        {
            CreateMap<Lighting_content, Lighting_content_Dto>()
                .ForMember(target => target.Creation_Date, option => option.MapFrom(source => source.creation_date.ToString("yyyy-MM-dd")))
                .ForMember(target => target.Image_Url, option => option.MapFrom(source => source.image_name))
                .ReverseMap();
        }
    }
}