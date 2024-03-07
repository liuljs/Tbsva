using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Dtos
{
    //***自行放入資料（如：會員權限、角色、等級）
    public class UserIdentityCookieDto
    {
        /// <summary>
        /// manager.id
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// manager.name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 取得登入資訊
        /// </summary>
        //public List<ClassificationModuleV2Dto> auths { get; set; }

        /// <summary>
        /// CustomAuthorize   Role.GetIdentityRole(Convert.ToInt32(authParamV2.status));
        /// </summary>
        public string identity { get; set; }
    }
}