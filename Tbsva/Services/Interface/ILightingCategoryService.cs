using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface ILightingCategoryService
    {
        /// <summary>
        /// 新增一筆目錄
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 目錄資料</returns>
        Lighting_category Insert_Lighting_category(HttpRequest _request);

        /// <summary>
        /// 取得一筆目錄資料
        /// </summary>
        /// <param name="id">輸入文章目錄id編號</param>
        /// <returns>200 Ok取得一筆目錄資料404 NotFound</returns>
        Lighting_category Get_Lighting_category(int id);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request">輸入文章目錄id編號</param>
        /// <param name="_Lighting_category"></param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void Update_Lighting_category(HttpRequest request, Lighting_category _Lighting_category);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="_Lighting_category"></param>
        /// <param name="id">輸入文章目錄id編號</param>
        /// <returns>成功204 , 失敗404</returns>
        void Delete_Lighting_category(Lighting_category _Lighting_category);

        /// <summary>
        /// 取得所有目錄資料
        /// </summary>
        /// <returns>所有目錄資料</returns>
        List<Lighting_category> Get_Lighting_category_ALL();

    }
}
