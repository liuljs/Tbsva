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
    public class TaiwanDojoProfile : Profile
    {
        public TaiwanDojoProfile()
        {
            CreateMap<TaiwanDojo, TaiwanDojoDto>()
                .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
                //.ForMember(target => target.updatedDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.updatedDate)))
                .ReverseMap();

            CreateMap<TaiwanDojoImage, TaiwanDojoImageDto>()
                .ForMember(target => target.imageId, option => option.MapFrom(source => source.taiwanDojoImageId))
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.imageName))
                .ReverseMap();
        }
    }
}