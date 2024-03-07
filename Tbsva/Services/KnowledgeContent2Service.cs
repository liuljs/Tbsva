using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class Knowledge2Service : IKnowledge2Service
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dapperHelper"></param>
        /// <param name="imageFileHelper"></param>
        public Knowledge2Service(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            _IDapperHelper = dapperHelper;
            m_ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="_request">用戶端送來的表單資訊轉入類別資料</param>
        /// <returns></returns>
        public Knowledge_content2 Insert_Knowledge2(HttpRequest _request)
        {
            Knowledge_content2 _knowledge_Content2 = Request_data(_request);  //將接收來的參數轉進_knowledge_Content型別
            string _sql = @"INSERT INTO [KNOWLEDGE_CONTENT2]
                                                            ([KNOWLEDGETID]
                                                            ,[TITLE]
                                                            ,[CATEGORY]
                                                            ,[NUMBER]
                                                            ,[BRIEF]
                                                            ,[CONTENT]
                                                            ,[FIRST]
                                                            ,[SORT]
                                                            ,[ENABLED]
                                                            ,[CREATIONDATE])
                                                        VALUES
                                                            ( @KNOWLEDGETID,
                                                               @TITLE,
                                                              @CATEGORY,
                                                              @NUMBER,
                                                              @BRIEF,
                                                              @CONTENT,
                                                              @FIRST,
                                                              @SORT,
                                                              @ENABLED,
                                                              @CREATIONDATE )  
                                                            SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。

            int id = _IDapperHelper.QuerySingle(_sql, _knowledge_Content2);       //新增資料
            _knowledge_Content2.id = id;

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(_request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                _knowledge_Content2.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                _knowledge_Content2.sort = Convert.ToInt32(_request.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [KNOWLEDGE_CONTENT2] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            _IDapperHelper.ExecuteSql(_sql, _knowledge_Content2);

            return _knowledge_Content2;
        }

        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="_request">讀取表單資料</param>
        /// <returns>回傳表單類別_Knowledge_Content</returns>
        private Knowledge_content2 Request_data(HttpRequest _request)
        {
            Knowledge_content2 _knowledge_Content2 = new Knowledge_content2();
            _knowledge_Content2.knowledgetId = Guid.NewGuid();                                                      //內容的id
            _knowledge_Content2.title = _request.Form["title"];                                                                //標題
            _knowledge_Content2.category = _request.Form["category"];                                                //位階
            _knowledge_Content2.number = _request.Form["number"];                                                //戒牒號碼
            _knowledge_Content2.brief = _request.Form["brief"];                                                            //簡述
            _knowledge_Content2.content = _request.Form["content"];                                                   //內容
            _knowledge_Content2.first = Convert.ToBoolean(Convert.ToByte(_request.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            _knowledge_Content2.sort = Convert.ToInt32(_request.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            _knowledge_Content2.enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            _knowledge_Content2.creationDate = DateTime.Now;

            return _knowledge_Content2;
        }
        #endregion

  
        #region 取得一筆資料
        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="knowledgetId">輸入內容knowledgetId編號</param>
        /// <returns>_knowledge_Content</returns>
        public Knowledge_content2 Get_Knowledge2(Guid knowledgetId)
        {
            Knowledge_content2 _knowledge_Content2 = new Knowledge_content2();
            _knowledge_Content2.knowledgetId = knowledgetId;

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 ";
            string _sql = $"SELECT * FROM [KNOWLEDGE_CONTENT2] WHERE [KNOWLEDGETID]=@KNOWLEDGETID {adminQuery} ";

            _knowledge_Content2 = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _knowledge_Content2);

            return _knowledge_Content2;
        }
        #endregion


        #region 更新一筆資料
        /// <summary>
        /// 更新一筆資料
        /// </summary>
        /// <param name="_request">用戶端的要求資訊</param>
        /// <param name="_Knowledge_content2">更新的資料類別</param>
        public void Update_Knowledge2(HttpRequest _request, Knowledge_content2 _Knowledge_content2)
        {
            _Knowledge_content2 = Request_data_mod(_request, _Knowledge_content2);

            //2.處理資料庫更新資料
            string _sql = @"UPDATE [KNOWLEDGE_CONTENT2]
                                        SET 
                                             [TITLE] = @TITLE
                                            ,[CATEGORY] = @CATEGORY
                                            ,[NUMBER] = @NUMBER
                                            ,[BRIEF] = @BRIEF
                                            ,[CONTENT] = @CONTENT
                                            ,[FIRST] = @FIRST
                                            ,[SORT] = @SORT
                                            ,[ENABLED] = @ENABLED
                                            ,[UPDATEDDATE] = @UPDATEDDATE
                                        WHERE [KNOWLEDGETID] = @KNOWLEDGETID";
            _IDapperHelper.ExecuteSql(_sql, _Knowledge_content2);
        }
         /// <summary>
        /// 讀取表單資料,轉到_knowledge_Content2
        /// </summary>
        /// <param name="_request">讀取表單資料(修改時用)</param>
        /// <param name="_knowledge_Content2">控制器取到資料</param>
        /// <returns></returns>
        public Knowledge_content2 Request_data_mod(HttpRequest _request, Knowledge_content2 _knowledge_Content2)
        {
            _knowledge_Content2.title = _request.Form["title"];
            _knowledge_Content2.category = _request.Form["category"];                                                                        //位階
            _knowledge_Content2.number = _request.Form["number"];                                                                        //戒牒號碼
            _knowledge_Content2.brief = _request.Form["brief"];                                                                                   //簡述
            _knowledge_Content2.content = _request.Form["content"];                                                                        //內容
            _knowledge_Content2.first = Convert.ToBoolean(Convert.ToByte(_request.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            _knowledge_Content2.sort = Convert.ToInt32(_request.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            _knowledge_Content2.enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            _knowledge_Content2.updatedDate = DateTime.Now;

            return _knowledge_Content2;
        }
        #endregion


        #region  刪除內容
        /// <summary>
        /// 刪除內容
        /// </summary>
        /// <param name="_Knowledge_content2"></param>
        public void Delete_Knowledge2(Knowledge_content2 _Knowledge_content2)
        {
            //刪除資料庫內容
            string _sql = @"DELETE FROM [KNOWLEDGE_CONTENT2] WHERE [KNOWLEDGETID] = @KNOWLEDGETID";

            _IDapperHelper.ExecuteSql(_sql, _Knowledge_content2);
        }
        #endregion


        #region  讀取全部內容
        public List<Knowledge_content2> Get_Knowledge2_ALL(int? _count, int? _page, string _category)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string search_sql = string.Empty;

            if (_count != null && _count > 0 && _page != null && _page > 0)
            {
                int startRowjumpover = 0;  //預設跳過筆數
                startRowjumpover = Convert.ToInt32((_page - 1) * _count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {_count}  ROWS ONLY ";
            }
            if (!string.IsNullOrWhiteSpace(_category))
            {
                search_sql += $" And category = '{_category}' ";
            }
            string adminQuery = Auth.Role.IsAdmin ? " where 1=1 " : " WHERE ENABLED = 1 ";   //登入取得所有資料:未登入只能取得上線資料
            string _sql = $"SELECT * FROM KNOWLEDGE_CONTENT2 {adminQuery} " +
                                  $" {search_sql} " +
                                  @" ORDER BY [FIRST] DESC , SORT , CREATIONDATE DESC " +
                                  $"{page_sql}";
            List<Knowledge_content2> _knowledge_Content2s = _IDapperHelper.QuerySetSql<Knowledge_content2>(_sql).ToList();
           
            return _knowledge_Content2s;
        }
        #endregion
    }
}