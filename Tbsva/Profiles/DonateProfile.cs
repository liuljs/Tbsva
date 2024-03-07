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
    public class DonateProfile : Profile
    {
        public DonateProfile()
        {
            CreateMap<Donate, DonateDto>()
                 .ForMember(target => target.orderId, option => option.MapFrom(source => source.OrderId))
                 .ForMember(target => target.donateType, option => option.MapFrom(source => source.donateType))
                 .ForMember(target => target.buyerType, option => option.MapFrom(source => source.BuyerType))
                 .ForMember(target => target.buyerName, option => option.MapFrom(source => source.BuyerName))
                 .ForMember(target => target.buyerSex, option => option.MapFrom(source => source.buyerSex))
                 .ForMember(target => target.affiliatedArea, option => option.MapFrom(source => source.affiliatedArea))
                 .ForMember(target => target.buyerId, option => option.MapFrom(source => source.BuyerId))
                 .ForMember(target => target.buyerPhone, option => option.MapFrom(source => source.BuyerPhone))
                 .ForMember(target => target.mobilePhone, option => option.MapFrom(source => source.mobilePhone))
                 .ForMember(target => target.buyerEmail, option => option.MapFrom(source => source.BuyerEmail))
                 .ForMember(target => target.address1, option => option.MapFrom(source => source.address1))
                 .ForMember(target => target.address2, option => option.MapFrom(source => source.address2))
                 .ForMember(target => target.address3, option => option.MapFrom(source => source.address3))

                 .ForMember(target => target.payType, option => option.MapFrom(source => source.PayType))
                 .ForMember(target => target.payTypeName, option => option.MapFrom(source => source.DonatePayType))
                 .ForMember(target => target.orderDate, option => option.MapFrom(source => source.OrderDate.ToString("yyyy-MM-dd")))
                 .ForMember(target => target.amount, option => option.MapFrom(source => source.Amount))

                 .ForMember(target => target.needReceipt, option => option.MapFrom(source => source.NeedReceipt))
                 .ForMember(target => target.receiptTitle, option => option.MapFrom(source => source.ReceiptTitle))
                 .ForMember(target => target.receiptPostMethod, option => option.MapFrom(source => source.ReceiptPostMethod))
                 .ForMember(target => target.needAnonymous, option => option.MapFrom(source => source.NeedAnonymous))
                 .ForMember(target => target.remark, option => option.MapFrom(source => source.Remark))
                 //.ForMember(target => target.updateDate, option => option.MapFrom(source => source.UpdateDate.ToString("yyyy-MM-dd")))
                 .ForMember(target => target.updateDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.UpdateDate)))

                 .ForMember(target => target.PayStatus, option => option.MapFrom(source => source.PayStatusVirtual))
                .ReverseMap();

            CreateMap<Donate, DonationsDto>()
                 .ForMember(target => target.buyerName, option => option.MapFrom(source => source.BuyerName))
                 .ForMember(target => target.orderDate, option => option.MapFrom(source => source.OrderDate.ToString("yyyy-MM-dd")))
                 .ForMember(target => target.amount, option => option.MapFrom(source => source.Amount))
                .ReverseMap();

            CreateMap<DonatePayType, DonatePayTypeDto>()
                .ForMember(target => target.name, option => option.MapFrom(source => source.Name))
                .ReverseMap();

            CreateMap<DonateRelatedItem, DonateRelatedItemDto>()
                 .ForMember(target => target.id, option => option.MapFrom(source => source.Id))
                 .ForMember(target => target.donateRelatedItemId, option => option.MapFrom(source => source.donateRelatedItemId))
                 .ForMember(target => target.primary, option => option.MapFrom(source => source.primary))
                 .ForMember(target => target.secondary, option => option.MapFrom(source => source.secondary))
                 .ForMember(target => target.imageURL, option => option.MapFrom(source => source.ImageName))
                 .ForMember(target => target.content1, option => option.MapFrom(source => source.Content1))
                 .ForMember(target => target.content2, option => option.MapFrom(source => source.Content2))
                 .ForMember(target => target.content3, option => option.MapFrom(source => source.content3))
                 .ForMember(target => target.amount, option => option.MapFrom(source => source.Amount))
                 .ForMember(target => target.notes, option => option.MapFrom(source => source.Notes))
                 .ForMember(target => target.sort, option => option.MapFrom(source => source.Sort))
                 .ForMember(target => target.first, option => option.MapFrom(source => source.first))
                 .ForMember(target => target.enabled, option => option.MapFrom(source => source.enabled))
                 .ForMember(target => target.creationDate, option => option.MapFrom(source => Tools.Formatter.FormatDateV2(source.CreateDate))) 
                .ReverseMap();

            CreateMap<DonateRelatedItemImage, DonateRelatedItemImageDto>()
                .ForMember(target => target.imageId, option => option.MapFrom(source => source.donateRelatedItemImageId))
                .ForMember(target => target.imageURL, option => option.MapFrom(source => source.imageName))
                .ReverseMap();

            CreateMap<DonateRelatedItemRecord, DonateRelatedItemRecordDto>()
                 .ForMember(target => target.id, option => option.MapFrom(source => source.Id))
                 .ForMember(target => target.orderId, option => option.MapFrom(source => source.OrderId))
                 .ForMember(target => target.title, option => option.MapFrom(source => source.Title))
                 .ForMember(target => target.amount, option => option.MapFrom(source => source.Amount))
                 .ForMember(target => target.qty, option => option.MapFrom(source => source.Qty))
                .ReverseMap();
        }
    }
}