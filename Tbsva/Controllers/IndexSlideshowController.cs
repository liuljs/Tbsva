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
    public class IndexSlideshowController : ApiController
    {
        #region DI依賴注入功能
        private IIndexSlideshowService _indexSlideshowService;
        private IMapper Mapper;
        private IImageFormatHelper ImageFormatHelper;
        public IndexSlideshowController(IIndexSlideshowService indexSlideshowService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            _indexSlideshowService = indexSlideshowService;
            Mapper = mapper;
            ImageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增一筆資料
        // POST: api/indexSlideshow
        [HttpPost]
        public IHttpActionResult InsertIndexSlideshow()
        {
            try
            {
                HttpRequest httpRequest = HttpContext.Current.Request;    //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                //if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("name")))
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                //}
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
                #region 檢查有沒有fullImage欄必填與上傳檔案
                bool _isChech = false;
                foreach (string fieldName in httpRequest.Files.AllKeys)
                {
                    if (fieldName.ToLower() == "fullimage")  // 檢查有滿版圖片檔案後就跳出_isChech標true通過
                    {
                        _isChech = true;  //確定有fullimage欄
                        if (httpRequest.Files[fieldName].ContentLength <= 0)    //主要檢查第一個fullImage有沒有檔案容量，以postman有勾選但未選上傳時，file欄就會算進來AllKeys，如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                        }
                        break;
                    }
                }
                if (!_isChech)  //沒有fullImage這個file欄位
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片fullImage參數"));
                }
                #endregion

                //testhttpRequest.Files[test]
                //foreach (string test in httpRequest.Files.AllKeys)
                //{
                //   Int32 _test = httpRequest.Files[test].ContentLength;
                //}
                //與上方寫法相等[]裏抓file輸入框名稱
                //foreach (string key in httpRequest.Files)
                //{
                //    Int32 _test2 = httpRequest.Files[key].ContentLength;
                //}

                //再檢查所有上傳檔案MIME
                if (ImageFormatHelper.CheckImageMIME2(httpRequest.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }

                //取得用戶端所上傳的檔案集合HttpRequest.Files 屬性
                //HttpFileCollection fileCollection = httpRequest.Files;

                //處理多張圖檔上傳檢查，欄位選項有勾選才檢查，修改時上傳圖檔非必選
                //if (fileCollection.Count > 0)      //Postman左邊欄位有勾才會計算不然會是NULL
                //{
                //    for (int i = 0; i < fileCollection.Count; i++)
                //    {
                //        HttpPostedFile _file = fileCollection[i];
                //        string _type = _file.ContentType;  //辨斷檔案格式，若沒有上傳會是null
                //       //所以先辨斷有沒有容量，有在辨斷圖片格式有沒有錯
                //        if (_type != null && _file.ContentLength > 0)      //沒有檔時會出現null，確定有上傳檔在檢查
                //        {
                //            if (!_type.Contains("image"))
                //            {
                //                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                //            }
                //        }
                //    }
                //}
                #endregion

                IndexSlideshow indexSlideshow = _indexSlideshowService.InsertIndexSlideshow(httpRequest);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
                IndexSlideshowDto indexSlideshowDto = Mapper.Map<IndexSlideshowDto>(indexSlideshow);

                //return Request.CreateErrorResponse這裏弄錯時會出現引數3無法從轉換string
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, indexSlideshowDto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\n{ex.Message}\n{ex.StackTrace}");
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // GET: api/indexSlideshow/{slideId}
        [AllowAnonymous]
        public IHttpActionResult GetIndexSlideshow(Guid id)
        {
            IndexSlideshow indexSlideshow = _indexSlideshowService.GetIndexSlideshow(id);
            if (indexSlideshow !=null)
            {
                IndexSlideshowDto indexSlideshowDto = Mapper.Map<IndexSlideshowDto>(indexSlideshow);
                return Ok(indexSlideshowDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        // PUT: api/IndexSlideshow/{slideId}
        [HttpPut]
        public IHttpActionResult UpdateIndexSlideshow(Guid id)
        {
            try
            {
                IndexSlideshow indexSlideshow = _indexSlideshowService.GetIndexSlideshow(id);
                if (indexSlideshow != null)
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    //if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("name")))
                    //{
                    //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("name") == null ? "必須有name" : "name參數格式錯誤"));
                    //}
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("first")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("sort") == null ? "必須有sort參數" : "sort參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }
                    #region 圖檔檢驗MIME(非必選，沒上傳則不檢查)
                    if (httpRequest.Files.Count > 0)  //有file欄時開始檢查
                    {
                        //再檢查所有上傳檔案MIME，MIME2已有check ContentLength>0
                        if (ImageFormatHelper.CheckImageMIME2(httpRequest.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                        }
                    }
                    #endregion
                    //修改
                    _indexSlideshowService.UpdateIndexSlideshow(httpRequest, indexSlideshow);
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
        // DELETE: api/IndexSlideshow/{slideId}
        [HttpDelete]
        public IHttpActionResult DeleteIndexSlideshow(Guid id)
        {
            IndexSlideshow indexSlideshow = _indexSlideshowService.GetIndexSlideshow(id);
            if (indexSlideshow != null)
            {
                _indexSlideshowService.DeleteIndexSlideshow(indexSlideshow);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion


        #region 取得所有內容
        // GET: api/IndexSlideshow
        [AllowAnonymous]
        public IHttpActionResult GetIndexSlideshows()
        {
            List<IndexSlideshow> indexSlideshows = _indexSlideshowService.GetIndexSlideshows();
            if (indexSlideshows.Count > 0)
            {
                List<IndexSlideshowDto> indexSlideshowDtos = Mapper.Map<List<IndexSlideshowDto>>(indexSlideshows);
                return Ok(indexSlideshowDtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion



    }
}
