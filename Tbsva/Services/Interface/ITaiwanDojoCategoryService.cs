using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface ITaiwanDojoCategoryService
    {
        /// <summary>
        /// 新增一筆目錄
        /// </summary>
        /// <param name="request"></param>
        /// <returns>201 Created; 目錄資料</returns>
        TaiwanDojoCategory InsertTaiwanDojoCategory(HttpRequest request);

        /// <summary>
        /// 取得一筆目錄資料
        /// </summary>
        /// <param name="categoryId">輸入目錄categoryId編號</param>
        /// <returns>200 Ok取得一筆目錄資料404 NotFound</returns>
        TaiwanDojoCategory GetTaiwanDojoCategory(Guid categoryId);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request">輸入目錄categoryId編號</param>
        /// <param name="taiwanDojoCategory"></param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateTaiwanDojoCategory(HttpRequest request, TaiwanDojoCategory taiwanDojoCategory);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="taiwanDojoCategory"></param>
        /// <returns>成功204 , 失敗404</returns>
        void DeleteTaiwanDojoCategory(TaiwanDojoCategory taiwanDojoCategory);

        /// <summary>
        /// 所有目錄資料
        /// </summary>
        /// <returns></returns>
        List<TaiwanDojoCategory> GetTaiwanDojoCategories();
    }
}
