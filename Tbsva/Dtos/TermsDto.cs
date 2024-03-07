using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class SendTermsGetContentDto
    {
        /// <summary>
        /// 取得內容的Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 編輯服務條款內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public string Creation_Date { get; set; }        
    }

    public class SendTermsAddImageDto
    {
        /// <summary>
        /// 圖片路徑連結位置
        /// </summary>
        public string Image_Link { get; set; }
    }
}