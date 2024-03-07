using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Helpers
{
    public interface IRepeatLimit
    {
        /// <summary>
        /// 只能有一筆置頂
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">資料庫資料表</param>
        /// <param name="paramData">類別</param>
        /// <param name="sqls">另加sql</param>
        /// <returns>MessageOnlyOne</returns>
        MessageOnlyOne messageOnlyOne<T>(string tableName, T paramData, string sqls);


        /// <summary>
        /// 資料庫只能有一筆資料
        /// </summary>
        /// <param name="tableName">要搜尋的資料表名</param>
        /// <returns></returns>
        OnlyOneData onlyOneData(string tableName);
    }
}
