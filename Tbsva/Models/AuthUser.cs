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
using WebShopping.Dtos;
using WebShopping.Helpers;
using WebShopping.Models;
using WebShopping.Services;

namespace WebShoppingAdmin.Models
{
    public class AuthUser : BaseAuth
    {
        IDapperHelper m_IDrapperHelp;

        public bool Login(AuthParam param)
        {
            SqlCommand sql = new SqlCommand(@"SELECT [ID], [NAME], [PASSWORD], [PWD_SALT], [TEMP_PASSWORD] FROM [member] WHERE [ACCOUNT] = @ACCOUNT AND Enabled = 1 AND BLACKLIST = 0");
            sql.Parameters.AddWithValue("@ACCOUNT", param.account);
            DataTable dt = db.GetResult(sql);
            if (dt.Rows.Count > 0)
            {
                string id = dt.Rows[0]["ID"].ToString();
                string name = dt.Rows[0]["NAME"].ToString();
                string realPwd = dt.Rows[0]["PASSWORD"].ToString();
                string tempPwd = dt.Rows[0]["TEMP_PASSWORD"].ToString();

                string encryptPwd = $"{param.password}{dt.Rows[0]["PWD_SALT"]}";
                string pwd = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(encryptPwd))).Replace("-", null);
                if (realPwd == pwd)
                {
                    UserLoginAction(id, name);
                    //TODO:舊密碼登入成功，如果有臨時密碼，就清空
                    if (String.IsNullOrEmpty(tempPwd))
                        UpdateMemberInfo(new Guid(id), new RecvMemberPasswordInfoDto() { id = new Guid(id), Password = realPwd, TempPassword = tempPwd }, EmunPwdInfo.Logined);
                    else
                        UpdateMemberInfo(new Guid(id), new RecvMemberPasswordInfoDto() { id = new Guid(id), Password = realPwd, TempPassword = tempPwd }, EmunPwdInfo.OldPwdLogined);
                    return true;
                }


