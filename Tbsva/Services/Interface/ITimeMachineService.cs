using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface ITimeMachineService
    {
        /// <summary>
        /// 1.新增時光走廊
        /// </summary>
        /// <param name="request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        TimeMachine InsertTimeMachine(HttpRequest request);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <param name="timeMachineId">{timeMachineId}</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        TimeMachine GetTimeMachine(Guid timeMachineId);

        /// <summary>
        /// 3.修改
        /// </summary>
        /// <param name="request">輸入{timeMachineId}編號</param>
        /// <param name="timeMachine">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateTimeMachine(HttpRequest request, TimeMachine timeMachine);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="timeMachine">用timeMachine找出資料後給類別，類別timeMachine在刪資料</param>
        void DeleteTimeMachine(TimeMachine timeMachine);

        /// <summary>
        /// 5.顯示所有資料
        /// </summary>
        /// <param name="count">一頁要顯示幾筆</param>
        /// <param name="page">第幾頁開始</param> 
        /// <returns>List<TimeMachine></returns>
        List<TimeMachine> GetTimeMachines(int? count, int? page);
    }
}
