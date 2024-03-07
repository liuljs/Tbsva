using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Helpers
{
    public class ImageFormatHelper : IImageFormatHelper
    {
        /// <summary>
        /// 透過MIME檢查上傳檔案是否為圖檔
        /// </summary>
        /// <param name="fileCollection">上傳檔案的集合</param>
        /// <returns>true:為圖檔;fale:包含非圖檔</returns>
        public bool CheckImageMIME(HttpFileCollection fileCollection)
        {
            bool _isOk = true;

            for (int i = 0; i < fileCollection.Count; i++)
            {
                var file = fileCollection[i];
                string _type = file.ContentType.ToLower();
                if (!_type.Contains("image"))
                {
                    _isOk = false;
                    break;
                }
            }

            return _isOk;
        }

        /// <summary>
        /// 透過MIME檢查上傳檔案是否為圖檔
        /// </summary>
        /// <param name="fileCollection">上傳檔案的集合</param>
        /// <returns>true:有圖檔才檢查沒圖不檢查;fale:包含非圖檔</returns>
        public bool CheckImageMIME2(HttpFileCollection fileCollection)
        {
            bool _isOk = true;  //代表圖片正常

            //處理多張圖檔上傳檢查，欄位選項有勾選才檢查，修改時上傳圖檔非必選
            if (fileCollection.Count > 0)      //Postman左邊欄位有勾才會計算不然會是NULL不存在
            {
                for (int i = 0; i < fileCollection.Count; i++)
                {
                    HttpPostedFile _file = fileCollection[i];
                    //string _type = _file.ContentType.ToLower();  //err ToLower設定在這裏，若file沒有上傳檔案時會是null, null.ToLower()就錯了
                    string _type = _file.ContentType;  //辨斷檔案格式，若沒有上傳會是null
                    //辨斷有上傳或有容量才進入檢查
                    if (_type != null && _file.ContentLength > 0)      //沒有檔時會出現null，確定有上傳檔在檢查
                    {
                        if (!_type.ToLower().Contains("image"))
                        {
                            _isOk = false;
                            break;
                        }
                    }
                }
            }

            return _isOk;
        }

    }
}