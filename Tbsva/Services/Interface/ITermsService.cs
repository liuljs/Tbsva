using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebShopping.Services
{
    public interface ITermsService
    {        
        Terms GetContent(Guid id);            //取得單筆內容
        List<Terms> GetContents();            //取得所有的內容
        bool DeleteAllContents();               //清空所有資料表內容
        Terms AddContent(HttpRequest request);//新增1筆內容(FormData)
        Terms AddContent(Terms terms);    //新增1筆內容(Json)
        string AddImage(HttpRequest request);   //新增1筆圖片

    }
}
