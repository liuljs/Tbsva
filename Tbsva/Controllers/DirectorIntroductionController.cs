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

namespace WebShopping.Services
{
    [CustomAuthorize(Role.Admin)]
    public class DirectorIntroductionController : ApiController
    {
        #region DI依賴注入功能
        private IDirectorIntroductionService _directorIntroductionService;
        private IMapper _mapper;
        private IImageFormatHelper _imageFormatHelper;
        private IRepeatLimit _repeatLimit;

        public DirectorIntroductionController(IDirectorIntroductionService directorIntroductionService, IMapper mapper, IImageFormatHelper imageFormatHelper, IRepeatLimit repeatLimit)
        {
            _directorIntroductionService = directorIntroductionService;
            _mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
            _repeatLimit = repeatLimit;
        }
        #endregion

        #region 新增一筆資料
        // POST: api/directorIntroduction/
        [HttpPost]
        public IHttpActionResult InsertDirectorIntroduction()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;  //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(request.Form.Get("name")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("subtitle")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("subtitle") == null ? "必須有subtitle" : "subtitle參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("brief")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("content")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("first")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                }
                //first(只能開啟一個置頂), 新增時若選開啟時需辨斷資料庫是否已經有存在1筆，若有則不能再新增
                DirectorIntroduction directorIntroduction = new DirectorIntroduction();  //先建一個空的
                if (request.Form.Get("first") == "1")
                {
                    string _tableName = "[DIRECTOR_INTRODUCTION]";

                    if (MessageOnlyOne.FirstOnlyOne == _repeatLimit.messageOnlyOne(_tableName, directorIntroduction, ""))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, MessageOnlyOne.FirstOnlyOne.ToString()));
                    }
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                }
                if (request.Form.Get("cNameArr") == null)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有cNameArr參數"));
                }
                #region 圖檔檢驗(需上傳)
                if (request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                }
                if (request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_imageFormatHelper.CheckImageMIME(request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion
                directorIntroduction = _directorIntroductionService.InsertDirectorIntroduction(request);    //透過介面.實作
                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉成<Dto型別>(要轉換的類別)
                DirectorIntroductionDto directorIntroductionDto = _mapper.Map<DirectorIntroductionDto>(directorIntroduction);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, directorIntroductionDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // Get: api/directorIntroduction/{directorIntroductionId}
        [AllowAnonymous]
        public IHttpActionResult GetDirectorIntroduction(Guid id)
        {
            DirectorIntroduction directorIntroduction = _directorIntroductionService.GetDirectorIntroduction(id);
            if (directorIntroduction != null)
            {
                DirectorIntroductionDto directorIntroductionDto = _mapper.Map<DirectorIntroductionDto>(directorIntroduction);
                return Ok(directorIntroductionDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        // PUT: api/directorIntroduction/{directorIntroductionId}
        [HttpPut]
        public IHttpActionResult UpdateDirectorIntroduction(Guid id)
        {
            try
            {
                DirectorIntroduction directorIntroduction = _directorIntroductionService.GetDirectorIntroduction(id);         //先取出要修改的資料
                if (directorIntroduction != null)
                {
                    HttpRequest request = HttpContext.Current.Request;  //取得使用者要求的Request物件
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(request.Form.Get("name")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("subtitle")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("subtitle") == null ? "必須有subtitle" : "subtitle參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("brief")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("content")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("first")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                    }
                    //first(只能開啟一個置頂), 新增時若選開啟時需辨斷資料庫是否已經有存在1筆，若有則不能再新增
                    if (request.Form.Get("first") == "1")
                    {
                        string _tableName = "[DIRECTOR_INTRODUCTION]";

                        if (MessageOnlyOne.FirstOnlyOne == _repeatLimit.messageOnlyOne(_tableName, directorIntroduction, ""))
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, MessageOnlyOne.FirstOnlyOne.ToString()));
                        }
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("sort") == null ? "必須有sort參數" : "sort參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }
                    #region 圖檔檢驗(非必選，沒上傳則不檢查)
                    //修改時上傳圖檔非必選，因為上傳時已必傳上傳了
                    if (request.Files.Count > 0)      //檢查有沒有上傳圖片的欄位選項，因為要有欄位時_request.Files[0]才不會錯
                    {
                        if (request.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳，不然CheckImageMIME會出錯
                        {
                            if (_imageFormatHelper.CheckImageMIME(request.Files) == false)  //這裏若file欄裏面沒有選檔案時CheckImageMIME還是會進入檢查, 除非將file欄不勾選
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                            }
                        }
                    }
                    #endregion
                    //修改
                    _directorIntroductionService.UpdateDirectorIntroduction(request, directorIntroduction);
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
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}"); //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 刪除
        // DELETE: api/directorIntroduction/{directorIntroductionId}
        [HttpDelete]
        public IHttpActionResult DeleteDirectorIntroduction(Guid id)
        {
            DirectorIntroduction directorIntroduction = _directorIntroductionService.GetDirectorIntroduction(id);
            if (directorIntroduction != null)
            {
                _directorIntroductionService.DeleteDirectorIntroduction(directorIntroduction);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: api/directorIntroduction/
        [AllowAnonymous]
        public IHttpActionResult GetDirectorIntroduction()
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

            List<DirectorIntroduction> directorIntroductions = _directorIntroductionService.GetDirectorIntroductions(_count, _page);
            if (directorIntroductions.Count > 0)
            {
                List<DirectorIntroductionDto> directorIntroductionDtos = _mapper.Map<List<DirectorIntroductionDto>>(directorIntroductions);
                return Ok(directorIntroductionDtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 文字編輯器內容區插入一個圖片(新增與修改時)
        // POST: api/directorIntroduction/image
        [HttpPost]
        [Route("directorIntroduction/image")]
        public IHttpActionResult image()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;  //取得使用者要求的Request物件
                #region 圖檔檢驗(需上傳)
                if (request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                }
                if (request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_imageFormatHelper.CheckImageMIME(request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion

                DirectorIntroductionImage directorIntroductionImage = _directorIntroductionService.AddUploadImage(request);
                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                DirectorIntroductionImageDto directorIntroductionImageDto = _mapper.Map<DirectorIntroductionImageDto>(directorIntroductionImage);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, directorIntroductionImageDto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        // POST: api/directorIntroduction/image/{directorIntroductionId}
        [HttpPost]
        [Route("directorIntroduction/image/{id}")]
        public IHttpActionResult image(Guid id)
        {
            try
            {
                DirectorIntroduction directorIntroduction = _directorIntroductionService.GetDirectorIntroduction(id);
                if (directorIntroduction != null)
                {
                    HttpRequest request = HttpContext.Current.Request;
                    #region 圖檔檢驗(需上傳)
                    if (request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                    }
                    if (request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                    }
                    if (_imageFormatHelper.CheckImageMIME(request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }
                    #endregion

                    DirectorIntroductionImage directorIntroductionImage = _directorIntroductionService.AddUploadImage(request, id);

                    // AutoMapper轉換型別
                    //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                    DirectorIntroductionImageDto directorIntroductionImageDto = _mapper.Map<DirectorIntroductionImageDto>(directorIntroductionImage);

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, directorIntroductionImageDto));
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
