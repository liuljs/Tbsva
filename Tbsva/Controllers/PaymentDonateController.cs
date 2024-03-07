using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebShopping.Dtos;
using WebShopping.Helpers;
using WebShopping.Services;

namespace WebShopping.Controllers
{
    public class PaymentDonateController : ApiController
    {
        #region DI依賴注入功能
        private IPaymentDonateService _IPaymentDonateService;  //欄位

        public PaymentDonateController(IPaymentDonateService iPaymentDonateService)
        {
            _IPaymentDonateService = iPaymentDonateService;
        }
        #endregion

        #region 虛擬帳號或超商代碼回傳入口
        /// <summary>
        /// 虛擬帳號或超商代碼回傳入口
        /// </summary>
        /// <returns></returns>
        [Route("PaymentDonate/Receive")]
        public IHttpActionResult Receive()
        {
            HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
            try
            {
                string JSON = SystemFunctions.GetJsonData(_request);  //自動接收所有回傳參數，並排列成JSON格式
                PaymentReceiveDto _PaymentReceiveDto = JsonConvert.DeserializeObject<PaymentReceiveDto>(JSON);  //將JSON格式轉成類別
                _PaymentReceiveDto.json = JSON; //整個JSON再單獨給這個欄位
                string _result = string.Empty;           //宣告依公式加參數編出來的驗證碼
                bool Check = _IPaymentDonateService.ValidateToken(_request, out _result);   //比對是否通過驗證
                _IPaymentDonateService.WriteLogFileTxt(_request, "Receive", JSON, _result); //記錄一些表單回傳值
                if (Check)  //true || 比對金流送來ValidateToken是否一樣，一樣才處理
                {
                    _IPaymentDonateService.UpdateReceive(_PaymentReceiveDto);                 // 將金流傳進來的欄位更新捐款主表資料
                    string url = $"/flowPayment.html?id={_PaymentReceiveDto.OrderNo}";  //送前端再調出訂單某些資料
                    HttpContext.Current.Response.Redirect(url);  //轉到指定訂單畫面
                    return Ok(_PaymentReceiveDto);      //或類別，但要先將上一行註解
                }
                else
                {
                    return Ok();   //return NotFound();
                }

            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        # region 信用卡付款刷卡回傳入口
        /// <summary>
        /// 信用卡付款刷卡回傳入口
        /// </summary>
        /// <returns></returns>
        [Route("PaymentDonate/Return")]
        public IHttpActionResult Return()
        {
            HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
            try
            {
                string JSON = SystemFunctions.GetJsonData(_request);  //自動接收所有回傳參數，並排列成JSON格式
                PaymentReturnDto _PaymentReturnDto = JsonConvert.DeserializeObject<PaymentReturnDto>(JSON);     //將JSON格式轉成類別
                _PaymentReturnDto.json = JSON; //整個JSON再單獨給這個欄位
                string _result = string.Empty;           //宣告依公式加參數編出來的驗證碼
                bool Check = _IPaymentDonateService.ValidateToken(_request, out _result);   //比對是否通過驗證
                _IPaymentDonateService.WriteLogFileTxt(_request, "Return", JSON, _result); //記錄一些表單回傳值
                if (Check)  //true || 比對金流送來ValidateToken是否一樣，一樣才處理
                {
                    _IPaymentDonateService.UpdateReturn(_PaymentReturnDto);                // 將金流傳進來的欄位更新捐款主表資料
                    string url = $"/Order_Return.html?id={_PaymentReturnDto.OrderNo}"; //送前端再調出訂單某些資料
                    HttpContext.Current.Response.Redirect(url);
                    return Ok(_PaymentReturnDto);
                }
                else
                {
                    return Ok();//return NotFound();
                }
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            } 
        }
        #endregion

    }
}
