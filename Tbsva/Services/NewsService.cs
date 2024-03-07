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
    public class NewsService : INewsService
    {
        #region DI依賴注入功能
        private IDapperHelper DapperHelper;
        private IImageFileHelper ImageFileHelper;
        public NewsService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            DapperHelper = dapperHelper;
            ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        public News InsertNews(HttpRequest httpRequest)
        {
            News news = RequestData(httpRequest);

            //設定上傳檔案的檔名對應相同的類別名稱
            //HttpRequest.Files 屬性 取得用戶端所上傳的檔案集合
            //httpRequest.Files 將文件集合加載到 HttpFileCollection 變量中。
            ImageFileHelper.SetImageFileName(news, httpRequest.Files);

            string _sql = @" INSERT INTO [News]
                                                    ([News_ID]
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
                                                    (@News_ID,
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

            if (news.start_date == DateTime.MinValue)
            {
                _sql = _sql.Replace("@START_DATE", "NULL");
            }
            if (news.end_date == DateTime.MinValue)
            {
                _sql = _sql.Replace("@END_DATE", "NULL");
            }
            int id = DapperHelper.QuerySingle(_sql, news);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            news.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(httpRequest.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                news.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                news.sort = Convert.ToInt32(httpRequest.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [news] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            DapperHelper.ExecuteSql(_sql, news);         //更新排序

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().News);
            string _saveFolderPath = ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{id}\");

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            //ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], news.image_name, _RootPath_ImageFolderPath);

            //2.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            ImageFileHelper.SaveMoreUploadImageFile(httpRequest.Files, _saveFolderPath);

            //3.1將編輯內容圖片裏剛產生的NewsImage表裏的圖檔尚未關連此news.Id更新進去
            HandleContentImages(httpRequest.Form["cNameArr"], _RootPath_ImageFolderPath, news.news_id);

            //4.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);

            return news;
        }
        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="httpRequest">讀取表單資料</param>
        /// <returns>回傳表單類別</returns>
        private News RequestData(HttpRequest httpRequest)
        {
            News news = new News();
            news.news_id = Guid.NewGuid();                                                                                            //最新消息類別另設 ID 唯一索引用
            //news.image_name = news.news_id + ".png";                                                                       //cover圖片檔名使用id來當
            news.name = httpRequest.Form["name"];                                                                                //最新消息標題

            news.brief = httpRequest.Form["brief"];                                                                                   //最新消息簡述 
            news.video = httpRequest.Form["video"];                                                                                //影片網址 
            news.content = httpRequest.Form["content"];                                                                        //最新消息內容
            news.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            news.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            news.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)

            //上架日, Start_Date Postman進來, 會是空值或沒有此欄位或有值，有值以外兩個條件都不會成立，所以news.Start_Date沒收到會是null
            if (Tools.Formatter.IsDate(httpRequest.Form["startDate"]))
            {
                news.start_date = Convert.ToDateTime(httpRequest.Form["startDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                news.start_date = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
                 //不然NewsDto newsDto = Mapper.Map<NewsDto>(news);
                 //會將null轉成空字串News DateTime? Start_Date (null) => NewsDto string Start_Date ("")
                 //結果NewsProfile FormatDate又將如果值是DateTime.MinValue時又轉為""
            }
            //下架日
            if (Tools.Formatter.IsDate(httpRequest.Form["endDate"]))
            {
                news.end_date = Convert.ToDateTime(httpRequest.Form["endDate"]);
            }
            else //若不是日期，這裏其實可以不用設定
            {
                news.end_date = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
                //不然NewsDto newsDto = Mapper.Map<NewsDto>(news);
                //會將null轉成空字串News DateTime? Start_Date (null) => NewsDto string Start_Date ("")
                //結果NewsProfile FormatDate又將如果值是DateTime.MinValue時又轉為""
            }
            news.creation_date = DateTime.Now;

            return news;
        }
        #endregion

        #region 取得一筆資料
        public News GetNews(Guid newsId)
        {
            News news = new News();
            news.news_id = newsId;      //最新消息類別另設 ID
            //2021 / 11 / 30   2021-12-01 13:59:01  2021 / 12 / 02
            //Start_Date <= getdate() < End_Date +1 End_Date 是當天上午12:00 => 00:00
            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 AND (START_DATE IS NULL OR START_DATE <= GETDATE() ) AND (END_DATE IS NULL OR GETDATE() < DATEADD(DAY, 0, END_DATE) ) ";
            string _sql = $"SELECT * FROM [news] WHERE [news_ID] = @news_ID {adminQuery}";
            news = DapperHelper.QuerySqlFirstOrDefault(_sql, news);

            //加上http網址
            if (news != null)  //確定有資料在處理
            {
                string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
                SystemFunctions.picaddhttp(fields, news, Tools.GetInstance().News, news.id);
            }
            return news;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateNews(HttpRequest httpRequest, News news)
        {
            news = RequestDataMod(httpRequest, news); //將接收來的參數，和抓到的要修改資料合併

            //1.設定上傳檔案的檔名對應相同的類別名稱
            //HttpRequest.Files 屬性 取得用戶端所上傳的檔案集合
            //httpRequest.Files 將文件集合加載到 HttpFileCollection 變量中。
            ImageFileHelper.SetImageFileName(news, httpRequest.Files);

            //2.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().News);
            string _saveFolderPath = ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{news.id}\");

            //3.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            ImageFileHelper.SaveMoreUploadImageFile(httpRequest.Files, _saveFolderPath);

            //4.組裝修改刪除不變更時sql
            //string _sqlpic = picSql(news, httpRequest);
            string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
            string _sqlpic = SystemFunctions.picSql<News>(fields, news, httpRequest); 

            string _sql = $@"UPDATE [news]
                                           SET
                                               [NAME] = @NAME
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
                                         WHERE [news_ID] = @news_ID ";

            if (news.start_date == DateTime.MinValue)
            {
                _sql = _sql.Replace("@START_DATE", "NULL");
            }
            if (news.end_date == DateTime.MinValue)
            {
                _sql = _sql.Replace("@END_DATE", "NULL");
            }
            DapperHelper.ExecuteSql(_sql, news);
        }
        /// <summary>
        /// 讀取表單資料,轉到news
        /// </summary>
        /// <param name="httpRequest">讀取表單資料(修改時用)</param>
        /// <param name="news">控制器取到資料</param>
        /// <returns>news</returns>
        private News RequestDataMod(HttpRequest httpRequest, News original_news)
        {
            News news = new News();                                                         //設定一組空的沒資料會null, 讓圖片預設是null不執行SQL指令，此null是類別裏真的null 
            news.id = original_news.id;
            news.news_id = original_news.news_id;
            news.name = httpRequest.Form["name"];                                                                                //最新消息標題
            news.brief = httpRequest.Form["brief"];                                                                                   //最新消息簡述 
            news.video = httpRequest.Form["video"];                                                                                //影片網址 
            news.content = httpRequest.Form["content"];                                                                        //最新消息內容
            news.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            news.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            news.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)

            //上架日, Start_Date Postman進來, 會是空值或沒有此欄位或有值，有值以外兩個條件都不會成立，所以News.Start_Date沒收到會是null
            if (Tools.Formatter.IsDate(httpRequest.Form["startDate"]))
            {
                news.start_date = Convert.ToDateTime(httpRequest.Form["startDate"]);
            }
            else //更新時，若時間改成不選時要設定
            {
                news.start_date = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
                                                          //不然NewsDto newsDto = Mapper.Map<NewsDto>(news);
                                                          //會將null轉成空字串News DateTime? Start_Date (null) => NewsDto string Start_Date ("")
                                                          //結果NewsProfile FormatDate又將如果值是DateTime.MinValue時又轉為""
            }
            //下架日
            if (Tools.Formatter.IsDate(httpRequest.Form["endDate"]))
            {
                news.end_date = Convert.ToDateTime(httpRequest.Form["endDate"]);
            }
            else //更新時，若時間改成不選時要設定
            {
                news.end_date = DateTime.MinValue;  //將null設定成0001/1/1 上午 12:00:00
                //不然NewsDto newsDto = Mapper.Map<NewsDto>(news);
                //會將null轉成空字串News DateTime? Start_Date (null) => NewsDto string Start_Date ("")
                //結果NewsProfile FormatDate又將如果值是DateTime.MinValue時又轉為""
            }
            news.updated_date = DateTime.Now;

            return news;
        }
        #endregion

        #region 刪除一筆資料
        public void DeleteNews(News news)
        {
            //1. 取得放置圖片的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().News);

            //取得這筆內容下相關內容圖片檔名清單
            string _sql = @"SELECT [IMAGE_NAME] FROM [news_IMAGE] WHERE news_ID = @news_ID";
            //調出此筆內容下的多筆圖片名稱，到List<string>
            List<string> news_image_name_list = DapperHelper.QuerySetSql<News, string>(_sql, news).ToList();

            //刪除內容下內容圖, List<string> _news_image_name_list
            string news_image_name_list_path = string.Empty;  //宣告一個變數之後要加入路徑
            for (int i = 0; i < news_image_name_list.Count; i++)
            {
                //路徑與圖檔結合, 再辨斷存在就刪除
                news_image_name_list_path = Path.Combine(_RootPath_ImageFolderPath, news_image_name_list[i]);
                if (File.Exists(news_image_name_list_path))
                {
                    File.Delete(news_image_name_list_path);
                }
            }

            //刪除封面與導覽圖片(刪自動id目錄)
            //取出內容圖片檔案
            //string _image_name = Path.GetFileName(news.image_name);   //取出內容封面檔名
            //string _image_name_FilePath = Path.Combine(_RootPath_ImageFolderPath, _image_name);  //封面檔名加上路徑
            //if (File.Exists(_image_name_FilePath))
            //{
            //    File.Delete(_image_name_FilePath);
            //}
            string folder = Path.Combine(_RootPath_ImageFolderPath, news.id.ToString());
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            //刪除資料庫內容 資料庫有設定重疊顯示，所以刪主鍵關連news_IMAGE資料會一同刪除FK_news_image_news
            _sql = @"DELETE FROM [news] WHERE news_ID = @news_ID";
            //執行刪除
            DapperHelper.ExecuteSql(_sql, news);
        }
        #endregion

        #region 取得分頁最新消息管理
        public List<News> GetNewsAll(int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY

            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }
            string adminQuery = Auth.Role.IsAdmin ? " WHERE 1 = 1 " : " WHERE ENABLED = 1 AND (START_DATE IS NULL OR START_DATE <= GETDATE() ) AND (END_DATE IS NULL OR GETDATE() < DATEADD(DAY, 0, END_DATE) ) ";  //登入取得所有資料:未登入只能取得上線資料

            string _sql = @"SELECT * FROM [NEWS]  " +
                                  $" {adminQuery} " +
                                 @"ORDER BY [FIRST] DESC , CREATION_DATE DESC , SORT " +
                                  $" {page_sql} ";
            List<News> news = DapperHelper.QuerySetSql<News>(_sql).ToList();

            //將資料庫image_name加上URL
            for (int i = 0; i < news.Count; i++)
            {
                //news[i].image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().News, news[i].image_name);

                string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
                SystemFunctions.picaddhttp(fields, news[i], Tools.GetInstance().News, news[i].id);
            }
            return news;
        }
        #endregion

        /// <summary>
        /// 新增一個[NewsImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 因為正在新增內容還未送出，所以內容的NewsImage_news_id關連news.news_id還沒產生
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns>newsImage</returns>
        public NewsImage AddUploadImage(HttpRequest httpRequest)
        {
            NewsImage newsImage = new NewsImage();
            newsImage.news_image_id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
            newsImage.image_name = newsImage.news_image_id + ".png";   //用news_image_id來命名圖檔
            //newsImage.news_id = "關連內容news_id尚未產生，進入新增狀態時尚未送出所以沒有值";

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().News);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], newsImage.image_name, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [NEWS_IMAGE]
                                            ([NEWS_IMAGE_ID]
                                            ,[IMAGE_NAME])
                                        VALUES
                                            (@NEWS_IMAGE_ID,
                                            @IMAGE_NAME )
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = DapperHelper.QuerySingle(_sql, newsImage);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            newsImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            newsImage.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().News, newsImage.image_name);

            return newsImage;
        }

        /// <summary>
        /// 更新最新消息管理的內容圖片
        /// 新增一個[NewsImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="News_Id"></param>
        /// <returns></returns>
        public NewsImage AddUploadImage(HttpRequest httpRequest, Guid News_Id)
        {
            NewsImage newsImage = new NewsImage();
            newsImage.news_image_id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
            newsImage.image_name = newsImage.news_image_id + ".png";   //用news_image_id來命名圖檔
            newsImage.news_id = News_Id;                                                            //關聯內容news_id

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().News);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], newsImage.image_name, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [news_IMAGE]
                                            ([news_IMAGE_ID]
                                            ,[IMAGE_NAME]
                                            ,[news_ID])
                                        VALUES
                                            (@news_IMAGE_ID,
                                            @IMAGE_NAME,
                                            @news_ID)
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = DapperHelper.QuerySingle(_sql, newsImage);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            newsImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            newsImage.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().News, newsImage.image_name);

            return newsImage;
        }


        #region 工具區
        /// <summary>
        /// 在新增內容時，下面內文若有插入圖片，插入的圖片還沒有News_id, 送出前fNameArr收集了內容裏插入了多少張圖
        /// 然後在送出後在將確定有插入的內容圖片更新NewsImage.news_id=News_id
        /// 為何不先給插入圖片id，因為插入圖片隨時可能又del，當del掉後若已給id就無法用null來辨斷刪除
        /// </summary>
        /// <param name="images">httpRequest.Form["fNameArr"]內容圖片的檔名JSON字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="Id">關連內容的news_id</param>
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
                            NewsImage newsImage = new NewsImage();
                            newsImage.news_image_id = Guid.Parse(_imageGuid);        //存放目錄下檔名不含副檔給NewsImage表裏id
                            newsImage.news_id = Id;                                        //關聯最新消息管理內容id

                            //更新News內容的news_id給NewsImage.news_id 連立關連
                            string _sql = @"UPDATE [news_IMAGE]
                                                           SET [news_ID] =@news_ID
                                                         WHERE news_IMAGE_ID = @news_IMAGE_ID ";
                            DapperHelper.ExecuteSql(_sql, newsImage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刪除[News_Id] IS NULL不用的內容圖片
        /// </summary>
        /// <param name="saveFolderPath"></param>
        private void RemoveContentImages(string saveFolderPath)
        {
            //取得存放磁碟目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png

            //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
            string _sql = @"SELECT [IMAGE_NAME] FROM [news_IMAGE] WHERE news_ID IS NULL";
            List<string> _imageDataBaseList = DapperHelper.QuerySetSql<string>(_sql).ToList();

            //刪除多餘的內容圖檔, _imageDataBaseList 資料庫檔案名清單，_imageFileList 實際目錄檔路徑檔案清單
            for (int y = 0; y < _imageDataBaseList.Count; y++)
            {
                for (int i = 0; i < _imageFileList.Count; i++)
                {
                    string _removeFileName = Path.GetFileName(_imageFileList[i]);   //除掉路徑留檔案名稱

                    //資料庫News_Id=NULL的檔案名 == 資料匣存放的檔名&&存放檔實際是否存在
                    if (_imageDataBaseList[y] == _removeFileName && File.Exists(_imageFileList[i]))
                    {
                        File.Delete(_imageFileList[i]);
                    }
                }
            }
            //刪除資料庫裡無相關連的內容圖片記錄
            _sql = @"DELETE FROM [news_IMAGE] WHERE [news_ID] IS NULL";

            DapperHelper.ExecuteSql(_sql);
        }
        #endregion

    }
}