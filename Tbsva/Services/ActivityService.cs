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
    public class ActivityService : IActivityService
    {
        #region DI依賴注入功能
        private IDapperHelper DapperHelper;
        private IImageFileHelper ImageFileHelper;
        public ActivityService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            DapperHelper = dapperHelper;
            ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        public Activity InsertActivity(HttpRequest httpRequest)
        {
            Activity activity = RequestData(httpRequest);

            //設定上傳檔案的檔名對應相同的類別名稱
            //HttpRequest.Files 屬性 取得用戶端所上傳的檔案集合
            //httpRequest.Files 將文件集合加載到 HttpFileCollection 變量中。
            ImageFileHelper.SetImageFileName(activity, httpRequest.Files);

            string _sql = @" INSERT INTO [ACTIVITY]
                                                    ([ACTIVITY_ID]
                                                    ,[ACTIVITY_CATEGORY_ID]
                                                    ,[IMAGE_NAME]
                                                    ,[NAME]
                                                    ,[VIDEO]
                                                    ,[BRIEF]
                                                    ,[CONTENT]
                                                    ,[FIRST]
                                                    ,[SORT]
                                                    ,[ENABLED]
                                                    ,[START_DATE]
                                                    ,[END_DATE]
                                                    ,[CREATION_DATE]
                                                    ,[navPics01]
                                                    ,[navPics02]
                                                    ,[navPics03]
                                                    ,[navPics04]
                                                    ,[navPics05]
                                                    ,[navPics06]
                                                    ,[navPics07]
                                                    ,[navPics08] )
                                                VALUES
                                                    (@ACTIVITY_ID,
                                                     @ACTIVITY_CATEGORY_ID,
                                                     @IMAGE_NAME, 
                                                     @NAME, 
                                                     @VIDEO,
                                                     @BRIEF,
                                                     @CONTENT,
                                                     @FIRST,
                                                     @SORT,
                                                     @ENABLED, 
                                                     @START_DATE, 
                                                     @END_DATE,
                                                     @CREATION_DATE,
                                                     @navPics01,
                                                     @navPics02,
                                                     @navPics03,
                                                     @navPics04,
                                                     @navPics05,
                                                     @navPics06,
                                                     @navPics07,
                                                     @navPics08 )
                                                         SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。

            if (activity.start_date == DateTime.MinValue)
            {
                _sql = _sql.Replace("@START_DATE", "NULL");
            }
            if (activity.end_date == DateTime.MinValue)
            {
                _sql = _sql.Replace("@END_DATE", "NULL");
            }
            int id = DapperHelper.QuerySingle(_sql, activity);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            activity.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(httpRequest.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                activity.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                activity.sort = Convert.ToInt32(httpRequest.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [ACTIVITY] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            DapperHelper.ExecuteSql(_sql, activity);         //更新排序

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Activity);
            string _saveFolderPath = ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{id}\");

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            //ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], activity.image_name, _RootPath_ImageFolderPath);

            //2.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            ImageFileHelper.SaveMoreUploadImageFile(httpRequest.Files, _saveFolderPath);

            //3.1將編輯內容圖片裏剛產生的ActivityImage表裏的圖檔尚未關連此activity.Id更新進去
            HandleContentImages(httpRequest.Form["cNameArr"], _RootPath_ImageFolderPath, activity.activity_id);

            //4.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);

            return activity;
        }
        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="httpRequest">讀取表單資料</param>
        /// <returns>回傳表單類別</returns>
        private Activity RequestData(HttpRequest httpRequest)
        {
            Activity activity = new Activity();
            activity.activity_id = Guid.NewGuid();                                                                                            //活動類別另設 ID 唯一索引用
            //activity.image_name = activity.activity_id + ".png";                                                                       //cover圖片檔名使用id來當
            activity.name = httpRequest.Form["name"];                                                                                //活動標題
            activity.activity_category_id = new Guid(httpRequest.Form["categoryId"]);                             //活動隸屬目錄

            string _sql = @" SELECT [NAME] FROM [ACTIVITY_CATEGORY] WHERE [CATEGORY_ID] = @CATEGORY_ID";
            ActivityCategory activityCategory = new ActivityCategory();
            activityCategory.category_id = activity.activity_category_id;               //取得的活動隸屬目錄Guid傳給要查詢的目錄category_id
            activityCategory = DapperHelper.QuerySqlFirstOrDefault(_sql, activityCategory);  //查詢
            activity.categoryName = activityCategory.name;  //將查到的目錄名傳給activity類別

            activity.brief = httpRequest.Form["brief"];                                                                                   //活動簡述 
            activity.video = httpRequest.Form["video"];                                                                                //影片網址 
            activity.content = httpRequest.Form["content"];                                                                        //活動內容
            activity.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            activity.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            activity.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)

            //上架日, Start_Date Postman進來, 會是空值或沒有此欄位或有值，有值以外兩個條件都不會成立，所以Activity.Start_Date沒收到會是null
            if (Tools.Formatter.IsDate(httpRequest.Form["startDate"]))
            {
                activity.start_date = Convert.ToDateTime(httpRequest.Form["startDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                activity.start_date = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
                 //不然ActivityDto activityDto = Mapper.Map<ActivityDto>(activity);
                 //會將null轉成空字串Activity DateTime? Start_Date (null) => ActivityDto string Start_Date ("")
                 //結果ActivityProfile FormatDate又將如果值是DateTime.MinValue時又轉為""
            }
            //下架日
            if (Tools.Formatter.IsDate(httpRequest.Form["endDate"]))
            {
                activity.end_date = Convert.ToDateTime(httpRequest.Form["endDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                activity.end_date = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
                //不然ActivityDto activityDto = Mapper.Map<ActivityDto>(activity);
                //會將null轉成空字串Activity DateTime? Start_Date (null) => ActivityDto string Start_Date ("")
                //結果ActivityProfile FormatDate又將如果值是DateTime.MinValue時又轉為""
            }
            activity.creation_date = DateTime.Now;

            return activity;
        }
        #endregion

        #region 取得一筆資料
        public Activity GetActivity(Guid activityId)
        {
            Activity activity = new Activity();
            activity.activity_id = activityId;      //活動類別另設 ID
            //2021 / 11 / 30   2021-12-01 13:59:01  2021 / 12 / 02
            //Start_Date <= getdate() < End_Date +1 End_Date 是當天上午12:00 => 00:00
            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 AND (START_DATE IS NULL OR START_DATE <= GETDATE() ) AND (END_DATE IS NULL OR GETDATE() < DATEADD(DAY, 0, END_DATE) ) ";
            string _sql = $"SELECT * FROM [ACTIVITY] WHERE [ACTIVITY_ID] = @ACTIVITY_ID {adminQuery}";
            activity = DapperHelper.QuerySqlFirstOrDefault(_sql, activity);

            if (activity != null)  //調出目錄名稱
            {
                _sql = @"SELECT [NAME] FROM [ACTIVITY_CATEGORY] WHERE [CATEGORY_ID] = @CATEGORY_ID";
                ActivityCategory activityCategory = new ActivityCategory();
                activityCategory.category_id = activity.activity_category_id;               //取得的活動隸屬目錄Guid傳給要查詢的目錄category_id
                activityCategory = DapperHelper.QuerySqlFirstOrDefault(_sql, activityCategory);  //查詢
                activity.categoryName = activityCategory.name;  //將查到的目錄名傳給activity類別
            }
            //加上http網址
            if (activity != null)  //確定有資料在處理
            {
                string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
                SystemFunctions.picaddhttp(fields, activity, Tools.GetInstance().Activity, activity.id);

                //if (activity.image_name != null)
                //{
                //    activity.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", activity.image_name);
                //}
                //if (activity.navPics01 != null)
                //{
                //    activity.navPics01 = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", activity.navPics01);
                //}
                //if (activity.navPics02 != null)
                //{
                //    activity.navPics02 = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", activity.navPics02);
                //}
                //if (activity.navPics03 != null)
                //{
                //    activity.navPics03 = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", activity.navPics03);
                //}
                //if (activity.navPics04 != null)
                //{
                //    activity.navPics04 = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", activity.navPics04);
                //}
                //if (activity.navPics05 != null)
                //{
                //    activity.navPics05 = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", activity.navPics05);
                //}
                //if (activity.navPics06 != null)
                //{
                //    activity.navPics06 = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", activity.navPics06);
                //}
                //if (activity.navPics07 != null)
                //{
                //    activity.navPics07 = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", activity.navPics07);
                //}
                //if (activity.navPics08 != null)
                //{
                //    activity.navPics08 = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", activity.navPics08);
                //}

            }
            return activity;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateActivity(HttpRequest httpRequest, Activity activity)
        {
            activity = RequestDataMod(httpRequest, activity); //將接收來的參數，和抓到的要修改資料合併

            //1.設定上傳檔案的檔名對應相同的類別名稱
            //HttpRequest.Files 屬性 取得用戶端所上傳的檔案集合
            //httpRequest.Files 將文件集合加載到 HttpFileCollection 變量中。
            ImageFileHelper.SetImageFileName(activity, httpRequest.Files);

            //2.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Activity);
            string _saveFolderPath = ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{activity.id}\");

            //3.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            ImageFileHelper.SaveMoreUploadImageFile(httpRequest.Files, _saveFolderPath);

            //4.組裝修改刪除不變更時sql
            //string _sqlpic = picSql(activity, httpRequest);
            string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
            string _sqlpic = SystemFunctions.picSql<Activity>(fields, activity, httpRequest);

            //#region 辨斷有沒有上傳檔案
            //if (httpRequest.Files.Count > 0) //1檢查有沒有上傳圖片的欄位選項
            //{
            //    if (httpRequest.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳
            //    {
            //        //以下處理圖片
            //        //1. 取得要放置的目錄路徑
            //        string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Activity);

            //        // Activity activity = ActivityService.GetActivity(id); 裏面的image_name有加網址ImageFileHelper.GetImageLink，改只取檔案名與副檔
            //        string _image_name = Path.GetFileName(activity.image_name);

            //        //2.存放上傳封面圖片(1實體檔,2檔名,3路徑)
            //        ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], _image_name, _RootPath_ImageFolderPath);
            //    }
            //}

            ////將在控制器上取出的資料庫裏的有加http://..最後檔名取出，不然資料庫更新時會出錯超出欄位大小
            //activity.image_name = Path.GetFileName(activity.image_name);
            //#endregion

            string _sql = $@"UPDATE [ACTIVITY]
                                           SET
                                               [ACTIVITY_CATEGORY_ID] = @ACTIVITY_CATEGORY_ID
                                              ,[NAME] = @NAME
                                              ,[VIDEO] = @VIDEO
                                              ,[BRIEF] = @BRIEF
                                              ,[CONTENT] = @CONTENT
                                              ,[FIRST] = @FIRST
                                              ,[SORT] = @SORT
                                              ,[ENABLED] = @ENABLED
                                              ,[START_DATE] = @START_DATE
                                              ,[END_DATE] = @END_DATE
                                              ,[UPDATED_DATE] = @UPDATED_DATE
                                               {_sqlpic}
                                         WHERE [ACTIVITY_ID] = @ACTIVITY_ID ";

            if (activity.start_date == DateTime.MinValue)
            {
                _sql = _sql.Replace("@START_DATE", "NULL");
            }
            if (activity.end_date == DateTime.MinValue)
            {
                _sql = _sql.Replace("@END_DATE", "NULL");
            }
            DapperHelper.ExecuteSql(_sql, activity);
        }
        /// <summary>
        /// 讀取表單資料,轉到activity
        /// </summary>
        /// <param name="httpRequest">讀取表單資料(修改時用)</param>
        /// <param name="activity">控制器取到資料</param>
        /// <returns>activity</returns>
        private Activity RequestDataMod(HttpRequest httpRequest, Activity original_activity)
        {
            Activity activity = new Activity();                                                         //設定一組空的沒資料會null, 讓圖片預設是null不執行SQL指令，此null是類別裏真的null 
            activity.id = original_activity.id;
            activity.activity_id = original_activity.activity_id;
            activity.activity_category_id = new Guid(httpRequest.Form["categoryId"]);                          //活動隸屬目錄
            activity.categoryName = original_activity.categoryName;
            //activity.image_name = activity.activity_id + ".png";                                                                 //圖片檔名使用id來當 圖片在新增時已命名
            activity.name = httpRequest.Form["name"];                                                                                //活動標題
            activity.brief = httpRequest.Form["brief"];                                                                                   //活動簡述 
            activity.video = httpRequest.Form["video"];                                                                                //影片網址 
            activity.content = httpRequest.Form["content"];                                                                        //活動內容
            activity.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            activity.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            activity.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)

            //上架日, Start_Date Postman進來, 會是空值或沒有此欄位或有值，有值以外兩個條件都不會成立，所以Activity.Start_Date沒收到會是null
            if (Tools.Formatter.IsDate(httpRequest.Form["startDate"]))
            {
                activity.start_date = Convert.ToDateTime(httpRequest.Form["startDate"]);
            }
            else //更新時，若時間改成不選時要設定
            {
                activity.start_date = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
                                                          //不然ActivityDto activityDto = Mapper.Map<ActivityDto>(activity);
                                                          //會將null轉成空字串Activity DateTime? Start_Date (null) => ActivityDto string Start_Date ("")
                                                          //結果ActivityProfile FormatDate又將如果值是DateTime.MinValue時又轉為""
            }
            //下架日
            if (Tools.Formatter.IsDate(httpRequest.Form["endDate"]))
            {
                activity.end_date = Convert.ToDateTime(httpRequest.Form["endDate"]);
            }
            else //更新時，若時間改成不選時要設定
            {
                activity.end_date = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
                //不然ActivityDto activityDto = Mapper.Map<ActivityDto>(activity);
                //會將null轉成空字串Activity DateTime? Start_Date (null) => ActivityDto string Start_Date ("")
                //結果ActivityProfile FormatDate又將如果值是DateTime.MinValue時又轉為""
            }
            activity.updated_date = DateTime.Now;

            return activity;
        }
        #endregion

        #region 刪除一筆資料
        public void DeleteActivity(Activity activity)
        {
            //1. 取得放置圖片的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Activity);

            //取得這筆內容下相關內容圖片檔名清單
            string _sql = @"SELECT [IMAGE_NAME] FROM [ACTIVITY_IMAGE] WHERE ACTIVITY_ID = @ACTIVITY_ID";
            //調出此筆內容下的多筆圖片名稱，到List<string>
            List<string> activity_image_name_list = DapperHelper.QuerySetSql<Activity, string>(_sql, activity).ToList();

            //刪除內容下內容圖, List<string> _activity_image_name_list
            string activity_image_name_list_path = string.Empty;  //宣告一個變數之後要加入路徑
            for (int i = 0; i < activity_image_name_list.Count; i++)
            {
                //路徑與圖檔結合, 再辨斷存在就刪除
                activity_image_name_list_path = Path.Combine(_RootPath_ImageFolderPath, activity_image_name_list[i]);
                if (File.Exists(activity_image_name_list_path))
                {
                    File.Delete(activity_image_name_list_path);
                }
            }

            //刪除封面與導覽圖片(刪自動id目錄)
            //取出內容圖片檔案
            //string _image_name = Path.GetFileName(activity.image_name);   //取出內容封面檔名
            //string _image_name_FilePath = Path.Combine(_RootPath_ImageFolderPath, _image_name);  //封面檔名加上路徑
            //if (File.Exists(_image_name_FilePath))
            //{
            //    File.Delete(_image_name_FilePath);
            //}
            string folder = Path.Combine(_RootPath_ImageFolderPath, activity.id.ToString());
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            //刪除資料庫內容 資料庫有設定重疊顯示，所以刪主鍵關連ACTIVITY_IMAGE資料會一同刪除FK_activity_image_Activity
            _sql = @"DELETE FROM [ACTIVITY] WHERE ACTIVITY_ID = @ACTIVITY_ID";
            //執行刪除
            DapperHelper.ExecuteSql(_sql, activity);
        }
        #endregion

        #region 取得分頁活動管理
        public List<Activity> GetActivities(string categoryId, int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string categoryId_sql = null;         // 活動隸屬目錄 ID  sql

            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }
            string adminQuery = Auth.Role.IsAdmin ? " WHERE 1 = 1 " : " WHERE A2.ENABLED = 1 AND (START_DATE IS NULL OR START_DATE <= GETDATE() ) AND (END_DATE IS NULL OR GETDATE() < DATEADD(DAY, 0, END_DATE) ) ";  //登入取得所有資料:未登入只能取得上線資料
            //若有輸出活動隸屬目錄 ID，驗證是GUID格式D 以連字號分隔的32位數:00000000-0000-0000-0000-000000000000
            categoryId_sql = Guid.TryParseExact(categoryId, "D", out Guid categoryIdguid) ? $" AND [ACTIVITY_CATEGORY_ID] = '{categoryIdguid}' " : "";
            //categoryId_sql = Tools.Formatter.IsGuidValid(categoryId) ? $" AND [ACTIVITY_CATEGORY_ID] = {categoryId}" : "";

            string _sql = @"SELECT A2.*, A1.NAME AS CATEGORYNAME FROM [ACTIVITY] A2 " +
                                  @"LEFT JOIN [ACTIVITY_CATEGORY] A1 ON A1.[CATEGORY_ID] = A2.[ACTIVITY_CATEGORY_ID] " +
                                  $" {adminQuery} " +
                                  $" {categoryId_sql} " +
                                  @"ORDER BY A2.[FIRST] DESC, A2.[CREATION_DATE] DESC, A2.[SORT] " +
                                  $" {page_sql} ";
            List<Activity> activity = DapperHelper.QuerySetSql<Activity>(_sql).ToList();

            //將資料庫image_name加上URL
            for (int i = 0; i < activity.Count; i++)
            {
                //activity[i].image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity, activity[i].image_name);

                string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
                SystemFunctions.picaddhttp(fields, activity[i], Tools.GetInstance().Activity, activity[i].id);
            }
            return activity;
        }
        #endregion

        /// <summary>
        /// 新增一個[ActivityImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 因為正在新增內容還未送出，所以內容的ActivityImage_activity_id關連activity.activity_id還沒產生
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns>activityImage</returns>
        public ActivityImage AddUploadImage(HttpRequest httpRequest)
        {
            ActivityImage activityImage = new ActivityImage();
            activityImage.activity_image_id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
            activityImage.image_name = activityImage.activity_image_id + ".png";   //用activity_image_id來命名圖檔
            //activityImage.activity_id = "關連內容activity_id尚未產生，進入新增狀態時尚未送出所以沒有值";

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Activity);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], activityImage.image_name, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [ACTIVITY_IMAGE]
                                            ([ACTIVITY_IMAGE_ID]
                                            ,[IMAGE_NAME])
                                        VALUES
                                            (@ACTIVITY_IMAGE_ID,
                                            @IMAGE_NAME )
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = DapperHelper.QuerySingle(_sql, activityImage);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            activityImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            activityImage.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity, activityImage.image_name);

            return activityImage;
        }

        /// <summary>
        /// 更新活動管理的內容圖片
        /// 新增一個[ActivityImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="Activity_Id"></param>
        /// <returns></returns>
        public ActivityImage AddUploadImage(HttpRequest httpRequest, Guid Activity_Id)
        {
            ActivityImage activityImage = new ActivityImage();
            activityImage.activity_image_id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
            activityImage.image_name = activityImage.activity_image_id + ".png";   //用activity_image_id來命名圖檔
            activityImage.activity_id = Activity_Id;                                                            //關聯內容activity_id

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Activity);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], activityImage.image_name, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [ACTIVITY_IMAGE]
                                            ([ACTIVITY_IMAGE_ID]
                                            ,[IMAGE_NAME]
                                            ,[ACTIVITY_ID])
                                        VALUES
                                            (@ACTIVITY_IMAGE_ID,
                                            @IMAGE_NAME,
                                            @ACTIVITY_ID)
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = DapperHelper.QuerySingle(_sql, activityImage);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            activityImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            activityImage.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity, activityImage.image_name);

            return activityImage;
        }


        #region 工具區
        /// <summary>
        /// 在新增內容時，下面內文若有插入圖片，插入的圖片還沒有Activity_id, 送出前fNameArr收集了內容裏插入了多少張圖
        /// 然後在送出後在將確定有插入的內容圖片更新ActivityImage.Activity_id=Activity_id
        /// 為何不先給插入圖片id，因為插入圖片隨時可能又del，當del掉後若已給id就無法用null來辨斷刪除
        /// </summary>
        /// <param name="images">httpRequest.Form["fNameArr"]內容圖片的檔名JSON字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="Id">關連內容的activity_id</param>
        private void HandleContentImages(string images, string saveFolderPath, Guid Id)
        {
            //JsonConvert.DeserializeObject<類別>("字串")，<>中定義類別，將JSON字串反序列化成物件
            //JSON字串
            //string Json = "{ 'Name': 'Berry', 'Age': 18, 'Marry': false, 'Habit': [ 'Sing','Dance','Code','Sleep' ] }";
            //轉成物件
            //Introduction Introduction = JsonConvert.DeserializeObject<Introduction>(Json);
            //顯示
            //Response.Write(Introduction.Name);

            //fNameArr沒有欄位只有值_retFileNameList[i]
            //取得回傳內容圖片的檔名fNameArr["abc.png","def.png"]， Json反序列化(Deserialize)為物件(Object)
            List<string> _retFileNameList = JsonConvert.DeserializeObject<List<string>>(images);

            //取得存放saveFolderPath目錄下所有.png圖檔
            //GetFiles (string path, string searchPattern);
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png
            if (_retFileNameList != null && _retFileNameList.Count > 0)    //fNameArr確認有檔才處理
            {
                for (int i = 0; i < _retFileNameList.Count; i++) //處理裏面的所有檔fNameArr ["abc.png","def.png"]
                {
                    for (int y = 0; y < _imageFileList.Count; y++)  //fNameArr和目錄下實際所有.png圖檔比對一下
                    {
                        string _urlFileName = _retFileNameList[i].ToLower().Trim();   //處理fNameArr內的檔轉小寫去空白
                        string _entityFileName = Path.GetFileName(_imageFileList[y]);  //目錄下的檔, 只需留檔案名稱與副檔名
                        string _imageGuid = Path.GetFileNameWithoutExtension(_imageFileList[y]);  //只留下檔案名稱，路徑與副檔名都去除
                        //判斷fNameArr[]傳進來的參數圖檔==目前目錄下的檔&&判斷目錄下的檔案是否存在。
                        if (_urlFileName == _entityFileName && File.Exists(_imageFileList[y]))
                        {
                            ActivityImage activityImage = new ActivityImage();
                            activityImage.activity_image_id = Guid.Parse(_imageGuid);        //存放目錄下檔名不含副檔給ActivityImage表裏id
                            activityImage.activity_id = Id;                                        //關聯活動管理內容id

                            //更新Activity內容的activity_id給ActivityImage.activity_id 連立關連
                            string _sql = @"UPDATE [ACTIVITY_IMAGE]
                                                           SET [ACTIVITY_ID] =@ACTIVITY_ID
                                                         WHERE ACTIVITY_IMAGE_ID = @ACTIVITY_IMAGE_ID ";
                            DapperHelper.ExecuteSql(_sql, activityImage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刪除[Activity_Id] IS NULL不用的內容圖片
        /// </summary>
        /// <param name="saveFolderPath"></param>
        private void RemoveContentImages(string saveFolderPath)
        {
            //取得存放磁碟目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png

            //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
            string _sql = @"SELECT [IMAGE_NAME] FROM [ACTIVITY_IMAGE] WHERE ACTIVITY_ID IS NULL";
            List<string> _imageDataBaseList = DapperHelper.QuerySetSql<string>(_sql).ToList();

            //刪除多餘的內容圖檔, _imageDataBaseList 資料庫檔案名清單，_imageFileList 實際目錄檔路徑檔案清單
            for (int y = 0; y < _imageDataBaseList.Count; y++)
            {
                for (int i = 0; i < _imageFileList.Count; i++)
                {
                    string _removeFileName = Path.GetFileName(_imageFileList[i]);   //除掉路徑留檔案名稱

                    //資料庫Activity_Id=NULL的檔案名 == 資料匣存放的檔名&&存放檔實際是否存在
                    if (_imageDataBaseList[y] == _removeFileName && File.Exists(_imageFileList[i]))
                    {
                        File.Delete(_imageFileList[i]);
                    }
                }
            }
            //刪除資料庫裡無相關連的內容圖片記錄
            _sql = @"DELETE FROM [ACTIVITY_IMAGE] WHERE [ACTIVITY_ID] IS NULL";

            DapperHelper.ExecuteSql(_sql);
        }
        #endregion

        /// <summary>
        /// 組裝修改刪除不變更時sql
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="_request"></param>
        /// <returns></returns>
        //private string picSql(Activity activity, HttpRequest _request)
        //{
        //    string pic = string.Empty;
        //    //var null = _request.Form["Picture01"];
        //    //圖片更新有4種狀況
        //    //1.新增:選圖片上傳_request.Files;AllKeys就會有欄位,檔名就會代給類別_pictureList.Picture01 = _file.FileName
        //    //因為Picture01欄位有圖檔, 所以_request.Form["Picture01"]就不見變成null, 但下面的null並不是真的null, 而時字串的"null"
        //    //所以新增時以下左邊會成立，右邊不成立
        //    //2.修改:有重新上傳的_request.Files,AllKeys就會有欄位會有值, 
        //    //3.沒有變更:沒有重新上傳檔案,但在_request.Form[""] = xx.PNG會接到現有的檔案名稱，所以下面條件都不成立
        //    //3.按垃圾筒,並沒有上傳Files內容，而是傳回表單字串null(_request.Form["xx"] == "null"，所以會執行下面右方sql將資料欄改NULL
        //    //4.完全沒變動:會變成_request.Form的值, 原本有圖檔的如_request.Form["Picture01"]=01.PNG, 沒有圖檔的就會是空值，下面條件都不成立
        //    if (activity.navPics01 != null) pic += ",[navPics01]=@navPics01"; if (_request.Form["navPics01"] == "null") pic += ",[navPics01]=NULL";
        //    if (activity.navPics02 != null) pic += ",[navPics02]=@navPics02"; if (_request.Form["navPics02"] == "null") pic += ",[navPics02]=NULL";
        //    if (activity.navPics03 != null) pic += ",[navPics03]=@navPics03"; if (_request.Form["navPics03"] == "null") pic += ",[navPics03]=NULL";
        //    if (activity.navPics04 != null) pic += ",[navPics04]=@navPics04"; if (_request.Form["navPics04"] == "null") pic += ",[navPics04]=NULL";
        //    if (activity.navPics05 != null) pic += ",[navPics05]=@navPics05"; if (_request.Form["navPics05"] == "null") pic += ",[navPics05]=NULL";
        //    if (activity.navPics06 != null) pic += ",[navPics06]=@navPics06"; if (_request.Form["navPics06"] == "null") pic += ",[navPics06]=NULL";
        //    if (activity.navPics07 != null) pic += ",[navPics07]=@navPics07"; if (_request.Form["navPics07"] == "null") pic += ",[navPics07]=NULL";
        //    if (activity.navPics08 != null) pic += ",[navPics08]=@navPics08"; if (_request.Form["navPics08"] == "null") pic += ",[navPics08]=NULL";
        //    return pic;
        //}


    }
}