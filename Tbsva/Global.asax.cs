using WebShopping;
using WebShopping.Auth;
using Newtonsoft.Json;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace WebShoppingAdmin
{
    /// <summary>
    /// 一開始執行啟動
    ///1.進入UnityWebApiActivator.cs Start() => UnityConfig.cs => Global.asax.cs => WebApiConfig.cs =>
    /// 到驗證CustomAuthorize.cs=> Global.asax.cs
    /// 登入後端
    ///1.Global.asax.cs=> CustomAuthorize.cs => AuthAdminController.cs Login =>
    ///BaseModel.cs載入db =>執行obj.Logi AuthAdmin.cs Login 登入驗證載入cookie
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            UnityWebApiActivator.Start();          
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            ;
        }

        /// <summary>
        /// Authen right for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      //**************************************************************************
        //*** 搭配 LoginController。登入與權限控管。
        //      需搭配  System.Web.Security 與 System.Security.Principal命名空間。
        //      Global.asax文件裡面的事件 -- https://dotblogs.com.tw/mis2000lab/2008/04/28/3526
        //**************************************************************************
        //1.IsAuthenticated: 指是否已驗證使用者。
        //2.FormsIdentity:表示使用表單驗證的已驗證使用者識別。
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            //string cookieName = FormsAuthentication.FormsCookieName;  //從驗證票據獲取Cookie的名字。
            ////取得Cookie.
            //HttpCookie authCookie = Context.Request.Cookies[cookieName];
            //if (authCookie == null)
            //    return;
            //FormsAuthenticationTicket authTicket = null;
            ////獲取驗證票據。
            //authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //if (authTicket == null)
            //    return;

            //string[] roles1 = authTicket.UserData.Split(new char[] { ',' });
            //FormsIdentity id2 = new FormsIdentity(authTicket);
            //GenericPrincipal principal = new GenericPrincipal(id2, roles1);
            ////把生成的驗證票信息和角色信息賦給當前用戶
            //Context.User = principal;
            ////在每個頁面讀取Cookie的值。可以通過下面的語句來讀取Cookie的值
            //var user = HttpContext.Current.User.Identity.Name;
            //======================================
            if (HttpContext.Current.User != null)   // 有cookie執行下方程式，沒有不擋，因為有些是AllowAnonymous
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        //Get current user identitied by forms
                        // 先取得登入者的身份識別 -- FormsIdentity。  需搭配 System.Web.Security命名空間
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        // get FormsAuthenticationTicket object
                        // 再取出使用者的票證 (authTicket) -- FormsAuthenticationTicket
                        FormsAuthenticationTicket ticket = id.Ticket;

                        //把 ticket驗證的表單加密
                        //var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                        //建立Cookie
                        //HttpCookie authenticationcookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        //if (ticket.IsPersistent)
                        //{
                        //    //authenticationcookie.Expires = ticket.Expiration;
                        //    //authenticationcookie.Expires = DateTime.UtcNow.AddMinutes(30);
                        //}

                        // 取出票證 (authTicket)中的 "角色"  並轉成字串陣列 string[]。
                        string userData = ticket.UserData;
                        var _identityData = JsonConvert.DeserializeObject<IdentityData>(userData); //解析登入時儲存的UserData
                        // 如果登入者是「多重的」角色，例如 1,3,5。這樣的安排可以寫下面的程式來擷取。
                        string[] roles = _identityData.Identity.Split(',');
                        // set the new identity for current user.
                        // 指派 "角色" 到目前這個 HttpContext 的 User物件
                        // ......當初建立票證 (authTicket)時，UserData屬性裡面放了什麼資訊（如：會員權限、等級、群組） 
                        // ......然後把資料放入 HttpContext 的 User物件裡面
                        HttpContext.Current.User = new GenericPrincipal(id, roles);
                        // GenericPrincipal 需搭配 System.Security.Principal命名空間

                        //      https://dotblogs.com.tw/mickey/2017/01/01/154812
                        //      https://blog.miniasp.com/post/2008/06/11/How-to-define-Roles-but-not-implementing-Role-Provider-in-ASPNET.aspx
                        //      http://kevintsengtw.blogspot.com/2013/11/aspnet-mvc.html
                    }
                }
            }
        }

        /// <summary>
        /// WebApi由於是無狀態的，因此無法使用這樣的方式直接儲存
        ///所以需要在檔案：~/Global.asax，內新增以下程式碼即可支援：
        ///https://dotblogs.com.tw/Leo_CodeSpace/2017/05/09/141817
        /// </summary>
        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }

    }   
}



