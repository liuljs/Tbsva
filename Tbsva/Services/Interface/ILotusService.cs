using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface ILotusService
    {
        /// <summary>
        /// 1.新增
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        Lotus InsertLotus(HttpRequest request);
        /// <summary>
        /// 01新增一個LotusImage內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="request"></param>
        /// <returns>201, LotusImage</returns>
        LotusImage AddUploadImage(HttpRequest request);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <param name="Id">輸入id編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        Lotus GetLotus(Guid Id);

        /// <summary>
        ///  3.修改
        /// </summary>
        /// <param name="request">輸入id編號</param>
        /// <param name="lotus">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateLotus(HttpRequest request, Lotus lotus);
        /// <summary>
        /// 02新增一筆LotusImage內容圖片,有關聯ID, 適用於在修改時使用
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="lotus_Id">活動管理Id</param>
        /// <returns>201, LotusImage</returns>
        LotusImage AddUploadImage(HttpRequest request, Guid lotus_Id);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="lotus">用id找出資料後給類別，類別id在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void DeleteLotus(Lotus lotus);

        /// <summary>
        /// 5.顯示所有資料
        /// </summary>
        /// <param name="title">標題</param>
        /// <param name="number">冊號</param>
        /// <param name="count">一頁要顯示幾筆</param>
        /// <param name="page">第幾頁開始</param> 
        /// <returns>List<Lotus></returns>
        List<Lotus> GetLotusAll(string title, string number, int? count, int? page);

        /// <summary>
        /// 6.取得總筆數
        /// </summary>
        /// <param name="title">標題</param>
        /// <param name="number">冊號</param>
        /// <returns>總筆數</returns>
        int GetLotusTotalCount(string title, string number);
    }
}
