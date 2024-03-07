using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface ITaiwanDojoService
    {
        /// <summary>
        /// 1.新增
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        TaiwanDojo InsertTaiwanDojo(HttpRequest request);
        /// <summary>
        /// 01新增一個TaiwanDojoImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="request"></param>
        /// <returns>201, ActivityImage</returns>
        TaiwanDojoImage AddUploadImage(HttpRequest request);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <param name="taiwanDojoId">輸入contentId編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        TaiwanDojo GetTaiwanDojo(Guid taiwanDojoId);

        /// <summary>
        ///  3.修改
        /// </summary>
        /// <param name="request">輸入taiwanDojoId編號</param>
        /// <param name="taiwanDojo">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateTaiwanDojo(HttpRequest request, TaiwanDojo taiwanDojo);
        /// <summary>
        /// 02新增一筆TaiwanDojoImage內容圖片,有關聯taiwanDojoId, 適用於在修改時使用
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="taiwanDojoId">taiwanDojoId</param>
        /// <returns>201, TaiwanDojoIdImage</returns>
        TaiwanDojoImage AddUploadImage(HttpRequest request, Guid taiwanDojoId);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="taiwanDojo">用taiwanDojoId找出資料後給類別，類別contentId在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void DeleteTaiwanDojo(TaiwanDojo taiwanDojo);

        /// <summary>
        /// 5.顯示所有資料
        /// </summary>
        /// <param name="categoryId">隸屬目錄 ID</param>
        /// <param name="count">一頁要顯示幾筆</param>
        /// <param name="page">第幾頁開始</param> 
        /// <returns>List<TaiwanDojo></returns>
        List<TaiwanDojo> GetTaiwanDojos(string categoryId, int? count, int? page);
    }
}
