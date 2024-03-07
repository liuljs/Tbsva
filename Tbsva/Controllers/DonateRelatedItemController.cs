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
    public class DonateRelatedItemController : ApiController
    {
        #region DI依賴注入功能
        private IDonateRelatedItemService _IDonateRelatedItemService;
        private IMapper _mapper;
        private IImageFormatHelper _IImageFormatHelper;
        private IRepeatLimit _repeatLimit;
        /// <summary>
        /// DI依賴注入功能
        /// </summary>
        /// <param name="iDonateRelatedItemService">內容輸入</param>
        /// <param name="iImageFormatHelper">圖檔格式</param>
        public DonateRelatedItemController(IDonateRelatedItemService iDonateRelatedItemService, IMapper mapper, IImageFormatHelper iImageFormatHelper, IRepeatLimit repeatLimit)
        {
            _IDonateRelatedItemService = iDonateRelatedItemService;
            _mapper = mapper;
            _IImageFormatHelper = iImageFormatHelper;
            _repeatLimit = repeatLimit;
        }
        #endregion

        #region 新增
        // POST: api/DonateRelatedItem
        [HttpPost]
        public IHttpActionResult InsertDonateRelatedItem()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(_request.Form.Get("primary")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("primary") == null ? "必須有primary欄位" : "primary參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("secondary")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("secondary") == null ? "必須有secondary欄位" : "secondary參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("title")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("title") == null ? "必須有title欄位" : "title參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("amount")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("amount") == null ? "必須有amount欄位" : "amount參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("first")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                }
                //first(只能開啟一個置頂), 新增時若選開啟時需辨斷資料庫是否已經有存在1筆，若有則不能再新增
                DonateRelatedItem donateRelatedItem = new DonateRelatedItem();  //先建一個空的
                if (_request.Form.Get("first") == "1" && _request.Form.Get("primary") == "2")  //2結緣捐贈才有一筆置頂
                {
                    string _tableName = "[DonateRelatedItem]";

                    if (MessageOnlyOne.FirstOnlyOne == _repeatLimit.messageOnlyOne(_tableName, donateRelatedItem, "AND [primary] = '2'"))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, MessageOnlyOne.FirstOnlyOne.ToString()));
                    }
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                }
                #region 圖檔檢驗
                //上傳圖檔非必選
                if (_request.Files.Count > 0)      //檢查有沒有上傳圖片的欄位選項，因為要有欄位時_request.Files[0]才不會錯
                {
                    if (_request.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳，不然CheckImageMIME會出錯
                    {
                        if (_IImageFormatHelper.CheckImageMIME(_request.Files) == false)  //這裏若file欄裏面沒有選檔案時CheckImageMIME還是會進入檢查, 除非將file欄不勾選
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                        }
                    }
                }
                #endregion

                donateRelatedItem = _IDonateRelatedItemService.Insert_DonateRelatedItem(_request);    //透過介面.實作
                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉成<Dto型別>(要轉換的類別)
                DonateRelatedItemDto donateRelatedItemDto = _mapper.Map<DonateRelatedItemDto>(donateRelatedItem);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, donateRelatedItemDto));                    
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // GET: api/DonateRelatedItem/5
        [AllowAnonymous]
        public IHttpActionResult GetDonateRelatedItem(Guid id)
        {
            DonateRelatedItem _DonateRelatedItem = _IDonateRelatedItemService.Get_DonateRelatedItem(id);
            if (_DonateRelatedItem != null)
            {
                DonateRelatedItemDto donateRelatedItemDto = _mapper.Map<DonateRelatedItemDto>(_DonateRelatedItem);
                return Ok(donateRelatedItemDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 更新一筆資料
        // PUT: api/DonateRelatedItem/5
        [HttpPut]
        public IHttpActionResult UpdateDonateRelatedItem(Guid Id)
        {
            try
            {
                //1.先取出要修改的資料
                DonateRelatedItem _DonateRelatedItem = _IDonateRelatedItemService.Get_DonateRelatedItem(Id);
                //2.辨斷有無資料，並辨斷接收進來的資料
                if (_DonateRelatedItem !=null)
                {
                    HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("primary")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("primary") == null ? "必須有primary欄位" : "primary參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("secondary")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("secondary") == null ? "必須有secondary欄位" : "secondary參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("title")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("title") == null ? "必須有title欄位" : "title參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("amount")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("amount") == null ? "必須有amount欄位" : "amount參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("first")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                    }
                    //first(只能開啟一個置頂), 新增時若選開啟時需辨斷資料庫是否已經有存在1筆，若有則不能再新增
                    //DonateRelatedItem donateRelatedItem = new DonateRelatedItem();  //先建一個空的
                    if (_request.Form.Get("first") == "1" && _request.Form.Get("primary") == "2")  //2結緣捐贈才有一筆置頂，如果要改成結緣
                    {
                        string _tableName = "[DonateRelatedItem]";

                        if (MessageOnlyOne.FirstOnlyOne == _repeatLimit.messageOnlyOne(_tableName, _DonateRelatedItem, "AND [primary] = '2'"))
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, MessageOnlyOne.FirstOnlyOne.ToString()));
                        }
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("sort") == null ? "必須有sort參數" : "sort參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }
                    #region 圖檔檢驗
                    //上傳圖檔非必選
                    if (_request.Files.Count > 0)      //檢查有沒有上傳圖片的欄位選項，因為要有欄位時_request.Files[0]才不會錯
                    {
                        if (_request.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳，不然CheckImageMIME會出錯
                        {
                            if (_IImageFormatHelper.CheckImageMIME(_request.Files) == false)  //這裏若file欄裏面沒有選檔案時CheckImageMIME還是會進入檢查, 除非將file欄不勾選
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                            }
                        }
                    }
                    #endregion
                    //修改
                    _IDonateRelatedItemService.Update_DonateRelatedItem(_request, _DonateRelatedItem);
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
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 刪除
        // DELETE: api/DonateRelatedItem/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult DeleteDonateRelatedItem(Guid id)
        {
            DonateRelatedItem _DonateRelatedItem = _IDonateRelatedItemService.Get_DonateRelatedItem(id);
            if (_DonateRelatedItem != null)
            {
                _IDonateRelatedItemService.Delete_DonateRelatedItem(_DonateRelatedItem);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: api/DonateRelatedItem
        [AllowAnonymous]
        public IHttpActionResult GetDonateRelatedItem_ALL()
        {
            HttpRequest request = HttpContext.Current.Request;
            string _primary = null;
            //int? _first = null;
            int? _count = null;
            int? _page = null;

            if (string.IsNullOrWhiteSpace(request["primary"]))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, request["primary"] == null ? "必須有primary參數" : "primary參數格式錯誤"));
            }
            _primary = request["primary"];

            //if (!string.IsNullOrWhiteSpace(request["first"]))
            //{
            //    _first = Convert.ToInt32(request["first"]);
            //}
            if (!string.IsNullOrWhiteSpace(request["count"]))
            {
                _count = Convert.ToInt32(request["count"]);
            }
            if (!string.IsNullOrWhiteSpace(request["page"]))
            {
                _page = Convert.ToInt32(request["page"]);
            }

            List<DonateRelatedItem> _DonateRelatedItems = _IDonateRelatedItemService.Get_DonateRelatedItem_ALL(_primary, _count, _page);
            if (_DonateRelatedItems.Count > 0)
            {
                List<DonateRelatedItemDto> donateRelatedItemDtos = _mapper.Map<List<DonateRelatedItemDto>>(_DonateRelatedItems);
                return Ok(donateRelatedItemDtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion


        #region 文字編輯器內容區插入一個圖片(新增與修改時)
        // POST: api/DonateRelatedItem/image
        [HttpPost]
        [Route("DonateRelatedItem/image")]
        public IHttpActionResult image()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;  //取得使用者要求的Request物件
                #region 圖檔檢驗(需上傳)
                if (request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                }
                if (request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_IImageFormatHelper.CheckImageMIME(request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion

                DonateRelatedItemImage DonateRelatedItemImage = _IDonateRelatedItemService.AddUploadImage(request);
                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                DonateRelatedItemImageDto DonateRelatedItemImageDto = _mapper.Map<DonateRelatedItemImageDto>(DonateRelatedItemImage);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, DonateRelatedItemImageDto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        // POST: api/DonateRelatedItem/image/{DonateRelatedItemId}
        [HttpPost]
        [Route("DonateRelatedItem/image/{id}")]
        public IHttpActionResult image(Guid id)
        {
            try
            {
                DonateRelatedItem DonateRelatedItem = _IDonateRelatedItemService.Get_DonateRelatedItem(id);
                if (DonateRelatedItem != null)
                {
                    HttpRequest request = HttpContext.Current.Request;
                    #region 圖檔檢驗(需上傳)
                    if (request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                    }
                    if (request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                    }
                    if (_IImageFormatHelper.CheckImageMIME(request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }
                    #endregion

                    DonateRelatedItemImage DonateRelatedItemImage = _IDonateRelatedItemService.AddUploadImage(request, id);

                    // AutoMapper轉換型別
                    //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                    DonateRelatedItemImageDto DonateRelatedItemImageDto = _mapper.Map<DonateRelatedItemImageDto>(DonateRelatedItemImage);

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, DonateRelatedItemImageDto));
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
