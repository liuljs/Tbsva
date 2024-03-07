using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShoppingAdmin.Models
{
    public class ManagerGroupInsertParam
    {
        /// <summary>
        /// 管理帳號ID
        /// </summary>
        [Required]
        public Guid account_id { get; set; }

        /// <summary>
        /// 管理群組ID
        /// </summary>        
        public Guid id { get; set; }

        /// <summary>
        /// 群組名稱
        /// </summary>
        [Required, StringLength(20)]
        public string name { get; set; }

        [Required]
        public List<Module> auth { get; set; }
    }
}