using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 會員資訊(傳送)
    /// </summary>
    public class SendMemberGetDto
    {
        /// <summary>
        /// 會員Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 會員編號
        /// </summary>        
        public string No { get; set; }

        /// <summary>
        /// 登入帳號(信箱)
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 姓名        
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性別        
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// 建立時間        
        /// </summary>
        public string Creation_Date { get; set; }

        /// <summary>
        /// 生日        
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 電話號碼        
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 住址        
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 黑名單
        /// </summary>
        public byte Blacklist { get; set; }

        /// <summary>
        /// 啟停用
        /// </summary>
        public byte Enabled { get; set; }

        /// <summary>
        /// 最後登入時間
        /// </summary>
        public string LastLogin { get; set; }

    }

    /// <summary>
    /// 新增會員資訊(收到)
    /// </summary>
    public class RecvMemberAddDto
    {
        /// <summary>
        /// 使用者ID
        /// </summary>
        public Guid id { get; set; }

        [Required, StringLength(50), EmailAddress]
        /// <summary>
        /// 登入帳號(信箱)
        /// </summary>
        public string Account { get; set; }

        [Required, StringLength(30)]
        /// <summary>
        /// 姓名        
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 生日        
        /// </summary>
        public DateTime Birthday { get; set; }

        [Required, StringLength(15)]
        /// <summary>
        /// 電話號碼        
        /// </summary>
        public string Phone { get; set; }

        [StringLength(50)]
        /// <summary>
        /// 住址        
        /// </summary>
        public string Address { get; set; }

        [StringLength(30)]
        /// <summary>
        /// 密碼        
        /// </summary>
        public string Password { get; set; }

        [StringLength(10)]
        /// <summary>
        /// 第三方的類型        
        /// </summary>
        public string login_type { get; set; }

        [StringLength(100)]
        /// <summary>
        /// 第三方的ID   
        /// </summary>
        public string login_id { get; set; }

        [Required]
        /// <summary>
        /// 性別        
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// 黑名單        
        /// </summary>
        public byte Blacklist { get; set; }

        /// <summary>
        /// 啟停用
        /// </summary>
        public byte Enabled { get; set; }
    }

    /// <summary>
    /// 更新會員資訊(收到)
    /// </summary>
    public class RecvMemberUpdateDto
    {
        [Required, StringLength(50), EmailAddress]
        /// <summary>
        /// 登入帳號(信箱)
        /// </summary>
        public string Account { get; set; }

        [Required, StringLength(30)]
        /// <summary>
        /// 姓名        
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 生日        
        /// </summary>
        public DateTime Birthday { get; set; }

        [StringLength(15)]
        /// <summary>
        /// 電話號碼        
        /// </summary>
        public string Phone { get; set; }

        [StringLength(50)]
        /// <summary>
        /// 住址        
        /// </summary>
        public string Address { get; set; }

        [Required]
        /// <summary>
        /// 性別        
        /// </summary>
        public byte Gender { get; set; }

        //[Required]
        /// <summary>
        /// 黑名單        
        /// </summary>
        public byte Blacklist { get; set; }

        /// <summary>
        /// 啟停用
        /// </summary>
        public byte Enabled { get; set; }
    }

    /// <summary>
    /// 更新會員密碼(收到)
    /// </summary>
    public class RecvMemberPasswordDto
    {
        /// <summary>
        /// 使用者ID
        /// </summary>
        public Guid id { get; set; }
        [StringLength(30)]
        /// <summary>
        /// 原密碼        
        /// </summary>
        public string OldPassword { get; set; }
        [StringLength(30)]
        /// <summary>
        /// 密碼        
        /// </summary>
        public string Password { get; set; }
    }
    /// <summary>
    /// 忘記會員密碼(收到)
    /// </summary>
    public class RecvForgetPasswordDto
    {
        [Required, StringLength(50), EmailAddress]
        /// <summary>
        /// 登入帳號(信箱)
        /// </summary>
        public string Account { get; set; }
    }

    public class RecvMemberPasswordInfoDto
    {
        /// <summary>
        /// 使用者ID
        /// </summary>
        public Guid id { get; set; }
        [StringLength(30)]
        /// <summary>
        /// 密碼        
        /// </summary>
        public string Password { get; set; }
        [StringLength(30)]
        /// <summary>
        /// 臨時密碼        
        /// </summary>
        public string TempPassword { get; set; }
    }

    public class FBUserDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }

    public class LineUserTokenDto
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
        public string id_token { get; set; }
    }
    public class LineUserVerifyDto
    {
        public string iss { get; set; }
        public string sub { get; set; }
        public string aud { get; set; }
        public string exp { get; set; }
        public string[] amr { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string email { get; set; }
    }

    public class GoogleUserTokenDto
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public string refresh_token { get; set; }
    }
    public class GoogleUserInfoDto
    {
        public string Id { get; set; }
        public string email { get; set; }
        public bool verified_email { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
    }

    public class LoginInfoDto {
        public Guid id { get; set; }
        public string name { get; set; }
        public string module { get; set; }
        public string Identity { get; set; }
        /// <summary>
        /// 購物車品項數量
        /// </summary>
        public int Count { get; set; }

    }
}