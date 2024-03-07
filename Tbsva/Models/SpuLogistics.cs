using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public enum Logistics
    {
        SevenEleven, //7-11
        FamilyMart, //全家
        HiLife, //萊爾富
        HomeDelivery //宅配
    }

    public class SpuLogistics
    {
        public int Id { get; set; }

        public int Spu_Id { get; set; }

        public Logistics Code { get; set; }

        public decimal Shipping_Fee { get; set; }

        public OpenStatus Enable { get; set; }
    }
}