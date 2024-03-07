using AutoMapper;
using WebShopping.Dtos;
using WebShopping.Helpers;
using WebShopping.Models;
using WebShopping.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using WebShopping.Auth;

namespace WebShopping.Controllers
{
    /// <summary>
    /// Logger處理
    /// 開發階段用來讀取及刪除當天的LOG
    /// </summary>
    public class LoggerController : ApiController
    {
        public LoggerController()
        {
        }

        /// <summary>
        /// 接收遠端傳來的資料,並寫入LOG
        /// </summary>
        /// <returns></returns>
        [Route("Logger/GetFormData")]
        public IHttpActionResult GetFormData()
        {
            HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
            string data = Helpers.SystemFunctions.GetFormData(_request);
            Helpers.SystemFunctions.WriteLogFile($"\n\n{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}收到Form-data\n{data}");
            return Ok(data);
        }

        [HttpGet]
        //[Route("Logger")]
        public IHttpActionResult Get()
        {
            if (DateTime.Now > new DateTime(2021, 8, 1)) return Ok("此功能已過期!");
            var str = Helpers.SystemFunctions.ReadLogFile();
            return Ok(str);
        }

        [HttpDelete]
        public IHttpActionResult Delete()
        {
            if (DateTime.Now > new DateTime(2021, 8, 1)) return Ok("此功能已過期!");
            var str = Helpers.SystemFunctions.ReadLogFile();
            Helpers.SystemFunctions.DeleteLogFile();
            return Ok(str);
        }

    }
}
