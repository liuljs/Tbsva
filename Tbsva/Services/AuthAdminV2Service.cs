using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using WebShopping.Auth;
using WebShopping.Helpers;
using WebShopping.Models;
using WebShopping.Dtos;
using AutoMapper;

namespace WebShopping.Services
{
    public class AuthAdminV2Service : IAuthAdminV2Service
    {
        #region DI依賴注入功能
        private IDapperHelper _DapperHelper;
        private IMapper m_Mapper;
        public AuthAdminV2Service(IDapperHelper dapperHelper, IMapper mapper)
        {
            _DapperHelper = dapperHelper;
            m_Mapper = mapper;
        }
        #endregion

        public bool Login(AuthParamV2 authParamV2)
        {
            string _sql = "SELECT [ID], [NAME], [PASSWORD], [PWD_SALT], [STATUS] FROM [MANAGER] WHERE [ACCOUNT] = @ACCOUNT AND DELETED = 'N' AND ENABLED = 1";

            string realPwdKeyin = authParamV2.password;  //輸入的密碼
            authParamV2 = _DapperHelper.QuerySqlFirstOrDefault(_sql, authParamV2);  //抓取資料庫資料
            if (authParamV2 != null)
            {
                Guid id = authParamV2.id;
                string name = authParamV2.name;
                string realPwd = authParamV2.password; //資料庫編碼過的密碼
                string encryptPwd = $"{realPwdKeyin}{authParamV2.pwd_salt}";  //輸入的密碼加資料庫額外編碼
                string pwd = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(encryptPwd))).Replace("-", null);
                if (realPwd == pwd)  //資料庫編碼過的密碼  == 原本使用的編碼方式
                {
                    _sql = @"Update [MANAGER] SET lastlogined=getdate() WHERE [ID]=@ID";
                    _DapperHelper.ExecuteSql(_sql, authParamV2);  //更新登入時間

                    //1.取得使用者的所有群組底下模組詳細資料
                    List<ModuleV2> moduleV2s = GetUserGroupModulePermision(authParamV2);
                    //2.取得目錄的分類
                    List<Classification> classifications = GetClassification();

                    //3.合併群組下的模組成為一組，以有選的合併沒選edit=1 view=1
                    List<ModuleV2> mergeModulePermision = MergeModulePermision(moduleV2s, classifications);

                    //4.模組加上目錄分類
                    List<ClassificationModuleV2> classificationModuleV2 = GetUserPermision(mergeModulePermision, classifications);

                    //5.轉換型別
                    List<ClassificationModuleV2Dto> classificationModuleV2Dto = m_Mapper.Map<List<ClassificationModuleV2Dto>>(classificationModuleV2);

                    //6.將值加入類別Session用
                    UserIdentityDto userIdentityDto = new UserIdentityDto();
                    userIdentityDto.id = id;
                    userIdentityDto.name = name;
                    userIdentityDto.auths = classificationModuleV2Dto;
                    userIdentityDto.identity = Role.GetIdentityRole(Convert.ToInt32(authParamV2.status));

                    //加入JsonConvert.SerializeObject(物件)，將物件序列化成JSON格式。
                    var moduleSession = JsonConvert.SerializeObject(userIdentityDto);

                    var session = HttpContext.Current.Session; //宣告Session
                    //session.Timeout = 1;  //已在Web.config中設定<sessionState mode="InProc" timeout="30"/>
                    //Global Application_PostAuthorizeRequest HttpContext.Current.SetSessionStateBehavior 支援 Session 
                    session.Add("AuthModule", moduleSession);  //加入模組到session, cookie受限4096位元無法使用
                    var AuthModule = session["AuthModule"];

                    //7.將值加入類別Cookie用
                    UserIdentityCookieDto userIdentityCookieDto = new UserIdentityCookieDto();
                    userIdentityCookieDto.id = id;
                    userIdentityCookieDto.name = name;
                    userIdentityCookieDto.identity = Role.GetIdentityRole(Convert.ToInt32(authParamV2.status));

                    //加入JsonConvert.SerializeObject(物件)，將物件序列化成JSON格式。
                    var moduleCookie = JsonConvert.SerializeObject(userIdentityCookieDto);


                    //另一種寫法
                    //var module = JsonConvert.SerializeObject(new UserIdentityDto
                    //{
                    //    id = id.ToString(),
                    //    name = name,
                    //    auths = classificationModuleV2Dto,
                    //    identity = Role.GetIdentityRole(Convert.ToInt32(authParamV2.status))
                    //});

                    // https://dotblogs.com.tw/mickey/2017/01/01/154812 
                    // https://dotblogs.com.tw/mis2000lab/2014/08/01/authentication-mode-forms_web-config
                    // https://blog.miniasp.com/post/2008/06/11/How-to-define-Roles-but-not-implementing-Role-Provider-in-ASPNET.aspx
                    // http://kevintsengtw.blogspot.com/2013/11/aspnet-mvc.html
                    // 以下需要搭配 System.Web.Security 命名空間。
                    var ticket = new FormsAuthenticationTicket(   // 登入成功，取得門票 (票證)。請自行填寫以下資訊。
                            version: 1,   // 版本號（Ver.）
                            name: id.ToString(),   // ***自行放入資料（如：使用者帳號、真實名稱...等等）
                            issueDate: DateTime.UtcNow,  // 登入成功後，核發此票證的本機日期和時間（資料格式 DateTime）
                            //Cookie有效時間=現在時間往後+10分鐘
                            //若還在裏面執行測試中例如中斷點cookie會自動延長當初加的時間，但若結束沒有動作一樣會過期
                            //在執行中例如讀寫刪資料時，cookie還未過期或剛好剩1秒時送出執行，這時會自動延長原本的30分
                            //但若在過期後送出，則cookie不會自動延長
                            expiration: DateTime.UtcNow.AddMinutes(30),
                            isPersistent: true,// 是否要記住我 true or false（畫面上通常會用 CheckBox表示）
                            userData: moduleCookie, // ***自行放入資料（如：會員權限、角色、等級、X群組）
                            // 與票證一起存放的使用者特定資料。
                            // 需搭配 Global.asax設定檔 - Application_AuthenticateRequest事件。
                            //儲存 Cookie 的路徑
                            cookiePath: FormsAuthentication.FormsCookiePath);

                    //把 ticket驗證的表單加密
                    var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    //建立Cookie
                    HttpCookie authenticationcookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    // 重點！！避免 Cookie遭受攻擊、盜用或不當存取。請查詢 https://docs.microsoft.com/zh-tw/dotnet/api/system.net.cookie?view=netframework-4.8。
                    authenticationcookie.HttpOnly = true;   // 必須上網透過http才可以存取Cookie。不允許用戶端（寫前端程式）存取
                    //cookie.Domain = HttpContext.Current.Request.UrlReferrer.DnsSafeHost;
                    authenticationcookie.Domain = HttpContext.Current.Request.Url.IdnHost;
                    //if (ticket.IsPersistent)
                    //{
                    //    authenticationcookie.Expires = ticket.Expiration;
                    //}
                    //這裏好像沒作用但瀏覽器上顯示的時間是這組，若沒加看起來不會顯示，但實際運作是上面的expiration: DateTime.UtcNow.AddMinutes(30),
                    authenticationcookie.Expires = DateTime.UtcNow.AddMinutes(30);
                    //cookie.SameSite = SameSiteMode.None;
                    //cookie.Secure = true;
                    HttpContext.Current.Response.Cookies.Add(authenticationcookie); // 完成 Cookie，寫入使用者的瀏覽器與設備中


                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;


        }

