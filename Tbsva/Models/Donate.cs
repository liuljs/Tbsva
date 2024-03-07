using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class Donate
    {
        /// <summary>
        /// 1.訂單編號2021 10+22+011
        /// </summary>
        [Key]
        [StringLength(11)]
        public string OrderId { get; set; }

        /// <summary>
        /// 2.捐款方式 一般捐款:1  結緣捐贈:2
        /// </summary>
        [RegularExpression("[1]|[2]")]
        public byte donateType { get; set; }        

        #region 基本資料
        /// <summary>
        /// 3.捐款單位0無 1個人2公司(非必填)
        /// </summary>
        [RegularExpression("[0]|[1]|[2]")]
        public byte BuyerType { get; set; }

        /// <summary>
        /// 4.捐款姓名（公司）
        /// </summary>
        [StringLength(50)]
        public string BuyerName { get; set; }

        /// <summary>
        /// 5.非必填，預設在0其它，1男2女
        /// </summary>
        //[RegularExpression("[0]|[1]|[2]")]
        public byte buyerSex { get; set; }

        /// <summary>
        /// 6.隸屬地區 0無1北部2中部3南部4海外
        /// </summary>
        public byte affiliatedArea { get; set; }        

        /// <summary>
        /// 7.身分證號（統編）
        /// </summary>
        [StringLength(50)]
        public string BuyerId { get; set; }

        /// <summary>
        /// 8.聯絡電話
        /// </summary>
        [StringLength(20)]
        public string BuyerPhone { get; set; }

        /// <summary>
        /// 9.手機電話
        /// </summary>
        [StringLength(20)]
        public string mobilePhone { get; set; }

        /// <summary>
        /// 10.電子信箱
        /// </summary>
        [StringLength(100)]
        public string BuyerEmail { get; set; }

        /// <summary>
        /// 11.必填(聯絡地址)
        /// </summary>
        [StringLength(250)]
        public string address1 { get; set; }

        /// <summary>
        /// 12.非必填(戶籍地址)
        /// </summary>
        [StringLength(250)]
        public string address2 { get; set; }

        /// <summary>
        /// 13.非必填(寄件地址)
        /// </summary>
        [StringLength(250)]
        public string address3 { get; set; }
        #endregion

        #region 付款資料
        /// <summary>
        /// 14.付款方式01線上刷卡2虛擬帳號7超商代碼13LINE Pay83銀行匯款
        /// </summary>
        [StringLength(2)]
        public string PayType { get; set; }

        ////付款方式代出Name
        public DonatePayType DonatePayType { get; set; }

        /// <summary>
        /// 15.訂單日期
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 16.總金額
        /// </summary>
        public decimal Amount { get; set; }
        #endregion

        /// <summary>
        /// 前端RelatedItem訂購明細項目表格區
        /// </summary>

        #region 收據區
        /// <summary>
        /// 17.是否開收據,預設0不開
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool NeedReceipt { get; set; }

        /// <summary>
        /// 18.收據抬頭
        /// </summary>
        [StringLength(50)]
        public string ReceiptTitle { get; set; }

        /// <summary>
        /// 19.是否寄送預設0
        /// </summary>
        [Required]
        [StringLength(1)]
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool ReceiptPostMethod { get; set; }

        /// <summary>
        /// 20.是否開公開捐款人在前台 預設0
        /// </summary>
        [StringLength(1)]
        [RegularExpression(@"^([0-1]{1})$")] //只能輸入01共1位數
        public bool NeedAnonymous { get; set; }

        /// <summary>
        /// 21.備註
        /// </summary>
        [StringLength(250)]
        public string Remark { get; set; }

        /// <summary>
        /// 22.金流回覆時更新
        /// </summary>
        public DateTime? UpdateDate { get; set; }
        #endregion

        # region 金流共同回傳
        /// <summary>
        /// 23.回應碼Code == "000"成功
        /// </summary>
        [StringLength(3)]
        public string Code { get; set; }

        /// <summary>
        /// 24.回OrderId訂單編號, 商家自訂訂單編號
        /// </summary>
        [StringLength(20)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 25.Payware 訂單編號
        /// </summary>
        [StringLength(20)]
        public string AcquirerOrderNo { get; set; }
        #endregion

        #region 金流回傳欄(虛擬帳號或超商代碼) Code == "000"成功
        /// <summary>
        /// 26.銀行代碼888
        /// </summary>
        [StringLength(10)]
        public string AtmBankNo { get; set; }

        /// <summary>
        /// 27.交易號碼AtmNo=953071278000**81, CvsNo PW109280000749
        /// </summary>
        [StringLength(30)]
        public string AtmNo_CvsNo { get; set; }

        /// <summary>
        /// 28.繳費期限
        /// </summary>
        public DateTime? PayEndDate { get; set; }

        /// <summary>
        /// 29.付款方式, 金流回傳的付款方式
        /// </summary>
        [StringLength(5)]
        public string Pay_zg { get; set; }
        #endregion

        #region 信用卡Code == "000"成功
        /// <summary>
        /// 30.授權金額
        /// </summary>
        public decimal? AuthAmount { get; set; }
        /// <summary>
        /// 31.授權時間
        /// </summary>
        public DateTime? AuthTime { get; set; }
        /// <summary>
        /// 32.繳款狀態預設1 1待付款2已付款
        /// </summary>
        public byte? PayStatus { get; set; }

        /// <summary>
        /// 繳款狀態 1待付款2已付款3已逾期4失敗
        /// </summary>
        public byte? PayStatusVirtual
        {
            get {
                ////第一個條件(只有第一步送出捐款資料時), 未接到金流回傳，所以算失敗
                ////但83銀行匯款(不是虛擬帳號，沒有過期問題)也不會送金流要排除
                ////13：LINE Pay有過期但因為送出後無法回傳只有繳款QRcode畫面
                if ((PayStatus == 1 && (PayType != "83" && PayType != "13") ) && (Code == null || Code != "000"))
                {
                    return 4;
                }
                ////第二個條件2虛擬帳號7超商代碼, PayEndDate繳費期限還未到時算1待付款
                if ( PayStatus == 1 && (PayType == "2" || PayType == "7") && Code == "000" && (PayEndDate > DateTime.Now) )
                {
                    return 1;
                }
                ////第三個條件2虛擬帳號7超商代碼, PayEndDate繳費期限已到時(PayStatus=3已逾期)
                if (PayStatus == 1 && (PayType == "2" || PayType == "7") && Code == "000" && (PayEndDate < DateTime.Now))
                {
                    return 3;
                }
                //條件不成立原判
                return PayStatus;
                // sql
                //,case when(PayStatus = 1 and PayType <> '83') and(Code is NULL or Code <> '000')  then 4
                //  when PayStatus = 1 and(PayType = '2' or PayType = '7') and(Code = '000') and PayEndDate > GETDATE()  then 1
                //  when PayStatus = 1 and(PayType = '2' or PayType = '7') and(Code = '000') and PayEndDate<GETDATE()  then 3
                //  else PayStatus end AS PayStatusVirtual
                //  FROM [Donate]
            }
        }

        /// <summary>
        /// 33.狀態預設1 1未結案2已結案
        /// </summary>
        public byte? DonateStatus { get; set; }
        #endregion

        //public List<DonateRelatedItemRecord> DonateRelatedItemRecord { get; set; } = new List<DonateRelatedItemRecord>();
        /// <summary>
        /// 前端RelatedItem訂購明細項目送出後存入DonateRelatedItemRecord
        /// </summary>
        public List<DonateRelatedItemRecord> DonateRelatedItemRecord { get; set; }

     



    }
}