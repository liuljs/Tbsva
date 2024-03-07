using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class TimeMachineService : ITimeMachineService
    {
        #region DI依賴注入功能
        private IDapperHelper _dapperHelper;
        private IImageFileHelper _imageFileHelper;
        public TimeMachineService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            _dapperHelper = dapperHelper;
            _imageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        public TimeMachine InsertTimeMachine(HttpRequest request)
        {
            TimeMachine timeMachine = RequestData(request);

            //1.設定上傳檔案的檔名對應相同的類別名稱
            //request.Files 屬性 取得用戶端所上傳的檔案集合
            //request.Files 將文件集合加載到 HttpFileCollection 變量中。
            SetImageFileName(timeMachine, request.Files);

            string _sql = @"INSERT INTO [TIME_MACHINE]
                                                    ([TIMEMACHINEID]
                                                    ,[NAME]
                                                    ,[COURSE]
                                                    ,[BRIEF]
                                                    ,[FIRST]
                                                    ,[SORT]
                                                    ,[ENABLED]
                                                    ,[navPics01]
                                                    ,[navPics02]
                                                    ,[STARTDATE]
                                                    ,[ENDDATE]
                                                    ,[CREATIONDATE] )
                                                VALUES
                                                    (@TIMEMACHINEID,
                                                     @NAME, 
                                                     @COURSE,
                                                     @BRIEF,
                                                     @FIRST,
                                                     @SORT,
                                                     @ENABLED, 
                                                     @navPics01, 
                                                     @navPics02, 
                                                     @STARTDATE, 
                                                     @ENDDATE,
                                                     @CREATIONDATE )
                                                     SELECT CAST(SCOPE_IDENTITY() AS INT)";
            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            //CAST (運算式 AS 資料型別 [ (資料長度) ])

            if (timeMachine.startDate == DateTime.MinValue)
            {
                _sql = _sql.Replace("@STARTDATE", "NULL");
            }
            if (timeMachine.endDate == DateTime.MinValue)
            {
                _sql = _sql.Replace("@ENDDATE", "NULL");
            }
            //2.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            int id = _dapperHelper.QuerySingle(_sql, timeMachine);
            timeMachine.id = id; //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                timeMachine.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                timeMachine.sort = Convert.ToInt32(request.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [TIME_MACHINE] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            _dapperHelper.ExecuteSql(_sql, timeMachine);         //更新排序

            //處理圖片
            //3.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().timeMachine);
            string _saveFolderPath = _imageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{id}\");

            //4.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            _imageFileHelper.SaveMoreUploadImageFile(request.Files, _saveFolderPath);

            return timeMachine;
        }

        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="request">讀取表單資料</param>
        /// <returns>回傳表單類別</returns>
        private TimeMachine RequestData(HttpRequest request)
        {
            TimeMachine timeMachine = new TimeMachine();
            timeMachine.timeMachineId = Guid.NewGuid();          //時光走廊 Guid
            timeMachine.name = request.Form["name"];               //時間標題2002 12月
            timeMachine.course = Convert.ToDateTime(request.Form["course"]);          //副標題
            timeMachine.brief = request.Form["brief"];                  //簡述
            timeMachine.first = Convert.ToBoolean(Convert.ToByte(request.Form["first"]));                 //是否置頂(0 關閉;1 開啟)
            timeMachine.sort = Convert.ToInt32(request.Form["sort"]);                                                    //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            timeMachine.enabled = Convert.ToBoolean(Convert.ToByte(request.Form["enabled"]));   //是否啟用(0 關閉;1 開啟)

            //上架日, startDate Postman進來, 會是空值或沒有此欄位或有值，有值以外兩個條件都不會成立，所以startDate沒收到會是null
            if (Tools.Formatter.IsDate(request.Form["startDate"]))
            {
                timeMachine.startDate = Convert.ToDateTime(request.Form["startDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                timeMachine.startDate = DateTime.MinValue;
            }
            //下架日
            if (Tools.Formatter.IsDate(request.Form["endDate"]))
            {
                timeMachine.endDate = Convert.ToDateTime(request.Form["endDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                timeMachine.endDate = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
            }
            timeMachine.creationDate = DateTime.Now;

            return timeMachine;
        }
        #endregion

        #region 取得一筆資料
        public TimeMachine GetTimeMachine(Guid timeMachineId)
        {
            TimeMachine timeMachine = new TimeMachine();
            timeMachine.timeMachineId = timeMachineId;  //資料Guid欄，要用他來搜尋

            //2021 / 11 / 30   2021-12-01 13:59:01  2021 / 12 / 02
            //startDate <= getdate() < endDate +1 endDate 是當天上午12:00 => 00:00, 沒設定時間時就全抓
            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 AND (STARTDATE IS NULL OR STARTDATE <= GETDATE() ) AND (ENDDATE IS NULL OR GETDATE() < DATEADD(DAY, 0, ENDDATE) ) ";
            string _sql = $"SELECT * FROM [TIME_MACHINE] WHERE [TIMEMACHINEID] = @TIMEMACHINEID {adminQuery} ";

            timeMachine = _dapperHelper.QuerySqlFirstOrDefault(_sql, timeMachine);

            //加上http網址
            if (timeMachine != null)
            {
                if (timeMachine.navPics01 !=null)
                {
                    timeMachine.navPics01 = Tools.GetInstance().GetImageLink(Tools.GetInstance().timeMachine + $@"/{timeMachine.id}", timeMachine.navPics01);
                }
                if (timeMachine.navPics02 != null)
                {
                    timeMachine.navPics02 = Tools.GetInstance().GetImageLink(Tools.GetInstance().timeMachine + $@"/{timeMachine.id}", timeMachine.navPics02);
                }
            }

            return timeMachine;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateTimeMachine(HttpRequest request, TimeMachine timeMachine)
        {
            timeMachine = RequestDataMod(request, timeMachine);   //將接收來的參數，和抓到的要修改資料合併

            //1.設定圖片檔名
            SetImageFileName(timeMachine, request.Files);

            //2.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().timeMachine);
            string _saveFolderPath = _imageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{timeMachine.id}\");

            //3.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            _imageFileHelper.SaveMoreUploadImageFile(request.Files, _saveFolderPath);

            //4.組裝修改刪除不變更時sql
            string _sqlpic = picSql(timeMachine, request);

            string _sql = $@"UPDATE [TIME_MACHINE]
                                           SET
                                               [NAME] = @NAME
                                              ,[COURSE] = @COURSE
                                              ,[BRIEF] = @BRIEF
                                              ,[FIRST] = @FIRST
                                              ,[SORT] = @SORT
                                              ,[ENABLED] = @ENABLED
                                               {_sqlpic}
                                              ,[STARTDATE] = @STARTDATE
                                              ,[ENDDATE] = @ENDDATE
                                              ,[UPDATEDDATE] = @UPDATEDDATE
                                         WHERE [TIMEMACHINEID] = @TIMEMACHINEID ";

            if (timeMachine.startDate == DateTime.MinValue)
            {
                _sql = _sql.Replace("@STARTDATE", "NULL");
            }
            if (timeMachine.endDate == DateTime.MinValue)
            {
                _sql = _sql.Replace("@ENDDATE", "NULL");
            }
            _dapperHelper.ExecuteSql(_sql, timeMachine);
        }
        /// <summary>
        /// 讀取表單資料,轉到timeMachine
        /// </summary>
        /// <param name="request">讀取表單資料(修改時用)</param>
        /// <param name="timeMachine">控制器取到資料</param>
        /// <returns>timeMachine</returns>
        private TimeMachine RequestDataMod(HttpRequest request, TimeMachine original_timeMachine)
        {
            //original_timeMachine是controller抓資料庫的資料
            TimeMachine timeMachine = new TimeMachine();               //設定一組空的沒資料會null, 讓圖片預設是null不執行SQL指令，此null是類別裏真的null 
            timeMachine.id = original_timeMachine.id;                           //將資料庫id傳給空的timeMachine
            timeMachine.timeMachineId = original_timeMachine.timeMachineId;                //將資料庫slideId傳給空的timeMachine

            timeMachine.name = request.Form["name"];               //時間標題2002 12月
            timeMachine.course = Convert.ToDateTime(request.Form["course"]);        //副標題
            timeMachine.brief = request.Form["brief"];                                                    //簡述
            timeMachine.first = Convert.ToBoolean(Convert.ToByte(request.Form["first"]));                 //是否置頂(0 關閉;1 開啟)
            timeMachine.sort = Convert.ToInt32(request.Form["sort"]);                                                    //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            timeMachine.enabled = Convert.ToBoolean(Convert.ToByte(request.Form["enabled"]));   //是否啟用(0 關閉;1 開啟)

            //上架日, startDate Postman進來, 會是空值或沒有此欄位或有值，有值以外兩個條件都不會成立，所以startDate沒收到會是null
            if (Tools.Formatter.IsDate(request.Form["startDate"]))
            {
                timeMachine.startDate = Convert.ToDateTime(request.Form["startDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                timeMachine.startDate = DateTime.MinValue;
            }
            //下架日
            if (Tools.Formatter.IsDate(request.Form["endDate"]))
            {
                timeMachine.endDate = Convert.ToDateTime(request.Form["endDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                timeMachine.endDate = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
            }
            timeMachine.updatedDate = DateTime.Now;

            return timeMachine;
        }
        #endregion

        #region 刪除一筆資料
        public void DeleteTimeMachine(TimeMachine timeMachine)
        {
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().timeMachine);

            string folder = Path.Combine(_RootPath_ImageFolderPath, timeMachine.id.ToString());
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            string _sql = @"DELETE FROM [TIME_MACHINE] WHERE [TIMEMACHINEID] = @TIMEMACHINEID";
            //執行刪除
            _dapperHelper.ExecuteSql(_sql, timeMachine);
        }
        #endregion

        #region 取得所有資料
        public List<TimeMachine> GetTimeMachines(int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY

            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }

            string adminQuery = Auth.Role.IsAdmin ? "" : " WHERE ENABLED = 1 AND (STARTDATE IS NULL OR STARTDATE <= GETDATE() ) AND (ENDDATE IS NULL OR GETDATE() < DATEADD(DAY, 0, ENDDATE) ) ";
            string _sql = $"SELECT * FROM [TIME_MACHINE]  {adminQuery} " +
                                    @" ORDER BY [FIRST] DESC , CREATIONDATE DESC, SORT" +
                                    $" {page_sql} ";
            List<TimeMachine> timeMachines = _dapperHelper.QuerySetSql<TimeMachine>(_sql).ToList();

            //將資料庫圖檔加上URL
            for (int i = 0; i < timeMachines.Count; i++)
            {
                if (timeMachines[i].navPics01 != null)
                {
                    timeMachines[i].navPics01 = Tools.GetInstance().GetImageLink(Tools.GetInstance().timeMachine + $@"/{timeMachines[i].id}", timeMachines[i].navPics01);
                }
                if (timeMachines[i].navPics02 != null)
                {
                    timeMachines[i].navPics02 = Tools.GetInstance().GetImageLink(Tools.GetInstance().timeMachine + $@"/{timeMachines[i].id}", timeMachines[i].navPics02);
                }
            }
            return timeMachines;
        }
        #endregion


        /// <summary>
        /// 設定上傳檔案的檔名對應相同的類別名稱
        /// </summary>
        /// <param name="timeMachine">類別</param>
        /// <param name="fileCollection">httpRequest.Files</param>
        /// https://docs.microsoft.com/zh-tw/dotnet/api/system.web.httprequest.files?view=netframework-4.7.2&f1url=%3FappId%3DDev16IDEF1%26l%3DZH-TW%26k%3Dk(System.Web.HttpRequest.Files);k(TargetFrameworkMoniker-.NETFramework,Version%253Dv4.7.2);k(DevLang-csharp)%26rd%3Dtrue
        private void SetImageFileName(TimeMachine timeMachine, HttpFileCollection fileCollection)
        {
            // Files.AllKeys 這會將所有文件的名稱放入一個字符串數組中。
            //keyName (navPics01，navPics02) = AllKeys 取得字串陣列，包含檔案集合中所有成員的索引鍵 (名稱)。
            foreach (string keyName in fileCollection.AllKeys)
            {
                HttpPostedFile file = fileCollection[keyName];
                if (file != null && file.ContentLength > 0)
                {
                    switch (keyName.ToLower())
                    {
                        case "navpics01":
                            timeMachine.navPics01 = file.FileName;
                            break;
                        case "navpics02":
                            timeMachine.navPics02 = file.FileName;
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// 組裝修改刪除不變更時sql
        /// </summary>
        /// <param name="timeMachine"></param>
        /// <param name="_request"></param>
        /// <returns></returns>
        private string picSql(TimeMachine timeMachine, HttpRequest _request)
        {
            string pic = string.Empty;
            //var null = _request.Form["Picture01"];
            //圖片更新有4種狀況
            //1.新增:選圖片上傳_request.Files;AllKeys就會有欄位,檔名就會代給類別_pictureList.Picture01 = _file.FileName
            //因為Picture01欄位有圖檔, 所以_request.Form["Picture01"]就不見變成null, 但下面的null並不是真的null, 而時字串的"null"
            //所以新增時以下左邊會成立，右邊不成立
            //2.修改:有重新上傳的_request.Files,AllKeys就會有欄位會有值, 
            //3.沒有變更:沒有重新上傳檔案,但在_request.Form[""] = xx.PNG會接到現有的檔案名稱，所以下面條件都不成立
            //3.按垃圾筒,並沒有上傳Files內容，而是傳回表單字串null(_request.Form["xx"] == "null"，所以會執行下面右方sql將資料欄改NULL
            //4.完全沒變動:會變成_request.Form的值, 原本有圖檔的如_request.Form["Picture01"]=01.PNG, 沒有圖檔的就會是空值，下面條件都不成立
            if (timeMachine.navPics01 != null) pic += ",[navPics01]=@navPics01"; if (_request.Form["navPics01"] == "null") pic += ",[navPics01]=NULL";
            if (timeMachine.navPics02 != null) pic += ",[navPics02]=@navPics02"; if (_request.Form["navPics02"] == "null") pic += ",[navPics02]=NULL";

            return pic;
        }

    }
}