using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;


namespace WebShopping.Services
{
    public class ArticleContentService : IArticleContentService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;

        public ArticleContentService(IDapperHelper IDapperHelper, IImageFileHelper imageFileHelper)
        {
            _IDapperHelper = IDapperHelper;
            m_ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增文章實作
        /// <summary>
        /// 新增一筆文章
        /// </summary>
        /// <param name="_request">用戶端送來的表單資訊</param>
        /// <returns></returns>
        public article_content Insert_article_content(HttpRequest _request)
        {
            article_content _article_content = Request_data(_request);  //將接收來的參數轉進article_content型別

            string _sql = @"INSERT INTO [article_content]
                                ([id]
                                ,[article_category_id]
                                ,[title]
                                ,[subtitle]
                                ,[image_name]
                                ,[brief]
                                ,[content]
                                ,[Enabled]
                                ,[Sort]
                                ,[first])
                            VALUES
                                ( @id,
                                  @article_category_id,
                                  @title, 
                                  @subtitle, 
                                  @image_name,
                                  @brief,
                                  @content,
                                  @Enabled,
                                  @Sort,
                                  @first ) ";
            //將收集完成的類別資料_article_content用來新增一筆資料並使用交易鎖定方式
            _IDapperHelper.ExecuteSql(_sql, _article_content);  //到此一筆資料已新增進去

            //以下為圖片處理
            //1.取得文章要放置的目錄路徑
            string _RootPath_ImageFolderPath = Get_RootPath_ImageFolderPath();

            //2.存放上傳文章封面圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _article_content.image_name, _RootPath_ImageFolderPath);

            //3.將編輯內容圖片裏剛產生的article_image表裏的圖檔尚未關連此文章_article_content.id更新進去
            HandleContentImages(_request.Form["cNameArr"], _RootPath_ImageFolderPath, _article_content.id);

            //4.刪除未在編輯內容裏的圖片
            RemoveContentImages(_RootPath_ImageFolderPath);

            return _article_content;
        }

        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="_request">讀取表單資料</param>
        /// <returns>回傳表單類別_article_content</returns>
        private article_content Request_data(HttpRequest _request)
        {
            article_content _article_content = new article_content();
            _article_content.id = Guid.NewGuid();
            _article_content.article_category_id = Convert.ToInt32(_request.Form["articleCategoryId"]);       //目錄id
            _article_content.title = _request.Form["title"];                                                                                         //文章標題
            _article_content.image_name = _article_content.id + ".png";                                                               //圖片檔名使用id來當
            _article_content.subtitle = _request.Form["subtitle"];                                                                            //副標題
            _article_content.brief = _request.Form["brief"];                                                                                      //文章簡述
            _article_content.content = _request.Form["content"];                                                                            //文章內容
            _article_content.creation_date = DateTime.Now;                                                                                    //新增時間
            _article_content.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));         //是否啟用(0/1)
            //var aaa = _request.Form["Sort"];  IsNullOrWhiteSpace驗證 沒有欄位null, 空值string.Empty,"",空白" ";IsNullOrEmpty少了空白的驗證" "
            //Sort若表單未傳任何值時 _article_content.Sort=0，而_request.Form["Sort"], null是沒此欄，或有此欄但沒有資料，或是打一個空格
            //
            if (string.IsNullOrWhiteSpace(_request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                _article_content.Sort = Convert.ToInt32(Tools.GetInstance().DefaultValueSort);                  //沒有設定欄位給一個預設值
            }
            else 
            {
                _article_content.Sort = Convert.ToInt32(_request.Form["sort"]);                                           //排序 
            }
            //是否置頂
            ////if ("Y" == _request.Form["first"])
            ////{ _article_content.first = "Y"; } 
            ////else 
            ////{ _article_content.first = "N"; }
            _article_content.first = Convert.ToBoolean(Convert.ToByte(_request.Form["first"]));                 //是否置頂(0 關閉;1 開啟)

            string _sql = @"SELECT name FROM [article_category] WHERE [id]=@id";
            article_category _article_category = new article_category();
            _article_category.id = _article_content.article_category_id;
            _article_category = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _article_category);
            _article_content.Article_Category_Name = _article_category.name;

