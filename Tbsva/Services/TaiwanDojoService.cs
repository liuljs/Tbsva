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
    public class TaiwanDojoService : ITaiwanDojoService
    {
        #region DI依賴注入功能
        private IDapperHelper _dapperHelper;
        private IImageFileHelper _imageFileHelper;
        public TaiwanDojoService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            _dapperHelper = dapperHelper;
            _imageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        public TaiwanDojo InsertTaiwanDojo(HttpRequest request)
        {
            TaiwanDojo taiwanDojo = RequestData(request);
            string _sql = @" INSERT INTO [TAIWAN_DOJO]
                                                    ([CATEGORYID]
                                                    ,[TAIWANDOJOID]
                                                    ,[NAME]
                                                    ,[LATITUDE]
                                                    ,[LONGITUDE]
                                                    ,[BRIEF]
                                                    ,[AREA]
                                                    ,[ADDRESS]
                                                    ,[VIDEO]
                                                    ,[CONTENT]
                                                    ,[ICON]
                                                    ,[FIRST]
                                                    ,[SORT]
                                                    ,[ENABLED]
                                                    ,[CREATIONDATE] )
                                                VALUES
                                                    (@CATEGORYID,
                                                     @TAIWANDOJOID,
                                                     @NAME,
                                                     @LATITUDE,
                                                     @LONGITUDE,
                                                     @BRIEF,
                                                     @AREA,
                                                     @ADDRESS,
                                                     @VIDEO,
                                                     @CONTENT,
                                                     @ICON,
                                                     @FIRST,
                                                     @SORT,
                                                     @ENABLED, 
                                                     @CREATIONDATE )
                                                         SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。

            int id = _dapperHelper.QuerySingle(_sql, taiwanDojo);
            taiwanDojo.id = id;
            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                taiwanDojo.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                taiwanDojo.sort = Convert.ToInt32(request.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [TAIWAN_DOJO] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            _dapperHelper.ExecuteSql(_sql, taiwanDojo);

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Taiwandojo);

            //2.將編輯內容圖片裏剛產生的taiwan_dojo_image表裏的圖檔尚未關連此taiwanDojoId更新進去
            HandleContentImages(request.Form["cNameArr"], _RootPath_ImageFolderPath, taiwanDojo.taiwanDojoId);

            //3.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);

            return taiwanDojo;
        }
        private TaiwanDojo RequestData(HttpRequest request)
        {
            TaiwanDojo taiwanDojo = new TaiwanDojo();
            taiwanDojo.categoryId = new Guid(request.Form["categoryId"]);                                           //隸屬目錄id
            taiwanDojo.taiwanDojoId = Guid.NewGuid();                                                                          //內容的id
            taiwanDojo.name = request.Form["name"];                                                                              //名稱
            taiwanDojo.latitude = request.Form["latitude"];                                                                      //經度
            taiwanDojo.longitude = request.Form["longitude"];                                                               //緯度
            taiwanDojo.brief = request.Form["brief"];                                                                                //簡述
            taiwanDojo.area = request.Form["area"];                                                                                    //地區
            taiwanDojo.address = request.Form["address"];                                                                        //地址
            taiwanDojo.video = request.Form["video"];                                                                                //地圖
            taiwanDojo.content = request.Form["content"];                                                                        //內容資訊
            taiwanDojo.icon = Convert.ToBoolean(Convert.ToByte(request.Form["icon"]));                   //小圖示開關(0 關閉;1 開啟)
            taiwanDojo.first = Convert.ToBoolean(Convert.ToByte(request.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            taiwanDojo.sort = Convert.ToInt32(request.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            taiwanDojo.enabled = Convert.ToBoolean(Convert.ToByte(request.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            taiwanDojo.creationDate = DateTime.Now;

            string _sql = @" SELECT [NAME] FROM [TAIWAN_DOJO_CATEGORY] WHERE [CATEGORYID] = @CATEGORYID";
            TaiwanDojoCategory taiwanDojoCategory = new TaiwanDojoCategory();
            taiwanDojoCategory.categoryId = taiwanDojo.categoryId;               //取得隸屬目錄Guid傳給要查詢的目錄categoryId
            taiwanDojoCategory = _dapperHelper.QuerySqlFirstOrDefault(_sql, taiwanDojoCategory);  //查詢
            taiwanDojo.categoryName = taiwanDojoCategory.name;  //將查到的目錄名傳給TaiwanDojo類別

            return taiwanDojo;
        }
        #endregion

        #region 取得一筆資料
        public TaiwanDojo GetTaiwanDojo(Guid taiwanDojoId)
        {
            TaiwanDojo taiwanDojo = new TaiwanDojo();
            taiwanDojo.taiwanDojoId = taiwanDojoId;                 //內容的id

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 ";
            string _sql = $"SELECT * FROM [TAIWAN_DOJO] WHERE [TAIWANDOJOID] = @TAIWANDOJOID {adminQuery}";
            taiwanDojo = _dapperHelper.QuerySqlFirstOrDefault(_sql, taiwanDojo);

            //調出目錄名稱
            if (taiwanDojo != null)
            {
                _sql = @"SELECT [NAME] FROM [TAIWAN_DOJO_CATEGORY] WHERE [CATEGORYID] = @CATEGORYID";
                TaiwanDojoCategory taiwanDojoCategory = new TaiwanDojoCategory();
                taiwanDojoCategory.categoryId = taiwanDojo.categoryId;               //取得的隸屬目錄Guid傳給要查詢的目錄categoryId
                taiwanDojoCategory = _dapperHelper.QuerySqlFirstOrDefault(_sql, taiwanDojoCategory);  //查詢
                taiwanDojo.categoryName = taiwanDojoCategory.name;  //將查到的目錄名傳給TaiwanDojo類別
            }
            return taiwanDojo;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateTaiwanDojo(HttpRequest request, TaiwanDojo taiwanDojo)
        {
            taiwanDojo = RequestDataMod(request, taiwanDojo);        //將接收來的taiwanDojo參數，和抓到的要修改資料合併

            string _sql = @"UPDATE [TAIWAN_DOJO]
                                           SET
                                               [CATEGORYID] = @CATEGORYID
                                              ,[NAME] = @NAME
                                              ,[LATITUDE] = @LATITUDE
                                              ,[LONGITUDE] = @LONGITUDE
                                              ,[BRIEF] = @BRIEF
                                              ,[AREA] = @AREA
                                              ,[ADDRESS] = @ADDRESS
                                              ,[VIDEO] = @VIDEO
                                              ,[CONTENT] = @CONTENT
                                              ,[ICON] = @ICON
                                              ,[FIRST] = @FIRST
                                              ,[SORT] = @SORT
                                              ,[ENABLED] = @ENABLED
                                              ,[UPDATEDDATE] = @UPDATEDDATE
                                         WHERE [TAIWANDOJOID] = @TAIWANDOJOID ";
            _dapperHelper.ExecuteSql(_sql, taiwanDojo);
        }
        /// <summary>
        /// 讀取表單資料,轉到類別
        /// </summary>
        /// <param name="request">讀取表單資料(修改時用)</param>
        /// <param name="taiwanDojo"></param>
        /// <returns></returns>
        private TaiwanDojo RequestDataMod(HttpRequest request, TaiwanDojo taiwanDojo)
        {
            taiwanDojo.categoryId = new Guid(request.Form["categoryId"]);                                           //隸屬目錄id
            taiwanDojo.name = request.Form["name"];                                                                               //名稱
            taiwanDojo.latitude = request.Form["latitude"];                                                                       //經度
            taiwanDojo.longitude = request.Form["longitude"];                                                                 //緯度
            taiwanDojo.brief = request.Form["brief"];                                                                                 //簡述
            taiwanDojo.area = request.Form["area"];                                                                                    //地區
            taiwanDojo.address = request.Form["address"];                                                                        //地址
            taiwanDojo.video = request.Form["video"];                                                                                //地圖
            taiwanDojo.content = request.Form["content"];                                                                        //內容資訊，
            taiwanDojo.icon = Convert.ToBoolean(Convert.ToByte(request.Form["icon"]));                   //小圖示開關(0 關閉;1 開啟)
            taiwanDojo.first = Convert.ToBoolean(Convert.ToByte(request.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            taiwanDojo.sort = Convert.ToInt32(request.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            taiwanDojo.enabled = Convert.ToBoolean(Convert.ToByte(request.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            taiwanDojo.updatedDate = DateTime.Now;

            return taiwanDojo;
        }
        #endregion

        #region 刪除
        public void DeleteTaiwanDojo(TaiwanDojo taiwanDojo)
        {
            //1. 取得放置圖片的目錄路徑
            string rootPathImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Taiwandojo);

            //取得這筆內容下相關內容圖片檔名清單
            string _sql = @"SELECT [IMAGENAME] FROM [TAIWAN_DOJO_IMAGE] WHERE [TAIWANDOJOID] = @TAIWANDOJOID";
            //調出此筆內容下的多筆圖片名稱，到List<string>
            List<string> imageNameList = _dapperHelper.QuerySetSql<TaiwanDojo, string>(_sql, taiwanDojo).ToList();

            //刪除內容下內容圖, List<string> imageNameList
            string imageNameListPath = string.Empty;  //宣告一個變數之後要加入路徑
            for (int i = 0; i < imageNameList.Count; i++)
            {
                //路徑與圖檔結合, 再辨斷存在就刪除
                imageNameListPath = Path.Combine(rootPathImageFolderPath, imageNameList[i]);
                if (File.Exists(imageNameListPath))
                {
                    File.Delete(imageNameListPath);
                }
            }

            //刪除內容封面(無封面圖)
            //刪除資料庫內容 資料庫有設定重疊顯示，所以刪主鍵關連[taiwan_dojo_image資料會一同刪除FK_taiwan_dojo_image_taiwan_dojo
            _sql = @"DELETE FROM [TAIWAN_DOJO] WHERE [TAIWANDOJOID] = @TAIWANDOJOID";

            //執行刪除
            _dapperHelper.ExecuteSql(_sql, taiwanDojo);
        }
        #endregion

        #region 取得分頁活動管理
        public List<TaiwanDojo> GetTaiwanDojos(string categoryId, int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string categoryId_sql = null;         // 隸屬目錄 ID  sql

            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }

            string adminQuery = Auth.Role.IsAdmin ? " WHERE 1 = 1 " : " WHERE A2.ENABLED = 1 ";  //登入取得所有資料:未登入只能取得上線資料
            //若有輸出隸屬目錄 ID，驗證是GUID格式D 以連字號分隔的32位數:00000000-0000-0000-0000-000000000000
            categoryId_sql = Guid.TryParseExact(categoryId, "D", out Guid categoryIdguid) ? $" AND A2.[CATEGORYID] = '{categoryIdguid}' " : "";

            string _sql = @"SELECT A2.*, A1.NAME AS CATEGORYNAME FROM [TAIWAN_DOJO] A2 " +
                                  @"LEFT JOIN [TAIWAN_DOJO_CATEGORY] A1 ON A1.[CATEGORYID] = A2.[CATEGORYID] " +
                                  $" {adminQuery} " +
                                  $" {categoryId_sql} " +
                                  @"ORDER BY A2.[FIRST] DESC, A2.[SORT], A2.[CREATIONDATE] DESC " +
                                  $" {page_sql} ";

            List<TaiwanDojo> taiwanDojos = _dapperHelper.QuerySetSql<TaiwanDojo>(_sql).ToList();

            return taiwanDojos;
        }
        #endregion

        #region 文字編輯器內容區插入一個圖片(新增與修改時)
        public TaiwanDojoImage AddUploadImage(HttpRequest request)
        {
            TaiwanDojoImage taiwanDojoImage = new TaiwanDojoImage();
            taiwanDojoImage.taiwanDojoImageId = Guid.NewGuid();                                             //主索引鍵（P.K.）自動產生一個Guid
            taiwanDojoImage.imageName = taiwanDojoImage.taiwanDojoImageId + ".png";   //用taiwanDojoImageId來命名圖檔

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Taiwandojo);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            _imageFileHelper.SaveUploadImageFile(request.Files[0], taiwanDojoImage.imageName, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [TAIWAN_DOJO_IMAGE]
                                            ([TAIWANDOJOIMAGEID]
                                            ,[IMAGENAME])
                                        VALUES
                                            (@TAIWANDOJOIMAGEID,
                                             @IMAGENAME )
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _dapperHelper.QuerySingle(_sql, taiwanDojoImage);     //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            taiwanDojoImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            taiwanDojoImage.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().Taiwandojo, taiwanDojoImage.imageName);

            return taiwanDojoImage;
        }

        public TaiwanDojoImage AddUploadImage(HttpRequest request, Guid taiwanDojoId)
        {
            TaiwanDojoImage taiwanDojoImage = new TaiwanDojoImage();
            taiwanDojoImage.taiwanDojoImageId = Guid.NewGuid();                                            //主索引鍵（P.K.）自動產生一個Guid
            taiwanDojoImage.imageName = taiwanDojoImage.taiwanDojoImageId + ".png";   //用taiwanDojoImageId來命名圖檔
            taiwanDojoImage.taiwanDojoId = taiwanDojoId;                                                           //關聯內容taiwanDojoId

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Taiwandojo);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            _imageFileHelper.SaveUploadImageFile(request.Files[0], taiwanDojoImage.imageName, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [TAIWAN_DOJO_IMAGE]
                                            ([TAIWANDOJOIMAGEID]
                                            ,[IMAGENAME]
                                            ,[TAIWANDOJOID])
                                        VALUES
                                            (@TAIWANDOJOIMAGEID,
                                             @IMAGENAME,
                                             @TAIWANDOJOID)
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _dapperHelper.QuerySingle(_sql, taiwanDojoImage);     //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            taiwanDojoImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            taiwanDojoImage.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().Taiwandojo, taiwanDojoImage.imageName);

            return taiwanDojoImage;
        }
        #endregion

        #region 工具區
        /// <summary>
        /// 在新增內容時，下面內文若有插入圖片，插入的圖片還沒有TAIWANDOJOID, 送出前cNameArr收集了內容裏插入了多少張圖
        /// 然後在送出後在將確定有插入的內容圖片更新taiwan_dojo_image.taiwanDojoId=taiwanDojoId
        /// 為何不先給插入圖片id，因為插入圖片隨時可能又del，當del掉後若已給id就無法用null來辨斷刪除
        /// </summary>
        /// <param name="images">httpRequest.Form["cNameArr"]內容圖片的檔名JSON字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="Id">關連內容的taiwanDojoId</param>
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
                            TaiwanDojoImage taiwanDojoImage = new TaiwanDojoImage();
                            taiwanDojoImage.taiwanDojoImageId = Guid.Parse(_imageGuid);        //存放目錄下檔名不含副檔給taiwan_dojo_image表裏id
                            taiwanDojoImage.taiwanDojoId = Id;                                        //關聯內容id

                            //更新taiwan_dojo內容的taiwanDojoId給taiwan_dojo_image.taiwanDojoId 連立關連
                            string _sql = @"UPDATE [TAIWAN_DOJO_IMAGE]
                                                           SET [TAIWANDOJOID] =@TAIWANDOJOID
                                                         WHERE TAIWANDOJOIMAGEID = @TAIWANDOJOIMAGEID ";
                            _dapperHelper.ExecuteSql(_sql, taiwanDojoImage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刪除[taiwanDojoId] IS NULL不用的內容圖片
        /// </summary>
        /// <param name="saveFolderPath"></param>
        private void RemoveContentImages(string saveFolderPath)
        {
            //取得存放磁碟目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png

            //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
            string _sql = @"SELECT [IMAGENAME] FROM [TAIWAN_DOJO_IMAGE] WHERE TAIWANDOJOID IS NULL";
            List<string> _imageDataBaseList = _dapperHelper.QuerySetSql<string>(_sql).ToList();

            //刪除多餘的內容圖檔, _imageDataBaseList 資料庫檔案名清單，_imageFileList 實際目錄檔路徑檔案清單
            for (int y = 0; y < _imageDataBaseList.Count; y++)
            {
                for (int i = 0; i < _imageFileList.Count; i++)
                {
                    string _removeFileName = Path.GetFileName(_imageFileList[i]);   //除掉路徑留檔案名稱

                    //資料庫TAIWANDOJOID=NULL的檔案名 == 資料匣存放的檔名&&存放檔實際是否存在
                    if (_imageDataBaseList[y] == _removeFileName && File.Exists(_imageFileList[i]))
                    {
                        File.Delete(_imageFileList[i]);
                    }
                }
            }
            //刪除資料庫裡無相關連的內容圖片記錄
            _sql = @"DELETE FROM [TAIWAN_DOJO_IMAGE] WHERE [TAIWANDOJOID] IS NULL";

            _dapperHelper.ExecuteSql(_sql);
        }
        #endregion
    }
}