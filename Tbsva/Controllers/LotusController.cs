using AutoMapper;
using Newtonsoft.Json;
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
    public class LotusController : ApiController
    {
        #region DI依賴注入功能
        private ILotusService LotusService;
        private IMapper Mapper;
        private IImageFormatHelper ImageFormatHelper;
        private IImageFileHelper ImageFileHelper;

        public LotusController(ILotusService lotusService, IMapper mapper, IImageFormatHelper imageFormatHelper, IImageFileHelper imageFileHelper)
        {
            LotusService = lotusService;
            Mapper = mapper;
            ImageFormatHelper = imageFormatHelper;
            ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region 新增一筆資料
        // POST: api/lotus
        [HttpPost]
        public IHttpActionResult InsertLotus()
        {
            try
            {
                HttpRequest httpRequest = HttpContext.Current.Request;    //取得使用者要求的Request物件
                //"必須有title欄位" : "title參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("title")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("title") == null ? "必須有title" : "title參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("brief")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("number")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("number") == null ? "必須有number欄位" : "number參數格式錯誤"));
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

                #region 圖檔檢驗(需上傳)
                if (httpRequest.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查有沒有選要上傳的檔
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "沒有任何上傳圖片參數"));
                }
                #region 檢查有沒有cover封面圖片欄必填與上傳檔案
                if (ImageFileHelper.CheckCover(httpRequest, out string message))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
                }
                //bool _isChech = false;
                //foreach (string fieldName in httpRequest.Files.AllKeys)
                //{
                //    if (fieldName.ToLower() == "cover")  // 檢查有滿版圖片檔案後就跳出_isChech標true通過
                //    {
                //        _isChech = true;  //1.確定有cover欄
                //        if (httpRequest.Files[fieldName].ContentLength <= 0)    //cover沒有檔案容量
                //        {
                //            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                //        }
                //        break;  //只是要檢查cover，確定有也有上傳就跳出foreach
                //    }
                //}
                //if (!_isChech)  //沒有cover這個file欄位
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片cover參數"));
                //}
                #endregion

                //再檢查所有上傳檔案MIME
                if (ImageFormatHelper.CheckImageMIME2(httpRequest.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion

                Lotus lotus = LotusService.InsertLotus(httpRequest);    //透過介面.實作

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
                LotusDto lotusDto = Mapper.Map<LotusDto>(lotus);

                //return Request.CreateErrorResponse這裏弄錯時會出現引數3無法從轉換string
                //string JSON = JsonConvert.SerializeObject(lotusDto);  //類別序列化成JSON
                //SystemFunctions.WriteLogFile(JSON);   //寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, lotusDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // Get : api/lotus/{lotusId}
        [AllowAnonymous]
        public IHttpActionResult GetLotus(Guid id)
        {
            Lotus lotus = LotusService.GetLotus(id);
            if (lotus != null)
            {
                LotusDto lotusDto = Mapper.Map<LotusDto>(lotus);
                return Ok(lotusDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        // PUT: api/lotus/{lotusId}
        [HttpPut]
        public IHttpActionResult UpdateLotus(Guid id)
        {
            try
            {
                Lotus lotus = LotusService.GetLotus(id);         //先取出要修改的資料
                if (lotus != null)
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("title")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("title") == null ? "必須有title" : "title參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("brief")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("number")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("number") == null ? "必須有number欄位" : "number參數格式錯誤"));
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
                    //再檢查所有上傳檔案MIME
                    if (ImageFormatHelper.CheckImageMIME2(httpRequest.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }  

                    //修改
                    LotusService.UpdateLotus(httpRequest, lotus);
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
        // DELETE: api/lotus/{lotusId}
        [HttpDelete]
        public IHttpActionResult DeleteLotus(Guid id)
        {
            Lotus lotus = LotusService.GetLotus(id);
            if (lotus != null)
            {
                LotusService.DeleteLotus(lotus);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: api/lotus/
        // ?{count="每頁一次渲染的消息筆數"}&{page="目前的頁碼"}
        [AllowAnonymous]
        public IHttpActionResult GetLotusAll()
        {
            HttpRequest httpRequest = HttpContext.Current.Request;   //取得使用者要求的Request物件
            string _title = string.Empty;
            string _number = string.Empty;
            int? _count = null;
            int? _page = null;

            if (!string.IsNullOrWhiteSpace(httpRequest["title"]))
            {
                _title = httpRequest["title"].ToString();
            }
            if (!string.IsNullOrWhiteSpace(httpRequest["number"]))
            {
                _number = httpRequest["number"].ToString();
            }
            if (!string.IsNullOrWhiteSpace(httpRequest["count"]))
            {
                _count = Convert.ToInt32(httpRequest["count"]);
            }
            if (!string.IsNullOrWhiteSpace(httpRequest["page"]))
            {
                _page = Convert.ToInt32(httpRequest["page"]);
            }

            List<Lotus> lotus = LotusService.GetLotusAll(_title, _number, _count, _page);
            if (lotus.Count > 0)
            {
                List<LotusDto> lotusDtos = Mapper.Map<List<LotusDto>>(lotus);  // 多筆轉換型別
                return Ok(lotusDtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region  取得總筆數
        // GET: api/lotus/totalCount
        [Route("lotus/totalCount")]
        public IHttpActionResult GetLotusTotalCount()
        {
            HttpRequest httpRequest = HttpContext.Current.Request;   //取得使用者要求的Request物件
            string _title = string.Empty;
            string _number = string.Empty;

            if (!string.IsNullOrWhiteSpace(httpRequest["title"]))
            {
                _title = httpRequest["title"].ToString();
            }
            if (!string.IsNullOrWhiteSpace(httpRequest["number"]))
            {
                _number = httpRequest["number"].ToString();
            }
            int count = LotusService.GetLotusTotalCount(_title, _number);
            if (count >= 0)
            {
                return Ok(new { totalCount = count });
            }
            else
            {
                return NotFound();
            }
        }
        #endregion


        #region 文字編輯器內容區插入一個圖片(新增與修改時)
        // POST: api/lotus/image
        /// <summary>
        /// 新增活動管理內容圖片
        /// 內容新增插入一個圖片:新增時,在內容裏(加入一個內容圖片)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("lotus/image")]
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

                LotusImage lotusImage = LotusService.AddUploadImage(httpRequest);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                LotusImageDto lotusImageDto = Mapper.Map<LotusImageDto>(lotusImage);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, lotusImageDto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        // POST: api/lotus/image/{lotusId}
        /// <summary>
        /// 更新活動管理的內容圖片
        /// 內容新增插入一個圖片:新增時,在內容裏(加入一個內容圖片)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("lotus/image/{id}")]
        public IHttpActionResult image(Guid id)
        {
            try
            {
                Lotus lotus = LotusService.GetLotus(id);
                if (lotus != null)
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

                    LotusImage lotusImage = LotusService.AddUploadImage(httpRequest, id);

                    // AutoMapper轉換型別
                    //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                    LotusImageDto lotusImageDto = Mapper.Map<LotusImageDto>(lotusImage);

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, lotusImageDto));
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