        /// <summary>
        /// https://ithelp.ithome.com.tw/articles/10198150
        /// 使用 FormsAuthentication 進行 API 授權驗證
        /// </summary>
        /// <returns></returns>
        public object LoginInfo()
        {
            var session = HttpContext.Current.Session; //宣告Session
            var AuthModule = session["AuthModule"];

            //取得 ASP.NET 使用者
            var user = HttpContext.Current.User;

            //是否通過驗證
            if (user?.Identity?.IsAuthenticated == true || AuthModule != null)
            {
                //取得 FormsIdentity
                var identity = (FormsIdentity)user.Identity;

                //取得 FormsAuthenticationTicket
                var ticket = identity.Ticket;
                var Expiration = identity.Ticket.Expiration;
                var CookiePath = identity.Ticket.CookiePath;
                var IssueDate = identity.Ticket.IssueDate;
                var IsPersistent = identity.Ticket.IsPersistent;
                var Expired = identity.Ticket.Expired;
                var Version = identity.Ticket.Version;

                //將 Ticket 內的 UserData 解析回 User 物件
                //string ReturnJson = ticket.UserData;  //1.直接return ReturnJson這個會是加上\"id\":\"ac498354-108a-4eba-bbab-a46ac09291ba\"

                //var session = HttpContext.Current.Session; //宣告Session
                var ReturnJson = session["AuthModule"].ToString();

                //2.C# JSON格式去掉反斜杠, 若是用匿名JsonConvert.SerializeObject(new，進去的資料只能用以下將斜杠拿掉
                //https://www.newtonsoft.com/json/help/html/readjsonwithjsontextreader.htm
                //Object json = new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(ReturnJson)));
                //return json;  

                //3.JsonConvert.DeserializeObject<類別>("字串") 將JSON格式轉換成物件。
                return JsonConvert.DeserializeObject<UserIdentityDto>(ReturnJson);
                
            }
            return null;

        }


