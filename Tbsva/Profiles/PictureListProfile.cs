using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Dtos;
using WebShopping.Models;

namespace WebShopping.Profiles
{
    public class PictureListProfile : Profile
    {
        public PictureListProfile()
        {
            CreateMap<PictureList, PictureListDto>()            
            .ReverseMap();
        }
    }
}