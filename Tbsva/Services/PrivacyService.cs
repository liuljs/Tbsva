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
    public class PrivacyService : IPrivacyService
    {

        private IDapperHelper m_IDrapperHelp;

        public PrivacyService(IDapperHelper p_IDrapperHelp)
        {
            m_IDrapperHelp = p_IDrapperHelp;
        }
            
        /// <summary>
        /// 取得單筆內容
        /// </summary>
        /// <param name="id">Content 的Id值</param>
        /// <returns></returns>
        public Privacy GetContent(Guid id)
        {
            Privacy privacy_ = new Privacy();
           
            string _sql = @"SELECT * FROM [PRIVACY] WHERE ID = @ID";
            privacy_.Id = id;
            privacy_ = m_IDrapperHelp.QuerySqlFirstOrDefault<Privacy>(_sql, privacy_);
                            
            return privacy_;            
        }

        /// <summary>
        /// 取得所有的內容
        /// </summary>        
        /// <returns></returns>
        public List<Privacy> GetContents()
        {          
            string _sql = @"SELECT * FROM [PRIVACY] ORDER BY CREATION_DATE DESC";

            List<Privacy> ltPrivacy_ = m_IDrapperHelp.QuerySetSql<Privacy>(_sql).ToList();

            return ltPrivacy_;

            /* List<Privacy> ltPrivacy_ = new List<Privacy>();

             using (var _cn = new ConnectionFactory().CreateConnection())
             {
                 string _sql = @"SELECT * FROM [PRIVACY] ORDER BY CREATION_DATE DESC";

                 ltPrivacy_ = _cn.Query<Privacy>(_sql).ToList();
             }

             return ltPrivacy_;  */
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
                    var _sql = @"TRUNCATE TABLE [PRIVACY]";

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
        public Privacy AddContent(HttpRequest request)
        {
            Privacy privacy_ = new Privacy();
            var _formData = request.Form;
            privacy_.Id = Guid.NewGuid();
            privacy_.Content = _formData["content"];
            privacy_.Creation_Date = DateTime.Now;          

            using (var _cn = new ConnectionFactory().CreateConnection())
            {
                _cn.Open();

                using (var transaction = _cn.BeginTransaction())
                {                                 
                    var _sql = @"INSERT INTO [PRIVACY] ([ID],[CONTENT],[CREATION_DATE]) VALUES(@ID, @CONTENT, @CREATION_DATE)";

                    _cn.Execute(_sql, privacy_, transaction: transaction);

                    transaction.Commit();
                }
            }

            return privacy_;
        }

        /// <summary>
        /// 新增1筆內容
        /// </summary>
        /// <param name="request">內容相關資訊</param>
        /// <returns></returns>        
        public Privacy AddContent(Privacy privacy)
        {
            Privacy privacy_ = new Privacy();
            privacy_.Id = Guid.NewGuid();
            privacy_.Content = privacy.Content;
            privacy_.Creation_Date = DateTime.Now;

            using (var _cn = new ConnectionFactory().CreateConnection())
            {
                _cn.Open();

                using (var transaction = _cn.BeginTransaction())
                {
                    var _sql = @"INSERT INTO [PRIVACY] ([ID],[CONTENT],[CREATION_DATE]) VALUES(@ID, @CONTENT, @CREATION_DATE)";

                    _cn.Execute(_sql, privacy_, transaction: transaction);

                    transaction.Commit();
                }
            }

            return privacy_;
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
            string strPathName_ = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Privacies); //圖片存放路徑

            var file = request.Files[0];
            if (file.ContentLength > 0)
            {
                file.SaveAs($"{strPathName_}{strFileName_}");             
            }
            else
            {
                throw new FileNotFoundException();
            }
            
            string strImage_Link_ =Tools.GetInstance().GetImageLink(Tools.GetInstance().Privacies, strFileName_); //回傳圖片的URL && 檔名

            return strImage_Link_;
        }
    }
}