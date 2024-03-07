using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface IKnowledge2Service
    {
        /// <summary>
        /// 1.新增
        /// </summary>
        /// <param name="_request">用戶端送來的表單資訊</param>
        /// <returns>201 Created; 資料</returns>
        Knowledge_content2 Insert_Knowledge2(HttpRequest _request);

        /// <summary>
        ///2. 取得一筆
        /// </summary>
        /// <param name="knowledgetId">輸入knowledgetId編號</param>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        Knowledge_content2 Get_Knowledge2(Guid knowledgetId);

        /// <summary>
        /// 3.修改
        /// </summary>
        /// <param name="request">輸入id編號</param>
        /// <param name="_Knowledge_content2">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void Update_Knowledge2(HttpRequest request, Knowledge_content2 _Knowledge_content2);

        /// <summary>
        /// 4.刪除一筆
        /// </summary>
        /// <param name="_Knowledge_content2">用knowledgetId找出資料後給類別，類別knowledgetId在刪資料</param>
        /// <returns>成功204 , 失敗404</returns>
        void Delete_Knowledge2(Knowledge_content2 _Knowledge_content2);

        /// <summary>
        /// 5.取得所有
        /// </summary>
        /// <param name="_count">一頁要顯示幾筆</param>
        /// <param name="_page">第幾頁開始</param>
        /// <param name="_category">後台輸入就不要再手動輸入欄位[位階]法師、教授師、講師、助教</param>
        /// <returns></returns>
        List<Knowledge_content2> Get_Knowledge2_ALL(int? _count, int? _page, string _category);

    }
}
