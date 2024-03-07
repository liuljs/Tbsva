
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebShopping.Auth;

namespace WebShoppingAdmin.Models
{
    public class BaseAuth : BaseModel
    {   
      
         public void Logout()
        {
            FormsAuthentication.SignOut();

            if (HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                cookie.HttpOnly = true;
                cookie.Domain = HttpContext.Current.Request.UrlReferrer?.DnsSafeHost;
                cookie.Expires = DateTime.MinValue;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public object GetUser()
        {
            //取得 ASP.NET 使用者
            var user = HttpContext.Current.User;

            //是否通過驗證
            if (user?.Identity?.IsAuthenticated == true)
            {
                //取得 FormsIdentity
                var identity = (FormsIdentity)user.Identity;

                //取得 FormsAuthenticationTicket
                var ticket = identity.Ticket;

                //將 Ticket 內的 UserData 解析回 User 物件
                return ticket.UserData;
            }
            return null;
        }       
    }
   
    public class AuthParam
    {
        [Required, StringLength(50), RegularExpression(@"[A-Za-z0-9\@\.]+")]
        public string account { get; set; }
        [Required, StringLength(100)]
        public string password { get; set; }
    }

}