using WebShopping.Connection;
using WebShopping.Helpers;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Configuration;
using System.IO;
using WebShopping.Dtos;
using AutoMapper;
using Newtonsoft.Json;
using static WebShopping.Dtos.EditorDto;

namespace WebShopping.Services
{
    public class PaymentService : IPaymentService
    {
        IDapperHelper m_DapperHelper;


        private IMapper m_Mapper;

        public PaymentService(IDapperHelper dapper, IMapper mapper)
        {
            m_DapperHelper = dapper;
            m_Mapper = mapper;
        }

        /// <summary>
        /// 金流回傳虛擬帳號或超商代碼
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int UpdateReceive(PaymentReceiveDto dto) {
            //11	待付款-買家尚未付款
            int order_status_id = 11;

            string _sql = $@"UPDATE [ORDERS] SET 
                bank_no=@AtmBankNo
                ,Pay_zg=@Pay_zg
                ,payment_no=ISNULL(@AtmNo,'')+ISNULL(@CvsNo,'')
                ,pay_end_date=@PayEndDate,update_date=GETDATE()
                WHERE ID=@OrderNo

                Declare @member_id uniqueidentifier
                Select @member_id=member_id From [ORDERS] WHERE ID=@OrderNo
                INSERT INTO [transaction](member_id,order_id,transaction_status_id,creation_date,content)
                VALUES(@member_id,@OrderNo,{order_status_id},getdate(),@json)
            ";
            int i = m_DapperHelper.ExecuteSql(_sql, dto);
            return i;
        }

        /// <summary>
        /// 金流回覆付款成功
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int UpdateReturn(PaymentReturnDto dto)
        {
            //13  待付款-付款失敗
            //16  已付款-買家成功付款
            //21  待出貨-未出貨
            int order_status_id = 13;
            if (dto.Code == "000") order_status_id = 21;//16

            string _sql = $@"UPDATE [ORDERS] SET 
                order_status_id={order_status_id},auth_total=@AuthAmount,pay_date=@AuthTime,update_date=GETDATE()
                ,delivery_date='{DateTime.Today.AddDays(3).ToString("yyyy/MM/dd")}'
                ,arrival_date='{DateTime.Today.AddDays(4).ToString("yyyy/MM/dd")}'
                WHERE ID=@OrderNo

                Declare @member_id uniqueidentifier
                Select @member_id=member_id From [ORDERS] WHERE ID=@OrderNo
                INSERT INTO [transaction](member_id,order_id,transaction_status_id,creation_date,content)
                VALUES(@member_id,@OrderNo,{order_status_id},getdate(),@json)
            ";
            //SystemFunctions.WriteLogFile($"sql={_sql};OrderNo={dto.OrderNo}");
            int i = m_DapperHelper.ExecuteSql(_sql, dto);
            return i;
        }

    }
}