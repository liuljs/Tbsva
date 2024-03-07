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
    public class KnowledgeService : IKnowledgeService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dapperHelper"></param>
        /// <param name="imageFileHelper"></param>
        public KnowledgeService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            _IDapperHelper = dapperHelper;
            m_ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="_request">用戶端送來的表單資訊轉入類別資料</param>
        /// <returns></returns>
        public Knowledge_content Insert_Knowledge(HttpRequest _request)
        {
            Knowledge_content _knowledge_Content = Request_data(_request);  //將接收來的參數轉進_knowledge_Content型別
            string _sql = @"INSERT INTO [KNOWLEDGE_CONTENT]
                                                            ([KNOWLEDGETID]
                                                            ,[IMAGENAME]
                                                            ,[TITLE]
                                                            ,[CATEGORY]
                                                            ,[NUMBER]
                                                            ,[BRIEF]
                                                            ,[CONTENT]
                                                            ,[CREATIONDATE]
                                                            ,[FIRST]
                                                            ,[SORT]
                                                            ,[ENABLED])
                                                        VALUES
                                                            ( @KNOWLEDGETID,
                                                              @IMAGENAME,
                                                              @TITLE,
                                                              @CATEGORY,
                                                              @NUMBER,
                                                              @BRIEF,
                                                              @CONTENT,
                                                              @CREATIONDATE,
                                                              @FIRST,
                                                              @SORT,
                                                              @ENABLED )  
                                                            SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。

            int id = _IDapperHelper.QuerySingle(_sql, _knowledge_Content);       //新增資料
            _knowledge_Content.id = id;

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(_request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                _knowledge_Content.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                _knowledge_Content.sort = Convert.ToInt32(_request.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [KNOWLEDGE_CONTENT] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            _IDapperHelper.ExecuteSql(_sql, _knowledge_Content);

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Knowledge);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _knowledge_Content.imageName, _RootPath_ImageFolderPath);
            //3.0內容若有新增插入一個圖片

            //3.1將編輯內容圖片裏剛產生的Knowledge_image表裏的圖檔尚未關連此文章_knowledge_Content.id更新進去
            HandleContentImages(_request.Form["cNameArr"], _RootPath_ImageFolderPath, _knowledge_Content.knowledgetId);

            //4.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);

            return _knowledge_Content;
        }

        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="_request">讀取表單資料</param>
        /// <returns>回傳表單類別_Knowledge_Content</returns>
        private Knowledge_content Request_data(HttpRequest _request)
        {
            Knowledge_content _knowledge_Content = new Knowledge_content();
            _knowledge_Content.knowledgetId = Guid.NewGuid();                                                      //內容的id
            _knowledge_Content.imageName = _knowledge_Content.knowledgetId + ".png";       //圖片檔名使用id來當
            _knowledge_Content.title = _request.Form["title"];                                                             //標題
            _knowledge_Content.category = _request.Form["category"];                                              //位階
            _knowledge_Content.number = _request.Form["number"];                                              //戒牒號碼
            _knowledge_Content.brief = _request.Form["brief"];                                                          //簡述
            _knowledge_Content.content = _request.Form["content"];                                               //內容
            _knowledge_Content.first = Convert.ToBoolean(Convert.ToByte(_request.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            _knowledge_Content.sort = Convert.ToInt32(_request.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            _knowledge_Content.enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            _knowledge_Content.creationDate = DateTime.Now;

            return _knowledge_Content;
        }

        /// <summary>
        /// 新增一個[Knowledge_image內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 因為正在新增內容還未送出，所以內容的knowledgetId關連id還沒產生
        /// </summary>
        /// <param name="_request">點選內文裏的圖片icon</param>
        /// <returns>_imageUrl</returns>
        public Knowledge_image AddUploadImage(HttpRequest _request)
        {
            Knowledge_image _knowledge_Image = new Knowledge_image();
            _knowledge_Image.knowledgeImageId = Guid.NewGuid();                                               //自動產生一個Guid
            _knowledge_Image.imageName = _knowledge_Image.knowledgeImageId + ".png";  //用id來命名圖檔

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Knowledge);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _knowledge_Image.imageName, _RootPath_ImageFolderPath);

            //3.取得要回傳的網址
            //string _imageUrl = Tools.GetInstance().GetImageLink(Tools.GetInstance().Knowledge, _knowledge_Image.imageName);

            //4.加入資料庫
            string _sql = @"INSERT INTO [KNOWLEDGE_IMAGE]
                                            ([KNOWLEDGEIMAGEID]
                                            ,[IMAGENAME])
                                        VALUES
                                            (@KNOWLEDGEIMAGEID,
                                             @IMAGENAME )
                                     SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, _knowledge_Image);  //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            _knowledge_Image.id = id ;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            _knowledge_Image.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().Knowledge, _knowledge_Image.imageName);

            return _knowledge_Image;
        }
        #endregion

        #region 取得一筆資料
        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="knowledgetId">輸入內容knowledgetId編號</param>
        /// <returns>_knowledge_Content</returns>
        public Knowledge_content Get_Knowledge(Guid knowledgetId)
        {
            Knowledge_content _knowledge_Content = new Knowledge_content();
            _knowledge_Content.knowledgetId = knowledgetId;

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 ";
            string _sql = $"SELECT * FROM [KNOWLEDGE_CONTENT] WHERE [KNOWLEDGETID]=@KNOWLEDGETID {adminQuery} ";

            _knowledge_Content = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _knowledge_Content);

            //_knowledge_Content.imageName加上http網址
            if (_knowledge_Content != null)
            {
                _knowledge_Content.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().Knowledge, _knowledge_Content.imageName);
            }
            return _knowledge_Content;
        }
        #endregion

        #region 更新一筆資料
        /// <summary>
        /// 更新一筆資料
        /// </summary>
        /// <param name="_request">用戶端的要求資訊</param>
        /// <param name="_Knowledge_content">更新的資料類別</param>
        public void Update_Knowledge(HttpRequest _request, Knowledge_content _Knowledge_content)
        {
            _Knowledge_content = Request_data_mod(_request, _Knowledge_content);

            //1.辨斷有沒有上傳檔案
            if (_request.Files.Count > 0)
            {
                //以下處理圖片
                //1. 取得要放置的目錄路徑
                string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Knowledge);

                //Get_Knowledge_content裏面的imageName有加網址m_ImageFileHelper.GetImageLink，改只取檔案名與副檔
                string _image_name = Path.GetFileName(_Knowledge_content.imageName);

                //2.存放上傳文章封面圖片(1實體檔,2檔名,3路徑)
                m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _image_name, _RootPath_ImageFolderPath);
            }

            //Get_Knowledge取出的資料裏的有加http://, 要去除只留檔名
            _Knowledge_content.imageName = Path.GetFileName(_Knowledge_content.imageName);

            //2.處理資料庫更新資料
            string _sql = @"UPDATE [KNOWLEDGE_CONTENT]
                                        SET 
                                             [IMAGENAME] = @IMAGENAME
                                            ,[TITLE] = @TITLE
                                            ,[CATEGORY] = @CATEGORY
                                            ,[NUMBER] = @NUMBER
                                            ,[BRIEF] = @BRIEF
                                            ,[CONTENT] = @CONTENT
                                            ,[FIRST] = @FIRST
                                            ,[SORT] = @SORT
                                            ,[ENABLED] = @ENABLED
                                            ,[UPDATEDDATE] = @UPDATEDDATE
                                        WHERE [KNOWLEDGETID] = @KNOWLEDGETID";
            _IDapperHelper.ExecuteSql(_sql, _Knowledge_content);
        }
         /// <summary>
        /// 讀取表單資料,轉到_knowledge_Content
        /// </summary>
        /// <param name="_request">讀取表單資料(修改時用)</param>
        /// <param name="_knowledge_Content">控制器取到資料</param>
        /// <returns></returns>
        public Knowledge_content Request_data_mod(HttpRequest _request, Knowledge_content _knowledge_Content)
        {
            _knowledge_Content.title = _request.Form["title"];
            _knowledge_Content.category = _request.Form["category"];                                                                       //位階
            _knowledge_Content.number = _request.Form["number"];                                                                       //戒牒號碼
            _knowledge_Content.brief = _request.Form["brief"];                                                                                   //簡述
            _knowledge_Content.content = _request.Form["content"];                                                                        //內容
            _knowledge_Content.first = Convert.ToBoolean(Convert.ToByte(_request.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            _knowledge_Content.sort = Convert.ToInt32(_request.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            _knowledge_Content.enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            _knowledge_Content.updatedDate = DateTime.Now;

            return _knowledge_Content;
        }

        #endregion


        /// <summary>
        /// 新增一個Knowledge_image內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 修改時用
        /// </summary>
        /// <param name="_request">點選內文裏的圖片新增</param>
        /// <param name="Knowledge_content_Id">關連內容的id</param>
        /// <returns>_imageUrl</returns>
        public Knowledge_image AddUploadImage(HttpRequest _request, Guid Knowledge_content_Id)
        {
            Knowledge_image _knowledge_Image = new Knowledge_image();
            _knowledge_Image.knowledgeImageId = Guid.NewGuid();                                                     //自動產生一個Guid
            _knowledge_Image.imageName = _knowledge_Image.knowledgeImageId + ".png";       //用id來命名圖檔
            _knowledge_Image.knowledgetId = Knowledge_content_Id;  //關連內容的id

            //1. 取得要放置的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Knowledge);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _knowledge_Image.imageName, _RootPath_ImageFolderPath);

            //3.取得要回傳的網址
            //string _imageUrl = Tools.GetInstance().GetImageLink(Tools.GetInstance().Knowledge, _knowledge_Image.imageName);

            //4.加入資料庫, 這裏會給@knowledgetId值，這樣Knowledge_image的knowledgetId就會有值
            string _sql = @"INSERT INTO [KNOWLEDGE_IMAGE]
                                            ([KNOWLEDGEIMAGEID]
                                            ,[IMAGENAME]
                                            ,[KNOWLEDGETID])
                                        VALUES
                                            (@KNOWLEDGEIMAGEID,
                                             @IMAGENAME,
                                             @KNOWLEDGETID )
                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, _knowledge_Image);     //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            _knowledge_Image.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            _knowledge_Image.imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().Knowledge, _knowledge_Image.imageName);

            return _knowledge_Image;
        }

        /// <summary>
        /// 刪除內容與內容圖片
        /// </summary>
        /// <param name="_Knowledge_content"></param>
        public void Delete_Knowledge(Knowledge_content _Knowledge_content)
        {
            //1. 取得放置圖片的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Knowledge);

            //取得這筆內容下相關內容圖片檔名清單
            string _sql = @"SELECT [IMAGENAME] FROM [KNOWLEDGE_IMAGE] WHERE KNOWLEDGETID = @KNOWLEDGETID";
            //輸出<TReturn> 宣告<T1，TReturn輸出>(傳入字串，T1傳入類別)
            //IEnumerable<TReturn> QuerySetSql<T1, TReturn>(string sql, T1 paramData)
            //用內容的類別_Knowledge_content，來找出Knowledge_image表格條件knowledgetId = @id的相關image_name list所有圖檔
            List<string> _Knowledge_image_name_List = _IDapperHelper.QuerySetSql<Knowledge_content, string>(_sql, _Knowledge_content).ToList();

            //刪除內容下內容圖, List<string> _Knowledge_image_name_List
            //string _Knowledge_image_name_List_Path = string.Empty; //宣告一個變數之後要加入路徑
            for (int i = 0; i < _Knowledge_image_name_List.Count; i++)
            {
                //路徑與圖檔結合
                string _Knowledge_image_name_List_Path = Path.Combine(_RootPath_ImageFolderPath, _Knowledge_image_name_List[i]);
                if (File.Exists(_Knowledge_image_name_List_Path))
                {
                    File.Delete(_Knowledge_image_name_List_Path);
                }
            }

            //刪除內容封面
            //取出文章圖片檔案
            string _image_name = Path.GetFileName(_Knowledge_content.imageName);  //取出檔名
            string _image_name_FilePath = Path.Combine(_RootPath_ImageFolderPath, _image_name);  //檔案加上路徑
            if (File.Exists(_image_name_FilePath))
            {
                File.Delete(_image_name_FilePath);
            }

            //刪除資料庫內容
            _sql = @"DELETE FROM [KNOWLEDGE_CONTENT] WHERE [KNOWLEDGETID] = @KNOWLEDGETID";
            //資料庫有設定重疊顯示，所以刪主鍵關連資料會一同刪除

            _IDapperHelper.ExecuteSql(_sql, _Knowledge_content);
        }


        public List<Knowledge_content> Get_Knowledge_ALL(int? _count, int? _page, string _category)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string search_sql = string.Empty;

            if (_count != null && _count > 0 && _page != null && _page > 0)
            {
                int startRowjumpover = 0;  //預設跳過筆數
                startRowjumpover = Convert.ToInt32((_page - 1) * _count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {_count}  ROWS ONLY ";
            }

            if (!string.IsNullOrWhiteSpace(_category))
            {
                search_sql += $" And category = '{_category}' ";
            }
            string adminQuery = Auth.Role.IsAdmin ? " where 1=1 " : " where Enabled = 1 ";   //登入取得所有資料:未登入只能取得上線資料
            string _sql = $"Select * from Knowledge_content {adminQuery} " +
                                 $" {search_sql} " +
                                 @"Order by [FIRST] DESC , Sort , creationDate DESC " +
                                 $"{page_sql}";
            List<Knowledge_content> _knowledge_Contents = _IDapperHelper.QuerySetSql<Knowledge_content>(_sql).ToList();
            //轉換image_name加上url;
            for (int i = 0; i < _knowledge_Contents.Count; i++)
            {
                _knowledge_Contents[i].imageName = Tools.GetInstance().GetImageLink(Tools.GetInstance().Knowledge, _knowledge_Contents[i].imageName);
            }

            return _knowledge_Contents;
        }


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
                            Knowledge_image _knowledge_Image = new Knowledge_image();
                            _knowledge_Image.knowledgeImageId = Guid.Parse(_imageGuid);                //存放目錄下檔名不含副檔給Knowledge_image表裏id
                            _knowledge_Image.knowledgetId = id;                  //關聯內容id

                            //更新內容的id給Knowledge_image，
                            string _sql = @"UPDATE [Knowledge_image]
                                                            SET [knowledgetId] = @knowledgetId
                                                             WHERE knowledgeImageId = @knowledgeImageId";
                            _IDapperHelper.ExecuteSql(_sql, _knowledge_Image);
                            
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
            string _sql = @"SELECT [imageName] FROM [Knowledge_image] WHERE knowledgetId IS NULL";
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
            _sql = @"DELETE FROM [Knowledge_image] WHERE [knowledgetId] IS NULL";

            _IDapperHelper.ExecuteSql(_sql);
        }
        #endregion

    }
}