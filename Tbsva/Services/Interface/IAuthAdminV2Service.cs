using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IAuthAdminV2Service
    {
        /// <summary>
        /// 帳號登入	Route：/AuthAdminV2/Login
        /// </summary>
        /// <param name="param">account,password</param>
        /// {
        ///    "account" : "account",
        ///    "password" : "password"
        /// }
        /// <returns></returns>
        bool Login(AuthParamV2 param);

        /// <summary>
        /// 取得登入資訊 /AuthAdminV2/LoginInfo
        /// </summary>
        /// <returns></returns>
        object LoginInfo();

        /// <summary>
        /// 登出/AuthAdmin/Logout
        /// </summary>
        void Logout();

    }
}
