using WebShopping.Dtos;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Services
{
    public interface IPaymentService
    {
        /// <summary>
        /// 更新金流回傳的虛擬帳號或超商代碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        int UpdateReceive(PaymentReceiveDto dto);
        
        /// <summary>
        /// 更新付款結果
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        int UpdateReturn(PaymentReturnDto dto);
    }
}