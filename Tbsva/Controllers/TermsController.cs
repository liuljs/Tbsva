using WebShoppingAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebShopping.Models;
using WebShopping.Services;
using System.Net;
using System.Net.Http;
using WebShopping.Helpers;
using WebShopping.Dtos;
using WebShopping.Auth;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 服務條款政策
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    public class TermsController : ApiController
    {
        private ITermsService m_ITermsService;

        private IImageFormatHelper m_ImageFormatHelper;
        
        public TermsController(ITermsService p_ITermsService, IImageFormatHelper p_ImageFormatHelper)
        {
            m_ITermsService = p_ITermsService;
            m_ImageFormatHelper = p_ImageFormatHelper;
        }

        /// <summary>
        /// 取得編輯服務條款的內容(單一)
        /// </summary>
        /// <param name="id">圖片的id</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetContent(Guid id)
        {
            var content_ = m_ITermsService.GetContent(id);
            
            if (content_ != null)
            {
                SendTermsGetContentDto dto_ = ReturnGetContentItem(content_);

                return Ok(dto_);
            }
            else
            {
                return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// 轉換成GetContent回傳的類別
        /// </summary>
        /// <param name="p_strImgLink"></param>
        /// <returns></returns>
        private SendTermsGetContentDto ReturnGetContentItem(Terms p_Terms)
        {
            SendTermsGetContentDto dto_ = new SendTermsGetContentDto();
            dto_.Id = p_Terms.Id;
            dto_.Content = p_Terms.Content;
            dto_.Creation_Date = p_Terms.Creation_Date.ToString("yyyy/MM/dd HH:mm");

            return dto_;
        }

        /// <summary>
        /// 取得編輯服務條款的內容(全部)
        /// </summary>       
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetContents()
        {
            var contents_ = m_ITermsService.GetContents();
            
            if (contents_.Count > 0)
            {
                List<SendTermsGetContentDto> dtos_ = ReturnGetContentItems(contents_);

                return Ok(dtos_);
            }
            else
            {
                return Ok();//StatusCode(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// 轉換成GetContents回傳的類別
        /// </summary>
        /// <param name="p_strImgLink"></param>
        /// <returns></returns>
        private List<SendTermsGetContentDto> ReturnGetContentItems(List<Terms> p_Terms)
        {
            List<SendTermsGetContentDto> dtos_ = new List<SendTermsGetContentDto>();
            for(int i=0; i <p_Terms.Count; i++)
            {
                SendTermsGetContentDto dto_ = new SendTermsGetContentDto();
                dto_.Id = p_Terms[i].Id;
                dto_.Content = p_Terms[i].Content;
                dto_.Creation_Date = p_Terms[i].Creation_Date.ToString("yyyy/MM/dd HH:mm");

                dtos_.Add(dto_);
            }

            return dtos_;
        }


        #region By Form-Data 格式
        /// <summary>
        /// 新增編輯服務條款的內容(1筆)
        /// By Form-Data 格式
        /// </summary>
        /// <returns></returns>
        /*   [HttpPost]
           public IHttpActionResult AddContent()
           {
                Check if the request contains multipart/form-data.
               if (!Request.Content.IsMimeMultipartContent())
               {
                   return StatusCode(HttpStatusCode.UnsupportedMediaType);
               }

              try
               {
                   清空資料表所有內容
                   m_ITermsService.DeleteAllContents();

                   var result_ = m_ITermsService.AddContent(HttpContext.Current.Request);    //新增1筆內容

                   return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, result_));

               }
               catch (Exception ex)
               {
                   return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
               }
           }*/
        #endregion

        #region  By Json 格式
        /// <summary>
        /// 新增編輯服務條款的內容(1筆)       
        /// By Json 格式
        /// </summary>
        /// <returns></returns>
        [Route("Terms/AddContent")]
        [HttpPost]
        public IHttpActionResult AddContent(Terms p_abountMe)
        {
            try
            {
                //清空資料表所有內容
                m_ITermsService.DeleteAllContents();

                var result_ = m_ITermsService.AddContent(p_abountMe);    //新增1筆內容

                SendTermsGetContentDto dto_ =ReturnGetContentItem(result_);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, dto_));

            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        [Route("Terms/AddImage")]
        [HttpPost]
        /// <summary>
        /// 新增編輯服務條款的圖片(1筆) 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IHttpActionResult AddImage()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            HttpRequest _request = HttpContext.Current.Request;

            if (_request.Files.Count == 0)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片檔案"));
            }

            if (m_ImageFormatHelper.CheckImageMIME(_request.Files) == false)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "圖片格式錯誤"));
            }

            try
            {
                var _result = m_ITermsService.AddImage(_request); //新增圖片
                SendTermsAddImageDto dto_ = ReturnAddImageItmes(_result);
                //return Ok(dto_);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, dto_));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// 轉換成AddImage回傳的類別
        /// </summary>
        /// <param name="p_strImgLink"></param>
        /// <returns></returns>
        private SendTermsAddImageDto ReturnAddImageItmes(string p_strImgLink)
        {
            SendTermsAddImageDto dto_ = new SendTermsAddImageDto();
            dto_.Image_Link = p_strImgLink;

            return dto_;
        }
    }
}