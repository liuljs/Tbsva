using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebShopping.Auth;
using WebShopping.Models;
using WebShopping.Services;
using WebShoppingAdmin.Models;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 後台登入
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    [RoutePrefix("AuthAdminV2")]
    public class AuthAdminV2Controller : ApiController
    {
        #region DI依賴注入功能
        private IAuthAdminV2Service _AuthAdminV2Service;
        private IMapper Mapper;

        public AuthAdminV2Controller(IAuthAdminV2Service authAdminV2Service, IMapper mapper)
        {
            _AuthAdminV2Service = authAdminV2Service;
            Mapper = mapper;
        }
        #endregion

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        // POST: api/AuthAdminV2/Login
        public object GetAuthAdminV2Login([FromBody] AuthParamV2 authParamV2)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //1.先登入找資料
                    bool check = _AuthAdminV2Service.Login(authParamV2);
                    if (check == true)
                    {
                        //return new ApiResult();
                        return Ok(new ApiResult());
                        //return Ok(new
                        //{
                        //    Result = "Ok",
                        //    Content = "null"
                        //});
                    }
                    else
                    {
                        //return obj.Login(param) ? new ApiResult() : new ErrApiResult("帳號或密碼錯誤");
                        //new ErrApiResult("帳號或密碼錯誤");
                        return Ok(new ErrApiResult("帳號或密碼錯誤"));
                        //return Content(HttpStatusCode.OK, new ErrApiResult("帳號或密碼錯誤") );
                    }
                }
                catch (Exception ex)
                {
                    return new ErrApiResult(ex.Message);
                }
            }
            else
            {
                return new ErrApiResult(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
            }
        }


        //https://www.itread01.com/content/1548408965.html
        [HttpPost]
        [Route("LoginInfo")]
        public object LoginInfo()
        {
            try
            {
                object objects = _AuthAdminV2Service.LoginInfo();
                return new ApiResult(objects);
                //return Content(HttpStatusCode.OK, objects);  //這個會沒有頭，若沒有資料時

                //return Ok(new {
                //    //message = "abc",
                //    objects
                //});

            }
            catch (Exception ex)
            {
                return new ErrApiResult(ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            try
            {
                _AuthAdminV2Service.Logout();
                //return Ok(new ApiResult());
                return Ok(new ApiResult());
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
                //return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));              
            }
        }


    }
}
