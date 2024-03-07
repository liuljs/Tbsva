using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class SendAboutMeGetContentDto
    {
        /// <summary>
        /// 取得內容的Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 編輯關於我們內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public string Creation_Date { get; set; }        
    }

    public class SendAboutMeAddImageDto
    {
        /// <summary>
        /// 圖片路徑連結位置
        /// </summary>
        public string Image_Link { get; set; }
    }
}