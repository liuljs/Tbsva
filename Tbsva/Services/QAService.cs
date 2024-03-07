using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class QAService : IQAService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dapperHelper"></param>
        public QAService(IDapperHelper dapperHelper)
        {
            _IDapperHelper = dapperHelper;
        }
        #endregion

        #region  新增實作
        /// <summary>
        /// 新增QA
        /// </summary>
        /// <param name="_request"></param>
        /// <returns>_qa</returns>
        public QA InsertQA(HttpRequest _request)
        {
            QA _qa = Request_data(_request);          //將接收來的參數轉進型別
            string _sql = @"INSERT INTO [QA]
                                ([Question]
                                ,[Answer]
                                ,[creation_date]
                                ,[Enabled]
                                ,[Sort])
                            VALUES
                                ( @Question,
                                  @Answer,
                                  @creation_date,
                                  @Enabled,
                                  @Sort ) 
                                   select scope_identity()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, _qa);  //產生最新新增的id
            _qa.id = id;
            return _qa;
        }
        /// <summary>
        /// 設置新增QA的資料
        /// </summary>
        /// <param name="_request">用戶端的要求資訊</param>
        /// <returns>回傳表單傳入QA類別</returns>
        private QA Request_data(HttpRequest _request)
        {
            QA _qa = new QA();
            _qa.Question = _request.Form["Question"];                                                                   //常見問題
            _qa.Answer = _request.Form["Answer"];                                                                         //問題回答
            _qa.creation_date = DateTime.Now;                                                                                 //新增時間
            _qa.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["Enabled"]));     //是否啟用(0/1)
            _qa.Sort = Convert.ToInt32(_request.Form["Sort"]);                                                      //排序

            return _qa;
        }
        #endregion

        #region 取得一筆資料
        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="_id">輸入id編號</param>
        /// <returns>回傳一筆類別資料_qa</returns>
        public QA GetQA(int _id)
        {
            QA _qa = new QA();  //建立空類別
            _qa.id = _id;               //將接收的id值先傳給QA類別的id

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND Enabled = 1 ";  //登入無限條件，未登入只顯示上線資料
            string _sql = $"SELECT * FROM [QA] WHERE [id]=@id {adminQuery} ";
            _qa = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _qa);  //如同Lambda的FirstOrDefault()，會將符合條件的第一筆回傳回來，如果沒有符合回傳null。

            return _qa;
        }
        #endregion

        #region 更新一筆資料
        /// <summary>
        /// 更新一筆資料
        /// </summary>
        /// <param name="_request">用戶端的要求修改的資訊</param>
        /// <param name="_qa">先取出要修改的資料</param>
        public void UpdateQA(HttpRequest _request, QA _qa)
        {
            _qa = Request_data_mod(_request, _qa);  //將表單值轉入類別
            //處理資料庫更新資料
            string _sql = @"UPDATE [QA]
                                        SET 
                                            [Question] = @Question
                                            ,[Answer] = @Answer
                                            ,[updated_date] = @updated_date
                                            ,[Enabled] = @Enabled
                                            ,[Sort] = @Sort
                                        WHERE [id] = @id";
            _IDapperHelper.ExecuteSql(_sql, _qa);
        }
                /// <summary>
        /// 讀取表單資料,轉到_qa
        /// </summary>
        /// <param name="_request">讀取表單資料(修改時用)</param>
        /// <param name="_qa">控制器傳進來抓到要修改的資料</param>
        /// <returns>_qa</returns>
        public QA Request_data_mod(HttpRequest _request, QA _qa)
        {
            _qa.Question = _request.Form["Question"];                        //常見問題
            _qa.Answer = _request.Form["Answer"];                              //問題回答
            _qa.updated_date = DateTime.Now;                                     //更新時間
            _qa.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["Enabled"]));     //是否啟用(0/1)
            _qa.Sort = Convert.ToInt32(_request.Form["Sort"]);           //排序
            return _qa;
        }
        #endregion

        #region 刪除
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="_qa">控制器那裏取得的資料</param>
        public void DeleteQA(QA _qa)
        {
            string _sql = @"DELETE FROM [QA] WHERE ID = @ID";
            _IDapperHelper.ExecuteSql(_sql, _qa);
        }
        #endregion

        #region 取得全部資料
        /// <summary>
        /// 取得全部QA
        /// </summary>
        /// <param name="_count">一頁要顯示幾筆</param>
        /// <param name="_page">第幾頁開始</param>
        /// <returns></returns>
        public List<QA> GetQA(int? _count, int? _page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            if (_count != null && _count > 0 && _page != null && _page > 0)
            {
                int startRowjumpover = 0;  //預設跳過筆數
                startRowjumpover = Convert.ToInt32((_page - 1) * _count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {_count}  ROWS ONLY ";
            }
            string adminQuery = Auth.Role.IsAdmin ? "" : " where Enabled = 1";   //登入取得所有資料:未登入只能取得上線資料
            string _sql = $"Select * from QA {adminQuery} " +
                                  @"Order by Sort " +
                                  $"{page_sql}";

             List<QA> _qa = _IDapperHelper.QuerySetSql<QA>(_sql).ToList();

            return _qa;
        }
        #endregion
    }
}