using WebShopping.Dtos;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Services
{
    public interface IOrdersService
    {
        List<Orders> GetOrders();               //取得訂單(無條件的)
        List<Orders> GetOrdersByCondition(int ?id, int[] order_status_id, DateTime ?startDate, DateTime ?endDate, int ?count, int ?page);    //取得訂單(條件的)
        Orders GetOrderDetail(int? id);         //取得訂單詳情
        //bool CancelOrder();                     //取消訂單        
        int MemberUpdateOrderStatus(Orders order, string msg);//會員更新訂單狀態
        int AdminUpdateOrderStatus(Orders order, string msg, string memo, bool IsbackStore, Guid Manager_Id);//管理更新訂單狀態
        int AdminUpdateOrderMemo(OrderMemo order);//管理者更改訂單備註
        int AddCart(SendAddCartDto p_SendAddCartDto);//加入購物車
        List<RecvCartDto> GetCart(Guid member_id);//取得購物車
        int UpdateCart(CardUpdateDto dto);//更新購物車數量
        int DeleteCart(CardUpdateDto dto);//刪除購物車品項

        int UpdatePurchase(PurchaseDto dto);//更新購買資訊(購物車,仍未轉訂單)
        PurchaseDto GetPurchase(PurchaseDto dto);//取得購買資訊
        PurchaseDto CreatPurchase(PurchaseDto dto);//新增購買資訊(購物車轉訂單)

        List<Orders> MemberGetOrders(Guid memner_id, int? p_id, int[] p_order_status_id, DateTime? p_startDate, DateTime? p_endDate, int? p_count, int? p_page);//會員取得訂單(無條件的)
        List<Orders> MemberGetOrdersByCondition(Guid memner_id, int? p_id, int? p_order_status_id, DateTime? p_startDate, DateTime? p_endDate, int? p_count, int? p_page);//會員取得訂單(條件的)
        List<SendOrderItemsGetDto> MemberGetOrderItems(int orders_id);//取得訂單明細
    }
}