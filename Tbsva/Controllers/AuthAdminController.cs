using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebShopping.Auth;
using WebShoppingAdmin.Models;

namespace WebShoppingAdmin.Controllers
{
    /// <summary>
    /// 後台登入
    /// </summary>
    [CustomAuthorize(Role.Admin)]   
    [RoutePrefix("AuthAdmin")]
    public class AuthAdminController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public object Login(AuthParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (AuthAdmin obj = new AuthAdmin())
                    {
                        return obj.Login(param) ? new ApiResult() : new ErrApiResult("帳號或密碼錯誤");
                    }
                }
                catch (Exception ex)
                {
                    return new ErrApiResult(ex.Message);
                }
            }
            else
                return new ErrApiResult(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            try
            {
                using (AuthAdmin obj = new AuthAdmin())
                {
                    obj.Logout();
                    return Ok(new ApiResult());
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
                //return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));              
            }
        }

        [HttpPost]
        [Route("LoginInfo")]
        public object LoginInfo()
        {
            try
            {
                using (AuthAdmin obj = new AuthAdmin())
                {
                    return new ApiResult(obj.GetUser());
                }
            }
            catch (Exception ex)
            {
                return new ErrApiResult(ex.Message);
            }
        }
    }
}
