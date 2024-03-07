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
    public class ClassificationModuleV2Profile : Profile
    {
        public ClassificationModuleV2Profile()
        {
            CreateMap<ClassificationModuleV2, ClassificationModuleV2Dto>()
                 .ForMember(target => target.categoryId, option => option.MapFrom(source => source.categoryId))
                 .ForMember(target => target.category, option => option.MapFrom(source => source.category))
                 .ForMember(target => target.name, option => option.MapFrom(source => source.name))
                .ReverseMap();

            CreateMap<ModuleV2, ModuleV2Dto>()
                 .ForMember(target => target.authId, option => option.MapFrom(source => source.authId))
                 .ForMember(target => target.name, option => option.MapFrom(source => source.name))
                 .ForMember(target => target.path, option => option.MapFrom(source => source.path))
                 .ForMember(target => target.belong, option => option.MapFrom(source => source.belong))
                 .ForMember(target => target.view, option => option.MapFrom(source => source.view))
                 .ForMember(target => target.edit, option => option.MapFrom(source => source.edit))
                 .ForMember(target => target.category, option => option.MapFrom(source => source.category))
                .ReverseMap();
        }
    }
}