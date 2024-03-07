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
    public class ConvertFormsProfile : Profile
    {
        public ConvertFormsProfile()
        {
            CreateMap<ConvertForms, ConvertFormsDto>()
                .ForMember(target => target.birthz, option => option.MapFrom(source => source.birthz.ToString("yyyy-MM-dd")))
                .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
                //.ForMember(target => target.creationDate, option => option.MapFrom(source => source.creationDate.ToString("yyyy/MM/dd HH:mm")))
                .ReverseMap();  
        }
    }
}