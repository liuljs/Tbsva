using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using WebShopping.Models;
using WebShopping.Dtos;
using WebShopping.Helpers;

namespace WebShopping.Profiles
{
    public class KnowledgeContentListProfile : Profile
    {
        public KnowledgeContentListProfile()
        {
            CreateMap<Knowledge_content, Knowledge_content_Dto>()
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.imageName))
                .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
                .ReverseMap();

            CreateMap<Knowledge_image, Knowledge_image_Dto>()
                .ForMember(target => target.imageId, option => option.MapFrom(source => source.knowledgeImageId))
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.imageName))
                .ReverseMap();
        }
    }
}