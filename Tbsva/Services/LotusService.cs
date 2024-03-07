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
    public class LotusService : ILotusService
    {
        #region DI依賴注入功能
        private IDapperHelper DapperHelper;
        private IImageFileHelper ImageFileHelper;
        public LotusService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            DapperHelper = dapperHelper;
            ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        public Lotus InsertLotus(HttpRequest httpRequest)
        {
            Lotus lotus = RequestData(httpRequest);

            //設定上傳檔案的檔名對應相同的類別名稱
            //HttpRequest.Files 屬性 取得用戶端所上傳的檔案集合
            //httpRequest.Files 將文件集合加載到 HttpFileCollection 變量中。
            ImageFileHelper.SetImageFileName(lotus, httpRequest.Files);

            //若要新增有置頂資料前先關閉所有置頂資料
            if (lotus.first)                 //bool是否置頂(0 關閉;1 開啟))
            {
                string _sql_top = "UPDATE [lotus] SET [first] = 0";
                DapperHelper.ExecuteSql(_sql_top);
            }

            string _sql = @" INSERT INTO [lotus] 
                                                       ([lotus_id]
                                                       ,[image_name]
                                                       ,[title]
                                                       ,[subTitle]
                                                       ,[brief]
                                                       ,[number]
                                                       ,[recommendTitle]
                                                       ,[recommend]
                                                       ,[content]
                                                       ,[first]
                                                       ,[sort]
                                                       ,[enabled]
                                                       ,[createDate]
                                                       ,[information01]
                                                       ,[information02]
                                                       ,[information03]
                                                       ,[information04]
                                                       ,[information05]
                                                       ,[information06]
                                                       ,[information07]
                                                       ,[information08]
                                                       ,[navPics01]
                                                       ,[navPics02]
                                                       ,[navPics03]
                                                       ,[navPics04]
                                                       ,[navPics05]
                                                       ,[navPics06]
                                                       ,[navPics07]
                                                       ,[navPics08])
                                                 VALUES
                                                       (@lotus_id,
                                                       @image_name,
                                                       @title,
                                                       @subTitle,
                                                       @brief,
                                                       @number,
                                                       @recommendTitle,
                                                       @recommend,
                                                       @content,
                                                       @first,
                                                       @sort,
                                                       @enabled,
                                                       @createDate,
                                                       @information01,
                                                       @information02,
                                                       @information03,
                                                       @information04,
                                                       @information05,
                                                       @information06,
                                                       @information07,
                                                       @information08,
                                                       @navPics01,
                                                       @navPics02,
                                                       @navPics03,
                                                       @navPics04,
                                                       @navPics05,
                                                       @navPics06,
                                                       @navPics07,
                                                       @navPics08 )
                                                         SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。

            int id = DapperHelper.QuerySingle(_sql, lotus);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            lotus.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(httpRequest.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                lotus.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                lotus.sort = Convert.ToInt32(httpRequest.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [Lotus] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            DapperHelper.ExecuteSql(_sql, lotus);         //更新排序

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lotus);
            string _saveFolderPath = ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{id}\");

            //2.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            ImageFileHelper.SaveMoreUploadImageFile(httpRequest.Files, _saveFolderPath);

            //3.1將編輯內容圖片裏剛產生的LotusImage表裏的圖檔尚未關連此lotus.Id更新進去
            HandleContentImages(httpRequest.Form["cNameArr"], _RootPath_ImageFolderPath, lotus.lotus_id);

            //4.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);

            return lotus;
        }
        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="httpRequest">讀取表單資料</param>
        /// <returns>回傳表單類別</returns>
        private Lotus RequestData(HttpRequest httpRequest)
        {
            Lotus lotus = new Lotus();
            lotus.lotus_id = Guid.NewGuid();                                                                                            //類別另設 ID 唯一索引用
            //cover圖片另處理
            lotus.title = httpRequest.Form["title"];                                                                                   //標題
            lotus.subTitle = httpRequest.Form["subTitle"];                                                                    //副標題
            lotus.brief = httpRequest.Form["brief"];                                                                              //簡述 
            lotus.number = httpRequest.Form["number"];                                                                   //文集冊號
            lotus.recommendTitle = httpRequest.Form["recommendTitle"];                                      //推薦標題
            lotus.recommend = httpRequest.Form["recommend"];                                                     //推薦序
            lotus.content = httpRequest.Form["content"];                                                                     //內容
            lotus.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                 //是否置頂(0 關閉;1 開啟)
            lotus.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                    //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            lotus.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            lotus.createDate = DateTime.Now;

            lotus.information01 = httpRequest.Form["information01"];  //文集資訊欄位1-8
            lotus.information02 = httpRequest.Form["information02"];
            lotus.information03 = httpRequest.Form["information03"];
            lotus.information04 = httpRequest.Form["information04"];
            lotus.information05 = httpRequest.Form["information05"];
            lotus.information06 = httpRequest.Form["information06"];
            lotus.information07 = httpRequest.Form["information07"];
            lotus.information08 = httpRequest.Form["information08"];

            return lotus;
        }
        #endregion

        #region 取得一筆資料
        public Lotus GetLotus(Guid lotusId)
        {
            Lotus lotus = new Lotus();
            lotus.lotus_id = lotusId;      //類別另設 ID
            //2021 / 11 / 30   2021-12-01 13:59:01  2021 / 12 / 02
            //Start_Date <= getdate() < End_Date +1 End_Date 是當天上午12:00 => 00:00
            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 ";
            string _sql = $"SELECT * FROM [Lotus] WHERE [lotus_id] = @lotus_id {adminQuery}";
            lotus = DapperHelper.QuerySqlFirstOrDefault(_sql, lotus);

            //加上http網址
            if (lotus != null)  //確定有資料在處理
            {
                string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
                SystemFunctions.picaddhttp(fields, lotus, Tools.GetInstance().Lotus, lotus.id);
            }
            return lotus;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateLotus(HttpRequest httpRequest, Lotus lotus)
        {
            lotus = RequestDataMod(httpRequest, lotus); //將接收來的參數，和抓到的要修改資料合併

            //1.設定上傳檔案的檔名對應相同的類別名稱
            //HttpRequest.Files 屬性 取得用戶端所上傳的檔案集合
            //httpRequest.Files 將文件集合加載到 HttpFileCollection 變量中。
            ImageFileHelper.SetImageFileName(lotus, httpRequest.Files);

            //2.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lotus);
            string _saveFolderPath = ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{lotus.id}\");

            //3.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            ImageFileHelper.SaveMoreUploadImageFile(httpRequest.Files, _saveFolderPath);

            //4.組裝修改刪除不變更時sql
            //string _sqlpic = picSql(lotus, httpRequest);
            string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
            string _sqlpic = SystemFunctions.picSql<Lotus>(fields, lotus, httpRequest);

            //若要新增有置頂資料前先關閉所有置頂資料
            if (lotus.first)                 //bool是否置頂(0 關閉;1 開啟))
            {
                string _sql_top = "UPDATE [lotus] SET [first] = 0";
                DapperHelper.ExecuteSql(_sql_top);
            }

            string _sql = $@" UPDATE [lotus]
                                                   SET
                                                       [title] = @title
                                                      ,[subTitle] = @subTitle
                                                      ,[brief] = @brief
                                                      ,[number] = @number
                                                      ,[recommendTitle] = @recommendTitle
                                                      ,[recommend] = @recommend
                                                      ,[content] = @content
                                                      ,[first] = @first
                                                      ,[sort] = @sort
                                                      ,[enabled] = @enabled
                                                      ,[updateDate] = @updateDate
                                                      ,[information01] = @information01
                                                      ,[information02] = @information02
                                                      ,[information03] = @information03
                                                      ,[information04] = @information04
                                                      ,[information05] = @information05
                                                      ,[information06] = @information06
                                                      ,[information07] = @information07
                                                      ,[information08] = @information08
                                                        {_sqlpic}
                                         WHERE [Lotus_ID] = @Lotus_ID ";

            DapperHelper.ExecuteSql(_sql, lotus);
        }
        /// <summary>
        /// 讀取表單資料,轉到lotus
        /// </summary>
        /// <param name="httpRequest">讀取表單資料(修改時用)</param>
        /// <param name="original_lotus">控制器取到資料</param>
        /// <returns>lotus</returns>
        private Lotus RequestDataMod(HttpRequest httpRequest, Lotus original_lotus)
        {
            Lotus lotus = new Lotus();                                                         //設定一組空的沒資料會null, 讓圖片預設是null不執行SQL指令，此null是類別裏真的null 
            lotus.id = original_lotus.id;
            lotus.lotus_id = original_lotus.lotus_id;
            //cover圖片另處理
            lotus.title = httpRequest.Form["title"];                                                                                    //標題
            lotus.subTitle = httpRequest.Form["subTitle"];                                                                     //副標題
            lotus.brief = httpRequest.Form["brief"];                                                                                //簡述 
            lotus.number = httpRequest.Form["number"];                                                                     //文集冊號
            lotus.recommendTitle = httpRequest.Form["recommendTitle"];                                        //推薦標題
            lotus.recommend = httpRequest.Form["recommend"];                                                       //推薦序
            lotus.content = httpRequest.Form["content"];                                                                       //內容
            lotus.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            lotus.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            lotus.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            lotus.updateDate = DateTime.Now;
            lotus.information01 = httpRequest.Form["information01"];  //文集資訊欄位1-8
            lotus.information02 = httpRequest.Form["information02"];
            lotus.information03 = httpRequest.Form["information03"];
            lotus.information04 = httpRequest.Form["information04"];
            lotus.information05 = httpRequest.Form["information05"];
            lotus.information06 = httpRequest.Form["information06"];
            lotus.information07 = httpRequest.Form["information07"];
            lotus.information08 = httpRequest.Form["information08"];
            return lotus;
        }
        #endregion

        #region 刪除一筆資料
        public void DeleteLotus(Lotus lotus)
        {
            //1. 取得放置圖片的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lotus);

            //取得這筆內容下相關內容圖片檔名清單
            string _sql = @"SELECT [IMAGE_NAME] FROM [lotus_image] WHERE lotus_id = @lotus_id";
            //調出此筆內容下的多筆圖片名稱，到List<string>
            List<string> lotus_image_name_list = DapperHelper.QuerySetSql<Lotus, string>(_sql, lotus).ToList();

            //刪除內容下內容圖, List<string> _lotus_image_name_list
            string lotus_image_name_list_path = string.Empty;  //宣告一個變數之後要加入路徑
            for (int i = 0; i < lotus_image_name_list.Count; i++)
            {
                //路徑與圖檔結合, 再辨斷存在就刪除
                lotus_image_name_list_path = Path.Combine(_RootPath_ImageFolderPath, lotus_image_name_list[i]);
                if (File.Exists(lotus_image_name_list_path))
                {
                    File.Delete(lotus_image_name_list_path);
                }
            }

            //刪除封面與導覽圖片(刪自動id目錄)
            string folder = Path.Combine(_RootPath_ImageFolderPath, lotus.id.ToString());
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            //刪除資料庫內容 資料庫有設定重疊顯示，所以刪主鍵關連lotus_IMAGE資料會一同刪除FK_lotus_image_lotus
            _sql = @"DELETE FROM [Lotus] WHERE lotus_id = @lotus_id";
            //執行刪除
            DapperHelper.ExecuteSql(_sql, lotus);
        }
        #endregion

        #region 取得分頁
        public List<Lotus> GetLotusAll(string title, string number, int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string search_sql = string.Empty;

            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }
            if (!string.IsNullOrWhiteSpace(title))
            {
                search_sql += $" And title like '%{title}%' ";
            }
            if (!string.IsNullOrWhiteSpace(number))
            {
                search_sql += $" And number = '{number}' ";
            }

            string adminQuery = Auth.Role.IsAdmin ? " WHERE 1 = 1 " : " WHERE ENABLED = 1 ";  //登入取得所有資料:未登入只能取得上線資料

            string _sql = @"SELECT * FROM [lotus] " +
                                  $" {adminQuery} " +
                                  $" {search_sql} " +
                                 @"ORDER BY [FIRST] DESC , createDate DESC , SORT " +
                                  $" {page_sql} ";
            List<Lotus> lotus = DapperHelper.QuerySetSql<Lotus>(_sql).ToList();

            //將資料庫image_name加上URL
            for (int i = 0; i < lotus.Count; i++)
            {
                //lotus[i].image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Lotus, lotus[i].image_name);
                string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
                SystemFunctions.picaddhttp(fields, lotus[i], Tools.GetInstance().Lotus, lotus[i].id);
            }
            return lotus;
        }
        #endregion

        /// <summary>
        /// 新增一個[NotusImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 因為正在新增內容還未送出，所以內容的LotusImage_lotus_id關連lotus.lotus_id還沒產生
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns>lotusImage</returns>
        public LotusImage AddUploadImage(HttpRequest httpRequest)
        {
            LotusImage lotusImage = new LotusImage();
            lotusImage.lotus_image_id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
            lotusImage.image_name = lotusImage.lotus_image_id + ".png";   //用lotus_image_id來命名圖檔
            //lotusImage.lotus_id = "關連內容lotus_id尚未產生，進入新增狀態時尚未送出所以沒有值";

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lotus);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], lotusImage.image_name, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [lotus_image]
                                            ([lotus_image_id]
                                            ,[image_name])
                                        VALUES
                                            (@lotus_image_id,
                                            @image_name )
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = DapperHelper.QuerySingle(_sql, lotusImage);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            lotusImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            lotusImage.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Lotus, lotusImage.image_name);

            return lotusImage;
        }

        /// <summary>
        /// 更新最新消息管理的內容圖片
        /// 新增一個[LotusImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="Lotus_Id"></param>
        /// <returns></returns>
        public LotusImage AddUploadImage(HttpRequest httpRequest, Guid Lotus_Id)
        {
            LotusImage lotusImage = new LotusImage();
            lotusImage.lotus_image_id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
            lotusImage.image_name = lotusImage.lotus_image_id + ".png";   //用lotus_image_id來命名圖檔
            lotusImage.lotus_id = Lotus_Id;                                                            //關聯內容lotus_id

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lotus);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], lotusImage.image_name, _RootPath_ImageFolderPath);

            //3.加入資料庫
            string _sql = @"INSERT INTO [lotus_image]
                                            ([lotus_image_id]
                                            ,[image_name]
                                            ,[lotus_id])
                                        VALUES
                                            (@lotus_image_id,
                                            @image_name,
                                            @lotus_id)
                                                SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = DapperHelper.QuerySingle(_sql, lotusImage);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            lotusImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            lotusImage.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Lotus, lotusImage.image_name);

            return lotusImage;
        }

        public int GetLotusTotalCount(string title, string number)
        {
            string search_sql = string.Empty;
            if (!string.IsNullOrWhiteSpace(title))
            {
                search_sql += $" And title like '%{title}%' ";
            }
            if (!string.IsNullOrWhiteSpace(number))
            {
                search_sql += $" And number = '{number}' ";
            }
            string adminQuery = Auth.Role.IsAdmin ? " WHERE 1 = 1 " : " WHERE ENABLED = 1 ";  //登入取得所有資料:未登入只能取得上線資料

            string _sql = @"SELECT COUNT(id) FROM [lotus] " +
                                  $" {adminQuery} " +
                                  $" {search_sql} ";
            int TotalCount = DapperHelper.ExecuteScalar(_sql);
            //int TotalCount = _IDapperHelper.QuerySqlFirstOrDefault<int>(_sql);終於搞懂前面的T
            //string _sql = @"SELECT * FROM [lotus] " +
            //                      $" {adminQuery} " +
            //                      $" {search_sql} " +
            //                     @"ORDER BY [FIRST] DESC , createDate DESC , SORT " +
            //                      $" {page_sql} ";
            //List<Lotus> lotus = DapperHelper.QuerySetSql<Lotus>(_sql).ToList();

            return TotalCount;
        }

        #region 工具區
        /// <summary>
        /// 在新增內容時，下面內文若有插入圖片，插入的圖片還沒有Lotus_id, 送出前fNameArr收集了內容裏插入了多少張圖
        /// 然後在送出後在將確定有插入的內容圖片更新LotusImage.lotus_id=Lotus_id
        /// 為何不先給插入圖片id，因為插入圖片隨時可能又del，當del掉後若已給id就無法用null來辨斷刪除
        /// </summary>
        /// <param name="images">httpRequest.Form["fNameArr"]內容圖片的檔名JSON字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="Id">關連內容的lotus_id</param>
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
                            LotusImage lotusImage = new LotusImage();
                            lotusImage.lotus_image_id = Guid.Parse(_imageGuid);        //存放目錄下檔名不含副檔給LotusImage表裏id
                            lotusImage.lotus_id = Id;                                        //關聯管理內容id

                            //更新Lotus內容的lotus_id給LotusImage.lotus_id 連立關連
                            string _sql = @"UPDATE [lotus_image]
                                                           SET [lotus_id] =@lotus_id
                                                         WHERE [lotus_image_id] = @lotus_image_id ";
                            DapperHelper.ExecuteSql(_sql, lotusImage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刪除[Lotus_Id] IS NULL不用的內容圖片
        /// </summary>
        /// <param name="saveFolderPath"></param>
        private void RemoveContentImages(string saveFolderPath)
        {
            //取得存放磁碟目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png

            //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
            string _sql = @"SELECT [IMAGE_NAME] FROM [lotus_image] WHERE lotus_id IS NULL";
            List<string> _imageDataBaseList = DapperHelper.QuerySetSql<string>(_sql).ToList();

            //刪除多餘的內容圖檔, _imageDataBaseList 資料庫檔案名清單，_imageFileList 實際目錄檔路徑檔案清單
            for (int y = 0; y < _imageDataBaseList.Count; y++)
            {
                for (int i = 0; i < _imageFileList.Count; i++)
                {
                    string _removeFileName = Path.GetFileName(_imageFileList[i]);   //除掉路徑留檔案名稱

                    //資料庫Lotus_Id=NULL的檔案名 == 資料匣存放的檔名&&存放檔實際是否存在
                    if (_imageDataBaseList[y] == _removeFileName && File.Exists(_imageFileList[i]))
                    {
                        File.Delete(_imageFileList[i]);
                    }
                }
            }
            //刪除資料庫裡無相關連的內容圖片記錄
            _sql = @"DELETE FROM [lotus_image] WHERE [lotus_id] IS NULL";

            DapperHelper.ExecuteSql(_sql);
        }
        #endregion

    }
}