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
    public class AuthAdmin : BaseAuth
    {   
        public bool Login(AuthParam param)
        {
            //1.抓出帳號資料
            SqlCommand sql = new SqlCommand(@"SELECT [ID], [NAME], [PASSWORD], [PWD_SALT], [STATUS] FROM [MANAGER] WHERE [ACCOUNT] = @ACCOUNT AND DELETED = 'N' AND ENABLED = 1");
            sql.Parameters.AddWithValue("@ACCOUNT", param.account);
            DataTable dt = db.GetResult(sql);
            if (dt.Rows.Count > 0)
            {
                string id = dt.Rows[0]["ID"].ToString();
                string name = dt.Rows[0]["NAME"].ToString();
                string realPwd = dt.Rows[0]["PASSWORD"].ToString();
                string encryptPwd = $"{param.password}{dt.Rows[0]["PWD_SALT"]}";
                string pwd = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(encryptPwd))).Replace("-", null);
                //2.比對存在資料庫已加過密的密碼realPwd=在將輸入密碼與PWD_SALT組合後加密比對
                if (realPwd == pwd)
                {
                    sql = new SqlCommand("Update [MANAGER] SET lastlogined=getdate() WHERE [ID]=@ID");
                    sql.Parameters.AddWithValue("@ID", id);
                    try
                    {
                        db.OpenConn();
                        db.Execute(sql);
                    }
                    finally
                    {
                        db.CloseConn();
                    }

                    //3.用帳號MANAGER_ID抓出此帳號屬於那幾個群組，只是後面好像沒有用到註解
                    //groupName = (dtManagerGroup.Rows.Count > 0) ? dtManagerGroup.Rows[0]["name"] : string.Empty,
                    sql = new SqlCommand(
                    @"SELECT DISTINCT B.[NAME]
                    FROM [LNK_MANAGER_GROUP] A 
                    JOIN [MANAGER_GROUP] B 
                    ON A.[MANAGER_GRP_ID] = B.[ID]                  
                    WHERE A.[MANAGER_ID] = @ID");
                    sql.Parameters.AddWithValue("@ID", id);
                    DataTable dtManagerGroup = db.GetResult(sql);

                    //4.LNK_MANAGER_GROUP(記錄帳號有那些群組)
                    //連LNK_MODULE(群組對應那模組功能)
                    //連MODULE(記錄群組下有那些模組功能)
                    //抓出使用者加入那些群組，群組所設定有那些模組權限
                    sql = new SqlCommand(
                    @"SELECT DISTINCT D.[CODE], D.[ACT_VIEW], D.[ACT_DEL], D.[ACT_EDT]
                    FROM [LNK_MANAGER_GROUP] A 
                    JOIN [LNK_MODULE] C ON C.[MANAGER_GRP_ID] = A.[MANAGER_GRP_ID]
                    JOIN [MODULE] D ON D.[ID] = C.[MODULE_ID]
                    WHERE A.[MANAGER_ID] = @ID
                    ORDER BY D.[CODE]");
                    sql.Parameters.AddWithValue("@ID", id);
                    DataTable dtModule = db.GetResult(sql);

                    //5.將使用者的模組權限帶入並設定一個原有標準模組比對處理
                    dtModule = CombineGroupPermission(dtModule);                

                    string module = JsonConvert.SerializeObject(new
                    {
                        id = id,
                        name = name,
                        //groupName = (dtManagerGroup.Rows.Count > 0) ? dtManagerGroup.Rows[0]["name"] : string.Empty,
                        module = dtModule,
                        Identity = Role.GetIdentityRole(Convert.ToInt32(dt.Rows[0]["STATUS"]))
                    });
                    // https://dotblogs.com.tw/mickey/2017/01/01/154812 
                    // https://dotblogs.com.tw/mis2000lab/2014/08/01/authentication-mode-forms_web-config
                    // https://blog.miniasp.com/post/2008/06/11/How-to-define-Roles-but-not-implementing-Role-Provider-in-ASPNET.aspx
                    // http://kevintsengtw.blogspot.com/2013/11/aspnet-mvc.html
                    // 以下需要搭配 System.Web.Security 命名空間。
                    var ticket = new FormsAuthenticationTicket(   // 登入成功，取得門票 (票證)。請自行填寫以下資訊。
                    version: 1,   // 版本號（Ver.）
                    name: id,   // ***自行放入資料（如：使用者帳號、真實名稱...等等）
                    issueDate: DateTime.UtcNow,  // 登入成功後，核發此票證的本機日期和時間（資料格式 DateTime）
                    expiration: DateTime.UtcNow.AddMinutes(30),//Cookie有效時間=現在時間往後+30分鐘
                    isPersistent: true,// 是否要記住我 true or false（畫面上通常會用 CheckBox表示）
                    userData: module, // ***自行放入資料（如：會員權限、角色、等級、群組）
                                      // 與票證一起存放的使用者特定資料。
                                      // 需搭配 Global.asax設定檔 - Application_AuthenticateRequest事件。
                    cookiePath: FormsAuthentication.FormsCookiePath);

                    var encryptedTicket = FormsAuthentication.Encrypt(ticket); //把 ticket驗證的表單加密
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    // 重點！！避免 Cookie遭受攻擊、盜用或不當存取。請查詢 https://docs.microsoft.com/zh-tw/dotnet/api/system.net.cookie?view=netframework-4.8。
                    cookie.HttpOnly = true;   // 必須上網透過http才可以存取Cookie。不允許用戶端（寫前端程式）存取
                    //cookie.Domain = HttpContext.Current.Request.UrlReferrer.DnsSafeHost;
                    cookie.Domain = HttpContext.Current.Request.Url.IdnHost;
                    cookie.Expires = DateTime.UtcNow.AddMinutes(30);
                    //cookie.SameSite = SameSiteMode.None;
                    //cookie.Secure = true;

                    HttpContext.Current.Response.Cookies.Add(cookie); // 完成 Cookie，寫入使用者的瀏覽器與設備中

                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// 合併底下多群組的權限
        /// </summary>
        /// <param name="p_table"> dtModule使用者現有的所有群組表單 </param>
        /// <returns></returns>
        private DataTable CombineGroupPermission(DataTable p_table)
        {
            //DataTable dtModule 轉 List permissionList_
            List<PermissionParam> permissionList_ = new List<PermissionParam>();

            permissionList_ = (from DataRow dr in p_table.Rows
                           select new PermissionParam()
                           {
                               CODE = dr["CODE"].ToString(),
                               ACT_VIEW = dr["ACT_VIEW"].ToString(),
                               ACT_DEL = dr["ACT_DEL"].ToString(),
                               ACT_EDT = dr["ACT_EDT"].ToString()
                           }).ToList();


            //建立一個初始標準模組 Table dtCombineModule_
            //例如一組有四個模組 MM OM PM SM 全部設定沒有權限
            //因為帳號可能會有好幾個群組，若有兩個，可能會有四個模組以上
            //所以建一個標準，拿現有帳號來比對Y
            DataTable dtCombineModule_ = GetDefaultModule();

            //合併 MM OM PM SM 的群組權限是Y的情況
            //permissionList_是現有帳號下群組組合下可能超過四組模組
            //所以以ACT_VIEW ACT_DEL ACT_EDT是Y時就取代
            //將permissionList_現有帳號所屬群組下的模組比對
            //dtCombineModule_是建一個標準模組權限但都設定N
            //Code                        ACT_VIEW檢視  ACT_DEL 編輯  ACT_EDT 刪除
            //MM會員管理               N                            N                              N 
            //OM訂單管理               N                            N                              N 
            //PM產品管理               N                            N                              N 
            //SM管理者管               N                            N                              N 
            //xx	首頁輪播管理               N                            N                              N 
            //xx	•	年度功德主方案               N                            N                              N 

            foreach (var v in permissionList_)
            {                
                if (v.ACT_VIEW == "Y")
                {
                    //加入 ["CODE'] 判斷式
                    IEnumerable<DataRow> rows_ = dtCombineModule_.Rows.Cast<DataRow>().Where(r => r["CODE"].ToString() == v.CODE);
                    //更新 [ACT_VIEW] 的Cell值("Y")
                    rows_.ToList().ForEach(r => r.SetField("ACT_VIEW", "Y"));
                }

                if (v.ACT_DEL == "Y")
                {
                    IEnumerable<DataRow> rows_ = dtCombineModule_.Rows.Cast<DataRow>().Where(r => r["CODE"].ToString() == v.CODE);
                    rows_.ToList().ForEach(r => r.SetField("ACT_DEL", "Y"));
                }

                if (v.ACT_EDT == "Y")
                {
                    IEnumerable<DataRow> rows_ = dtCombineModule_.Rows.Cast<DataRow>().Where(r => r["CODE"].ToString() == v.CODE);
                    rows_.ToList().ForEach(r => r.SetField("ACT_EDT", "Y"));
                }
            }

            return dtCombineModule_;
        }

        /// <summary>
        /// 加入一組預設MM OM PM SM (ACT_VIEW=N ACT_DEL=N ACT_EDT=N)
        /// </summary>
        /// <returns></returns>
        private DataTable GetDefaultModule()
        {
            DataTable _dtModule = new DataTable();  //設定一個空的
            //設定表格欄位
            _dtModule.Columns.Add("CODE", typeof(string));
            _dtModule.Columns.Add("ACT_VIEW", typeof(char));
            _dtModule.Columns.Add("ACT_DEL", typeof(char));
            _dtModule.Columns.Add("ACT_EDT", typeof(char));

            for (int i = 0; i < 4; i++)
            {
                DataRow _row = _dtModule.NewRow();

                switch (i)
                {
                    case 0:
                        _row["CODE"] = "MM";
                        break;
                    case 1:
                        _row["CODE"] = "OM";
                        break;
                    case 2:
                        _row["CODE"] = "PM";
                        break;
                    case 3:
                        _row["CODE"] = "SM";
                        break;
                }

                _row["ACT_VIEW"] = 'N';
                _row["ACT_DEL"] = 'N';
                _row["ACT_EDT"] = 'N';

                _dtModule.Rows.Add(_row);
            }
         
            return _dtModule;
        }

        private DataTable GetSuperAdminModule()
        {
            DataTable _dtModule = new DataTable();
            _dtModule.Columns.Add("CODE", typeof(string));
            _dtModule.Columns.Add("ACT_VIEW", typeof(char));
            _dtModule.Columns.Add("ACT_DEL", typeof(char));
            _dtModule.Columns.Add("ACT_EDT", typeof(char));

            for (int i = 0; i < 4; i++)
            {
                DataRow _row = _dtModule.NewRow();

                switch (i)
                {
                    case 0:
                        _row["CODE"] = "MM";
                        break;
                    case 1:
                        _row["CODE"] = "OM";
                        break;
                    case 2:
                        _row["CODE"] = "PM";
                        break;
                    case 3:
                        _row["CODE"] = "SM";
                        break;
                }

                _row["ACT_VIEW"] = 'Y';
                _row["ACT_DEL"] = 'Y';
                _row["ACT_EDT"] = 'Y';
            }

            return _dtModule;
        }

    }
   
    /// <summary>
    /// 權限類別
    /// </summary>
    public class PermissionParam
    {
        public string CODE { get; set; }
        public string ACT_VIEW { get; set; }
        public string ACT_DEL { get; set; }
        public string ACT_EDT { get; set; }

    }
}