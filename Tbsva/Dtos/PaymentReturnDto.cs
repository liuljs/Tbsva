using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 付款資訊(付款成功)
    /// </summary>
    public class PaymentReturnDto
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
        /// 訂單金額
        /// </summary>
        [Required]
        public int Amount { get; set; }

        /// <summary>
        /// 授權金額
        /// </summary>
        [Required]
        public int AuthAmount { get; set; }

        /// <summary>
        /// 授權時間
        /// </summary>
        public DateTime AuthTime { get; set; }

        /// <summary>
        /// 回應碼
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 回應訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 購買商品名
        /// </summary>
        public string Od_sob { get; set; }

        /// <summary>
        /// 購買人
        /// </summary>
        public string Pur_name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 消費者備註
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 店到店地址
        /// </summary>
        public string Logistics_store { get; set; }

        /// <summary>
        /// 金流訂單號
        /// </summary>
        public string AcquirerOrderNo { get; set; }

        /// <summary>
        /// 授權碼
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// 交易時間
        /// </summary>
        public DateTime TransTime { get; set; }

        /// <summary>
        /// 商家訂單號
        /// </summary>
        [Required]
        public string OrderNo { get; set; }

        /// <summary>
        /// 信用卡後4碼
        /// </summary>
        public string LastPan { get; set; }

        /// <summary>
        /// 發票號碼
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// 發票日期
        /// </summary>
        public string InvoiceDate { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string Classif { get; set; }

        /// <summary>
        /// 付款子方式
        /// </summary>
        public string Classif_sub { get; set; }

        /// <summary>
        /// 授權碼
        /// </summary>
        public string Auth_code { get; set; }

        /// <summary>
        /// 虛擬帳號或超商代碼
        /// </summary>
        public string Payment_no { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string ValidateToken { get; set; }

        /// <summary>
        /// 編碼
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// 傳送方式
        /// </summary>
        public string SendType { get; set; }


        public string json { get; set; }
    }

    
}