using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class ModuleV2Dto
    {
        public string authId { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string belong { get; set; }
        public string view { get; set; }
        public string edit { get; set; }
        public string category { get; set; }
    }
}