using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebShopping.Auth;
using WebShoppingAdmin.Models;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 後台模組管理
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    [RoutePrefix("Module")]
    public class ModuleController : ApiController
    {
        [HttpPost]
        [Route("Get")]
        public object Get()
        {
            try
            {
                using (Module obj = new Module())
                {
                    return new ApiResult(obj.Get());
                }
            }
            catch (Exception ex)
            {
                return new ErrApiResult(ex.Message);
            }
        }
    }
}
