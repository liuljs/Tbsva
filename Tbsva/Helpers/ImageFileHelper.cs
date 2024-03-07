using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace WebShopping.Helpers
{
    public class ImageFileHelper : IImageFileHelper
    {
        private string m_WebSiteImgUrl = ConfigurationManager.AppSettings["WebSiteImgUrl"];

        /// <summary>
        /// 取得圖片存放目錄路徑
        /// </summary>
        /// <param name="rootFolder">初始路徑</param>
        /// <param name="imageFolder">圖片目錄</param>
        /// <returns>圖片目錄路徑</returns>
        public string GetImageFolderPath(string rootFolder, string imageFolder)
        {
            string _savePath = rootFolder + imageFolder;

            if (!Directory.Exists(_savePath))
                Directory.CreateDirectory(_savePath);

            return _savePath;
        }

        /// <summary>
        /// 保存上傳圖片
        /// </summary>
        /// <param name="postedFile">上傳圖片的PostedFile</param>
        /// <param name="fileName">圖片名稱</param>
        /// <param name="saveFolder">保存目錄</param>
        /// <returns>保存路徑</returns>
        public string SaveUploadImageFile(HttpPostedFile postedFile, string fileName, string saveFolder)
        {
            string _filePath = saveFolder + fileName;

            var file = postedFile;
            if (postedFile.ContentLength > 0)
            {
                file.SaveAs(_filePath);
            }
            else
            {
                throw new FileNotFoundException("上傳圖片檔案錯誤");
            }

            return _filePath;
        }

        /// <summary>
        ///  取得圖片連結字串
        /// </summary>
        /// <param name="p_strFolder"> 依此資料夾存放分類的圖片 </param>        
        /// <param name="p_strName"> 檔名 </param>        
        /// <returns> 圖片連結字串 </returns>
        public string GetImageLink(string folder, string fileName)
        {
            return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名           
        }


        /// <summary>
        /// 只要是file，有容量就上傳，檢查交給controller去擋
        /// </summary>
        /// <param name="fileCollection">request.Files提供用戶端所上傳檔案的存取權，並進行組織</param>
        /// <param name="_RootPath_ImageFolderPath">_saveFolderPath存放資料匣</param>
        /// <param name="file.FileName">依照迴圈自動取得用戶端上檔案的完整名稱</param>
        public void SaveMoreUploadImageFile(HttpFileCollection fileCollection, string _RootPath_ImageFolderPath)
        {
            for (int i = 0; i < fileCollection.Count; i++)
            {
                HttpPostedFile _file = fileCollection[i];
                if (_file.ContentLength > 0)
                {
                    string _filePath = Path.Combine(_RootPath_ImageFolderPath, _file.FileName);
                    _file.SaveAs(_filePath);
                }
                   // ImageFileHelper.SaveUploadImageFile(_file, _file.FileName, _RootPath_ImageFolderPath);
            }
        }

        /// <summary>
        /// 檢查有沒有cover封面圖片欄必填與上傳檔案
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="message">問題回應</param>
        /// <returns>_isChech2, message</returns>
        public bool CheckCover(HttpRequest httpRequest, out string message)
        {
            bool _isChech = false;
            message = string.Empty ;
            bool _isChech2 = false;  //有問題變true, 沒問題false跳過

            foreach (string fieldName in httpRequest.Files.AllKeys)
            {
                if (fieldName.ToLower() == "cover")  // 檢查有滿版圖片檔案後就跳出_isChech標true通過
                {
                    _isChech = true;  //1.確定有cover欄
                    if (httpRequest.Files[fieldName].ContentLength <= 0)    //cover沒有檔案容量
                    {
                        message = "必須上傳圖片檔案";
                        _isChech2 = true;
                    }
                    break;  //只是要檢查cover，確定有也有上傳就跳出foreach
                }
            }
            if (!_isChech)  //沒有cover這個file欄位
            {
                message = "必須有上傳圖片cover參數";
                _isChech2 = true;
            }

            return _isChech2;
        }

        /// <summary>
        /// 設定上傳檔案的檔名對應相同的類別名稱
        /// </summary>
        /// <param name="model">指定任何類別</param>
        /// <param name="fileCollection">httpRequest.Files</param>
        /// https://docs.microsoft.com/zh-tw/dotnet/api/system.web.httprequest.files?view=netframework-4.7.2&f1url=%3FappId%3DDev16IDEF1%26l%3DZH-TW%26k%3Dk(System.Web.HttpRequest.Files);k(TargetFrameworkMoniker-.NETFramework,Version%253Dv4.7.2);k(DevLang-csharp)%26rd%3Dtrue
        public void SetImageFileName(object model, HttpFileCollection fileCollection)
        {
            // Files.AllKeys 這會將所有文件的名稱放入一個字符串數組中。
            //keyName (cover，navpics01~08) = AllKeys
            foreach (string keyName in fileCollection.AllKeys)
            {
                HttpPostedFile file = fileCollection[keyName];
                if (file != null && file.ContentLength > 0)
                {
                    //👍
                    //var propertyInfo1 = model.GetType();
                    string PropertyName = keyName;  //優先以AllKeys取得的欄位名
                    //封面圖進來欄位名cover，但資料庫類別命是image_name
                    switch (keyName.ToLower())
                    {
                        case "cover":
                            PropertyName = "image_name";
                            break;
                    }
                    //var propertyInfo = model;
                    var propertyInfo = model.GetType().GetProperty(PropertyName);  //GetProperty(PropertyName)file欄位辨斷model是否有對應相同名稱欄位欄
                    //賦值需上一行先決定要取那個欄位model propertyInfo.SetValue(model, file.FileName);
                    //取值 string FiledContent = (string)propertyInfo.GetValue(model);
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(model, file.FileName);   // 確定model有要的屬性名, 將值傳入model對應欄位值 propertyInfo.SetValue(物件, 值);
                    }
                    //switch (keyName.ToLower())
                    //    {
                    //        case "cover":
                    //            activity.cover = file.FileName;
                    //            break;
                    //        case "navpics01":
                    //            activity.navPics01 = file.FileName;
                    //            break;
                    //        case "navpics02":
                    //            activity.navPics02 = file.FileName;
                    //            break;
                    //        case "navpics03":
                    //            activity.navPics03 = file.FileName;
                    //            break;
                    //        case "navpics04":
                    //            activity.navPics04 = file.FileName;
                    //            break;
                    //        case "navpics05":
                    //            activity.navPics05 = file.FileName;
                    //            break;
                    //        case "navpics06":
                    //            activity.navPics06 = file.FileName;
                    //            break;
                    //        case "navpics07":
                    //            activity.navPics07 = file.FileName;
                    //            break;
                    //        case "navpics08":
                    //            activity.navPics08 = file.FileName;
                    //            break;
                    //    }

                }
            }
        }


    }
}