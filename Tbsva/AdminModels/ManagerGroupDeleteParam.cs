using System;
using System.ComponentModel.DataAnnotations;
//using ecSqlDBManager;

namespace WebShoppingAdmin.Models
{
    /// <summary>
    /// ManagerGroup/Delete 刪除管理群組
    /// </summary>
    public class ManagerGroupDeleteParam
    {
        /// <summary>
        /// 管理帳號ID
        /// </summary>
        [Required(ErrorMessage = "帳號ID必填")]
        public Guid account_id { get; set; }
        /// <summary>
        /// 管理群組ID
        /// </summary>
        [Required(ErrorMessage = "群組ID必填")]
        public Guid id { get; set; }
    }
}