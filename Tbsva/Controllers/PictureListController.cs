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
    public class PictureListController : ApiController
    {
        #region DI依賴注入功能
        private readonly IPictureListService _IPictureListService;
        private IMapper m_Mapper;
        private IImageFormatHelper _imageFormatHelper;

        /// <summary>
        /// DI依賴注入功能
        /// </summary>
        /// <param name="IPictureListService">介面</param>
        /// <param name="mapper">自動轉型</param>
        /// <param name="IImageFormatHelper">透過MIME檢查上傳檔案是否為圖檔</param>
        public PictureListController(IPictureListService IPictureListService, IMapper mapper, IImageFormatHelper IImageFormatHelper)
        {
            _IPictureListService = IPictureListService;
            m_Mapper = mapper;
            _imageFormatHelper = IImageFormatHelper;
        }
        #endregion

        #region 修改一筆資料
        // PUT: /LightingContent/5
        [HttpPut]
        public IHttpActionResult UpdatePictureList()
        {
            try
            {
                //1.先取出要修改的資料
                PictureList _pictureList = _IPictureListService.Get_PictureList();
                if (_pictureList != null)
                {
                    HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件

                    //取得用戶端所上傳的檔案集合HttpRequest.Files 屬性
                    HttpFileCollection fileCollection = _request.Files;

                    //處理多張圖檔上傳檢查，欄位選項有勾選才檢查，修改時上傳圖檔非必選
                    if (fileCollection.Count > 0)      //Postman左邊欄位有勾才會計算不然會是NULL
                    {
                        for (int i = 0; i < fileCollection.Count; i++)
                        {
                            HttpPostedFile _file = fileCollection[i];
                            string _type = _file.ContentType;  //辨斷檔案格式，若沒有上傳會是null
                            //所以先辨斷有沒有容量，有在辨斷圖片格式有沒有錯
                            if (_type != null && _file.ContentLength > 0)      //沒有檔時會出現null，確定有上傳檔在檢查
                            {
                                if (!_type.Contains("image")) 
                                {
                                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片格式錯誤"));
                                }
                            }
                        }
                    }

                    //修改
                    _IPictureListService.Update_PictureList(_request, fileCollection, _pictureList);

                    //再取出一次
                    //_pictureList = _IPictureListService.Get_PictureList();

                    //回傳
                    return StatusCode(HttpStatusCode.NoContent);
                    //return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, _pictureList));
                    //return Ok(_pictureList);
                }
                else
                {
                    return NotFound();
                }  
            }
            catch (Exception ex)
            {
                SystemFunctions.WriteLogFile($"{ex.Message}\n{ex.StackTrace}");
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
        #endregion

        #region 取得所有內容(只會有一筆資料)
        // GET: /PictureList/
        [AllowAnonymous]
        public IHttpActionResult GetPictureList()
        {
            PictureList _pictureList = _IPictureListService.Get_PictureList();

            // AutoMapper轉換型別
            //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
            PictureListDto _pictureListDto = m_Mapper.Map<PictureListDto>(_pictureList);

            if (_pictureList != null)
            {
                return Ok(_pictureListDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion


    }
}
