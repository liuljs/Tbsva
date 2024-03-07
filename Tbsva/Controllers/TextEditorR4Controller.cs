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
    public class TextEditorR4Controller : ApiController
    {
        #region DI依賴注入功能
        private readonly ITextEditorService _textEditorService;
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;
        /// <summary>
        ///  建構子
        /// </summary>
        /// <param name="textEditorService">介面</param>
        /// <param name="mapper">自動轉型</param>
        /// <param name="imageFormatHelper">透過MIME檢查上傳檔案是否為圖檔</param>
        public TextEditorR4Controller(ITextEditorService textEditorService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            _textEditorService = textEditorService;
            m_Mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
        }
        #endregion

        private string tableName = "text_editor_r4";
        private string directoryName = "textEditorR4";

        #region 新增
        // POST: api/textEditorR{num}
        [HttpPost]
        public IHttpActionResult InsertTextEditor()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
                if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content欄位" : "content參數格式錯誤"));
                }

                _textEditorService.DeleteAllContents(tableName);               //清空資料表所有內容

                TextEditor textEditor = _textEditorService.InsertTextEditor(tableName, _request);  //新增資料
                TextEditorDto textEditorDto = m_Mapper.Map<TextEditorDto>(textEditor);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, textEditorDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");  //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 內容區插入一個圖片
        // GET: /textEditorR{num}/image
        [HttpPost]
        [Route("textEditorR4/image")]
        public IHttpActionResult AddImage()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request; //取得使用者要求的Request物件
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

                string _imageUrl = _textEditorService.AddImage(directoryName, request);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new { imageURL = _imageUrl }));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得所有內容(只會有一筆資料)
        // GET: /textEditorR{num}/
        [AllowAnonymous]
        public IHttpActionResult GetTextEditor()
        {
            TextEditor textEditor = _textEditorService.GetTextEditor(tableName);
            if (textEditor != null)
            {
                TextEditorDto textEditorDto = m_Mapper.Map<TextEditorDto>(textEditor);
                return Ok(textEditorDto);
            }
            else
            {
                return NotFound();
            }
        }
         #endregion
        }
}
