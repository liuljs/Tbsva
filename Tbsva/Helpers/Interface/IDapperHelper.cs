using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopping.Helpers
{
    public interface IDapperHelper
    {
        //https://esofar.gitbooks.io/dapper-tutorial-cn/content/methods/query.html (中文)
        //https://dapper-tutorial.net/query (英文)
        T QuerySqlFirstOrDefault<T>(string sql);

        T QuerySqlFirstOrDefault<T>(string sql, T paramData);

        T QuerySqlFirstOrDefault<T>(string sql, object paramData);

        TReturn QuerySqlFirstOrDefault<T1, TReturn>(string sql, T1 paramData);
   
        IEnumerable<T> QuerySetSql<T>(string sql);

        IEnumerable<T> QuerySetSql<T>(string sql, T paramData);
       
        IEnumerable<TReturn> QuerySetSql<T1, TReturn>(string sql, T1 paramData);

        int ExecuteSql(string sql);

        int ExecuteSql<T>(string sql, T paramData);

        int ExecuteSql<T>(string sql, IEnumerable<T> paramData);
        int QuerySingle<T>(string sql, T paramData);

        /// <summary>
        /// ExecuteScalar 是一種擴展方法，可以從任何 IDbConnection 類型的對象調用。 它執行查詢，並返回查詢返回的結果集中第一行的第一列。 其他列或行將被忽略。
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecuteScalar(string sql);
    }
}
