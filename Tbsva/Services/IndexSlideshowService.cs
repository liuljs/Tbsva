using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class IndexSlideshowService : IIndexSlideshowService
    {
        #region DI依賴注入功能
        private IDapperHelper DapperHelper;
        private IImageFileHelper ImageFileHelper;
        public IndexSlideshowService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            DapperHelper = dapperHelper;
            ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region  新增實作
        public IndexSlideshow InsertIndexSlideshow(HttpRequest httpRequest)
        {
            IndexSlideshow indexSlideshow = RequestData(httpRequest);

            //設定上傳檔案的檔名對應相同的類別名稱
            //HttpRequest.Files 屬性 取得用戶端所上傳的檔案集合
            //httpRequest.Files 將文件集合加載到 HttpFileCollection 變量中。
            SetImageFileName(indexSlideshow, httpRequest.Files);

            string _sql = @"INSERT INTO [index_slideshow2]
                                   ([slideId]
                                   ,[name]
                                   ,[fullImage]
                                   ,[tabletImage]
                                   ,[smPhoneImage]
                                   ,[hyperlink]
                                   ,[first]
                                   ,[sort]
                                   ,[enabled]
                                   ,[creationDate])
                             VALUES
                                   (@slideId,
                                    @name,
                                    @fullImage,
                                    @tabletImage,
                                    @smPhoneImage,
                                    @hyperlink,
                                    @first,
                                    @sort,
                                    @enabled,
                                    @creationDate )
                                    SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。

            int id = DapperHelper.QuerySingle(_sql, indexSlideshow);
            indexSlideshow.id = id;  //取得剛剛新增的id，回傳給類別id，這樣回傳時才會有id
                                     //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(httpRequest.Form["sort"]))         //空值,或空格，或沒設定此欄位null
            {
                indexSlideshow.sort = id;        //沒有Sort值時給剛新增的id值
            }
            else
            {
                indexSlideshow.sort = Convert.ToInt32(httpRequest.Form["sort"]);       //有值就給值排序 
            }
            _sql = @"UPDATE [index_slideshow2] 
                                SET [SORT] = @SORT 
                                WHERE [ID] = @ID ";
            DapperHelper.ExecuteSql(_sql, indexSlideshow);         //更新排序

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Banners) ;
            string _saveFolderPath = ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{id}\");
            //2處理多張圖檔上傳
            UploadImage(httpRequest.Files, _saveFolderPath);

            return indexSlideshow;
        }
        private IndexSlideshow RequestData(HttpRequest httpRequest)
        {
            IndexSlideshow indexSlideshow = new IndexSlideshow();  //設定一組空的
            indexSlideshow.slideId = Guid.NewGuid();

            //indexSlideshow.name = httpRequest.Form["name"];                                                                             //輪播圖片名稱
            indexSlideshow.name = "Name";                                                                             //輪播圖片名稱
            indexSlideshow.hyperlink = httpRequest.Form["hyperlink"];                                                                //超連結址
            indexSlideshow.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            indexSlideshow.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            indexSlideshow.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            indexSlideshow.creationDate = DateTime.Now;

            return indexSlideshow;
        }
        #endregion

        #region 取得一筆資料
        public IndexSlideshow GetIndexSlideshow(Guid id)
        {
            IndexSlideshow indexSlideshow = new IndexSlideshow();
            indexSlideshow.slideId = id;

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 ";
            string _sql = $"SELECT * FROM [index_slideshow2] WHERE [slideId] = @slideId {adminQuery}";
            indexSlideshow = DapperHelper.QuerySqlFirstOrDefault(_sql, indexSlideshow);

            //加上http網址
            if (indexSlideshow != null)  //確定有資料在處理
            {
                if (indexSlideshow.fullImage != null)
                {
                    indexSlideshow.fullImage = Tools.GetInstance().GetImageLink(Tools.GetInstance().Banners + $@"/{indexSlideshow.id}", indexSlideshow.fullImage);
                }
                if (indexSlideshow.tabletImage != null)
                {
                    indexSlideshow.tabletImage = Tools.GetInstance().GetImageLink(Tools.GetInstance().Banners + $@"/{indexSlideshow.id}", indexSlideshow.tabletImage);
                }
                if (indexSlideshow.smPhoneImage != null)
                {
                    indexSlideshow.smPhoneImage = Tools.GetInstance().GetImageLink(Tools.GetInstance().Banners + $@"/{indexSlideshow.id}", indexSlideshow.smPhoneImage);
                }
            }
             return indexSlideshow;
        }
        #endregion

        public void UpdateIndexSlideshow(HttpRequest httpRequest, IndexSlideshow indexSlideshow)
        {
            indexSlideshow = RequestDataMod(httpRequest, indexSlideshow);  //將接收來的參數，和抓到的要修改資料合併
            
            //設定圖片檔名
            SetImageFileName(indexSlideshow, httpRequest.Files);

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Banners);
            string _saveFolderPath = ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{indexSlideshow.id}\");
            //2處理多張圖檔上傳
            UploadImage(httpRequest.Files, _saveFolderPath);

            //存入資料庫時先將圖片前的url去除
            //if (indexSlideshow.fullImage != null)
            //{
            //    indexSlideshow.fullImage = Path.GetFileName(indexSlideshow.fullImage);
            //}
            //if (indexSlideshow.tabletImage != null)
            //{
            //    indexSlideshow.tabletImage = Path.GetFileName(indexSlideshow.tabletImage);
            //}
            //if (indexSlideshow.smPhoneImage != null)
            //{
            //    indexSlideshow.smPhoneImage = Path.GetFileName(indexSlideshow.smPhoneImage);
            //}

            //組裝修改刪除不變更時sql
            string _sqlpic = picSql(indexSlideshow, httpRequest);

            string _sql = $@"UPDATE [index_slideshow2]
                                                SET   
                                                      [name] = @name
                                                      {_sqlpic}
                                                      ,[hyperlink] = @hyperlink
                                                      ,[first] = @first
                                                      ,[sort] = @sort
                                                      ,[enabled] = @enabled
                                                      ,[updatedDate] = @updatedDate
                                                    WHERE [slideId] = @slideId";

            DapperHelper.ExecuteSql(_sql, indexSlideshow);
        }
        /// <summary>
        /// 讀取表單資料
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="indexSlideshow"></param>
        /// <returns></returns>
        private IndexSlideshow RequestDataMod(HttpRequest httpRequest, IndexSlideshow original_indexSlideshow)
        {
            //original_indexSlideshow是controller抓資料庫的資料
            IndexSlideshow indexSlideshow = new IndexSlideshow();               //設定一組空的沒資料會null, 讓圖片預設是null不執行SQL指令，此null是類別裏真的null 
            indexSlideshow.id = original_indexSlideshow.id;                //將資料庫id傳給空的indexSlideshow
            indexSlideshow.slideId = original_indexSlideshow.slideId;                //將資料庫slideId傳給空的indexSlideshow
            //indexSlideshow.name = httpRequest.Form["name"];                                                                               //輪播圖片名稱
            indexSlideshow.name = "Name";                                                                             //輪播圖片名稱
            indexSlideshow.hyperlink = httpRequest.Form["hyperlink"];                                                                 //超連結址
            indexSlideshow.first = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["first"]));                    //是否置頂(0 關閉;1 開啟)
            indexSlideshow.sort = Convert.ToInt32(httpRequest.Form["sort"]);                                                      //排序,預設可為流水號編號（從編號 1 號開始;設為 0 即為為置頂）
            indexSlideshow.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));     //是否啟用(0 關閉;1 開啟)
            indexSlideshow.updatedDate = DateTime.Now;

            return indexSlideshow;
        }

        public void DeleteIndexSlideshow(IndexSlideshow indexSlideshow)
        {
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Banners);
            //string _saveFolderPath = ImageFileHelper.GetImageFolderPath(_RootPath_ImageFolderPath, $@"{indexSlideshow.id}\");

            string folder = Path.Combine(_RootPath_ImageFolderPath, indexSlideshow.id.ToString());
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder,true);
            }
            string _sql = @"DELETE FROM [index_slideshow2] WHERE [slideId] = @slideId";
            //執行刪除
            DapperHelper.ExecuteSql(_sql, indexSlideshow);
        }

        public List<IndexSlideshow> GetIndexSlideshows()
        {
            string adminQuery = Auth.Role.IsAdmin ? "" : " WHERE ENABLED = 1 ";
            string _sql = $"SELECT * FROM [index_slideshow2]  {adminQuery} " +
                      @" ORDER BY [FIRST] DESC , CREATIONDATE DESC , SORT ";

            List<IndexSlideshow> indexSlideshows = DapperHelper.QuerySetSql<IndexSlideshow>(_sql).ToList();

            //將資料庫圖檔加上URL
            for (int i = 0; i < indexSlideshows.Count; i++)
            {
                if (indexSlideshows[i].fullImage != null)
                {
                    indexSlideshows[i].fullImage = Tools.GetInstance().GetImageLink(Tools.GetInstance().Banners + $@"/{indexSlideshows[i].id}", indexSlideshows[i].fullImage);
                }
                if (indexSlideshows[i].tabletImage != null)
                {
                    indexSlideshows[i].tabletImage = Tools.GetInstance().GetImageLink(Tools.GetInstance().Banners + $@"/{indexSlideshows[i].id}", indexSlideshows[i].tabletImage);
                }
                if (indexSlideshows[i].smPhoneImage != null)
                {
                    indexSlideshows[i].smPhoneImage = Tools.GetInstance().GetImageLink(Tools.GetInstance().Banners + $@"/{indexSlideshows[i].id}", indexSlideshows[i].smPhoneImage);
                }
            }

            return indexSlideshows;
        }


        /// <summary>
        /// 設定上傳檔案的檔名對應相同的類別名稱
        /// </summary>
        /// <param name="indexSlideshow">類別</param>
        /// <param name="fileCollection">httpRequest.Files</param>
        /// https://docs.microsoft.com/zh-tw/dotnet/api/system.web.httprequest.files?view=netframework-4.7.2&f1url=%3FappId%3DDev16IDEF1%26l%3DZH-TW%26k%3Dk(System.Web.HttpRequest.Files);k(TargetFrameworkMoniker-.NETFramework,Version%253Dv4.7.2);k(DevLang-csharp)%26rd%3Dtrue
        private void SetImageFileName(IndexSlideshow indexSlideshow, HttpFileCollection fileCollection)
        {
            // Files.AllKeys 這會將所有文件的名稱放入一個字符串數組中。
            //keyName (fullImage，tabletImage，smPhoneImage) = AllKeys
            foreach (string keyName in fileCollection.AllKeys)
            {
                HttpPostedFile file = fileCollection[keyName];
                if (file != null && file.ContentLength > 0)
                {
                    switch (keyName.ToLower())
                    {
                        case "fullimage":
                            indexSlideshow.fullImage = file.FileName;
                            break;
                        case "tabletimage":
                            indexSlideshow.tabletImage = file.FileName;
                            break;
                        case "smphoneimage":
                            indexSlideshow.smPhoneImage = file.FileName;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 組裝修改刪除不變更時sql
        /// </summary>
        /// <param name="indexSlideshow"></param>
        /// <param name="_request"></param>
        /// <returns></returns>
        private string picSql(IndexSlideshow indexSlideshow, HttpRequest _request)
        {
            string pic = string.Empty;
            //var null = _request.Form["Picture01"];
            //圖片更新有4種狀況
            //1.新增:選圖片上傳_request.Files;AllKeys就會有欄位,檔名就會代給類別_pictureList.Picture01 = _file.FileName
            //因為Picture01欄位有圖檔, 所以_request.Form["Picture01"]就不見變成null, 但下面的null並不是真的null, 而時字串的"null"
            //所以新增時以下左邊會成立，右邊不成立
            //2.修改:有重新上傳的_request.Files,AllKeys就會有欄位會有值, 
            //3.沒有變更:沒有重新上傳檔案,但在_request.Form[""] = xx.PNG會接到現有的檔案名稱，所以下面條件都不成立
            //3.按垃圾筒,並沒有上傳Files內容，而是傳回表單字串null(_request.Form["xx"] == "null"，所以會執行下面右方sql將資料欄改NULL
            //4.完全沒變動:會變成_request.Form的值, 原本有圖檔的如_request.Form["Picture01"]=01.PNG, 沒有圖檔的就會是空值，下面條件都不成立
            if (indexSlideshow.fullImage != null) pic += ",[fullImage]=@fullImage"; if (_request.Form["fullImage"] == "null") pic += ",[fullImage]=NULL";
            if (indexSlideshow.tabletImage != null) pic += ",[tabletImage]=@tabletImage"; if (_request.Form["tabletImage"] == "null") pic += ",[tabletImage]=NULL";
            if (indexSlideshow.smPhoneImage != null) pic += ",[smPhoneImage]=@smPhoneImage"; if (_request.Form["smPhoneImage"] == "null") pic += ",[smPhoneImage]=NULL";

            return pic;
        }


        /// <summary>
        /// 處理多張圖檔上傳
        /// </summary>
        /// <param name="fileCollection">多檔傳進來Request.Files</param>
        /// <param name="_RootPath_ImageFolderPath">路徑</param>
        /// HttpFileCollection Files = Request.Files;
        ///foreach (string FileKey in Files.AllKeys) {
        ///    HttpPostedFile uplodaFile = Files[FileKey];
        ///    if (uplodaFile.ContentLength > 0) {
        ///       //do something...
        ///    }
        ///}
        ///https://dotblogs.com.tw/cloudio/2008/11/11/5958
        ///https://csharp.hotexamples.com/examples/System.Web/HttpFileCollection/-/php-httpfilecollection-class-examples.html
        private void UploadImage(HttpFileCollection fileCollection, string _RootPath_ImageFolderPath)
        {
            for (int i = 0; i < fileCollection.Count; i++)
            {
                HttpPostedFile _file = fileCollection[i];
                if (_file.ContentLength > 0)
                    ImageFileHelper.SaveUploadImageFile(_file, _file.FileName, _RootPath_ImageFolderPath);
            }
        }




    }
}