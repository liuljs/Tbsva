using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IActivityCategoryService
    {
        /// <summary>
        /// 新增一筆目錄
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns>201 Created; 目錄資料</returns>
        ActivityCategory InsertActivityCategory(HttpRequest httpRequest);

        /// <summary>
        /// 取得一筆目錄資料
        /// </summary>
        /// <param name="id">輸入目錄id編號</param>
        /// <returns>200 Ok取得一筆目錄資料404 NotFound</returns>
        ActivityCategory GetActivityCategory(Guid id);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="httpRequest">輸入目錄category_id編號</param>
        /// <param name="activityCategory"></param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateActivityCategory(HttpRequest httpRequest, ActivityCategory activityCategory);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="activityCategory"></param>
        /// <returns>成功204 , 失敗404</returns>
        void DeleteActivityCategory(ActivityCategory activityCategory);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <returns>所有目錄資料</returns>
        List<ActivityCategory> GetActivityCategories();
    }
}
