using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebShopping.Connection
{
    public class ConnectionFactory
    {

        public enum DbAction
        {
            INSERT,
            DELETE,
            UPDATE
        }

        public IDbConnection CreateConnection(string name = "default")
        {
            switch (name)
            {
                case "default":
                    {
                        var ConnectionString = ConfigurationManager.AppSettings["DbConnString"];

                        return new SqlConnection(ConnectionString);
                    }
                default:
                    {
                        throw new Exception("name 不存在。");
                    }
            }
        }
    }
}