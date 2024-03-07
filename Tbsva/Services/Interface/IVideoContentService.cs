using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IVideoContentService
    {
        /// <summary>
        /// 1.新增
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        VideoContent InsertVideoContent(HttpRequest request);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <param name="id">輸入id編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        VideoContent GetVideoContent(Guid id);

        /// <summary>
        /// 3.修改
        /// </summary>
        /// <param name="request">輸入id編號</param>
        /// <param name="videoContent">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateVideoContent(HttpRequest request, VideoContent videoContent);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="videoContent">用id找出資料後給類別，類別id在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void DeleteVideoContent(VideoContent videoContent);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VideoCategory_id">所在目錄下</param>
        /// <param name="count">一頁要顯示幾筆</param>
        /// <param name="page">第幾頁開始</param>
        /// <param name="keyword">搜尋關鍵字(標題,敘述內容)</param>
        /// <returns>List<VideoContent></returns>
        List<VideoContent> GetVideoContents(int? VideoCategory_id, int? count, int? page, string keyword);
    }
}