        public void Logout()
        {
            FormsAuthentication.SignOut();
            //var session = HttpContext.Current.Session; //宣告Session
            //session.Abandon();

            if (HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName,"");
                cookie.HttpOnly = true;
                cookie.Domain = HttpContext.Current.Request.UrlReferrer?.DnsSafeHost;
                cookie.Expires = DateTime.MinValue;
                HttpContext.Current.Response.Cookies.Add(cookie);

                //建立一個同名的 Cookie 來覆蓋原本的 Cookie
                //HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                //cookie1.Expires = DateTime.Now.AddYears(-1);
                //HttpContext.Current.Response.Cookies.Add(cookie1);

                ///登入cookie時會產生的
                HttpCookie cookie1 = new HttpCookie(".ASPXAUTH", "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(cookie1);

                //建立 ASP.NET 的 Session Cookie 同樣是為了覆蓋
                HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
                cookie2.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(cookie2);
            }
        }




        #region 取得使用者的所有群組底下模組詳細資料
        /// <summary>
        /// 取得使用者的所有群組底下模組詳細資料
        /// </summary>
        /// <param name="authParamV2">使用者GUID ID AC498354-108A-4EBA-BBAB-A46AC09291BA</param>
        /// <returns>List<ModuleV2> moduleV2s</returns>
        /// 1.連帳群關連表
        // [lnk_manager_group] A 
        // manager_id	                                                             manager_grp_id
        // AC498354-108A-4EBA-BBAB-A46AC09291BA	     30D74223-134E-4472-BB78-B0AFF01FCB6B
        ///2.連群模關連表
        // [lnk_module] C
        // manager_grp_id	                                                module_id
        //30D74223-134E-4472-BB78-B0AFF01FCB6B	8AD19281-3444-4282-82BC-15338ABBEDE5
        //30D74223-134E-4472-BB78-B0AFF01FCB6B   EA50CA97-433B-4E5B-8C30-3BBF4904BD91
        //30D74223-134E-4472-BB78-B0AFF01FCB6B	7B6093B0-3C84-405C-B7CA-9C71C62F3C33
        ///3.連模表 D
        //id	                                                                          authId	name	path	belong	view	edit	category
        //8AD19281-3444-4282-82BC-15338ABBEDE5 e01  首頁輪播管理 carousel.html   front   1	1	editCategory
        //EA50CA97-433B-4E5B-8C30-3BBF4904BD91 f01   一般捐款 donate_01.html      1	1	financialCategory
        //7B6093B0-3C84-405C-B7CA-9C71C62F3C33 m01 資訊列表 index.html back	1	1	managerCategory
        public List<ModuleV2> GetUserGroupModulePermision(AuthParamV2 authParamV2)
        {
            //manager.id = '43748868-b71a-422d-8305-c28a249c2e02'
            //1.用帳號條件搜尋LNK_MANAGER_GROUP得知加入幾個MANAGER_GRP_ID群組
            //再用群組搜尋LNK_MODULE得知底下的module_id
            //在連MODULE取得此帳號下的所有模組，若加入兩個會有2倍，但用DISTINCT排除相同的CODE
            //D.[authId], D.[view], D.[edit], D.category  
            //A01 0   0   editCategory
            //B01 0   1   financialCategory
            //B01 1   0   financialCategory
            //C01 0   0   managerCategory
            //C01 1   1   managerCategory
            string _sql = "SELECT DISTINCT D.[authId], D.[name], D.[path], D.[belong], D.[view], D.[edit], D.[category] " +
                                   "FROM[LNK_MANAGER_GROUP] A " +
                                   "JOIN[LNK_MODULE] C ON C.[MANAGER_GRP_ID] = A.[MANAGER_GRP_ID] " +
                                   "JOIN[MODULE] D ON D.[ID] = C.[MODULE_ID]" +
                                   "WHERE A.[MANAGER_ID] = @ID";
            // List<ModuleDto> moduleDto = _DapperHelper.QuerySetSql<ModuleDto>(_sql, authParamV2).ToList();

            List<ModuleV2> moduleV2s = new List<ModuleV2>();
            //moduleDto = _DapperHelper.QuerySetSql<ModuleDto>(_sql, id).ToList();
            moduleV2s = _DapperHelper.QuerySetSql<ModuleV2>(_sql, new ModuleV2() { id = authParamV2.id }).ToList();
            return moduleV2s;
        }
        #endregion



