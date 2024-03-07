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
    public class KnowledgeController : ApiController
    {
        #region DI依賴注入功能
        private IKnowledgeService _IKnowledgeService;  //欄位
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;

        /// <summary>
        ///  DI依賴注入功能
        /// </summary>
        /// <param name="knowledgeService">內容輸入</param>
        /// <param name="mapper">AutoMapper</param>
        /// <param name="imageFormatHelper">圖檔格式</param>
        public KnowledgeController(IKnowledgeService knowledgeService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            _IKnowledgeService = knowledgeService;  //參數
            m_Mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增
        // POST: /knowledge
        [HttpPost]
        public IHttpActionResult InsertKnowledge()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值

                if (string.IsNullOrWhiteSpace(_request.Form.Get("title")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("title") == null ? "必須有title" : "title參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("category")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("category") == null ? "必須有category" : "category參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("brief")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content" : "content參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("first")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                }
                if (_request.Form.Get("cNameArr") == null)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有cNameArr參數"));
                }
                #region 圖檔檢驗(需上傳)
                if (_request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                }
                if (_request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_imageFormatHelper.CheckImageMIME(_request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion

                Knowledge_content _knowledge_Content = _IKnowledgeService.Insert_Knowledge(_request);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                Knowledge_content_Dto _knowledge_Content_Dto = m_Mapper.Map<Knowledge_content_Dto>(_knowledge_Content);

                //return Request.CreateErrorResponse這裏弄錯時會出現引數3無法從轉換string
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _knowledge_Content_Dto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // GET: /Knowledge/{knowledgetId}
        [AllowAnonymous]
        //[Route("knowledge/{knowledgetId}")]
        public IHttpActionResult GetKnowledge(Guid id)
        {
            Knowledge_content _knowledge_Content = _IKnowledgeService.Get_Knowledge(id);
            if (_knowledge_Content != null)
            {
                Knowledge_content_Dto _knowledge_Content_Dto = m_Mapper.Map<Knowledge_content_Dto>(_knowledge_Content);
                return Ok(_knowledge_Content_Dto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 更新一筆資料
        // PUT: /knowledge/{knowledgetId}
        [HttpPut]
        //[Route("knowledge/{knowledgetId}")]
        public IHttpActionResult UpdateKnowledge(Guid id)
        {
            try
            {
                //1.先取出要修改的資料
                Knowledge_content _knowledge_Content = _IKnowledgeService.Get_Knowledge(id);
                //2.辨斷有無資料，並辨斷接收進來的資料
                if (_knowledge_Content != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                     //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("title")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("title") == null ? "必須有title" : "title參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("category")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("category") == null ? "必須有category" : "category參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("brief")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("brief") == null ? "必須有brief欄位" : "brief參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("content")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("content") == null ? "必須有content" : "content參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("first")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("first") == null ? "必須有first參數" : "first參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("sort") == null ? "必須有sort參數" : "sort參數格式錯誤"));
                    }
                    #region 圖檔檢驗
                    // 修改時上傳圖檔非必選
                    if (_request.Files.Count > 0)      //檢查有沒有上傳圖片的欄位選項，因為要有欄位時_request.Files[0]才不會錯
                    {
                        if (_request.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳，不然CheckImageMIME會出錯
                        {
                            if (_imageFormatHelper.CheckImageMIME(_request.Files) == false)  //這裏若file欄裏面沒有選檔案時CheckImageMIME還是會進入檢查, 除非將file欄不勾選
                            {
                                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                            }
                        }
                    }
                    #endregion

                    //修改
                    _IKnowledgeService.Update_Knowledge(_request, _knowledge_Content);
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
        // DELETE: /knowledge/{knowledgetId}
        [HttpDelete]
        public IHttpActionResult DeleteKnowledge(Guid id)
        {
            Knowledge_content _knowledge_Content = _IKnowledgeService.Get_Knowledge(id);
            if (_knowledge_Content != null)
            {
                _IKnowledgeService.Delete_Knowledge(_knowledge_Content);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: /Knowledge
        [AllowAnonymous]
        public IHttpActionResult GetKnowledge()
        {
            HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
            int? _count = null;
            int? _page = null;
            string _category = string.Empty;

            if (!string.IsNullOrWhiteSpace(_request["count"]))
            {
                _count = Convert.ToInt32(_request["count"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["page"]))
            {
                _page = Convert.ToInt32(_request["page"]);
            }

            if (!string.IsNullOrWhiteSpace(_request["category"]))
            {
                _category = _request["category"].ToString().Trim();
            }

            //if (string.IsNullOrWhiteSpace(_request.Form.Get("category")))
            //{
            //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("category") == null ? "必須有category參數" : "category參數格式錯誤"));
            //}

            List<Knowledge_content> _knowledge_Contents = _IKnowledgeService.Get_Knowledge_ALL(_count, _page, _category);

            if (_knowledge_Contents.Count > 0)
            {
                List<Knowledge_content_Dto> _knowledge_Content_Dtos = m_Mapper.Map<List<Knowledge_content_Dto>>(_knowledge_Contents);
                return Ok(_knowledge_Content_Dtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion


        #region 內容區插入一個圖片(新增與修改時)
        /// <summary>
        /// 內容新增插入一個圖片:新增時,在內容裏(加入一個內容圖片)
        /// </summary>
        /// <returns>201;圖片URL Image_Url = _imageUrl</returns>
        [HttpPost]
        [Route("knowledge/image")]
        public IHttpActionResult AddContentImage()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request;  //取得使用者要求的Request物件
                #region 圖檔檢驗(需上傳)
                if (_request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                }
                if (_request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                }
                if (_imageFormatHelper.CheckImageMIME(_request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                }
                #endregion

                Knowledge_image knowledge_Image = _IKnowledgeService.AddUploadImage(_request);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                Knowledge_image_Dto knowledge_Image_Dto = m_Mapper.Map<Knowledge_image_Dto>(knowledge_Image);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, knowledge_Image_Dto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// 內容新增插入一個圖片:(修改)時(加入一個內容圖片)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>201;圖片URL Image_Url = _imageUrl</returns>
        [HttpPost]
        [Route("knowledge/image/{id}")]
        public IHttpActionResult AddContentImage(Guid id)
        {
            try
            {
                Knowledge_content _knowledge_Content = _IKnowledgeService.Get_Knowledge(id);
                if (_knowledge_Content != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;
                    #region 圖檔檢驗(需上傳)
                    if (_request.Files.Count == 0)      //這裏是檢查有沒有上傳圖片的欄位選項，不是檢查裏面有沒有值
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須有上傳圖片參數"));
                    }
                    if (_request.Files[0].ContentLength <= 0)    //如果有上欄位值，再檢查有沒有上傳檔案，若沒有則擋下，因為CheckImageMIME沒有檔案時會出錯
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "必須上傳圖片檔案"));
                    }
                    if (_imageFormatHelper.CheckImageMIME(_request.Files) == false) //這裏若file欄裏面沒有選檔案時還是會進入檢查
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                    }
                    #endregion

                    Knowledge_image knowledge_Image = _IKnowledgeService.AddUploadImage(_request, id);

                    // AutoMapper轉換型別
                    //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                    Knowledge_image_Dto knowledge_Image_Dto = m_Mapper.Map<Knowledge_image_Dto>(knowledge_Image);

                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, knowledge_Image_Dto));
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