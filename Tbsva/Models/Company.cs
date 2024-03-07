using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;
using ecSqlDBManager;
using System.Data.SqlClient;
using System.Web.Http;

namespace WebShoppingAdmin.Models
{
    public class Company : BaseModel
    {
        public DataTable Get()
        {
            SqlCommand sql = new SqlCommand(@"
            SELECT TOP 1
                [ID], [UID], [NAME], [PRINCIPAL], [TEL], [CELL_PHONE], [ADDRESS], [EMAIL], [TERMINAL_ID], [MERCHANT_ID]
            FROM [COMPANY]");

            DataTable result = db.GetResult(sql);

            return result;
        }

        public void Update(CompanyUpdateParam param)
        {
            try
            {
                db.OpenConn();

                SqlCommand sql = new SqlCommand(@"
                SELECT TOP 1
                    [ID], [PRINCIPAL], [TEL], [CELL_PHONE], [ADDRESS], [EMAIL]
                FROM [COMPANY]");

                DataTable oldData = db.GetResult(sql);

                sql = new SqlCommand(@"
                UPDATE [COMPANY] SET
                    [PRINCIPAL] = @PRINCIPAL, [TEL] = @TEL, [CELL_PHONE] = @CELL_PHONE, [ADDRESS] = @ADDRESS, [EMAIL] = @EMAIL
                WHERE [ID] = @ID");
                sql.Parameters.AddWithValue("@PRINCIPAL", param.principal);
                sql.Parameters.AddWithValue("@TEL", param.tel);
                sql.Parameters.AddWithValue("@CELL_PHONE", param.cell_phone);
                sql.Parameters.AddWithValue("@ADDRESS", param.address);
                sql.Parameters.AddWithValue("@EMAIL", param.email);
                sql.Parameters.AddWithValue("@ID", param.id);

                db.ExecuteNonCommit(sql);

                EventLog(db, "company", DbAction.UPDATE, param.account_id, oldData, param);

                db.Commit();
                db.CloseConn();
            }
            catch (Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }
        }
    }

    public class CompanyUpdateParam
    {
        /// <summary>
        /// 管理帳號ID
        /// </summary>
        [Required]
        public Guid account_id { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        [Required]
        public Guid id { get; set; }
        /// <summary>
        /// 負責人
        /// </summary>
        [Required, StringLength(30)]
        public string principal { get; set; }
        /// <summary>
        /// 公司桌機
        /// </summary>
        [StringLength(15), Phone]
        public string tel { get; set; }
        /// <summary>
        /// 手機
        /// </summary>
        [StringLength(15), Phone]
        public string cell_phone { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(80)]
        public string address { get; set; }
        /// <summary>
        /// 郵件
        /// </summary>
        [StringLength(50), EmailAddress]
        public string email { get; set; }
    }
}