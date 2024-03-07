using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Models
{
    public class DonatePayType
    {
        /// <summary>
        /// 付款方式01線上刷卡  2虛擬帳號 47-11(超商代碼) 6FamilyMart(超商代碼) 9OK 超商(超商代碼)
        /// 10萊爾富(超商代碼) 83銀行匯款
        /// </summary>
        [StringLength(2)]
        public string Id { get; set; }

        /// <summary>
        /// 付款方式
        ///  01：信用卡、2：虛擬帳號、7：超商代碼  (4|5)：7-11Ibon、
        ///  6：FamiPort、9：OK 超商、10：LifeET 12：支付寶、13：LINE Pay、14：微信支付 
        ///  19：ApplePay、20：GooglePay。 
        /// </summary>
        [StringLength(20)]
        public string Name { get; set; }
    }
}