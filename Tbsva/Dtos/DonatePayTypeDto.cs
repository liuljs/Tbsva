using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    public class DonatePayTypeDto
    {
        //public string Id { get; set; }
        /// <summary>
        /// 付款方式
        ///  01：信用卡、2：虛擬帳號、7：超商代碼  (4|5)：7-11Ibon、
        ///  6：FamiPort、9：OK 超商、10：LifeET 12：支付寶、13：LINE Pay、14：微信支付 
        ///  19：ApplePay、20：GooglePay。 
        /// </summary>
        public string name { get; set; }
    }
}