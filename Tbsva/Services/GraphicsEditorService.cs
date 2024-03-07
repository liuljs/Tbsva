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
    public class GraphicsEditorService : IGraphicsEditorService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;

        public GraphicsEditorService(IDapperHelper iDapperHelper, IImageFileHelper imageFileHelper)
        {
            _IDapperHelper = iDapperHelper;
            m_ImageFileHelper = imageFileHelper;
        }
        #endregion


        #region 新增一筆資料
        public GraphicsEditor InsertGraphicsEditor(HttpRequest httpRequest, GraphicsEditor original_graphicsEditor)
        {
            GraphicsEditor graphicsEditor = RequestData(httpRequest, original_graphicsEditor);

            TableNameDirectoryName(out string _tableName, out string _directoryName);

            //設定上傳檔案的檔名對應相同的類別名稱
            //HttpRequest.Files 屬性 取得用戶端所上傳的檔案集合
            //httpRequest.Files 將文件集合加載到 HttpFileCollection 變量中。
            m_ImageFileHelper.SetImageFileName(graphicsEditor, httpRequest.Files);

            //4.組裝修改刪除不變更時sql
            string[] fields = { "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
            SystemFunctions.picSqlInsert<GraphicsEditor>(fields, graphicsEditor, httpRequest , out string _Sqltop, out string _Sqldown);

            DeleteAllNoDelPicContents();        //清空資料表所有內容(但不刪除圖片檔)

            string _sql = $@"INSERT INTO [{_tableName}]
                                       ( [GraphicsEditorID]
                                        ,[TITLE]
                                        ,[SUBTITLE]
                                        ,[OTHER]
                                        ,[content1]
                                        ,[content2]
                                        ,[content3]
                                        ,[content4]
                                        ,[content5]
                                        ,[content6]
                                        ,[content7]
                                        ,[content8]
                                        {_Sqltop}
                                        ,[creationDate])
                                       VALUES
                                       ( @GraphicsEditorID,
                                         @TITLE,
                                         @SUBTITLE,
                                         @OTHER,
                                         @CONTENT1,
                                         @CONTENT2,
                                         @CONTENT3,
                                         @CONTENT4,
                                         @CONTENT5,
                                         @CONTENT6,
                                         @CONTENT7,
                                         @CONTENT8,
                                          {_Sqldown}
                                         @CREATIONDATE )
                                          SELECT SCOPE_IDENTITY()";  
            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。

            int id = _IDapperHelper.QuerySingle(_sql, graphicsEditor);
            graphicsEditor.id = id;

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(_directoryName);
            string _saveFolderPath = m_ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{id}\");

            //2.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            m_ImageFileHelper.SaveMoreUploadImageFile(httpRequest.Files, _saveFolderPath);

            //3.1將編輯內容圖片裏剛產生的graphics_editor_r1_image表裏的圖檔尚未關連此[graphicsEditorId]更新進去
            //HandleContentImages(httpRequest.Form["cNameArr"], _RootPath_ImageFolderPath, graphicsEditor.graphicsEditorId, tableNameImage);

            //4.刪除未在編輯內容裏的圖片
            //RemoveContentImages(_RootPath_ImageFolderPath, tableNameImage);


            return graphicsEditor;
        }


        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="request">讀取表單資料</param>
        /// <returns>回傳表單類別</returns>
        private GraphicsEditor RequestData(HttpRequest request, GraphicsEditor original_graphicsEditor)
        {
            GraphicsEditor graphicsEditor = new GraphicsEditor();

            graphicsEditor.graphicsEditorId = Guid.NewGuid();                //內容類別另設 ID 唯一索引用
            graphicsEditor.title = request.Form["title"];
            graphicsEditor.subTitle = request.Form["subTitle"];
            graphicsEditor.other = request.Form["other"];
            graphicsEditor.content1 = request.Form["content1"];              //內容(無圖插入)
            graphicsEditor.content2 = request.Form["content2"];              //內容(無圖插入)
            graphicsEditor.content3 = request.Form["content3"];              //內容(無圖插入)
            graphicsEditor.content4 = request.Form["content4"];              //內容(無圖插入)
            graphicsEditor.content5 = request.Form["content5"];              //內容(無圖插入)
            graphicsEditor.content6 = request.Form["content6"];              //內容(無圖插入)
            graphicsEditor.content7 = request.Form["content7"];              //內容(有圖插入)
            graphicsEditor.content8 = request.Form["content8"];              //內容(有圖插入)

            //若有新增過資料時，舊的圖檔資料copy進來，若有上傳SetImageFileName
            //會在把檔名寫入，若沒有則按舊的original_graphicsEditor
            //若是null傳進來不是圖片就不要將舊的資料傳進來
            //舊的資料除了確定是要刪的null不傳進來，不變更的也傳進來
            //SetImageFileName辨斷沒有上傳時並不會更新file.Name, 但舊的因為下面已傳進新的類別所以更新時會在重新更新
            //這裏是因為在新增前先刪除了
            if (original_graphicsEditor !=null)
            {
                if (request.Form["navPics01"] != "null")
                    graphicsEditor.navPics01 = original_graphicsEditor.navPics01;

                if (request.Form["navPics02"] != "null")
                    graphicsEditor.navPics02 = original_graphicsEditor.navPics02;

                if (request.Form["navPics03"] != "null")
                    graphicsEditor.navPics03 = original_graphicsEditor.navPics03;

                if (request.Form["navPics04"] != "null")
                    graphicsEditor.navPics04 = original_graphicsEditor.navPics04;

                if (request.Form["navPics05"] != "null")
                    graphicsEditor.navPics05 = original_graphicsEditor.navPics05;

                if (request.Form["navPics06"] != "null")
                    graphicsEditor.navPics06 = original_graphicsEditor.navPics06;

                if (request.Form["navPics07"] != "null")
                    graphicsEditor.navPics07 = original_graphicsEditor.navPics07;

                if (request.Form["navPics08"] != "null")
                    graphicsEditor.navPics08 = original_graphicsEditor.navPics08;
            }

            graphicsEditor.creationDate = DateTime.Now;

            return graphicsEditor;
        }
        #endregion


        #region 取出資料
        public GraphicsEditor GetGraphicsEditor()
        {
            TableNameDirectoryName(out string _tableName, out string _directoryName);

            GraphicsEditor graphicsEditor = new GraphicsEditor();
            string _sql = $"SELECT TOP 1 * FROM [{_tableName}] ";
            graphicsEditor = _IDapperHelper.QuerySqlFirstOrDefault<GraphicsEditor>(_sql);

            //導覽圖片1~8加上http網址
            if (graphicsEditor != null)  //確定有資料在處理
            {
                string[] fields = { "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
                SystemFunctions.picaddhttp(fields, graphicsEditor, _directoryName, graphicsEditor.id);
            }

            return graphicsEditor;
        }
        #endregion


        #region 取出單純舊資料導覽圖片1~8不加URL
        public GraphicsEditor GetGraphicsEditorNoUrl()
        {
            TableNameDirectoryName(out string _tableName, out string _directoryName);

            GraphicsEditor graphicsEditor = new GraphicsEditor();
            string _sql = $"SELECT TOP 1 * FROM [{_tableName}] ";
            graphicsEditor = _IDapperHelper.QuerySqlFirstOrDefault<GraphicsEditor>(_sql);

            return graphicsEditor;
        }
        #endregion


        #region 更新一筆資料
        public void UpdateGraphicsEditor(HttpRequest httpRequest, GraphicsEditor graphicsEditor)
        {
            graphicsEditor = RequestDataMod(httpRequest, graphicsEditor); //將接收來的參數，和抓到的要修改資料合併

            TableNameDirectoryName(out string _tableName, out string _directoryName);

            //1.設定上傳檔案的檔名對應相同的類別名稱
            //HttpRequest.Files 屬性 取得用戶端所上傳的檔案集合
            //httpRequest.Files 將文件集合加載到 HttpFileCollection 變量中。
            m_ImageFileHelper.SetImageFileName(graphicsEditor, httpRequest.Files);

            //2.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(_directoryName);
            string _saveFolderPath = m_ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{graphicsEditor.id}\");

            //3.處理多張圖檔上傳(request.Files收集所有上傳檔,要存放路徑)
            m_ImageFileHelper.SaveMoreUploadImageFile(httpRequest.Files, _saveFolderPath);

            //4.組裝修改刪除不變更時sql
            //string _sqlpic = picSql(lotus, httpRequest);
            string[] fields = { "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };
            string _sqlpic = SystemFunctions.picSql<GraphicsEditor>(fields, graphicsEditor, httpRequest);


            string _sql = $@" UPDATE [{_tableName}]
                                                   SET
                                                       [title] = @title
                                                      ,[subTitle] = @subTitle
                                                      ,[other] = @other
                                                      ,[content1] = @content1
                                                      ,[content2] = @content2
                                                      ,[content3] = @content3
                                                      ,[content4] = @content4
                                                      ,[content5] = @content5
                                                      ,[content6] = @content6
                                                      ,[content7] = @content7
                                                      ,[content8] = @content8
                                                        {_sqlpic} 
                                                      ,[updateDate] = @updateDate";

            _IDapperHelper.ExecuteSql(_sql, graphicsEditor);
        }

        /// <summary>
        /// 讀取表單資料,轉到graphicsEditor
        /// </summary>
        /// <param name="request">讀取表單資料(修改時用)</param>
        /// <param name="original_graphicsEditor">舊的資料</param>
        /// <returns>graphicsEditor</returns>
        private GraphicsEditor RequestDataMod(HttpRequest request, GraphicsEditor original_graphicsEditor)
        {
            GraphicsEditor graphicsEditor = new GraphicsEditor();      //設定一組空的沒資料會null, 讓圖片預設是null不執行SQL指令，此null是類別裏真的null 
            graphicsEditor.id = original_graphicsEditor.id;  //舊id轉移至新model
            graphicsEditor.graphicsEditorId = original_graphicsEditor.graphicsEditorId;

            graphicsEditor.title = request.Form["title"];                                 //標題
            graphicsEditor.subTitle = request.Form["subTitle"];                   //副標題
            graphicsEditor.other = request.Form["other"];                           //其它
            graphicsEditor.content1 = request.Form["content1"];              //內容(無圖插入)
            graphicsEditor.content2 = request.Form["content2"];              //內容(無圖插入)
            graphicsEditor.content3 = request.Form["content3"];              //內容(無圖插入)
            graphicsEditor.content4 = request.Form["content4"];              //內容(無圖插入)
            graphicsEditor.content5 = request.Form["content5"];              //內容(無圖插入)
            graphicsEditor.content6 = request.Form["content6"];              //內容(無圖插入)
            graphicsEditor.content7 = request.Form["content7"];              //內容(有圖插入)
            graphicsEditor.content8 = request.Form["content8"];              //內容(有圖插入)
            graphicsEditor.updateDate = DateTime.Now;

            return graphicsEditor;

        }
        #endregion



        #region 刪除所有資料
        public void DeleteAllContents()
        {
            TableNameDirectoryName(out string _tableName, out string _directoryName);

            string _sql = $@"TRUNCATE TABLE [{_tableName}]";
            _IDapperHelper.ExecuteSql(_sql);

            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(_directoryName);
            DeleteAFile(_RootPath_ImageFolderPath); 
        }
        #endregion


        #region 刪除所有資料不刪檔
        public void DeleteAllNoDelPicContents()
        {
            TableNameDirectoryName(out string _tableName, out string _directoryName);

            string _sql = $@"TRUNCATE TABLE [{_tableName}]";
            _IDapperHelper.ExecuteSql(_sql);

            ////1.取圖片路徑C:\xxx
            //string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(_directoryName);
            //DeleteAFile(_RootPath_ImageFolderPath);
        }
        #endregion

        /// <summary>
        /// 新增一個[GraphicsEditorImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 因為正在新增內容還未送出，所以內容的[graphicsEditorId].[graphicsEditorId]關連[graphics_editor_r1].[graphicsEditorId]還沒產生
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="directoryName">目錄名稱</param>
        /// <param name="tableNameImage">編輯器內插入圖片的表格名</param>
        /// <returns>GraphicsEditorImage</returns>
        //public GraphicsEditorImage AddUploadImage(HttpRequest httpRequest, string directoryName, string tableNameImage)
        //{
        //    GraphicsEditorImage graphicsEditorImage = new GraphicsEditorImage();
        //    graphicsEditorImage.graphics_editor_image_id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
        //    graphicsEditorImage.image_name = graphicsEditorImage.graphics_editor_image_id + ".png";

        //    //處理圖片
        //    //1.取圖片路徑C:\xxx
        //    string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(directoryName);

        //    //2.存放上傳圖片(1實體檔,2檔名,3路徑)
        //    m_ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], graphicsEditorImage.image_name, _RootPath_ImageFolderPath);

        //    //3.加入資料庫
        //    string _sql = $@"INSERT INTO [{tableNameImage}]
        //                                    ([graphics_editor_image_id]
        //                                    ,[image_name])
        //                                VALUES
        //                                    (@graphics_editor_image_id,
        //                                    @image_name )
        //                                        SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
        //    int id = _IDapperHelper.QuerySingle(_sql, graphicsEditorImage);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
        //    graphicsEditorImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

        //    //4.將圖片檔加上Url
        //    graphicsEditorImage.image_name = Tools.GetInstance().GetImageLink(directoryName, graphicsEditorImage.image_name);

        //    return graphicsEditorImage;
        //}

        /// <summary>
        /// 更新內容圖片
        /// 新增一個[GraphicsEditorImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="Id"></param>
        /// <param name="directoryName">目錄名稱</param>
        /// <param name="tableNameImage">編輯器內插入圖片的表格名</param>
        /// <returns></returns>
        //public GraphicsEditorImage AddUploadImage(HttpRequest httpRequest, Guid Id, string directoryName, string tableNameImage)
        //{
        //    GraphicsEditorImage graphicsEditorImage = new GraphicsEditorImage();
        //    graphicsEditorImage.graphics_editor_image_id = Guid.NewGuid();                                     //主索引鍵（P.K.）自動產生一個Guid
        //    graphicsEditorImage.image_name = graphicsEditorImage.graphics_editor_image_id + ".png";   //用lotus_image_id來命名圖檔
        //    graphicsEditorImage.graphicsEditorId = Id;                                                            //關聯內容graphicsEditorId

        //    //處理圖片
        //    //1.取圖片路徑C:\xxx
        //    string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(directoryName);
        //    //string _saveFolderPath = m_ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{id}\");

        //    //2.存放上傳圖片(1實體檔,2檔名,3路徑)
        //    m_ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], graphicsEditorImage.image_name, _RootPath_ImageFolderPath);

        //    //3.加入資料庫
        //    string _sql = $@"INSERT INTO [{tableNameImage}]
        //                                    ([graphics_editor_image_id]
        //                                    ,[image_name]
        //                                    ,[graphicsEditorId])
        //                                VALUES
        //                                    (@graphics_editor_image_id,
        //                                    @image_name,
        //                                    @graphicsEditorId)
        //                                        SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
        //    int id = _IDapperHelper.QuerySingle(_sql, graphicsEditorImage);         //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
        //    graphicsEditorImage.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

        //    //4.將圖片檔加上Url
        //    graphicsEditorImage.image_name = Tools.GetInstance().GetImageLink(directoryName, graphicsEditorImage.image_name);


        //    return graphicsEditorImage;
        //}


        #region 編輯時插入圖片小圖
        public string AddImage(HttpRequest request, string _directoryName)
        {

            //設定圖片檔名
            string _strFileName = Guid.NewGuid() + ".png";
            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(_directoryName);

            //2上傳圖片(1實體圖片檔, 2檔名, 3放圖路徑)
            m_ImageFileHelper.SaveUploadImageFile(request.Files[0], _strFileName, _RootPath_ImageFolderPath);

            //3回傳http圖片網址GetImageLink(1.指定目錄,2檔名)
            string _imageUrl = Tools.GetInstance().GetImageLink(_directoryName, _strFileName);

            return _imageUrl;
        }
        #endregion


        #region 工具區

        /// <summary>
        /// _request.Path取得的路徑對應相的資料表格與目錄
        /// </summary>
        /// <param name="_tableName"></param>
        /// <param name="_directoryName"></param>
        public void TableNameDirectoryName(out string _tableName, out string _directoryName)
        {
            HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
            string Route = _request.Path.Replace("/api/", "").Replace("/", "");   //取出Route的網址並取代/ 線上是這樣/api/graphicsEditorR2/, 後面的取代/是最後若有打/也要取代
            //string Route = _request.Path.Replace("/", ""); //測試用這個
            
            _tableName = string.Empty;  //主資料表
            _directoryName = string.Empty; //目錄名
            //http://localhost:59415/graphicsEditorR11 Route這裏基本上若亂打或空根本進不來，所以不會是空值或名稱不對
            // "Message": "找不到與要求 URI 'http://localhost:59415/graphicsEditorR11/' 相符的 HTTP 資源。",
            //"MessageDetail": "找不到與名稱為 'graphicsEditorR11' 的控制器相符的類型。"

            switch (Route)
            {
                case "graphicsEditorR1":
                    _tableName = "graphics_editor_r1";    //主資料表
                    _directoryName = "graphicsEditorR1";    //目錄名
                    break;
                case "graphicsEditorR2":
                    _tableName = "graphics_editor_r2";    //主資料表
                    _directoryName = "graphicsEditorR2";    //目錄名
                    break;
                case "graphicsEditorR3":
                    _tableName = "graphics_editor_r3";    //主資料表
                    _directoryName = "graphicsEditorR3";    //目錄名
                    break;
                case "graphicsEditorR4":
                    _tableName = "graphics_editor_r4";    //主資料表
                    _directoryName = "graphicsEditorR4";    //目錄名
                    break;
                case "graphicsEditorR5":
                    _tableName = "graphics_editor_r5";    //主資料表
                    _directoryName = "graphicsEditorR5";    //目錄名
                    break;
                case "graphicsEditorR6":
                    _tableName = "graphics_editor_r6";    //主資料表
                    _directoryName = "graphicsEditorR6";    //目錄名
                    break;
                case "graphicsEditorR7":
                    _tableName = "graphics_editor_r7";    //主資料表
                    _directoryName = "graphicsEditorR7";    //目錄名
                    break;
                case "graphicsEditorR8":
                    _tableName = "graphics_editor_r8";    //主資料表
                    _directoryName = "graphicsEditorR8";    //目錄名
                    break;
                case "graphicsEditorR9":
                    _tableName = "graphics_editor_r9";    //主資料表
                    _directoryName = "graphicsEditorR9";    //目錄名
                    break;
                case "graphicsEditorR10":
                    _tableName = "graphics_editor_r10";    //主資料表
                    _directoryName = "graphicsEditorR10";    //目錄名
                    break;
                case "graphicsEditorR11":
                    _tableName = "graphics_editor_r11";    //主資料表
                    _directoryName = "graphicsEditorR11";    //目錄名
                    break;
            }
        }


        /// <summary>
        /// 刪除一個資料夾下的所有檔案以及子目錄檔案
        /// </summary>
        /// <param name="path">要刪除的目錄下的路徑</param>
        private void DeleteAFile(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);  //路徑取得給DirectoryInfo

            foreach (FileInfo file in di.GetFiles())  //取得路徑下所有檔案
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())  //取得路徑下所有目錄
            {
                dir.Delete(true);
            }
        }

        /// <summary>
        /// 在新增內容時，下面內文若有插入圖片，插入的圖片還沒有Lotus_id, 送出前fNameArr收集了內容裏插入了多少張圖
        /// 然後在送出後在將確定有插入的內容圖片更新LotusImage.lotus_id=Lotus_id
        /// 為何不先給插入圖片id，因為插入圖片隨時可能又del，當del掉後若已給id就無法用null來辨斷刪除
        /// </summary>
        /// <param name="images">httpRequest.Form["fNameArr"]內容圖片的檔名JSON字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="Id">關連內容的graphicsEditorId</param>
        /// <param name="tableNameImage">編輯器內插入圖片的表格名</param>
        //private void HandleContentImages(string images, string saveFolderPath, Guid Id, string tableNameImage)
        //{
        //    List<string> _retFileNameList = JsonConvert.DeserializeObject<List<string>>(images);

        //    //取得存放saveFolderPath目錄下所有.png圖檔
        //    //GetFiles (string path, string searchPattern);
        //    List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png
        //    if (_retFileNameList != null && _retFileNameList.Count > 0)    //fNameArr確認有檔才處理
        //    {
        //        for (int i = 0; i < _retFileNameList.Count; i++) //處理裏面的所有檔fNameArr ["abc.png","def.png"]
        //        {
        //            for (int y = 0; y < _imageFileList.Count; y++)  //fNameArr和目錄下實際所有.png圖檔比對一下
        //            {
        //                string _urlFileName = _retFileNameList[i].ToLower().Trim();   //處理fNameArr內的檔轉小寫去空白
        //                string _entityFileName = Path.GetFileName(_imageFileList[y]);  //目錄下的檔, 只需留檔案名稱與副檔名
        //                string _imageGuid = Path.GetFileNameWithoutExtension(_imageFileList[y]);  //只留下檔案名稱，路徑與副檔名都去除
        //                //判斷fNameArr[]傳進來的參數圖檔==目前目錄下的檔&&判斷目錄下的檔案是否存在。
        //                if (_urlFileName == _entityFileName && File.Exists(_imageFileList[y]))
        //                {
        //                    GraphicsEditorImage graphicsEditorImage = new GraphicsEditorImage();
        //                    graphicsEditorImage.graphics_editor_image_id = Guid.Parse(_imageGuid);        //存放目錄下檔名不含副檔給LotusImage表裏id
        //                    graphicsEditorImage.graphicsEditorId = Id;                                                           //關聯管理內容id

        //                    //更新graphics_editor_r1內容的graphicsEditorId給graphics_editor_r1_image.graphicsEditorId 連立關連
        //                    string _sql = $@"UPDATE [{tableNameImage}]
        //                                                   SET [graphicsEditorId] =@graphicsEditorId
        //                                                 WHERE [graphics_editor_image_id] = @graphics_editor_image_id ";
        //                    _IDapperHelper.ExecuteSql(_sql, graphicsEditorImage);
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 刪除[graphicsEditorId] IS NULL不用的內容圖片
        /// </summary>
        /// <param name="saveFolderPath"></param>
        /// <param name="tableNameImage">編輯器內插入圖片的表格名</param>
        //private void RemoveContentImages(string saveFolderPath, string tableNameImage)
        //{
        //    //取得存放磁碟目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
        //    List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();  //搜尋存放圖檔目錄副檔.png

        //    //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
        //    string _sql = $@"SELECT [IMAGE_NAME] FROM [{tableNameImage}] WHERE [graphicsEditorId] IS NULL";
        //    List<string> _imageDataBaseList = _IDapperHelper.QuerySetSql<string>(_sql).ToList();

        //    //刪除多餘的內容圖檔, _imageDataBaseList 資料庫檔案名清單，_imageFileList 實際目錄檔路徑檔案清單
        //    for (int y = 0; y < _imageDataBaseList.Count; y++)
        //    {
        //        for (int i = 0; i < _imageFileList.Count; i++)
        //        {
        //            string _removeFileName = Path.GetFileName(_imageFileList[i]);   //除掉路徑留檔案名稱

        //            //資料庫Lotus_Id=NULL的檔案名 == 資料匣存放的檔名&&存放檔實際是否存在
        //            if (_imageDataBaseList[y] == _removeFileName && File.Exists(_imageFileList[i]))
        //            {
        //                File.Delete(_imageFileList[i]);
        //            }
        //        }
        //    }
        //    //刪除資料庫裡無相關連的內容圖片記錄
        //    _sql = $@"DELETE FROM [{tableNameImage}] WHERE [graphicsEditorId] IS NULL";

        //    _IDapperHelper.ExecuteSql(_sql);
        //}

        #endregion


    }

}