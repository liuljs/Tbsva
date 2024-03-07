using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using ecSqlDBManager;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.Security;
using System.Text;
using WebShopping.Auth;
using WebShopping.Helpers;

namespace WebShoppingAdmin.Models
{
    public class Manager : BaseModel
    {
        /// <summary>
        /// 取得帳號相關資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable Get(Guid? id)
        {
//            id account email name    enabled lastlogined
//AC498354-108A-4EBA-BBAB-A46AC09291BA    liuljs   liuljs@gmail.com 測試  1   2022 - 04 - 21 10:20:33.290
            SqlCommand sql = new SqlCommand(string.Format(@"
            SELECT
                A.[id], A.[account], A.[email], A.[name], A.[enabled], A.[lastlogined]
            FROM [MANAGER] A WHERE A.[ACCOUNT] <> 'admin' {0}             
            ORDER BY A.[NAME]", id == null ? "AND DELETED = 'N'" : "AND A.[ID] = @MANAGER_ID"));

            if (id != null)
                sql.Parameters.AddWithValue("@MANAGER_ID", id); //管理者的id

            DataTable resultTop_ = db.GetResult(sql);

            //   id account email name    enabled  lastlogined    [info]
            //1.帳號資料表並加設info內層dr1["info"]要放群組與相關連模組
            resultTop_.Columns.Add("info", typeof(DataTable));
                        
            foreach (DataRow dr1 in resultTop_.Rows)
            {
                string strManagerId_ = dr1["ID"].ToString();


                //1.用manager 帳號id，先MANAGER.id找LNK_MANAGER_GROUP.MANAGER_ID，
                //接下來有了群組id在找MANAGER_GROUP.id
                // 得知群組名稱，並列出群組id
                sql = new SqlCommand(string.Format(@"
                SELECT
                    C.[id], C.[authId] , C.[name], Format(C.[creationDate] , N'yyyy-MM-dd HH:mm') AS creationDate
                FROM [MANAGER] A
                JOIN [LNK_MANAGER_GROUP] B ON A.[ID] = B.[MANAGER_ID]
                JOIN [MANAGER_GROUP] C ON C.[authId] = B.[MANAGER_GRP_ID] 
                WHERE A.[ID] = @MANAGER_ID"));
                sql.Parameters.AddWithValue("@MANAGER_ID", strManagerId_);

                // groupName groupId
                //test群組1 30D74223-134E-4472-BB78-B0AFF01FCB6B
                //群組2 DFED12CF-4917-4BF2-876F-D10C719D32E1
                DataTable resultMiddle_ = db.GetResult(sql);

                //   groupName groupId [auth]
                //2.群組資料表並加設auth內層dr2["auth"] 要放模組資料
                resultMiddle_.Columns.Add("auths", typeof(DataTable));

                //帳號id 來抓LNK_MANAGER_GROUP.MANAGER_ID, 找出群組MANAGER_GRP_ID
                //在用MANAGER_GRP_ID找LNK_MODULE的MANAGER_GRP_ID
                //找出MODULE_ID，再來找[module].id
                //調出module欄位
                //WHERE A.[MANAGER_GRP_ID]<=改抓群組
                foreach (DataRow dr2 in resultMiddle_.Rows)
                {
                    sql = new SqlCommand(@"
                    SELECT DISTINCT D.[authId], D.[name], D.[path], D.[belong], D.[view], D.[edit], D.[category]
                    FROM [LNK_MANAGER_GROUP] A 
                    JOIN [LNK_MODULE] C ON C.[MANAGER_GRP_ID] = A.[MANAGER_GRP_ID]
                    JOIN [MODULE] D ON D.[ID] = C.[MODULE_ID]
                    WHERE A.[MANAGER_GRP_ID] = @ID
                    ORDER BY D.[authId] ASC "
                    );
                    
                    sql.Parameters.AddWithValue("@ID", dr2["authId"]);
                    DataTable resultSub_ = db.GetResult(sql);

                    //模組資料抓出來，放入群組DataTable auth欄位
                    dr2["auths"] = resultSub_;                    
                }

                if (resultMiddle_.Rows.Count > 0)
                    dr1["info"] = resultMiddle_;  //最後群組表格放入帳號DataTable
            }

            return resultTop_;
        }

        public void Insert(ManagerInsertParam param)
        {
            try
            {
                db.OpenConn();

                //0.已經有相同的管理帳號
                SqlCommand sql = new SqlCommand(@"
                SELECT [ID] FROM [MANAGER] WHERE [ACCOUNT] = @ACCOUNT");
                sql.Parameters.AddWithValue("@ACCOUNT", param.account);
                DataTable dt = db.GetResult(sql);
                if (dt.Rows.Count > 0)
                    throw new Exception("已經有相同的管理帳號");

                //0.已經有相同的註冊信箱
                sql = new SqlCommand(@"
                SELECT [ID] FROM [MANAGER] WHERE [EMAIL] = @EMAIL");
                sql.Parameters.AddWithValue("@EMAIL", param.email);
                DataTable dt1 = db.GetResult(sql);
                if (dt1.Rows.Count > 0)
                    throw new Exception("已經有相同的註冊信箱");

                //0. 產生雜湊碼
                string strSalt_ = Tools.GetInstance().CreateSaltCode();
                string strRandomPW_ = Tools.GetInstance().CreateRandomCode(10); //隨機取得密碼

                //0.將 (隨機取得密碼+雜湊碼)加SHA1
                string pwd = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{strRandomPW_}{strSalt_}"))).Replace("-", null);
                //string pwd = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{param.account}{salt}"))).Replace("-", null);
                //string pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(param.account + salt, "SHA1");

                // 1.新增帳號MANAGER主檔
                sql = new SqlCommand(@"
                INSERT INTO [MANAGER] ([ID], [ACCOUNT], [EMAIL], [NAME], [PASSWORD], [PWD_SALT], [STATUS])
                VALUES (@ID, @ACCOUNT, @EMAIL, @NAME, @PASSWORD, @PWD_SALT, @STATUS)");
                sql.Parameters.AddWithValue("@ID", param.account_id);
                sql.Parameters.AddWithValue("@ACCOUNT", param.account);
                sql.Parameters.AddWithValue("@EMAIL", param.email);
                sql.Parameters.AddWithValue("@NAME", param.name);
                sql.Parameters.AddWithValue("@PASSWORD", pwd);
                sql.Parameters.AddWithValue("@PWD_SALT", strSalt_);
                sql.Parameters.AddWithValue("@STATUS", (int)RoleType.Admin);
                db.ExecuteNonCommit(sql);

                //2.新增帳號id與群組id關連
                // 把群組"groups" : ["30D74223-134E-4472-BB78-B0AFF01FCB6B"]關連進帳號關聯群組表
                // 一個帳號可以有多個群組
                if (param.groups != null)
                {
                    sql = new SqlCommand(@"
                    INSERT INTO [LNK_MANAGER_GROUP] ([MANAGER_ID], [MANAGER_GRP_ID])
                    VALUES (@MANAGER_ID, @MANAGER_GRP_ID)");
                    foreach (string groupId in param.groups)
                    {
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@MANAGER_ID", param.account_id);
                        sql.Parameters.AddWithValue("@MANAGER_GRP_ID", groupId);
                        db.ExecuteNonCommit(sql);
                    }
                }

                EventLog(db, "manager&lnk", DbAction.INSERT, param.account_id, null, param);

                db.Commit();
                db.CloseConn();

                //發送Email
                SystemFunctions.SendMail(Tools.Company_Name, Tools.Admin_Mail, param.email, new List<string>(), new List<string>(), 
                                             $"{Tools.Company_Name}密碼訊息通知", $"預設密碼 : {strRandomPW_}, 請至 {WebShopping.Helpers.Tools.WebSiteUrl}/backStage/login.html 更改密碼");
            }
            catch (Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }
        }

        public void Update(ManagerUpdateParam param)
        {
            try
            {
                db.OpenConn();

                // 取得舊資料
                DataTable oldData, oldGroupData;
                GetOldData(param.account_id, out oldData, out oldGroupData);

                // 主檔, 更新EMAIL NAME ENABLED
                SqlCommand sql = new SqlCommand(@"
                UPDATE [MANAGER] SET [EMAIL] = @EMAIL, [NAME] = @NAME, [ENABLED] = @ENABLED
                WHERE ID = @ID");
                sql.Parameters.AddWithValue("@ID", param.account_id);
                sql.Parameters.AddWithValue("@EMAIL", param.email);
                sql.Parameters.AddWithValue("@NAME", param.name);
                sql.Parameters.AddWithValue("@ENABLED", param.enabled);
                db.ExecuteNonCommit(sql);

                // 重新群組關連 先刪
                sql = new SqlCommand(@"
                DELETE FROM [LNK_MANAGER_GROUP] WHERE [MANAGER_ID] = @MANAGER_ID");
                sql.Parameters.AddWithValue("@MANAGER_ID", param.account_id);
                db.ExecuteNonCommit(sql);

                //確定有輸入群組(所以如果沒有帶原本群組，原本的就會被刪)
                if (param.groups != null)
                {
                    sql = new SqlCommand(@"
                    INSERT INTO [LNK_MANAGER_GROUP] ([MANAGER_ID], [MANAGER_GRP_ID])
                    VALUES (@MANAGER_ID, @MANAGER_GRP_ID)");
                    foreach (string groupId in param.groups)
                    {
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@MANAGER_ID", param.account_id);
                        sql.Parameters.AddWithValue("@MANAGER_GRP_ID", groupId);
                        db.ExecuteNonCommit(sql);
                    }
                }

                EventLog(db, "manager&lnk", DbAction.UPDATE, param.account_id, new { master = oldData, lnk = oldGroupData }, param);

                db.Commit();
                db.CloseConn();
            }
            catch (Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }
        }

        public void Delete(ManagerDeleteParam param)
        {
            try
            {
                db.OpenConn();

                // 取得舊資料
                DataTable oldData, oldGroupData;
                GetOldData(param.id, out oldData, out oldGroupData);

                // 主檔(這個主檔不會刪，不然EventLog會找不到兇手)
                SqlCommand sql = new SqlCommand(@"
                UPDATE [MANAGER] SET [DELETED] = @DELETED
                WHERE ID = @ID");
                sql.Parameters.AddWithValue("@ID", param.id);
                sql.Parameters.AddWithValue("@DELETED", 'Y');
                db.ExecuteNonCommit(sql);

                // 刪除群組關連(如果真的誤刪就誤刪了，回覆後重新給予關連吧)
                sql = new SqlCommand(@"
                DELETE FROM [LNK_MANAGER_GROUP] WHERE [MANAGER_ID] = @MANAGER_ID");
                sql.Parameters.AddWithValue("@MANAGER_ID", param.id);
                db.ExecuteNonCommit(sql);

                EventLog(db, "manager&lnk", DbAction.DELETE, param.account_id, new { master = oldData, lnk = oldGroupData }, null);

                db.Commit();
                db.CloseConn();
            }
            catch (Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }
        }

        public string UpdatePassword(ManagerUpdatePassWordParam param)
        {
            try
            {
                //結果字串回傳
                string strResult_ = string.Empty;

                //取得 ASP.NET 使用者
                var user_ = HttpContext.Current.User;

                //是否通過驗證
                if (user_?.Identity?.IsAuthenticated == true)
                {
                    //取得 FormsIdentity
                    var identity = (FormsIdentity)user_.Identity;

                    //取得 FormsAuthenticationTicket
                    var ticket = identity.Ticket;

                    //管理者Id
                    string strManagerId_ = ticket.Name;

                    db.OpenConn();

                    // 取得舊資料
                    SqlCommand sql = new SqlCommand(@"
                        SELECT
                            [ID], [NAME], [ACCOUNT], [PASSWORD], [PWD_SALT], [CREATE_DATE], [EMAIL]
                        FROM [MANAGER]
                        WHERE [ID] = @ID");                    
                    sql.Parameters.AddWithValue("@ID", strManagerId_);
                    DataTable oldData_ = db.GetResult(sql);

                    //比對密碼是否相同
                    if (oldData_.Rows.Count > 0)
                    {
                        string strCurrentPwd_ = oldData_.Rows[0]["PASSWORD"].ToString();
                        string strEncryptPwd_ = $"{param.old_password}{oldData_.Rows[0]["PWD_SALT"]}";    //雜湊碼(輸入的舊密碼 + 雜湊值)
                        string strInputOldPwd_ = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(strEncryptPwd_))).Replace("-", null);

                        //現有密碼 = 輸入的舊密碼
                        if (strCurrentPwd_ == strInputOldPwd_)
                        {
                            //輸入的新密碼 = 再次輸入的新密碼
                            if (param.new_password == param.new_password_again)
                            {
                                string strUpdateEncryptPwd_ = $"{param.new_password}{oldData_.Rows[0]["PWD_SALT"]}";    //雜湊碼(輸入的新密碼 + 雜湊值)
                                string strUpdatePwd_ = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(strUpdateEncryptPwd_))).Replace("-", null);

                                //密碼更新
                                sql = new SqlCommand(@"
                                UPDATE [MANAGER] SET [PASSWORD] = @PASSWORD
                                WHERE ID = @ID");
                                sql.Parameters.AddWithValue("@PASSWORD", strUpdatePwd_);
                                sql.Parameters.AddWithValue("@ID", strManagerId_);
                                db.ExecuteNonCommit(sql);

                                EventLog(db, "manager", DbAction.UPDATE, Guid.Parse(strManagerId_), new { master = oldData_ }, param);
                            }
                            else
                            {
                                SystemFunctions.WriteLogFile($"[UpdatePassword]-[二次密碼不相同]-[Account : {oldData_.Rows[0]["ACCOUNT"]}]-[Password : {param.new_password}]-[Again Password : {param.new_password_again}]");
                                strResult_ = "二次密碼不相同";
                            }
                        }
                        else
                        {
                            SystemFunctions.WriteLogFile($"[UpdatePassword]-[帳號或密碼錯誤]-[Account : {oldData_.Rows[0]["ACCOUNT"]}]-[Password : {param.old_password}]");
                            strResult_ = "帳號或密碼錯誤";
                        }
                    }                                      
                }
                
                db.Commit();
                db.CloseConn();

                return strResult_;
            }
            catch(Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }            
        }

        public string ResetPassword(ManagerResetPassWordParam param)
        {
            try
            {
                //結果字串回傳
                string strResult_ = string.Empty;

                //取得 ASP.NET 使用者
                var user_ = HttpContext.Current.User;

                //是否通過驗證
                if (user_?.Identity?.IsAuthenticated == true)
                {
                    ////取得 FormsIdentity
                    //var identity = (FormsIdentity)user_.Identity;

                    ////取得 FormsAuthenticationTicket
                    //var ticket = identity.Ticket;

                    ////管理者Id
                    string strManagerId_ = param.id.ToString();

                    db.OpenConn();

                    // 取得舊資料
                    SqlCommand sql = new SqlCommand(@"
                        SELECT
                            [ID], [NAME], [ACCOUNT], [PASSWORD], [PWD_SALT], [CREATE_DATE], [EMAIL]
                        FROM [MANAGER]
                        WHERE [ID] = @ID");
                    sql.Parameters.AddWithValue("@ID", strManagerId_);
                    DataTable oldData_ = db.GetResult(sql);

                    //資料存在
                    if (oldData_.Rows.Count > 0)
                    {
                        //輸入的新密碼 = 再次輸入的新密碼
                        if (param.new_password == param.new_password_again)
                        {
                            string strUpdateEncryptPwd_ = $"{param.new_password}{oldData_.Rows[0]["PWD_SALT"]}";    //雜湊碼(輸入的新密碼 + 雜湊值)
                            string strUpdatePwd_ = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(strUpdateEncryptPwd_))).Replace("-", null);

                            //密碼更新
                            sql = new SqlCommand(@"
                                UPDATE [MANAGER] SET [PASSWORD] = @PASSWORD
                                WHERE ID = @ID");
                            sql.Parameters.AddWithValue("@PASSWORD", strUpdatePwd_);
                            sql.Parameters.AddWithValue("@ID", strManagerId_);
                            db.ExecuteNonCommit(sql);

                            EventLog(db, "manager", DbAction.UPDATE, Guid.Parse(strManagerId_), new { master = oldData_ }, param);
                        }
                        else
                        {
                            SystemFunctions.WriteLogFile($"[UpdatePassword]-[二次密碼不相同]-[Account : {oldData_.Rows[0]["ACCOUNT"]}]-[Password : {param.new_password}]-[Again Password : {param.new_password_again}]");
                            strResult_ = "二次密碼不相同";
                        }
                    }
                }

                db.Commit();
                db.CloseConn();

                return strResult_;
            }
            catch (Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }
        }

        /// <summary>
        /// 回拋舊資料
        /// </summary>
        private void GetOldData(Guid managerId, out DataTable oldData, out DataTable oldGroupData)
        {
            SqlCommand sql = new SqlCommand(@"
                SELECT
                    [ID], [EMAIL], [NAME], [ENABLED]
                FROM [MANAGER]
                WHERE [ID] = @ID");
            sql.Parameters.AddWithValue("@ID", managerId);

            oldData = db.GetResult(sql);

            sql = new SqlCommand(@"
                SELECT
                    [MANAGER_ID], [MANAGER_GRP_ID]
                FROM [LNK_MANAGER_GROUP]
                WHERE [MANAGER_ID] = @MANAGER_ID");
            sql.Parameters.AddWithValue("@MANAGER_ID", managerId);

            oldGroupData = db.GetResult(sql);
        }
    }

    public class ManagerGetParam
    {
        /// <summary>
        /// 管理帳號ID
        /// </summary>
        public Guid? id { get; set; }
    }

    public class ManagerDeleteParam
    {
        /// <summary>
        /// 登入帳號ID
        /// </summary>
        [Required]
        public Guid account_id { get; set; }
        /// <summary>
        /// 管理帳號ID
        /// </summary>
        [Required]
        public Guid id { get; set; }
    }

    public class ManagerUpdateParam : ManagerInsertParam
    {
        /// <summary>
        /// 是否啟用
        /// </summary>        
        public byte enabled { get; set; }
    }

    /// <summary>
    /// 更新密碼
    /// </summary>
    public class ManagerUpdatePassWordParam
    {
        /// <summary>
        /// 舊密碼
        /// </summary>                
        [Required, StringLength(100)]
        public string old_password { get; set; }

        /// <summary>
        /// 新密碼
        /// </summary>        
        [Required, StringLength(100)]
        public string new_password { get; set; }

        /// <summary>
        /// 新密碼(Again)
        /// </summary>        
        [Required, StringLength(100)]
        public string new_password_again { get; set; }
    }

    /// <summary>
    /// 重設密碼
    /// </summary>
    public class ManagerResetPassWordParam
    {
        /// <summary>
        /// 管理員ID
        /// </summary>        
        public Guid id { get; set; }

        /// <summary>
        /// 新密碼
        /// </summary>        
        [Required, StringLength(100)]
        public string new_password { get; set; }

        /// <summary>
        /// 新密碼(Again)
        /// </summary>        
        [Required, StringLength(100)]
        public string new_password_again { get; set; }
    }
}