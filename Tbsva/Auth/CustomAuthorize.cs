using WebShopping.Connection;
using WebShoppingAdmin.Models;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Security;

namespace WebShopping.Auth
{
    public enum RoleType
    {
        Guest = 0,
        User = 1,
        Admin = 2,
        Unknown = 9
    }

    /// <summary>
    /// 各功能的權限控制, 名稱與RoleType一致
    /// </summary>
    public class Role
    {
        public const string Guest = nameof(RoleType.Guest);
        public const string User = nameof(RoleType.User);
        public const string Admin = nameof(RoleType.Admin);
        public const string Unknown = nameof(RoleType.Unknown);

        public static string GetIdentityRole(int role)
        {
            string _role = Unknown;

            switch (role)
            {
                case 0:
                    _role = Guest;
                    break;
                case 1:
                    _role = User;
                    break;
                case 2:
                    _role = Admin;
                    break;
            }

            return _role;
        }

        /// <summary>
        /// 是否是後台管理者
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                bool isadmin = HttpContext.Current.User.IsInRole("Admin");
                return isadmin;
            }
        }
    }

    public class IdentityData
    {
        public string Identity { get; set; }
    }

    public class CustomAuthorize : System.Web.Http.AuthorizeAttribute
    {
        /// <summary>
        /// 設定權限的角色
        /// </summary>
        /// <param name="roles">角色陣列集合</param>
        public CustomAuthorize(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }

        /// <summary>
        /// api 權限登入後必經驗證點
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //20220127暫時關閉後台登入驗證以下註解

            var currentIdentity = actionContext.RequestContext.Principal.Identity;
            if (!currentIdentity.IsAuthenticated) //確認是否經過登入驗證
                return false;

            var session = HttpContext.Current.Session; //宣告Session
            var AuthModule = session["AuthModule"]; //登入時載入模組的session
            if (AuthModule == null)  //當初存的模組權限若消失也要擋
                return false;

            var _userName = currentIdentity.Name; //User Id
            var _formsIdentity = (FormsIdentity)currentIdentity;
            var data = _formsIdentity.Ticket.UserData; //取得UserData的字串
            var _identityData = JsonConvert.DeserializeObject<IdentityData>(data); //解析登入時儲存的UserData

            IPrincipal _user = actionContext.ControllerContext.RequestContext.Principal; //取得目前的User Role
            _user = new GenericPrincipal(currentIdentity, Roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)); //設定User Role

            if (_user.IsInRole(_identityData.Identity))
            {
                return true;
            }
            else
            {
                return false;
            }

            //return true;  //20220127暫時關閉後台登入驗證，要取消請刪除此行，並將上面註解移除
        }

        //private void GetUserModule(string accountId)
        //{
        //    List<Module> _modules = new List<Module>();

        //    using (var _cn = new ConnectionFactory().CreateConnection())
        //    {
        //        string _sql = @"SELECT A.[CODE],A.[]
        //                        FROM [MODULE] A 
        //                        WHERE ID = @ID";

        //        _modules = _cn.Query<Module>(_sql, new { ID = accountId }).ToList();
        //    }
        //}
    }
}