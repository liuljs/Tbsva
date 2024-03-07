using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebShopping.Services
{
    public interface IAboutMeService
    {
        /// <summary>
        /// 取得單筆內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AboutMe GetContent(Guid id);
        /// <summary>
        /// 取得所有的內容
        /// </summary>
        /// <returns></returns>
        List<AboutMe> GetContents();
        /// <summary>
        /// 清空所有資料表內容
        /// </summary>
        /// <returns></returns>
        bool DeleteAllContents();
        /// <summary>
        /// 新增1筆內容(FormData)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        AboutMe AddContent(HttpRequest request);
        /// <summary>
        /// 新增1筆內容(Json)
        /// </summary>
        /// <param name="aboutme"></param>
        /// <returns></returns>
        AboutMe AddContent(AboutMe aboutme);
        /// <summary>
        /// 新增1筆圖片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        string AddImage(HttpRequest request);   

    }
}
