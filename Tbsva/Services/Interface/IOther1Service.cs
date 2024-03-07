using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IOther1Service
    {
        /// <summary>
        /// 新增也是編輯(先刪除在新增)
        /// </summary>
        /// <param name="request">接收表單內容</param>
        /// <returns>201</returns>
        Other1 Insert_Other1(HttpRequest request);

        /// <summary>
        /// 清空所有資料表內容
        /// </summary>
        void DeleteAllContents();

        /// <summary>
        /// 新增剌符介紹的圖片
        /// </summary>
        /// <param name="_request">插入圖示按鍵</param>
        /// <returns>201, _imageUrl圖片URL</returns>
        string AddImage(HttpRequest _request);

        /// <summary>
        /// 取得剌符介紹內容
        /// </summary>
        /// <returns>Other1</returns>
        Other1 Get_Other1();
    }
}
