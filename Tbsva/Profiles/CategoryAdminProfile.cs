using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using WebShopping.Dtos;
using WebShopping.Models;

namespace WebShopping.Profiles
{
    public class CategoryAdminProfile : Profile
    {
        public CategoryAdminProfile()
        {
            CreateMap<SubCategory2, SubCategory2Dto>().ReverseMap();
            CreateMap<SubCategory1, SubCategory1Dto>().ReverseMap();
            CreateMap<Category, CategoryGetDto>().ReverseMap();
            CreateMap<EditCategoryDto, Category>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name.Trim()))
                .ForMember(x => x.SubCategories, y => y.Ignore())                
                .ReverseMap();
            CreateMap<EditSubCategoryDto, SubCategory1>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name.Trim()))
                .ReverseMap();
            CreateMap<EditSubCategoryDto, SubCategory2>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name.Trim()))
                .ReverseMap();
            CreateMap<EditFirstCategoryDto, SubCategory1>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name.Trim()))
                .ForMember(x => x.SubCategories, y => y.Ignore())
                .ReverseMap();
        }
    }
}