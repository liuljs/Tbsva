using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Dtos;
using WebShopping.Models;

namespace WebShopping.Profiles
{
    public class OtherAccessoriesProfile : Profile
    {
        public OtherAccessoriesProfile()
        {
            CreateMap<OtherAccessories, OtherAccessoriesDto>()
            .ForMember(target => target.Creation_Date, option => option.MapFrom(source => source.creation_date.ToString("yyyy/MM/dd HH:mm")))
            .ReverseMap();
        }
    }
}