using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class DonateService : IDonateService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;

        public DonateService(IDapperHelper iDapperHelper)
        {
            _IDapperHelper = iDapperHelper;
        }
        #endregion

        #region  新增實作
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="_request"></param>
        /// <returns></returns>
        public Donate Insert_Donate(HttpRequest _request)
        {
            Donate _Donate = InitFormdata(_request);
            string _sql = @"INSERT INTO [Donate]
                                       ([OrderId]
                                       ,[donateType]
                                       ,[BuyerType]
                                       ,[BuyerName]
                                       ,[buyerSex]
                                       ,[affiliatedArea]
                                       ,[BuyerId]
                                       ,[BuyerPhone]
                                       ,[mobilePhone]
                                       ,[BuyerEmail]
                                       ,[address1]
                                       ,[address2]
                                       ,[address3]
                                       ,[PayType]
                                       ,[Amount]
                                       ,[NeedReceipt]
                                       ,[ReceiptTitle]
                                       ,[ReceiptPostMethod]
                                       ,[NeedAnonymous]
                                       ,[Remark]
                                       ,[PayStatus]
                                       ,[DonateStatus])
                                 VALUES
                                       (@OrderId,
                                        @donateType,
                                        @BuyerType,
                                        @BuyerName,
                                        @buyerSex,
                                        @affiliatedArea,
                                        @BuyerId,
                                        @BuyerPhone,
                                        @mobilePhone,
                                        @BuyerEmail,
                                        @address1,
                                        @address2,
                                        @address3,
                                        @PayType,
                                        @Amount,
                                        @NeedReceipt,
                                        @ReceiptTitle,
                                        @ReceiptPostMethod,
                                        @NeedAnonymous,
                                        @Remark,
                                        @PayStatus,
                                        @DonateStatus)";
            _IDapperHelper.ExecuteSql(_sql, _Donate);
            //將訂單號碼傳給DonateRelatedItemRecord訂購明細記錄
            _Donate.DonateRelatedItemRecord.ForEach(x => x.OrderId = _Donate.OrderId);
            _sql = @"INSERT INTO [DonateRelatedItemRecord]
                                ([OrderId]
                                ,[Title]
                                ,[Amount]
                                ,[Qty])
                            VALUES
                                (@OrderId,
                                 @Title,
                                 @Amount,
                                 @Qty)";
            _IDapperHelper.ExecuteSql(_sql, _Donate.DonateRelatedItemRecord);

            return _Donate;
        }
        /// <summary>
        /// 接收捐款基本資料與訂購資料
        /// </summary>
        /// <param name="_request"></param>
        /// <returns></returns>
        private Donate InitFormdata(HttpRequest _request)
        {
            Donate _Donate = new Donate();
            DateTime now = DateTime.Now;
            string OrderId = $"'{now.ToString("yyyyMMdd")}%'";
            string _sql = $"SELECT MAX([OrderId]) AS OrderId FROM [Donate] Where [OrderId] Like {OrderId}";
            _Donate = _IDapperHelper.QuerySqlFirstOrDefault<Donate>(_sql);

            // 1.取得訂單編號20211014002, 第一筆
            if (_Donate.OrderId == null)
            {
                _Donate.OrderId = $"{now.ToString("yyyyMMdd")}001";
            }
            else
            {
                // newid 20211014002 取後3碼加1  002+1
                int newId = Convert.ToInt32(_Donate.OrderId.ToString().Substring(_Donate.OrderId.ToString().Length - 3)) + 1;
                //newId 002+1會變2+1=3，0不見，底下PadLeft指定長度3，先靠右對齊，不足補0
                _Donate.OrderId = $"{now.ToString("yyyyMMdd")}{newId.ToString().PadLeft(3,'0')}";
            }

            //2.捐款方式 一般捐款:1 結緣捐贈:2 
            _Donate.donateType = Convert.ToByte(_request.Form.Get("donateType"));

            //3.捐款單位 0無 1個人2公司(非必填)
            _Donate.BuyerType = Convert.ToByte(_request.Form.Get("buyerType"));

            //4.捐款姓名（公司）
            _Donate.BuyerName = string.IsNullOrWhiteSpace(_request.Form.Get("buyerName")) ? null : _request.Form.Get("buyerName");

            //5.姓別 未填時預設在0其它，1男2女
            _Donate.buyerSex = Convert.ToByte(_request.Form.Get("buyerSex"));

            //6.隸屬地區 0無1北部2中部3南部4海外
            _Donate.affiliatedArea = Convert.ToByte(_request.Form.Get("affiliatedArea"));

            //7.身分證號（統編）(非必填)
            _Donate.BuyerId = string.IsNullOrWhiteSpace(_request.Form.Get("buyerId")) ? null : _request.Form.Get("buyerId");
 
            //8.聯絡電話
            _Donate.BuyerPhone = string.IsNullOrWhiteSpace(_request.Form.Get("buyerPhone")) ? null : _request.Form.Get("buyerPhone");

            //9.手機電話(非必填)
            _Donate.mobilePhone = string.IsNullOrWhiteSpace(_request.Form.Get("mobilePhone")) ? null : _request.Form.Get("mobilePhone");

            //10.電子信箱
            _Donate.BuyerEmail = string.IsNullOrWhiteSpace(_request.Form.Get("buyerEmail")) ? null : _request.Form.Get("buyerEmail");

            //11.聯絡地址
            _Donate.address1 = string.IsNullOrWhiteSpace(_request.Form.Get("address1")) ? null : _request.Form.Get("address1");

            //12.戶籍地址(非必填)
            _Donate.address2 = string.IsNullOrWhiteSpace(_request.Form.Get("address2")) ? null : _request.Form.Get("address2");

            //13.寄件地址(非必填)
            _Donate.address3 = string.IsNullOrWhiteSpace(_request.Form.Get("address3")) ? null : _request.Form.Get("address3");

            //==========付款資料==========
            //14.付款方式"01線上刷卡", "2虛擬帳號", "4 7-11(超商代碼)","6 FamilyMart(超商代碼)","9 OK 超商(超商代碼)","10萊爾富(超商代碼)","83銀行匯款" 
            _Donate.PayType = string.IsNullOrWhiteSpace(_request.Form.Get("payType")) ? null : _request.Form.Get("payType");
            if (!string.IsNullOrWhiteSpace(_Donate.PayType))
            {
                _sql = @" SELECT [Name] FROM [DonatePayType] Where [Id] = @PayType";
                //TReturn QuerySqlFirstOrDefault<進入類別T1, 出去類別TReturn>(string sql, T1 paramData);
                _Donate.DonatePayType = _IDapperHelper.QuerySqlFirstOrDefault<Donate, DonatePayType>(_sql, _Donate);
            }

            //15.訂單日期
            _Donate.OrderDate = DateTime.Now;
            //16.總金額
            _Donate.Amount = Convert.ToInt32(_request.Form.Get("amount"));

            //==========收據區==========
            //17.是否開收據 1是0否預設0
            //if ("Y" == _request.Form.Get("NeedReceipt"))
            //{ _Donate.NeedReceipt = "Y"; }
            //else
            //{ _Donate.NeedReceipt = "N"; }
            _Donate.NeedReceipt = Convert.ToBoolean(Convert.ToByte(_request.Form.Get("needReceipt")));

            //18.收據抬頭
            _Donate.ReceiptTitle = _request.Form.Get("receiptTitle");

            //19.是否寄送  1是0否預設0
            //if ("Y" == _request.Form.Get("ReceiptPostMethod"))
            //{ _Donate.ReceiptPostMethod = "Y"; }
            //else
            //{ _Donate.ReceiptPostMethod = "N"; }
            _Donate.ReceiptPostMethod = Convert.ToBoolean(Convert.ToByte(_request.Form.Get("receiptPostMethod")));

            //20.是否開公開捐款姓名及金額 1是0否預設0
            //if ("Y" == _request.Form.Get("NeedAnonymous"))
            //{ _Donate.NeedAnonymous = "Y"; }
            //else
            //{ _Donate.NeedAnonymous = "N"; }
            _Donate.NeedAnonymous = Convert.ToBoolean(Convert.ToByte(_request.Form.Get("needAnonymous")));

            //21.備註
            _Donate.Remark = string.IsNullOrWhiteSpace(_request.Form.Get("remark")) ? null : _request.Form.Get("remark");

            //訂購明細記錄
            string _DonateRelatedItemRecord = string.IsNullOrWhiteSpace(_request.Form.Get("donateRelatedItemRecord")) ? string.Empty : _request.Form.Get("donateRelatedItemRecord");
            //DeserializeObject反序列 將接收進來JSON格式轉換成類別物件public List<DonateRelatedItemRecord> DonateRelatedItemRecord
            _Donate.DonateRelatedItemRecord = JsonConvert.DeserializeObject<List<DonateRelatedItemRecord>>(_DonateRelatedItemRecord);

            //32.繳款狀態 1待付款 2已付款 3已逾期 4失敗
            _Donate.PayStatus = 1;

            // 33.狀態 1未結案2結案
            _Donate.DonateStatus = 1;

            //SystemFunctions.WriteLogFile($"\n\n{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}收到Form-data\n{SystemFunctions.GetFormData(_request)}");
            string JSON = JsonConvert.SerializeObject(_Donate);  //類別序列化成JSON

            return _Donate;
        }
        #endregion

        #region 取得一筆資料
        public Donate Get_Donate(string OrderId)
        {
            Donate _Donate = new Donate()  //設定捐款主表資料類別空盒子
            { 
                OrderId = OrderId  //把輸入的id傳給類別OrderId屬性
            };
            string _sql = @"SELECT  [OrderId]
                                                      ,[donateType]
                                                      ,[BuyerType]
                                                      ,[BuyerName]
                                                      ,[buyerSex]
                                                      ,[affiliatedArea]
                                                      ,[BuyerId]
                                                      ,[BuyerPhone]
                                                      ,[mobilePhone]
                                                      ,[BuyerEmail]
                                                      ,[address1]
                                                      ,[address2]
                                                      ,[address3]
                                                      ,[PayType]
                                                      ,[OrderDate]
                                                      ,[Amount]
                                                      ,[NeedReceipt]
                                                      ,[ReceiptTitle]
                                                      ,[ReceiptPostMethod]
                                                      ,[NeedAnonymous]
                                                      ,[Remark]
                                                      ,[UpdateDate]
                                                      ,[Code]
                                                      ,[OrderNo]
                                                      ,[AcquirerOrderNo]
                                                      ,[AtmBankNo]
                                                      ,[AtmNo_CvsNo]
                                                      ,[PayEndDate]
                                                      ,[Pay_zg]
                                                      ,[AuthAmount]
                                                      ,[AuthTime]
                                                      ,[PayStatus]
                                                      ,[DonateStatus]
                                                    FROM [Donate]
                                                    Where [OrderId] = @OrderId";
            _Donate = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _Donate);

            if (_Donate != null)
            {
                _sql = @" SELECT [Name] FROM [DonatePayType] Where [Id] = @PayType";
                //TReturn QuerySqlFirstOrDefault<進入類別T1, 出去類別TReturn>(string sql, T1 paramData);
                _Donate.DonatePayType = _IDapperHelper.QuerySqlFirstOrDefault<Donate, DonatePayType>(_sql, _Donate);

                _sql = @" SELECT  [Id], [OrderId], [Title], [Amount], [Qty] FROM [DonateRelatedItemRecord] Where [OrderId] = @OrderId";
                //QuerySetSql<左進給的查的類別, 右出要顯示的資料> T1 paramData傳入左進的類別，會進動對應_Donate.OrderId
                _Donate.DonateRelatedItemRecord = _IDapperHelper.QuerySetSql<Donate, DonateRelatedItemRecord>(_sql, _Donate).ToList();
            }

            return _Donate;

        }
        #endregion

        #region 取得捐款明細項目(後端)
        public List<Donate> Get_Donate_ALL(int? _count, int? _page, string _OrderId, int? _donateType, string _BuyerName, string _PayType, DateTime? _StartDate, DateTime? _EndDate, int? _PayStatus)
        {
            string page_sql = string.Empty;  //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string search_sql = string.Empty;
            if (_count != null && _count > 0 && _page != null && _page > 0)
            {
                int startRowjumpover = 0;  //預設跳過筆數
                startRowjumpover = Convert.ToInt32((_page - 1) * _count);  // <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {_count}  ROWS ONLY ";
            }

            if (!string.IsNullOrWhiteSpace(_OrderId))
            {
                search_sql += $" And OrderId like '%{_OrderId}%' ";
            }
            if (_donateType != null & _donateType > 0)
            {
                search_sql += $" And donateType = {_donateType} ";
            }
            if (!string.IsNullOrWhiteSpace(_BuyerName))
            {
                search_sql += $" And BuyerName like '%{_BuyerName}%' ";
            }
            if (!string.IsNullOrWhiteSpace(_PayType))
            {
                search_sql += $" And PayType = '{_PayType}' ";
            }
            if (_StartDate != null && _StartDate > DateTime.MinValue)
            {
                search_sql += $" And OrderDate >= '{Convert.ToDateTime(_StartDate).ToString("yyyy-MM-dd HH:mm:ss.fff")}' ";
            }
            if (_EndDate != null && _EndDate > DateTime.MinValue)
            {
                search_sql += $" And OrderDate < '{Convert.ToDateTime(_EndDate).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss.fff")}' ";  //時間從上午12:00 = 00:00 需加一天才是這一天結束
            }
            if (_PayStatus != null & _PayStatus > 0)
            {
                search_sql += $" And PayStatus = {_PayStatus} ";
            }
            if (!string.IsNullOrWhiteSpace(search_sql))
            {
                search_sql = @"where 1 = 1" + search_sql;
            }

            string _sql = $@"SELECT * FROM [Donate]
                                    {search_sql}
                                    order by OrderId Desc
                                    {page_sql}  ";
            List<Donate> _Donates = _IDapperHelper.QuerySetSql<Donate>(_sql).ToList();

            //Donate.DonateRelatedItemRecord
            //導入所有 捐款訂單 的 訂購明細記錄
            if (_Donates.Count > 0)  //檢查有無訂單
            {
                foreach (Donate _Donate in _Donates)  //(單筆訂單類別 _單筆訂單 in 多筆訂單)
                {
                    _sql = @" SELECT [Name] FROM [DonatePayType] Where [Id] = @PayType";
                    //TReturn QuerySqlFirstOrDefault<進入類別T1, 出去類別TReturn>(string sql, T1 paramData);
                    _Donate.DonatePayType = _IDapperHelper.QuerySqlFirstOrDefault<Donate, DonatePayType>(_sql, _Donate);

                    _sql = @"SELECT [Id], [OrderId], [Title], [Amount], [Qty] FROM [DonateRelatedItemRecord] Where [OrderId] = @OrderId";
                    //單筆訂單.訂購明細記錄 = _介面.Dapper多筆查詢<輸入訂單類別, 輸出查詢訂購明細記錄>(_sql, _輸入單筆訂單類別.自動辨斷用OrderId查).多筆清單();
                    _Donate.DonateRelatedItemRecord = _IDapperHelper.QuerySetSql<Donate, DonateRelatedItemRecord>(_sql, _Donate).ToList();
                 }                
            }

            return _Donates;
        }
        #endregion

        #region 取得捐款明細項目(前端)
        public List<Donate> Get_Donations_ALL(string BuyerName, int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }

            string search_sql = string.Empty;
            if (!string.IsNullOrWhiteSpace(BuyerName))
            {
                search_sql += $" And BuyerName like '%{BuyerName}%' ";
            }

            string _sql = $@"SELECT BuyerName, affiliatedArea, donateType, OrderDate, Amount FROM [Donate]
                                    where NeedAnonymous = '1'
                                    {search_sql}
                                    order by OrderDate Desc 
                                    {page_sql}  ";
            List<Donate> _Donates = _IDapperHelper.QuerySetSql<Donate>(_sql).ToList();

            return _Donates;
        }
        #endregion

        #region 更新捐款狀態為已結案
        public void UpdateDonateStatus(Donate donate)
        {
            int _DonateStatus = 2;  //2結案
            string _sql = $@"UPDATE [Donate]
                                       SET [DonateStatus] = {_DonateStatus}
                                       Where [OrderId] = @OrderId ";
            //執行更新
            int result = _IDapperHelper.ExecuteSql(_sql, donate);
        }
        #endregion

        #region 更新繳款狀態為已付款(83銀行匯款)
        public void UpdateBankRemittanceStatus(Donate donate)
        {
            int _PayStatus = 2;  //2已付款
            string _sql = $@"UPDATE [Donate]
                                       SET [PayStatus] = {_PayStatus}
                                       Where [OrderId] = @OrderId ";
            //執行更新
            int result = _IDapperHelper.ExecuteSql(_sql, donate);
        }
        #endregion

        #region 更新繳款狀態為已付款(2虛擬帳號, 7超商代碼)
        public void UpdateVirtualAtmSuperMarketStatus(Donate donate)
        {
            int _PayStatus = 2;  //2已付款
            string _sql = $@"UPDATE [Donate]
                                       SET [PayStatus] = {_PayStatus}
                                       Where [OrderId] = @OrderId ";
            //執行更新
            int result = _IDapperHelper.ExecuteSql(_sql, donate);
        }
        #endregion


        #region 取得要更新繳款狀態為已付款(2虛擬帳號, 7超商代碼)要未過期
        public Donate Get_Donate_UpdateVirtualAtmSuperMarketStatus(string _OrderId)
        {
            string search_sql = string.Empty;
            DateTime now = DateTime.Now;

            //繳費期限要 大於 現在時間才沒有過期
            search_sql = $" And PayEndDate > '{now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
            //search_sql += $" And PayEndDate < '{Convert.ToDateTime(now).ToString("yyyy-MM-dd HH:mm:ss")}' ";

            string _sql = $@"SELECT * FROM [Donate] Where [OrderId] = '{_OrderId}' " +
                                   $" {search_sql} ";

            Donate _Donate = _IDapperHelper.QuerySqlFirstOrDefault<Donate>(_sql);

            return _Donate;
        }
        #endregion
    }
}