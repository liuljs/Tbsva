using ecSqlDBManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace WebShoppingAdmin.Models
{
    public class BaseModel : IDisposable
    {
        protected string DbConnString { get { return ConfigurationManager.AppSettings["DbConnString"]; } }
        protected DataBase db;

        public BaseModel()
        {
            db = new DataBase(DbConnString);
        }

        public void Dispose()
        {
            db.Dispose();
        }

        /// <summary>
        /// 事件log
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <param name="action"></param>
        /// <param name="accountId"></param>
        /// <param name="oldData"></param>
        /// <param name="newData"></param>
        protected void EventLog(DataBase db, string tableName, DbAction action, Guid accountId, object oldData, object newData)
        {
            SqlCommand sql = new SqlCommand(
            @"INSERT INTO [EVENT_LOG] ([TABLE_NAME], [ACTION], [ACCOUNT_ID], [OLD_DATA], [NEW_DATA]) 
                    VALUES (@TABLE_NAME, @ACTION, @ACCOUNT_ID, @OLD_DATA, @NEW_DATA)");
            sql.Parameters.AddWithValue("@TABLE_NAME", tableName);
            sql.Parameters.AddWithValue("@ACTION", action.ToString());
            sql.Parameters.AddWithValue("@ACCOUNT_ID", accountId);
            sql.Parameters.AddWithValue("@OLD_DATA", JsonConvert.SerializeObject(oldData));
            sql.Parameters.AddWithValue("@NEW_DATA", JsonConvert.SerializeObject(newData));

            db.ExecuteNonCommit(sql);
        }

        public enum DbAction
        {
            INSERT,
            DELETE,
            UPDATE
        }
    }
}