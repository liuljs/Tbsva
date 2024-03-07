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
    public class AboutMeService : IAboutMeService
    {

        private IDapperHelper m_IDrapperHelp;

        public AboutMeService(IDapperHelper p_IDrapperHelp)
        {
            m_IDrapperHelp = p_IDrapperHelp;
        }
            
        /// <summary>
        /// 取得單筆內容
        /// </summary>
        /// <param name="id">Content 的Id值</param>
        /// <returns></returns>
        public AboutMe GetContent(Guid id)
        {
            AboutMe aboutMe_ = new AboutMe();
           
            string _sql = @"SELECT * FROM [ABOUT_ME] WHERE ID = @ID";
            aboutMe_.Id = id;
            aboutMe_ = m_IDrapperHelp.QuerySqlFirstOrDefault<AboutMe>(_sql, aboutMe_);
                            
            return aboutMe_;            
        }

        /// <summary>
        /// 取得所有的內容
        /// </summary>        
        /// <returns></returns>
        public List<AboutMe> GetContents()
        {          
            string _sql = @"SELECT * FROM [ABOUT_ME] ORDER BY CREATION_DATE DESC";

            List<AboutMe> ltAboutMe_ = m_IDrapperHelp.QuerySetSql<AboutMe>(_sql).ToList();

            return ltAboutMe_;

            /* List<AboutMe> ltAboutMe_ = new List<AboutMe>();

             using (var _cn = new ConnectionFactory().CreateConnection())
             {
                 string _sql = @"SELECT * FROM [ABOUT_ME] ORDER BY CREATION_DATE DESC";

                 ltAboutMe_ = _cn.Query<AboutMe>(_sql).ToList();
             }

             return ltAboutMe_;  */
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
                    var _sql = @"TRUNCATE TABLE [ABOUT_ME]";

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
        public AboutMe AddContent(HttpRequest request)
        {
            AboutMe aboutMe_ = new AboutMe();
            var _formData = request.Form;
            aboutMe_.Id = Guid.NewGuid();
            aboutMe_.Content = _formData["content"];
            aboutMe_.Creation_Date = DateTime.Now;          

            using (var _cn = new ConnectionFactory().CreateConnection())
            {
                _cn.Open();

                using (var transaction = _cn.BeginTransaction())
                {                                 
                    var _sql = @"INSERT INTO [ABOUT_ME] ([ID],[CONTENT],[CREATION_DATE]) VALUES(@ID, @CONTENT, @CREATION_DATE)";

                    _cn.Execute(_sql, aboutMe_, transaction: transaction);

                    transaction.Commit();
                }
            }

            return aboutMe_;
        }

        /// <summary>
        /// 新增1筆內容
        /// </summary>
        /// <param name="request">內容相關資訊</param>
        /// <returns></returns>        
        public AboutMe AddContent(AboutMe aboutme)
        {
            AboutMe aboutMe_ = new AboutMe();
            aboutMe_.Id = Guid.NewGuid();
            aboutMe_.Content = aboutme.Content;
            aboutMe_.Creation_Date = DateTime.Now;

            using (var _cn = new ConnectionFactory().CreateConnection())
            {
                _cn.Open();

                using (var transaction = _cn.BeginTransaction())
                {
                    var _sql = @"INSERT INTO [ABOUT_ME] ([ID],[CONTENT],[CREATION_DATE]) VALUES(@ID, @CONTENT, @CREATION_DATE)";

                    _cn.Execute(_sql, aboutMe_, transaction: transaction);

                    transaction.Commit();
                }
            }

            return aboutMe_;
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
            string strPathName_ = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Abouts); //圖片存放路徑

            var file = request.Files[0];
            if (file.ContentLength > 0)
            {
                file.SaveAs($"{strPathName_}{strFileName_}");             
            }
            else
            {
                throw new FileNotFoundException();
            }
            
            string strImage_Link_ =Tools.GetInstance().GetImageLink(Tools.GetInstance().Abouts, strFileName_); //回傳圖片的URL && 檔名

            return strImage_Link_;
        }

    }
}