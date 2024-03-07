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
using WebShopping.Models;
using WebShopping.Services;


namespace WebShopping.Controllers
{
    /// <summary>
    /// 後台權限需登入設定，全域
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    public class ArticleCategoryController : ApiController
    {
        #region DI依賴注入功能
        /// <summary>
        /// DI依賴注入功能宣告
        /// </summary>
        private readonly IArticleCategoryService _IArticleCategoryService;
        private IMapper m_Mapper;
        /// <summary>
        /// DI依賴注入功能
        /// </summary>
        /// <param name="IArticleCategoryService"></param>
        /// <param name="mapper"></param>
        public ArticleCategoryController(IArticleCategoryService IArticleCategoryService, IMapper mapper)
        {
            _IArticleCategoryService = IArticleCategoryService;
            m_Mapper = mapper;
        }
        #endregion

        #region 新增一筆文章目錄
        // POST: /ArticleCategory/ => {controller}
        /// <summary>
        /// 新增一筆目錄
        /// </summary>
        /// <returns>201; 目錄資料</returns>
        [HttpPost]
        public IHttpActionResult InsertArticleCategory()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(_request.Form.Get("name")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("name") == null ? "必須有name欄位" : "name參數格式錯誤"));
                }
                //if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                //}
                if (string.IsNullOrWhiteSpace(_request.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                }
                //if (string.IsNullOrWhiteSpace(_request.Form.Get("sort")))
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("sort") == null ? "必須有sort參數" : "sort參數格式錯誤"));
                //}

                article_category _article_category = _IArticleCategoryService.Insert_article_category(_request);

                //_article_category.enabled = true;

               ///一般轉型轉換，將上面資料庫抓出來的類別資料傳給要顯示的Dto類別
               //article_category_Dto _article_category_Dto = new article_category_Dto();
               //_article_category_Dto.id = _article_category.id;
               //_article_category_Dto.name = _article_category.name;
               //_article_category_Dto.content = _article_category.content;
               //_article_category_Dto.Enabled = Convert.ToByte(_article_category.Enabled);
               //_article_category_Dto.Sort = _article_category.Sort;

               // AutoMapper轉換型別
               //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
               article_category_Dto _article_category_Dto = m_Mapper.Map<article_category_Dto>(_article_category);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _article_category_Dto));

            }
            catch (Exception ex)  //資料加入時若錯誤以下會提示
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
                
            }
        }
        #endregion

        # region 取得一筆文章目錄
        // GET: /ArticleCategory/5
        /// <summary>
        /// 取得一筆文章目錄
        /// </summary>
        /// <param name="id">輸入文章目錄id編號</param>
        /// <returns>200 Ok,取得一筆目錄資料404 NotFound</returns>
        [AllowAnonymous]
        public IHttpActionResult GetArticleCategory(int id)
        {
            article_category _article_category = _IArticleCategoryService.Get_article_category(id);
            if ( _article_category != null )
            {
                article_category_Dto _article_category_Dto = m_Mapper.Map<article_category_Dto>(_article_category); // 轉換型別
                return Ok(_article_category_Dto);
            }
            else
            {
                return NotFound();      //找不到資料會傳404 NotFound
            }
        }
        #endregion

        #region 修改資料
        // PUT: /ArticleCategory/5
        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="id">輸入文章目錄id編號</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        [HttpPut]
        //public void Put(int id, [FromBody] string value)
        public IHttpActionResult UpdateArticleCategory(int id)
        {
            try
            {
                article_category _article_category = _IArticleCategoryService.Get_article_category(id);  //先取出要修改的資料

                if (_article_category != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("name")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("name") == null ? "必須有name欄位" : "name參數格式錯誤"));
                    }
                    //if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                    //{
                    //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                    //}
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("sort") == null ? "必須有sort參數" : "sort參數格式錯誤"));
                    }
                    //讀出_request Form裏的HasKeys值
                    //if (_request.Form.HasKeys())
                    //{
                    //    string param = string.Empty;
                    //    foreach (string key in _request.Form.AllKeys)
                    //    {
                    //        param += $"{key}={_request.Form[key]};";
                    //    }
                    //}
                    _IArticleCategoryService.Update_article_category(_request, _article_category);

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

        #region 刪除一筆資料
        // DELETE: /ArticleCategory/5
        /// <summary>
        /// 刪除一筆資料
        /// </summary>
        /// <param name="id">輸入文章目錄id編號</param>
        /// <returns>成功204 , 失敗404</returns>
        [HttpDelete]
        public IHttpActionResult DeleteArticleCategory(int id)
        {
            try
            {
                article_category _article_category = _IArticleCategoryService.Get_article_category(id);  //先取出要修改的資料

                if (_article_category != null)
                {
                    _IArticleCategoryService.Delete_article_category(_article_category);
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

        #region 取得所有文章目錄
        // GET: /ArticleCategory
        /// <summary>
        /// 取得全部文章目錄
        /// </summary>
        /// <returns>200,全部文章目錄</returns>
        [AllowAnonymous]
        public IHttpActionResult GetArticleCategory_ALL()
        {
            try
            {
                List<article_category> _article_Categories = _IArticleCategoryService.Get_article_category_ALL();

                if (_article_Categories !=null)
                {
                    //article_category_Dto _article_category_Dtoies = m_Mapper.Map<article_category_Dto>(_article_Categories); // 單筆轉換型別
                    List<article_category_Dto> _article_category_Dtoies = m_Mapper.Map<List<article_category_Dto>>(_article_Categories); // 多筆轉換型別

                    return Ok(_article_category_Dtoies);
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
