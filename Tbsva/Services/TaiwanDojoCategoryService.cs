using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class TaiwanDojoCategoryService : ITaiwanDojoCategoryService
    {
        #region DI依賴注入功能
        private IDapperHelper _dapperHelper;
        public TaiwanDojoCategoryService(IDapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }
        #endregion

        #region 新增
        public TaiwanDojoCategory InsertTaiwanDojoCategory(HttpRequest request)
        {
            TaiwanDojoCategory taiwanDojoCategory = new TaiwanDojoCategory();
            taiwanDojoCategory = requestData(taiwanDojoCategory, request);  //讀取表單資料進類別
            string _sql = @"INSERT INTO [TAIWAN_DOJO_CATEGORY]
                                               ([CATEGORYID]
                                               ,[NAME]
                                               ,[BERIF]
                                               ,[SORT]
                                               ,[ENABLED]
                                               ,[CREATIONDATE])
                                         VALUES
                                               (@CATEGORYID
                                               ,@NAME
                                               ,@BERIF
                                               ,@SORT
                                               ,@ENABLED
                                               ,@CREATIONDATE )
                                            SELECT SCOPE_IDENTITY()";
            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            //CAST (運算式 AS 資料型別 [ (資料長度) ])     
            //SELECT CAST(SCOPE_IDENTITY() AS int)";
            int id = _dapperHelper.QuerySingle(_sql, taiwanDojoCategory);
            taiwanDojoCategory.id = id;

            if (string.IsNullOrWhiteSpace(request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                taiwanDojoCategory.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                taiwanDojoCategory.sort = Convert.ToInt32(request.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [TAIWAN_DOJO_CATEGORY] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            _dapperHelper.ExecuteSql(_sql, taiwanDojoCategory);         //更新排序

            return taiwanDojoCategory;
        }
        /// <summary>
        /// 讀取表單資料
        /// </summary>
        /// <param name="taiwanDojoCategory">類別資料</param>
        /// <param name="request">讀取表單資料</param>
        /// <returns></returns>
        private TaiwanDojoCategory requestData(TaiwanDojoCategory taiwanDojoCategory, HttpRequest request)
        {
            taiwanDojoCategory.categoryId = Guid.NewGuid();
            taiwanDojoCategory.name = request.Form["name"];
            taiwanDojoCategory.berif = request.Form["berif"];
            taiwanDojoCategory.sort = Convert.ToInt32(request.Form["sort"]);
            taiwanDojoCategory.enabled = Convert.ToBoolean(Convert.ToByte(request.Form["enabled"]));
            taiwanDojoCategory.creationDate = DateTime.Now;

            return taiwanDojoCategory;
        }
        #endregion

        #region 取得一筆資料
        /// <summary>
        /// 取得一筆目錄資料
        /// </summary>
        /// <param name="categoryId">輸入目錄categoryId編號</param>
        /// <returns>200 Ok取得一筆目錄資料404 NotFound</returns>
        public TaiwanDojoCategory GetTaiwanDojoCategory(Guid categoryId)
        {
            TaiwanDojoCategory taiwanDojoCategory = new TaiwanDojoCategory();
            taiwanDojoCategory.categoryId = categoryId;    //將輸入的categoryId傳給類別內categoryId

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 " ;
            string _sql = $"SELECT * FROM [TAIWAN_DOJO_CATEGORY] WHERE [CATEGORYID] = @CATEGORYID {adminQuery} ";

            taiwanDojoCategory = _dapperHelper.QuerySqlFirstOrDefault(_sql, taiwanDojoCategory);

            return taiwanDojoCategory;
        }
        #endregion

        #region 修改資料
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request">輸入目錄id編號</param>
        /// <param name="taiwanDojoCategory"></param>
        public void UpdateTaiwanDojoCategory(HttpRequest request, TaiwanDojoCategory taiwanDojoCategory)
        {
            taiwanDojoCategory.name = request.Form["name"];
            taiwanDojoCategory.berif = request.Form["berif"];
            taiwanDojoCategory.sort = Convert.ToInt32(request.Form["sort"]);
            taiwanDojoCategory.enabled = Convert.ToBoolean(Convert.ToByte(request.Form["enabled"]));

            //$@"" 用法 @純字串 $可以設定變數{adminQuery} 
            string _sql = @"UPDATE [TAIWAN_DOJO_CATEGORY]
                                        SET [NAME] = @NAME,
                                          [BERIF] = @BERIF,
                                          [SORT] = @SORT,
                                          [ENABLED] = @ENABLED,
                                          [UPDATEDDATE] = GETDATE()
                                      WHERE [CATEGORYID] = @CATEGORYID ";
            //執行更新
            int result = _dapperHelper.ExecuteSql(_sql, taiwanDojoCategory);
        }
        #endregion

        #region 刪除一筆資料
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="taiwanDojoCategory">輸入目錄categoryId編號</param>
        public void DeleteTaiwanDojoCategory(TaiwanDojoCategory taiwanDojoCategory)
        {
            string _sql = @"DELETE FROM [TAIWAN_DOJO_CATEGORY] WHERE [CATEGORYID] = @CATEGORYID";
            _dapperHelper.ExecuteSql(_sql, taiwanDojoCategory);
        }
        #endregion

        #region 取得所有目錄資料
        /// <summary>
        /// 取得所有目錄資料
        /// </summary>
        /// <returns>所有目錄資料</returns>
        public List<TaiwanDojoCategory> GetTaiwanDojoCategories()
        {
            string adminQuery = Auth.Role.IsAdmin ? "" : " WHERE ENABLED = 1 ";
            string _sql = $"SELECT * FROM [TAIWAN_DOJO_CATEGORY] {adminQuery} ORDER BY SORT ";

            List<TaiwanDojoCategory> taiwanDojoCategories = _dapperHelper.QuerySetSql<TaiwanDojoCategory>(_sql).ToList();

            return taiwanDojoCategories;
        }
        #endregion

    }
}