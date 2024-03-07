using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IDonateService
    {
        /// <summary>
        /// 新增捐款表單資料Donate ,DonateRelatedItemRecord
        /// 前端網頁會處理=>此API確認寫入OK=>送金流=>金流反回=>PaymentController.cs(Receive,Return)
        /// PaymentController前面內容一樣，但UpdateReceive,UpdateReturn裏面資料表需要改寫
        /// </summary>
        /// <param name="request"></param>
        Donate Insert_Donate(HttpRequest request);

        /// <summary>
        /// 取得單筆內容
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns>Donate</returns>
        Donate Get_Donate(string OrderId);

        /// <summary>
        /// 取得所有內容(後端用)
        /// </summary>
        /// <param name="count">一頁要顯示幾筆</param>
        /// <param name="page">第幾頁開始</param>
        /// <param name="OrderId">搜尋訂單編號2021 10 28 001</param>
        /// <param name="DonateType">1一般捐款2 結緣捐贈</param>
        /// <param name="BuyerName">搜尋捐款姓名</param>
        /// <param name="PayType">搜尋付款方式01刷卡2虛擬帳號</param>
        /// <param name="StartDate">OrderDate訂單日期開始</param>
        /// <param name="EndDate">OrderDate訂單日期結束</param>
        /// <param name="PayStatus">搜尋繳款狀態1待付款2已付款</param>
        /// <returns>List<Donate></returns>
        List<Donate> Get_Donate_ALL(int? count, int? page, string OrderId, int? DonateType, string BuyerName, string PayType, DateTime? StartDate, DateTime? EndDate, int? PayStatus);

        /// <summary>
        /// 取得所有內容(前端用)
        /// </summary>
        /// <param name="BuyerName">搜尋捐款姓名</param>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <returns>List<Donate></returns>
        List<Donate> Get_Donations_ALL(string BuyerName, int? count, int? page);

        /// <summary>
        /// 更新捐款狀態為已結案
        /// </summary>
        /// <param name="donate">.OrderId</param>
        void UpdateDonateStatus(Donate donate);

        /// <summary>
        /// 更新繳款狀態為已付款(83銀行匯款)
        /// </summary>
        /// <param name="donate">.OrderId</param>
        void UpdateBankRemittanceStatus(Donate donate);


        /// <summary>
        /// 更新繳款狀態為已付款(2虛擬帳號, 7超商代碼)
        /// </summary>
        /// <param name="donate">.OrderId</param>
        void UpdateVirtualAtmSuperMarketStatus(Donate donate);

        /// <summary>
        /// 取得未過期的2虛擬帳號, 7超商代碼
        /// </summary>
        /// <param name="OrderId">搜尋訂單編號2021 10 28 001</param>
        /// <returns>Donate</returns>
        Donate Get_Donate_UpdateVirtualAtmSuperMarketStatus(string OrderId);

    }
}
