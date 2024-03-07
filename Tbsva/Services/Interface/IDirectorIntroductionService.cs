using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IDirectorIntroductionService
    {
        /// <summary>
        /// 1.新增
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        DirectorIntroduction InsertDirectorIntroduction(HttpRequest request);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <param name="directorIntroductionId">{directorIntroductionId}</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        DirectorIntroduction GetDirectorIntroduction(Guid directorIntroductionId);

        /// <summary>
        /// 3.修改
        /// </summary>
        /// <param name="request">輸入{directorIntroductionId}編號</param>
        /// <param name="directorIntroduction">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateDirectorIntroduction(HttpRequest request, DirectorIntroduction directorIntroduction);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="directorIntroduction">用directorIntroduction找出資料後給類別，類別directorIntroduction在刪資料</param>
        void DeleteDirectorIntroduction(DirectorIntroduction directorIntroduction);

        /// <summary>
        /// 5.顯示所有資料
        /// </summary>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <returns>List<DirectorIntroduction></returns>
        List<DirectorIntroduction> GetDirectorIntroductions(int? count, int? page);

        /// <summary>
        /// 01新增一個DirectorIntroductionImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="request"></param>
        /// <returns>201, DirectorIntroductionImage</returns>
        DirectorIntroductionImage AddUploadImage(HttpRequest request);

        /// <summary>
        /// 02新增一筆DirectorIntroductionImage內容圖片,有關聯ID, 適用於在修改時使用
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="directorIntroductionId">關聯directorIntroductionId</param>
        /// <returns>201, DirectorIntroductionImage</returns>
        DirectorIntroductionImage AddUploadImage(HttpRequest request, Guid directorIntroductionId);
    }
}
