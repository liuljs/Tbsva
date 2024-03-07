using AutoMapper;
using WebShopping.Auth;
using WebShopping.Dtos;
using WebShopping.Models;
using WebShopping.Services;
using WebShoppingAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 前台會員訂單
    /// </summary>
    [CustomAuthorize(Role.User)]
    public class OrdersController : ApiController
    {
        private IOrdersService m_OrdersService;
        private IMapper m_Mapper;

        public OrdersController(IOrdersService orderService, IMapper mapper)
        {
            m_OrdersService = orderService;
            m_Mapper = mapper;
        }

        // GET: api/OrderAdmin
        /// <summary>
        /// 取得訂單資料(有條件)
        /// </summary>       
        /// <returns></returns>        
        [HttpPost]
        [Route("Orders/GetOrders")]
        public IHttpActionResult GetOrders(OrderQuery query)
        {
            List<Orders> orders_ = m_OrdersService.MemberGetOrders(new Guid(User.Identity.Name), query.id, query.order_status_id, query.startDate, query.endDate, query.count, query.page);

            if (orders_.Count > 0)
            {
                List<SendOrdersGetDto> dtos_ = m_Mapper.Map<List<SendOrdersGetDto>>(orders_); // 轉換型別
                //放明細
                foreach (SendOrdersGetDto dto in dtos_) 
                    dto.Items = m_OrdersService.MemberGetOrderItems(Convert.ToInt32(dto.Id));

                return Ok(dtos_);
            }
            else
            {
                return Ok();
                //return StatusCode(HttpStatusCode.NotFound);
            }
        }
        [HttpGet]
        [Route("Orders/GetOrder/{id}")]
        public IHttpActionResult GetOrder(int id)
        {
            List<Orders> orders_ = m_OrdersService.MemberGetOrdersByCondition(new Guid(User.Identity.Name), id, null, null, null, null, null);

            if (orders_.Count > 0)
            {
                SendOrdersGetDto dto = m_Mapper.Map<SendOrdersGetDto>(orders_[0]); // 轉換型別
                dto.Items = m_OrdersService.MemberGetOrderItems(Convert.ToInt32(dto.Id));

                return Ok(dto);
            }
            else
            {
                return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Orders/CancelOrder")]
        public IHttpActionResult CancelOrder(CardUpdateDto dto)
        {
            List<Orders> orders_ = m_OrdersService.MemberGetOrdersByCondition(new Guid(User.Identity.Name), dto.Id, null, null, null, null, null);

            if (orders_.Count > 0)
            {
                string msg = "無法取消!!";

                Orders order = orders_[0];

                //檢查訂單狀態，能否取消
                if (order.Is_Cancel == 0 && order.Is_Return == 0 && order.Deleted==0)
                    switch (order.Order_Status_Id)
                    {
                        case 11:
                        case 12:
                        case 13:
                        case 16:
                        case 21:
                            //取消訂單，狀態改為51
                            order.Order_Status_Id = 51;
                            order.Is_Cancel = 1;
                            m_OrdersService.MemberUpdateOrderStatus(order, "消費者取消訂單");
                            msg = "取消成功!!";
                            break;
                        default:
                            msg = "取消失敗!!";
                            break;
                    }

                return Ok(msg);
            }
            else
            {
                return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// 退貨訂單
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Orders/ReturnOrder")]
        public IHttpActionResult ReturnOrder(CardUpdateDto dto)
        {
            List<Orders> orders_ = m_OrdersService.MemberGetOrdersByCondition(new Guid(User.Identity.Name), dto.Id, null, null, null, null, null);

            if (orders_.Count > 0)
            {
                string msg = "無法退貨!!";

                Orders order = orders_[0];

                //檢查訂單狀態，能否退貨
                if (order.Is_Cancel == 0 && order.Is_Return == 0 && order.Deleted == 0)
                    switch (order.Order_Status_Id)
                    {
                        case 32:
                        case 41:
                        case 42:
                            //退貨訂單，狀態改為61
                            order.Order_Status_Id = 61;
                            order.Is_Return = 1;
                            m_OrdersService.MemberUpdateOrderStatus(order, "消費者退貨訂單");
                            msg = "退貨成功!!";
                            break;
                        default:
                            msg = "退貨失敗!!";
                            break;
                    }

                return Ok(msg);
            }
            else
            {
                return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// 完成訂單
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Orders/FinishOrder")]
        public IHttpActionResult FinishOrder(CardUpdateDto dto)
        {
            List<Orders> orders_ = m_OrdersService.MemberGetOrdersByCondition(new Guid(User.Identity.Name), dto.Id, null, null, null, null, null);

            if (orders_.Count > 0)
            {
                string msg = "無法變更!!";

                Orders order = orders_[0];

                //檢查訂單狀態，能否退貨
                if (order.Is_Cancel == 0 && order.Is_Return == 0 && order.Deleted == 0)
                    switch (order.Order_Status_Id)
                    {
                        case 23:
                        case 31:
                        case 32:
                        case 41:
                            //完成訂單，狀態改為42
                            order.Order_Status_Id = 42;
                            m_OrdersService.MemberUpdateOrderStatus(order, "消費者完成訂單");
                            msg = "消費者完成訂單成功!!";
                            break;
                        default:
                            msg = "消費者完成訂單失敗!!";
                            break;
                    }

                return Ok(msg);
            }
            else
            {
                return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }

        ///// <summary>
        /////  會員取得訂單資料(無條件)
        ///// </summary>
        ///// <param name="id"> 訂單id </param>
        ///// <param name="order_status_id"> 狀態id </param>
        ///// <param name="startDate"> 訂單建立日期_開始 </param>
        ///// <param name="endDate"> 訂單建立日期_結束 </param>
        ///// <param name="count"> 一頁幾筆 </param>
        ///// <param name="page"> 第幾頁 </param>
        ///// <returns></returns>        
        //[HttpGet]
        //[Route("Orders/GetMemberOrders")]
        //public IHttpActionResult GetMemberOrders(int? id,int? order_status_id, DateTime? startDate, DateTime? endDate, int? count, int? page)
        //{
        //    List<Orders> orders_ = m_OrdersService.MemberGetOrdersByCondition(new Guid(User.Identity.Name),id, order_status_id, startDate, endDate, count, page);

        //    if (orders_.Count > 0)
        //    {
        //        List<SendOrdersGetDto> dtos_ = m_Mapper.Map<List<SendOrdersGetDto>>(orders_); // 轉換型別

        //        return Ok(dtos_);
        //    }
        //    else
        //    {
        //        return StatusCode(HttpStatusCode.NotFound);
        //    }
        //}

        [Route("Orders/AddCart")]
        [HttpPost]
        public object AddCart(SendAddCartDto p_SendAddCartDto)//商品加入購物車
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //只能取自己的
                    Guid id = new Guid(User.Identity.Name);
                    p_SendAddCartDto.member_id = id;

                    int i = m_OrdersService.AddCart(p_SendAddCartDto);
                    //return Ok(i);
                    return new ApiResult(i);
                }
                else
                {
                    var _errMsg = ModelState.Values.Where(x => x.Errors.Count > 0)?.FirstOrDefault()?.Errors.Select(y => y.ErrorMessage).FirstOrDefault();
                    return Content(HttpStatusCode.BadRequest, _errMsg);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// 取得購物車清單
        /// </summary>       
        /// <returns></returns>        
        [HttpGet]
        [Route("Orders/GetCart")]
        public IHttpActionResult GetCart()
        {
            List<RecvCartDto> orders_ = m_OrdersService.GetCart(new Guid(User.Identity.Name));

            if (orders_.Count > 0)
            {
                //List<RecvCartDto> dtos_ = m_Mapper.Map<List<RecvCartDto>>(orders_); // 轉換型別
                var dto = new RecvMasterCartDto();
                dto.Items = m_Mapper.Map<List<RecvCartDto>>(orders_); // 轉換型別
                return Ok(dto);
            }
            else
            {
                return Ok();
                //return StatusCode(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// 更改購物車數量
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Orders/UpdateCart")]
        public IHttpActionResult UpdateCart(CardUpdateDto dto)
        {
            if (m_OrdersService.UpdateCart(dto) > 0)
                return StatusCode(HttpStatusCode.NoContent);
            else
                return Ok();//return NotFound();
        }

        /// <summary>
        /// 刪除購物車品項
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Orders/DeleteCart")]
        public IHttpActionResult DeleteCart(CardUpdateDto dto)
        {
            if (m_OrdersService.DeleteCart(dto) > 0)
                return GetCart();
            //return StatusCode(HttpStatusCode.NoContent);
            else
                return Ok();//return NotFound();
        }

        //[HttpGet]
        //[Route("Orders/GetPurchase")]
        //public IHttpActionResult GetPurchase(PurchaseDto dto)
        //{
        //    //只能取自己的
        //    Guid id = new Guid(User.Identity.Name);
        //    dto.member_id = id;

        //    if (m_OrdersService.UpdatePurchase(dto) > 0)
        //        return StatusCode(HttpStatusCode.NoContent);
        //    else
        //        return NotFound();
        //}
        /// <summary>
        /// 更新購買資訊(購物車,仍未轉訂單)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Orders/UpdatePurchase")]
        public IHttpActionResult UpdatePurchase(PurchaseDto dto)
        {
            //只能取自己的
            Guid id = new Guid(User.Identity.Name);
            dto.member_id = id;

            if (m_OrdersService.UpdatePurchase(dto) > 0)
                return StatusCode(HttpStatusCode.NoContent);
            else
                return Ok();//return NotFound();
        }

        /// <summary>
        /// 新增購買資訊(購物車轉訂單)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Orders/CreatPurchase")]
        public IHttpActionResult CreatPurchase()
        {
            PurchaseDto sentDto = new PurchaseDto();
            sentDto.member_id = new Guid(User.Identity.Name);

            PurchaseDto dto = m_OrdersService.CreatPurchase(sentDto);

            if (dto == null) return Ok("庫存不足!!");
            int order_id = dto.id;//取回訂單編號
            //轉到下訂單
            //bool isTry = true;
            string host = Helpers.Tools.Host;  //Web.config, 切換正式與測試
            //string host = "https://service.payware.com.tw";
            //if(isTry)host = "https://test.payware.com.tw";

            if (order_id > 0) {
                string PayType = string.Empty;
                //1   信用卡
                //2   ATM轉帳
                //3   超商帳單代收
                //4   7-11
                //5   全家
                //6   萊爾富
                switch (dto.pay_type_id) {
                    case 1: PayType = "01"; break;
                    case 2: PayType = "2"; break;
                    case 3: PayType = "7"; break;
                    case 4: PayType = "5"; break;
                    case 5: PayType = "6"; break;
                    case 6: PayType = "10"; break;
                    default : PayType = "01"; break;
                }
                PaymentDto payment = new PaymentDto()
                {
                    RequestUrl = $"{host}/wpss/authpay.aspx",
                    ReturnURL = $"{Helpers.Tools.WebSiteUrl}/api/Payment/ReturnURL",
                    PayType = PayType,
                    OrderNo = dto.id.ToString(),
                    Amount = Convert.ToInt32(dto.order_total)+ Convert.ToInt32(dto.delivery_fee),
                    Product = "商品",
                    OrderDesc = dto.memo_customer,
                    Mobile = dto.purchaser_phone,
                    TelNumber = dto.purchaser_tel,
                    Address = dto.receiver_address,
                    Email = dto.purchaser_email,
                    ValidateKey = "validateKey",
                    memberId = dto.member_id.ToString(),
                    GoBackURL = Helpers.Tools.WebSiteUrl,
                    ReceiveURL = $"{Helpers.Tools.WebSiteUrl}/api/Payment/ReceiveURL",
                    DeadlineDate = DateTime.Today.AddDays(7).ToString("yyyy/MM/dd"),
                    Invoice = new InvoiceDto()
                    {
                        deferred = 7,
                        Carrier = "",
                        InvoiceName = ""
                    }
                };
                return Ok(payment);
            }
            else
                return Ok();//return NotFound();
        }


    }
}