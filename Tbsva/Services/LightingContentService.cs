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
    public class LightingContentService : ILightingContentService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dapperHelper"></param>
        /// <param name="imageFileHelper"></param>
        public LightingContentService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
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
        public Lighting_content Insert_Lighting_content(HttpRequest _request)
        {
            Lighting_content _lighting_Content = Request_data(_request);  //將接收來的參數轉進Lighting_content型別
            string _sql = @"INSERT INTO [Lighting_content]
                                ([id]
                                ,[Lighting_category_id]
                                ,[image_name]
                                ,[title]
                                ,[brief]
                                ,[content]
                                ,[more_pic_url]
                                ,[creation_date]
                                ,[Enabled]
                                ,[Sort]
                                ,[first])
                            VALUES
                                ( @id,
                                  @Lighting_category_id,
                                  @image_name,
                                  @title,
                                  @brief,
                                  @content,
                                  @more_pic_url,
                                  @creation_date,
                                  @Enabled,
                                  @Sort,
                                  @first ) ";
            _IDapperHelper.ExecuteSql(_sql, _lighting_Content);  //新增資料

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lighting);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _lighting_Content.image_name, _RootPath_ImageFolderPath);

            //3.0內容若有新增插入一個圖片

            //3.1將編輯內容圖片裏剛產生的Lighting_image表裏的圖檔尚未關連此文章_lighting_Content.id更新進去
            HandleContentImages(_request.Form["fNameArr"], _RootPath_ImageFolderPath, _lighting_Content.id );

            //4.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);

            return _lighting_Content;
        }
        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="_request">讀取表單資料</param>
        /// <returns>回傳表單類別_lighting_Content</returns>
        private Lighting_content Request_data(HttpRequest _request)
        {
            Lighting_content _lighting_Content = new Lighting_content();
            _lighting_Content.id = Guid.NewGuid();
            _lighting_Content.Lighting_category_id = Convert.ToInt32(_request.Form["Lighting_category_id"]);  //關連目錄id
            _lighting_Content.image_name = _lighting_Content.id + ".png";                                                                //圖片檔名使用id來當
            _lighting_Content.title = _request.Form["title"];                                                                                            //標題
            _lighting_Content.brief = _request.Form["brief"];                                                                                         //簡述
            _lighting_Content.content = _request.Form["content"];                                                                              //內容
            _lighting_Content.more_pic_url = _request.Form["more_pic_url"];                                                          //更多照片網址
            _lighting_Content.creation_date = DateTime.Now;                                                                                       //新增時間
            _lighting_Content.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["Enabled"]));           //是否啟用(0/1)

            //var aaa = _request.Form["Sort"];  IsNullOrWhiteSpace驗證 沒有欄位null, 空值string.Empty,"",空白" ";IsNullOrEmpty少了空白的驗證" "
            //Sort若表單未傳任何值時 _article_content.Sort=0，而_request.Form["Sort"], null是沒此欄，或有此欄但沒有資料，或是打一個空格
            //
            if (string.IsNullOrWhiteSpace(_request.Form["Sort"]))         //空值,或空格，或沒設定此欄位null
            {
                _lighting_Content.Sort = Convert.ToInt32(Tools.GetInstance().DefaultValueSort);                  //沒有設定欄位給一個預設值
            }
            else
            {
                _lighting_Content.Sort = Convert.ToInt32(_request.Form["Sort"]);                                           //排序 
            }
            //是否置頂
            if ("Y" == _request.Form["first"])
            { _lighting_Content.first = "Y"; }
            else
            { _lighting_Content.first = "N"; }

            string _sql = @"SELECT name FROM [Lighting_category] WHERE [id]=@id";
            Lighting_category _lighting_Category = new Lighting_category();
            _lighting_Category.id = _lighting_Content.Lighting_category_id;
            _lighting_Category = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _lighting_Category);
            _lighting_Content.Lighting_Category_Name = _lighting_Category.name;

            return _lighting_Content;
        }
        /// <summary>
        /// 新增一個[Lighting_image內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 因為正在新增內容還未送出，所以內容的Lighting_content_id關連id還沒產生
        /// </summary>
        /// <param name="_request">點選內文裏的圖片icon</param>
        /// <returns>_imageUrl</returns>
        public string AddUploadImage(HttpRequest _request)
        {
            Lighting_image _lighting_Image = new Lighting_image();
            _lighting_Image.id = Guid.NewGuid();                                           //自動產生一個Guid
            _lighting_Image.image_name = _lighting_Image.id + ".png";   //用id來命名圖檔

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lighting);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _lighting_Image.image_name, _RootPath_ImageFolderPath);

            //3.取得要回傳的網址
            //string _imageUrl = m_ImageFileHelper.GetImageLink(ConfigurationManager.AppSettings["Lighting"], _article_image.image_name);
            string _imageUrl = Tools.GetInstance().GetImageLink(Tools.GetInstance().Lighting, _lighting_Image.image_name);
            // string _imageUrl = m_ImageFileHelper.GetImageLink(Tools.GetInstance().Lighting, _lighting_Image.image_name);

            //4.加入資料庫
            string _sql = @"INSERT INTO [Lighting_image]
                                            ([id]
                                            ,[image_name])
                                        VALUES
                                            (@id,
                                            @image_name )";
            _IDapperHelper.ExecuteSql(_sql, _lighting_Image);

            return _imageUrl;
        }
        #endregion

        #region 取得一筆資料
        /// <summary>
        /// 取得一筆資料
        /// </summary>
        /// <param name="id">輸入內容id編號</param>
        /// <returns>_lighting_Content</returns>
        public Lighting_content Get_Lighting_content(Guid id)
        {
            Lighting_content _lighting_Content = new Lighting_content();
            _lighting_Content.id = id;
            string adminQuery = Auth.Role.IsAdmin ? "" : " AND L2_CO.Enabled = 1 ";

            //string _sql = $"SELECT * FROM [Lighting_content] WHERE [id]=@id {adminQuery} ";
            //增加目錄名稱Lighting_Category_Name
            string _sql = $"SELECT L2_CO.*,L1_CA.name AS Lighting_Category_Name FROM [Lighting_content] L2_CO " +
            $"LEFT JOIN [Lighting_category] L1_CA ON L1_CA.id = L2_CO.Lighting_category_id " +
            $"WHERE L2_CO.id=@id {adminQuery} ";

            _lighting_Content = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _lighting_Content);

            //_lighting_Content.image_name加上http網址
            if (_lighting_Content != null)
            {
                _lighting_Content.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Lighting, _lighting_Content.image_name);
            }

            return _lighting_Content;
        }
        #endregion

        #region 更新一筆資料
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_request">用戶端的要求資訊</param>
        /// <param name="_Lighting_content">更新的資料類別</param>
        public void Update_Lighting_content(HttpRequest _request, Lighting_content _Lighting_content)
        {
            _Lighting_content = Request_data_mod(_request, _Lighting_content);   //將接收來的參數，和抓到的要修改資料合併

            if (_request.Files.Count > 0)  //辨斷有沒有上傳檔案
            {
                //以下處理圖片
                //1. 取得要放置的目錄路徑
                //string _RootPath_ImageFolderPath = Get_RootPath_ImageFolderPath(); 舊的
                string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lighting);

                //Get_Lighting_content裏面的image_name有加網址m_ImageFileHelper.GetImageLink，改只取檔案名與副檔
                string _image_name = Path.GetFileName(_Lighting_content.image_name);

                //2.存放上傳文章封面圖片(1實體檔,2檔名,3路徑)
                m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _image_name, _RootPath_ImageFolderPath);

                //3.將編輯內容圖片裏剛產生的Lighting_image表裏的圖檔尚未關連此內容_Lighting_content.id更新進去
                //這裏有點怪AddUploadImage(HttpRequest request, Guid Lighting_content_Id)在加入內容圖片時Lighting_image已經有就Lighting_content.id
                //HandleContentImages(_request["fNameArr"], _RootPath_ImageFolderPath, _Lighting_content.id);

                //4.刪除未在編輯內容裏的圖片IS NULL
                //RemoveContentImages(_RootPath_ImageFolderPath);
            }
            string _sql = @"UPDATE [Lighting_content]
                                        SET 
                                            [Lighting_category_id] = @Lighting_category_id
                                            ,[title] = @title
                                            ,[brief] = @brief
                                            ,[content] = @content
                                            ,[more_pic_url] = @more_pic_url
                                            ,[updated_date] = @updated_date
                                            ,[Enabled] = @Enabled
                                            ,[Sort] = @Sort
                                            ,[first] = @first
                                        WHERE [id] = @id";
            _IDapperHelper.ExecuteSql(_sql, _Lighting_content);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_request">讀取表單資料(修改時用)</param>
        /// <param name="_lighting_Content">控制器取到資料</param>
        /// <returns>_lighting_Content</returns>
        private Lighting_content Request_data_mod(HttpRequest _request, Lighting_content _lighting_Content)
        {
            _lighting_Content.Lighting_category_id = Convert.ToInt32(_request.Form["Lighting_category_id"]);
            //_lighting_Content.image_name = _lighting_Content.id + ".png";  //圖片在新增時已命名
            _lighting_Content.title = _request.Form["title"];
            _lighting_Content.brief = _request.Form["brief"];
            _lighting_Content.content = _request.Form["content"];
            _lighting_Content.more_pic_url = _request.Form["more_pic_url"];
            _lighting_Content.updated_date = DateTime.Now;
            _lighting_Content.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["Enabled"]));

            if (string.IsNullOrWhiteSpace(_request.Form["Sort"]))         //空值,或空格，或沒設定此欄位null
            {
                _lighting_Content.Sort = Convert.ToInt32(Tools.GetInstance().DefaultValueSort);                  //沒有設定欄位給一個預設值
            }
            else
            {
                _lighting_Content.Sort = Convert.ToInt32(_request.Form["Sort"]);                                           //排序 
            }

            //是否置頂
            if ("Y" == _request.Form["first"])
            { _lighting_Content.first = "Y"; }
            else
            { _lighting_Content.first = "N"; }

            return _lighting_Content;
        }


        /// <summary>
        /// 新增一個Lighting_image內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 修改時用
        /// </summary>
        /// <param name="_request">點選內文裏的圖片新增</param>
        /// <param name="_Lighting_content_Id">關連目錄的id</param>
        /// <returns></returns>
        public string AddUploadImage(HttpRequest _request, Guid _Lighting_content_Id)
        {
            Lighting_image _lighting_Image = new Lighting_image();
            _lighting_Image.id = Guid.NewGuid();                                               //自動產生一個Guid
            _lighting_Image.image_name = _lighting_Image.id + ".png";       //用id來命名圖檔
            _lighting_Image.Lighting_content_id = _Lighting_content_Id;       //關聯目錄id

            //1. 取得要放置的目錄路徑
            //string _RootPath_ImageFolderPath = Get_RootPath_ImageFolderPath(); 舊的
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lighting);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _lighting_Image.image_name, _RootPath_ImageFolderPath);

            //3.取得要回傳的網址
            // string _imageUrl = m_ImageFileHelper.GetImageLink(ConfigurationManager.AppSettings["Lighting"], _article_image.image_name);
            // string _imageUrl = m_ImageFileHelper.GetImageLink(Tools.GetInstance().Lighting, _lighting_Image.image_name);
            string _imageUrl = Tools.GetInstance().GetImageLink(Tools.GetInstance().Lighting, _lighting_Image.image_name);

            //4.加入資料庫, 這裏會給@Lighting_content_id值，這樣Lighting_image的Lighting_content_id就會有值
            string _sql = @"INSERT INTO [Lighting_image]
                                            ([id]
                                            ,[image_name]
                                            ,[Lighting_content_id])
                                        VALUES
                                            (@id,
                                             @image_name,
                                             @Lighting_content_id )";
            _IDapperHelper.ExecuteSql(_sql, _lighting_Image);

            return _imageUrl;
        }
        #endregion

        /// <summary>
        /// 刪除內容與內容圖片
        /// </summary>
        /// <param name="_Lighting_content">Lighting_content _lighting_Content = _ILightingContentService.Get_Lighting_content(id); 先取出內容資料</param>
        public void Delete_Lighting_content(Lighting_content _Lighting_content)
        {
            //1. 取得放置圖片的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lighting);

            //取得這筆內容下相關內容圖片檔名清單
            string _sql = @"SELECT [image_name] FROM [Lighting_image] where Lighting_content_id = @id";
            List<string> _Lighting_image_name_List = _IDapperHelper.QuerySetSql<Lighting_content, string>(_sql, _Lighting_content).ToList();
 
            string _Lighting_image_name_List_Path = string.Empty;   //宣告一個變數，準備要路徑+檔名

            //刪除內容下內容圖
            //刪除Lighting_content_id = @id  =>  Lighting_image.[image_name] 
            for (int i = 0; i < _Lighting_image_name_List.Count; i++)
            {
                _Lighting_image_name_List_Path = Path.Combine(_RootPath_ImageFolderPath, _Lighting_image_name_List[i]);
                if (File.Exists(_Lighting_image_name_List_Path))
                {
                    File.Delete(_Lighting_image_name_List_Path);
                }
            }

            //刪除內容圖片
            string _image_name = Path.GetFileName(_Lighting_content.image_name);  //取出內容圖片
            string _image_name_FilePath = Path.Combine(_RootPath_ImageFolderPath, _image_name); //加上檔案路徑
            //刪除封面圖片
            if (File.Exists(_image_name_FilePath))
            {
                File.Delete(_image_name_FilePath);
            }

            //刪除內容資料
            _sql = @"DELETE FROM [Lighting_content] WHERE id = @id ";
            //資料庫有設定重疊顯示，所以刪主鍵關連資料會一同刪除
            _IDapperHelper.ExecuteSql(_sql, _Lighting_content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Lighting_category_id">所在目錄下</param>
        /// <param name="_count">一頁要顯示幾筆</param>
        /// <param name="_page">第幾頁開始</param>
        /// <param name="_keyword">搜尋關鍵字(標題,敘述內容)</param>
        /// <returns></returns>
        public List<Lighting_content> Get_Lighting_content_ALL(int? _Lighting_category_id, int? _count, int? _page, string _keyword)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string Lighting_category_id_sql = string.Empty;   //所屬的目錄id sql
            string search_sql = string.Empty;

            if (_count != null && _count > 0 && _page != null && _page > 0)
            {
                int startRowjumpover = 0;   //跳過筆數
                startRowjumpover = Convert.ToInt32((_page - 1) * _count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {_count}  ROWS ONLY ";
            }

            string adminQuery = Auth.Role.IsAdmin ? " where 1 = 1 " : " where L2_CO.Enabled = 1 ";    //登入取得所有資料:未登入只能取得上線資料
            Lighting_category_id_sql = _Lighting_category_id > 0 ? $" and Lighting_category_id = {_Lighting_category_id}" : "";
            if (!string.IsNullOrWhiteSpace(_keyword))
            {
                search_sql = $" and ( L2_CO.title like '%{_keyword}%' or L2_CO.brief like '%{_keyword}%' or L2_CO.content like '%{_keyword}%' ) ";
            }

            string _sql = $"SELECT L2_CO.*,L1_CA.name AS Lighting_Category_Name FROM [Lighting_content] L2_CO " +
                                    $"LEFT JOIN [Lighting_category] L1_CA ON L1_CA.id = L2_CO.Lighting_category_id " +
                                    $"{ adminQuery } " +
                                    $"{ Lighting_category_id_sql } " +
                                    $"{ search_sql }" +
                                    $"Order by L2_CO.[FIRST] DESC , L2_CO.Sort , L2_CO.creation_date DESC   " +
                                    $"{ page_sql }";

            //string _sql = $"SELECT * FROM [Lighting_content] { adminQuery } " +
            //                        $"{ Lighting_category_id_sql } " +
            //                        $"{ search_sql }" +
            //                        $"Order by [FIRST] DESC , Sort , creation_date DESC " +
            //                        $"{ page_sql }";

            List<Lighting_content> _lighting_Contents = _IDapperHelper.QuerySetSql<Lighting_content>(_sql).ToList();

            //將資料庫image_name加上URL
            for (int i = 0; i < _lighting_Contents.Count; i++)
            {
                _lighting_Contents[i].image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Lighting, _lighting_Contents[i].image_name);
            }

            return _lighting_Contents;
        }



        #region 工具區
        //private string Get_RootPath_ImageFolderPath()
        //{
        //    string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Lighting); //圖片存放硬碟路徑
        //    return _RootPath_ImageFolderPath;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="images">_request.Form["fNameArr"]內容圖片的檔名Json字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="id">關連內容的ID</param>
        /// DeserializeObject(): 將JSON格式轉換成物件。
        /// 反序列化成物件
        /// JsonConvert.DeserializeObject<類別>("字串"),<>中定義類別,將JSON字串反序列化成物件。
        private void HandleContentImages(string images, string saveFolderPath, Guid id)
        {
            //取得回傳內容圖片的檔名fNameArr["abc.png","def.png"]， Json反序列化(Deserialize)為物件(Object)
            List<string> _retFileNameList = JsonConvert.DeserializeObject<List<string>>(images);

            //取得存放saveFolderPath目錄下所有.png圖檔
            //GetFiles (string path, string searchPattern);
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();
             if (_retFileNameList != null && _retFileNameList.Count > 0)   //fNameArr確認有檔才處理
            {
                for (int i = 0; i < _retFileNameList.Count; i++)  //處理裏面的所有檔fNameArr ["abc.png","def.png"]
                {
                    for (int y = 0; y < _imageFileList.Count; y++)  //fNameArr和目錄下實際所有.png圖檔比對一下
                    {
                        string _urlFileName = _retFileNameList[i].ToLower().Trim(); //fNameArr內的檔轉小寫去空白
                        string _entityFileName = Path.GetFileName(_imageFileList[y]);  //目錄下的檔只需留檔案名稱
                        string _imageGuid = Path.GetFileNameWithoutExtension(_imageFileList[y]);  //傳回[沒有副檔名]的指定路徑字串的[檔案名稱]。
                     
                        //判斷fNameArr[]傳進來的參數圖檔==目前目錄下的檔&&判斷目錄下的檔案是否存在。
                        if (_urlFileName == _entityFileName && File.Exists(_imageFileList[y]))
                        {  
                            Lighting_image _lighting_Image = new Lighting_image();
                            _lighting_Image.id = Guid.Parse(_imageGuid);                //存放目錄下檔名不含副檔給Lighting_image表裏id
                            _lighting_Image.Lighting_content_id = id;                       //關連內容的ID

                            //更新內容的id給Lighting_image，
                            string _sql = @"UPDATE [Lighting_image]
                                                            SET [Lighting_content_id] = @Lighting_content_id
                                                             WHERE id = @id";
                            _IDapperHelper.ExecuteSql(_sql, _lighting_Image);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 刪除[Lighting_content_id] IS NULL不用的內容圖片
        /// </summary>
        /// <param name="saveFolderPath"></param>
        private void RemoveContentImages(string saveFolderPath)
        {
            //取得存放磁碟目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();

            //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
            string _sql = @"SELECT [image_name] FROM [Lighting_image] WHERE Lighting_content_id IS NULL";
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
            _sql = @"DELETE FROM [Lighting_image] WHERE [Lighting_content_id] IS NULL";

            _IDapperHelper.ExecuteSql(_sql);
        }
        #endregion

    }
}