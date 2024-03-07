using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    /// <summary>
    /// 修改採購資訊
    /// </summary>
    public class PurchaseDto
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 會員編號
        /// </summary>
        [Required]
        public Guid member_id { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        public Int16 order_status_id { get; set; }

        /// <summary>
        /// 訂單總金額
        /// </summary>
        public decimal order_total { get; set; }

        /// <summary>
        /// 運費
        /// </summary>
        public decimal delivery_fee { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public Int16 pay_type_id { get; set; }

        /// <summary>
        /// 寄送方式
        /// </summary>
        public Int16 delivery_type_id { get; set; }

        /// <summary>
        /// 配送時間
        /// </summary>
        public string delivery_time { get; set; }

        /// <summary>
        /// 獲得點數
        /// </summary>
        public int point { get; set; }

        /// <summary>
        /// 商家備註
        /// </summary>
        [MaxLength(500)]
        public string memo_store { get; set; }

        /// <summary>
        /// 消費者備註
        /// </summary>
        [MaxLength(500)]
        public string memo_customer { get; set; }

        /// <summary>
        /// 銀行代碼
        /// </summary>
        public string bank_no { get; set; }

        /// <summary>
        /// 繳費代碼
        /// </summary>
        public string payment_no { get; set; }

        /// <summary>
        /// 是否刪除
        /// </summary>
        public byte deleted { get; set; }

        /// <summary>
        /// 是否取消
        /// </summary>
        public byte is_cancel { get; set; }

        /// <summary>
        /// 付款截止日
        /// </summary>
        public DateTime? pay_end_date { get; set; }

        /// <summary>
        /// 退款資訊
        /// </summary>
        public string refund { get; set; }

        /// <summary>
        /// 退款金額
        /// </summary>
        public decimal refund_amount { get; set; }

        /// <summary>
        /// 購買人姓名
        /// </summary>
        public string purchaser_name { get; set; }

        /// <summary>
        /// 購買人性別
        /// </summary>
        public string purchaser_sex { get; set; }

        /// <summary>
        /// 購買人電話
        /// </summary>
        public string purchaser_phone { get; set; }

        /// <summary>
        /// 購買人市話
        /// </summary>
        public string purchaser_tel { get; set; }

        /// <summary>
        /// 購買人地址
        /// </summary>
        public string purchaser_address { get; set; }

        /// <summary>
        /// 購買人信箱
        /// </summary>
        public string purchaser_email { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string receiver_name { get; set; }

        /// <summary>
        /// 收件人性別
        /// </summary>
        public string receiver_sex { get; set; }

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string receiver_phone { get; set; }

        /// <summary>
        /// 收件人市話
        /// </summary>
        public string receiver_tel { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string receiver_address { get; set; }

        /// <summary>
        /// 收件人信箱
        /// </summary>
        public string receiver_email { get; set; }

        /// <summary>
        /// 發票資訊
        /// </summary>
        public string invoice { get; set; }
        
        /// <summary>
        /// 建立日
        /// </summary>
        public DateTime creation_date { get; set; }

        /// <summary>
        /// 訂購日
        /// </summary>
        public DateTime? purchase_date { get; set; }

        /// <summary>
        /// 最後更新日
        /// </summary>
        public DateTime? update_date { get; set; }
        
        /// <summary>
        /// 送出日
        /// </summary>
        public DateTime? delivery_date { get; set; }

        /// <summary>
        /// 到達日
        /// </summary>
        public DateTime? arrival_date { get; set; }

    }
}