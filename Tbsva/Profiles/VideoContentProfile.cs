using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Dtos;
using WebShopping.Models;

namespace WebShopping.Profiles
{
    public class VideoContentProfile : Profile
    {
        public VideoContentProfile()
        {
            CreateMap<VideoContent, VideoContentDto>()
                .ForMember(target => target.Creation_Date, option => option.MapFrom(source => source.creation_date.ToString("yyyy-MM-dd")))
                .ForMember(target => target.Image_Url, option => option.MapFrom(source => source.image_name))
                //存入的資料是前端前來已編碼HtmlEncode存入資料庫，輸出的資料HtmlDecode解碼成原本的字串
                .ForMember(target => target.Video_url, option => option.MapFrom(source => HttpUtility.HtmlDecode(source.Video_url)))
                .ReverseMap();
        }
    }
}