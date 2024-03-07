using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    /// <summary>
    /// 目錄加模組
    /// </summary>
    public class ClassificationModuleV2
    {
        public string categoryId { get; set; }

        public string category { get; set; }

        public string name { get; set; }

        public List<ModuleV2> auth { get; set; }
    }
}