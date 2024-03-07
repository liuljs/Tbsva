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
    /// 後台權限需登入設定Admin，全域
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    ///RoutePrefix路由前綴
    [RoutePrefix("taiwanDojo")]
    public class TaiwanDojoCategoryController : ApiController
    {
        #region DI依賴注入功能
        private ITaiwanDojoCategoryService _taiwanDojoCategoryService;
        private IMapper _mapper;
        public TaiwanDojoCategoryController(ITaiwanDojoCategoryService taiwanDojoCategoryService, IMapper mapper)
        {
            _taiwanDojoCategoryService = taiwanDojoCategoryService;
            _mapper = mapper;
        }
        #endregion

        #region 新增
        // POST: api/taiwanDojo/category
        [HttpPost]
        [Route("category")]
        public IHttpActionResult GetTaiwanDojoCategory()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;
                if (string.IsNullOrWhiteSpace(request.Form.Get("name")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("name") == null ? "必須有name欄位" : "name參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(request.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    //CreateErrorResponse 沒有泛型會錯return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled參數" : new apiNewResultSuccess("400", "errorToBadRequest", "name參數格式錯誤" , "", DateTime.Now)));
                }
                TaiwanDojoCategory taiwanDojoCategory = _taiwanDojoCategoryService.InsertTaiwanDojoCategory(request);
                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
                TaiwanDojoCategoryDto taiwanDojoCategoryDto = _mapper.Map<TaiwanDojoCategoryDto>(taiwanDojoCategory);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, taiwanDojoCategoryDto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // Get: api/taiwanDojo/category/{categoryId}
        [AllowAnonymous]
        [HttpGet]  ///這裏不用設定也可以正確抓到
        [Route("category/{categoryId}")]
        public IHttpActionResult GetTaiwanDojoCategory(Guid categoryId)
        {
            TaiwanDojoCategory taiwanDojoCategory = _taiwanDojoCategoryService.GetTaiwanDojoCategory(categoryId);
            if (taiwanDojoCategory != null)
            {
                TaiwanDojoCategoryDto taiwanDojoCategoryDto = _mapper.Map<TaiwanDojoCategoryDto>(taiwanDojoCategory);
                return Ok(taiwanDojoCategoryDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        // PUT: api/taiwanDojo/category/{categoryId}
        [HttpPut]
        [Route("category/{categoryId}")]
        public IHttpActionResult UpdateTaiwanDojoCategory(Guid categoryId)
        {
            try
            {
                TaiwanDojoCategory taiwanDojoCategory = _taiwanDojoCategoryService.GetTaiwanDojoCategory(categoryId);
                if (taiwanDojoCategory != null)
                {
                    HttpRequest request = HttpContext.Current.Request;
                    if (string.IsNullOrWhiteSpace(request.Form.Get("name")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("name") == null ? "必須有name欄位" : "name參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(request.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }
                    _taiwanDojoCategoryService.UpdateTaiwanDojoCategory(request, taiwanDojoCategory);

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
        // DELETE: api/taiwanDojo/category/{categoryId}
        [HttpDelete]
        [Route("category/{categoryId}")]
        public IHttpActionResult DeleteTaiwanDojoCategory(Guid categoryId)
        {
            try
            {
                TaiwanDojoCategory taiwanDojoCategory = _taiwanDojoCategoryService.GetTaiwanDojoCategory(categoryId);
                if (taiwanDojoCategory != null)
                {
                    _taiwanDojoCategoryService.DeleteTaiwanDojoCategory(taiwanDojoCategory);
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
        // Get: api/taiwanDojo/category/
        [AllowAnonymous]
        [HttpGet]          ///這裏要設定不會自動辨斷"Message": "要求的資源不支援 http 方法 'GET'。"
        [Route("category")]
        public IHttpActionResult GetTaiwanDojoCategories()
        {
            try
            {
                List<TaiwanDojoCategory> taiwanDojoCategories = _taiwanDojoCategoryService.GetTaiwanDojoCategories();
                if (taiwanDojoCategories != null)
                {
                    List<TaiwanDojoCategoryDto> taiwanDojoCategoryDtos = _mapper.Map<List<TaiwanDojoCategoryDto>>(taiwanDojoCategories);
                    return Ok(taiwanDojoCategoryDtos);
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
