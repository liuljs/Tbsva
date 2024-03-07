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
    public class VideoController : ApiController
    {
        #region DI依賴注入功能
        private IVideoService VideoService;
        private IMapper Mapper;
        private IImageFormatHelper ImageFormatHelper;

        public VideoController(IVideoService videoService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            VideoService = videoService;
            Mapper = mapper;
            ImageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增一筆資料
        // POST: api/video
        [HttpPost]
        public IHttpActionResult InsertVideo()
        {
            try
            {
                HttpRequest httpRequest = HttpContext.Current.Request;    //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                //檢查沒有上傳封面圖片時等於是要用youtube自動縮圖，所以需要檢查cover欄位
                if (httpRequest.Files.Count == 0)
                {
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("cover")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("cover") == null ? "必須有Youtube自動縮圖網址cover" : "cover參數格式錯誤"));
                    }
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("name")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                    //return Content(HttpStatusCode.BadRequest, httpRequest.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤" );
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("videoCategory")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("videoCategory") == null ? "必須有關聯目錄videoCategory" : "videoCategory參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("brief")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("video")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("video") == null ? "必須有影片網址video" : "video參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("content")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("content") == null ? "必須有content" : "content參數格式錯誤"));
                }
                if (httpRequest.Form.Get("cNameArr") == null)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有cNameArr參數"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("first")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                }                  

                #region 圖檔檢驗(非必選，沒上傳則不檢查)
                //修改時上傳圖檔非必選，因為上傳時已必傳上傳了
                if (httpRequest.Files.Count > 0)      //檢查有沒有上傳圖片的欄位選項，因為要有欄位時_request.Files[0]才不會錯
                {
                    if (httpRequest.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳，不然CheckImageMIME會出錯
                    {
                        if (ImageFormatHelper.CheckImageMIME(httpRequest.Files) == false)  //這裏若file欄裏面沒有選檔案時CheckImageMIME還是會進入檢查, 除非將file欄不勾選
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                        }
                    }
                }
                #endregion
                
                Video video = VideoService.InsertVideo(httpRequest);    //透過介面.實作

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
                //VideoProfile .ForMember(target => target.imageURL, option => option.MapFrom(source => VideoService.CustomizeToYouTubeCover(source)))
                // video.image_name有值則image_name轉imageURL，沒值則video.cover轉imageURL
                VideoDto videoDto = Mapper.Map<VideoDto>(video);

                //return Request.CreateErrorResponse這裏弄錯時會出現引數3無法從轉換string
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, videoDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // Get : api/video/{videoId}
        [AllowAnonymous]
        public IHttpActionResult GetVideo(Guid id)
        {
            Video video = VideoService.GetVideo(id);
            if (video != null)
            {
                VideoDto videoDto = Mapper.Map<VideoDto>(video);
                return Ok(videoDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        // PUT: api/video/{videoId}
        [HttpPut]
        public IHttpActionResult UpdateVideo(Guid id)
        {
            try
            {
                Video video = VideoService.GetVideo(id);         //先取出要修改的資料
                if (video != null)
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (httpRequest.Files.Count == 0)
                    {
                        if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("cover")))
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("cover") == null ? "必須有Youtube自動縮圖網址cover" : "cover參數格式錯誤"));
                        }
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("name")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("videoCategory")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("videoCategory") == null ? "必須有關聯目錄videoCategory" : "videoCategory參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("brief")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("video")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("video") == null ? "必須有影片網址video" : "video參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("content")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("content") == null ? "必須有content" : "content參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("first")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("sort") == null ? "必須有sort參數" : "sort參數格式錯誤"));
                    }
                    #region 圖檔檢驗
                    //上傳圖檔非必選
                    if (httpRequest.Files.Count > 0)      //檢查有沒有上傳圖片的欄位選項，因為要有欄位時_request.Files[0]才不會錯
                    {
                        if (httpRequest.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳，不然CheckImageMIME會出錯
                        {
                            if (ImageFormatHelper.CheckImageMIME(httpRequest.Files) == false)  //這裏若file欄裏面沒有選檔案時CheckImageMIME還是會進入檢查, 除非將file欄不勾選
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                            }
                        }
                    }
                    #endregion
                    //修改
                    VideoService.UpdateVideo(httpRequest, video);
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

        #region 刪除
        // DELETE: api/video/{videoId}
        [HttpDelete]
        public IHttpActionResult DeleteVideo(Guid id)
        {
            Video video = VideoService.GetVideo(id);
            if (video != null)
            {
                VideoService.DeleteVideo(video);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: api/video/
        // ?{videoCategory="活動隸屬目錄 ID"}&{count="每頁一次渲染的消息筆數"}&{page="目前的頁碼"}
        [AllowAnonymous]
        public IHttpActionResult GetVideo()
        {
            HttpRequest httpRequest = HttpContext.Current.Request;   //取得使用者要求的Request物件
            string _videoCategory = null;
            int? _count = null;
            int? _page = null;

            if (!string.IsNullOrWhiteSpace(httpRequest["videoCategory"]))
            {
                _videoCategory = httpRequest["videoCategory"];
            }
            if (!string.IsNullOrWhiteSpace(httpRequest["count"]))
            {
                _count = Convert.ToInt32(httpRequest["count"]);
            }
            if (!string.IsNullOrWhiteSpace(httpRequest["page"]))
            {
                _page = Convert.ToInt32(httpRequest["page"]);
            }

            List<Video> activities = VideoService.GetActivities(_videoCategory, _count, _page);
            if (activities.Count > 0)
            {
                List<VideoDto> videoDtos = Mapper.Map<List<VideoDto>>(activities);  // 多筆轉換型別
                return Ok(videoDtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 文字編輯器內容區插入一個圖片(新增與修改時)
        // POST: api/video/image
        /// <summary>
        /// 新增活動管理內容圖片
        /// 內容新增插入一個圖片:新增時,在內容裏(加入一個內容圖片)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("video/image")]
        public IHttpActionResult image()
        {
            try
            {
                HttpRequest httpRequest = HttpContext.Current.Request;  //取得使用者要求的Request物件
                #region 圖檔檢驗(需上傳)
                if (httpRequest.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                }
                if (httpRequest.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (ImageFormatHelper.CheckImageMIME(httpRequest.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion

                VideoImage videoImage = VideoService.AddUploadImage(httpRequest);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                VideoImageDto videoImageDto = Mapper.Map<VideoImageDto>(videoImage);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, videoImageDto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        // POST: api/video/image/{videoId}
        /// <summary>
        /// 更新活動管理的內容圖片
        /// 內容新增插入一個圖片:(修改)時(加入一個內容圖片)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("video/image/{id}")]
        public IHttpActionResult image(Guid id)
        {
            try
            {
                Video video = VideoService.GetVideo(id);
                if (video != null)
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    #region 圖檔檢驗(需上傳)
                    if (httpRequest.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                    }
                    if (httpRequest.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                    }
                    if (ImageFormatHelper.CheckImageMIME(httpRequest.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }
                    #endregion

                    VideoImage videoImage = VideoService.AddUploadImage(httpRequest, id);

                    // AutoMapper轉換型別
                    //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                    VideoImageDto videoImageDto = Mapper.Map<VideoImageDto>(videoImage);

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, videoImageDto));
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
