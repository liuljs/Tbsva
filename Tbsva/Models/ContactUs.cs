using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using ecSqlDBManager;
using System.Data.SqlClient;
using System.Web.Http;
using WebShopping.Helpers;

namespace WebShoppingAdmin.Models
{
    public class ContactUs : BaseModel
    {
        //public DataTable Get()
        //{
        //    SqlCommand sql = new SqlCommand(@"
        //    SELECT TOP 1
        //        [ID], [UID], [NAME], [PRINCIPAL], [TEL], [CELL_PHONE], [ADDRESS], [EMAIL], [TERMINAL_ID], [MERCHANT_ID]
        //    FROM [ContactUs]");

        //    DataTable result = db.GetResult(sql);

        //    return result;
        //}

        public void Insert(ContactUsInsertParam param)
        {
            try
            {
                //TODO:聯絡我們，先不用入資料庫，以下保留，將來想入資料庫時，再用下面資料修改
                {
                    //db.OpenConn();

                    //SqlCommand sql = new SqlCommand(@"
                    //SELECT TOP 1
                    //    [ID], [PRINCIPAL], [TEL], [CELL_PHONE], [ADDRESS], [EMAIL]
                    //FROM [ContactUs]");

                    //DataTable oldData = db.GetResult(sql);

                    //sql = new SqlCommand(@"
                    //UPDATE [ContactUs] SET
                    //    [PRINCIPAL] = @PRINCIPAL, [TEL] = @TEL, [CELL_PHONE] = @CELL_PHONE, [ADDRESS] = @ADDRESS, [EMAIL] = @EMAIL
                    //WHERE [ID] = @ID");
                    //sql.Parameters.AddWithValue("@PRINCIPAL", param.principal);
                    //sql.Parameters.AddWithValue("@TEL", param.tel);
                    //sql.Parameters.AddWithValue("@CELL_PHONE", param.cell_phone);
                    //sql.Parameters.AddWithValue("@ADDRESS", param.address);
                    //sql.Parameters.AddWithValue("@EMAIL", param.email);
                    //sql.Parameters.AddWithValue("@ID", param.id);

                    //db.ExecuteNonCommit(sql);

                    //EventLog(db, "ContactUs", DbAction.UPDATE, param.account_id, oldData, param);

                    //db.Commit();
                    //db.CloseConn();
                }

                //寄信
                //發送Email
                SystemFunctions.SendMail(Tools.Company_Name, param.email, Tools.Admin_Mail, new List<string>(), new List<string>(),
                                         $"{Tools.Company_Name}聯絡我們-{param.Name}",
                                         $"姓名 : {param.Name}<br><br>\n" +
                                         $"姓別 : {FormatGender(param.Gender)}<br><br>\n" +
                                         $"電話 : {param.cell_phone}<br><br>\n" +
                                         $"信箱 : {param.email}<br><br>\n" +
                                         $"留言內容 :<br> {param.message}<br>\n"
                                         );

            }
            catch (Exception ex)
            {
                //if (db.SqlConn.State != ConnectionState.Closed)
                //    db.CloseConn();

                throw ex;
            }
        }

        string FormatGender(string gender) {
            switch (gender)
            {
                case "0":return "女性";
                case "1": return "男性";
                default: return "其它";
            }
        }
    }

    public class ContactUsInsertParam
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required, StringLength(30)]
        public string Name { get; set; }

        /// <summary>
        /// 性別        
        /// </summary>
        [Required, StringLength(1)]
        public string Gender { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        [StringLength(15), Phone]
        public string cell_phone { get; set; }

        /// <summary>
        /// 郵件
        /// </summary>
        [Required, StringLength(50), EmailAddress]
        public string email { get; set; }

        /// <summary>
        /// 留言內容
        /// </summary>
        [Required, StringLength(300)]
        public string message { get; set; }

    }
}