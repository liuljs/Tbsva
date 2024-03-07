using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface INewsService
    {
        /// <summary>
        /// 1.新增最新活動
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        News InsertNews(HttpRequest request);
        /// <summary>
        /// 01新增一個NewsImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="request"></param>
        /// <returns>201, NewsImage</returns>
        NewsImage AddUploadImage(HttpRequest request);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <param name="Id">輸入id編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        News GetNews(Guid Id);

        /// <summary>
        ///  3.修改
        /// </summary>
        /// <param name="request">輸入id編號</param>
        /// <param name="news">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateNews(HttpRequest request, News news);
        /// <summary>
        /// 02新增一筆NewsImage內容圖片,有關聯ID, 適用於在修改時使用
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="News_Id">活動管理Id</param>
        /// <returns>201, NewsImage</returns>
        NewsImage AddUploadImage(HttpRequest request, Guid News_Id);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="news">用id找出資料後給類別，類別id在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void DeleteNews(News news);

        /// <summary>
        /// 5.顯示活動管理的資料
        /// </summary>
        /// <param name="count">一頁要顯示幾筆</param>
        /// <param name="page">第幾頁開始</param> 
        /// <returns>List<News></returns>
        List<News> GetNewsAll( int? count, int? page);

    }
}
