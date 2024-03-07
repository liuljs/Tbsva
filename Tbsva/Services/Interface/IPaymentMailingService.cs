using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IPaymentMailingService
    {
        /// <summary>
        /// 新增也是編輯(先刪除在新增)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        PaymentMailing Insert_PaymentMailing(HttpRequest request);

        /// <summary>
        /// 清空所有資料表內容
        /// </summary>
        /// <returns></returns>
        int DeleteAllContents();

        /// <summary>
        /// 新增付款及郵寄介紹的圖片
        /// </summary>
        /// <param name="_request">插入圖示按鍵</param>
        /// <returns>201, _imageUrl圖片URL</returns>
        string AddImage(HttpRequest _request);

        /// <summary>
        /// 取得付款及郵寄介紹內容
        /// </summary>
        /// <returns></returns>
        PaymentMailing Get_PaymentMailing();
    }
}
