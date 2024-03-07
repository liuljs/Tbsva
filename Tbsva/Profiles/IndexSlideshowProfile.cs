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
    public class IndexSlideshowProfile : Profile
    {
        public IndexSlideshowProfile()
        {
            CreateMap<IndexSlideshow, IndexSlideshowDto>()
                .ForMember(target => target.imageURL01, option => option.MapFrom(source => source.fullImage))
                .ForMember(target => target.imageURL02, option => option.MapFrom(source => source.tabletImage))
                .ForMember(target => target.imageURL03, option => option.MapFrom(source => source.smPhoneImage))
                .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))
                .ReverseMap();
        }
    }
}