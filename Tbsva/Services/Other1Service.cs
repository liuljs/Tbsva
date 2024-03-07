using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class Other1Service : IOther1Service
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;

        public Other1Service(IDapperHelper iDapperHelper, IImageFileHelper imageFileHelper)
        {
            _IDapperHelper = iDapperHelper;
            m_ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region 新增一筆資料
        /// <summary>
        /// 新增一筆資料
        /// </summary>
        /// <param name="request">接收content</param>
        /// <returns></returns>
        public Other1 Insert_Other1(HttpRequest _request)
        {
            Other1 _other1 = new Other1();
            _other1.id = Guid.NewGuid();
            _other1.content = _request.Form["content"];                      //內容
            _other1.creation_date = DateTime.Now;
            string _sql = @"INSERT INTO [Other1]
                                       ( [id]
                                        ,[content]
                                        ,[creation_date])
                                       VALUES
                                       ( @id,
                                         @content,
                                         @creation_date ) ";
            _IDapperHelper.ExecuteSql(_sql, _other1);
            return _other1;   //這裏的資料是來自上面表單收集來的
        }
        #endregion

        #region 刪除所有資料
        public void DeleteAllContents()
        {
            string _sql = @"TRUNCATE TABLE [Other1]";
            _IDapperHelper.ExecuteSql(_sql);
        }
        #endregion

        #region 編輯時插入圖片小圖
        /// <summary>
        /// 在內容裏插入一張圖並上傳後在傳回url路徑
        /// </summary>
        /// <param name="_request">點選編輯插入圖片小圖</param>
        /// <returns>_imageUrl</returns>
        public string AddImage(HttpRequest _request)
        {
            //設定圖片檔名
            string _strFileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_fff")}" + ".png";
            //處理圖片
            //1取放圖路徑GetImagePathName(指定目錄)
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Other1);
            //2上傳圖片(1實體圖片檔, 2檔名, 3放圖路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _strFileName, _RootPath_ImageFolderPath);

            //3回傳http圖片網址GetImageLink(1.指定目錄,2檔名)
            string _imageUrl = Tools.GetInstance().GetImageLink(Tools.GetInstance().Other1, _strFileName);

            return _imageUrl;
        }
        #endregion

        #region 取出資料
        /// <summary>
        /// 取出裏面所有資料，只會有一筆
        /// </summary>
        /// <returns></returns>
        public Other1 Get_Other1()
        {
            Other1 _other1 = new Other1();
            string _sql = $"SELECT top 1 * FROM [Other1] order by creation_date desc";

            _other1 = _IDapperHelper.QuerySqlFirstOrDefault<Other1>(_sql);

            return _other1;
        }
        #endregion

    }
}