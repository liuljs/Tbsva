using WebShopping.Auth;
using WebShoppingAdmin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebShoppingAdmin.Controllers
{
    /// <summary>
    /// 後台管理者管理
    /// </summary>
    [CustomAuthorize(Role.Admin)]    
    [RoutePrefix("Manager")]
    public class ManagerController : ApiController
    {
        [HttpPost]
        [Route("Get")]
        public object Get(ManagerGetParam managerGetParam)
        {
            try
            {
                using (Manager obj = new Manager())
                {
                    return new ApiResult(obj.Get(managerGetParam?.id));   //id可以等於null

                    //Guid? id = managerGetParam?.id;  //id可以等於null

                    //return new ApiResult(obj.Get(id));
                }
            }
            catch (Exception ex)
            {
                return new ErrApiResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("Insert")]
        public object Insert(ManagerInsertParam param)
        {
            param.account_id = Guid.NewGuid();

            // 無視 id 的驗證錯誤
            if (ModelState.IsValid)
            {
                try
                {
                    using (Manager obj = new Manager())
                    {            
                        obj.Insert(param);
                        return new ApiResult(obj.Get(param.account_id));
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

        [HttpPost]
        [Route("Update")]
        public object Update(ManagerUpdateParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (Manager obj = new Manager())
                    {
                        obj.Update(param);
                        return new ApiResult(obj.Get(param.account_id));
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

        [HttpPost]
        [Route("Delete")]
        public object Delete(ManagerDeleteParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (Manager obj = new Manager())
                    {
                        obj.Delete(param);
                        return new ApiResult();
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

        /// <summary>
        /// 修改自己管理密碼
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdatePassword")]
        public object UpdatePasswrod(ManagerUpdatePassWordParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (Manager obj = new Manager())
                    {
                        string strResult_ = obj.UpdatePassword(param);

                        return strResult_ == string.Empty ? new ApiResult() : new ErrApiResult(strResult_);
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


        [HttpPost]
        [Route("ResetPassword")]
        public object ResetPasswrod(ManagerResetPassWordParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (Manager obj = new Manager())
                    {
                        string strResult_ = obj.ResetPassword(param);

                        return strResult_ == string.Empty ? new ApiResult() : new ErrApiResult(strResult_);
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
