using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IDonateRelatedItemService
    {
        /// <summary>
        /// 1.新增訂購明細項目
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        DonateRelatedItem Insert_DonateRelatedItem(HttpRequest _request);

        /// <summary>
        ///2. 取得一筆訂購明細項目
        /// </summary>
        /// <param name="id">輸入id編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        DonateRelatedItem Get_DonateRelatedItem(Guid id);

        /// <summary>
        /// 3.修改訂購明細項目
        /// </summary>
        /// <param name="request">輸入id編號</param>
        /// <param name="DonateRelatedItem">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void Update_DonateRelatedItem(HttpRequest request, DonateRelatedItem DonateRelatedItem);

        /// <summary>
        /// 3.刪除一筆
        /// </summary>
        /// <param name="DonateRelatedItem">用id找出資料後給類別，類別id在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void Delete_DonateRelatedItem(DonateRelatedItem DonateRelatedItem);

        /// <summary>
        /// 4.取得所有訂購明細項目
        /// </summary>
        /// <param name="primary">主類: 1一般捐款 2結緣捐贈</param>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <returns>DonateRelatedItem</returns>
        List<DonateRelatedItem> Get_DonateRelatedItem_ALL(string primary, int? count, int? page);


        /// <summary>
        /// 01新增一個DonateRelatedItemImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="request"></param>
        /// <returns>201, DonateRelatedItemImage</returns>
        DonateRelatedItemImage AddUploadImage(HttpRequest request);

        /// <summary>
        /// 02新增一筆DonateRelatedItemImage內容圖片,有關聯ID, 適用於在修改時使用
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="donateRelatedItemId">donateRelatedItemId</param>
        /// <returns>201, DonateRelatedItemImage</returns>
        DonateRelatedItemImage AddUploadImage(HttpRequest request, Guid donateRelatedItemId);
    }
}
