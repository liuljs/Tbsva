using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    /// <summary>
    /// manager 資料表
    /// </summary>
    public class AuthParamV2
    {
        public Guid id { get; set; }

        public string account { get; set; }

        public string password { get; set; }

        public string pwd_salt { get; set; }

        public string email { get; set; }

        public string name { get; set; }

        public DateTime create_date { get; set; }

        public string deleted { get; set; }

        public DateTime? delete_date { get; set; }

        public byte? status { get; set; }

        public byte enabled { get; set; }

        public DateTime? lastlogined { get; set; }
    }
}