using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    /// <summary>
    /// 權限模組外層目錄
    /// </summary>
    //    id	categoryId	  category	name
    //1	e00 editCategory    編輯類別
    //2	f00 financialCategory   財務類別
    //3	m00 managerCategory 管理類別
    public class Classification
    {
        public int id { get; set; }

        public string categoryId { get; set; }

        public string category { get; set; }

        public string name { get; set; }
    }
}