using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Dtos;
using WebShopping.Models;

namespace WebShopping.Profiles
{
    public class VideoCategoryProfile : Profile
    {
        public VideoCategoryProfile()
        {
            CreateMap<VideoCategory, VideoCategoryDto>().ReverseMap();
        }
    }
}