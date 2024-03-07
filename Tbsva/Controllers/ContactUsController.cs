using WebShopping.Auth;
using WebShoppingAdmin.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Http;

namespace WebShoppingAdmin.Controllers
{
    /// <summary>
    /// 聯絡我們，目前沒有入資料庫，所以只需開放前台使用
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    [RoutePrefix("ContactUs")]
    public class ContactUsController : ApiController
    {
        //[HttpPost]
        //[Route("Get")]
        //public object Get()
        //{
        //    try
        //    {
        //        using (ContactUs obj = new ContactUs())
        //        {
        //            return new ApiResult(obj.Get());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ErrApiResult(ex.Message);
        //    }
        //}

        [HttpPost]
        [AllowAnonymous]
        //[Route("Insert")]
        public object Insert(ContactUsInsertParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (ContactUs obj = new ContactUs())
                    {
                        obj.Insert(param);
                        return new ApiResult();//obj.Get()
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
    }
}
