using AutoMapper;
using WebShopping.Auth;
using WebShopping.Dtos;
using WebShopping.Helpers;
using WebShopping.Models;
using WebShopping.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 後台Faq管理
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    public class FaqController : ApiController
    {
        private IFaqService m_faqService;

        private IImageFormatHelper m_imageFormatHelper;

        private IMapper m_Mapper;

        public FaqController(IFaqService faqService, IImageFormatHelper imageFormatHelper, IMapper mapper)
        {
            m_faqService = faqService;
            m_imageFormatHelper = imageFormatHelper;
            m_Mapper = mapper;
        }

        /// <summary>
        /// 取得單筆消息資料
        /// </summary>
        /// <param name="id">消息ID</param>
        /// <returns>200;單筆消息資料</returns>
        [AllowAnonymous]
        public IHttpActionResult GetFaq(int id)
        {
            Faq _faq = m_faqService.GetFaqData(id);

            if (_faq != null)
            {
                FaqGetDto _faqGetDto = SwitchFaqData(_faq);

                return Ok(_faqGetDto);
            }
            else
            {
                return Ok();//return NotFound();
            }
        }

        private FaqGetDto SwitchFaqData(Faq faq)
        {
            FaqGetDto _faqGetDto = new FaqGetDto();

            _faqGetDto.Id = faq.Id;
            _faqGetDto.Question = faq.Question;
            _faqGetDto.Asked = faq.Asked;
            _faqGetDto.Sort = faq.Sort;
            _faqGetDto.Enabled = faq.Enabled;

            return _faqGetDto;
        }

        /// <summary>
        /// 取得全部消息資料
        /// </summary>
        /// <returns>200;全部消息資料</returns>
        [AllowAnonymous]
        public IHttpActionResult GetFaq()
        {
            List<Faq> _faq = m_faqService.GetFaqSetData();

            if (_faq.Count > 0)
            {
                List<FaqGetDto> _faqGetDtos = SwitchFaqSetData(_faq);

                return Ok(_faqGetDtos);
            }
            else
            {
                return Ok();//return NotFound();
            }
        }

        private List<FaqGetDto> SwitchFaqSetData(List<Faq> faq)
        {
            List<FaqGetDto> _faqGetDtos = new List<FaqGetDto>();

            for (int i = 0; i < faq.Count; i++)
            {
                FaqGetDto _faqGetDto = new FaqGetDto();

                _faqGetDto.Id = faq[i].Id;
                _faqGetDto.Question = faq[i].Question;
                _faqGetDto.Asked = faq[i].Asked;
                _faqGetDto.Sort = faq[i].Sort;
                _faqGetDto.Enabled = faq[i].Enabled;

                _faqGetDtos.Add(_faqGetDto);
            }

            return _faqGetDtos;
        }

        /// <summary>
        /// 新增一筆最新消息
        /// </summary>
        /// <returns>201;新增消息資料</returns>
        [HttpPost]
        public IHttpActionResult InsertFaq()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件

                if (string.IsNullOrWhiteSpace(_request.Form.Get("Question")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Question") == null ? "必須有question參數" : "question參數格式錯誤"));
                }

                if (string.IsNullOrWhiteSpace(_request.Form.Get("Asked")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Asked") == null ? "必須有Asked參數" : "Asked參數格式錯誤"));
                }

                if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                }

                if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                }


                Faq _faq = m_faqService.InsertFaq(_request);

                FaqInsertDto _faqInsertDto = new FaqInsertDto();
                _faqInsertDto.Id = _faq.Id;
                _faqInsertDto.Question = _faq.Question;
                _faqInsertDto.Asked = _faq.Asked;
                _faqInsertDto.Sort = _faq.Sort;
                _faqInsertDto.Enabled = _faq.Enabled;

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, _faqInsertDto));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

         /// <summary>
        /// 更新一筆Faq
        /// </summary>
        /// <param name="id">Faq ID</param>
        /// <returns>204</returns>
        [HttpPut]
        public IHttpActionResult UpdateNew(int id)
        {
            try
            {
                Faq _faq = m_faqService.GetFaqData(id);

                if (_faq != null)
                {
                    HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件

                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Question")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Question") == null ? "必須有question參數" : "question參數格式錯誤"));
                    }

                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Asked")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Asked") == null ? "必須有Asked參數" : "Asked參數格式錯誤"));
                    }

                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Sort")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Sort") == null ? "必須有Sort參數" : "Sort參數格式錯誤"));
                    }

                    if (string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, _request.Form.Get("Enabled") == null ? "必須有Enabled參數" : "Enabled參數格式錯誤"));
                    }

                    m_faqService.UpdateFaq(_request, _faq);

                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return Ok();//return NotFound();
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// 刪除一筆最新消息
        /// </summary>
        /// <param name="id">消息ID</param>
        /// <returns>204</returns>
        [HttpDelete]
        public IHttpActionResult DeleteFaq(int id)
        {
            Faq _faq = m_faqService.GetFaqData(id);

            if (_faq != null)
            {
                m_faqService.DeleteFaq(_faq);

                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return Ok();//return NotFound();
            }
        }
    }
}
