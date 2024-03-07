using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IQAService
    {
        /// <summary>
        /// 1.新增
        /// </summary>
        /// <param name="_request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        QA InsertQA(HttpRequest _request);

        /// <summary>
        /// 2.取得1筆資料
        /// </summary>
        /// <param name="_id">輸入id編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        QA GetQA(int _id);

        /// <summary>
        /// 3.修改
        /// </summary>
        /// <param name="_request">輸入要修改的新內容</param>
        /// <param name="_qa">抓出原本要修改的內容</param>
        void UpdateQA(HttpRequest _request, QA _qa);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="_qa">抓出要刪除的內容</param>
        void DeleteQA(QA _qa);

        /// <summary>
        /// 5.取得所有資料
        /// </summary>
        /// <param name="_count">一頁要顯示幾筆</param>
        /// <param name="_page">第幾頁開始</param>
        /// <returns>回傳多筆資料List<QA></returns>
        List<QA> GetQA(int? _count, int? _page);
    }
}
