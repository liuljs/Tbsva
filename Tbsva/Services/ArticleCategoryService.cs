using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class ArticleCategoryService : IArticleCategoryService
    {
        #region DI依賴注入功能
        /// <summary>
        ///  DI依賴注入功能
        /// </summary>
        private IDapperHelper _IDapperHelper;
        public ArticleCategoryService(IDapperHelper IDapperHelper)
        {
            _IDapperHelper = IDapperHelper;
        }
        #endregion

        #region  新增文章目錄實作
        /// <summary>
        /// 新增文章目錄實作
        /// </summary>
        /// <param name="_request">用戶端送來的表單資訊，Postman使用Body form-data</param>
        /// <returns>_article_category_1</returns>
        public article_category Insert_article_category(HttpRequest _request)
        {
            article_category _article_category = Request_data(_request);
            string _sql = @"INSERT INTO [article_category]
                                               ([name]
                                               ,[content]
                                               ,[Enabled]
                                               ,[Sort])
                                         VALUES
                                               (@name
                                               ,@content
                                               ,@Enabled
                                               ,@Sort )
                                                select scope_identity()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, _article_category); //需使用QuerySingle，因Execute所傳回是新增成功數值
            _article_category.id = id;
            if (string.IsNullOrWhiteSpace(_request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                _article_category.Sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                _article_category.Sort = Convert.ToInt32(_request.Form["sort"]);       //排序 
            }            
            _sql = @"UPDATE [article_category] 
                                SET [Sort] = @Sort 
                             Where [id] = @id ";
            _IDapperHelper.ExecuteSql(_sql, _article_category);         //更新排序

            //回傳
            return _article_category;
        }
        /// <summary>
        /// 讀取表單資料
        /// </summary>
        /// <param name="_request">讀取表單資料</param>
        /// <returns>回傳表單類別_article_category</returns>
        private article_category Request_data(HttpRequest _request)
        {
            article_category _article_category = new article_category();
            _article_category.name = _request.Form["name"];                                       //目錄名稱
            _article_category.content = _request.Form["content"];                                //目錄內容
            //_article_category.creation_date = DateTime.Now;
            _article_category.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));  //是否啟用(0/1)
            _article_category.Sort = Convert.ToInt32(_request.Form["sort"]);                //排序         

            return _article_category;
        }
        #endregion

        #region 取得一筆目錄資料
        /// <summary>
        /// 取得一筆目錄資料
        /// </summary>
        /// <param name="id">輸入文章目錄id編號</param>
        /// <returns>_article_category</returns>
        public article_category Get_article_category(int id)
        {
            article_category _article_category = new article_category();        //設定_article_category容器
            _article_category.id = id;                                                                   //將輸入的目錄id傳給_article_category容器id

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND Enabled = 1 ";    //登入取得所有資料:未登入只能取得上線資料
            string _sql = $"SELECT * FROM [article_category] WHERE [id]=@id {adminQuery} ";

            _article_category = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _article_category);

            return _article_category;
        }
        #endregion

        #region 修改資料
        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="_request">name,content,Enabled,Sort</param>
        /// <param name="_article_category">article_category資料表類別</param>
        public void Update_article_category(HttpRequest _request, article_category _article_category)
        {
            //將網頁上表單值傳給資料表類別
            _article_category.name = _request.Form["name"];
            _article_category.content = _request.Form["content"];
            _article_category.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));  //是否啟用(0/1)
            _article_category.Sort = Convert.ToInt16(_request.Form["sort"]);

            //$@"" 用法 @純字串 $可以設定變數{adminQuery} 
            string _sql = $@"UPDATE [dbo].[article_category]
                                       SET [name] = @name,
                                              [content] = @content,
                                              [updated_date] = getdate(),
                                              [Enabled] = @Enabled,
                                              [Sort] = @Sort
                                       Where [id] = @id ";
            //執行更新
            int result = _IDapperHelper.ExecuteSql(_sql, _article_category);
        }
        #endregion

        #region 刪除一筆資料
        /// <summary>
        /// 刪除一筆資料
        /// </summary>
        /// <param name="_article_category">id</param>
        public void Delete_article_category(article_category _article_category)
        {
            string _sql = @"DELETE FROM [dbo].[article_category] WHERE id = @id";
            _IDapperHelper.ExecuteSql(_sql, _article_category);
        }
        #endregion

        #region 取得所有目錄資料
        /// <summary>
        /// 取得所有目錄資料
        /// </summary>
        /// <returns>_article_Categories</returns>
        public List<article_category> Get_article_category_ALL()
        {
            string adminQuery = Auth.Role.IsAdmin ? "" : " Where Enabled = 1 ";    //登入取得所有資料:未登入只能取得上線資料
            string _sql = $"SELECT * FROM [article_category] {adminQuery} Order by Sort ";

            List<article_category> _article_Categories = _IDapperHelper.QuerySetSql<article_category>(_sql).ToList();

            return _article_Categories;
        }
        #endregion

    }
}