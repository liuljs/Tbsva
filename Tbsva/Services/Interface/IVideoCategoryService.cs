using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IVideoCategoryService
    {
        /// <summary>
        /// 新增一筆目錄
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 目錄資料</returns>
        VideoCategory InsertVideoCategory(HttpRequest request);

        /// <summary>
        /// 取得一筆目錄資料
        /// </summary>
        /// <param name="id">輸入目錄id編號</param>
        /// <returns>VideoCategory 200 Ok取得一筆目錄資料404 NotFound</returns>
        VideoCategory GetVideoCategory(int id);

        /// <summary>
        /// 修改目錄
        /// </summary>
        /// <param name="request">輸入目錄id編號</param>
        /// <param name="videoCategory">先取出資料庫資料確定有資料在將request進來的資料覆蓋上去</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateVideoCategory(HttpRequest request, VideoCategory videoCategory);

        /// <summary>
        /// 刪除目錄
        /// </summary>
        /// <param name="videoCategory">先抓出資料庫資料在用id自動對應刪除</param>
        void DeleteVideoCategory(VideoCategory videoCategory);

        /// <summary>
        /// 取得所有目錄資料
        /// </summary>
        /// <returns>所有目錄資料</returns>
        List<VideoCategory> GetVideoCategories();
    }
}
