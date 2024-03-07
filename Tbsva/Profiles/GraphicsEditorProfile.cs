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
    public class GraphicsEditorProfile : Profile
    {
        public GraphicsEditorProfile()
        {
            CreateMap<GraphicsEditor, GraphicsEditorDto>()
            .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))
            .ForMember(target => target.imageURL01, option => option.MapFrom(source => source.navPics01))
            .ForMember(target => target.imageURL02, option => option.MapFrom(source => source.navPics02))
            .ForMember(target => target.imageURL03, option => option.MapFrom(source => source.navPics03))
            .ForMember(target => target.imageURL04, option => option.MapFrom(source => source.navPics04))
            .ForMember(target => target.imageURL05, option => option.MapFrom(source => source.navPics05))
            .ForMember(target => target.imageURL06, option => option.MapFrom(source => source.navPics06))
            .ForMember(target => target.imageURL07, option => option.MapFrom(source => source.navPics07))
            .ForMember(target => target.imageURL08, option => option.MapFrom(source => source.navPics08))
            .ReverseMap();

            //CreateMap<GraphicsEditorImage, GraphicsEditorImageDto>()
            //.ForMember(target => target.imageId, option => option.MapFrom(source => source.graphics_editor_image_id))
            //.ForMember(target => target.imageURL, option => option.MapFrom(source => source.image_name))
            //.ReverseMap();
        }
    }
}