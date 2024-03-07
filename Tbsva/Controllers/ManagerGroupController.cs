using WebShopping.Auth;
using WebShoppingAdmin.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebShoppingAdmin.Controllers
{
    /// <summary>
    /// 管理者群組
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    [RoutePrefix("ManagerGroup")]
    public class ManagerGroupController : ApiController
    {
        /// <summary>
        /// /ManagerGroup/Get 取得管理群組
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Get")]
        public object Get([FromBody] ManagerGroupGetParam param)
        {
            try
            {
                using (ManagerGroup obj = new ManagerGroup())
                {
                    Guid? id = param?.id;  //id可以等於null

                    return new ApiResult(obj.Get(id));
                }
            }
            catch (Exception ex)
            {
                return new ErrApiResult(ex.Message);
            }
        }

        /// <summary>
        /// 新增群組並在新增模組，新增管理群組(新)
        /// </summary>
        /// <param name="param">群組加模組</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Insert")]
        public object Insert(ManagerGroupInsertParam param)
        {
            param.id = Guid.NewGuid(); //新增的群組ID

            // 無視 id 的驗證錯誤
            if (ModelState.IsValid)
            {
                try
                {
                    using (ManagerGroup obj = new ManagerGroup())
                    {
                        obj.Insert(param); //1.新增
                        return new ApiResult(obj.Get(param.id));  //2.取群組下的模組
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
        /// 更新管理群組/ManagerGroup/Update
        /// </summary>
        /// <param name="param">以param.id 群組ID起頭查詢</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Update")]
        public object Update(ManagerGroupUpdateParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (ManagerGroup obj = new ManagerGroup())
                    {
                        obj.Update(param);
                        return new ApiResult(obj.Get(param.id));
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
        /// 刪除管理群組ManagerGroup/Delete
        /// </summary>
        /// <param name="param">以param.id 群組ID起頭查詢</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public object Delete(ManagerGroupDeleteParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (ManagerGroup obj = new ManagerGroup())
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
        /// 更新管理群組與模組的關聯
        /// 這功能像是在把某個群組下的模組換成另一個群組下的模組，目前看起來用不到
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateLnk")]
        public object UpdateLnk(ManagerGroupLnkParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (ManagerGroup obj = new ManagerGroup())
                    {
                        obj.UpdateLnk(param);
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
    }
}
