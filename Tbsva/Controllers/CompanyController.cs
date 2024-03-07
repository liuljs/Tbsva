using WebShopping.Auth;
using WebShoppingAdmin.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Http;

namespace WebShoppingAdmin.Controllers
{
    /// <summary>
    /// 公司資訊
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    [RoutePrefix("Company")]
    public class CompanyController : ApiController
    {
        [HttpPost]
        [Route("Get")]
        public object Get()
        {
            try
            {
                using (Company obj = new Company())
                {
                    return new ApiResult(obj.Get());
                }
            }
            catch (Exception ex)
            {
                return new ErrApiResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("Update")]
        public object Update(CompanyUpdateParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (Company obj = new Company())
                    {
                        obj.Update(param);
                        return new ApiResult(obj.Get());
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