            return _article_content;
        }
        #endregion

        #region 取得一筆文章資料
        /// <summary>
        /// 取得一筆文章資料
        /// </summary>
        /// <param name="id">輸入文章文章id編號</param>
        /// <returns>_article_content</returns>
        public article_content Get_article_content(Guid id)
        {
            article_content _article_content = new article_content();         //設定_article_content空的容器
            _article_content.id = id;                                                                //將輸入的文章id傳給_article_content容器id

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND A2_CO.Enabled = 1 ";    //登入取得所有資料:未登入只能取得上線資料

            //string _sql = $"SELECT * FROM [article_content] WHERE [id]=@id {adminQuery} ";
            //增加目錄名稱Article_Category_Name
            string _sql = $"SELECT A2_CO.*,A1_CA.name AS Article_Category_Name FROM [article_content] A2_CO " +
            $"LEFT JOIN [article_category] A1_CA ON A1_CA.id = A2_CO.article_category_id " +
            $"WHERE A2_CO.id=@id {adminQuery} ";

            _article_content = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _article_content);

            //加上http網址
            if (_article_content != null)
            {
                _article_content.image_name = m_ImageFileHelper.GetImageLink(ConfigurationManager.AppSettings["Article"], _article_content.image_name);
                //文章 帶分類的名稱, 使用類別
                //article_category _article_category = new article_category();
                //_article_category.id = _article_content.article_category_id;
                //_sql = "SELECT * FROM [article_category] WHERE [id]=@id";
                //_article_content.Article_Category_Collection = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _article_category);
            }
            return _article_content;
        }
        #endregion

        #region 更新一筆文章資料
        /// <summary>
        /// 傳進ArticleContentController取得的Get_article_content(id)確定有資料在處理
        /// 接收Request_data_mod傳進來的值，在傳給剛剛抓到的資料，覆蓋_article_content
        /// 先處理文章圖片更新，再更新資料庫
        /// 文章內容圖交給AddUpdateUploadImage單獨處理
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="_article_content">更新的資料類別</param>
        public void Update_article_content(HttpRequest _request, article_content _article_content)
        {
            _article_content = Request_data_mod(_request , _article_content);  //將接收來的參數，和抓到的要修改資料轉進article_content型別

            if (_request.Files.Count > 0)
            {
                //以下為圖片處理
                //1.取得文章要放置的目錄路徑
                string _RootPath_ImageFolderPath = Get_RootPath_ImageFolderPath();

                //Get_article_content裏面的image_name有加網址m_ImageFileHelper.GetImageLink，改只取檔案名與副檔
                string _image_name = Path.GetFileName(_article_content.image_name);

                //2.存放上傳文章封面圖片(1實體檔,2檔名,3路徑)
                m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _image_name, _RootPath_ImageFolderPath);

                //3.將編輯內容圖片裏剛產生的article_image表裏的圖檔尚未關連此文章_article_content.id更新進去
                //HandleContentImages(_request.Form["cNameArr"], _RootPath_ImageFolderPath, _article_content.id);
                //重點AddUpdateUploadImage在修改時使用加入內文圖片時已在article_image新增時就article_content_id加入         

                //4.刪除未在編輯內容裏的圖片
                //RemoveContentImages(_RootPath_ImageFolderPath);
            }
            string _sql = @"UPDATE  [article_content]
                                        SET 
                                         [article_category_id] = @article_category_id
                                        ,[title] = @title
                                        ,[subtitle] = @subtitle
                                        ,[brief] = @brief
                                        ,[content] = @content
                                        ,[updated_date] = @updated_date
                                        ,[Enabled] = @Enabled
                                        ,[Sort] = @Sort
                                        ,[first] = @first
                                        WHERE [id] = @id ";
            _IDapperHelper.ExecuteSql(_sql, _article_content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_request">讀取表單資料(修改時用)</param>
        /// <param name="_article_content">Get_article_content取到的要修改的資料</param>
        /// <returns></returns>
        private article_content Request_data_mod(HttpRequest _request, article_content _article_content)
        {
            //article_content _article_content = new article_content();
            //_article_content.id = Guid.NewGuid();
            _article_content.article_category_id = Convert.ToInt32(_request.Form["articleCategoryId"]);   //目錄id
            _article_content.title = _request.Form["title"];                                                                                        //文章標題
           //_article_content.image_name = _article_content.id + ".png";                                                           //圖片檔名使用id來當
            _article_content.subtitle = _request.Form["subtitle"];                                                                          //副標題
            _article_content.brief = _request.Form["brief"];                                                                                     //簡述
            _article_content.content = _request.Form["content"];                                                                          //內容
            _article_content.updated_date = DateTime.Now;                                                                                  //更新時間
            _article_content.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["enabled"]));       //是否啟用(0/1)

            if (string.IsNullOrWhiteSpace(_request.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                _article_content.Sort = Convert.ToInt32(Tools.GetInstance().DefaultValueSort);                  //沒有設定欄位給一個預設值
            }
            else
            {
                _article_content.Sort = Convert.ToInt32(_request.Form["sort"]);                                           //排序 
            }

            //是否置頂
            //if ("Y" == _request.Form["first"])
            //{ _article_content.first = "Y"; }
            //else
            //{ _article_content.first = "N"; }
            _article_content.first = Convert.ToBoolean(Convert.ToByte(_request.Form["first"]));                 //是否置頂(0 關閉;1 開啟)

            return _article_content;
        }
        #endregion

        #region 刪除
        /// <summary>
        /// 先刪article_image，再刪文章
        /// </summary>
        /// <param name="_article_content">傳入_IArticleContentService.Get_article_content(id);抓到的值</param>
        public void Delete_article_content(article_content _article_content)
        {
            //1.取得圖片要放置的目錄路徑
            string _RootPath_ImageFolderPath = Get_RootPath_ImageFolderPath();
            //string _sql = @"DELETE FROM [article_image] WHERE article_content_id = @id";
            string _sql = @"SELECT [image_name] FROM [article_image] where article_content_id = @id";

            //輸出<TReturn> 宣告<T1泛型，回傳類>(傳入字串，傳入類別)
            //IEnumerable<TReturn> QuerySetSql<T1, TReturn>(string sql, T1 paramData)
            //QuerySetSql<article_content, string>(_sql, _article_content)
            //_cn.Query<TReturn輸出的值>(sql, paramData);
            //取得這筆文章下相關內容圖片檔名
            List<string> _article_image_name_List = _IDapperHelper.QuerySetSql<article_content, string> (_sql, _article_content).ToList();

            string _article_image_name_List_Path = string.Empty; //宣告一個變數之後要加入路徑

            //刪除內容圖檔
            for (int i = 0; i < _article_image_name_List.Count; i++)
            {
                _article_image_name_List_Path = Path.Combine(_RootPath_ImageFolderPath, _article_image_name_List[i]);

                if (File.Exists(_article_image_name_List_Path))
                {
                    File.Delete(_article_image_name_List_Path);
                }
            }

            //取出文章圖片檔案
            string _image_name = Path.GetFileName(_article_content.image_name);  //取出檔名
            string _image_name_FilePath = Path.Combine(_RootPath_ImageFolderPath, _image_name);  //加上檔案路徑
            //刪除文章封面圖檔
            if (File.Exists(_image_name_FilePath))
            {
                File.Delete(_image_name_FilePath);
            }

            _sql = @"DELETE FROM [article_content] WHERE id = @id";
            //資料庫FK_article_image_article_content刪除有設定重疊顯示，所以刪主鍵關連資料會一同刪除
            _IDapperHelper.ExecuteSql(_sql, _article_content);
        }
        #endregion

        #region 取全部資料
        /// <summary>
        /// 取得全部文章(可加以下參數條件縮小範圍)
        /// </summary>
        /// <param name="_article_category_id">所在目錄下</param>
        /// <param name="_count">一頁要顯示幾筆</param>
        /// <param name="_page">第幾頁開始</param>
        /// <returns>_article_content_ies</returns>
        public List<article_content> Get_article_content_ALL(int? _article_category_id , int? _count, int? _page)
        {
            string page_sql = string.Empty;     //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string article_category_id_sql = string.Empty;  //所屬的目錄id sql

            if (_count !=null && _count > 0 && _page != null && _page > 0)
            {
                int startRowjumpover = 0;
                startRowjumpover = Convert.ToInt32((_page - 1) * _count);
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {_count} ROWS ONLY ";
            }

            string adminQuery = Auth.Role.IsAdmin ? "" : " where A2_CO.Enabled = 1 ";    //登入取得所有資料:未登入只能取得上線資料

            if (string.IsNullOrWhiteSpace(adminQuery))
            {
                article_category_id_sql = _article_category_id > 0 ? $" where article_category_id ={_article_category_id} " : "";
            }
            else
            {
                article_category_id_sql = _article_category_id > 0 ? $" and article_category_id ={_article_category_id} " : "";
            }

            //string _sql = $"SELECT * FROM [article_content] {adminQuery} " +
            //            $"{article_category_id_sql} " +
            //            $"Order by [FIRST] DESC , Sort , creation_date DESC " +
            //            $"{page_sql}";

            string _sql = $"SELECT A2_CO.*,A1_CA.name AS Article_Category_Name FROM [article_content] A2_CO " +
                                    $"LEFT JOIN [article_category] A1_CA ON A1_CA.id = A2_CO.article_category_id " +
                                    $"{adminQuery} " +
                                    $"{article_category_id_sql} " +
                                    $"Order by A2_CO.[FIRST] DESC , A2_CO.Sort , A2_CO.creation_date DESC  " +
                                    $"{page_sql}";

            List<article_content> _article_content_ies = _IDapperHelper.QuerySetSql<article_content>(_sql).ToList();

            for (int i = 0; i < _article_content_ies.Count; i++)
            {
                _article_content_ies[i].image_name = m_ImageFileHelper.GetImageLink(ConfigurationManager.AppSettings["Article"], _article_content_ies[i].image_name);
            }

            return _article_content_ies;
        }
        #endregion

        #region 加圖進入編輯器
        /// <summary>
        /// 新增一個article_image內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 新增時用
        /// </summary>
        /// <param name="_request">點選內文裏的圖片icon</param>
        /// <returns>_imageUrl</returns>
        public article_image AddUploadImage(HttpRequest _request)
        {
            article_image _article_image = new article_image();
            _article_image.article_image_id = Guid.NewGuid();                                     //自動產生一個Guid
            _article_image.image_name = _article_image.article_image_id + ".png"; //用id來命名圖檔

            //1.取得圖片要放置的目錄路徑
            string _RootPath_ImageFolderPath = Get_RootPath_ImageFolderPath();

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _article_image.image_name, _RootPath_ImageFolderPath);

            //3.取得要回傳的網址
            //string _imageUrl = m_ImageFileHelper.GetImageLink(ConfigurationManager.AppSettings["Article"], _article_image.image_name);

            //3.加入資料庫
            string _sql = @" INSERT INTO [article_image]
                                                   ([article_image_id]
                                                   ,[image_name])
                                             VALUES
                                                   (@article_image_id,
                                                    @image_name)
                                                         SELECT SCOPE_IDENTITY()";
            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, _article_image);     //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            _article_image.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id

            //4.將圖片檔加上Url
            _article_image.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Article, _article_image.image_name);

            return _article_image;
        }

        /// <summary>
        /// 新增一個article_image內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// 修改時用
        /// </summary>
        /// <param name="_request">點選內文裏的圖片icon</param>
        /// <param name="_article_content_Id"></param>
        /// <returns>_article_image</returns>
        public article_image AddUpdateUploadImage(HttpRequest _request, Guid _article_content_Id)
        {
            article_image _article_image = new article_image();
            _article_image.article_image_id = Guid.NewGuid();                                     //自動產生一個Guid
            _article_image.image_name = _article_image.article_image_id + ".png"; //用id來命名圖檔
            _article_image.article_content_id = _article_content_Id;   //關聯文章id

            //1.取得圖片要放置的目錄路徑
            string _RootPath_ImageFolderPath = Get_RootPath_ImageFolderPath();

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _article_image.image_name, _RootPath_ImageFolderPath);

            //3.取得要回傳的網址
            //string _imageUrl = m_ImageFileHelper.GetImageLink(ConfigurationManager.AppSettings["Article"], _article_image.image_name);

            //3.加入資料庫, 這裏會給@article_content_id值，這樣article_image的article_content_id就會有值
            string _sql = @" INSERT INTO [article_image]
                                                   ([article_image_id]
                                                   ,[article_content_id]
                                                   ,[image_name]
                                                    )
                                             VALUES
                                                   (@article_image_id,
                                                    @article_content_id,
                                                    @image_name )
                                                        SELECT SCOPE_IDENTITY()";
            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, _article_image);   //1.新增資料需使用QuerySingle，因Execute所傳回是新增成功數值
            _article_image.id = id;

            //4.將圖片檔加上Url
            _article_image.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Article, _article_image.image_name);

            return _article_image;
        }
        #endregion


        #region 工具區
        /// <summary>
        /// 取得網站根目錄資料路徑並加上指定的文章目錄路徑
        /// </summary>
        /// <returns>_RootPath_ImageFolderPath</returns>
        private string Get_RootPath_ImageFolderPath()
        {
            string m_imageFolder = @"\Admin\backStage\img\Article\";
            string _RootPath = HttpContext.Current.Server.MapPath("/");
            string _RootPath_Del_Slash = Path.GetDirectoryName(_RootPath);  //去除尾部\\
            string _RootPath_Up = Path.GetDirectoryName(_RootPath_Del_Slash); //再跑一次會往上一層推
            string _RootPath_ImageFolderPath = m_ImageFileHelper.GetImageFolderPath(_RootPath_Up, m_imageFolder); //將兩個路徑結合
        
            return _RootPath_ImageFolderPath;
        }
        /// <summary>
        /// article_image
        /// 搬移內容圖片位置和建立文章和內容圖片之間的連結
        /// 將內容裏的圖片cNameArr["abc.png","def.png"]先辨斷在目錄下是否檔案有存在，若有則將新文意的id更新給這個圖檔
        /// 新增文章的資料用到
        /// </summary>
        /// <param name="images">_request.Form["cNameArr"]內容圖片的檔名Json字串</param>
        /// <param name="saveFolderPath">圖片保存目錄</param>
        /// <param name="article_content_id">關連文章的ID</param>
        private void HandleContentImages(string images, string saveFolderPath, Guid article_content_id)
        {
            //取得回傳內容圖片的檔名cNameArr["abc.png","def.png"]， Json反序列化(Deserialize)為物件(Object)
            List<string> _retFileNameList = JsonConvert.DeserializeObject<List<string>>(images);

            //取得存放目錄下所有.png圖檔路徑
            List<string> _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();

            if (_retFileNameList != null && _retFileNameList.Count > 0)  //cNameArr確認有檔才處理
            {
                for (int i = 0; i < _retFileNameList.Count; i++)  //處理裏面的所有檔cNameArr ["abc.png","def.png"]
                {
                    for (int y = 0; y < _imageFileList.Count; y++)  //和目錄下實際所有.png圖檔+路徑比對一下
                    {
                        string _urlFileName = _retFileNameList[i].ToLower().Trim();  //cNameArr內的檔轉小寫去空白
                        string _entityFileName = Path.GetFileName(_imageFileList[y]);  //目錄下的檔只需留檔案名稱
                        string _imageGuid = Path.GetFileNameWithoutExtension(_imageFileList[y]);  //傳回[沒有副檔名]的指定路徑字串的[檔案名稱]。

                        //判斷cNameArr[]傳進來的參數圖檔==目前目錄下的檔&&判斷目錄下的檔案是否存在。
                        if (_urlFileName == _entityFileName && File.Exists(_imageFileList[y]))
                        {
                            article_image _article_image = new article_image();
                            _article_image.article_image_id = Guid.Parse(_imageGuid);                       //存放目錄下檔名不含副檔給article_image表裏id
                            _article_image.article_content_id = article_content_id;     //關連文章的ID

                            //更新文章內容的id給article_image，
                            //UploadImage在新增內文圖文時新增完成後關連文章article_content_id還未取得，因為文章還未新增
                            //當內文裏有插入新的圖片時此時的article_content_id=null, AddUploadImage實際已新增資料庫
                            //並且上傳檔案，只是關連article_content_id暫為null，若在新增文章前未將內文裏的圖片移除
                            //並新增完成，update就會附給他關連id, 但若只是編輯最後確移除此圖，實際資料庫與圖檔都還在
                            //cNameArr在編輯送出時就不會有這個圖片的記錄，也不會被附予article_content_id，所以下面的移除
                            //這時就要交給RemoveContentImages來將資料庫的null刪除
                            string _sql = @"UPDATE [article_image] 
                                            SET article_content_id = @article_content_id
                                            WHERE article_image_id = @article_image_id";

                            _IDapperHelper.ExecuteSql(_sql, _article_image);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 處理 article_image
        /// 刪除不用的內容圖片，抓出資料庫[article_image]裏article_content_id是null的資料比對目前存放的檔案
        /// 找到就刪除並清除資料庫的資料
        /// 新增資料才用到,新增時在編輯內容資料時，插入一個圖片，但這時圖片資料庫都已產生
        /// 若此時將編輯中的圖片刪掉，只是畫面刪掉，但其實這個圖片在資料庫與目錄裏都還在
        /// </summary>
        /// <param name="imageFileList">多餘的內容圖片路徑集合</param>
        private void RemoveContentImages(string saveFolderPath)
        {
            //取得存放文章目錄下所有.png圖檔路徑 GetFiles:傳回指定目錄中的檔案名稱 (包括路徑)
            var _imageFileList = Directory.GetFiles(saveFolderPath, "*.png").ToList();

            //查詢資料庫為NULL有那些，比對實際目錄下有那些，找到的就刪除
            string _sql = @"SELECT [image_name] FROM [article_image] WHERE article_content_id IS NULL";
            List<string> _imageDataBaseList = _IDapperHelper.QuerySetSql<string>(_sql).ToList();

            //刪除多餘的內容圖檔, _imageDataBaseList資料庫檔案名清單，_imageFileList實際目錄檔路徑檔案清單
            for (int y = 0; y < _imageDataBaseList.Count; y++)
            {
                for (int i = 0; i < _imageFileList.Count; i++)
                {
                    string _removeFileName = Path.GetFileName(_imageFileList[i]);  //除掉路徑

                    //資料庫檔==存放檔&&存放檔是否存在
                    if (_imageDataBaseList[y] == _removeFileName && File.Exists(_imageFileList[i])) 
                    {
                        File.Delete(_imageFileList[i]);
                    }
                }
            }

            //刪除資料庫裡無相關連的內容圖片記錄
            _sql = @"DELETE FROM [article_image] WHERE article_content_id IS NULL";

            _IDapperHelper.ExecuteSql(_sql);
        }
        #endregion


    }
}