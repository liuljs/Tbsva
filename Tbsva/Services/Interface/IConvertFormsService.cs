using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IConvertFormsService
    {
        /// <summary>
        ///  1.新增皈戒申請明細項目(前後端)
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns>201 Created; 資料</returns>
        ConvertForms InsertConvertForms(HttpRequest httpRequest);

        /// <summary>
        /// 2.取得單筆皈戒申請明細項目
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        ConvertForms GetConvertForms(Guid id);

        /// <summary>
        /// 3.更新皈戒申請
        /// </summary>
        /// <param name="httpRequest">輸入id編號</param>
        /// <param name="convertForms">要修改的內容</param>
        void UpdateConvertForms(HttpRequest httpRequest, ConvertForms convertForms);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="convertForms">用id找出資料後給類別，類別id在刪資料</param>
        void DeleteConvertForms(ConvertForms convertForms);

        /// <summary>
        /// 5.取得所有皈戒申請
        /// </summary>
        /// <param name="count">一頁幾筆</param>
        /// <param name="page">第幾頁</param>
        /// <returns>List<ConvertForms></returns>
        List<ConvertForms> GetConvertFormsAll(int? count, int? page);

        /// <summary>
        ///  6.取得皈戒申請總筆數
        /// </summary>
        /// <returns>int</returns>
        int GetConvertFormsTotalCount();

        /// <summary>
        /// 7.更新皈戒申請的審核
        /// </summary>
        /// <param name="httpRequest">輸入id編號</param>
        /// <param name="convertForms">要修改的內容</param>
        void UpdateAuditOption(HttpRequest httpRequest, ConvertForms convertForms);
    }
}
