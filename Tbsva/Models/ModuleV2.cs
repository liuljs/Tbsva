using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    //   id authId  name path    belong view    edit category
    //8AD19281-3444-4282-82BC-15338ABBEDE5 e01 首頁輪播管理 carousel.html   front   1	1	editCategory
    //EA50CA97-433B-4E5B-8C30-3BBF4904BD91 f01 一般捐款 donate_01.html      1	1	financialCategory
    //7B6093B0-3C84-405C-B7CA-9C71C62F3C33 m01 資訊列表 index.html back	1	1	managerCategory
    /// <summary>
    /// [module]資料表
    /// </summary>
    public class ModuleV2
    {
        public Guid id { get; set; }
        public string authId { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string belong { get; set; }
        public string view { get; set; }
        public string edit { get; set; }
        public string category { get; set; }
    }
}