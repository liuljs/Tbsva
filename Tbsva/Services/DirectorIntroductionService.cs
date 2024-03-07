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
    public class DirectorIntroductionService : IDirectorIntroductionService
    {
        #region DI依賴注入功能
        private IDapperHelper _dapperHelper;
        private IImageFileHelper _imageFileHelper;
        public DirectorIntroductionService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            _dapperHelper = dapperHelper;
            _imageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        public DirectorIntroduction InsertDirectorIntroduction(HttpRequest request)
        {
            DirectorIntroduction directorIntroduction = RequestData(request);

            string _sql = @" INSERT INTO [DIRECTOR_INTRODUCTION]
                                                    ([DIRECTORINTRODUCTIONID]
                                                    ,[IMAGENAME]
                                                    ,[NAME]
                                                    ,[SUBTITLE]
                                                    ,[BRIEF]
                                                    ,[CONTENT]
                                                    ,[FIRST]
                                                    ,[SORT]
                                                    ,[ENABLED]
                                                    ,[STARTDATE]
                                                    ,[ENDDATE]
                                                    ,[CREATIONDATE] )
                                                VALUES
                                                    (@DIRECTORINTRODUCTIONID,
                                                     @IMAGENAME, 
                                                     @NAME, 
                                                     @SUBTITLE,
                                                     @BRIEF,
                                                     @CONTENT,
                                                     @FIRST,
                                                     @SORT,
                                                     @ENABLED, 
                                                     @STARTDATE, 
                                                     @ENDDATE,
                                                     @CREATIONDATE )
                                                         SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            if (directorIntroduction.startDate == DateTime.MinValue)
            {
                _sql = _sql.Replace("@STARTDATE", "NULL");
            }
            if (directorIntroduction.endDate == DateTime.MinValue)
            {
                _sql = _sql.Replace("@ENDDATE", "NULL");
            }
            int id = _dapperHelper.QuerySingle(_sql, directorIntroduction);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            directorIntroduction.id = id;   //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                directorIntroduction.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                directorIntroduction.sort = Convert.ToInt32(request.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [DIRECTOR_INTRODUCTION]
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            _dapperHelper.ExecuteSql(_sql, directorIntroduction);         //更新排序

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DirectorIntroduction);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            _imageFileHelper.SaveUploadImageFile(request.Files[0], directorIntroduction.imageName, _RootPath_ImageFolderPath);

            //3.1將編輯內容圖片裏剛產生的director_introduction_image表裏的圖檔尚未關連此directorIntroductionId更新進去
            HandleContentImages(request.Form["cNameArr"], _RootPath_ImageFolderPath, directorIntroduction.directorIntroductionId);

            //4.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);

            return directorIntroduction;
        }
        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="request">讀取表單資料</param>
        /// <returns>回傳表單類別</returns>
        private DirectorIntroduction RequestData(HttpRequest request)
        {
            DirectorIntroduction directorIntroduction = new DirectorIntroduction();
            directorIntroduction.directorIntroductionId = Guid.NewGuid();                                                           //類別另設 ID 唯一索引用
            directorIntroduction.imageName = directorIntroduction.directorIntroductionId + ".png";              //圖片檔名使用id來當
            directorIntroduction.name = request.Form["name"];                                                                             //標題
            directorIntroduction.subtitle = request.Form["subtitle"];                                                                      //副標題
            directorIntroduction.brief = request.Form["brief"];                                                                                //簡述
            directorIntroduction.content = request.Form["content"];                                                                      //內容
            directorIntroduction.first = Convert.ToBoolean(Convert.ToByte(request.Form["first"]));                 //是否置頂(0 關閉;1 開啟)
            directorIntroduction.sort = Convert.ToInt32(request.Form["sort"]);                                                    //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            directorIntroduction.enabled = Convert.ToBoolean(Convert.ToByte(request.Form["enabled"]));   //是否啟用(0 關閉;1 開啟)

            //上架日, startDate Postman進來, 會是空值或沒有此欄位或有值，有值以外兩個條件都不會成立，所以startDate沒收到會是null
            if (Tools.Formatter.IsDate(request.Form["startDate"]))
            {
                directorIntroduction.startDate = Convert.ToDateTime(request.Form["startDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                directorIntroduction.startDate = DateTime.MinValue;
            }
            //下架日
            if (Tools.Formatter.IsDate(request.Form["endDate"]))
            {
                directorIntroduction.endDate = Convert.ToDateTime(request.Form["endDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                directorIntroduction.endDate = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
            }
            directorIntroduction.creationDate = DateTime.Now;

            return directorIntroduction;
        }
        #endregion

        #region 取得一筆資料
        public DirectorIntroduction GetDirectorIntroduction(Guid directorIntroductionId)
        {
            DirectorIntroduction directorIntroduction = new DirectorIntroduction();
            directorIntroduction.directorIntroductionId = directorIntroductionId;    //另設 ID
            //2021 / 11 / 30   2021-12-01 13:59:01  2021 / 12 / 02
            //Start_Date <= getdate() < End_Date +1 End_Date 是當天上午12:00 => 00:00
            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 AND (STARTDATE IS NULL OR STARTDATE <= GETDATE() ) AND (ENDDATE IS NULL OR GETDATE() < DATEADD(DAY, 0, ENDDATE) ) ";
            string _sql = $"SELECT * FROM [DIRECTOR_INTRODUCTION] WHERE [DIRECTORINTRODUCTIONID] = @DIRECTORINTRODUCTIONID {adminQuery}";
            directorIntroduction = _dapperHelper.QuerySqlFirstOrDefault(_sql, directorIntroduction);

            //加上http網址
            if (directorIntroduction !=null)
            {
                directorIntroduction.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().DirectorIntroduction, directorIntroduction.imageName);
            }
            return directorIntroduction;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateDirectorIntroduction(HttpRequest request, DirectorIntroduction directorIntroduction)
        {
            directorIntroduction = RequestDataMod(request, directorIntroduction);//將接收來的參數，和抓到的要修改資料合併

            #region 辨斷有沒有上傳檔案
            if (request.Files.Count > 0) //1檢查有沒有上傳圖片的欄位選項
            {
                if (request.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳
                {
                    //以下處理圖片
                    //1. 取得要放置的目錄路徑
                    string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DirectorIntroduction);

                    //  裏面的imageName有加網址ImageFileHelper.GetImageLink，改只取檔案名與副檔
                    string _image_name = Path.GetFileName(directorIntroduction.imageName);

                    //2.存放上傳封面圖片(1實體檔,2檔名,3路徑)
                    _imageFileHelper.SaveUploadImageFile(request.Files[0], _image_name, _RootPath_ImageFolderPath);
                }
            }
            //將在控制器上取出的資料庫裏的有加http://..最後檔名取出，不然資料庫更新時會出錯超出欄位大小
            directorIntroduction.imageName = Path.GetFileName(directorIntroduction.imageName);
            #endregion

            string _sql = @"UPDATE [DIRECTOR_INTRODUCTION]
                                           SET
                                               [IMAGENAME] = @IMAGENAME
                                              ,[NAME] = @NAME
                                              ,[SUBTITLE] = @SUBTITLE
                                              ,[BRIEF] = @BRIEF
                                              ,[CONTENT] = @CONTENT
                                              ,[FIRST] = @FIRST
                                              ,[SORT] = @SORT
                                              ,[ENABLED] = @ENABLED
                                              ,[STARTDATE] = @STARTDATE
                                              ,[ENDDATE] = @ENDDATE
                                              ,[UPDATEDDATE] = @UPDATEDDATE
                                         WHERE [DIRECTORINTRODUCTIONID] = @DIRECTORINTRODUCTIONID ";
            if (directorIntroduction.startDate == DateTime.MinValue)
            {
                _sql = _sql.Replace("@STARTDATE", "NULL");
            }
            if (directorIntroduction.endDate == DateTime.MinValue)
            {
                _sql = _sql.Replace("@ENDDATE", "NULL");
            }
            _dapperHelper.ExecuteSql(_sql, directorIntroduction);
        }
        /// <summary>
        /// 讀取表單資料
        /// </summary>
        /// <param name="request">讀取表單資料(修改時用)</param>
        /// <param name="directorIntroduction">控制器取到資料</param>
        /// <returns></returns>
        private DirectorIntroduction RequestDataMod(HttpRequest request, DirectorIntroduction directorIntroduction)
        {
            directorIntroduction.name = request.Form["name"];                                                                             //標題
            directorIntroduction.subtitle = request.Form["subtitle"];                                                                      //副標題
            directorIntroduction.brief = request.Form["brief"];                                                                                //簡述
            directorIntroduction.content = request.Form["content"];                                                                      //內容
            directorIntroduction.first = Convert.ToBoolean(Convert.ToByte(request.Form["first"]));                 //是否置頂(0 關閉;1 開啟)
            directorIntroduction.sort = Convert.ToInt32(request.Form["sort"]);                                                    //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            directorIntroduction.enabled = Convert.ToBoolean(Convert.ToByte(request.Form["enabled"]));   //是否啟用(0 關閉;1 開啟)

            //上架日, startDate Postman進來, 會是空值或沒有此欄位或有值，有值以外兩個條件都不會成立，所以startDate沒收到會是null
            if (Tools.Formatter.IsDate(request.Form["startDate"]))
            {
                directorIntroduction.startDate = Convert.ToDateTime(request.Form["startDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                directorIntroduction.startDate = DateTime.MinValue;
            }
            //下架日
            if (Tools.Formatter.IsDate(request.Form["endDate"]))
            {
                directorIntroduction.endDate = Convert.ToDateTime(request.Form["endDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                directorIntroduction.endDate = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
            }
            directorIntroduction.updatedDate = DateTime.Now;

            return directorIntroduction;
        }
        #endregion

        #region 刪除一筆資料
        public void DeleteDirectorIntroduction(DirectorIntroduction directorIntroduction)
        {
            //1. 取得放置圖片的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DirectorIntroduction);

            //2.取得這筆內容下相關內容圖片檔名清單
            string _sql = @"SELECT [IMAGENAME] FROM [DIRECTOR_INTRODUCTION_IMAGE] WHERE DIRECTORINTRODUCTIONID = @DIRECTORINTRODUCTIONID";

            //調出此筆內容下的多筆圖片名稱，到List<string>
            List<string> _image_name_list = _dapperHelper.QuerySetSql<DirectorIntroduction, string>(_sql, directorIntroduction).ToList();

            //刪除內容下內容圖, List<string> _image_name_list
            string _image_name_list_path = string.Empty;  //宣告一個變數之後要加入路徑
            for (int i = 0; i < _image_name_list.Count; i++)
            {
                //路徑與圖檔結合, 再辨斷存在就刪除
                _image_name_list_path = Path.Combine(_RootPath_ImageFolderPath, _image_name_list[i]);
                if (File.Exists(_image_name_list_path))
                {
                    File.Delete(_image_name_list_path);
                }
            }
            //3.刪除內容封面
            //取出內容圖片檔案
            string _image_name = Path.GetFileName(directorIntroduction.imageName);   //取出內容封面檔名
            string _image_name_FilePath = Path.Combine(_RootPath_ImageFolderPath, _image_name);  //封面檔名加上路徑
            if (File.Exists(_image_name_FilePath))
            {
                File.Delete(_image_name_FilePath);
            }

            //4.刪除資料庫內容 資料庫有設定重疊顯示，所以刪主鍵關連director_introduction_image資料會一同刪除FK_director_introduction_image_director_introduction
            _sql = @"DELETE FROM [DIRECTOR_INTRODUCTION] WHERE DIRECTORINTRODUCTIONID = @DIRECTORINTRODUCTIONID";
            //執行刪除
            _dapperHelper.ExecuteSql(_sql, directorIntroduction);
        }
        #endregion

        #region 取得所有資料
        public List<DirectorIntroduction> GetDirectorIntroductions(int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY

            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }

            string adminQuery = Auth.Role.IsAdmin ? "" : " WHERE ENABLED = 1 AND (STARTDATE IS NULL OR STARTDATE <= GETDATE() ) AND (ENDDATE IS NULL OR GETDATE() < DATEADD(DAY, 0, ENDDATE) ) ";
            string _sql = $"SELECT * FROM [DIRECTOR_INTRODUCTION]  {adminQuery} " +
                                  @" ORDER BY [FIRST] DESC , CREATIONDATE DESC , SORT " +
                                  $" {page_sql} ";
            List<DirectorIntroduction> directorIntroductions = _dapperHelper.QuerySetSql<DirectorIntroduction>(_sql).ToList();
            //轉換image_name加上url
            for (int i = 0; i < directorIntroductions.Count; i++)
            {
                directorIntroductions[i].imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().DirectorIntroduction, directorIntroductions[i].imageName);
            }

            return directorIntroductions;
        }
        #endregion

        #region 新增編輯器圖片
        /// <summary>
        /// 新增編輯器圖片
        /// </summary>
        /// <param name="request"></param>
        /// <returns>directorIntroductionImage</returns>
        public DirectorIntroductionImage AddUploadImage(HttpRequest request)
        {
            DirectorIntroductionImage directorIntroductionImage = new DirectorIntroductionImage();
            directorIntroductionImage.directorIntroductionImageId = Guid.NewGuid();                                         //主索引鍵（P.K.）自動產生一個Guid
            directorIntroductionImage.imageName = directorIntroductionImage.directorIntroductionImageId + ".png";
            //directorIntroductionImage.directorIntroductionId = "關連內容directorIntroductionId尚未產生，進入新增狀態時尚未送出所以沒有值";

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DirectorIntroduction);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            _imageFileHelper.SaveUploadImageFile(request.Files[0], directorIntroductionImage.imageName , _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [DIRECTOR_INTRODUCTION_IMAGE]
                                            ([DIRECTORINTRODUCTIONIMAGEID]
                                            ,[IMAGENAME])
                                        VALUES
                                            (@DIRECTORINTRODUCTIONIMAGEID,
                                             @IMAGENAME )
                                                SELECT SCOPE_IDENTITY()";
            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _dapperHelper.QuerySingle(_sql, directorIntroductionImage);        //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            directorIntroductionImage.id = id; //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            directorIntroductionImage.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().DirectorIntroduction, directorIntroductionImage.imageName);

            return directorIntroductionImage;
        }

        /// <summary>
        /// 新增編輯器圖片(修改時使用)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="directorIntroductionId"></param>
        /// <returns>directorIntroductionImage</returns>
        public DirectorIntroductionImage AddUploadImage(HttpRequest request, Guid directorIntroductionId)
        {
            DirectorIntroductionImage directorIntroductionImage = new DirectorIntroductionImage();
            directorIntroductionImage.directorIntroductionImageId = Guid.NewGuid();          //主索引鍵（P.K.）自動產生一個Guid
            directorIntroductionImage.imageName = directorIntroductionImage.directorIntroductionImageId + ".png";
            directorIntroductionImage.directorIntroductionId = directorIntroductionId;          //關聯內容directorIntroductionId

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DirectorIntroduction);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            _imageFileHelper.SaveUploadImageFile(request.Files[0], directorIntroductionImage.imageName, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [DIRECTOR_INTRODUCTION_IMAGE]
                                            ([DIRECTORINTRODUCTIONIMAGEID]
                                            ,[IMAGENAME]
                                            ,[DIRECTORINTRODUCTIONID])
                                        VALUES
                                            (@DIRECTORINTRODUCTIONIMAGEID,
                                             @IMAGENAME, 
                                             @DIRECTORINTRODUCTIONID )
                                                SELECT SCOPE_IDENTITY()";
            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _dapperHelper.QuerySingle(_sql, directorIntroductionImage);        //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            directorIntroductionImage.id = id; //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            directorIntroductionImage.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().DirectorIntroduction, directorIntroductionImage.imageName);

            return directorIntroductionImage;
        }
        #endregion

        #region 工具區
        /// <summary>
        /// 在新增內容時，下面內文若有插入圖片，插入的圖片還沒有directorIntroductionId, 送出前cNameArr收集了內容裏插入了多少張圖
        /// 然後在送出後在將確定有插入的內容圖片更新
        /// 為何不先給插入圖片id，因為插入圖片隨時可能又del，當del掉後若已給id就無法用null來辨斷刪除
        /// </summary>
        /// <param name="images">httpRequest.Form["cNameArr"]內容圖片的檔名JSON字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="Id">關連內容的directorIntroductionId</param>
        private void HandleContentImages(string images, string saveFolderPath, Guid Id)
        {
            //JsonConvert.DeserializeObject<類別>("字串")，<>中定義類別，將JSON字串反序列化成物件
            //JSON字串
            //string Json = "{ 'Name': 'Berry', 'Age': 18, 'Marry': false, 'Habit': [ 'Sing','Dance','Code','Sleep' ] }";
            //轉成物件
            //Introduction Introduction = JsonConvert.DeserializeObject<Introduction>(Json);
            //顯示
            //Response.Write(Introduction.Name);

            //cNameArr沒有欄位只有值_retFileNameList[i]
            //取得回傳內容圖片的檔名cNameArr["abc.png","def.png"]， Json反序列化(Deserialize)為物件(Object)
            List<string> _retFileNameList = JsonConvert.DeserializeObject<List<string>>(images);

            //取得存放saveFolderPath目錄下所有.png圖檔
            //GetFiles (string path, string searchPattern);
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png
            if (_retFileNameList != null && _retFileNameList.Count > 0)    //cNameArr確認有檔才處理
            {
                for (int i = 0; i < _retFileNameList.Count; i++) //處理裏面的所有檔cNameArr ["abc.png","def.png"]
                {
                    for (int y = 0; y < _imageFileList.Count; y++)  //cNameArr和目錄下實際所有.png圖檔比對一下
                    {
                        string _urlFileName = _retFileNameList[i].ToLower().Trim();   //處理cNameArr內的檔轉小寫去空白
                        string _entityFileName = Path.GetFileName(_imageFileList[y]);  //目錄下的檔, 只需留檔案名稱與副檔名
                        string _imageGuid = Path.GetFileNameWithoutExtension(_imageFileList[y]);  //只留下檔案名稱，路徑與副檔名都去除
                        //判斷cNameArr[]傳進來的參數圖檔==目前目錄下的檔&&判斷目錄下的檔案是否存在。
                        if (_urlFileName == _entityFileName && File.Exists(_imageFileList[y]))
                        {
                            DirectorIntroductionImage directorIntroductionImage = new DirectorIntroductionImage();
                            directorIntroductionImage.directorIntroductionImageId = Guid.Parse(_imageGuid);        //存放目錄下檔名不含副檔給director_introduction_image表裏directorIntroductionImageId
                            directorIntroductionImage.directorIntroductionId = Id;                                        //關聯[directorIntroductionId]

                            //更新director_introduction內容的directorIntroductionId給director_introduction_image.directorIntroductionId 連立關連
                            string _sql = @"UPDATE [DIRECTOR_INTRODUCTION_IMAGE]
                                                           SET [DIRECTORINTRODUCTIONID] =@DIRECTORINTRODUCTIONID
                                                         WHERE DIRECTORINTRODUCTIONIMAGEID = @DIRECTORINTRODUCTIONIMAGEID ";
                            _dapperHelper.ExecuteSql(_sql, directorIntroductionImage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刪除[directorIntroductionId] IS NULL不用的內容圖片
        /// </summary>
        /// <param name="saveFolderPath"></param>
        private void RemoveContentImages(string saveFolderPath)
        {
            //取得存放磁碟目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png

            //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
            string _sql = @"SELECT [IMAGENAME] FROM [DIRECTOR_INTRODUCTION_IMAGE] WHERE DIRECTORINTRODUCTIONID IS NULL";
            List<string> _imageDataBaseList = _dapperHelper.QuerySetSql<string>(_sql).ToList();

            //刪除多餘的內容圖檔, _imageDataBaseList 資料庫檔案名清單，_imageFileList 實際目錄檔路徑檔案清單
            for (int y = 0; y < _imageDataBaseList.Count; y++)
            {
                for (int i = 0; i < _imageFileList.Count; i++)
                {
                    string _removeFileName = Path.GetFileName(_imageFileList[i]);   //除掉路徑留檔案名稱

                    //資料庫directorIntroductionId=NULL的檔案名 == 資料匣存放的檔名&&存放檔實際是否存在
                    if (_imageDataBaseList[y] == _removeFileName && File.Exists(_imageFileList[i]))
                    {
                        File.Delete(_imageFileList[i]);
                    }
                }
            }
            //刪除資料庫裡無相關連的內容圖片記錄
            _sql = @"DELETE FROM [DIRECTOR_INTRODUCTION_IMAGE] WHERE [DIRECTORINTRODUCTIONID] IS NULL";

            _dapperHelper.ExecuteSql(_sql);
        }
        #endregion

    }
}