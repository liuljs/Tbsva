using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IActivityService
    {
        /// <summary>
        /// 1.新增最新活動
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        Activity InsertActivity(HttpRequest request);
        /// <summary>
        /// 01新增一個ActivityImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="request"></param>
        /// <returns>201, ActivityImage</returns>
        ActivityImage AddUploadImage(HttpRequest request);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <param name="Id">輸入id編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        Activity GetActivity(Guid Id);

        /// <summary>
        ///  3.修改
        /// </summary>
        /// <param name="request">輸入id編號</param>
        /// <param name="activity">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateActivity(HttpRequest request, Activity activity);
        /// <summary>
        /// 02新增一筆ActivityImage內容圖片,有關聯ID, 適用於在修改時使用
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="Activity_Id">活動管理Id</param>
        /// <returns>201, ActivityImage</returns>
        ActivityImage AddUploadImage(HttpRequest request, Guid Activity_Id);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="activity">用id找出資料後給類別，類別id在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void DeleteActivity(Activity activity);

        /// <summary>
        /// 5.顯示活動管理的資料
        /// </summary>
        /// <param name="categoryId">活動隸屬目錄 ID</param>
        /// <param name="count">一頁要顯示幾筆</param>
        /// <param name="page">第幾頁開始</param> 
        /// <returns>List<Activity></returns>
        List<Activity> GetActivities(string categoryId, int? count, int? page);

        /// <summary>
        /// 6.列表區的置頂設定
        /// </summary>
        /// <param name="activity"></param>
        ///void UpdateTopOption(Activity activity);
    }
}
