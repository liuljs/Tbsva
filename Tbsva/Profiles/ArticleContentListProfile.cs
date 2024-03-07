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
    public class ArticleContentListProfile : Profile
    {
        public ArticleContentListProfile()
        {
            CreateMap<article_content, article_content_Dto>()
            .ForMember(target => target.id, option => option.MapFrom(source => source.id))
            .ForMember(target => target.articleCategoryId, option => option.MapFrom(source => source.article_category_id))
            .ForMember(target => target.title, option => option.MapFrom(source => source.title))
            .ForMember(target => target.subtitle, option => option.MapFrom(source => source.subtitle))
            .ForMember(target => target.imageURL, option => option.MapFrom(source => source.image_name))
            .ForMember(target => target.brief, option => option.MapFrom(source => source.brief))
            .ForMember(target => target.content, option => option.MapFrom(source => source.content))
            .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creation_date)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
            .ForMember(target => target.enabled, option => option.MapFrom(source => source.Enabled))
            .ForMember(target => target.sort, option => option.MapFrom(source => source.Sort))
            .ForMember(target => target.articleCategoryName, option => option.MapFrom(source => source.Article_Category_Name))
            .ReverseMap();

            CreateMap<article_image, article_image_Dto>()
            .ForMember(target => target.imageId, option => option.MapFrom(source => source.article_image_id))
            .ForMember(target => target.imageURL, option => option.MapFrom(source => source.image_name))
            .ReverseMap();
        }
    }
}