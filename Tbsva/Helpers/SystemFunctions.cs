using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace WebShopping.Helpers
{
    public class SystemFunctions
    {
        /// <summary>
        /// 發送Email
        /// </summary>
        /// <param name="p_strSenderName">送信人名稱</param>
        /// <param name="p_strSenderMail">送信人Email</param>
        /// <param name="p_strRecipient">收件人Email</param>     
        /// <param name="p_lstrCC">副本</param>
        /// <param name="p_lstrBCC">密件副本</param>
        /// <param name="p_strSubject">主題</param>
        /// <param name="p_strContent">內容</param>

        public static void SendMail(string p_strSenderName, string p_strSenderMail, string p_strRecipient, List<string> p_lstrCC, List<string> p_lstrBCC, string p_strSubject, string p_strContent)
        {
            //Tools.GetInstance().SendMail(Tools.Company_Name, Tools.Admin_Mail, "prosu@payware.com.tw", new List<string>(), new List<string>(), "營天下購物商城後台網站錯誤訊息通知", "測試中");

            SmtpClient client = new SmtpClient();
            client.Port = int.Parse(Tools.GetInstance().Mail_Port);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = Tools.GetInstance().Mail_Host;
            client.Credentials = new System.Net.NetworkCredential(Tools.GetInstance().Smtp_Id, Tools.GetInstance().Smtp_Pw);

            MailMessage mail = new MailMessage(new MailAddress(p_strSenderMail, p_strSenderName).ToString(), p_strRecipient);

            //若有多個副本
            if (p_lstrCC.Count > 0)
            {
                for (int i = 0; i < p_lstrCC.Count; i++)
                    mail.CC.Add(p_lstrCC[i]);
            }

            //若有多個密件副本
            if (p_lstrBCC.Count > 0)
            {
                for (int i = 0; i < p_lstrCC.Count; i++)
                    mail.Bcc.Add(p_lstrBCC[i]);
            }

            mail.Subject = p_strSubject;
            mail.Body = p_strContent;
            mail.IsBodyHtml = true;
            client.Send(mail);
        }

        /// <summary>
        /// 寫記錄
        /// </summary>
        /// <param name="p_strContent"> 內容 </param>
        public static void WriteLogFile(string p_strContent)
        {
            string DIRNAME = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\Log\";
            string FILENAME = DIRNAME + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            if (!Directory.Exists(DIRNAME))
                Directory.CreateDirectory(DIRNAME);

            if (!File.Exists(FILENAME))
            {
                File.Create(FILENAME).Close();
            }

            string strMsg_ = $"[{DateTime.Now.ToString("HH:mm:ss")}] ==> {p_strContent} {Environment.NewLine}";

            File.AppendAllText(FILENAME, strMsg_);
        }

        public static string ReadLogFile()
        {
            string DIRNAME = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\Log\";
            string FILENAME = DIRNAME + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string strMsg_ = string.Empty;

            if (!Directory.Exists(DIRNAME))
                Directory.CreateDirectory(DIRNAME);

            if (!File.Exists(FILENAME))
            {
                File.Create(FILENAME).Close();
            }

            strMsg_ = File.ReadAllText(FILENAME);

            return strMsg_;
        }
        public static void DeleteLogFile()
        {
            string DIRNAME = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\Log\";
            string FILENAME = DIRNAME + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string strMsg_ = string.Empty;

            if (!Directory.Exists(DIRNAME))
                Directory.CreateDirectory(DIRNAME);

            if (File.Exists(FILENAME))
            {
                File.Delete(FILENAME);
            }

        }

        public static string GetFormData(HttpRequest _request)
        {
            string str = "";
            str += "Request.Form<table border=1>";
            foreach (string k in _request.Form.Keys)
            {
                str += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", k, HttpUtility.UrlDecode(_request.Form[k], Encoding.GetEncoding(950)));
            }
            str += "</table><br><br>";

            str += "Request.QueryString<table border=1>";
            foreach (string k in _request.QueryString.Keys)
            {
                str += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", k, HttpUtility.UrlDecode(_request.QueryString[k], Encoding.GetEncoding(950)));
            }
            str += "</table><br><br>";


            return str;
        }

        public static string GetJsonData(HttpRequest _request)
        {
            string str = "";
            foreach (string k in _request.Form.Keys)
            {
                str += $"\t\"{k}\" : \"{HttpUtility.UrlDecode(_request.Form[k], Encoding.GetEncoding(950))}\",\n";
            }
            foreach (string k in _request.QueryString.Keys)
            {
                str += $"\t\"{k}\" : \"{HttpUtility.UrlDecode(_request.QueryString[k], Encoding.GetEncoding(950))}\",\n";
            }
            str = str.Trim('\n').Trim(',');
            str = "{\n" + str.Trim(',') + "\n}";

            return str;
        }

        /// <summary>
        /// 向遠端伺服器要資料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string RequestData(string url, string data, bool forTest, string method = "POST")
        {
            if (forTest) return "";//測試用

            if (method == "GET")
            {
                url = url + "?" + data;
                data = string.Empty;
            }
            byte[] postData = Encoding.GetEncoding("UTF-8").GetBytes(data);

            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Method = method;    // 方法
            request.KeepAlive = true; //是否保持連線
            request.ContentType = "application/x-www-form-urlencoded";

            request.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            if (method == "POST")
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(postData, 0, postData.Length);
                }

            string result = string.Empty;
            using (WebResponse response = request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(dataStream);
                result = sr.ReadToEnd();
            }
            return result;
        }


        /// <summary>
        /// 串要有值要更新，沒值不更新，null值要刪成NULL
        /// </summary>
        /// <typeparam name="T">也可以用 Object</typeparam>
        /// <param name="fields">設定要串sql的圖片欄位</param>
        /// <param name="model">指定任何類別</param>
        /// <param name="_request">接收欄位</param>
        /// <returns></returns>
        public static string picSql<T>(string[] fields, T model, HttpRequest _request)
        {
            string pic = string.Empty;
            //var null = _request.Form["Picture01"];
            //圖片更新有4種狀況
            //1.新增:選圖片上傳_request.Files;AllKeys就會有欄位,檔名就會代給類別_pictureList.Picture01 = _file.FileName
            //因為Picture01欄位有圖檔, 所以_request.Form["Picture01"]就不見變成null, 但下面的null並不是真的null, 而時字串的"null"
            //所以新增時以下左邊會成立，右邊不成立
            //2.修改:有重新上傳的_request.Files,AllKeys就會有欄位會有值, 
            //3.沒有變更:沒有重新上傳檔案,但在_request.Form[""] = xx.PNG會接到現有的檔案名稱，所以下面條件都不成立
            //3.按垃圾筒,並沒有上傳Files內容，而是傳回表單字串null(_request.Form["xx"] == "null"，所以會執行下面右方sql將資料欄改NULL
            //4.完全沒變動:會變成_request.Form的值, 原本有圖檔的如_request.Form["Picture01"]=01.PNG, 沒有圖檔的就會是空值，下面條件都不成立
            //string FiledContent每次代FiledContent會變回null, 但若string 只宣告一次，下面的FiledContent會延上面的值
            //propertyInfo一開始會是null，GetProperty完後若確定model有欄位propertyInfo不等null，下面的propertyInfo會延上面的值，重抓後會覆蓋
            System.Reflection.PropertyInfo propertyInfo;

            foreach (string field in fields)
            {
                propertyInfo = model.GetType().GetProperty(field);  //GetProperty("欄位")抓出欄位辨斷與model有無此欄
                if (propertyInfo != null)
                {
                    string FiledContent = (string)propertyInfo.GetValue(model);
                    if (FiledContent != null) pic += $",[{field}]=@{field}"; if (_request.Form[field] == "null") pic += $",[{field}]=NULL";
                }
            }
            return pic;
        }


        /// <summary>
        /// 在新增時串	新增圖文編輯內容資訊，新增，修改，刪除不同的SQL新增串法
        /// </summary>
        /// <typeparam name="T">也可以用 Object</typeparam>
        /// <param name="fields">設定要串sql的圖片欄位</param>
        /// <param name="model">指定任何類別</param>
        /// <param name="_request"></param>
        /// <param name="_Sqltop">上半</param>
        /// <param name="_Sqldown">下半</param>
        /// <returns></returns>
        public static void picSqlInsert<T>(string[] fields, T model, HttpRequest _request, out string _Sqltop, out string _Sqldown)
        {
            _Sqltop = string.Empty;  //主資料表
            _Sqldown = string.Empty; //目錄名
            //var null = _request.Form["Picture01"];
            //圖片更新有4種狀況
            //1.新增:選圖片上傳_request.Files;AllKeys就會有欄位,檔名就會代給類別_pictureList.Picture01 = _file.FileName
            //因為Picture01欄位有圖檔, 所以_request.Form["Picture01"]就不見變成null, 但下面的null並不是真的null, 而時字串的"null"
            //所以新增時以下左邊會成立，右邊不成立
            //2.修改:有重新上傳的_request.Files,AllKeys就會有欄位會有值, 
            //3.沒有變更:沒有重新上傳檔案,但在_request.Form[""] = xx.PNG會接到現有的檔案名稱，所以下面條件都不成立
            //3.按垃圾筒,並沒有上傳Files內容，而是傳回表單字串null(_request.Form["xx"] == "null"，所以會執行下面右方sql將資料欄改NULL
            //4.完全沒變動:會變成_request.Form的值, 原本有圖檔的如_request.Form["Picture01"]=01.PNG, 沒有圖檔的就會是空值，下面條件都不成立
            //string FiledContent每次代FiledContent會變回null, 但若string 只宣告一次，下面的FiledContent會延上面的值
            //propertyInfo一開始會是null，GetProperty完後若確定model有欄位propertyInfo不等null，下面的propertyInfo會延上面的值，重抓後會覆蓋
            System.Reflection.PropertyInfo propertyInfo;

            foreach (string field in fields)
            {
                propertyInfo = model.GetType().GetProperty(field);  //GetProperty("欄位")抓出欄位辨斷與model有無此欄
                if (propertyInfo != null)
                {
                    string FiledContent = (string)propertyInfo.GetValue(model);

                    if (FiledContent != null || _request.Form[field] == "null") _Sqltop += $",[{field}]";  //有檔案或改傳null(要刪除原本圖片)

                    //傳 "null"(要刪除原本圖片)值給NULL來更新
                    if (_request.Form[field] == "null")
                    {
                        _Sqldown += $"NULL,";
                    }
                    else  //若不是會有上傳空或有檔案時，如果是空不會有結果若是有檔則加入欄位
                    {
                        if (FiledContent != null) _Sqldown += $"@{field},";
                    }

                }
            }
           
        }

        /// <summary>
        /// 加入picaddhttp
        /// </summary>
        /// <param name="fields">string[] fields = { "image_name", "navPics01", "navPics02", "navPics03", "navPics04", "navPics05", "navPics06", "navPics07", "navPics08" };</param>
        /// <param name="model">任何類別</param>
        /// <param name="strFolder">Tools.GetInstance().xxx</param>
        /// <param name="id">自動id</param>
        public static void picaddhttp(string[] fields, Object model, string strFolder, int id)
        {
            System.Reflection.PropertyInfo propertyInfo;

            foreach (string field in fields)
            {
                propertyInfo = model.GetType().GetProperty(field);  //GetProperty("欄位")抓出欄位辨斷與model有無此欄
                if (propertyInfo != null)
                {
                    string FiledContent = (string)propertyInfo.GetValue(model);  //取出欄位值
                    if (FiledContent != null)
                    {
                        //加上網址後重新寫回model指定的欄位值
                        propertyInfo.SetValue(model, Tools.GetInstance().GetImageLink(strFolder + $@"/{id}", FiledContent));   // 確定model有要的屬性名, 將值傳入model對應欄位值 propertyInfo.SetValue(物件, 值);
                        //activity.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Activity + $@"/{activity.id}", FiledContent);
                    }
                }
            }

        }
    }
}