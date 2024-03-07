using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IIndexSlideshowService
    {
        /// <summary>
        /// 1.新增
        /// </summary>
        /// <param name="httpRequest">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        IndexSlideshow InsertIndexSlideshow(HttpRequest httpRequest);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <param name="id">輸入id編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        IndexSlideshow GetIndexSlideshow(Guid id);

        /// <summary>
        /// 3.修改
        /// </summary>
        /// <param name="httpRequest">輸入id編號</param>
        /// <param name="indexSlideshow">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateIndexSlideshow(HttpRequest httpRequest, IndexSlideshow indexSlideshow);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="indexSlideshow">用id找出資料後給類別，類別id在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void DeleteIndexSlideshow(IndexSlideshow indexSlideshow);

        /// <summary>
        /// 5.顯示所有資料
        /// </summary>
        /// <returns>List<IndexSlideshow></returns>
        List<IndexSlideshow> GetIndexSlideshows();
    }
}
