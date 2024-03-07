using WebShopping.Connection;
using WebShopping.Helpers;
using WebShopping.Models;
using WebShoppingAdmin.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace WebShopping.Services
{
    public class TermsService : ITermsService
    {

        private IDapperHelper m_IDrapperHelp;

        public TermsService(IDapperHelper p_IDrapperHelp)
        {
            m_IDrapperHelp = p_IDrapperHelp;
        }
            
        /// <summary>
        /// 取得單筆內容
        /// </summary>
        /// <param name="id">Content 的Id值</param>
        /// <returns></returns>
        public Terms GetContent(Guid id)
        {
            Terms terms_ = new Terms();
           
            string _sql = @"SELECT * FROM [TERMS] WHERE ID = @ID";
            terms_.Id = id;
            terms_ = m_IDrapperHelp.QuerySqlFirstOrDefault<Terms>(_sql, terms_);
                            
            return terms_;            
        }

        /// <summary>
        /// 取得所有的內容
        /// </summary>        
        /// <returns></returns>
        public List<Terms> GetContents()
        {          
            string _sql = @"SELECT * FROM [TERMS] ORDER BY CREATION_DATE DESC";

            List<Terms> ltTerms_ = m_IDrapperHelp.QuerySetSql<Terms>(_sql).ToList();

            return ltTerms_;

            /* List<Terms> ltTerms_ = new List<Terms>();

             using (var _cn = new ConnectionFactory().CreateConnection())
             {
                 string _sql = @"SELECT * FROM [TERMS] ORDER BY CREATION_DATE DESC";

                 ltTerms_ = _cn.Query<Terms>(_sql).ToList();
             }

             return ltTerms_;  */
        }

        /// <summary>
        /// 清空資料表所有內容
        /// </summary>
        public bool DeleteAllContents()
        {
            using (var _cn = new ConnectionFactory().CreateConnection())
            {
                _cn.Open();

                using (var transaction = _cn.BeginTransaction())
                {
                    var _sql = @"TRUNCATE TABLE [TERMS]";

                    _cn.Execute(_sql, null, transaction: transaction);

                    transaction.Commit();
                }
            }

            return true;
        }

        /// <summary>
        /// 新增1筆內容
        /// </summary>
        /// <param name="request">內容相關資訊</param>
        /// <returns></returns>        
        public Terms AddContent(HttpRequest request)
        {
            Terms terms_ = new Terms();
            var _formData = request.Form;
            terms_.Id = Guid.NewGuid();
            terms_.Content = _formData["content"];
            terms_.Creation_Date = DateTime.Now;          

            using (var _cn = new ConnectionFactory().CreateConnection())
            {
                _cn.Open();

                using (var transaction = _cn.BeginTransaction())
                {                                 
                    var _sql = @"INSERT INTO [TERMS] ([ID],[CONTENT],[CREATION_DATE]) VALUES(@ID, @CONTENT, @CREATION_DATE)";

                    _cn.Execute(_sql, terms_, transaction: transaction);

                    transaction.Commit();
                }
            }

            return terms_;
        }

        /// <summary>
        /// 新增1筆內容
        /// </summary>
        /// <param name="request">內容相關資訊</param>
        /// <returns></returns>        
        public Terms AddContent(Terms terms)
        {
            Terms terms_ = new Terms();
            terms_.Id = Guid.NewGuid();
            terms_.Content = terms.Content;
            terms_.Creation_Date = DateTime.Now;

            using (var _cn = new ConnectionFactory().CreateConnection())
            {
                _cn.Open();

                using (var transaction = _cn.BeginTransaction())
                {
                    var _sql = @"INSERT INTO [TERMS] ([ID],[CONTENT],[CREATION_DATE]) VALUES(@ID, @CONTENT, @CREATION_DATE)";

                    _cn.Execute(_sql, terms_, transaction: transaction);

                    transaction.Commit();
                }
            }

            return terms_;
        }

        /// <summary>
        /// 新增圖片
        /// </summary>
        /// <param name="request">內容相關資訊</param>
        /// <returns></returns>
        public string AddImage(HttpRequest request)
        {            
            var formData_ = request.Form;

            string strFileName_ = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_fff")}.png";
            string strPathName_ = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Terms); //圖片存放路徑

            var file = request.Files[0];
            if (file.ContentLength > 0)
            {
                file.SaveAs($"{strPathName_}{strFileName_}");             
            }
            else
            {
                throw new FileNotFoundException();
            }
            
            string strImage_Link_ =Tools.GetInstance().GetImageLink(Tools.GetInstance().Terms, strFileName_); //回傳圖片的URL && 檔名

            return strImage_Link_;
        }
    }
}