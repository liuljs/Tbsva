using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebShopping.Auth;
using WebShopping.Services;
using WebShopping.Models;
using WebShopping.Dtos;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 後台權限需登入設定Admin，全域
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    public class LightingCategoryController : ApiController
    {
        #region DI依賴注入功能
        /// <summary>
        /// DI依賴注入功能宣告
        /// </summary>
        private ILightingCategoryService _ILightingCategoryService;
        private IMapper m_Mapper;
        /// <summary>
        /// DI依賴注入功能
        /// </summary>
        /// <param name="lightingCategoryService"></param>
        /// <param name="mapper"></param>
        public LightingCategoryController(ILightingCategoryService lightingCategoryService, IMapper mapper)
        {
            _ILightingCategoryService = lightingCategoryService;
            m_Mapper = mapper;
        }
        #endregion

        #region 新增
        // POST: /LightingCategory
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201; 目錄資料</returns>
        [HttpPost]
        public IHttpActionResult InsertLightingCategory()
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
                if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                }
                //if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                //}

                Lighting_category _lighting_Category = _ILightingCategoryService.Insert_Lighting_category(_request);
                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
                Lighting_category_Dto _lighting_Category_Dto = m_Mapper.Map<Lighting_category_Dto>(_lighting_Category);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _lighting_Category_Dto));
            }
            catch (Exception ex)
            {
               // throw new Exception(ex.Message);
               return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // GET: /LightingCategory/5
        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="id">目錄id</param>
        /// <returns></returns>
        [AllowAnonymous]
        public IHttpActionResult GetLightingCategory(int id)
        {
            Lighting_category _lighting_Category = _ILightingCategoryService.Get_Lighting_category(id);
            if(_lighting_Category !=null)
            {
                Lighting_category_Dto _lighting_Category_Dto = m_Mapper.Map<Lighting_category_Dto>(_lighting_Category);
                return Ok(_lighting_Category_Dto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        // PUT: /LightingCategory/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult UpdateLightingCategory(int id)
        {
            try
            {
                Lighting_category _lighting_Category = _ILightingCategoryService.Get_Lighting_category(id);   //先取出要修改的資料
                if (_lighting_Category != null)
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
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                    }
                    _ILightingCategoryService.Update_Lighting_category(_request, _lighting_Category);

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
        // DELETE: /LightingCategory/5
        /// <summary>
        ///  刪除一筆資料
        /// </summary>
        /// <param name="id">輸入目錄id編號</param>
        /// <returns>成功204 , 失敗404</returns>
        [HttpDelete]
        public IHttpActionResult DeleteLightingCategory(int id)
        {
            try
            {
                Lighting_category _lighting_Category = _ILightingCategoryService.Get_Lighting_category(id);  //先取出要刪的資料
                if (_lighting_Category != null)
                {
                    _ILightingCategoryService.Delete_Lighting_category(_lighting_Category);
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

        #region 取得全部目錄
        // GET: /LightingCategory
        /// <summary>
        /// 取得全部目錄
        /// </summary>
        /// <returns>200,全部目錄</returns>
        [AllowAnonymous]
        public IHttpActionResult GetLightingCategory_ALL()
        {
            try
            {
                List<Lighting_category> _lighting_Categories = _ILightingCategoryService.Get_Lighting_category_ALL();
                if (_lighting_Categories !=null)
                {
                    // 多筆轉換型別
                    List<Lighting_category_Dto> _lighting_Categories_Dto = m_Mapper.Map<List<Lighting_category_Dto>>(_lighting_Categories);

                    return Ok(_lighting_Categories_Dto);
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
