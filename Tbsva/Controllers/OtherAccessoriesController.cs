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
    public class OtherAccessoriesController : ApiController
    {
        #region DI依賴注入功能
        private readonly IOtherAccessoriesService _IOtherAccessoriesService;
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="iOtherAccessoriesService">介面</param>
        /// <param name="mapper">自動轉型</param>
        /// <param name="imageFormatHelper">透過MIME檢查上傳檔案是否為圖檔</param>
        public OtherAccessoriesController(IOtherAccessoriesService iOtherAccessoriesService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            _IOtherAccessoriesService = iOtherAccessoriesService;
            m_Mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增
        // POST: /OtherAccessories
        [HttpPost]
        public IHttpActionResult InsertOtherAccessories()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
                //var abc = _request.Form["content"];
                //var abc2 = _request.Form.Get("content");
                if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                }
                _IOtherAccessoriesService.DeleteAllContents();               //清空資料表所有內容
                OtherAccessories _otherAccessories = _IOtherAccessoriesService.Insert_OtherAccessories(_request);  //新增資料
                OtherAccessoriesDto _otherAccessoriesDto = m_Mapper.Map<OtherAccessoriesDto>(_otherAccessories);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _otherAccessoriesDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");  //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 內容區插入一個圖片
        // GET: /OtherAccessories/AddImage
        [HttpPost]
        [Route("OtherAccessories/AddImage")]
        public IHttpActionResult AddImage()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
                if (_request.Files.Count == 0)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_imageFormatHelper.CheckImageMIME(_request.Files) == false)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                string _imageUrl = _IOtherAccessoriesService.AddImage(_request);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new { Image_Link = _imageUrl }));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得所有內容(只會有一筆資料)
        // GET: /OtherAccessories/
        [AllowAnonymous]
        public IHttpActionResult GetOtherAccessories()
        {
            OtherAccessories _otherAccessories = _IOtherAccessoriesService.Get_OtherAccessories();
            if (_otherAccessories != null)
            {
                OtherAccessoriesDto _otherAccessoriesDto = m_Mapper.Map<OtherAccessoriesDto>(_otherAccessories);
                return Ok(_otherAccessoriesDto);
            }
            else
            {
                return NotFound();
            }
        }
         #endregion
        }
}
