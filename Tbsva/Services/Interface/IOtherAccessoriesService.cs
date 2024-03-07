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
    public interface IOtherAccessoriesService
    {
        /// <summary>
        /// 新增也是編輯(先刪除在新增)
        /// </summary>
        /// <param name="request">接收表單內容</param>
        /// <returns>201</returns>
        OtherAccessories Insert_OtherAccessories(HttpRequest request);

        /// <summary>
        /// 清空所有資料表內容
        /// </summary>
        void DeleteAllContents();

        /// <summary>
        /// 新增其它配件或服務的圖片
        /// </summary>
        /// <param name="_request">插入圖示按鍵</param>
        /// <returns>201, _imageUrl圖片URL</returns>
        string AddImage(HttpRequest _request);

        /// <summary>
        /// 取得其它配件或服務內容
        /// </summary>
        /// <returns>OtherAccessories</returns>
        OtherAccessories Get_OtherAccessories();
    }
}
