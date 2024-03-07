using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class ActivityCategoryService : IActivityCategoryService
    {
        #region DI依賴注入功能
        private IDapperHelper dapperHelper;
        public ActivityCategoryService(IDapperHelper dapperHelper)
        {
            this.dapperHelper = dapperHelper;
        }
        #endregion

        #region 新增
        /// <summary>
        /// 新增一筆目錄
        /// </summary>
        /// <param name="httpRequest">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 目錄資料</returns>
        public ActivityCategory InsertActivityCategory(HttpRequest httpRequest)
        {
            ActivityCategory activityCategory = new ActivityCategory();
            activityCategory = requestData(activityCategory, httpRequest);  //讀取表單資料進類別
            string _sql = @"INSERT INTO [ACTIVITY_CATEGORY]
                                               ([CATEGORY_ID]
                                               ,[NAME]
                                               ,[BERIF]
                                               ,[SORT]
                                               ,[ENABLED]
                                               ,[CREATION_DATE])
                                         VALUES
                                               (@CATEGORY_ID
                                               ,@NAME
                                               ,@BERIF
                                               ,@SORT
                                               ,@ENABLED
                                               ,@CREATION_DATE )
                                            SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
                                       //CAST (運算式 AS 資料型別 [ (資料長度) ])     
                                       //SELECT CAST(SCOPE_IDENTITY() AS int)";
            int id = dapperHelper.QuerySingle(_sql, activityCategory); //需使用QuerySingle，因Execute所傳回是新增成功數值
            activityCategory.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id
            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(httpRequest.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                activityCategory.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                activityCategory.sort = Convert.ToInt32(httpRequest.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [ACTIVITY_CATEGORY] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            dapperHelper.ExecuteSql(_sql, activityCategory);         //更新排序

            return activityCategory;
        }
        /// <summary>
        ///  讀取表單資料
        /// </summary>
        /// <param name="activityCategory">類別資料</param>
        /// <param name="httpRequest">讀取表單資料</param>
        /// <returns>activityCategory</returns>
        private ActivityCategory requestData(ActivityCategory activityCategory, HttpRequest httpRequest)
        {
            activityCategory.category_id = Guid.NewGuid();
            activityCategory.name = httpRequest.Form["name"];                                                                                      //目錄名稱 
            activityCategory.berif = httpRequest.Form["berif"];                                                                                         //目錄簡述
            activityCategory.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                            //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            activityCategory.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));           //目錄狀態  必填 {int}  啟用狀態（0 關閉；1 開啟）
            activityCategory.creation_date = DateTime.Now;                                                                                            //新增時間

            return activityCategory;
        }
        #endregion

        #region 取得一筆資料
        /// <summary>
        /// 取得一筆目錄資料
        /// </summary>
        /// <param name="categoryId">輸入目錄category_id編號</param>
        /// <returns>200 Ok取得一筆目錄資料404 NotFound</returns>
        public ActivityCategory GetActivityCategory(Guid categoryId)
        {
            ActivityCategory activityCategory = new ActivityCategory();             //先設定一個空類別
            activityCategory.category_id = categoryId;                                        //將輸入的id傳給此目錄類別的目錄activityCategory.category_id

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 ";
            string _sql = $"SELECT * FROM [ACTIVITY_CATEGORY] WHERE [CATEGORY_ID] = @CATEGORY_ID {adminQuery} ";

            activityCategory = dapperHelper.QuerySqlFirstOrDefault(_sql, activityCategory);

            return activityCategory;
        }
        #endregion

        #region 修改資料
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="httpRequest">輸入目錄id編號</param>
        /// <param name="activityCategory"></param>
        /// <returns>204 No Content , 404 NotFound</returns>
        public void UpdateActivityCategory(HttpRequest httpRequest, ActivityCategory activityCategory)
        {
            activityCategory.name = httpRequest.Form["name"];
            activityCategory.berif = httpRequest.Form["berif"];
            activityCategory.sort = Convert.ToInt32(httpRequest.Form["sort"]);
            activityCategory.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));

            //$@"" 用法 @純字串 $可以設定變數{adminQuery} 
            string _sql = @"UPDATE [ACTIVITY_CATEGORY]
                                        SET [NAME] = @NAME,
                                          [BERIF] = @BERIF,
                                          [SORT] = @SORT,
                                          [ENABLED] = @ENABLED,
                                          [UPDATED_DATE] = GETDATE()
                                      WHERE [CATEGORY_ID] = @CATEGORY_ID ";
            //執行更新
            int result = dapperHelper.ExecuteSql(_sql, activityCategory);
        }
        #endregion

        #region 刪除一筆資料
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="activityCategory">輸入目錄id編號</param>
        /// <returns>成功204 , 失敗404</returns>
        public void DeleteActivityCategory(ActivityCategory activityCategory)
        {
            string _sql = @"DELETE FROM [ACTIVITY_CATEGORY] WHERE [CATEGORY_ID] = @CATEGORY_ID";
            dapperHelper.ExecuteSql(_sql, activityCategory);
        }
        #endregion

        #region 取得所有目錄資料
        /// <summary>
        /// 取得所有目錄資料
        /// </summary>
        /// <returns>所有目錄資料</returns>
        public List<ActivityCategory> GetActivityCategories()
        {
            string adminQuery = Auth.Role.IsAdmin ? "" : " WHERE ENABLED = 1 ";
            string _sql = $"SELECT * FROM [ACTIVITY_CATEGORY] {adminQuery} ORDER BY SORT ";

            List<ActivityCategory> activityCategories = dapperHelper.QuerySetSql<ActivityCategory>(_sql).ToList();

            return activityCategories;
        }
        #endregion

    }
}