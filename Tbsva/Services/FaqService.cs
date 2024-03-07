using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using WebShopping.Connection;
using System.IO;
using WebShopping.Helpers;
using System.Configuration;
using Newtonsoft.Json;
using WebShopping.Dtos;

namespace WebShopping.Services
{
    public class FaqService : IFaqService
    {
        private IImageFileHelper m_ImageFileHelper;

        private IDapperHelper m_DapperHelper;

        //private string m_imageFolder = @"\Admin\backStage\img\faq\";

        public FaqService(IImageFileHelper imageFileHelper, IDapperHelper dapperHelper)
        {
            m_ImageFileHelper = imageFileHelper;
            m_DapperHelper = dapperHelper;
        }

        /// <summary>
        /// 取得單筆Faq資料
        /// </summary>
        /// <param name="id">FaqID</param>
        /// <returns>單筆Faq資料</returns>
        public Faq GetFaqData(int id)
        {
            Faq _faq = new Faq();
            _faq.Id = id;

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND Enabled=1 ";

            string _sql = $"SELECT * FROM [Faq] WHERE [ID]=@ID {adminQuery} ORDER BY [Sort]";

            _faq = m_DapperHelper.QuerySqlFirstOrDefault(_sql, _faq);

            return _faq;
        }

        /// <summary>
        /// 取得全部Faq資料
        /// </summary>
        /// <returns>全部Faq資料</returns>
        public List<Faq> GetFaqSetData()
        {
            string adminQuery = Auth.Role.IsAdmin ? "" : " WHERE Enabled=1 ";

            string _sql = $"SELECT * FROM [Faq] {adminQuery} ORDER BY Sort";

            List<Faq> _faq = m_DapperHelper.QuerySetSql<Faq>(_sql).ToList();

            return _faq;
        }

        /// <summary>
        /// 新增Faq
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <returns>新增Faq的資料</returns>
        public Faq InsertFaq(HttpRequest request)
        {
            Faq _faq = SetInsertNewData(request);

            string _sql = @"INSERT INTO [Faq] (Question, Asked, Sort, Enabled) VALUES (@Question,@Asked, @Sort, @Enabled)";

            m_DapperHelper.ExecuteSql(_sql, _faq);

            Faq faq = m_DapperHelper.QuerySqlFirstOrDefault<Faq>("Select top 1 id from [Faq] Order by id desc");
            _faq.Id = faq.Id;

            return _faq;
        }

         /// <summary>
        /// 設置新增Faq的資料
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <returns>新增Faq的資料</returns>
        private Faq SetInsertNewData(HttpRequest request)
        {
            Faq _faq = new Faq();
            //_faq.Id = Guid.NewGuid(); //訊息的 id(流水號)
            _faq.Question = request.Form["question"]; //常見問題
            _faq.Asked = request.Form["asked"]; //問題回答
            _faq.Sort =Convert.ToInt32(request.Form["sort"]); //排序
            _faq.Enabled = Convert.ToByte(request.Form["enabled"]); //是否啟用(0/1)

            return _faq;
        }

        /// <summary>
        /// 更新單筆Faq資料
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="faq">更新的資料類別</param>
        public void UpdateFaq(HttpRequest request, Faq faq)
        {
            faq.Question = request.Form["question"];
            faq.Asked = request.Form["asked"];
            faq.Sort = Convert.ToInt32(request.Form["sort"]);
            faq.Enabled = Convert.ToByte(request.Form["enabled"]);

            string _sql = $@"UPDATE [Faq] 
                             SET [Question]=@Question,[Asked]=@Asked,[Sort]=@Sort,[Enabled]=@Enabled,[updated_date]=getdate() 
                             WHERE [ID] = @ID";

            m_DapperHelper.ExecuteSql(_sql, faq);
        }

        /// <summary>
        /// 刪除一筆Faq
        /// </summary>
        /// <param name="id">FaqID</param>
        public void DeleteFaq(Faq faq)
        {
            string _sql = @"DELETE FROM [Faq] WHERE ID = @ID";

            m_DapperHelper.ExecuteSql(_sql, faq);
        }
    }
}