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
    [CustomAuthorize(Role.Admin)]
    public class TimeMachineController : ApiController
    {
        #region DI依賴注入功能
        private ITimeMachineService _machineService;
        private IMapper _mapper;
        private IImageFormatHelper _imageFormatHelper;

        public TimeMachineController(ITimeMachineService machineService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            _machineService = machineService;
            _mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增一筆資料
        // POST: api/timeMachine/
        [HttpPost]
        public IHttpActionResult InsertTimeMachine()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;  //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(request.Form.Get("name")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("course")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("course") == null ? "必須有course" : "course參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("brief")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("first")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                }

                //檢查若所有上傳檔案的MIME格式
                if (_imageFormatHelper.CheckImageMIME2(request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }

                //新增
                TimeMachine timeMachine = _machineService.InsertTimeMachine(request);  //介面=>實作

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉成<Dto類別>(要轉換的類別)
                TimeMachineDto timeMachineDto = _mapper.Map<TimeMachineDto>(timeMachine);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, timeMachineDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // POST: api/timeMachine/{timeMachineId}
        [AllowAnonymous]
        public IHttpActionResult GetTimeMachine(Guid ID)
        {
            TimeMachine timeMachine = _machineService.GetTimeMachine(ID);
            if (timeMachine != null)
            {
                TimeMachineDto timeMachineDto = _mapper.Map<TimeMachineDto>(timeMachine);
                return Ok(timeMachineDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        // Put: api/timeMachine/{timeMachineId}
        [HttpPut]
        public IHttpActionResult UpdateTimeMachine(Guid id)
        {
            try
            {
                TimeMachine timeMachine = _machineService.GetTimeMachine(id);         //先取出要修改的資料
                if (timeMachine != null)
                {
                    HttpRequest request = HttpContext.Current.Request;  //取得使用者要求的Request物件
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值 
                    if (string.IsNullOrWhiteSpace(request.Form.Get("name")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("course")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("course") == null ? "必須有course" : "course參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("brief")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("first")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("sort") == null ? "必須有sort參數" : "sort參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }

                    //檢查若所有上傳檔案的MIME格式
                    if (_imageFormatHelper.CheckImageMIME2(request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }

                    //修改
                    _machineService.UpdateTimeMachine(request, timeMachine);
                    //回傳
                    return StatusCode(HttpStatusCode.NoContent);   //PUT 成功204沒有內容
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}"); //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 刪除
        // DELETE: api/timeMachine/{timeMachineId}
        [HttpDelete]
        public IHttpActionResult DeleteTimeMachine(Guid id)
        {
            TimeMachine timeMachine = _machineService.GetTimeMachine(id);
            if (timeMachine != null)
            {
                _machineService.DeleteTimeMachine(timeMachine);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得分頁所有內容
        // GET: api/timeMachine/
        [AllowAnonymous]
        public IHttpActionResult GetTimeMachines()
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

            List<TimeMachine> timeMachines = _machineService.GetTimeMachines(_count, _page);
            if (timeMachines.Count > 0)
            {
                List<TimeMachineDto> timeMachinesDto = _mapper.Map<List<TimeMachineDto>>(timeMachines);
                return Ok(timeMachinesDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

    }
}
