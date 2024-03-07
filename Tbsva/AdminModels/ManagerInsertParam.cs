using System;
using System.ComponentModel.DataAnnotations;

namespace WebShoppingAdmin.Models
{
    /// <summary>
    /// Manager/Insert新增管理帳號
    /// </summary>
    public class ManagerInsertParam 
    {
        /// <summary>
        /// 登入帳號ID
        /// </summary>
        //[Required]
        public Guid account_id { get; set; }
        ///// <summary>
        ///// 管理群組ID
        ///// </summary>        
        //public Guid id { get; set; }
        /// <summary>
        /// 管理帳號
        /// </summary>
        [Required, StringLength(20), RegularExpression(@"[A-Za-z0-9]+")]
        public string account { get; set; }
        /// <summary>
        /// 帳號名稱
        /// </summary>
        [Required, StringLength(30)]
        public string name { get; set; }
        /// <summary>
        /// 帳號郵件
        /// </summary>
        [Required, StringLength(50), EmailAddress]
        public string email { get; set; }
        /// <summary>
        /// 群組ID陣列
        /// </summary>
        public string[] groups { get; set; }
    }
}