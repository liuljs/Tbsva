using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IVideo2Service
    {
        /// <summary>
        /// 1.新增活動影片
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        Video2 InsertVideo(HttpRequest request);
        /// <summary>
        /// 01新增一個VideoImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="request"></param>
        /// <returns>201, VideoImage</returns>
        VideoImage2 AddUploadImage(HttpRequest request);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <param name="video_id">輸入video_id編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        Video2 GetVideo(Guid video_id);

        /// <summary>
        ///  3.修改
        /// </summary>
        /// <param name="request">輸入id編號</param>
        /// <param name="video2">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateVideo(HttpRequest request, Video2 video2);
        /// <summary>
        /// 02新增一筆VideoImage內容圖片,有關聯ID, 適用於在修改時使用
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="Video_Id">活動管理Id</param>
        /// <returns>201, VideoImage</returns>
        VideoImage2 AddUploadImage(HttpRequest request, Guid Video_Id);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="video2">用id找出資料後給類別，類別id在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void DeleteVideo(Video2 video2);

        /// <summary>
        /// 5.顯示活動管理的資料
        /// </summary>
        /// <param name="first">非必填: 置頂狀態（0 顯示所有資料；1只顯示置頂1筆）</param>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <returns>List<Video></returns>
        List<Video2> GetVideoAll(int? first, int? count, int? page);
    }
}
