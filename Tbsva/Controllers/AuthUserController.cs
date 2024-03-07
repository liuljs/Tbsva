using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebShopping.Auth;
using WebShopping.Dtos;
using WebShopping.Services;
using WebShoppingAdmin.Models;
using Newtonsoft.Json;

namespace WebShoppingAdmin.Controllers
{
    /// <summary>
    /// 前台登入
    /// </summary>
    [CustomAuthorize(Role.User)]   
    [RoutePrefix("AuthUser")]
    public class AuthUserController : ApiController
    {
        //private IMemberService m_IMemberService;

        //public AuthUserController(IMemberService IMemberService)
        //{
        //    m_IMemberService = IMemberService;
        //}

        private string loginedurl = "/member_profile.html";

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public object Login(AuthParam param)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (AuthUser obj = new AuthUser())
                    {
                        return obj.Login(param) ? new ApiResult() : new ErrApiResult("帳號或密碼錯誤");
                    }
                }
                catch (Exception ex)
                {
                    return new ErrApiResult(ex.Message);
                }
            }
            else
                return new ErrApiResult(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("FBLogin")]
        public object FBLogin(string token)
        {
            //HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
            //string token = _request["token"];
            if (ModelState.IsValid)
            {
                try
                {
                    using (AuthUser obj = new AuthUser())
                    {
                        string result = WebShopping.Helpers.SystemFunctions.RequestData("https://graph.facebook.com/me", $"access_token={token}&fields=id,name,email", false);
                        var fbUserDto = JsonConvert.DeserializeObject<FBUserDto>(result);
                        if (obj.FBLogin(fbUserDto))
                        {
                            //HttpContext.Current.Response.Redirect(loginedurl);
                            //return new ApiResult();
                            return Ok(fbUserDto);
                        }
                        else
                            return new ErrApiResult("FB登入失敗");
                    }
                }
                catch (Exception ex)
                {
                    return new ErrApiResult("FB登入失敗!!");
                    return new ErrApiResult(ex.Message);
                }
            }
            else
                return new ErrApiResult(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("LineLogin")]
        public object LineLogin(string code,string state)
        {
            //HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
            //string token = _request["token"];
            //return Ok(code);
            WebShopping.Helpers.SystemFunctions.WriteLogFile($"Line [{code},{state}]");
            if (ModelState.IsValid)
            {
                try
                {
                    using (AuthUser obj = new AuthUser())
                    {
                        bool forTest = false;
                        string redirect_uri = $"{WebShopping.Helpers.Tools.WebSiteUrl}/api/AuthUser/LineLogin";
                        string client_id = WebShopping.Helpers.Tools.oAuthLineClientId; //"1656162595";

                        string result = WebShopping.Helpers.SystemFunctions.RequestData("https://api.line.me/oauth2/v2.1/token",
                            $@"grant_type=authorization_code&code={code}&redirect_uri={redirect_uri}&client_id={client_id}&client_secret=8bac80529082846bb2028c512dc26545", forTest);

                        {
                        if (forTest) result = @"{
    ""access_token"": ""eyJhbGciOiJIUzI1NiJ9.cS2HM0nyi-eMgbi4irxCc9mJjGsdWUIe3w57JFORfF1v7F1kKJaM2d-RGZirW_7__t432Po5PWHUqbp5ETqyQXFh51bDK1vfaX9xU095LQu5NlT6ApLYRrAaO2UX2Ncik6qOIDJG2wvakbiZ0JjdzoH712nP-e_MDW3fKfWbP2Y.D3xKxO-BTxeoCfFU4VjPC5hoM52xEu6AGelG3qYTf0A"",
    ""token_type"": ""Bearer"",
    ""refresh_token"": ""hHvt4lv02OwPGAVko6rP"",
    ""expires_in"": 2592000,
    ""scope"": ""openid profile"",
    ""id_token"": ""eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwczovL2FjY2Vzcy5saW5lLm1lIiwic3ViIjoiVTc1ODE1MGRmYTQwOTJjMjBmZjE2YTJiMDNjYjgyZDU4IiwiYXVkIjoiMTY1NjE2MjU5NSIsImV4cCI6MTYyNTExMzYxMywiaWF0IjoxNjI1MTEwMDEzLCJhbXIiOlsibGluZXNzbyJdLCJuYW1lIjoi6Zmz5a-25qi5IiwicGljdHVyZSI6Imh0dHBzOi8vcHJvZmlsZS5saW5lLXNjZG4ubmV0LzBtMDA4MzFhZjI3MjUxYmE3YjEzMjgzNDY3MWRmOGE4NzE1YWNiYzAwMjgyODAiLCJlbWFpbCI6InByb3N1QGNhc2hmbG93LnR3In0.FhAHg1kvofoVaFIEDPKaeIFvZansoF1ncpDyCiGfp28""
}";
                        }

                        WebShopping.Helpers.SystemFunctions.WriteLogFile($"Line result:{result}");
                        var tokenDto = JsonConvert.DeserializeObject<LineUserTokenDto>(result);

                        string result2 = WebShopping.Helpers.SystemFunctions.RequestData("https://api.line.me/oauth2/v2.1/verify",
                            $@"id_token={tokenDto.id_token}&client_id={client_id}", forTest);

                        {
                        if (forTest) result2 = @"{
    ""iss"": ""https://access.line.me"",
    ""sub"": ""U758150dfa4092c20ff16a2b03cb82d58"",
    ""aud"": ""1656162595"",
    ""exp"": 1625050841,
    ""iat"": 1625047241,
    ""amr"": [
        ""linesso""
    ],
    ""name"": ""陳寶樹"",
    ""picture"": ""https://profile.line-scdn.net/0m00831af27251ba7b132834671df8a8715acbc0028280"",
    ""email"": ""prosu@cashflow.tw""
}";
                        }

                        WebShopping.Helpers.SystemFunctions.WriteLogFile($"Line result2:{result2}\n");
                        var verifyDto = JsonConvert.DeserializeObject<LineUserVerifyDto>(result2);

                        //return obj.LineLogin(verifyDto) ? new ApiResult() : new ErrApiResult("LINE登入失敗");
                        if (obj.LineLogin(verifyDto))
                        {
                            HttpContext.Current.Response.Redirect(loginedurl);
                            return Ok(verifyDto);
                        }
                        else
                            return Ok($"LINE登入失敗!!");
                    }
                }
                catch (Exception ex)
                {
                    return Ok($"LINE登入失敗!!\n{ex.Message}\n{ex.StackTrace}");
                    return new ErrApiResult(ex.Message);
                }
            }
            else {
                //return new ErrApiResult($"code={code};state={state}");
                return new ErrApiResult(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GoogleLogin")]
        public object GoogleLogin(string code, string scope)
        {
            //HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
            //string token = _request["token"];
            WebShopping.Helpers.SystemFunctions.WriteLogFile($"Google [{code},{scope}]");
            //return Ok(code);
            if (ModelState.IsValid)
            {
                try
                {
                    using (AuthUser obj = new AuthUser())
                    {
                        bool forTest = false;
                        string redirect_uri = $"{WebShopping.Helpers.Tools.WebSiteUrl}/api/AuthUser/GoogleLogin";
                        if (HttpContext.Current.Request.Url.Host == "localhost")
                            redirect_uri = "http://localhost:59415/AuthUser/GoogleLogin";
                        string client_id = WebShopping.Helpers.Tools.oAuthGoogleClientId; //"271222022400-pnqe16fno0j8mj0h86shbnqalq2gf3m3.apps.googleusercontent.com";
                        string client_secret = WebShopping.Helpers.Tools.oAuthGoogleClientSecret; //"PfmRwvMOhcSb2yHXK2FriFlS";
                                                                                           
                        string result = WebShopping.Helpers.SystemFunctions.RequestData("https://oauth2.googleapis.com/token",
                            $@"code={(code)}&client_id={(client_id)}&client_secret={(client_secret)}&redirect_uri={(redirect_uri)}&grant_type=authorization_code", forTest);
                        {
                            WebShopping.Helpers.SystemFunctions.WriteLogFile($"Google result:{result}");
                            var tokenDto = JsonConvert.DeserializeObject<GoogleUserTokenDto>(result);

                            string result2 = WebShopping.Helpers.SystemFunctions.RequestData("https://www.googleapis.com/oauth2/v1/userinfo",
                                $@"alt=json&access_token={tokenDto.access_token}", forTest,"GET");

                            WebShopping.Helpers.SystemFunctions.WriteLogFile($"Google result2:{result2}\n");
                            var verifyDto = JsonConvert.DeserializeObject<GoogleUserInfoDto>(result2);

                            if (obj.GoogleLogin(verifyDto)) { 
                                HttpContext.Current.Response.Redirect(loginedurl);
                                return Ok(verifyDto);
                            }
                            else
                                return Ok($"Google登入失敗!!");
                        }

                    }
                }
                catch (Exception ex)
                {
                    WebShopping.Helpers.SystemFunctions.WriteLogFile($"Google登入失敗!!\nMessage={ex.Message}\nStackTrace={ex.StackTrace}");
                    return Ok($"Google登入失敗!!\n{ex.Message}\n{ex.StackTrace}");
                    return new ErrApiResult(ex.Message);
                }
            }
            else
            {
                //return new ErrApiResult($"code={code};state={state}");
                return new ErrApiResult(ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            try
            {
                using (AuthUser obj = new AuthUser())
                {
                    obj.Logout();
                    return Ok(new ApiResult());
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
                //return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));              
            }
        }

        [HttpPost]
        [Route("LoginInfo")]
        public object LoginInfo()
        {
            try
            {
                using (AuthUser obj = new AuthUser())
                {
                    LoginInfoDto user = JsonConvert.DeserializeObject<LoginInfoDto>($"{obj.GetUser()}");
                    user.Count = obj.ShoppingCartCount(new Guid(User.Identity.Name));
                    return new ApiResult(user);
                }
            }
            catch (Exception ex)
            {
                return new ErrApiResult(ex.Message);
            }
        }
    }
}
