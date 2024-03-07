using AutoMapper;
using WebShopping.Dtos;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Profiles
{
    public class OrdersAdminProfile : Profile
    {
        public OrdersAdminProfile()
        {
            CreateMap<Orders, SendOrdersGetDto>()
                .ForMember(x => x.Creation_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Creation_Date)))
                .ForMember(x => x.Purchase_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Purchase_Date)))
                .ForMember(x => x.Delivery_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Delivery_Date)))
                .ForMember(x => x.Arrival_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Arrival_Date)))
                .ForMember(x => x.Pay_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Pay_Date)))
                .ForMember(x => x.Pay_End_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Pay_End_Date)))
                //.ForMember(x => x.Delivery_Type, y => y.Ignore
                //.ForMember(x => x.Id , y => y.MapFrom(s => $"#{s.Id.ToString("D8")}"))                          //訂單編號  <== 訂單編號Id (EX: #00000001)
                .ForMember(x => x.Order_Status, y => y.MapFrom(s => s.Order_Status_Dic[s.Order_Status_Id]))     //訂單狀態  <==  訂單狀態Id
                .ForMember(x => x.Pay_Type, y => y.MapFrom(s => s.Pay_Type_Array[s.Pay_Type_Id]))               //付款方式  <==  付款方式Id
                .ForMember(x => x.Delivery_Type, y => y.MapFrom(s => s.Delivery_Type_Array[s.Delivery_Type_Id]))//配送方式  <==  配送方式Id
                .ReverseMap();

            CreateMap<Orders, SendOrderDetailGetDto>()
                .ForMember(x => x.Creation_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Creation_Date)))
                .ForMember(x => x.Purchase_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Purchase_Date)))
                .ForMember(x => x.Delivery_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Delivery_Date)))
                .ForMember(x => x.Arrival_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Arrival_Date)))
                .ForMember(x => x.Update_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Update_Date)))
                .ForMember(x => x.Pay_End_Date, y => y.MapFrom(s => WebShopping.Helpers.Tools.Formatter.FormatDate(s.Pay_End_Date)))
                .ForMember(x => x.Address, y => y.MapFrom(s => s.MemberInfo.Address))   //購買人住址  
                .ForMember(x => x.Phone, y => y.MapFrom(s => s.MemberInfo.Phone))       //購買人電話號碼
                .ForMember(x => x.Name, y => y.MapFrom(s => s.MemberInfo.Name))          //購買人姓名
                .ForMember(x => x.Account, y => y.MapFrom(s => s.MemberInfo.Account))   //購買人信箱
                .ForMember(x => x.Order_Status, y => y.MapFrom(s => s.Order_Status_Dic[s.Order_Status_Id]))     //訂單狀態  <==  訂單狀態Id
                .ForMember(x => x.Pay_Type, y => y.MapFrom(s => s.Pay_Type_Array[s.Pay_Type_Id]))               //付款方式  <==  付款方式Id
                .ForMember(x => x.Delivery_Type, y => y.MapFrom(s => s.Delivery_Type_Array[s.Delivery_Type_Id]))//配送方式  <==  配送方式Id
                .ReverseMap();
        }
    }
}