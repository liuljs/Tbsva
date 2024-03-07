using AutoMapper;
using WebShopping.Dtos;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Profiles
{
    public class ProductAdminProfile : Profile
    {
        public ProductAdminProfile()
        {
            //CreateMap<Spu2, ProductAdminInsertDto2>().ReverseMap();
            //TODO:有缺的型別，還要再對應
            CreateMap<Spu, ProductAdminDto>().ReverseMap();

            CreateMap<Spu, SpuGetDto>()
                .ForMember(x => x.Created_At, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Created_At)))
                .ForMember(x => x.Updated_At, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Updated_At)))
                .ForMember(x => x.Starts_At, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Starts_At)))
                .ForMember(x => x.Ends_At, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Ends_At)))
                .ForMember(x => x.Marketing_Starts_At, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Marketing_Starts_At)))
                .ForMember(x => x.Marketing_Ends_At, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Marketing_Ends_At)))
                .ReverseMap();

            CreateMap<SpuDetail, DetailInsertDto>().ReverseMap();
            CreateMap<SpuCategory, ProductCategoryInsertDto>().ReverseMap();
            CreateMap<SpuLogistics, LogisticsInsertDto>().ReverseMap();
            CreateMap<Sku, SellInfoInsertDto>().ReverseMap();
            //CreateMap<Spu, MarketingInsertDto>().ReverseMap();
            CreateMap<Spu_QnA, Spu_QnA_Send>()
                .ForMember(x=>x.created_at,y=>y.MapFrom(s=>WebShopping.Helpers.Tools.Formatter.FormatDate(s.created_at)))
                .ForMember(x => x.updated_at, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.updated_at)))
                .ReverseMap();
        }
    }
}