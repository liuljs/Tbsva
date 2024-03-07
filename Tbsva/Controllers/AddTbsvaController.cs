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

    public class AddTbsvaController : ApiController
    {
        #region DI依賴注入功能
        private IAddTbsvaService _addTbsvaService;
        private IMapper _mapper;
        public AddTbsvaController(IAddTbsvaService addTbsvaService, IMapper mapper)
        {
            _addTbsvaService = addTbsvaService;
            _mapper = mapper;
        }
        #endregion

        #region 新增
        // POST: api/addTbsva
        [HttpPost]
        public IHttpActionResult InsertAddTbsva()
        {
            try
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
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("birthz")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("birthz") == null ? "必須有birthz欄位" : "birthz參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("affiliatedAreaz")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("affiliatedAreaz") == null ? "必須有affiliatedAreaz欄位" : "affiliatedAreaz參數格式錯誤"));
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
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled欄位" : "enabled參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("audit")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("audit") == null ? "必須有audit欄位" : "audit參數格式錯誤"));
                }
                AddTbsva addTbsva = _addTbsvaService.InsertAddTbsva(httpRequest);
                // AutoMapper轉換類別
                //宣告轉出Dto類別 dto = _mapper.Map.轉出<Dto類別>(要轉換的類別)
                AddTbsvaDto addTbsvaDto = _mapper.Map<AddTbsvaDto>(addTbsva);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, addTbsvaDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // GET: api/addTbsva/{addTbsvaId}
        public IHttpActionResult GetAddTbsva(Guid id)
        {
            AddTbsva addTbsva = _addTbsvaService.GetAddTbsva(id);
            if (addTbsva != null)
            {
                AddTbsvaDto addTbsvaDto = _mapper.Map<AddTbsvaDto>(addTbsva);
                return Ok(addTbsvaDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 更新一筆資料
        // PUT: api/addTbsva/{addTbsvaId}
        [HttpPut]
        public IHttpActionResult UpdateAddTbsva(Guid id)
        {
            try
            {
                //1.先取出要修改的資料
                AddTbsva addTbsva = _addTbsvaService.GetAddTbsva(id);
                //2.辨斷有無資料，並辨斷接收進來的資料
                if (addTbsva != null)
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
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("birthz")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("birthz") == null ? "必須有birthz欄位" : "birthz參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("affiliatedAreaz")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("affiliatedAreaz") == null ? "必須有affiliatedAreaz欄位" : "affiliatedAreaz參數格式錯誤"));
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
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled欄位" : "enabled參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("audit")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("audit") == null ? "必須有audit欄位" : "audit參數格式錯誤"));
                    }
                    //修改
                    _addTbsvaService.UpdateAddTbsva(httpRequest, addTbsva);
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
        // DELETE: api/addTbsva/{addTbsvaId}
        [HttpDelete]
        public IHttpActionResult DeleteAddTbsva(Guid id)
        {
            AddTbsva addTbsva = _addTbsvaService.GetAddTbsva(id);
            if (addTbsva !=null)
            {
                _addTbsvaService.DeleteAddTbsva(addTbsva);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: api/AddTbsva/
        [AllowAnonymous]
        public IHttpActionResult GetAddTbsvaAll()
        {
            HttpRequest request = HttpContext.Current.Request;
            string _namez = string.Empty;
            int? _count = null;
            int? _page = null;
            if (!string.IsNullOrWhiteSpace(request["namez"]))
            {
                _namez = request["namez"].ToString().Trim();
            }
            if (!string.IsNullOrWhiteSpace(request["count"]))
            {
                _count = Convert.ToInt32(request["count"]);
            }
            if (!string.IsNullOrWhiteSpace(request["page"]))
            {
                _page = Convert.ToInt32(request["page"]);
            }
            List<AddTbsva> addTbsvasAll = _addTbsvaService.GetAddTbsvasAll(_namez, _count, _page);
            if (addTbsvasAll.Count > 0)
            {
                List<AddTbsvaDto> addTbsvasAllDtos = _mapper.Map<List<AddTbsvaDto>>(addTbsvasAll);
                return Ok(addTbsvasAllDtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region  取得加入台密總總筆數
        // GET: api/addTbsva/totalCount
        [Route("addTbsva/totalCount")]
        public IHttpActionResult GetaddTbsvaTotalCount()
        {
            int count = _addTbsvaService.GetAddTbsvaTotalCount();
            if (count >=0)
            {
                return Ok(new { totalCount = count });
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region  更新加入台密總的審核
        // GET: api/addTbsva/updateAuditOption/{addTbsvaId}
        [Route("addTbsva/updateAuditOption/{id}")]
        [HttpPut]
        public IHttpActionResult GetAddTbsvaUpdateAuditOption(Guid id)
        {
            try
            {
                //1.先取出要修改的資料
                AddTbsva addTbsva = _addTbsvaService.GetAddTbsva(id);
                //2.辨斷有無資料，並辨斷接收進來的資料
                if (addTbsva != null)
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("audit")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("audit") == null ? "必須有audit欄位" : "audit參數格式錯誤"));
                    }
                    //修改
                    _addTbsvaService.UpdateAuditOption(httpRequest, addTbsva);
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
