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
    public class ArticleContentController : ApiController
    {
        #region DI依賴注入功能
        private readonly IArticleContentService _IArticleContentService;
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;

        /// <summary>
        /// DI依賴注入功能
        /// </summary>
        /// <param name="IArticleContentService">文章介面</param>
        /// <param name="mapper">自動轉型</param>
        /// <param name="IImageFormatHelper">透過MIME檢查上傳檔案是否為圖檔</param>
        public ArticleContentController(IArticleContentService IArticleContentService, IMapper mapper, IImageFormatHelper IImageFormatHelper)
        {
            _IArticleContentService = IArticleContentService;
            m_Mapper = mapper;
            _imageFormatHelper = IImageFormatHelper;
        }
        #endregion

        #region 新增一筆文章
        // POST: /ArticleContent/ => {controller}
        [HttpPost]
        public IHttpActionResult InsertArticleContent()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(_request.Form.Get("articleCategoryId")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("articleCategoryId") == null ? "必須有articleCategoryId目錄的ID欄位" : "articleCategoryId參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("title")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("title") == null ? "必須有title欄位" : "title參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("subtitle")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("subtitle") == null ? "必須有subtitle欄位" : "subtitle參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("brief")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                }
                //if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                //}
                if (string.IsNullOrWhiteSpace(_request.Form.Get("first")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                }
                if (_request.Form.Get("cNameArr") == null)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有cNameArr參數"));
                }
                #region 圖檔檢驗(需上傳)
                if (_request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                }
                if (_request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_imageFormatHelper.CheckImageMIME(_request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion

                article_content _article_content = _IArticleContentService.Insert_article_content(_request);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
                article_content_Dto _article_content_Dto = m_Mapper.Map<article_content_Dto>(_article_content);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _article_content_Dto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");  //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }

        }
        #endregion

        #region 取得一筆文章
        // GET: /ArticleContent/5
        [AllowAnonymous]
        public IHttpActionResult GetArticleContent(Guid id)
        {
            article_content _article_content = _IArticleContentService.Get_article_content(id);
            if ( _article_content != null )
            {
                article_content_Dto _article_content_Dto = m_Mapper.Map<article_content_Dto>(_article_content); // 轉換型別
                return Ok(_article_content_Dto);
            }
            else
            {
                return NotFound();      //找不到資料會傳404 NotFound
            }
        }
        #endregion

        #region 修改一筆文章
        // PUT: /ArticleContent/5
        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="id">輸入文章id編號</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        [HttpPut]
        public IHttpActionResult UpdateArticleContent(Guid id)
        {
            try
            {
                article_content _article_content = _IArticleContentService.Get_article_content(id);  //先取出要修改的資料

                if (_article_content != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("articleCategoryId")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("articleCategoryId") == null ? "必須有articleCategoryId目錄的ID欄位" : "articleCategoryId參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("title")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("title") == null ? "必須有title欄位" : "title參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("subtitle")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("subtitle") == null ? "必須有subtitle欄位" : "subtitle參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("brief")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }
                    //if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                    //{
                    //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                    //}
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("first")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                    }
                    //if (_request.Form.Get("cNameArr") == null)
                    //{
                    //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有cNameArr參數"));
                    //}
                    //if (_request.Files.Count == 0)
                    //{
                    //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                    //}
                    if (_imageFormatHelper.CheckImageMIME(_request.Files) == false)
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }

                    _IArticleContentService.Update_article_content(_request, _article_content);

                    return StatusCode(HttpStatusCode.NoContent);

                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");  //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 刪除文章與內容圖片
        // DELETE: /ArticleContent/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult DeleteArticleContent(Guid id)
        {
            article_content _article_content = _IArticleContentService.Get_article_content(id);
            if (_article_content !=null)
            {
                _IArticleContentService.Delete_article_content(_article_content);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else 
            {
                return NotFound();
            }

        }
        #endregion

        #region 取得所有文章
        // GET: /ArticleContent
        /// <summary>
        /// 取得全部文章(可加所屬目錄id,一頁要顯示幾筆,第幾頁開始)
        /// </summary>
        /// <returns>200,_article_content_Dto, 404</returns>
        [AllowAnonymous]
        public IHttpActionResult GetArticleContent()
        {
            HttpRequest _request = HttpContext.Current.Request;  //取得使用者要求的Request物件

            int? _article_category_id = null;
            int? _count = null;
            int? _page = null;

            if (!string.IsNullOrWhiteSpace(_request["articleCategoryId"]))
                _article_category_id = Convert.ToInt32(_request["articleCategoryId"]);

            if (!string.IsNullOrWhiteSpace(_request["count"])) 
                _count = Convert.ToInt32(_request["count"]);

            if (!string.IsNullOrWhiteSpace(_request["page"]))
                _page = Convert.ToInt32(_request["page"]);

            List<article_content> _article_content_ies = _IArticleContentService.Get_article_content_ALL(_article_category_id, _count, _page);

            if (_article_content_ies.Count > 0)
            {
               List<article_content_Dto> _article_content_Dto = m_Mapper.Map<List<article_content_Dto>>(_article_content_ies);  // 多筆轉換型別
               return Ok(_article_content_Dto);    
            }
            else
            {
                return NotFound();
            }
        }
        #endregion




        #region 文章內容區插入一個圖片
        /// <summary>
        /// 內容新增插入一個圖片:新增文章時,在內容裏(加入一個內容圖片)
        /// </summary>
        /// <returns>201;圖片URL</returns>
        [HttpPost]
        [Route("articleContent/image")]
        public IHttpActionResult AddContentImage()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件

                #region 圖檔檢驗(需上傳)
                if (_request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                }
                if (_request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_imageFormatHelper.CheckImageMIME(_request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion
                //string _imageUrl = _IArticleContentService.AddUploadImage(_request);
                article_image article_Image = _IArticleContentService.AddUploadImage(_request);
                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                article_image_Dto article_Image_Dto = m_Mapper.Map<article_image_Dto>(article_Image);
                //return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new { Image_Url = _imageUrl }));

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, article_Image_Dto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        /// <summary>
        /// 內容新增插入一個圖片:(修改文章)時(加入一個內容圖片)
        /// </summary>
        /// <param name="id">文章的id</param>
        /// <returns>201</returns>
        [HttpPost]
        [Route("articleContent/image/{id}")]
        public IHttpActionResult AddContentImage(Guid id)
        {
            try
            {
                article_content _article_content = _IArticleContentService.Get_article_content(id);
                if (_article_content != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                    #region 圖檔檢驗(需上傳)
                    if (_request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                    }
                    if (_request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                    }
                    if (_imageFormatHelper.CheckImageMIME(_request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }
                    #endregion

                    //string _imageUrl = _IArticleContentService.AddUpdateUploadImage(_request, id);
                    article_image article_Image = _IArticleContentService.AddUpdateUploadImage(_request, id);

                    // AutoMapper轉換型別
                    //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                    article_image_Dto article_Image_Dto = m_Mapper.Map<article_image_Dto>(article_Image);
                    //return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new { Image_Url = _imageUrl }));

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, article_Image_Dto));
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
