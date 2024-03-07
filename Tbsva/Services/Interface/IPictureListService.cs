using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IPictureListService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_request">接收表單非檔案資料</param>
        /// <param name="fileCollection">HttpFileCollection類別可讓您存取從用戶端上傳為檔案集合的所有檔案。 HttpPostedFile類別會提供屬性和方法，以取得個別檔案的相關資訊，並讀取和儲存檔案。</param>
        /// <param name="_pictureList"></param>

        void Update_PictureList(HttpRequest _request, HttpFileCollection fileCollection, PictureList _pictureList);

        /// <summary>
        /// 取得供請照圖片列表
        /// </summary>
        /// <returns></returns>
        PictureList Get_PictureList();
    }
}
