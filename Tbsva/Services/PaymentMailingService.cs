using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class PaymentMailingService : IPaymentMailingService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;

        public PaymentMailingService(IDapperHelper IDapperHelper, IImageFileHelper imageFileHelper)
        {
            _IDapperHelper = IDapperHelper;
            m_ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region 新增一筆資料
        /// <summary>
        /// 新增一筆資料
        /// </summary>
        /// <param name="_request"></param>
        /// <returns></returns>
        public PaymentMailing Insert_PaymentMailing(HttpRequest _request)
        {
            PaymentMailing _paymentMailing = new PaymentMailing();
            _paymentMailing.id = Guid.NewGuid();
            _paymentMailing.content = _request.Form["content"];                      //內容
            _paymentMailing.creation_date = DateTime.Now;                              //新增時間
            string _sql = @"INSERT INTO [PaymentMailing]
                                ( [id]
                                 ,[content]
                                 ,[creation_date])
                            VALUES
                                ( @id,
                                  @content,
                                  @creation_date ) ";
            _IDapperHelper.ExecuteSql(_sql, _paymentMailing);
            return _paymentMailing;
        }
        #endregion

        #region 刪除所有資料
        /// <summary>
        /// 刪除所有資料(在新增時會先用到)
        /// </summary>
        /// <returns></returns>
        public int DeleteAllContents()
        {
            string _sql = @"TRUNCATE TABLE [PaymentMailing]";
            int result = _IDapperHelper.ExecuteSql(_sql);
            return result;
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
            string _strFileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_fff")}" + ".png";

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().PaymentMailing);
            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            m_ImageFileHelper.SaveUploadImageFile(_request.Files[0], _strFileName, _RootPath_ImageFolderPath);

            //3.取得要回傳的網址
            string _imageUrl = Tools.GetInstance().GetImageLink(Tools.GetInstance().PaymentMailing, _strFileName);

            return _imageUrl;
        }
        #endregion

        #region 取出資料
        /// <summary>
        /// 取出裏面所有資料，只會有一筆
        /// </summary>
        /// <returns></returns>
        public PaymentMailing Get_PaymentMailing()
        {
            PaymentMailing _paymentMailing = new PaymentMailing();

            string _sql = $"SELECT top 1 * FROM [PaymentMailing] order by creation_date desc";

            _paymentMailing = _IDapperHelper.QuerySqlFirstOrDefault(_sql, _paymentMailing);

            return _paymentMailing;
        }
        #endregion

    }
}