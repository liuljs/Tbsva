using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 產品規格
    /// </summary>
    public class SpecDto
    {
        /// <summary>
        /// 規格Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 產品Id
        /// </summary>
        public int Spu_Id { get; set; }
    }
}