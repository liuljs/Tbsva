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

namespace WebShopping.Services
{
    /// <summary>
    /// 後台權限需登入設定，全域
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    public class VideoContentController : ApiController
    {
        #region DI依賴注入功能
        private IVideoContentService VideoContentService;
        private IMapper Mapper;
        private IImageFormatHelper ImageFormatHelper;

        public VideoContentController(IVideoContentService videoContentService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            VideoContentService = videoContentService;
            Mapper = mapper;
            ImageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增一筆資料
        // POST: /VideoContent
        [HttpPost]
        public IHttpActionResult InsertVideoContent()
        {
            try
            {
                HttpRequest httpRequest = HttpContext.Current.Request;  //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("VideoCategory_id")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("VideoCategory_id") == null ? "必須有關聯目錄VideoCategory_id" : "VideoCategory_id參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("title")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("title") == null ? "必須有title" : "title參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("brief")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("Enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("first")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                }
                #region 圖檔檢驗(需上傳)
                if (httpRequest.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                }
                if (httpRequest.Files[0].ContentLength <=0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (ImageFormatHelper.CheckImageMIME(httpRequest.Files) == false)  //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return  ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion
                VideoContent videoContent = VideoContentService.InsertVideoContent(httpRequest);  //透過介面

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
                VideoContentDto videoContentDto = Mapper.Map<VideoContentDto>(videoContent);

                //return Request.CreateErrorResponse這裏弄錯時會出現引數3無法從轉換string
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, videoContentDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // POST: /VideoContent/id
        [AllowAnonymous]
        public IHttpActionResult GetVideoContent(Guid id)
        {
            VideoContent videoContent = VideoContentService.GetVideoContent(id);
            if (videoContent !=null)
            {
                VideoContentDto videoContentDto = Mapper.Map<VideoContentDto>(videoContent);
                return Ok(videoContentDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改一筆資料
        // PUT: /VideoContent/id
        [HttpPut]
        public IHttpActionResult UpdateVideoContent(Guid id)
        {
            try
            {
                //1.先取出要修改的資料
                VideoContent videoContent = VideoContentService.GetVideoContent(id);
                //2.辨斷有無資料，並辨斷接收進來的資料
                if (videoContent != null)
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;  //取得使用者要求的Request物件
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("VideoCategory_id")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("VideoCategory_id") == null ? "必須有關聯目錄VideoCategory_id" : "VideoCategory_id參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("title")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("title") == null ? "必須有title" : "title參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("brief")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("content")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("content") == null ? "必須有content" : "content參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("Enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("first")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                    }
                    #region 修改時上傳圖檔非必選
                    if (httpRequest.Files.Count > 0)    //檢查有沒有上傳圖片的欄位選項，因為要有欄位時httpRequest.Files[0]才不會錯
                    {
                        if (httpRequest.Files[0].ContentLength > 0)  //檢查上傳檔案大小，確定是有上傳，不然CheckImageMIME會出錯
                        {
                            if (ImageFormatHelper.CheckImageMIME(httpRequest.Files) == false) //這裏若file欄裏面沒有選檔案時CheckImageMIME還是會進入檢查, 除非將file欄不勾選
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                            }
                        }
                    }
                    #endregion
                    //修改
                    VideoContentService.UpdateVideoContent(httpRequest, videoContent);
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
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");  //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 刪除一筆資料
        // DELETE: /VideoContent/id
        [HttpDelete]
        public IHttpActionResult DeleteVideoContent(Guid id) 
        {
            VideoContent videoContent = VideoContentService.GetVideoContent(id);
            if (videoContent != null)
            {
                VideoContentService.DeleteVideoContent(videoContent);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: /VideoContent
        [AllowAnonymous]
        public IHttpActionResult GetVideoContent()
        {
            HttpRequest httpRequest = HttpContext.Current.Request;   //取得使用者要求的Request物件

            int? _VideoCategory_id = null;
            int? _count = null;
            int? _page = null;
            string _keyword = string.Empty;

            if (!string.IsNullOrWhiteSpace(httpRequest["VideoCategory_id"]))
            {
                _VideoCategory_id = Convert.ToInt32(httpRequest["VideoCategory_id"]);
            }
            if (!string.IsNullOrWhiteSpace(httpRequest["count"]))
            {
                _count = Convert.ToInt32(httpRequest["count"]);
            }
            if (!string.IsNullOrWhiteSpace(httpRequest["page"]))
            {
                _page =  Convert.ToInt32(httpRequest["page"]);
            }
            if (!string.IsNullOrWhiteSpace(httpRequest["keyword"]))
            {
                _keyword = httpRequest["keyword"].ToString();
            }

            List<VideoContent> videoContents = VideoContentService.GetVideoContents(_VideoCategory_id, _count, _page, _keyword);
            if (videoContents.Count > 0)
            {
                List<VideoContentDto> videoContentDtos = Mapper.Map<List<VideoContentDto>>(videoContents);  // 多筆轉換型別
                return Ok(videoContentDtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
    }
}
