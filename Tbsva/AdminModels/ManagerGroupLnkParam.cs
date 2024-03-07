using System;
using System.ComponentModel.DataAnnotations;
//using ecSqlDBManager;

namespace WebShoppingAdmin.Models
{
    /// <summary>
    /// 	更新管理群組與模組的關聯
    /// </summary>
    public class ManagerGroupLnkParam
    {
        /// <summary>
        /// 管理帳號ID
        /// </summary>
        [Required]
        public Guid account_id { get; set; }

        /// <summary>
        /// 管理群組ID
        /// </summary>
        [Required]
        public Guid group_id { get; set; }

        /// <summary>
        /// 模組ID
        /// </summary>
        public Guid[] modules { get; set; }
    }
}