using AutoMapper;
using WebShopping.Dtos;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;

namespace WebShopping.Profiles
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<News, NewsDto>()
                .ForMember(target => target.newsId, option => option.MapFrom(source => source.news_id))
                .ForMember(target => target.cover, option => option.MapFrom(source => source.image_name))
                .ForMember(target => target.createDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creation_date)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
                .ForMember(target => target.startDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.start_date)))
                .ForMember(target => target.endDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.end_date)))
                .ForMember(target => target.imageURL01, option => option.MapFrom(source => source.navPics01))
                .ForMember(target => target.imageURL02, option => option.MapFrom(source => source.navPics02))
                .ForMember(target => target.imageURL03, option => option.MapFrom(source => source.navPics03))
                .ForMember(target => target.imageURL04, option => option.MapFrom(source => source.navPics04))
                .ForMember(target => target.imageURL05, option => option.MapFrom(source => source.navPics05))
                .ForMember(target => target.imageURL06, option => option.MapFrom(source => source.navPics06))
                .ForMember(target => target.imageURL07, option => option.MapFrom(source => source.navPics07))
                .ForMember(target => target.imageURL08, option => option.MapFrom(source => source.navPics08))
                .ReverseMap();

            CreateMap<NewsImage, NewsImageDto>()
                .ForMember(target => target.imageId, option => option.MapFrom(source => source.news_image_id))
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.image_name))
                .ReverseMap();
        }
    }
}