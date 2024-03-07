using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShopping.Dtos
{
    //測試,可參考 EnumSet.cs
    public enum Message
    {
        successToGetContent,     //登入成功
        errorToBadRequest,     //您的帳號或密碼欄位格式輸入錯誤！
        errorToNotFilledAccount,          //的帳號欄位尚未填寫！
        errorToNotFilledPassword,          //您的密碼欄位尚未填寫！
        errorToForbidden         //此帳號已被停用！
    }

    public enum Detail
    {
        登入成功,     //登入成功
        您的帳號或密碼欄位格式輸入錯誤,     //您的帳號或密碼欄位格式輸入錯誤！
        您的帳號欄位尚未填寫,          //的帳號欄位尚未填寫！
        您的密碼欄位尚未填寫,          //您的密碼欄位尚未填寫！
        此帳號已被停用         //此帳號已被停用！
    }

    public class apiNewResultSuccess
    {
        public object code { set; get; }
        public string message { set; get; }
        public string detail { set; get; }
        public object data { set; get; }
        public object timestamp { set; get; }

        public apiNewResultSuccess(object code, string message, string detail, object data, object timestamp)
        {
            this.code = code;
            this.message = message;
            this.detail = detail;
            this.data = data;
            this.timestamp = timestamp;
        }

        //public apiNewResultSuccess()
        //{
        //}

    }
}