                if (!String.IsNullOrEmpty(tempPwd))
                {
                    encryptPwd = $"{param.password}{dt.Rows[0]["PWD_SALT"]}";
                    string pwd2 = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(encryptPwd))).Replace("-", null);
                    if (tempPwd == pwd2)
                    {
                        UserLoginAction(id, name);
                        //登入成功，把臨時密碼寫到正式，並清空
                        UpdateMemberInfo(new Guid(id), new RecvMemberPasswordInfoDto() { id = new Guid(id), Password = realPwd, TempPassword = tempPwd }, EmunPwdInfo.TempPwdLogined);
                        return true;
                    }
                }

                return false;
            }
            else
                return false;
        }

        public bool FBLogin(FBUserDto param)
        {
            SqlCommand sql = new SqlCommand(@"SELECT [ID], [NAME] FROM [member] WHERE [ACCOUNT] = @ACCOUNT AND Enabled = 1 AND BLACKLIST = 0");
            sql.Parameters.AddWithValue("@ACCOUNT", param.email);
            DataTable dt = db.GetResult(sql);
            if (dt.Rows.Count > 0)
            {
                string id = dt.Rows[0]["ID"].ToString();
                string name = dt.Rows[0]["NAME"].ToString();
                UserLoginAction(id, name);
            }
            else
            {
                List<Member> members_ = new List<Member>();
                string _sql = @"INSERT INTO[MEMBER] ([ID], [ACCOUNT], [PASSWORD], [PWD_SALT], [NAME], [PHONE], [ADDRESS], [LOGIN_TYPE], [LOGIN_ID], [GENDER], [BLACKLIST], [Enabled]) 
                                VALUES(@ID, @ACCOUNT, @PASSWORD, @PWD_SALT, @NAME, @PHONE, @ADDRESS, @LOGIN_TYPE, @LOGIN_ID, @GENDER, @BLACKLIST, @Enabled)";

                Guid guid = Guid.NewGuid();
                if (m_IDrapperHelp == null) m_IDrapperHelp = new WebShopping.Helpers.DapperHelper();
                int i = m_IDrapperHelp.ExecuteSql<Member>(_sql, new Member()
                {
                    Id = guid,
                    Account = param.email,
                    Password = "",
                    Pwd_Salt = "",
                    Name = param.name,
                    Phone = "",
                    Address = "",
                    login_type = "FB",
                    login_id = param.id,
                    Gender = 2,
                    Blacklist = 0,
                    Enabled = 1
                });
                if(i==1)
                    UserLoginAction(guid.ToString(), param.name);
            }
            return true;
        }

        public bool LineLogin(LineUserVerifyDto param)
        {
            SqlCommand sql = new SqlCommand(@"SELECT [ID], [NAME] FROM [member] WHERE [ACCOUNT] = @ACCOUNT AND Enabled = 1 AND BLACKLIST = 0");
            sql.Parameters.AddWithValue("@ACCOUNT", param.email);
            DataTable dt = db.GetResult(sql);
            if (dt.Rows.Count > 0)
            {
                string id = dt.Rows[0]["ID"].ToString();
                string name = dt.Rows[0]["NAME"].ToString();
                UserLoginAction(id, name);
            }
            else
            {
                List<Member> members_ = new List<Member>();
                string _sql = @"INSERT INTO[MEMBER] ([ID], [ACCOUNT], [PASSWORD], [PWD_SALT], [NAME], [PHONE], [ADDRESS], [LOGIN_TYPE], [LOGIN_ID], [GENDER], [BLACKLIST], [Enabled]) 
                                VALUES(@ID, @ACCOUNT, @PASSWORD, @PWD_SALT, @NAME, @PHONE, @ADDRESS, @LOGIN_TYPE, @LOGIN_ID, @GENDER, @BLACKLIST, @Enabled)";

                Guid guid = Guid.NewGuid();
                if (m_IDrapperHelp == null) m_IDrapperHelp = new WebShopping.Helpers.DapperHelper();
                int i = m_IDrapperHelp.ExecuteSql<Member>(_sql, new Member()
                {
                    Id = guid,
                    Account = param.email,
                    Password = "",
                    Pwd_Salt = "",
                    Name = param.name,
                    Phone = "",
                    Address = "",
                    login_type = "Line",
                    login_id = param.sub,
                    Gender = 2,
                    Blacklist = 0,
                    Enabled = 1
                });
                if (i == 1)
                    UserLoginAction(guid.ToString(), param.name);
            }
            return true;
        }

        public bool GoogleLogin(GoogleUserInfoDto param)
        {
            SqlCommand sql = new SqlCommand(@"SELECT [ID], [NAME] FROM [member] WHERE [ACCOUNT] = @ACCOUNT AND Enabled = 1 AND BLACKLIST = 0");
            sql.Parameters.AddWithValue("@ACCOUNT", param.email);
            DataTable dt = db.GetResult(sql);
            if (dt.Rows.Count > 0)
            {
                string id = dt.Rows[0]["ID"].ToString();
                string name = dt.Rows[0]["NAME"].ToString();
                UserLoginAction(id, name);
            }
            else
            {
                List<Member> members_ = new List<Member>();
                string _sql = @"INSERT INTO[MEMBER] ([ID], [ACCOUNT], [PASSWORD], [PWD_SALT], [NAME], [PHONE], [ADDRESS], [LOGIN_TYPE], [LOGIN_ID], [GENDER], [BLACKLIST], [Enabled]) 
                                VALUES(@ID, @ACCOUNT, @PASSWORD, @PWD_SALT, @NAME, @PHONE, @ADDRESS, @LOGIN_TYPE, @LOGIN_ID, @GENDER, @BLACKLIST, @Enabled)";

                Guid guid = Guid.NewGuid();
                if (m_IDrapperHelp == null) m_IDrapperHelp = new WebShopping.Helpers.DapperHelper();
                int i = m_IDrapperHelp.ExecuteSql<Member>(_sql, new Member()
                {
                    Id = guid,
                    Account = param.email,
                    Password = "",
                    Pwd_Salt = "",
                    Name = param.name,
                    Phone = "",
                    Address = "",
                    login_type = "Google",
                    login_id = param.Id,
                    Gender = 2,
                    Blacklist = 0,
                    Enabled = 1
                });
                if (i == 1)
                    UserLoginAction(guid.ToString(), param.name);
            }
            return true;
        }

        /// <summary>
        /// 會員購物車品項數量
        /// </summary>
        /// <param name="member_id"></param>
        /// <returns></returns>
        public int ShoppingCartCount(Guid member_id) {
            int iCount = 0;

            string sql_ = $@"Select count(1) from order_item where orders_id=(Select id from [orders] where member_id=@member_id and order_status_id='99')";
            SqlCommand sql = new SqlCommand(sql_);
            sql.Parameters.AddWithValue("@member_id", member_id);
            db.SqlConn.Open();
            iCount = Convert.ToInt32(db.GetScalar(sql));
            db.SqlConn.Close();

            return iCount;
        }


        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public void UserLoginAction(string id, string name)
        {
            string _sql = $"UPDATE [MEMBER] SET lastlogined=GETDATE() WHERE [ID]='{id}'";
            if (m_IDrapperHelp == null) m_IDrapperHelp = new WebShopping.Helpers.DapperHelper();
            m_IDrapperHelp.ExecuteSql(_sql);

            string module = JsonConvert.SerializeObject(new
            {
                id = id,
                name = name,
                module = "",
                Identity = Role.GetIdentityRole(1)
            });

            var ticket = new FormsAuthenticationTicket(
            version: 1,
            name: id, //可以放使用者Id
            issueDate: DateTime.UtcNow,//現在UTC時間
            expiration: DateTime.UtcNow.AddMinutes(30),//Cookie有效時間=現在時間往後+30分鐘
            isPersistent: true,// 是否要記住我 true or false
            userData: module, //可以放使用者角色名稱
            cookiePath: FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket); //把驗證的表單加密
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            //cookie.Domain = HttpContext.Current.Request.UrlReferrer.DnsSafeHost;
            cookie.Domain = HttpContext.Current.Request.Url.IdnHost;
            cookie.Expires = DateTime.UtcNow.AddMinutes(30);
            //cookie.SameSite = SameSiteMode.None;
            //cookie.Secure = true;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public enum EmunPwdInfo
        {
            Logined,//登入，記登入時間
            TempPwdLogined,//臨時密碼登入成功
            OldPwdLogined//舊密碼登入成功

        }

        public string UpdateMemberInfo(Guid p_Id, RecvMemberPasswordInfoDto p_RecvMemberUpdateDto, EmunPwdInfo emunPwdInfo)
        {
            string sql_ = string.Empty;
            if (m_IDrapperHelp == null) m_IDrapperHelp = new WebShopping.Helpers.DapperHelper();

            switch (emunPwdInfo)
            {
                case EmunPwdInfo.Logined:
                    //登入，記登入時間
                    sql_ = @"UPDATE [MEMBER] SET [lastlogined]=getdate() WHERE [ID] = @ID";
                    m_IDrapperHelp.ExecuteSql<Member>(sql_, new Member()
                    {
                        Id = p_Id
                    });
                    break;
                case EmunPwdInfo.TempPwdLogined:
                    //臨時密碼登入，臨時密碼寫成正式，清空臨時密碼
                    sql_ = @"UPDATE [MEMBER] SET [Password]=@Password, [Temp_Password] = NULL, [lastlogined]=getdate() WHERE [ID] = @ID";
                    m_IDrapperHelp.ExecuteSql<Member>(sql_, new Member()
                    {
                        Id = p_Id,
                        Password = p_RecvMemberUpdateDto.TempPassword
                    });
                    break;
                case EmunPwdInfo.OldPwdLogined:
                    //舊密碼成功登入，清空臨時密碼
                    sql_ = @"UPDATE [MEMBER] SET [Temp_Password] = NULL, [lastlogined]=getdate() WHERE [ID] = @ID";
                    m_IDrapperHelp.ExecuteSql<Member>(sql_, new Member()
                    {
                        Id = p_Id
                    });
                    break;
            }

            return string.Empty;
        }

    }
}