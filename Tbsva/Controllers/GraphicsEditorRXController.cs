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
    ///RoutePrefix路由前綴
    //[RoutePrefix("graphicsEditorR1")]
    //無法從這裏設定RoutePrefix無法設定重覆
    //[RoutePrefix("api")]
    public class GraphicsEditorRXController : ApiController
    {
        #region DI依賴注入功能
        private readonly IGraphicsEditorService _graphicsEditorService;
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;
        private IRepeatLimit _repeatLimit;

        string tableName = string.Empty;
        //private string _tableName = "";  //主資料表
        //private string _directoryName = "";  //目錄名

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="graphicsEditorService">介面</param>
        /// <param name="mapper">自動轉型</param>
        /// <param name="imageFormatHelper">透過MIME檢查上傳檔案是否為圖檔</param>
        /// <param name="repeatLimit"></param>
        public GraphicsEditorRXController(IGraphicsEditorService graphicsEditorService, IMapper mapper, IImageFormatHelper imageFormatHelper, IRepeatLimit repeatLimit)
        {
            _graphicsEditorService = graphicsEditorService;
            m_Mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
            _repeatLimit = repeatLimit;
        }
        #endregion


        #region 1.新增
        // POST: api/graphicsEditorR{num}
        [Route("graphicsEditorR{version:int:regex(^([1-9]|1[0-1])$)}")]  //只能輸入1~10
        [HttpPost]
        public IHttpActionResult InsertGraphicsEditor()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件

                //先取看有沒有資料, 此資料取出不加Url
                GraphicsEditor original_graphicsEditor = _graphicsEditorService.GetGraphicsEditorNoUrl();

                //調出_tableName
                //_graphicsEditorService.TableNameDirectoryName(out string _tableName, out string _directoryName);
                //return Ok(_tableName);
                //加辨斷只能新增一筆資料，若有一筆則阻擋
                //if ( OnlyOneData.OnlyOne == _repeatLimit.onlyOneData(_tableName) )
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, OnlyOneData.OnlyOne.ToString()));
                //}

                #region 圖檔檢驗(有上傳才檢查)
                //再檢查所有上傳檔案MIME
                if (_imageFormatHelper.CheckImageMIME2(_request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion

                //_graphicsEditorService.DeleteAllNoDelPicContents();       //清空資料表所有內容(但不刪除圖片檔)

                GraphicsEditor graphicsEditor = _graphicsEditorService.InsertGraphicsEditor(_request, original_graphicsEditor);  //新增資料
                GraphicsEditorDto graphicsEditorDto = m_Mapper.Map<GraphicsEditorDto>(graphicsEditor);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, graphicsEditorDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");  //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion


        #region 2取得所有內容(只會有一筆資料)
        // GET: /graphicsEditorR{num}/
        //[Route("graphicsEditorR{version2:int:regex(1|2|3)}/{tableName:regex(^graphicsEditorR\\d{0,2}$)}")]

        // 路由可以設定傳參數進來
        // [Route("VA{version:int:regex(1|2|3)}/{A2:regex(^graphicsEditorR\\d{0,2}$)}")]
        // public IHttpActionResult GetGraphicsEditor(int version, string A2)
        // http://localhost:59415/VA1/graphicsEditorR1

        // [RegularExpression("[0]|[1]|[2]")]

        //^([1-9]|1[0-9]|2[0-9])$   只能輸入1~29
        //[Route("graphicsEditorR{version:int:regex(d{0,2}$)}")]
        // public IHttpActionResult GetGraphicsEditor(int version) <=可以這樣傳進來
        //https://www.regextester.com/93764 驗證網
        //[Route("{A2:regex(^graphicsEditorR\\d{0,2}$)}")]
        //[Route("{tableName:regex(^graphicsEditorR\\d{0,2}$)}")]
        //[Route("graphicsEditorR1")]
        //[Route("graphicsEditorR2")]
        //[Route("graphicsEditorR3")]
        //[Route("graphicsEditorR4")]
        //[Route("graphicsEditorR5")]
        //[Route("graphicsEditorR6")]
        //[Route("graphicsEditorR7")]
        //[Route("graphicsEditorR8")]
        //[Route("graphicsEditorR9")]
        //[Route("graphicsEditorR10")]
        [Route("graphicsEditorR{version:int:regex(^([1-9]|1[0-1])$)}")]  //只能輸入1~10
        [AllowAnonymous]
        public IHttpActionResult GetGraphicsEditor()
        {
            //HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
            //string Route = _request.Path.Replace("/api/", "").Replace("/", ""); //取出Route的網址並取代/ 線上是這樣/api/graphicsEditorR2
            //return Ok(Route);

            GraphicsEditor graphicsEditor = _graphicsEditorService.GetGraphicsEditor();
            if (graphicsEditor != null)
            {
                GraphicsEditorDto graphicsEditorDto = m_Mapper.Map<GraphicsEditorDto>(graphicsEditor);
                return Ok(graphicsEditorDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion


        #region 3修改資料
        // PUT: api/graphicsEditorR{num}/
        [Route("graphicsEditorR{version:int:regex(^([1-9]|1[0-1])$)}")]  //只能輸入1~10
        [HttpPut]
        public IHttpActionResult UpdateGraphicsEditor()
        {
            try
            {
                GraphicsEditor graphicsEditor = _graphicsEditorService.GetGraphicsEditor();       //先取出要修改的資料
                if (graphicsEditor != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值                             
       
                    #region 圖檔檢驗(有上傳才檢查)
                    //再檢查所有上傳檔案MIME
                    if (_imageFormatHelper.CheckImageMIME2(_request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }
                    #endregion

                    //修改
                    _graphicsEditorService.UpdateGraphicsEditor(_request, graphicsEditor);
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


        #region 4刪除
        // DELETE: api/graphicsEditorR{num}/
        [Route("graphicsEditorR{version:int:regex(^([1-9]|1[0-1])$)}")]  //只能輸入1~10
        [HttpDelete]
        public IHttpActionResult DeleteGraphicsEditor()
        {
            GraphicsEditor graphicsEditor = _graphicsEditorService.GetGraphicsEditor();       //先取出要修改
            if (graphicsEditor != null)
            {
                _graphicsEditorService.DeleteAllContents();
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion


        #region 5內容區插入一個圖片
        // GET: /graphicsEditorR{num}/image
        //[Route("graphicsEditorR1/image")]
        [Route("graphicsEditorR{version:int:regex(^([1-9]|1[0-1])$)}/image")]  //只能輸入1~10
        [HttpPost]
        public IHttpActionResult AddImage(int version)
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

                //string _directoryName = "graphicsEditorR" + version;
                string _directoryName = $"graphicsEditorR{version}";
                string _imageUrl = _graphicsEditorService.AddImage(request, _directoryName);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new { imageURL = _imageUrl }));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion





    }
}
