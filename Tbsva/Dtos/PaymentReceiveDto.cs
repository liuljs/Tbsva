using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 付款資訊(虛擬帳號)
    /// </summary>
    public class PaymentReceiveDto
    {
        /// <summary>
        /// 編碼方式
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// 商家名稱
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 子付款方式
        /// </summary>
        public string Pay_zg { get; set; }

        /// <summary>
        /// 超商代碼
        /// </summary>
        public string CvsNo { get; set; }

        /// <summary>
        /// 消費者備註
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 回傳成功或失敗
        /// Successded
        /// </summary>
        public string Desc { get; set; }

        public string Status { get; set; }

        public string Rvg2c { get; set; }

        public string Dcvc { get; set; }

        public string SmilePayNO { get; set; }

        /// <summary>
        /// 商家訂單號
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 訂單金額
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 銀行代碼
        /// </summary>
        public string AtmBankNo { get; set; }

        /// <summary>
        /// 虛擬帳號
        /// </summary>
        public string AtmNo { get; set; }

        /// <summary>
        /// 超商繳費代碼
        /// </summary>
        public string PaymentNo { get; set; }

        /// <summary>
        /// 付款截止日
        /// </summary>
        public string PayEndDate { get; set; }

        /// <summary>
        /// 金流商訂單編號
        /// </summary>
        public string AcquirerOrderNo { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string Od_sob { get; set; }

        /// <summary>
        /// 首頁位置
        /// </summary>
        public string GoBackURL { get; set; }

        /// <summary>
        /// 虛擬帳號接收頁
        /// </summary>
        public string ReceiveURL { get; set; }

        public string RequiredConfirm { get; set; }

        /// <summary>
        /// 狀態碼
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string ValidateToken { get; set; }

        /// <summary>
        /// 傳送方式
        /// </summary>
        public string SendType { get; set; }


        public string json { get; set; }

    }

}