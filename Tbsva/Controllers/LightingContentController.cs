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
    public class LightingContentController : ApiController
    {
        #region DI依賴注入功能
        private ILightingContentService _ILightingContentService;  //欄位
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;

        /// <summary>
        ///  DI依賴注入功能
        /// </summary>
        /// <param name="lightingContentService">內容輸入</param>
        /// <param name="mapper">AutoMapper</param>
        /// <param name="imageFormatHelper">圖檔格式</param>
        public LightingContentController(ILightingContentService lightingContentService , IMapper mapper, IImageFormatHelper imageFormatHelper )
        {
            _ILightingContentService = lightingContentService;  //參數
            m_Mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增
        // POST: /LightingContent
        [HttpPost]
        public IHttpActionResult InsertLightingContent()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(_request.Form.Get("Lighting_category_id")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Lighting_category_id") == null ? "必須有關聯目錄Lighting_category_id" : "Lighting_category_id參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("title")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("title") == null ? "必須有title" : "title參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("brief")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content" : "content參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                }
                //if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                //}
                if (string.IsNullOrWhiteSpace(_request.Form.Get("first")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                }
                if (_request.Form.Get("fNameArr") == null)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有fNameArr參數"));
                }
                if (_request.Files.Count == 0)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_imageFormatHelper.CheckImageMIME(_request.Files) == false)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                Lighting_content _lighting_Content = _ILightingContentService.Insert_Lighting_content(_request);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                Lighting_content_Dto _lighting_Content_Dto = m_Mapper.Map<Lighting_content_Dto>(_lighting_Content);

                //return Request.CreateErrorResponse這裏弄錯時會出現引數3無法從轉換string
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _lighting_Content_Dto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // GET: /LightingContent/5
        [AllowAnonymous]
        public IHttpActionResult GetLightingContent(Guid id)
        {
            Lighting_content _lighting_Content = _ILightingContentService.Get_Lighting_content(id);
            if (_lighting_Content != null)
            {
                Lighting_content_Dto _lighting_Content_Dto = m_Mapper.Map<Lighting_content_Dto>(_lighting_Content); // 轉換型別_lighting_Content=>_lighting_Content_Dto
                return Ok(_lighting_Content_Dto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改一筆資料
        // PUT: /LightingContent/5
        [HttpPut]
        public IHttpActionResult UpdateLightingContent(Guid id)
        {
            try
            {
                //1.先取出要修改的資料
                Lighting_content _lighting_Content = _ILightingContentService.Get_Lighting_content(id);
                //2.辨斷有無資料，並辨斷接收進來的資料
                if (_lighting_Content != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Lighting_category_id")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Lighting_category_id") == null ? "必須有關聯目錄Lighting_category_id" : "Lighting_category_id參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("title")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("title") == null ? "必須有title" : "title參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("brief")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content" : "content參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                    }
                    //if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                    //{
                    //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                    //}
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("first")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                    }
                    //if (_request.Form.Get("fNameArr") == null)
                    //{
                    //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有fNameArr參數"));
                    //}
                    //if (_request.Files.Count == 0)
                    //{
                    //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                    //}
                    //沒有上傳圖片時就不會檢查
                    if (_imageFormatHelper.CheckImageMIME(_request.Files) == false)
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }

                    //修改
                    _ILightingContentService.Update_Lighting_content(_request, _lighting_Content);

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
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 刪除內容與內容圖片
        // DELETE: /LightingContent/5
        /// <summary>
        /// 刪除內容與內容圖片
        /// </summary>
        /// <param name="id">要刪除的Guid id</param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult DeleteLightingContent(Guid id)
        {
            Lighting_content _lighting_Content = _ILightingContentService.Get_Lighting_content(id);  //先取出內容資料
            if (_lighting_Content != null)
            {
                _ILightingContentService.Delete_Lighting_content(_lighting_Content);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: /LightingContent
        [AllowAnonymous]
        public IHttpActionResult GetLightingContent()
        {
            HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件

            int? _Lighting_category_id = null;
            int? _count = null;
            int? _page = null;
            string _keyword = string.Empty;

            if (!string.IsNullOrWhiteSpace(_request["Lighting_category_id"]))
            {
                _Lighting_category_id = Convert.ToInt32(_request["Lighting_category_id"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["count"]))
            {
                _count = Convert.ToInt32(_request["count"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["page"]))
            {
                _page = Convert.ToInt32(_request["page"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["keyword"]))
            {
                _keyword = _request["keyword"].ToString();
            }

            List<Lighting_content> _lighting_Contents = _ILightingContentService.Get_Lighting_content_ALL(_Lighting_category_id, _count, _page, _keyword);

            if (_lighting_Contents.Count > 0)
            {
                List<Lighting_content_Dto> _lighting_Content_Dtos = m_Mapper.Map<List<Lighting_content_Dto>>(_lighting_Contents);  // 多筆轉換型別
                return Ok(_lighting_Content_Dtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion



        #region 內容區插入一個圖片(新增與修改時)
        /// <summary>
        /// 內容新增插入一個圖片:新增時,在內容裏(加入一個內容圖片)
        /// </summary>
        /// <returns>201;圖片URL</returns>
        [HttpPost]
        [Route("LightingContent/AddContentImage")]
        public IHttpActionResult AddContentImage()
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

                string _imageUrl = _ILightingContentService.AddUploadImage(_request);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new { Image_Url = _imageUrl }));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// 內容新增插入一個圖片:(修改)時(加入一個內容圖片)
        /// </summary>
        /// <param name="id">內容id</param>
        /// <returns>201;圖片URL</returns>
        [HttpPost]
        [Route("LightingContent/AddContentImage/{id}")]
        public IHttpActionResult AddContentImage(Guid id)
        {
            try
            {
                Lighting_content _lighting_Content = _ILightingContentService.Get_Lighting_content(id);
                if (_lighting_Content != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;
                    if (_request.Files.Count == 0)
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                    }
                    if (_imageFormatHelper.CheckImageMIME(_request.Files) == false)
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }

                    string _imageUrl = _ILightingContentService.AddUploadImage(_request, id);

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new { Image_Url = _imageUrl }));
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
