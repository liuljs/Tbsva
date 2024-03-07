using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Dtos;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Profiles
{
    public class ActivityCategoryProfile : Profile
    {
        public ActivityCategoryProfile()
        {
            CreateMap<ActivityCategory, ActivityCategoryDto>()
            .ForMember(target => target.categoryId, option => option.MapFrom(source => source.category_id))
            .ForMember(target => target.createDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creation_date)))
            .ReverseMap();
        }
    }
}