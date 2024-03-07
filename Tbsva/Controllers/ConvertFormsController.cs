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

    public class ConvertFormsController : ApiController
    {
        #region DI依賴注入功能
        private IConvertFormsService _convertFormsService;
        private IMapper _mapper;
        public ConvertFormsController(IConvertFormsService convertFormsService, IMapper mapper)
        {
            _convertFormsService = convertFormsService;
            _mapper = mapper;
        }
        #endregion
        #region 新增
        // POST: api/convertForms
        [HttpPost]
        public IHttpActionResult InsertConvertForms()
        {
            try
            {
                HttpRequest httpRequest = HttpContext.Current.Request;
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if(string.IsNullOrWhiteSpace(httpRequest.Form.Get("namez")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("namez") == null ? "必須有namez欄位" : "namez參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("gender")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("gender") == null ? "必須有gender欄位" : "gender參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("convertz")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("convertz") == null ? "必須有convertz欄位" : "convertz參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("birthz")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("birthz") == null ? "必須有birthz欄位" : "birthz參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("affiliatedAreaz")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("affiliatedAreaz") == null ? "必須有affiliatedAreaz欄位" : "affiliatedAreaz參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("residenceAddress")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("residenceAddress") == null ? "必須有residenceAddress欄位" : "residenceAddress參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("contactAddress")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("contactAddress") == null ? "必須有contactAddress欄位" : "contactAddress參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("contactNumber")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("contactNumber") == null ? "必須有contactNumber欄位" : "contactNumber參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("email")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("email") == null ? "必須有email欄位" : "email參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("sendCertificate")))  //是否寄發證書
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("sendCertificate") == null ? "必須有sendCertificate欄位" : "sendCertificate參數格式錯誤"));
                }
                if (httpRequest.Form.Get("sendCertificate") == "1")  //要寄發證書時
                {
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("sendAddress")))  //寄件地址需填
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("sendAddress") == null ? "必須有sendAddress欄位" : "sendAddress參數格式錯誤"));
                    }
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled欄位" : "enabled參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("audit")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("audit") == null ? "必須有audit欄位" : "audit參數格式錯誤"));
                }
                ConvertForms convertForms = _convertFormsService.InsertConvertForms(httpRequest);
                // AutoMapper轉換型別
                //宣告轉成Dto型別 dto = 轉成<Dto型別>(要轉換的類別)
                ConvertFormsDto convertFormsDto = _mapper.Map<ConvertFormsDto>(convertForms);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, convertFormsDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // GET: api/convertForms/{convertFormsId}
        //皈戒申請屬於個人資料需權限
        public IHttpActionResult GetConvertForms(Guid id)
        {
            ConvertForms convertForms = _convertFormsService.GetConvertForms(id);
            if (convertForms != null)
            {
                ConvertFormsDto convertFormsDto = _mapper.Map<ConvertFormsDto>(convertForms);
                return Ok(convertFormsDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 更新一筆資料
        // PUT: api/convertForms/{convertFormsId}
        [HttpPut]
        public IHttpActionResult UpdateConvertForms(Guid id)
        {
            try
            {
                //1.先取出要修改的資料
                ConvertForms convertForms = _convertFormsService.GetConvertForms(id);
                //2.辨斷有無資料，並辨斷接收進來的資料
                if (convertForms != null)
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("namez")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("namez") == null ? "必須有namez欄位" : "namez參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("gender")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("gender") == null ? "必須有gender欄位" : "gender參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("convertz")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("convertz") == null ? "必須有convertz欄位" : "convertz參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("birthz")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("birthz") == null ? "必須有birthz欄位" : "birthz參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("affiliatedAreaz")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("affiliatedAreaz") == null ? "必須有affiliatedAreaz欄位" : "affiliatedAreaz參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("residenceAddress")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("residenceAddress") == null ? "必須有residenceAddress欄位" : "residenceAddress參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("contactAddress")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("contactAddress") == null ? "必須有contactAddress欄位" : "contactAddress參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("contactNumber")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("contactNumber") == null ? "必須有contactNumber欄位" : "contactNumber參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("email")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("email") == null ? "必須有email欄位" : "email參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("sendCertificate")))  //是否寄發證書
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("sendCertificate") == null ? "必須有sendCertificate欄位" : "sendCertificate參數格式錯誤"));
                    }
                    if (httpRequest.Form.Get("sendCertificate") == "1")  //要寄發證書時
                    {
                        if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("sendAddress")))  //寄件地址需填
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("sendAddress") == null ? "必須有sendAddress欄位" : "sendAddress參數格式錯誤"));
                        }
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled欄位" : "enabled參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("audit")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("audit") == null ? "必須有audit欄位" : "audit參數格式錯誤"));
                    }
                    //修改
                    _convertFormsService.UpdateConvertForms(httpRequest, convertForms);
                    //回傳
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 刪除一筆資料
        // DELETE: api/convertForms/{id}
        [HttpDelete]
        public IHttpActionResult DeleteConvertForms(Guid id)
        {
            ConvertForms convertForms = _convertFormsService.GetConvertForms(id);
            if (convertForms != null)
            {
                _convertFormsService.DeleteConvertForms(convertForms);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: api/convertForms
        public IHttpActionResult GetConvertFormsAll()
        {
            HttpRequest request = HttpContext.Current.Request;
            int? _count = null;
            int? _page = null;
            if (!string.IsNullOrWhiteSpace(request["count"]))
            {
                _count = Convert.ToInt32(request["count"]);
            }
            if (!string.IsNullOrWhiteSpace(request["page"]))
            {
                _page = Convert.ToInt32(request["page"]);
            }
            List<ConvertForms> convertFormsAll = _convertFormsService.GetConvertFormsAll(_count, _page);
            if (convertFormsAll.Count > 0)
            {
                List<ConvertFormsDto> convertFormsAllDtos = _mapper.Map<List<ConvertFormsDto>>(convertFormsAll);
                return Ok(convertFormsAllDtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region  取得皈戒申請總筆數
        // GET: api/convertForms/totalCount
        [Route("convertForms/totalCount")]
        public IHttpActionResult GetConvertFormsTotalCount()
        {
            int count = _convertFormsService.GetConvertFormsTotalCount();
            if (count >= 0)
            {                
                return Ok(new { totalCount = count });
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region  更新皈戒申請的審核
        // GET: api/convertForms/updateAuditOption/{id}
        [Route("convertForms/updateAuditOption/{id}")]
        [HttpPut]
        public IHttpActionResult GetConvertFormsUpdateAuditOption(Guid id)
        {
            try
            {
                //1.先取出要修改的資料
                ConvertForms convertForms = _convertFormsService.GetConvertForms(id);
                //2.辨斷有無資料，並辨斷接收進來的資料
                if (convertForms != null)
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("audit")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("audit") == null ? "必須有audit欄位" : "audit參數格式錯誤"));
                    }
                    //修改
                    _convertFormsService.UpdateAuditOption(httpRequest, convertForms);
                    //回傳
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return NotFound();
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
