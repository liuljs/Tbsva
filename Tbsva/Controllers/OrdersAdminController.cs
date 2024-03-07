using AutoMapper;
using WebShopping.Auth;
using WebShopping.Dtos;
using WebShopping.Models;
using WebShopping.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 後台會員訂單
    /// </summary>
    [CustomAuthorize(Role.Admin)]    
    public class OrdersAdminController : ApiController
    {
        private IOrdersService m_OrdersService;
        private IMapper m_Mapper;

        public OrdersAdminController(IOrdersService orderService, IMapper mapper)        
        {
            m_OrdersService = orderService;
            m_Mapper = mapper;
        }

        // GET: api/OrderAdmin
        /// <summary>
        /// 取得訂單資料(無條件)
        /// </summary>       
        /// <returns></returns>        
        [HttpGet]
        public IHttpActionResult GetOrders()
        {
            List<Orders> orders_ = m_OrdersService.GetOrders();

              if (orders_.Count > 0)
              {
                  List<SendOrdersGetDto> dtos_ = m_Mapper.Map<List<SendOrdersGetDto>>(orders_); // 轉換型別

                  return Ok(dtos_);
              }
              else
              {
                  return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }

        // GET: api/OrderAdmin/?id=null&starDate=null&endDate=null&count=null&page=null
        /// <summary>
        ///  取得訂單資料(有條件)
        /// </summary>
        /// <param name="id"> 訂單id </param>
        /// <param name="startDate"> 訂單建立日期_開始 </param>
        /// <param name="endDate"> 訂單建立日期_結束 </param>
        /// <param name="count"> 一頁幾筆 </param>
        /// <param name="page"> 第幾頁 </param>
        /// <returns></returns>        
        //public IHttpActionResult GetOrders(int id, RecvOrdersGetDto p_RecvOrdersGetDto)
        [HttpPost]
        [Route("OrdersAdmin/GetOrders")]
        //public IHttpActionResult GetOrders(int? id, int? order_status_id, DateTime? startDate, DateTime? endDate, int? count, int? page)
        public IHttpActionResult GetOrders(OrderQuery query)
        {

            List<Orders> orders_ = m_OrdersService.GetOrdersByCondition(query.id, query.order_status_id, query.startDate, query.endDate, query.count, query.page);

            if (orders_.Count > 0)
            {
                List<SendOrdersGetDto> dtos_ = m_Mapper.Map<List<SendOrdersGetDto>>(orders_); // 轉換型別

                return Ok(dtos_);
            }
            else
            {
                return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }

        // GET: api/OrderAdmin/?id=1
        /// <summary>
        /// 取得訂單詳情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetOrderDetail(int ?id)
        {
             Orders orderDetail_ = m_OrdersService.GetOrderDetail(id);
            
             if (orderDetail_ != null)
             {
                SendOrderDetailGetDto dtos_ = m_Mapper.Map<SendOrderDetailGetDto>(orderDetail_); // 轉換型別

                 return Ok(dtos_);
             }
             else
             {
                return Ok();
                //return StatusCode(HttpStatusCode.NotFound);
             }
            //return StatusCode(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// 管理者更改訂單備註
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("OrdersAdmin/UpdateOrderMemo")]
        public IHttpActionResult UpdateOrderMemo(OrderMemo dto)
        {
            dto.Manager_Id = new Guid(User.Identity.Name);
            int i = m_OrdersService.AdminUpdateOrderMemo(dto);
            return Ok();
        }
        /// <summary>
        /// 修改訂單狀態
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("OrdersAdmin/UpdateOrderStatus")]
        public IHttpActionResult UpdateOrderStatus(ChangeOrderStatusDto dto)
        {
            //TODO:管理員修改訂單未完，要考慮各種訂單變化狀態
            /*
                14	待付款-賣家取消
                16	已付款-買家成功付款
                21	待出貨-未出貨
                22	待出貨-備貨中
                23	待出貨-已出貨
                31	運送中-未送達
                32	運送中-已送達
                41	已完成-未取貨
                42	已完成-已取貨
                52	取消-已取消
                62	退貨-已退款


             */
            List<Orders> orders_ = m_OrdersService.GetOrdersByCondition(dto.Id, null, null, null, null, null);

            if (orders_.Count > 0)
            {
                string msg = string.Empty;

                Guid Manager_Id = new Guid(User.Identity.Name);

                Orders order = orders_[0];
                if (order.Deleted == 0)//order.Is_Cancel == 0 && order.Is_Return == 0 &&
                    switch (dto.Order_Status_Id)
                    {
                        case 14:
                            {
                                //檢查訂單狀態，能否取消
                                switch (order.Order_Status_Id)
                                {
                                    case 11:
                                    case 12:
                                    case 13:
                                    case 16:
                                    case 21:
                                        //取消訂單，狀態改為52
                                        order.Order_Status_Id = 52;
                                        order.Is_Cancel = 1;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "管理者取消訂單", dto.memo, true, Manager_Id);
                                        msg = "取消成功!!";
                                        break;
                                    default:
                                        msg = $"取消失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        case 16:
                            {
                                switch (order.Order_Status_Id)
                                {
                                    case 11:
                                        //待出貨-未出貨
                                        order.Order_Status_Id = 21;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "待出貨-未出貨", dto.memo, true, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        case 21:
                            {
                                //檢查訂單狀態，能否出貨
                                switch (order.Order_Status_Id)
                                {
                                    case 11://測試用
                                        order.Order_Status_Id = 21;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "待出貨-未出貨", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    case 16:
                                        order.Order_Status_Id = 21;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "待出貨-未出貨", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        case 22:
                            {
                                //檢查訂單狀態，能否出貨
                                switch (order.Order_Status_Id)
                                {
                                    case 16:
                                    case 21:
                                        order.Order_Status_Id = 22;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "運送中-未送達", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        case 23:
                            {
                                //檢查訂單狀態，能否出貨
                                switch (order.Order_Status_Id)
                                {
                                    case 16:
                                    case 21:
                                    case 22:
                                        order.Order_Status_Id = 31;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "運送中-未送達", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    case 51:
                                        order.Order_Status_Id = 31;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "運送中-未送達", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        case 31:
                            {
                                //檢查訂單狀態，能否改運送中
                                switch (order.Order_Status_Id)
                                {
                                    case 23:
                                        order.Order_Status_Id = 31;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "運送中-未送達", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        case 32:
                            {
                                //檢查訂單狀態，能否改運送中
                                switch (order.Order_Status_Id)
                                {
                                    case 23:
                                    case 31:
                                        order.Order_Status_Id = 41;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "運送中-已送達", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        case 41:
                            {
                                //檢查訂單狀態，能否改已完成
                                switch (order.Order_Status_Id)
                                {
                                    case 23:
                                    case 31:
                                    case 32:
                                        order.Order_Status_Id = 41;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "已完成-未取貨", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        case 42:
                            {
                                //檢查訂單狀態，能否改已完成
                                switch (order.Order_Status_Id)
                                {
                                    case 23:
                                    case 31:
                                    case 32:
                                    case 41:
                                        order.Order_Status_Id = 42;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "已完成-已取貨", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        case 52:
                            {
                                //檢查訂單狀態，能否改已取消
                                switch (order.Order_Status_Id)
                                {
                                    case 11:
                                    case 51:
                                        order.Order_Status_Id = 52;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "取消-已取消", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        //case 61:
                        //    {
                        //        //檢查訂單狀態，能否改已已退款
                        //        switch (order.Order_Status_Id)
                        //        {
                        //            case 41:
                        //                order.Order_Status_Id = 61;
                        //                m_OrdersService.AdminUpdateOrderStatus(order, "前台消費者操作退貨", dto.memo, false, Manager_Id);
                        //                msg = "狀態變更成功!!";
                        //                break;
                        //            default:
                        //                msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                        //                break;
                        //        }
                        //    }
                        //    break;
                        case 62:
                            {
                                //檢查訂單狀態，能否改已已退款
                                switch (order.Order_Status_Id)
                                {
                                    case 61:
                                        order.Order_Status_Id = 62;
                                        m_OrdersService.AdminUpdateOrderStatus(order, "退貨-已退款", dto.memo, false, Manager_Id);
                                        msg = "狀態變更成功!!";
                                        break;
                                    default:
                                        msg = $"狀態變更失敗({order.Order_Status_Id}->{dto.Order_Status_Id})!!";
                                        break;
                                }
                            }
                            break;
                        default:
                            msg = $"狀態({order.Order_Status_Id}->{dto.Order_Status_Id})無法異動!!";
                            break;
                    }
                else
                    msg = "訂單處於取消/退貨或刪除的情況下，無法變更狀態!!";
                return Ok(msg);
            }
            else
            {
                return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }
    }
}