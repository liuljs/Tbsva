using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Member
    {
        /// <summary>
        /// 會員Id
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// 會員編號
        /// </summary>        
        public int No { get; set; }

        [StringLength(50), EmailAddress]
        /// <summary>
        /// 登入帳號(信箱)
        /// </summary>
        public string Account { get; set; }

        [StringLength(100)]
        /// <summary>
        /// 登入密碼        
        /// </summary>
        public string Password { get; set; }

        [StringLength(100)]
        /// <summary>
        /// 臨時密碼        
        /// </summary>
        public string Temp_Password { get; set; }

        [StringLength(60)]
        /// <summary>
        /// 雜湊碼
        /// </summary>
        public string Pwd_Salt { get; set; }        

        [StringLength(100)]
        /// <summary>
        /// 輸入新密碼        
        /// </summary>
        public string New_Password { get; set; }

        [StringLength(100)]
        /// <summary>
        /// 再次輸入新密碼        
        /// </summary>
        public string New_Password_Again { get; set; }

        [StringLength(10)]
        /// <summary>
        /// 驗證碼
        /// </summary>
        public string Capthcas { get; set; }

        [StringLength(30)]
        /// <summary>
        /// 姓名        
        /// </summary>
        public string Name { get; set; }       

        /// <summary>
        /// 建立時間        
        /// </summary>
        public DateTime Creation_Date { get; set; }

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

        /// <summary>
        /// 最後登入時間
        /// </summary>
        public DateTime LastLogined { get; set; }
    }  
}