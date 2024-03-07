using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 付款資訊
    /// </summary>
    public class PaymentDto
    {
        /// <summary>
        /// 商家編號
        /// </summary>
        [Required]
        public string MerchantId { get { return Helpers.Tools.MerchantId; } }

        /// <summary>
        /// 端末編號
        /// </summary>
        [Required]
        public string TerminalId { get { return Helpers.Tools.TerminalId; } }

        /// <summary>
        /// 商家名稱
        /// </summary>
        [Required]
        public string MerchantName { get { return Helpers.Tools.Company_Name; } }

        /// <summary>
        /// 金流網址
        /// </summary>
        [Required]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 付款完成通知
        /// </summary>
        [Required]
        public string ReturnURL { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        [Required]
        public string PayType { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        [Required]
        public string OrderNo { get; set; }

        /// <summary>
        /// 訂單金額
        /// </summary>
        [Required]
        public int Amount { get; set; }

        /// <summary>
        /// 產品名稱
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// 消費者備註
        /// </summary>
        public string OrderDesc { get; set; }

        /// <summary>
        /// 編碼方式
        /// </summary>
        public string Encoding { get { return "utf-8"; } }

        /// <summary>
        /// 行動電話
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 市話
        /// </summary>
        public string TelNumber { get; set; }

        /// <summary>
        /// 寄送地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 信箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string ValidateKey { get; set; }

        /// <summary>
        /// 會員編號
        /// </summary>
        public string memberId { get; set; }

        /// <summary>
        /// 商家網址
        /// </summary>
        public string GoBackURL { get; set; }

        /// <summary>
        /// 虛擬帳號網址
        /// </summary>
        public string ReceiveURL { get; set; }

        /// <summary>
        /// 虛擬帳號繳款截止日
        /// </summary>
        public string DeadlineDate { get; set; }

        /// <summary>
        /// 要求強制回應
        /// </summary>
        public string RequiredConfirm { get { return "1"; }  }

        /// <summary>
        /// 發票資訊
        /// </summary>
        public InvoiceDto Invoice { get; set; }
    }

    public class InvoiceDto
    {
        /// <summary>
        /// 發票延遲開立天數
        /// </summary>
        public int deferred { get; set; }
        
        /// <summary>
        /// 發票型能
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// 三聯發票公司名
        /// </summary>
        public string InvoiceName { get; set; }
    }
}