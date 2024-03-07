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
    public class TimeMachineProfile : Profile
    {
        public TimeMachineProfile()
        {
            CreateMap<TimeMachine, TimeMachineDto>()
                .ForMember(target => target.course, option => option.MapFrom(source => source.course.ToString("yyyy/MM/dd")))
                .ForMember(target => target.imageURL01, option => option.MapFrom(source => source.navPics01))
                .ForMember(target => target.imageURL02, option => option.MapFrom(source => source.navPics02))
                .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
                .ForMember(target => target.startDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.startDate)))
                .ForMember(target => target.endDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.endDate)))
                .ReverseMap();
        }
    }
}