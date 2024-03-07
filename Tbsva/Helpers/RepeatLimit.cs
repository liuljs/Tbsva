using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Models;

namespace WebShopping.Helpers
{
    public class RepeatLimit : IRepeatLimit
    {
        IDapperHelper _dapperHelper;

        public RepeatLimit(IDapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        public MessageOnlyOne messageOnlyOne<T>(string tableName, T paramData, string sqls)
        {
            string _sql = $@"SELECT COUNT(FIRST) FROM {tableName} WHERE FIRST = 1 AND ID <> @ID {sqls} ";
            ///TOP 1 1 第一個數字是只顯示1筆，第二個數字是若有資料就是這個值2，若沒有就是0https://www.cnblogs.com/zhcncn/articles/3192863.html
            //string _sql = $@"SELECT TOP 1 2 FROM {tableName} WHERE FIRST = 1 AND ID <> @ID";

            int _listCount = _dapperHelper.QuerySqlFirstOrDefault<T, int>(_sql, paramData);
            //下面這個也可以
            //int _listCount = _dapperHelper.QuerySingle(_sql, paramData);

            if (_listCount > 0)  //只能開啟一個置頂，若查_listCount=1就不能再新增
            {
                return MessageOnlyOne.FirstOnlyOne;  //1 fisrt 置頂已經有1筆
            }
            return MessageOnlyOne.None;  //0 沒有任何置頂 
        }


        public OnlyOneData onlyOneData(string tableName)
        {
            string _sql = $@"SELECT COUNT(id) FROM {tableName} ";
            ///TOP 1 1 第一個數字是只顯示1筆，第二個數字是若有資料就是這個值2，若沒有就是0https://www.cnblogs.com/zhcncn/articles/3192863.html
            //string _sql = $@"SELECT TOP 1 2 FROM {tableName} WHERE FIRST = 1 AND ID <> @ID";
            //throw new ArgumentException("table name not fond");
            int TotalCount = _dapperHelper.ExecuteScalar(_sql);

            // int _listCount = _dapperHelper.QuerySqlFirstOrDefault<T, int>(_sql);
            //下面這個也可以
            //int _listCount = _dapperHelper.QuerySingle(_sql, paramData);

            if (TotalCount > 0)  //只能新增一筆資料
            {
                return OnlyOneData.OnlyOne;   //1 已經有1筆資料
            }
            return OnlyOneData.None;
        }

    }
}