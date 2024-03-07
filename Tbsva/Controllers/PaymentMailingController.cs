using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebShopping.Auth;
using WebShopping.Dtos;
using WebShopping.Helpers;
using WebShopping.Models;
using WebShopping.Services;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 後台權限需登入設定，全域
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    public class PaymentMailingController : ApiController
    {
        #region DI依賴注入功能
        private readonly IPaymentMailingService _IPaymentMailingService;
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;

        /// <summary>
        /// DI依賴注入功能
        /// </summary>
        /// <param name="IPaymentMailingService">介面</param>
        /// <param name="mapper">自動轉型</param>
        /// <param name="IImageFormatHelper">透過MIME檢查上傳檔案是否為圖檔</param>
        public PaymentMailingController(IPaymentMailingService IPaymentMailingService, IMapper mapper, IImageFormatHelper IImageFormatHelper)
        {
            _IPaymentMailingService = IPaymentMailingService;
            m_Mapper = mapper;
            _imageFormatHelper = IImageFormatHelper;
        }
        #endregion

        #region 新增
        // POST: /PaymentMailing
        [HttpPost]
        public IHttpActionResult InsertPaymentMailing()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                }
                _IPaymentMailingService.DeleteAllContents();                //清空資料表所有內容
                PaymentMailing _paymentMailing = _IPaymentMailingService.Insert_PaymentMailing(_request);  //新增資料
                PaymentMailingDto _paymentMailingDto  = m_Mapper.Map<PaymentMailingDto>(_paymentMailing);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _paymentMailingDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");  //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 內容區插入一個圖片
        /// <summary>
        /// 內容新增插入一個圖片
        /// </summary>
        /// <returns>201;圖片URL</returns>
        [HttpPost]
        [Route("PaymentMailing/AddImage")]
        public IHttpActionResult AddImage()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request;  //取得使用者要求的Request物件
                
                if (_request.Files.Count == 0)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_imageFormatHelper.CheckImageMIME(_request.Files) == false)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }

                string _imageUrl = _IPaymentMailingService.AddImage(_request);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new { Image_Link = _imageUrl }));

            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得所有內容(只會有一筆資料)
        // GET: /PaymentMailing/
        [AllowAnonymous]
        public IHttpActionResult GetPaymentMailing()
        {
            PaymentMailing _paymentMailing = _IPaymentMailingService.Get_PaymentMailing();

            if (_paymentMailing != null)
            {
                PaymentMailingDto _paymentMailingDto = m_Mapper.Map<PaymentMailingDto>(_paymentMailing);  // 轉換型別
                return Ok(_paymentMailingDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

    }
}
