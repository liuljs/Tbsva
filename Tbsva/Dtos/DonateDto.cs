using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class DonateDto
    {
        ////1.訂單編號
        public string orderId { get; set; }
        ////2.捐款方式
        public byte donateType { get; set; }
        #region 基本資料
        ////3.捐款單位
        public byte buyerType { get; set; }
        ////4.捐款姓名（公司）
        public string buyerName { get; set; }
        //// 5. 0其它，1男 2女
        public byte buyerSex { get; set; }
        ////6.隸屬地區
        public byte affiliatedArea { get; set; }
        ////7.身分證號（統編）
        public string buyerId { get; set; }
        ////8.聯絡電話
        public string buyerPhone { get; set; }
        ////9.手機電話
        public string mobilePhone { get; set; }
        ////10.電子信箱
        public string buyerEmail { get; set; }
        ////11.必填(聯絡地址)
        public string address1 { get; set; }
        ////12.非必填(戶籍地址)
        public string address2 { get; set; }
        ////13.非必填(寄件地址)
        public string address3 { get; set; }
        #endregion

        #region 付款資料
        ////14.付款方式01線上刷卡2虛擬帳號7超商代碼13LINE Pay83銀行匯款
        public string payType { get; set; }
        ////付款方式代出Name
        public DonatePayTypeDto payTypeName { get; set; }
        ////15.訂單日期
        public string orderDate { get; set; }

        ////16.總金額
        public int amount { get; set; }
        #endregion

        /// <summary>
        /// 前端RelatedItem訂購明細項目表格區
        /// </summary>
        #region 收據區
        ////17.是否開收據,預設0不開
        public byte needReceipt { get; set; }
        ////18.收據抬頭
        public string receiptTitle { get; set; }
        ////19.是否寄送預設0
        public byte receiptPostMethod { get; set; }
        ////20.是否開公開捐款人在前台 預設0
        public byte needAnonymous { get; set; }
        ////21.備註
        public string remark { get; set; }
        ////22.金流回覆時更新
        public string updateDate { get; set; }
        #endregion

        #region 金流共同回傳
        ////23.回應碼Code == "000"成功
        public string Code { get; set; }
        ////24.回OrderId訂單編號, 商家自訂訂單編號
        public string OrderNo { get; set; }
        ////25.Payware 訂單編號
        public string AcquirerOrderNo { get; set; }
        #endregion

        #region 金流回傳欄(虛擬帳號或超商代碼) Code == "000"成功
        //// 26.銀行代碼888
        public string AtmBankNo { get; set; }
        //// 27.交易號碼AtmNo=953071278000**81, CvsNo PW109280000749
        public string AtmNo_CvsNo { get; set; }
        //// 28.繳費期限
        public DateTime? PayEndDate { get; set; }
        //// 29.付款方式, 金流回傳的付款方式
        public string Pay_zg { get; set; }
        #endregion
        #region 信用卡Code == "000"成功
        //// 30.授權金額
        public decimal? AuthAmount { get; set; }
        //// 31.授權時間
        public DateTime? AuthTime { get; set; }
        //// 32.繳款狀態預設1 1待付款2已付款
        public byte? PayStatus { get; set; }
        //// 33.狀態預設1 1未結案2已結案
        public byte? DonateStatus { get; set; }
        #endregion

        //AutoMapper 轉型子類屬性欄位名稱DonateRelatedItemRecord要和被轉型Donate.cs一樣才轉的過去
        public List<DonateRelatedItemRecordDto> DonateRelatedItemRecord { get; set; }
    }

    /// <summary>
    /// 取得捐款明細項目(前端)/Donate/Donations/
    /// </summary>
    public class DonationsDto
    {
        ////4.捐款姓名（公司）
        public string buyerName { get; set; }
        ////6.隸屬地區
        public byte affiliatedArea { get; set; }
        ////2.捐款方式
        public byte donateType { get; set; }
        ////15.訂單日期
        public string orderDate { get; set; }
        ////16.總金額
        public int amount { get; set; }
    }

}