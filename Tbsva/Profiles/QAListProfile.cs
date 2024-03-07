using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Dtos;
using WebShopping.Models;

namespace WebShopping.Profiles
{
    public class QAListProfile : Profile
    {
        public QAListProfile()
        {
            CreateMap<QA, QA_Dto>()
            .ForMember(target => target.Creation_Date, option => option.MapFrom(source => source.creation_date.ToString("yyyy-MM-dd HH:mm")))
            .ReverseMap();
        }
    }
}