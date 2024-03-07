using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebShopping.Services
{
    public interface IPrivacyService
    {        
        Privacy GetContent(Guid id);            //取得單筆內容
        List<Privacy> GetContents();            //取得所有的內容
        bool DeleteAllContents();               //清空所有資料表內容
        Privacy AddContent(HttpRequest request);//新增1筆內容(FormData)
        Privacy AddContent(Privacy privacy);    //新增1筆內容(Json)
        string AddImage(HttpRequest request);   //新增1筆圖片

    }
}