        #region 取得目錄的分類classification
        public List<Classification> GetClassification()
        {
            List<Classification> classifications = new List<Classification>();
            string _sql = "SELECT [id], [categoryId], [category], [name] FROM [classification]";
            classifications = _DapperHelper.QuerySetSql<Classification>(_sql).ToList();

            return classifications;
        }
        #endregion


        #region 帳號多群組下多模組合併成一個模組
        /// <summary>
        /// moduleV2s合併DefaultModule
        /// </summary>
        /// <param name="moduleV2s">取得使用者的所有群組底下模組詳細資料</param>
        /// <param name="classifications">取得目錄的分類</param>
        /// <returns></returns>
        public List<ModuleV2> MergeModulePermision(List<ModuleV2> moduleV2s, List<Classification> classifications)
        {
            List<ModuleV2> DefaultModule = new List<ModuleV2>();  //DefaultModule開一個空的模組，用來收集合併後的標準模組

            //最外層迴圈classification分類資料表
            //id  categoryId    category                   name
            //1   e00                editCategory           編輯類別
            //2   f00                 financialCategory   財務類別
            //3   m00               managerCategory  管理類別
            foreach (Classification classification in classifications)
            {
                //classifications目錄的分類
                //moduleV2s是剛從資料庫抓進來的帳號下的所有模組where去找分類依序筆數category欄位相等的值例
                //1.先第一筆目錄item.category = editCategory，
                //2.moduleV2s.Where所有資料迴圈跑一次若有等於(item.category = editCategory)
                //在放入models
                //moduleV2Category == 若module有100筆，而category欄是editCategory值有十筆就會抓出十筆module

                var moduleV2Categorys = moduleV2s.Where(moduleV2s的資料 => moduleV2s的資料.category == classification.category).ToList();
                //var moduleV2Categorys = (from moduleV2s的資料 in moduleV2s
                //                         where moduleV2s的資料.category == classification.category
                //                         select moduleV2s的資料).ToList();
                //var 目標集合 = from 資料 in 資料集合
                //           where 資料 符合條件
                //           select 目標


                //以下主要是要把moduleV2Category帳號下的所有模組轉進來moduleV2sEmpty空的模組
                //但要整合把相同authId代號，若view與eidt值若是1整合成一筆
                //以下原理是將相目錄的分類moduleV2Category跑一次
                foreach (ModuleV2 moduleV2Category in moduleV2Categorys)
                {
                    //moduleV2sEmpty 一開始是一個空的模組沒資料
                    //所以n.authId一開始沒資料會等於moduleV2Category，所以會else加入moduleV2Category
                    //接下來繼續加入把剛剛加入的moduleV2sEmpty拿authId代號來比對與後面要加入的模組比，若相等時
                    //在辨斷要準備加入的moduleV2Category edit 或 view的值是不是1，若是再搜尋moduleV2sEmpty裏的資料
                    //一樣authId相等的那一筆找到後.Eidt = 1寫入1，這樣若有兩筆模組的最後只會有一筆
                    //
                    if (DefaultModule.Any(n => n.authId == moduleV2Category.authId))
                    {
                        if (moduleV2Category.edit == "1")  //辨斷module資料
                        {
                            DefaultModule.FirstOrDefault(Empty => Empty.authId == moduleV2Category.authId).edit = "1";
                            //或 moduleV2Category.edit = "1";
                        }
                        if (moduleV2Category.view == "1")
                        {
                            DefaultModule.FirstOrDefault(Empty => Empty.authId == moduleV2Category.authId).view = "1";
                            //或 moduleV2Category.view = "1";
                        }
                    }
                    else
                    {
                        //上面條件不成立時代表空的moduleV2sEmpty裏還沒有加入要比對的那筆資料
                        //所以就把這筆資料加入，但若又有相同authId時代表可能加入兩個群組
                        //裏面有兩個模組而且View Edit有不同，所以SELECT DISTINCT D.[authId]沒有過濾掉
                        //DefaultModule[i].id = moduleV2Category.id;

                        //for (int i = 0; i < DefaultModule.Count; i++)
                        //{
                        //    DefaultModule[i].id = moduleV2Category.id;
                        //}

                        DefaultModule.Add(new ModuleV2()
                        {
                            id = moduleV2Category.id,
                            authId = moduleV2Category.authId,
                            name = moduleV2Category.name,
                            path = moduleV2Category.path,
                            belong = moduleV2Category.belong,
                            view = moduleV2Category.view,
                            edit = moduleV2Category.edit,
                            category = moduleV2Category.category
                        });
                    }
                }

            }

            return DefaultModule;
        }

