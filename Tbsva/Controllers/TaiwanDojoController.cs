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
    public class TaiwanDojoController : ApiController
    {
        #region DI依賴注入功能
        private ITaiwanDojoService _taiwanDojoService;
        private IMapper _mapper;
        private IImageFormatHelper _imageFormatHelper;
        /// <summary>
        /// DI依賴注入功能
        /// </summary>
        /// <param name="taiwanDojoService">內容</param>
        /// <param name="mapper">自動轉型</param>
        /// <param name="imageFormatHelper">透過MIME檢查上傳檔案是否為圖檔</param>
        public TaiwanDojoController(ITaiwanDojoService taiwanDojoService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            _taiwanDojoService = taiwanDojoService;
            _mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增一筆資料
        // POST: api/taiwanDojo
        [HttpPost]
        public IHttpActionResult InsertTaiwanDojo()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;    //取得使用者要求的Request物件
                if (string.IsNullOrWhiteSpace(request.Form.Get("categoryId")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("categoryId") == null ? "必須有關聯目錄categoryId" : "categoryId參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("name")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("latitude")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("latitude") == null ? "必須有latitude" : "latitude參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("longitude")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("longitude") == null ? "必須有longitude" : "longitude參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("icon")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("icon") == null ? "必須有icon" : "icon參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("first")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                }
                if (request.Form.Get("cNameArr") == null)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有cNameArr參數"));
                }

                TaiwanDojo taiwanDojo = _taiwanDojoService.InsertTaiwanDojo(request);
                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換成<Dto型別>(要被轉換的類別)
                TaiwanDojoDto taiwanDojoDto = _mapper.Map<TaiwanDojoDto>(taiwanDojo);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, taiwanDojoDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");  //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // Get : api/taiwanDojo/{taiwanDojoId}
        [AllowAnonymous]
        //[Route("taiwanDojo/{taiwanDojoId}")]  //<=這裏不能設定路由會與取得全部目錄 taiwanDojo/category/ 發生錯誤，找到多個與 URL 相符的控制器
        public IHttpActionResult GetGetTaiwanDojo(Guid id)
        {
            TaiwanDojo taiwanDojo = _taiwanDojoService.GetTaiwanDojo(id);
            if (taiwanDojo != null)
            {
                TaiwanDojoDto taiwanDojoDto = _mapper.Map<TaiwanDojoDto>(taiwanDojo);
                return Ok(taiwanDojoDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        // PUT: api/taiwanDojo/{taiwanDojoId}
        [HttpPut]
        //[Route("taiwanDojo/{taiwanDojoId}")]
        public IHttpActionResult UpdateTaiwanDojo(Guid id)
        {
            try
            {
                TaiwanDojo taiwanDojo = _taiwanDojoService.GetTaiwanDojo(id);
                if (taiwanDojo != null)
                {
                    HttpRequest request = HttpContext.Current.Request;
                    if (string.IsNullOrWhiteSpace(request.Form.Get("categoryId")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("categoryId") == null ? "必須有關聯目錄categoryId" : "categoryId參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("name")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("latitude")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("latitude") == null ? "必須有latitude" : "latitude參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("longitude")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("longitude") == null ? "必須有longitude" : "longitude參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("icon")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("icon") == null ? "必須有icon" : "icon參數格式錯誤"));
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
                    //修改
                    _taiwanDojoService.UpdateTaiwanDojo(request, taiwanDojo);
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
        // DELETE: api/taiwanDojo/{taiwanDojoId}
        [HttpDelete]
        //[Route("taiwanDojo/{taiwanDojoId}")]
        public IHttpActionResult DeleteTaiwanDojo(Guid id)
        {
            TaiwanDojo taiwanDojo = _taiwanDojoService.GetTaiwanDojo(id);
            if (taiwanDojo != null)
            {
                _taiwanDojoService.DeleteTaiwanDojo(taiwanDojo);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: api/taiwanDojo/
        // ?{categoryId="活動隸屬目錄 ID"}&{count="每頁一次渲染的消息筆數"}&{page="目前的頁碼"}
        [AllowAnonymous]
        public IHttpActionResult GetTaiwanDojos()
        {
            HttpRequest request = HttpContext.Current.Request;   //取得使用者要求的Request物件
            string categoryId = null;
            int? count = null;
            int? page = null;

            if (!string.IsNullOrWhiteSpace(request["categoryId"]))
            {
                categoryId = request["categoryId"];
            }
            if (!string.IsNullOrWhiteSpace(request["count"]))
            {
                count = Convert.ToInt32(request["count"]);
            }
            if (!string.IsNullOrWhiteSpace(request["page"]))
            {
                page = Convert.ToInt32(request["page"]);
            }

            List<TaiwanDojo> taiwanDojos = _taiwanDojoService.GetTaiwanDojos(categoryId, count, page);
            if (taiwanDojos.Count > 0)
            {
                List<TaiwanDojoDto> taiwanDojoDtos = _mapper.Map<List<TaiwanDojoDto>>(taiwanDojos); // 多筆轉換型別
                return Ok(taiwanDojoDtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 文字編輯器內容區插入一個圖片(新增與修改時)
        // POST: api/taiwanDojo/image
        /// <summary>
        /// 新增內容圖片
        /// 內容新增插入一個圖片:新增時,在內容裏(加入一個內容圖片)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("taiwanDojo/image")]
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

                TaiwanDojoImage taiwanDojoImage = _taiwanDojoService.AddUploadImage(request);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                TaiwanDojoImageDto taiwanDojoImageDto = _mapper.Map<TaiwanDojoImageDto>(taiwanDojoImage);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, taiwanDojoImageDto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        // POST: api/taiwanDojo/image/{taiwanDojoId}
        /// <summary>
        /// 更新的內容圖片
        /// 內容新增插入一個圖片:新增時,在內容裏(加入一個內容圖片)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("taiwanDojo/image/{id}")]
        public IHttpActionResult image(Guid id)
        {
            try
            {
                TaiwanDojo taiwanDojo = _taiwanDojoService.GetTaiwanDojo(id);
                if (taiwanDojo != null)
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

                    TaiwanDojoImage taiwanDojoImage = _taiwanDojoService.AddUploadImage(request, id);

                    // AutoMapper轉換型別
                    //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                    TaiwanDojoImageDto taiwanDojoImageDto = _mapper.Map<TaiwanDojoImageDto>(taiwanDojoImage);

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, taiwanDojoImageDto));
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
