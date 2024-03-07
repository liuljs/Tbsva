using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class PictureListService : IPictureListService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;

        public PictureListService(IDapperHelper IDapperHelper, IImageFileHelper imageFileHelper)
        {
            _IDapperHelper = IDapperHelper;
            m_ImageFileHelper = imageFileHelper;
        }
        #endregion
        /// <summary>
        /// 取得供請照圖片列表裏面唯一一筆資料
        /// </summary>
        /// <returns></returns>
        public PictureList Get_PictureList()
        {
            PictureList _pictureList = new PictureList();

            string _sql = @"SELECT top 1 * FROM [PictureList]";

            _pictureList = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _pictureList);

            return _pictureList;
        }

        /// <summary>
        /// 更新供請照圖片列表裏面唯一一筆資料
        /// </summary>
        /// <param name="_request">接收表單傳過來的值</param>
        /// <param name="fileCollection">接收檔案</param>
        /// <param name="original_pictureList">抓出資料庫原本的資料</param>
        /// <returns>204</returns>
        public void Update_PictureList(HttpRequest _request, HttpFileCollection fileCollection, PictureList original_pictureList)
        {
            PictureList _pictureList = new PictureList();  //設定一組空的null, 讓Picture01~10預設是不執行SQL指令，此null是類別裏真的null
            _pictureList.id = original_pictureList.id;

            //_pictureList = new PictureList();     //不能設定這一行不然會洗掉重來
            _pictureList.title = _request.Form["title"];                          //供請照簡述
            _pictureList.more_pic_url = _request.Form["more_pic_url"];   //連結網址
            _pictureList.updated_date = DateTime.Now;
            //接收上傳圖片並附給類別
            //string[] ArrayAllKeysName = fileCollection.AllKeys;  //陣列，讀取欄位名稱
            foreach (string key in fileCollection.AllKeys)
            {
                //var _FileName = _file.FileName;                             //上傳的檔名
                //if (_file.ContentLength == 0 && _file.ContentType == null && _FileName == string.Empty)      //辨斷欄位有勾選但沒選檔案
                //{
                //    _FileName = null;       //沒上傳檔案,檔名會變空值，改回null
                //}
                var _file = fileCollection[key];
                if (_file != null)  //辨斷_file集合是不是null ContentType資料的 內容類型,ContentLength,FileName 
                    switch (key.ToLower())
                    {
                        case "picture01":
                            _pictureList.Picture01 = _file.FileName;
                            break;
                        case "picture02":
                            _pictureList.Picture02 = _file.FileName;
                            break;
                        case "picture03":
                            _pictureList.Picture03 = _file.FileName;
                            break;
                        case "picture04":
                            _pictureList.Picture04 = _file.FileName;
                            break;
                        case "picture05":
                            _pictureList.Picture05 = _file.FileName;
                            break;
                        case "picture06":
                            _pictureList.Picture06 = _file.FileName;
                            break;
                        case "picture07":
                            _pictureList.Picture07 = _file.FileName;
                            break;
                        case "picture08":
                            _pictureList.Picture08 = _file.FileName;
                            break;
                        case "picture09":
                            _pictureList.Picture09 = _file.FileName;
                            break;
                        case "picture10":
                            _pictureList.Picture10 = _file.FileName;
                            break;
                    }
            }

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
            if (_pictureList.Picture01 != null) pic += ",[Picture01]=@Picture01"; if (_request.Form["Picture01"] == "null") pic += ",[Picture01]=NULL";
            if (_pictureList.Picture02 != null) pic += ",[Picture02]=@Picture02"; if (_request.Form["Picture02"] == "null") pic += ",[Picture02]=NULL";
            if (_pictureList.Picture03 != null) pic += ",[Picture03]=@Picture03"; if (_request.Form["Picture03"] == "null") pic += ",[Picture03]=NULL";
            if (_pictureList.Picture04 != null) pic += ",[Picture04]=@Picture04"; if (_request.Form["Picture04"] == "null") pic += ",[Picture04]=NULL";
            if (_pictureList.Picture05 != null) pic += ",[Picture05]=@Picture05"; if (_request.Form["Picture05"] == "null") pic += ",[Picture05]=NULL";
            if (_pictureList.Picture06 != null) pic += ",[Picture06]=@Picture06"; if (_request.Form["Picture06"] == "null") pic += ",[Picture06]=NULL";
            if (_pictureList.Picture07 != null) pic += ",[Picture07]=@Picture07"; if (_request.Form["Picture07"] == "null") pic += ",[Picture07]=NULL";
            if (_pictureList.Picture08 != null) pic += ",[Picture08]=@Picture08"; if (_request.Form["Picture08"] == "null") pic += ",[Picture08]=NULL";
            if (_pictureList.Picture09 != null) pic += ",[Picture09]=@Picture09"; if (_request.Form["Picture09"] == "null") pic += ",[Picture09]=NULL";
            if (_pictureList.Picture10 != null) pic += ",[Picture10]=@Picture10"; if (_request.Form["Picture10"] == "null") pic += ",[Picture10]=NULL";

            string _sql = $@"UPDATE [PictureList]
                                        SET
                                               [title] = @title
                                              ,[more_pic_url] = @more_pic_url 
                                               {pic} 
                                              ,[updated_date] = @updated_date";
            _IDapperHelper.ExecuteSql(_sql, _pictureList);

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().PictureList);
            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            //處理單張m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _pictureList.Picture01, _RootPath_ImageFolderPath);

            //2.處理多張圖檔上傳
            for (int i = 0; i < fileCollection.Count; i++)
            {
                HttpPostedFile _file = fileCollection[i];
                if (_file.ContentLength > 0)
                    m_ImageFileHelper.SaveUploadImageFile(_file, _file.FileName, _RootPath_ImageFolderPath);
            }

            //return _pictureList;
        }
    }
}