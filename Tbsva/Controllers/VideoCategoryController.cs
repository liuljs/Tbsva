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
    public class VideoCategoryController : ApiController
    {
        #region DI依賴注入功能
        private IVideoCategoryService videoCategoryService;
        private IMapper mapper;
        /// <summary>
        /// DI依賴注入功能
        /// </summary>
        /// <param name="videoCategoryService"></param>
        /// <param name="mapper"></param>
        public VideoCategoryController(IVideoCategoryService videoCategoryService, IMapper mapper)
        {
            this.videoCategoryService = videoCategoryService;
            this.mapper = mapper;
        }
        #endregion

        #region 新增
        // POST: /VideoCategory/
        [HttpPost]
        public IHttpActionResult InsertVideoCategory()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(_request.Form.Get("name")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("name") == null ? "必須有name欄位" : "name參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                }

                VideoCategory videoCategory = videoCategoryService.InsertVideoCategory(_request);
                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
                VideoCategoryDto videoCategoryDto = mapper.Map<VideoCategoryDto>(videoCategory);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, videoCategoryDto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // GET: /VideoCategory/id
        [AllowAnonymous]
        public IHttpActionResult GetVideoCategory(int id)
        {
            VideoCategory videoCategory = videoCategoryService.GetVideoCategory(id);
            if (videoCategory != null)
            {
                VideoCategoryDto videoCategoryDto = mapper.Map<VideoCategoryDto>(videoCategory);
                return Ok(videoCategoryDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        // PUT: /VideoCategory/id
        [HttpPut]
        public IHttpActionResult UpdateVideoCategory(int id)
        {
            try
            {
                VideoCategory videoCategory = videoCategoryService.GetVideoCategory(id);   //先取出要修改的資料
                if (videoCategory != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("name")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("name") == null ? "必須有name欄位" : "name參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                    }

                    videoCategoryService.UpdateVideoCategory(_request, videoCategory);

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
        // DELETE: /VideoCategory/id
        [HttpDelete]
        public IHttpActionResult DeleteVideoCategory(int id)
        {
            try
            {
                VideoCategory videoCategory = videoCategoryService.GetVideoCategory(id);  //先取出要刪的資料
                if (videoCategory != null)
                {
                    videoCategoryService.DeleteVideoCategory(videoCategory);
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
        // GET: /VideoCategory/
        [AllowAnonymous]
        public IHttpActionResult GetVideoCategories()
        {
            try
            {
                List<VideoCategory> videoCategories = videoCategoryService.GetVideoCategories();
                if (videoCategories !=null)
                {
                    // 原資料庫VideoCategory型別 AutoMapper 多筆轉換成VideoCategoryDto型別
                    List<VideoCategoryDto> videoCategoryDtos = mapper.Map<List<VideoCategoryDto>>(videoCategories);

                    return Ok(videoCategoryDtos);
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
