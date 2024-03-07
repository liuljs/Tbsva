using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 解析編輯器內容
    /// </summary>
    public class EditorDto
    {
        public class Img
        {
            public string image { get; set; }
        }
        public class Attributes
        {
            public string align { get; set; }
            public int header { get; set; }
            public string color { get; set; }
        }

        public class Op
        {
            public Attributes attributes { get; set; }
            public object insert { get; set; }
        }

        public class Root
        {
            public List<Op> ops { get; set; }
        }
    }
}