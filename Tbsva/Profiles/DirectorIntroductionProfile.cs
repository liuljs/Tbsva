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
    public class DirectorIntroductionProfile : Profile
    {
        public DirectorIntroductionProfile()
        {
            CreateMap<DirectorIntroduction, DirectorIntroductionDto>()
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.imageName))
                .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
                .ForMember(target => target.startDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.startDate)))
                .ForMember(target => target.endDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.endDate)))
                .ReverseMap();

            CreateMap<DirectorIntroductionImage, DirectorIntroductionImageDto>()
                .ForMember(target => target.imageId, option => option.MapFrom(source => source.directorIntroductionImageId))
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.imageName))
                .ReverseMap();
        }
    }
}