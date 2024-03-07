using AutoMapper;
using WebShopping.Dtos;
using WebShopping.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 金流付款API
    /// </summary>
    public class PaymentController : ApiController
    {
        private IPaymentService m_PaymentService;
        private IMapper m_Mapper;

        public PaymentController(IPaymentService service, IMapper mapper)
        {
            m_PaymentService = service;
            m_Mapper = mapper;
        }

        /// <summary>
        /// 處理ReceiveURL
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        //[HttpPost]
        [Route("Payment/Receive")]
        public IHttpActionResult Receive()
        {
            HttpRequest _request = HttpContext.Current.Request;
            //string formdata = Helpers.SystemFunctions.GetFormData(_request);
            //Helpers.SystemFunctions.WriteLogFile($"\n\n{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}收到Form-data\n{formdata}");
            string json = Helpers.SystemFunctions.GetJsonData(_request);
            PaymentReceiveDto dto = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentReceiveDto>(json);
            dto.json = json;
            //檢查ValidateToken的內容是否正確
            bool check = false;
            {
                string ValidateToken = HttpUtility.UrlDecode(_request.Form["ValidateToken"], Encoding.UTF8);
                string HashData = _request.Form["OrderNo"]
                    + _request.Form["Amount"]
                    + _request.Form["AcquirerOrderNo"]
                    + _request.Form["Code"];
                string key = "validateKey";//ValidateKey
                byte[] bytes = Encoding.UTF8.GetBytes(HashData);
                string result = "";
                byte[] hashBytes;

                if (String.IsNullOrEmpty(key))
                {
                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    hashBytes = sha1.ComputeHash(bytes);
                }
                else
                {
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                    HMACSHA1 sha2 = new HMACSHA1(keyBytes);
                    hashBytes = sha2.ComputeHash(bytes);
                }
                foreach (byte b in hashBytes)
                {
                    result += b.ToString("x2");
                }

                //檢查內容合法性
                check = (result == ValidateToken);
                Helpers.SystemFunctions.WriteLogFile($"Payment/Receive:check=({check}) " +
                    $"[{_request.Form["OrderNo"]}][{_request.Form["Amount"]}][{_request.Form["AcquirerOrderNo"]}][{_request.Form["Code"]}]" +
                    $";json=({json})" +
                    $"; result=({result}) ; ValidateToken=({ValidateToken})");
            }

            if ( check)//true ||
            {
                //轉型完成，還要入資料庫
                int i = m_PaymentService.UpdateReceive(dto);
                string url = $"/flow_payments.html?id={dto.OrderNo}";
                HttpContext.Current.Response.Redirect(url);
                return Ok(dto);
            }
            else
            {
                return Ok();//return NotFound();
            }
            //return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// 處理ReturnURL
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Payment/Return")]
        public IHttpActionResult Return()
        {
            HttpRequest _request = HttpContext.Current.Request;
            //string formdata = Helpers.SystemFunctions.GetFormData(_request);
            //Helpers.SystemFunctions.WriteLogFile($"\n\n{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}收到Form-data\n{formdata}");
            string json = Helpers.SystemFunctions.GetJsonData(_request);
            PaymentReturnDto dto = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentReturnDto>(json);
            dto.json = json;

            //檢查ValidateToken的內容是否正確
            bool check = false;
            {
                string ValidateToken = HttpUtility.UrlDecode(_request.Form["ValidateToken"], Encoding.UTF8);
                string HashData = _request.Form["OrderNo"]
                    + _request.Form["Amount"]
                    + _request.Form["AcquirerOrderNo"]
                    + _request.Form["Code"];
                string key = ("validateKey");//ValidateKey
                byte[] bytes = Encoding.UTF8.GetBytes(HashData);
                string result = "";
                byte[] hashBytes;

                if (String.IsNullOrEmpty(key))
                {
                    SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
                    hashBytes = sha1.ComputeHash(bytes);
                }
                else
                {
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                    HMACSHA1 sha2 = new HMACSHA1(keyBytes);
                    hashBytes = sha2.ComputeHash(bytes);
                }
                foreach (byte b in hashBytes)
                {
                    result += b.ToString("x2");
                }

                //檢查內容合法性
                check = (result == ValidateToken);
                Helpers.SystemFunctions.WriteLogFile($"Payment/Return:check=({check}) " +
                                    $"[{_request.Form["OrderNo"]}][{_request.Form["Amount"]}][{_request.Form["AcquirerOrderNo"]}][{_request.Form["Code"]}]" +
                                    $";json=({json})" +
                                    $"; result=({result}) ; ValidateToken=({ValidateToken})");
            }

            if ( check)//true ||
            {
                //轉型完成，還要入資料庫
                int i = m_PaymentService.UpdateReturn(dto);
                string url = $"/member_order_list.html?id={dto.OrderNo}";
                HttpContext.Current.Response.Redirect(url);
                return Ok(dto);
            }
            else {
                return Ok();//return NotFound();
            }
            //return StatusCode(HttpStatusCode.NoContent);

        }

    }
}
