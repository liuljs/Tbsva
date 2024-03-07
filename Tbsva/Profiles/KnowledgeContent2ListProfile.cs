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
    public class KnowledgeContent2ListProfile : Profile
    {
        public KnowledgeContent2ListProfile()
        {
            CreateMap<Knowledge_content2, Knowledge_content2_Dto>()
                .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))    //這裏若有?代表會有空值所以會錯 public DateTime? Creation_Date { get; set; }
                .ReverseMap();         
        }
    }
}