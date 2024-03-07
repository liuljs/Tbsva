using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShoppingAdmin.Models
{
    public class ApiResult
    {
        public string Result { protected set; get; }

        public object Content { protected set; get; }

        public ApiResult()
        {
            Result = "OK";
            Content = null;
        }

        public ApiResult(object content)
        {
            Result = "OK";
            Content = content;
        }
    }

    public class ErrApiResult : ApiResult
    {
        public ErrApiResult(string errMsg)
        {
            Result = "NG";
            Content = errMsg;
        }
    }
}