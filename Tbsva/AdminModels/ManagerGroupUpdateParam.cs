using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using ecSqlDBManager;

namespace WebShoppingAdmin.Models
{
    /// <summary>
    /// ManagerGroup/Update 更新管理群組
    /// </summary>
    public class ManagerGroupUpdateParam : ManagerGroupDeleteParam
    {
        /// <summary>
        /// 群組名稱
        /// </summary>
        [Required, StringLength(20)]
        public string name { get; set; }

        [Required]
        public List<Module> auth { get; set; }
    }
}