        // List<ModuleV2> mergeModulePermision = MergeModulePermision(moduleV2s, classifications);
        // public List<ModuleV2> MergeModulePermision(List<ModuleV2> moduleV2s, List<Classification> classifications)
        #endregion


        #region 整合將目錄層掛上模組
        /// <summary>
        /// 最終整合完後module加上目錄層
        /// </summary>
        /// <param name="mergeModulePermision">合併完成的一組模組</param>
        /// <param name="classifications">取得目錄的分類</param>
        /// <returns></returns>
        public List<ClassificationModuleV2> GetUserPermision(List<ModuleV2> mergeModulePermision, List<Classification> classifications)
        {
            //1.多筆主體
            List<ClassificationModuleV2> classificationModuleV2s = new List<ClassificationModuleV2>();     ///設一個空的

            //1.[classification]表格的classifications類別依categoryId排新後迴圈
            foreach (Classification classification in classifications.OrderBy(x => x.categoryId))
            {
                #region 把目錄與模組載入ClassificationModuleV2
                //2.設一個空類別裏面有目錄與模組
                ClassificationModuleV2 classificationModuleV2 = new ClassificationModuleV2();
                //classificationModuleV2.classification = new Classification();
                classificationModuleV2.categoryId = classification.categoryId;
                classificationModuleV2.category = classification.category;
                classificationModuleV2.name = classification.name;
                //classificationModuleV2.classification.category = classification.category;
                //classificationModuleV2.classification.name = classification.name;
                //3.目錄下的模組
                //mergeModulePermision = new List<ModuleV2>();  //模組設一個空
                //classificationModuleV2.auth = mergeModulePermision; //合併完成的一組模組載入到auth，這裏面有所有目錄的模組
                classificationModuleV2.auth = new List<ModuleV2>();  // 模組設一個空
                #endregion

                //進來的目錄classifications值先傳進classificationModuleV2
                //然後category在等於目錄classification.category
                //抓出模組下category條件相等於上面foreach依序進來後下面Where，等於是要取相同目錄名下的模組               
                IEnumerable<ModuleV2> models = mergeModulePermision.Where(x => x.category == classification.category);

                foreach (ModuleV2 item in models)
                {
                    ModuleV2 moduleV2 = new ModuleV2();  //設一個空加入目錄相等的分類後的模組
                    moduleV2.authId = item.authId;
                    moduleV2.name = item.name;
                    moduleV2.path = item.path;
                    moduleV2.belong = item.belong;
                    moduleV2.view = item.view;
                    moduleV2.edit = item.edit;
                    moduleV2.category = item.category;
                    classificationModuleV2.auth.Add(moduleV2);   //加入auth第一個模組，接著..
                }

                classificationModuleV2s.Add(classificationModuleV2);  //單筆classificationModuleV2加入多筆classificationModuleV2s
            }

            return classificationModuleV2s;
        }
        #endregion


    }
}