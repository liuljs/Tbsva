using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Dtos;
using WebShopping.Helpers;
using WebShopping.Models;
using WebShopping.Services;

namespace WebShopping.Profiles
{
    public class VideoProfile : Profile
    {
        #region DI依賴注入功能
        //private IVideoService VideoService;  //無法使用
        //public VideoProfile(IVideoService videoService)
        //{
        //    VideoService = videoService;
        //}
        #endregion

        public VideoProfile()
        {
            CreateMap<Video, VideoDto>()
                .ForMember(target => target.videoId, option => option.MapFrom(source => source.video_id))
                .ForMember(target => target.videoCategory, option => option.MapFrom(source => source.video_category))
                //.ForMember(target => target.imageURL, option => option.MapFrom(source => VideoService.CustomizeToYouTubeCover(source)))
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.cover))
                .ForMember(target => target.createDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creation_date)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
                .ForMember(target => target.startDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.start_date)))
                .ForMember(target => target.endDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.end_date)))
                .ReverseMap();

            CreateMap<VideoImage, VideoImageDto>()
                .ForMember(target => target.imageId, option => option.MapFrom(source => source.video_image_id))
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.image_name))
                .ReverseMap();
        }
    }
}