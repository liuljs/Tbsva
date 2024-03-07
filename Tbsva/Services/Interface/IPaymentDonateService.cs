using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Dtos;

namespace WebShopping.Services
{
    public interface IPaymentDonateService
    {
        /// <summary>
        /// 虛擬帳號或超商代碼回傳進來時，將帳號更新進訂單資料表
        /// </summary>
        /// <param name="PaymentReceiveDto">付款資訊(虛擬帳號超商代碼)</param>
        void UpdateReceive(PaymentReceiveDto PaymentReceiveDto);

        /// <summary>
        /// 信用卡付款刷卡回傳回來時，更新成功或失敗於訂單狀態PayStatus繳款狀態
        /// </summary>
        /// <param name="PaymenetReturn">信用卡付款資訊</param>
        void UpdateReturn(PaymentReturnDto PaymenetReturn);

        /// <summary>
        /// 將金流回傳表單欄位收集，然後驗證金流傳來的已用公式加密ValidateToken欄位
        /// 在與和金流一樣公式加密產生的result看兩個是否相等，以防止別的地方發送使用，發送者需有ValidateToken
        /// ValidateToken經ValidateToken接收後，使用幾個回傳值組成HashData，經加密後比對與ValidateToken是否相同
        /// </summary>
        /// <param name="request">接收金流回傳的表單值</param>
        /// <param name="result">依公式加參數編出來的驗證碼回傳出來</param>
        /// <returns>true,false</returns> 
        bool ValidateToken(HttpRequest request, out string result);

        /// <summary>
        /// 金流回傳表單值記錄文字
        /// </summary>
        /// <param name="request">接收金流回傳的表單值</param>
        /// <param name="Way">Receive,Return</param>
        /// <param name="JSON">SystemFunctions.GetJsonData組成的金流回傳所有值</param>
        /// <param name="result">依公式加參數編出來的驗證碼</param>
        void WriteLogFileTxt(HttpRequest request, string Way, string JSON, string result);
    }
}
