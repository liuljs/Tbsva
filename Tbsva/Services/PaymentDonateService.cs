using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using WebShopping.Dtos;
using WebShopping.Helpers;

namespace WebShopping.Services
{
    public class PaymentDonateService : IPaymentDonateService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        public PaymentDonateService(IDapperHelper iDapperHelper)
        {
            _IDapperHelper = iDapperHelper;
        }
        #endregion

        /// <summary>
        /// 將虛擬帳號或超商代碼傳進來的欄位更新捐款主表資料
        /// </summary>
        /// <param name="PaymentReceiveDto"></param>
        public void UpdateReceive(PaymentReceiveDto PaymentReceiveDto)
        {
            string _sql = @"UPDATE [Donate] 
                                            SET 
                                             [Code] = @Code
                                            ,[OrderNo] = @OrderNo
                                            ,[AcquirerOrderNo] = @AcquirerOrderNo
                                            ,[AtmBankNo] = @AtmBankNo
                                            ,[AtmNo_CvsNo] = ISNULL(@AtmNo,'')+ISNULL(@CvsNo,'')
                                            ,UpdateDate=GETDATE()
                                            ,[PayEndDate] = @PayEndDate
                                            ,[Pay_zg] = @Pay_zg
                                            WHERE OrderId = @OrderNo";
            _IDapperHelper.ExecuteSql(_sql, PaymentReceiveDto);
        }

        /// <summary>
        /// 信用卡金流回覆付款
        /// </summary>
        /// <param name="PaymenetReturn"></param>
        public void UpdateReturn(PaymentReturnDto PaymenetReturn)
        {
            int PayStatus = 1;
            if (PaymenetReturn.Code == "000") PayStatus = 2;       //1待付款2已付款

            string _sql = $@"UPDATE [Donate] 
                                            SET 
                                             [Code] = @Code
                                            ,[OrderNo] = @OrderNo
                                            ,[AcquirerOrderNo] = @AcquirerOrderNo
                                            ,UpdateDate=GETDATE()
                                            ,[AuthAmount] = @AuthAmount
                                            ,[AuthTime] = @AuthTime
                                            ,[PayStatus] = {PayStatus}
                                            WHERE OrderId = @OrderNo";

           _IDapperHelper.ExecuteSql(_sql, PaymenetReturn);
        }

        public bool ValidateToken(HttpRequest _request, out string result)
        {
            string ValidateToken = HttpUtility.UrlDecode(_request.Form["ValidateToken"], Encoding.UTF8);
            string HashData = _request.Form["OrderNo"]
                + _request.Form["Amount"]
                + _request.Form["AcquirerOrderNo"]
                + _request.Form["Code"]
                + _request.Form["AuthCode"];
            string validateKey = "validateKey"; //商家自訂驗證字串
            byte[] bytes = Encoding.UTF8.GetBytes(HashData); //OrderNoAmountAcquirerOrderNoCode
            result = string.Empty;
            byte[] hashBytes;

            if (string.IsNullOrEmpty(validateKey))
            {
                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                hashBytes = sha1.ComputeHash(bytes);
            }
            else
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(validateKey);  //1加入商家自訂驗證字串
                HMACSHA1 sha2 = new HMACSHA1(keyBytes); 
                hashBytes = sha2.ComputeHash(bytes);                              //2加入商家自訂驗證字串+HashData
            }
            foreach (var b in hashBytes)
            {
                result += b.ToString("x2");  //--變成十六進位
            }
            return (result == ValidateToken);
        }

        public void WriteLogFileTxt(HttpRequest _request, string _Way, string JSON, string result)
        {
            //記錄文字檔
            SystemFunctions.WriteLogFile($"PaymentDonate/{_Way}:check=({result == _request.Form["ValidateToken"]}) " +
                $"OrderNo:[{ _request.Form["OrderNo"]}]-" +
                $"Amount:[{ _request.Form["Amount"]}]-" +
                $"AcquirerOrderNo:[{ _request.Form["AcquirerOrderNo"]}]-" +
                $"Code:[{ _request.Form["Code"]}]-" +
                $"AuthCode:[{ _request.Form["AuthCode"]}];" +
                $"json=({JSON})" +
                $"result=({result});" +
                $"ValidateToken=({_request.Form["ValidateToken"]})");
        }
    }
}