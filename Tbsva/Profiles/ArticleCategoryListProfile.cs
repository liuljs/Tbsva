using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Dtos;
using WebShopping.Models;

namespace WebShopping.Profiles
{
    public class ArticleCategoryListProfile : Profile
    {
        public ArticleCategoryListProfile()
        {
            CreateMap<article_category, article_category_Dto>()
                .ForMember(target => target.id, option => option.MapFrom(source => source.id))
                .ForMember(target => target.name, option => option.MapFrom(source => source.name))
                .ForMember(target => target.content, option => option.MapFrom(source => source.content))
                .ForMember(target => target.enabled, option => option.MapFrom(source => source.Enabled))
                .ForMember(target => target.sort, option => option.MapFrom(source => source.Sort))
                .ReverseMap();
        }
    }
}