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
    public class TextEditorProfile : Profile
    {
        public TextEditorProfile()
        {
            CreateMap<TextEditor, TextEditorDto>()
            .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.creationDate)))
            .ReverseMap();
        }
    }
}