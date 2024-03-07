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
    public class DonateController : ApiController
    {
        #region DI依賴注入功能
        private IDonateService _IDonateService;  //欄位
        private IMapper m_Mapper;
        public DonateController(IDonateService iDonateService, IMapper mapper)
        {
            _IDonateService = iDonateService;
            m_Mapper = mapper;
        }
        #endregion

        #region 前端捐款新增,新增的有Donate 捐款主表資料,DonateRelatedItemRecord訂購明細記錄
        //訂購明細記錄的資料是來自DonateRelatedItem訂購明細項目，由前端設計動態叫出DonateRelatedItem API
        // POST: api/Donate
        [HttpPost]
        [AllowAnonymous]
        [Route("Donate")]
        public IHttpActionResult InsertDonate()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                //  非必填
                if (string.IsNullOrWhiteSpace(_request.Form.Get("donateType")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("donateType") == null ? "必須有donateType" : "donateType參數格式錯誤"));
                }
                if (!new string[] { "1", "2" }.Contains(_request.Form.Get("donateType")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "捐款方式 一般捐款:1  結緣捐贈:2"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("buyerName")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("buyerName") == null ? "必須有buyerName" : "buyerName參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("affiliatedArea")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("affiliatedArea") == null ? "必須有affiliatedArea" : "affiliatedArea參數格式錯誤"));
                }
                if (!new string[] { "0", "1", "2", "3", "4" }.Contains(_request.Form.Get("affiliatedArea")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "隸屬地區 0無1北部2中部3南部4海外"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("buyerPhone")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("buyerPhone") == null ? "必須有buyerPhone" : "buyerPhone參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("buyerEmail")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("buyerEmail") == null ? "必須有buyerEmail" : "buyerEmail參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("address1")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("address1") == null ? "必須有address1" : "address1參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("payType")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("payType") == null ? "必須有payType" : "payType參數格式錯誤"));
                }
                if (!new string[] { "01", "2", "4", "6", "7","9", "10", "13", "83" }.Contains(_request.Form.Get("payType")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "無此付款方式:付款方式01線上刷卡,2虛擬帳號, 4 7-11(超商代碼),6FamilyMart(超商代碼),9 OK 超商(超商代碼),10萊爾富(超商代碼),83銀行匯款"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("amount")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("amount") == null ? "必須有amount" : "amount參數格式錯誤"));
                }
                //if ( (int.TryParse(_request.Form.Get("Amount"), out int _Amount) ? _Amount : 0 ) == 0 )
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "金額不能為0"));
                //}
                if (string.IsNullOrWhiteSpace(_request.Form.Get("needReceipt")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("needReceipt") == null ? "必須有needReceipt" : "needReceipt參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("receiptPostMethod")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("receiptPostMethod") == null ? "必須有receiptPostMethod" : "receiptPostMethod參數格式錯誤"));
                }
                if (!new string[] { "0", "1" }.Contains(_request.Form.Get("receiptPostMethod")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "是否寄送 是:1  否:0"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("needAnonymous")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("needAnonymous") == null ? "必須有needAnonymous" : "needAnonymous參數格式錯誤"));
                }
                if (!new string[] { "0", "1" }.Contains(_request.Form.Get("needAnonymous")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "是否開公開捐款人在前台  是:1  否:0"));
                }

                Donate _Donate = _IDonateService.Insert_Donate(_request);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別,若有子類別Profile要設定，子類別名稱也要一樣)
                DonateDto _DonateDto = m_Mapper.Map<DonateDto>(_Donate);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _DonateDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\n{ex.Message}\n{ex.StackTrace}");
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 後端取得單筆內容(用在後端檢視單筆內容時顯示)
        // GET: api/Donate/5
        [Route("Donate/{id}")]
        public IHttpActionResult GetDonate(string id)
        {
            Donate _Donate = _IDonateService.Get_Donate(id);
            if (_Donate != null)
            {
                DonateDto _Donate_Dto = m_Mapper.Map<DonateDto>(_Donate); // 轉換型別_Donate =>_Donate_Dto
                return Ok(_Donate_Dto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 後端取得所有內容
        // GET: api/Donate
        [Route("Donate")]
        public IHttpActionResult GetDonate()
        {
            HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件

            int? _count = null;
            int? _page = null;
            string _OrderId = string.Empty;
            int? _donateType = null;
            string _BuyerName = string.Empty;
            string _PayType = string.Empty;
            DateTime ? _StartDate = null ;
            DateTime? _EndDate = null;
            int? _PayStatus = null ;
            if (!string.IsNullOrWhiteSpace(_request["count"]))
            {
                _count = Convert.ToInt32(_request["count"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["page"]))
            {
                _page = Convert.ToInt32(_request["page"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["OrderId"]))
            {
                _OrderId = _request["OrderId"].ToString().Trim();
            }
            if (!string.IsNullOrWhiteSpace(_request["donateType"]))
            {
                _donateType = Convert.ToByte(_request["donateType"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["BuyerName"]))
            {
                _BuyerName = _request["BuyerName"].ToString();
            }
            if (!string.IsNullOrWhiteSpace(_request["PayType"]))
            {
                _PayType = _request["PayType"].ToString();
            }
            if (!string.IsNullOrWhiteSpace(_request["StartDate"]))
            {
                _StartDate = DateTime.Parse(_request["StartDate"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["EndDate"]))
            {
                _EndDate = DateTime.Parse(_request["EndDate"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["PayStatus"]))
            {
                _PayStatus = Convert.ToByte(_request["PayStatus"]);
            }
            List<Donate> _Donates = _IDonateService.Get_Donate_ALL(_count, _page, _OrderId, _donateType, _BuyerName, _PayType, _StartDate, _EndDate, _PayStatus);

            if (_Donates.Count > 0)
            {
                List<DonateDto> _Donate_Dtos = m_Mapper.Map<List<DonateDto>>(_Donates);  // 多筆轉換型別
                return Ok(_Donate_Dtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 前端取得所有內容(善捐名單NeedAnonymous是否開公開捐款姓名及金額)
        // GET: api/Donate/Donations
        [AllowAnonymous]
        [Route("Donate/Donations")]
        public IHttpActionResult GetDonations()
        {
            HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
            string _BuyerName = string.Empty;
            int? _count = null;
            int? _page = null;

            if (!string.IsNullOrWhiteSpace(_request["buyerName"]))
            {
                _BuyerName = _request["buyerName"].ToString();
            }
            if (!string.IsNullOrWhiteSpace(_request["count"]))
            {
                _count = Convert.ToInt32(_request["count"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["page"]))
            {
                _page = Convert.ToInt32(_request["page"]);
            }

            List<Donate> _Donates = _IDonateService.Get_Donations_ALL(_BuyerName, _count, _page);

            if (_Donates.Count > 0)
            {
                List<DonationsDto> _donationsDtos = m_Mapper.Map<List<DonationsDto>>(_Donates);  // 多筆轉換型別
                return Ok(_donationsDtos);
            }
            else
            {
                return NotFound();
            }

        }
        #endregion

        #region 更新捐款狀態為已結案
        // PUT: api/donate/{id}
        [HttpPut]
        [Route("donate/{id}")]
        public IHttpActionResult UpdateDonateStatus(string id)
        {
            try
            {
                Donate _Donate = _IDonateService.Get_Donate(id);
                if (_Donate != null)
                {
                    _IDonateService.UpdateDonateStatus(_Donate);
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion


        #region 更新繳款狀態為已付款(83銀行匯款)
        // PUT: api/bankRemittance/{id}
        [HttpPut]
        [Route("donate/bankRemittance/{id}")]
        public IHttpActionResult UpdateBankRemittanceStatus(string id)
        {
            try
            {
                Donate _Donate = _IDonateService.Get_Donate(id);
                if (_Donate != null)
                {
                    _IDonateService.UpdateBankRemittanceStatus(_Donate);
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion


        #region 更新繳款狀態為已付款(2虛擬帳號, 7超商代碼)
        // PUT: api/virtualAtmSuperMarket/{id}
        [HttpPut]
        [Route("donate/virtualAtmSuperMarket/{id}")]
        public IHttpActionResult UpdateVirtualAtmSuperMarketStatus(string id)
        {
            try
            {
                Donate _Donate = _IDonateService.Get_Donate_UpdateVirtualAtmSuperMarketStatus(id);

                if (_Donate != null)
                {
                    _IDonateService.UpdateVirtualAtmSuperMarketStatus(_Donate);
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion
    }
}
