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
    public class Knowledge2Controller : ApiController
    {
        #region DI依賴注入功能
        private IKnowledge2Service _IKnowledge2Service;  //欄位
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;

        /// <summary>
        /// DI依賴注入功能
        /// </summary>
        /// <param name="iKnowledge2Service">內容輸入</param>
        /// <param name="mapper">AutoMapper</param>
        /// <param name="imageFormatHelper"></param>
        public Knowledge2Controller(IKnowledge2Service iKnowledge2Service, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            _IKnowledge2Service = iKnowledge2Service;
            m_Mapper = mapper;
            _imageFormatHelper = imageFormatHelper;
        }
        #endregion

        #region 新增
        // POST: /knowledge2
        [HttpPost]
        public IHttpActionResult InsertKnowledge2()
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

                Knowledge_content2 _knowledge_Content2 = _IKnowledge2Service.Insert_Knowledge2(_request);

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                Knowledge_content2_Dto _knowledge_Content2_Dto = m_Mapper.Map<Knowledge_content2_Dto>(_knowledge_Content2);

                //return Request.CreateErrorResponse這裏弄錯時會出現引數3無法從轉換string
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _knowledge_Content2_Dto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得一筆資料
        // GET: /Knowledge2/{knowledgetId}
        [AllowAnonymous]
        //[Route("knowledge2/{knowledgetId}")]
        public IHttpActionResult GetKnowledge2(Guid id)
        {
            Knowledge_content2 _knowledge_Content2 = _IKnowledge2Service.Get_Knowledge2(id);
            if (_knowledge_Content2 != null)
            {
                Knowledge_content2_Dto _knowledge_Content2_Dto = m_Mapper.Map<Knowledge_content2_Dto>(_knowledge_Content2);
                return Ok(_knowledge_Content2_Dto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 更新一筆資料
        // PUT: /knowledge2/{knowledgetId}
        [HttpPut]
        //[Route("knowledge2/{knowledgetId}")]
        public IHttpActionResult UpdateKnowledge2(Guid id)
        {
            try
            {
                //1.先取出要修改的資料
                Knowledge_content2 _knowledge_Content2 = _IKnowledge2Service.Get_Knowledge2(id);
                //2.辨斷有無資料，並辨斷接收進來的資料
                if (_knowledge_Content2 != null)
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

                    //修改
                    _IKnowledge2Service.Update_Knowledge2(_request, _knowledge_Content2);
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
        // DELETE: /knowledge2/{knowledgetId}
        [HttpDelete]
        public IHttpActionResult DeleteKnowledge2(Guid id)
        {
            Knowledge_content2 _knowledge_Content2 = _IKnowledge2Service.Get_Knowledge2(id);
            if (_knowledge_Content2 != null)
            {
                _IKnowledge2Service.Delete_Knowledge2(_knowledge_Content2);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: /Knowledge2
        [AllowAnonymous]
        public IHttpActionResult GetKnowledge2()
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

            List<Knowledge_content2> _knowledge_Content2s = _IKnowledge2Service.Get_Knowledge2_ALL(_count, _page, _category);

            if (_knowledge_Content2s.Count > 0)
            {
                List<Knowledge_content2_Dto> _knowledge_Content2_Dtos = m_Mapper.Map<List<Knowledge_content2_Dto>>(_knowledge_Content2s);
                return Ok(_knowledge_Content2_Dtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
    }
}