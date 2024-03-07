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
    public class Video2Service : IVideo2Service
    {
        #region DI依賴注入功能
        private IDapperHelper DapperHelper;
        private IImageFileHelper ImageFileHelper;
        public Video2Service(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            DapperHelper = dapperHelper;
            ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        public Video2 InsertVideo(HttpRequest httpRequest)
        {
            Video2 video2 = RequestData(httpRequest);

            string _sql = @" INSERT INTO [VIDEO2]
                                                    ([VIDEO2ID]
                                                    ,[COVER]
                                                    ,[NAME]
                                                    ,[VIDEO]
                                                    ,[BRIEF]
                                                    ,[CONTENT]
                                                    ,[FIRST]
                                                    ,[SORT]
                                                    ,[ENABLED]
                                                    ,[CREATIONDATE] )
                                                VALUES
                                                    (@VIDEO2ID,
                                                     @COVER, 
                                                     @NAME, 
                                                     @VIDEO,
                                                     @BRIEF,
                                                     @CONTENT,
                                                     @FIRST,
                                                     @SORT,
                                                     @ENABLED, 
                                                     @CREATIONDATE )
                                                         SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。

            int id = DapperHelper.QuerySingle(_sql, video2);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            video2.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(httpRequest.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                video2.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                video2.sort = Convert.ToInt32(httpRequest.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [VIDEO2]
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            DapperHelper.ExecuteSql(_sql, video2);         //更新排序

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Video22);

            //1.辨斷有沒有上傳封面檔案
            if (httpRequest.Files.Count > 0)
            {
                if (httpRequest.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳，不然CheckImageMIME會出錯
                {
                    //2.存放上傳圖片(1實體檔,2檔名,3路徑)
                    ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], video2.cover, _RootPath_ImageFolderPath);
                }
            }

            //3.1將編輯內容圖片裏剛產生的VideoImage表裏的圖檔尚未關連此video.Id更新進去
            HandleContentImages(httpRequest.Form["cNameArr"], _RootPath_ImageFolderPath, video2.video2Id);

            //4.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);

            return video2;
        }
        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="httpRequest">讀取表單資料</param>
        /// <returns>回傳表單類別</returns>
        private Video2 RequestData(HttpRequest httpRequest)
        {
            Video2 video2 = new Video2();
            video2.video2Id = Guid.NewGuid();                                                                          //活動類別另設 ID 唯一索引用
            if (httpRequest.Files.Count > 0)        //有圖時優先處理上傳圖檔
            {
                if (httpRequest.Files[0].ContentLength > 0)
                {
                    video2.cover = video2.video2Id + ".png";                                                     //圖片檔名使用id來當
                }
            }
            else  //沒圖時直接抓cover
            {
                video2.cover = httpRequest.Form["cover"];                                                          //Youtube自動縮圖網址
            }
            video2.name = httpRequest.Form["name"];                                                           //活動影片標題
            video2.video = httpRequest.Form["video"];                                                                                //影片網址 
            video2.brief = httpRequest.Form["brief"];                                                                                   //活動影片簡述 
            video2.content = httpRequest.Form["content"];                                                                        //活動影片內容
            video2.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                 //是否置頂(0 關閉;1 開啟)
            video2.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                    //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            video2.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));   //是否啟用(0 關閉;1 開啟)

            video2.creationDate = DateTime.Now;

            return video2;
        }
        #endregion

        #region 取得一筆資料
        public Video2 GetVideo(Guid videoId)
        {
            Video2 video2 = new Video2();
            video2.video2Id = videoId;      //活動影片類別另設 ID
            //2021 / 11 / 30   2021-12-01 13:59:01  2021 / 12 / 02
            //Start_Date <= getdate() < End_Date +1 End_Date 是當天上午12:00 => 00:00
            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 ";
            string _sql = $"SELECT * FROM [VIDEO2] WHERE [VIDEO2ID] = @VIDEO2ID {adminQuery}";
            video2 = DapperHelper.QuerySqlFirstOrDefault(_sql, video2);

            //封面圖檔加上http網址，自動縮圖網址不用
            if (video2 != null)  //先辨類別有無資料
            {
                if (!video2.cover.StartsWith("http")) //在辨cover有無http, 無代表是上傳檔案，有代表是自動縮圖網址https://i.ytimg.com/vi_webp/nnApDW56lFc/maxresdefault.webp
                {
                    video2.cover = Tools.GetInstance().GetImageLink(Tools.GetInstance().Video22, video2.cover);
                }
            }
            return video2;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateVideo(HttpRequest httpRequest, Video2 video2)
        {
            video2 = RequestDataMod(httpRequest, video2); //將接收來的參數，和抓到的要修改資料合併

            #region 辨斷有沒有上傳檔案
            if (httpRequest.Files.Count > 0) //1檢查有沒有上傳圖片的欄位選項
            {
                if (httpRequest.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳
                {
                    //以下處理圖片
                    //1. 取得要放置的目錄路徑
                    string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Video22);

                    // Video video = VideoService.GetVideo(id); 裏面的image_name有加網址ImageFileHelper.GetImageLink，改只取檔案名與副檔
                    string _image_name = Path.GetFileName(video2.cover);

                    //2.存放上傳封面圖片(1實體檔,2檔名,3路徑)
                    ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], _image_name, _RootPath_ImageFolderPath);
                }
            }

            //將在控制器上取出的資料庫裏的有加http://..最後檔名取出，不然資料庫更新時會出錯超出欄位大小
            //video2.imageName = Path.GetFileName(video2.imageName);
            #endregion

            string _sql = @"UPDATE [VIDEO2]
                                           SET
                                               [COVER] = @COVER
                                              ,[NAME] = @NAME
                                              ,[VIDEO] = @VIDEO
                                              ,[BRIEF] = @BRIEF
                                              ,[CONTENT] = @CONTENT
                                              ,[FIRST] = @FIRST
                                              ,[SORT] = @SORT
                                              ,[ENABLED] = @ENABLED
                                              ,[UPDATEDDATE] = @UPDATEDDATE
                                         WHERE [VIDEO2ID] = @VIDEO2ID ";

            DapperHelper.ExecuteSql(_sql, video2);
        }
        /// <summary>
        /// 讀取表單資料,轉到video
        /// </summary>
        /// <param name="httpRequest">讀取表單資料(修改時用)</param>
        /// <param name="video2">控制器取到資料</param>
        /// <returns>video</returns>
        private Video2 RequestDataMod(HttpRequest httpRequest, Video2 video2)
        {
            if (httpRequest.Files.Count > 0)        //若有重新上傳檔案時, 用圖片檔名使用id來當
            {
                if (httpRequest.Files[0].ContentLength > 0)
                {
                    video2.cover = video2.video2Id + ".png";
                }
            }
            else  //如果沒有上傳封面檔，則以cover欄位當資料來存
            {
                video2.cover = httpRequest.Form["cover"];                                                                              //Youtube自動縮圖網址
            }
            video2.name = httpRequest.Form["name"];                                                                               //活動影片標題
            video2.brief = httpRequest.Form["brief"];                                                                                   //活動影片簡述 
            video2.video = httpRequest.Form["video"];                                                                                //影片網址 
            video2.content = httpRequest.Form["content"];                                                                        //影片活動內容
            video2.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                 //是否置頂(0 關閉;1 開啟)
            video2.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                    //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            video2.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));   //是否啟用(0 關閉;1 開啟)

            video2.updatedDate = DateTime.Now;

            return video2;
        }
        #endregion

        #region 刪除一筆資料
        public void DeleteVideo(Video2 video2)
        {
            //1. 取得放置圖片的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Video22);

            //Video已先在控制器取得此筆資料，需此資料的video_id，取得這筆內容下相關內容圖片檔名清單
            string _sql = @"SELECT [IMAGENAME] FROM [VIDEO_IMAGE2] WHERE VIDEO2ID = @VIDEO2ID";
            //調出此筆內容下的多筆圖片名稱，到List<string>
            List<string> video_image_name_list = DapperHelper.QuerySetSql<Video2, string>(_sql, video2).ToList();

            //刪除內容下內容圖, List<string> _video_image_name_list
            string video_image_name_list_path = string.Empty;  //宣告一個變數之後要加入路徑
            for (int i = 0; i < video_image_name_list.Count; i++)
            {
                //路徑與圖檔結合, 再辨斷存在就刪除
                video_image_name_list_path = Path.Combine(_RootPath_ImageFolderPath, video_image_name_list[i]);
                if (File.Exists(video_image_name_list_path))
                {
                    File.Delete(video_image_name_list_path);
                }
            }

            //刪除內容封面
            if (video2.cover != null)
            {
                //1. 8cd2c524-cff7-4e2b-99d2-9490e18367eb.png 封面圖檔檔名
                //2. https://www.youtube.com/embed/e73xGcXNPsQ   Youtube自動縮圖網址
                string _videocover = Path.GetFileNameWithoutExtension(video2.cover);  //只留下檔案名稱，路徑與副檔名都去除
                //D	以連字號分隔的32位數00000000-0000-0000-0000-000000000000
                //上傳的封面圖才會是Guid，回傳bool ，out Guid _videocoverout沒用到
                if (Guid.TryParseExact(_videocover, "D", out Guid _videocoverout))
                {
                    //取出內容圖片檔案
                    string _image_name = Path.GetFileName(video2.cover);   //取出內容封面檔名
                    string _image_name_FilePath = Path.Combine(_RootPath_ImageFolderPath, _image_name);  //封面檔名加上路徑
                    if (File.Exists(_image_name_FilePath))
                    {
                        File.Delete(_image_name_FilePath);
                    }
                }
            }

            //刪除資料庫內容 資料庫有設定重疊顯示，所以刪主鍵關連VIDEO_IMAGE資料會一同刪除FK_video_image_Video
            _sql = @"DELETE FROM [VIDEO2] WHERE VIDEO2ID = @VIDEO2ID";
            //執行刪除
            DapperHelper.ExecuteSql(_sql, video2);
        }
        #endregion

        #region 取得影片
        public List<Video2> GetVideoAll(int? first, int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string sqlfirst_sql1 = string.Empty;  //置頂設定
            string sqlfirst_sql2 = string.Empty;  //置頂設定
            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }
            //前端顯示只有一筆置頂
            if ( first != null && first == 1)
            {
                sqlfirst_sql1 = @"top 1";
                sqlfirst_sql2 = @"AND [first] = 'true'";
                //TOP 不可使用在 OFFSET 的同一個查詢或子查詢。
                page_sql = ""; //清除
            }
            string adminQuery = Auth.Role.IsAdmin ? $" WHERE 1=1 {sqlfirst_sql2} " : $" WHERE ENABLED = 1 {sqlfirst_sql2}";
            string _sql = $"SELECT {sqlfirst_sql1} * FROM [VIDEO2] {adminQuery} " +
                                  @" ORDER BY creationDate DESC , SORT " +
                                  $" {page_sql} ";

            List<Video2> video2 = DapperHelper.QuerySetSql<Video2>(_sql).ToList();

            //將資料庫cover加上URL
            //封面圖檔加上http網址，自動縮圖網址不用
            for (int i = 0; i < video2.Count; i++)
            {
                if (!video2[i].cover.StartsWith("http"))   //在辨cover有無http, 無代表是上傳檔案，有代表是自動縮圖網址https://i.ytimg.com/vi_webp/nnApDW56lFc/maxresdefault.webp
                {
                    video2[i].cover = Tools.GetInstance().GetImageLink(Tools.GetInstance().Video22, video2[i].cover);
                }
            }

            return video2;
        }
        #endregion

        /// <summary>
        /// 新增一個[VideoImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 因為正在新增內容還未送出，所以內容的VideoImage_video_id關連video.video_id還沒產生
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns>videoImage</returns>
        public VideoImage2 AddUploadImage(HttpRequest httpRequest)
        {
            VideoImage2 videoImage2 = new VideoImage2();
            videoImage2.videoImage2Id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
            videoImage2.imageName = videoImage2.videoImage2Id + ".png";   //用videoImage2Id 來命名圖檔
            //videoImage2.video2Id = "關連內容video2Id尚未產生，進入新增狀態時尚未送出所以沒有值";

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Video22);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], videoImage2.imageName, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [VIDEO_IMAGE2]
                                            ([VIDEOIMAGE2ID]
                                            ,[IMAGENAME])
                                        VALUES
                                            (@VIDEOIMAGE2ID,
                                            @IMAGENAME )
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = DapperHelper.QuerySingle(_sql, videoImage2);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            videoImage2.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            videoImage2.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().Video22, videoImage2.imageName);

            return videoImage2;
        }

        /// <summary>
        /// 更新活動管理的內容圖片
        /// 新增一個[VideoImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="Video_Id"></param>
        /// <returns></returns>
        public VideoImage2 AddUploadImage(HttpRequest httpRequest, Guid Video_Id)
        {
            VideoImage2 videoImage2 = new VideoImage2();
            videoImage2.videoImage2Id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
            videoImage2.imageName = videoImage2.videoImage2Id + ".png";   //用videoImage2Id 來命名圖檔
            videoImage2.video2Id = Video_Id;                                                            //關聯內容video_id

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Video22);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], videoImage2.imageName, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [VIDEO_IMAGE2]
                                            ([VIDEOIMAGE2ID]
                                            ,[IMAGENAME]
                                            ,[VIDEO2ID])
                                        VALUES
                                            (@VIDEOIMAGE2ID,
                                            @IMAGENAME,
                                            @VIDEO2ID)
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = DapperHelper.QuerySingle(_sql, videoImage2);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            videoImage2.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            videoImage2.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().Video22, videoImage2.imageName);

            return videoImage2;
        }


        #region 工具區
        /// <summary>
        /// 在新增內容時，下面內文若有插入圖片，插入的圖片還沒有Video_id, 送出前fNameArr收集了內容裏插入了多少張圖
        /// 然後在送出後在將確定有插入的內容圖片更新VideoImage.Video_id=Video_id
        /// 為何不先給插入圖片id，因為插入圖片隨時可能又del，當del掉後若已給id就無法用null來辨斷刪除
        /// </summary>
        /// <param name="images">httpRequest.Form["fNameArr"]內容圖片的檔名JSON字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="Id">關連內容的video_id</param>
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
                            VideoImage2 videoImage2 = new VideoImage2();
                            videoImage2.videoImage2Id = Guid.Parse(_imageGuid);        //存放目錄下檔名不含副檔給VideoImage表裏id
                            videoImage2.video2Id = Id;                                                          //關聯活動管理內容id

                            //更新Video內容的video_id給VideoImage.video_id 連立關連
                            string _sql = @"UPDATE [VIDEO_IMAGE2]
                                                           SET [VIDEO2ID] =@VIDEO2ID
                                                         WHERE VIDEOIMAGE2ID = @VIDEOIMAGE2ID ";
                            DapperHelper.ExecuteSql(_sql, videoImage2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刪除[video_Id] IS NULL不用的內容圖片
        /// </summary>
        /// <param name="saveFolderPath"></param>
        private void RemoveContentImages(string saveFolderPath)
        {
            //取得存放磁碟目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png

            //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
            string _sql = @"SELECT [IMAGENAME] FROM [VIDEO_IMAGE2] WHERE VIDEO2ID IS NULL";
            List<string> _imageDataBaseList = DapperHelper.QuerySetSql<string>(_sql).ToList();

            //刪除多餘的內容圖檔, _imageDataBaseList 資料庫檔案名清單，_imageFileList 實際目錄檔路徑檔案清單
            for (int y = 0; y < _imageDataBaseList.Count; y++)
            {
                for (int i = 0; i < _imageFileList.Count; i++)
                {
                    string _removeFileName = Path.GetFileName(_imageFileList[i]);   //除掉路徑留檔案名稱

                    //資料庫Video_Id=NULL的檔案名 == 資料匣存放的檔名&&存放檔實際是否存在
                    if (_imageDataBaseList[y] == _removeFileName && File.Exists(_imageFileList[i]))
                    {
                        File.Delete(_imageFileList[i]);
                    }
                }
            }
            //刪除資料庫裡無相關連的內容圖片記錄
            _sql = @"DELETE FROM [VIDEO_IMAGE2] WHERE [VIDEO2ID] IS NULL";

            DapperHelper.ExecuteSql(_sql);
        }
        #endregion

    }
}