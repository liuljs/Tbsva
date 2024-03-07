using WebShopping.Dtos;
using WebShopping.Helpers;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebShopping.Services
{
    public class MemberService : IMemberService
    {
        IDapperHelper m_IDrapperHelp;

        public MemberService(IDapperHelper p_IDrapperHelp)
        {
            m_IDrapperHelp = p_IDrapperHelp;
        }

        /// <summary>
        /// 取得指定會員資料
        /// </summary>
        /// <param name="p_Id"> 會員Id</param>
        /// <returns></returns>
        public Member GetMember(Guid p_Id)
        {
            Member member_ = new Member();            

            string sql_ = @"SELECT * FROM [MEMBER] WHERE ID = @ID";
            member_.Id = p_Id;           
            member_ = m_IDrapperHelp.QuerySqlFirstOrDefault<Member>(sql_, member_);

            return member_;            
        }

        /// <summary>
        /// 取得所有會員
        /// </summary>
        /// <returns></returns>
        public List<Member> GetMembers(int? count, int? page)
        {
            string pages = "", where = "";
            if (count != null && count>0 && page != null && page>0)
            {
                int startRowIndex = 0;
                startRowIndex = Convert.ToInt32(page - 1) * Convert.ToInt32(count);
                pages = $" OFFSET {startRowIndex} ROWS FETCH NEXT {count} ROWS ONLY ";
            }

            Member member_ = new Member();
            List<Member> members_ = new List<Member>();
            
            string sql_ = $@"SELECT * FROM [MEMBER]
                            
                            ORDER BY no desc
                           {pages} ";            
            members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, member_).ToList();

            return members_;
        }

        /// <summary>
        /// 刪除指定會員
        /// </summary>
        /// <param name="p_Id"></param>
        /// <returns></returns>
        public void DeleteMember(Guid p_Id)
        {        
            Member member_ = new Member();
            string sql_ = @"DELETE FROM [MEMBER] WHERE ID = @ID";
            member_.Id = p_Id;
            m_IDrapperHelp.QuerySqlFirstOrDefault<Member>(sql_, member_);                            
        }

        /// <summary>
        /// 新增會員
        /// </summary>
        /// <param name="p_RecvMemberAddDto"> 會員資訊 </param>
        /// <param name="p_isAdmin"> 是否為後台 </param>
        /// <returns></returns>
        public MessageStatus AddMember(RecvMemberAddDto p_RecvMemberAddDto, bool p_isAdmin = false)
        {            
            List<Member> members_ = new List<Member>();

            //是否已有存在帳號(Email)
            string sql_ = @"SELECT * FROM [MEMBER] WHERE ACCOUNT = @ACCOUNT";
            members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, new Member() { Account = p_RecvMemberAddDto.Account}).ToList();
            if (members_.Count > 0){
                return MessageStatus.RepeatAccount; //已經有相同的帳號
            }

            //是否有存在的手機號碼
            sql_ = @"SELECT * FROM [MEMBER] WHERE PHONE = @PHONE";
            members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, new Member() { Phone = p_RecvMemberAddDto.Phone }).ToList();
            if (members_.Count > 0)
            {                
                return MessageStatus.RepeatPhone; //已經有相同的手機號碼
            }

            if (p_isAdmin)
            {   //後台(自動產生密碼,並寄郵件)
                //新增密碼
                string strSalt_ = Tools.GetInstance().CreateSaltCode(); //產生雜湊碼
                string strRandomPW_ = Tools.GetInstance().CreateRandomCode(10); //隨機取得密碼
                string strPwd_ = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{strRandomPW_}{strSalt_}"))).Replace("-", null);
                                
                string _sql = @"INSERT INTO[MEMBER] ([ID], [ACCOUNT], [PASSWORD], [PWD_SALT], [NAME], [BIRTHDAY], [PHONE], [ADDRESS], [LOGIN_TYPE], [LOGIN_ID], [GENDER], [BLACKLIST], [Enabled]) 
                                VALUES(@ID, @ACCOUNT, @PASSWORD, @PWD_SALT, @NAME, @BIRTHDAY, @PHONE, @ADDRESS, @LOGIN_TYPE, @LOGIN_ID, @GENDER, @BLACKLIST, @Enabled)";

                m_IDrapperHelp.ExecuteSql<Member>(_sql, new Member()
                {
                    Id = Guid.NewGuid(),
                    Account = p_RecvMemberAddDto.Account,
                    Password = strPwd_,
                    Pwd_Salt = strSalt_,
                    Name = p_RecvMemberAddDto.Name,
                    Birthday = p_RecvMemberAddDto.Birthday,
                    Phone = p_RecvMemberAddDto.Phone,
                    Address = p_RecvMemberAddDto.Address,
                    login_type = p_RecvMemberAddDto.login_type,
                    login_id = p_RecvMemberAddDto.login_id,
                    Gender = p_RecvMemberAddDto.Gender,
                    Blacklist = 0,
                    Enabled = 1
                });

                //發送Email
                SystemFunctions.SendMail(Tools.Company_Name, Tools.Admin_Mail, p_RecvMemberAddDto.Account, new List<string>(), new List<string>(),
                                         $"{Tools.Company_Name}密碼訊息通知", $"預設密碼 : {strRandomPW_}, 請至 {WebShopping.Helpers.Tools.WebSiteUrl}/backStage/login.html 更改密碼");
            }
            else
            {   //前台
                //新增密碼
                string strSalt_ = Tools.GetInstance().CreateSaltCode(); //產生雜湊碼
                string strRandomPW_ = p_RecvMemberAddDto.Password;//使用者自行輸入的密碼
                string strPwd_ = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{strRandomPW_}{strSalt_}"))).Replace("-", null);
                p_RecvMemberAddDto.id = Guid.NewGuid();

                string _sql = @"INSERT INTO[MEMBER] ([ID], [ACCOUNT], [PASSWORD], [PWD_SALT], [NAME], [BIRTHDAY], [PHONE], [ADDRESS], [LOGIN_TYPE], [LOGIN_ID], [GENDER], [BLACKLIST], [Enabled]) 
                                VALUES(@ID, @ACCOUNT, @PASSWORD, @PWD_SALT, @NAME, @BIRTHDAY, @PHONE, @ADDRESS, @LOGIN_TYPE, @LOGIN_ID, @GENDER, @BLACKLIST, @Enabled)";

                m_IDrapperHelp.ExecuteSql<Member>(_sql, new Member()
                {
                    Id = p_RecvMemberAddDto.id,
                    Account = p_RecvMemberAddDto.Account,
                    Password = strPwd_,
                    Pwd_Salt = strSalt_,
                    Name = p_RecvMemberAddDto.Name,
                    Birthday = p_RecvMemberAddDto.Birthday,
                    Phone = p_RecvMemberAddDto.Phone,
                    Address = p_RecvMemberAddDto.Address,
                    login_type = p_RecvMemberAddDto.login_type,
                    login_id = p_RecvMemberAddDto.login_id,
                    Gender = p_RecvMemberAddDto.Gender,
                    Blacklist = 0,
                    Enabled = 1
                });
            }

            return MessageStatus.None;
        }

        //public int AddFBMember(RecvMemberAddDto p_RecvMemberAddDto)
        //{
        //    List<Member> members_ = new List<Member>();
        //    p_RecvMemberAddDto.id = Guid.NewGuid();
        //    string _sql = @"INSERT INTO[MEMBER] ([ID], [ACCOUNT], [PASSWORD], [PWD_SALT], [NAME], [BIRTHDAY], [PHONE], [ADDRESS], [LOGIN_TYPE], [LOGIN_ID], [GENDER], [BLACKLIST], [Enabled]) 
        //                        VALUES(@ID, @ACCOUNT, @PASSWORD, @PWD_SALT, @NAME, @BIRTHDAY, @PHONE, @ADDRESS, @LOGIN_TYPE, @LOGIN_ID, @GENDER, @BLACKLIST, @Enabled)";

        //    int i=m_IDrapperHelp.ExecuteSql<Member>(_sql, new Member()
        //    {
        //        Id = p_RecvMemberAddDto.id,
        //        Account = p_RecvMemberAddDto.Account,
        //        Password = "",
        //        Pwd_Salt = "",
        //        Name = p_RecvMemberAddDto.Name,
        //        Birthday = p_RecvMemberAddDto.Birthday,
        //        Phone = p_RecvMemberAddDto.Phone,
        //        Address = p_RecvMemberAddDto.Address,
        //        login_type = p_RecvMemberAddDto.login_type,
        //        login_id = p_RecvMemberAddDto.login_id,
        //        Gender = p_RecvMemberAddDto.Gender,
        //        Blacklist = 0,
        //        Enabled = 1
        //    });

        //    return i;
        //}

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="p_Id"> 會員Id </param>
        /// <param name="p_RecvMemberUpdateDto"> 更新會員資訊 </param>
        /// <param name="p_isAdmin"> 是否為後台 </param>
        /// <returns>列舉會員帳號狀態MessageStatus</returns>    
        public MessageStatus UpdateMember(Guid p_Id, RecvMemberUpdateDto p_RecvMemberUpdateDto, bool p_isAdmin = false)
        {
            List<Member> members_ = new List<Member>();

            //是否已有存在帳號
            string sql_ = @"SELECT * FROM [MEMBER] WHERE ID = @ID";
            members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, new Member() { Id = p_Id }).ToList();

            if (members_.Count == 0)
            {
                //return "查無此帳號";
                return MessageStatus.NoSuchAccount;  //資料庫查無此帳號，無法修改
            }

            //後台
            if (p_isAdmin)
            {
                //是否已有存在帳號(Email)，不包含本身現在這個Guid p_Id(後端在編輯會員未驗證帳號重覆後再增加)
                sql_ = @"SELECT ACCOUNT FROM [MEMBER] WHERE ACCOUNT = @ACCOUNT AND ID<>@ID";
                members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, new Member() { Account = p_RecvMemberUpdateDto.Account, Id = p_Id }).ToList();
                if (members_.Count > 0)
                {
                    return MessageStatus.RepeatAccount;  //已經有相同的帳號
                }
                //是否有存在的手機號碼(增加)
                sql_ = @"SELECT PHONE FROM [MEMBER] WHERE PHONE = @PHONE AND ID<>@ID";
                members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, new Member() { Phone = p_RecvMemberUpdateDto.Phone, Id = p_Id }).ToList();
                if (members_.Count > 0)
                {
                    return MessageStatus.RepeatPhone; //已經有相同的手機號碼
                }

                //更新會員資訊(不包含密碼)
                sql_ = @"UPDATE[MEMBER] SET [ACCOUNT] = @ACCOUNT, [NAME] = @NAME, [BIRTHDAY] = @BIRTHDAY, [PHONE] = @PHONE, [ADDRESS] = @ADDRESS, 
                       [GENDER] = @GENDER, [BLACKLIST] = @BLACKLIST, [Enabled] = @Enabled WHERE [ID] = @ID";                                
                
                m_IDrapperHelp.ExecuteSql<Member>(sql_, new Member()
                {           
                    Id      = p_Id,
                    Account = p_RecvMemberUpdateDto.Account,                    
                    Name    = p_RecvMemberUpdateDto.Name,                    
                    Birthday= p_RecvMemberUpdateDto.Birthday,
                    Phone   = p_RecvMemberUpdateDto.Phone,
                    Address = p_RecvMemberUpdateDto.Address,
                    Gender = p_RecvMemberUpdateDto.Gender,
                    Blacklist = p_RecvMemberUpdateDto.Blacklist,
                    Enabled = p_RecvMemberUpdateDto.Enabled
                });                                                   
            }
            //前台
            else
            {
                //是否已有存在帳號(Email)
                sql_ = @"SELECT * FROM [MEMBER] WHERE ACCOUNT = @ACCOUNT AND ID<>@ID";
                members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, new Member() { Account = p_RecvMemberUpdateDto.Account, Id= p_Id }).ToList();
                if (members_.Count > 0)
                {
                    return MessageStatus.RepeatAccount; //已經有相同的帳號
                }

                //是否有存在的手機號碼
                sql_ = @"SELECT * FROM [MEMBER] WHERE PHONE = @PHONE AND ID<>@ID";
                members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, new Member() { Phone = p_RecvMemberUpdateDto.Phone, Id = p_Id }).ToList();
                if (members_.Count > 0)
                {
                    return MessageStatus.RepeatPhone; //已經有相同的手機號碼
                }

                //更新會員資訊(不包含密碼)
                sql_ = @"UPDATE[MEMBER] SET [ACCOUNT] = @ACCOUNT, [NAME] = @NAME, [BIRTHDAY] = @BIRTHDAY, [PHONE] = @PHONE, [ADDRESS] = @ADDRESS, 
                       [GENDER] = @GENDER WHERE [ID] = @ID";

                m_IDrapperHelp.ExecuteSql<Member>(sql_, new Member()
                {
                    Id = p_Id,
                    Account = p_RecvMemberUpdateDto.Account,
                    Name = p_RecvMemberUpdateDto.Name,
                    Birthday = p_RecvMemberUpdateDto.Birthday,
                    Phone = p_RecvMemberUpdateDto.Phone,
                    Address = p_RecvMemberUpdateDto.Address,
                    Gender = p_RecvMemberUpdateDto.Gender
                });
            }

            //return string.Empty;
            return MessageStatus.None;  //都沒有問題
        }

        /// <summary>
        /// 更新會員密碼
        /// </summary>
        /// <param name="p_Id"> 會員Id </param>
        /// <param name="p_RecvMemberPasswordDto"> 更新會員資訊 </param>
        /// <param name="p_isAdmin"> 是否為後台 </param>
        /// <returns></returns>    
        public string ChangePassword(Guid p_Id, RecvMemberPasswordDto p_RecvMemberPasswordDto, bool p_isAdmin = false)
        {
            List<Member> members_ = new List<Member>();

            //是否已有存在帳號
            string sql_ = @"SELECT * FROM [MEMBER] WHERE ID = @ID";
            members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, new Member() { Id = p_Id }).ToList();

            if (members_.Count == 0)
            {
                return "查無此帳號";
            }

            string strSalt_ = Tools.GetInstance().CreateSaltCode(); //產生雜湊碼
            string strRandomPW_ = p_RecvMemberPasswordDto.Password;//使用者自行輸入的密碼
            string strPwd_ = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{strRandomPW_}{strSalt_}"))).Replace("-", null);

            //後台
            if (p_isAdmin)
            {
                //更新會員密碼
                sql_ = @"UPDATE[MEMBER] SET [PWD_SALT]=@PWD_SALT,[password]=@password WHERE [ID] = @ID";

                m_IDrapperHelp.ExecuteSql<Member>(sql_, new Member()
                {
                    Id = p_Id,
                    Pwd_Salt = strSalt_,
                    Password = strPwd_
                });
            }
            //前台
            else
            {

                string realPwd = members_[0].Password;//資料庫加密後密碼
                string encryptPwd = $"{p_RecvMemberPasswordDto.OldPassword}{members_[0].Pwd_Salt}";//輸入的原密碼+Salt
                string pwd = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(encryptPwd))).Replace("-", null);//使用者輸入加密後密碼

                if (realPwd != pwd)
                {
                    return "原密碼錯誤";
                }

                //更新會員密碼
                sql_ = @"UPDATE[MEMBER] SET [PWD_SALT]=@PWD_SALT,[password]=@password WHERE [ID] = @ID";

                m_IDrapperHelp.ExecuteSql<Member>(sql_, new Member()
                {
                    Id = p_Id,
                    Pwd_Salt = strSalt_,
                    Password = strPwd_
                });
            }

            return string.Empty;
        }

        /// <summary>
        /// 忘記會員密碼
        /// </summary>
        /// <param name="p_RecvForgetPasswordDto"> 更新會員資訊 </param>
        /// <returns></returns>    
        public string ForgetPassword(RecvForgetPasswordDto p_RecvForgetPasswordDto)
        {
            List<Member> members_ = new List<Member>();

            //是否已有存在帳號
            string sql_ = @"SELECT * FROM [MEMBER] WHERE Account = @Account";
            members_ = m_IDrapperHelp.QuerySetSql<Member>(sql_, new Member() { Account = p_RecvForgetPasswordDto.Account }).ToList();

            if (members_.Count == 0)
            {
                return "查無此帳號";
            }

            string strSalt_ = members_[0].Pwd_Salt; //產生雜湊碼
            string strRandomPW_ = Tools.GetInstance().CreateRandomCode(10); //隨機取得密碼
            string strPwd_ = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes($"{strRandomPW_}{strSalt_}"))).Replace("-", null);

            //更新會員密碼
            sql_ = @"UPDATE[MEMBER] SET [temp_password]=@temp_password WHERE [Account] = @Account";

            m_IDrapperHelp.ExecuteSql<Member>(sql_, new Member()
            {
                Account = p_RecvForgetPasswordDto.Account,
                Temp_Password = strPwd_
            });

            //發送Email您好，這邊是饅頭頭購物商城！
            string p_strContent =
            $@"您好，這邊是{WebShopping.Helpers.Tools.Company_Name}！<br>" +
            $@"這邊是系統幫您預設的新密碼：{strRandomPW_} <br>" +
            $@"為保障您的權益，請在登入後立即修改您的密碼。<br>" +
            $@"網站登入網址：「{WebShopping.Helpers.Tools.WebSiteUrl}/member_login.html」<br><br>" +
            $@"如有任何問題歡迎您隨時回覆與我們聯繫，感謝您！<br>";

            SystemFunctions.SendMail(Tools.Company_Name, Tools.Admin_Mail, p_RecvForgetPasswordDto.Account, new List<string>(), new List<string>(),
                                     $"{Tools.Company_Name}密碼訊息通知", p_strContent);

            return string.Empty;//"新密碼通知信已寄出"
        }


        /// <summary>
        /// 加入追踪清單
        /// </summary>
        /// <param name="p_Id"></param>
        /// <param name="sku_id"></param>
        /// <returns></returns>
        public string AddWish(WishReceiveDto wish)
        {
            try {
                string sql_ = @"SELECT ID as sku_id FROM [SKU] WHERE ID=@SKU_ID";
                var sku = m_IDrapperHelp.QuerySqlFirstOrDefault(sql_, wish);
                if (sku == null) return $"商品編號[{wish.Sku_Id}]不存在!!";

                sql_ = @"IF NOT EXISTS(SELECT * FROM [MEMBER_WISH] WHERE MEMBER_ID=@MEMBER_ID AND SKU_ID=@SKU_ID)
                        INSERT INTO [MEMBER_WISH](MEMBER_ID,SKU_ID) VALUES(@MEMBER_ID,@SKU_ID)";
                int i = m_IDrapperHelp.ExecuteSql(sql_, wish);
                return "";
            }
            catch(Exception ex) {
                return ex.Message;
            }
        }

        public List<WishReturnDto> GetWish(WishReceiveDto wish)
        {
            try
            {
                string sql_ = @"SELECT m.*,p.id as spu_id,p.title as spu_title,k.title as sku_title,p.product_cover,sell_price,stock_qty,discount_price,discount_percent FROM [MEMBER_WISH] m left join SKU k on m.sku_id=k.id left join SPU p on k.spu_id=p.id
                                WHERE MEMBER_ID=@MEMBER_ID";
                var result = m_IDrapperHelp.QuerySetSql<WishReceiveDto, WishReturnDto>(sql_, wish).ToList();
                return result;
            }
            catch (Exception ex)
            {
                //return ex.Message;
                return null;
            }
        }

        public int DeleteWish(WishReceiveDto wish)
        {
            try
            {
                string sql_ = @"DELETE [MEMBER_WISH] WHERE MEMBER_ID=@MEMBER_ID AND SKU_ID=@SKU_ID";
                int i = m_IDrapperHelp.ExecuteSql(sql_, wish);
                return i;
            }
            catch
            {
                //return ex.Message;
                return 0;
            }
        }
        public int DeleteAllWish(WishReceiveDto wish)
        {
            try
            {
                string sql_ = @"DELETE [MEMBER_WISH] WHERE MEMBER_ID=@MEMBER_ID";
                int i = m_IDrapperHelp.ExecuteSql(sql_, wish);
                return i;
            }
            catch
            {
                //return ex.Message;
                return 0;
            }
        }

    }
}
