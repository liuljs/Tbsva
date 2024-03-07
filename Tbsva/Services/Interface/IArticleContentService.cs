using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IArticleContentService
    {
        /// <summary>
        /// 1.新增一筆文章
        /// </summary>
        /// <param name="_request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 文章資料</returns>
        article_content Insert_article_content(HttpRequest _request);
        /// <summary>
        /// 新增一個article_image內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="_request">內容區圖片要求資訊</param>
        /// <returns>201, article_image</returns>
        article_image AddUploadImage(HttpRequest _request);


        /// <summary>
        ///2. 取得一筆文章
        /// </summary>
        /// <param name="id">輸入文章id編號</param>
        /// <returns>200 Ok取得一筆文章資料,404 NotFound</returns>
        article_content Get_article_content(Guid id);


        /// <summary>
        /// 3.修改文章
        /// </summary>
        /// <param name="request">輸入文章id編號</param>
        /// <param name="_article_content"></param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void Update_article_content(HttpRequest request, article_content _article_content);
        /// <summary>
        /// 新增一筆article_image內容圖片,有關聯文章ID, 適用於在修改文章時使用
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="article_content_Id"></param>
        /// <returns></returns>
        article_image AddUpdateUploadImage(HttpRequest request, Guid article_content_Id);


        /// <summary>
        /// 4.刪除一筆文章
        /// </summary>
        /// <param name="id">輸入文章id編號</param>
        /// <param name="_article_content">用id找出資料後給類別，類別id在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void Delete_article_content(article_content _article_content);

        /// <summary>
        /// 5.取得所有文章
        /// </summary>
        /// <param name="count">筆數</param>
        /// <param name="page">頁</param>
        /// <returns></returns>
        List<article_content> Get_article_content_ALL(int? _article_category_id, int? _count, int? _page);

    }
}
