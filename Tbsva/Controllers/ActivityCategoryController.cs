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
using WebShopping.Models;
using WebShopping.Services;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 後台權限需登入設定Admin，全域
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    ///RoutePrefix路由前綴
    [RoutePrefix("activity")]
    public class ActivityCategoryController : ApiController
    {
        #region DI依賴注入功能
        private IActivityCategoryService activityCategoryService;
        private IMapper mapper;
        public ActivityCategoryController(IActivityCategoryService activityCategoryService, IMapper mapper)
        {
            this.activityCategoryService = activityCategoryService;
            this.mapper = mapper;
        }
        #endregion

        //public HttpResponseMessage Post(Product product)
        //{
        ////    if (ModelState.IsValid)
        ////    {
        ////        // Do something with the product (not shown).

        ////        return new HttpResponseMessage(HttpStatusCode.OK);
        ////    }
        ////    else
        ////    {
        ////        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        ////    }
        //}

        #region 新增
        // POST: api/activity/category
        [HttpPost]
        [Route("category")]
        //public IHttpActionResult GetActivityCategory(ActivityCategory activityCategory) 使用ModelState驗證時改用這個
        public IHttpActionResult GetActivityCategory()
        {
            //if (!ModelState.IsValid)
            //{
            //    // ModelState錯誤訊息
            //    var ModelStateErrors = ModelState.Where(x => x.Value.Errors.Count > 0)
            //        .ToDictionary(
            //            k => k.Key,
            //            k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            //        );
            //}
            //if (ModelState.IsValid)
            //{
            //    // Do something with the product (not shown).
            //    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, activityCategoryDto));
            //}
            //else
            //{
            //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            //}
            //{

            //    "Message": "要求無效。",
            //    "ModelState": {
            //                    "activityCategory.name": [
            //                        "欄位 name 必須是最大長度為 20 的字串。"
            //        ],
            //        "activityCategory.berif": [
            //            "欄位 berif 必須是最大長度為 40 的字串。"
            //        ]
            //    }
            //}
            try
            {
                HttpRequest httpRequest = HttpContext.Current.Request;
                //if (ModelState.IsValid)
                //{
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("name")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("name") == null ? "必須有name欄位" : "name參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                        //CreateErrorResponse 沒有泛型會錯return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled參數" : new apiNewResultSuccess("400", "errorToBadRequest", "name參數格式錯誤" , "", DateTime.Now)));
                    }
                    ActivityCategory activityCategory = activityCategoryService.InsertActivityCategory(httpRequest);
                    // AutoMapper轉換型別
                    //宣告Dto型別 dto = 轉換<Dto型別>(要轉換的類別)
                    ActivityCategoryDto activityCategoryDto = mapper.Map<ActivityCategoryDto>(activityCategory);
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, activityCategoryDto));
                //}
                    //else
                //{
                //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));

                    //{
                    //    "Message": "欄位 name 必須是最大長度為 20 的字串。"
                    //}
                    //var Aa = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage;
                    //return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Aa));

                    //"欄位 name 必須是最大長度為 20 的字串。"
                    //var _errMsg = ModelState.Values.Where(x => x.Errors.Count > 0)?.FirstOrDefault()?.Errors.Select(y => y.ErrorMessage).FirstOrDefault();
                    //return Content(HttpStatusCode.BadRequest, _errMsg);

                    //這個會將所有錯誤列出"欄位 name 必須是最大長度為 20 的字串。; 欄位 berif 必須是最大長度為 40 的字串。"
                    //ModelState.Values 看起來像陣列
                    //var _errMsg2 = string.Join("; ", ModelState.Values
                    //    .SelectMany(x => x.Errors)
                    //    .Where(x => !string.IsNullOrEmpty(x.ErrorMessage))
                    //    .Select(x => x.ErrorMessage));
                    //return Content(HttpStatusCode.BadRequest, _errMsg2);
                    //測試不同錯誤
                //}
                //return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, new WebShoppingAdmin.Models.ApiResult(activityCategoryDto)));
                // 測試return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, new apiNewResultSuccess(HttpStatusCode.Created , Message.successToGetContent.ToString(), Detail.登入成功.ToString(), activityCategoryDto, Helpers.Tools.Formatter.FormatDate(DateTime.Now))));
                //return ApiResponse(new ReturnMessage { Status = ResultStatus.Failed, Message = error });
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
                //return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ModelState));
            }
        }
        #endregion

        #region 取得一筆資料
        // Get: api/activity/category/{categoryId}
        [AllowAnonymous]
        [HttpGet]  ///這裏不用設定也可以正確抓到
        [Route("category/{id}")]        // 這裏要設定id, 路由上是id,也要設定, 原本若沒有設定Route，其實是不需要設定{id} routeTemplate: "{controller}/{action}/{id}"
        public IHttpActionResult GetActivityCategory(Guid id)  //這裏不能使用category_id，路由id = RouteParameter.Optional
        {
            ActivityCategory activityCategory = activityCategoryService.GetActivityCategory(id);
            if (activityCategory !=null)
            {
                ActivityCategoryDto activityCategoryDto = mapper.Map<ActivityCategoryDto>(activityCategory);
                return Ok(activityCategoryDto);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region 修改資料
        //PUT : api/activity/category/{categoryId}
        [HttpPut]
        [Route("category/{id}")]
        public IHttpActionResult UpdateActivityCategory (Guid id)
        {
            try
            {
                ActivityCategory activityCategory = activityCategoryService.GetActivityCategory(id);  //先取出要修改的資料
                if (activityCategory != null)
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    //"必須有name欄位" : "name參數格式錯誤", 左邊是缺少這個欄位，右邊是沒有值
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("name")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("name") == null ? "必須有name欄位" : "name參數格式錯誤"));
                    }
                    if (string.IsNullOrWhiteSpace(httpRequest.Form.Get("enabled")))
                    {
                        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpRequest.Form.Get("enabled") == null ? "必須有enabled參數" : "enabled參數格式錯誤"));
                    }
                    activityCategoryService.UpdateActivityCategory(httpRequest, activityCategory);

                    return StatusCode(HttpStatusCode.NoContent);
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

        #region 刪除一筆資料
        // DELETE: api/activity/category/{categoryId}
        [HttpDelete]
        [Route("category/{id}")]        
        public IHttpActionResult DeleteActivityCategory(Guid id)
        {
            try
            {
                ActivityCategory activityCategory = activityCategoryService.GetActivityCategory(id);  //先取出要刪的資料
                if (activityCategory != null)
                {
                    activityCategoryService.DeleteActivityCategory(activityCategory);
                    return StatusCode(HttpStatusCode.NoContent);
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

        #region 取得全部目錄
        // Get: api/activity/category/
        [AllowAnonymous]
        [HttpGet]            ///這裏要設定不會自動辨斷"Message": "要求的資源不支援 http 方法 'GET'。"
        [Route("category")]
        public IHttpActionResult ActivityCategories()
        {
            try
            {
                List<ActivityCategory> activityCategories = activityCategoryService.GetActivityCategories();
                if (activityCategories != null)
                {
                    // 原資料庫ActivityCategory型別 AutoMapper 多筆轉換成ActivityCategoryDto型別
                    List<ActivityCategoryDto> activityCategoryDtos = mapper.Map<List<ActivityCategoryDto>>(activityCategories);

                    return Ok(activityCategoryDtos);
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
