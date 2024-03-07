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
    public class Other1Controller : ApiController
    {
        #region DI依賴注入功能
        private readonly IOther1Service _IOther1Service;
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="iOther1Service">介面</param>
        /// <param name="mapper">自動轉型</param>
        /// <param name="imageFormatHelper">透過MIME檢查上傳檔案是否為圖檔</param>
        public Other1Controller(IOther1Service iOther1Service, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            _IOther1Service = iOther1Service;
            m_Mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增
        // POST: /Other1
        [HttpPost]
        public IHttpActionResult InsertOther1()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
                if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                }
                _IOther1Service.DeleteAllContents();               //清空資料表所有內容
                Other1 _other = _IOther1Service.Insert_Other1(_request);  //新增資料
                Other1Dto _other1Dto = m_Mapper.Map<Other1Dto>(_other);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _other1Dto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");  //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 內容區插入一個圖片
        // GET: /Other1/AddImage
        [HttpPost]
        [Route("Other1/AddImage")]
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
                string _imageUrl = _IOther1Service.AddImage(_request);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new { Image_Link = _imageUrl }));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得所有內容(只會有一筆資料)
        // GET: /Other1/
        [AllowAnonymous]
        public IHttpActionResult GetOther1()
        {
            Other1 _other1 = _IOther1Service.Get_Other1();
            if (_other1 != null)
            {
                Other1Dto _other1Dto = m_Mapper.Map<Other1Dto>(_other1);
                return Ok(_other1Dto);
            }
            else
            {
                return NotFound();
            }
        }
         #endregion
        }
}
