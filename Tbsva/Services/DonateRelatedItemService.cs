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
    public class DonateRelatedItemService : IDonateRelatedItemService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;

        public DonateRelatedItemService(IDapperHelper iDapperHelper, IImageFileHelper imageFileHelper)
        {
            _IDapperHelper = iDapperHelper;
            m_ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        public DonateRelatedItem Insert_DonateRelatedItem(HttpRequest _request)
        {
            DonateRelatedItem _DonateRelatedItem = Request_data(_request);
            string _sql = @"INSERT INTO [DonateRelatedItem]
                                           ([donateRelatedItemId]
                                           ,[primary]
                                           ,[secondary]
                                           ,[title]
                                           ,[ImageName]
                                           ,[Content1]
                                           ,[Content2]
                                           ,[Content3]
                                           ,[Amount]
                                           ,[Notes]
                                           ,[Sort]
                                           ,[first]
                                           ,[enabled]
                                           ,[CreateDate])
                                     VALUES
                                           (@donateRelatedItemId,
                                            @primary,
                                            @secondary,
                                            @title,
                                            @ImageName,
                                            @Content1,
                                            @Content2,
                                            @Content3,
                                            @Amount,
                                            @Notes,
                                            @Sort,
                                            @first,
                                            @enabled,
                                            @CreateDate )
                     SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, _DonateRelatedItem);       //新增資料
            _DonateRelatedItem.Id = id;

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(_request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                _DonateRelatedItem.Sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                _DonateRelatedItem.Sort = Convert.ToInt32(_request.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [DonateRelatedItem] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            _IDapperHelper.ExecuteSql(_sql, _DonateRelatedItem);

            //處理圖片
            //1.取圖片路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DonateRelatedItem);

            //1.辨斷有沒有上傳封面檔案
            if (_request.Files.Count > 0)
            {
                if (_request.Files[0].ContentLength > 0)    //檢查上傳檔案大小，確定是有上傳，不然CheckImageMIME會出錯
                {
                    //2.存放上傳圖片(1實體檔,2檔名,3路徑)
                    m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _DonateRelatedItem.ImageName, _RootPath_ImageFolderPath);
                }
            }

            //2將編輯內容圖片裏剛產生的Knowledge_image表裏的圖檔尚未關連此文章_knowledge_Content.id更新進去
            HandleContentImages(_request.Form["cNameArr"], _RootPath_ImageFolderPath, _DonateRelatedItem.donateRelatedItemId);

            //3.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);


            return _DonateRelatedItem;
        }
        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="_request">讀取表單資料</param>
        /// <returns>_DonateRelatedItem</returns>
        private DonateRelatedItem Request_data(HttpRequest _request)
        {
            DonateRelatedItem _DonateRelatedItem = new DonateRelatedItem();
            _DonateRelatedItem.donateRelatedItemId = Guid.NewGuid();
            _DonateRelatedItem.primary = _request.Form["primary"];
            _DonateRelatedItem.secondary = _request.Form["secondary"];

            if (_request.Files.Count > 0)
            {
                if (_request.Files[0].ContentLength > 0)
                {
                    _DonateRelatedItem.ImageName = _DonateRelatedItem.donateRelatedItemId + ".png";
                }
            }
            _DonateRelatedItem.Title = _request.Form["title"];
            _DonateRelatedItem.Content1 = _request.Form["content1"];
            _DonateRelatedItem.Content2 = _request.Form["content2"];
            _DonateRelatedItem.content3 = _request.Form["content3"];

            _DonateRelatedItem.Amount = Convert.ToInt32(_request.Form["Amount"]);
            _DonateRelatedItem.Notes = _request.Form["Notes"];

            _DonateRelatedItem.first = Convert.ToBoolean(Convert.ToByte(_request.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            _DonateRelatedItem.Sort = Convert.ToInt32(_request.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            _DonateRelatedItem.enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            _DonateRelatedItem.CreateDate = DateTime.Now;

            return _DonateRelatedItem;
        }
        #endregion

        #region 取得一筆資料
        public DonateRelatedItem Get_DonateRelatedItem(Guid id)
        {
            DonateRelatedItem _DonateRelatedItem = new DonateRelatedItem();
            _DonateRelatedItem.donateRelatedItemId = id;

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 ";
            string _sql = $"SELECT * FROM [DonateRelatedItem] WHERE [donateRelatedItemId] = @donateRelatedItemId {adminQuery}";
            _DonateRelatedItem = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _DonateRelatedItem);

            if (_DonateRelatedItem != null)
            {
                if (_DonateRelatedItem.ImageName !=null)
                {
                    _DonateRelatedItem.ImageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().DonateRelatedItem, _DonateRelatedItem.ImageName);
                }
            }
            return _DonateRelatedItem;
        }
        #endregion

        #region 更新一筆資料
        /// <summary>
        /// 更新一筆資料
        /// </summary>
        /// <param name="_request">用戶端的要求資訊</param>
        /// <param name="_DonateRelatedItem">更新的資料類別</param>
        public void Update_DonateRelatedItem(HttpRequest _request, DonateRelatedItem _DonateRelatedItem)
        {
            _DonateRelatedItem = Request_data_mod(_request, _DonateRelatedItem);

            #region 辨斷有沒有上傳檔案
            //1.辨斷有沒有上傳檔案
            if (_request.Files.Count > 0)       //1有上傳時Request_data_mod就會將_DonateRelatedItem.ImageName = _DonateRelatedItem.Id + ".png";
            {
                //處理圖片
                //1.取圖片路徑
                string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DonateRelatedItem);

                //Get_DonateRelatedItem裏面的ImageName有加網址m_ImageFileHelper.GetImageLink, 所以要將網址除掉
                string _ImageName = Path.GetFileName(_DonateRelatedItem.ImageName);

                //2.存放上傳圖片(1實體檔,2檔名,3路徑)
                m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _ImageName, _RootPath_ImageFolderPath);
            }
            else if (_DonateRelatedItem.ImageName != null)  //2沒上傳檔案，辨斷是否有原圖檔
            {
                //Get_DonateRelatedItem裏面的ImageName有加網址m_ImageFileHelper.GetImageLink, 所以要將網址除掉
                _DonateRelatedItem.ImageName = Path.GetFileName(_DonateRelatedItem.ImageName);
            }
            #endregion

            string pic = string.Empty;
            if (_request.Form["ImageName"] == "null")  //按刪除清除圖片
            {
                //_DonateRelatedItem.ImageName = null;
                pic += ",[ImageName]=NULL";
            }
            else if (_DonateRelatedItem.ImageName != null)  //是否有原圖，或上傳檔案
            {
                pic += ",[ImageName] = @ImageName";
            }

            //2.處理資料庫更新資料
            string _sql = $@"UPDATE [DonateRelatedItem]
                                        SET 
                                             [primary] = @primary
                                            ,[secondary]  = @secondary
                                            ,[Title]  = @Title
                                             {pic} 
                                            ,[Content1] = @Content1
                                            ,[Content2] = @Content2
                                            ,[content3] = @content3
                                            ,[Amount] = @Amount
                                            ,[Notes] = @Notes
                                            ,[Sort] = @Sort
                                            ,[first] = @first
                                            ,[enabled] = @enabled
                                            ,[updatedDate] = @updatedDate
                                        WHERE [donateRelatedItemId] = @donateRelatedItemId";
            _IDapperHelper.ExecuteSql(_sql, _DonateRelatedItem);
        }
        public DonateRelatedItem Request_data_mod(HttpRequest _request, DonateRelatedItem _DonateRelatedItem)
        {
            _DonateRelatedItem.primary = _request.Form["primary"];
            _DonateRelatedItem.secondary = _request.Form["secondary"];

            if (_request.Files.Count > 0)  //若有重新上傳檔案時
            {
                if (_request.Files[0].ContentLength > 0)
                {
                    _DonateRelatedItem.ImageName = _DonateRelatedItem.donateRelatedItemId + ".png";
                }
            }
            _DonateRelatedItem.Title = _request.Form["title"];
            _DonateRelatedItem.Content1 = _request.Form["content1"];
            _DonateRelatedItem.Content2 = _request.Form["content2"];
            _DonateRelatedItem.content3 = _request.Form["content3"];

            _DonateRelatedItem.Amount = Convert.ToInt32(_request.Form["Amount"]);
            _DonateRelatedItem.Notes = _request.Form["Notes"];

            _DonateRelatedItem.first = Convert.ToBoolean(Convert.ToByte(_request.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            _DonateRelatedItem.Sort = Convert.ToInt32(_request.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            _DonateRelatedItem.enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)

            _DonateRelatedItem.updatedDate = DateTime.Now;

            return _DonateRelatedItem;
        }
        #endregion

        # region 刪除訂購明細項目資料
        public void Delete_DonateRelatedItem(DonateRelatedItem _DonateRelatedItem)
        {
            //1. 取得放置圖片的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DonateRelatedItem);

            //2.取得這筆內容下相關內容圖片檔名清單
            string _sql = @"SELECT [imageName] FROM [DonateRelatedItemImage] WHERE donateRelatedItemId = @donateRelatedItemId";

            //調出此筆內容下的多筆圖片名稱，到List<string>
            List<string> _image_name_list = _IDapperHelper.QuerySetSql<DonateRelatedItem, string>(_sql, _DonateRelatedItem).ToList();

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

            //3.刪除內容圖片
            if (_DonateRelatedItem.ImageName != null)
            {
                string _image_name = Path.GetFileName(_DonateRelatedItem.ImageName);   //取出內容圖片
                string _image_name_FilePath = Path.Combine(_RootPath_ImageFolderPath, _image_name);  //加上檔案路徑
                //刪除圖片
                if (File.Exists(_image_name_FilePath))
                {
                    File.Delete(_image_name_FilePath);
                }
            }
            //4.刪除資料庫內容 資料庫有設定重疊顯示，所以刪主鍵關連DonateRelatedItemImage資料會一同刪除
            _sql = @"DELETE FROM [DonateRelatedItem] WHERE [donateRelatedItemId] = @donateRelatedItemId ";
            //執行刪除
            _IDapperHelper.ExecuteSql(_sql, _DonateRelatedItem);
        }
        #endregion

        #region 取得所有資料
        public List<DonateRelatedItem> Get_DonateRelatedItem_ALL(string primary, int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            //string sqlfirst_sql1 = string.Empty;  //置頂設定2結緣捐贈
            //string sqlfirst_sql2 = string.Empty;  //置頂設定2結緣捐贈
            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }

            //2結緣捐贈才有一筆置頂
            //if (primary == "2" && first != null && first == 1)
            //{
            //    sqlfirst_sql1 = @"top 1";
            //    sqlfirst_sql2 = @"AND [first] = 'true'";
                //TOP 不可使用在 OFFSET 的同一個查詢或子查詢。
            //    page_sql = ""; //清除
            //}

            string adminQuery = Auth.Role.IsAdmin ? $" WHERE [primary] = {primary} " : $" WHERE ENABLED = 1 AND [primary] = {primary} ";
            string _sql = $"SELECT * FROM [DonateRelatedItem] {adminQuery} " +
                                  @"ORDER BY [FIRST] DESC, [secondary] , CreateDate DESC , SORT " +
                                  $" {page_sql} ";

            List<DonateRelatedItem> _DonateRelatedItems = _IDapperHelper.QuerySetSql<DonateRelatedItem>(_sql).ToList();

           //轉換image_name加上url
            for (int i = 0; i < _DonateRelatedItems.Count; i++)
            {
                if (_DonateRelatedItems[i].ImageName != null)  //封面圖片檔案沒有必填，所以沒有上傳的就不加網址
                {
                    _DonateRelatedItems[i].ImageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().DonateRelatedItem, _DonateRelatedItems[i].ImageName);
                }
            }
            return _DonateRelatedItems;
        }
        #endregion

        #region 新增修改編輯器圖片
        /// <summary>
        /// 1.新增編輯器圖片
        /// </summary>
        /// <param name="request"></param>
        /// <returns>donateRelatedItemImage</returns>
        public DonateRelatedItemImage AddUploadImage(HttpRequest request)
        {
            DonateRelatedItemImage donateRelatedItemImage = new DonateRelatedItemImage();
            donateRelatedItemImage.donateRelatedItemImageId = Guid.NewGuid();                                         //主索引鍵（P.K.）自動產生一個Guid
            donateRelatedItemImage.imageName = donateRelatedItemImage.donateRelatedItemImageId + ".png";
            //donateRelatedItemImage.donateRelatedItemId = "關連內容donateRelatedItemId尚未產生，進入新增狀態時尚未送出所以沒有值";

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DonateRelatedItem);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(request.Files[0], donateRelatedItemImage.imageName, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [DonateRelatedItemImage]
                                            ([donateRelatedItemImageId]
                                            ,[imageName])
                                        VALUES
                                            (@donateRelatedItemImageId,
                                             @imageName )
                                                SELECT SCOPE_IDENTITY()";

            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, donateRelatedItemImage);        //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            donateRelatedItemImage.id = id; //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            donateRelatedItemImage.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().DonateRelatedItem, donateRelatedItemImage.imageName);

            return donateRelatedItemImage;
        }

        /// <summary>
        /// 2.新增編輯器圖片(修改時使用)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="donateRelatedItemId"></param>
        /// <returns>directorIntroductionImage</returns>
        public DonateRelatedItemImage AddUploadImage(HttpRequest request, Guid donateRelatedItemId)
        {
            DonateRelatedItemImage donateRelatedItemImage = new DonateRelatedItemImage();
            donateRelatedItemImage.donateRelatedItemImageId = Guid.NewGuid();          //主索引鍵（P.K.）自動產生一個Guid
            donateRelatedItemImage.imageName = donateRelatedItemImage.donateRelatedItemImageId + ".png";
            donateRelatedItemImage.donateRelatedItemId = donateRelatedItemId;          //關聯內容donateRelatedItemId

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().DonateRelatedItem);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(request.Files[0], donateRelatedItemImage.imageName, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [DonateRelatedItemImage]
                                            ([donateRelatedItemImageId]
                                            ,[imageName]
                                            ,[donateRelatedItemId])
                                        VALUES
                                            (@donateRelatedItemImageId,
                                             @imageName, 
                                             @donateRelatedItemId )
                                                SELECT SCOPE_IDENTITY()";

            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, donateRelatedItemImage);        //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            donateRelatedItemImage.id = id; //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            donateRelatedItemImage.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().DonateRelatedItem, donateRelatedItemImage.imageName);

            return donateRelatedItemImage;
        }
        #endregion

        #region 工具區
        /// <summary>
        /// 在新增內容時，下面內文若有插入圖片，插入的圖片還沒有knowledgetId, 送出前cNameArr收集了內容裏插入了多少張圖
        /// 然後在送出後在將確定有插入的內容圖片更新Knowledge_image.knowledgetId=knowledgetId
        /// 為何不先給插入圖片id，因為插入圖片隨時可能又del，當del掉後若已給id就無法用null來辨斷刪除
        /// </summary>
        /// <param name="images">_request.Form["cNameArr"]內容圖片的檔名Json字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="id">關連內容的ID</param>
        private void HandleContentImages(string images, string saveFolderPath, Guid id)
        {
            //取得回傳內容圖片的檔名cNameArr["abc.png","def.png"]， Json反序列化(Deserialize)為物件(Object)
            List<string> _retFileNameList = JsonConvert.DeserializeObject<List<string>>(images);

            //取得存放saveFolderPath目錄下所有.png圖檔
            //GetFiles (string path, string searchPattern);
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();
            if (_retFileNameList != null && _retFileNameList.Count > 0)   //cNameArr確認有檔才處理
            {
                for (int i = 0; i < _retFileNameList.Count; i++)  //處理裏面的所有檔cNameArr ["abc.png","def.png"]
                {
                    for (int y = 0; y < _imageFileList.Count; y++)  //cNameArr和目錄下實際所有.png圖檔比對一下
                    {
                        string _urlFileName = _retFileNameList[i].ToLower().Trim(); //cNameArr內的檔轉小寫去空白
                        string _entityFileName = Path.GetFileName(_imageFileList[y]);  //目錄下的檔只需留檔案名稱
                        string _imageGuid = Path.GetFileNameWithoutExtension(_imageFileList[y]);  //傳回[沒有副檔名]的指定路徑字串的[檔案名稱]。

                        //判斷cNameArr[]傳進來的參數圖檔==目前目錄下的檔&&判斷目錄下的檔案是否存在。
                        if (_urlFileName == _entityFileName && File.Exists(_imageFileList[y]))
                        {
                            DonateRelatedItemImage donateRelatedItemImage = new DonateRelatedItemImage();
                            donateRelatedItemImage.donateRelatedItemImageId = Guid.Parse(_imageGuid);                //存放目錄下檔名不含副檔給Knowledge_image表裏id
                            donateRelatedItemImage.donateRelatedItemId = id;                  //關聯內容id

                            //更新內容的id給Knowledge_image，
                            string _sql = @"UPDATE [DonateRelatedItemImage]
                                                            SET [donateRelatedItemId] = @donateRelatedItemId
                                                             WHERE donateRelatedItemImageId = @donateRelatedItemImageId";
                            _IDapperHelper.ExecuteSql(_sql, donateRelatedItemImage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刪除[knowledgetId] IS NULL不用的內容圖片
        /// </summary>
        /// <param name="saveFolderPath"></param>
        private void RemoveContentImages(string saveFolderPath)
        {
            //取得存放磁碟目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();

            //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
            string _sql = @"SELECT [imageName] FROM [DonateRelatedItemImage] WHERE donateRelatedItemId IS NULL";
            List<string> _imageDataBaseList = _IDapperHelper.QuerySetSql<string>(_sql).ToList();

            //刪除多餘的內容圖檔, _imageDataBaseList資料庫檔案名清單，_imageFileList實際目錄檔路徑檔案清單
            for (int i = 0; i < _imageDataBaseList.Count; i++)
            {
                for (int y = 0; y < _imageFileList.Count; y++)
                {
                    string _removeFileName = Path.GetFileName(_imageFileList[i]);  //除掉路徑留檔案名稱
                    //資料庫NULL檔==存放檔&&存放檔是否存在
                    if (_imageDataBaseList[y] == _removeFileName && File.Exists(_imageFileList[i]))
                    {
                        File.Delete(_imageFileList[i]);
                    }
                }
            }
            //刪除資料庫裡無相關連的內容圖片記錄
            _sql = @"DELETE FROM [DonateRelatedItemImage] WHERE [donateRelatedItemId] IS NULL";

            _IDapperHelper.ExecuteSql(_sql);
        }
        #endregion
    }
}