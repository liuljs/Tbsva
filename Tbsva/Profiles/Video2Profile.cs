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
    public class Video2Profile : Profile
    {
        public Video2Profile()
        {
            CreateMap<Video2, Video2Dto>()
                .ForMember(target => target.video2Id, option => option.MapFrom(source => source.video2Id))
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.cover))
                //.ForMember(target => target.createDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
                .ForMember(target => target.createDate, option => option.MapFrom(source => source.creationDate.ToString("yyyy/MM/dd HH:mm")))
                .ReverseMap();

            CreateMap<VideoImage2, VideoImage2Dto>()
                .ForMember(target => target.imageId, option => option.MapFrom(source => source.videoImage2Id))
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.imageName))
                .ReverseMap();
        }
    }
}