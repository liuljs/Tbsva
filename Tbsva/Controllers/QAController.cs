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
    public class QAController : ApiController
    {
        #region DI依賴注入功能
        private IQAService _IQAService;         //欄位
        private IMapper m_Mapper;
        /// <summary>
        ///  DI依賴注入功能
        /// </summary>
        /// <param name="QAService">內容輸入</param>
        /// <param name="mapper">AutoMapper</param>
        public QAController(IQAService QAService, IMapper mapper)
        {
            _IQAService = QAService;         //參數
            m_Mapper = mapper;
        }
        #endregion

        #region 新增
        // POST: /QA
        [HttpPost]
        public IHttpActionResult InsertQA()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                if (string.IsNullOrWhiteSpace(_request.Form.Get("Question")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Question") == null ? "必須有Question參數" : "Question參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("Answer")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Answer") == null ? "必須有Answer參數" : "Answer參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                }
                if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                }

                QA _qa = _IQAService.InsertQA(_request);        //交由_介面.實作處理把新增的資料在傳出_qa

                // AutoMapper轉換型別
                //宣告Dto型別 dto = 轉換後<Dto型別>(要轉換的來源類別)
                QA_Dto _qa_Dto = m_Mapper.Map<QA_Dto>(_qa);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _qa_Dto));
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");   //有錯誤時會寫入App_Data\Log檔案
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取一筆資料
        // GET: /QA/5
        [AllowAnonymous]
        public IHttpActionResult GetQA(int id)
        {
            QA _qa = _IQAService.GetQA(id);  //給id,抓出資料
            if (_qa != null)
            {
                QA_Dto _qa_Dto = m_Mapper.Map<QA_Dto>(_qa);
                return Ok(_qa_Dto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 更新一筆資料
        // PUT: /QA/5
        [HttpPut]
        public IHttpActionResult UpdateQA(int id)
        {
            try
            {               
                QA _qa = _IQAService.GetQA(id);  //1.先取出要修改的資料
                if (_qa != null)
                {
                    HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Question")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Question") == null ? "必須有Question參數" : "Question參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Answer")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Answer") == null ? "必須有Answer參數" : "Answer參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                    }
                    //修改
                    _IQAService.UpdateQA(_request, _qa);
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
        // DELETE: /QA/5
        [HttpDelete]
        public IHttpActionResult DeleteQA(int id)
        {
            QA _qa = _IQAService.GetQA(id);  //抓出要刪除的資料
            if (_qa != null)
            {
                _IQAService.DeleteQA(_qa);
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 取得所有內容
        // GET: /QA
        [AllowAnonymous]
        public IHttpActionResult GetQA()
        {
            HttpRequest _request = HttpContext.Current.Request;   //取得使用者要求的Request物件
            int? _count = null;
            int? _page = null;

            if (!string.IsNullOrWhiteSpace(_request["count"]))
            {
                _count = Convert.ToInt32(_request["count"]);
            }
            if (!string.IsNullOrWhiteSpace(_request["page"]))
            {
                _page = Convert.ToInt32(_request["page"]);
            }

            List<QA> _qas = _IQAService.GetQA(_count, _page);  //取出所有資料
            if (_qas.Count > 0)
            {
                List<QA_Dto> _qa_Dtos = m_Mapper.Map<List<QA_Dto>>(_qas);
                return Ok(_qa_Dtos);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

    }
}
