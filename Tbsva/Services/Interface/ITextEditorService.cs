using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public interface ITextEditorService
    {
        /// <summary>
        /// 新增也是編輯(先刪除在新增)
        /// </summary>
        /// <param name="tableName">表格名</param>
        /// <param name="request">接收表單內容</param>
        /// <returns>201</returns>
        TextEditor InsertTextEditor(string tableName, HttpRequest request);

        /// <summary>
        /// 清空所有資料表內容
        /// </summary>
        /// <param name="tableName">表格名</param>
        void DeleteAllContents(string tableName);

        /// <summary>
        /// 新增圖片
        /// </summary>
        /// <param name="directoryName">目錄名</param>
        /// <param name="request">接收表單內容</param>
        /// <returns>imageURL</returns>
        string AddImage(string directoryName, HttpRequest request);

        /// <summary>
        /// 取得全部內容
        /// </summary>
        /// <param name="tableName">表格名</param>
        /// <returns>TextEditor</returns>
        TextEditor GetTextEditor(string tableName);
    }
}
