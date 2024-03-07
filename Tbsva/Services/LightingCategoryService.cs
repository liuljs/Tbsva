using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class LightingCategoryService : ILightingCategoryService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        public LightingCategoryService(IDapperHelper IDapperHelper)
        {
            _IDapperHelper = IDapperHelper;
        }
        #endregion

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="_request">用戶端送來的表單資訊，Postman使用Body form-data</param>
        /// <returns>_lighting_Category</returns>
        public Lighting_category Insert_Lighting_category(HttpRequest _request)
        {
            Lighting_category _lighting_Category = new Lighting_category();     //產生一個空類別
            _lighting_Category = Request_data( _lighting_Category , _request);
            string _sql = @"INSERT INTO [Lighting_category]
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
            int id = _IDapperHelper.QuerySingle(_sql, _lighting_Category); //需使用QuerySingle，因Execute所傳回是新增成功數值
            //Lighting_category _lighting_Category_id = _IDapperHelper.QuerySqlFirstOrDefault<Lighting_category>("Select top 1 id from [Lighting_category] Order by id desc");
            _lighting_Category.id = id;

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(_request.Form["Sort"]))         //空值,或空格，或沒設定此欄位null
            {
                _lighting_Category.Sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                _lighting_Category.Sort = Convert.ToInt32(_request.Form["Sort"]);       //排序 
            }
            _sql = @"UPDATE [Lighting_category] 
                                SET [Sort] = @Sort 
                             Where [id] = @id ";
            _IDapperHelper.ExecuteSql(_sql, _lighting_Category);         //更新排序

            return _lighting_Category;
        }

        /// <summary>
        /// 讀取表單資料
        /// </summary>
        /// <param name="_lighting_Category">類別資料</param>
        /// <param name="_request">讀取表單資料</param>
        /// <returns></returns>
        private Lighting_category Request_data(Lighting_category _lighting_Category, HttpRequest _request)
        {
            //Lighting_category _lighting_Category = new Lighting_category();     //產生一個空類別
            _lighting_Category.name = _request.Form["name"];
            _lighting_Category.content = _request.Form["content"];
            _lighting_Category.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["Enabled"]));  //是否啟用(0/1)
            _lighting_Category.Sort = Convert.ToInt32(_request.Form["Sort"]);

            return _lighting_Category;
        }
        #endregion

        #region 取得一筆資料
        /// <summary>
        /// 取得一筆
        /// </summary>
        /// <param name="id">目錄id</param>
        /// <returns>_lighting_Category</returns>
        public Lighting_category Get_Lighting_category(int id)
        {
            Lighting_category _lighting_Category = new Lighting_category();             //先設定一個空類別
            _lighting_Category.id = id;                                                                                //將輸入的id傳給此類別_lighting_Category.id

            string adminQuery = Auth.Role.IsAdmin ? "" : " and Enabled =1 ";
            string _sql = $"SELECT * FROM [Lighting_category] Where [id]=@id {adminQuery} ";

            _lighting_Category = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _lighting_Category);

            return _lighting_Category;
        }
        #endregion

        #region 修改資料
        /// <summary>
        ///  修改資料
        /// </summary>
        /// <param name="_request">name,content,Enabled,Sort</param>
        /// <param name="_Lighting_category">資料表類別</param>
        public void Update_Lighting_category(HttpRequest _request, Lighting_category _Lighting_category)
        {
            _Lighting_category.name = _request.Form["name"];
            _Lighting_category.content = _request.Form["content"];
            _Lighting_category.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["Enabled"]));
            _Lighting_category.Sort = Convert.ToInt16(_request.Form["Sort"]);

            //$@"" 用法 @純字串 $可以設定變數{adminQuery} 
            string _sql = @"UPDATE [Lighting_category]
                                        SET [name] = @name,
                                          [content] = @content,
                                          [updated_date] = getdate(),
                                          [Enabled] = @Enabled,
                                          [Sort] = @Sort
                                      WHERE [id] = @id ";
            //執行更新
            int result = _IDapperHelper.ExecuteSql(_sql, _Lighting_category);
        }
        #endregion

        #region 刪除一筆資料
        /// <summary>
        /// 刪除一筆資料
        /// </summary>
        /// <param name="_Lighting_category">id</param>
        public void Delete_Lighting_category(Lighting_category _Lighting_category)
        {
            string _sql = @"DELETE FROM [Lighting_category] WHERE id = @id";
            _IDapperHelper.ExecuteSql(_sql, _Lighting_category);
        }
        #endregion

        #region 取得所有目錄資料
        /// <summary>
        /// 取得所有目錄資料
        /// </summary>
        /// <returns>_lighting_Categories</returns>
        public List<Lighting_category> Get_Lighting_category_ALL()
        {
            string adminQuery = Auth.Role.IsAdmin ? "" : " Where Enabled = 1 ";
            string _sql = $"SELECT * FROM [Lighting_category] {adminQuery} Order by Sort ";

            List<Lighting_category> _lighting_Categories = _IDapperHelper.QuerySetSql<Lighting_category>(_sql).ToList();

            return _lighting_Categories;
        }
        #endregion

    }
}