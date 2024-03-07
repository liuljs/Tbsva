using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IAddTbsvaService
    {
        /// <summary>
        ///  1.新增加入台密總明細項目
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns>201 Created; 資料</returns>
        AddTbsva InsertAddTbsva(HttpRequest httpRequest);

        /// <summary>
        /// 2.取得單筆加入台密總明細項目(後端)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        AddTbsva GetAddTbsva(Guid id);

        /// <summary>
        /// 3.更新加入台密總
        /// </summary>
        /// <param name="httpRequest">輸入id編號</param>
        /// <param name="addTbsva">要修改的內容</param>
        void UpdateAddTbsva(HttpRequest httpRequest, AddTbsva addTbsva);

        /// <summary>
        /// 4.	刪除一筆加入台密總
        /// </summary>
        /// <param name="addTbsva">用id找出資料後給類別，類別id在刪資料</param>
        void DeleteAddTbsva(AddTbsva addTbsva);

        /// <summary>
        /// 5.取得加入台密總明細項目(前後端enabled區別)
        /// </summary>
        /// <param name="namez">會員姓名</param>
        /// <param name="count">一頁筆數</param>
        /// <param name="page">第幾頁</param>
        /// <returns></returns>
        List<AddTbsva> GetAddTbsvasAll(string namez, int? count, int? page);

        /// <summary>
        /// 6.取得加入台密總總筆數
        /// </summary>
        /// <returns>總筆數</returns>
        int GetAddTbsvaTotalCount();

        /// <summary>
        /// 7.更新加入台密總的審核
        /// </summary>
        /// <param name="httpRequest">輸入id編號</param>
        /// <param name="addTbsva">要修改的內容audit 審核0未通過 1通過</param>
        void UpdateAuditOption(HttpRequest httpRequest, AddTbsva addTbsva);
    }
}
