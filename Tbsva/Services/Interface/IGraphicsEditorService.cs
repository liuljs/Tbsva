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
    public interface IGraphicsEditorService
    {
        /// <summary>
        /// 1新增也是編輯
        /// </summary>
        /// <param name="request">接收表單內容</param>
        /// <param name="original_graphicsEditor">舊資料</param>
        /// <returns></returns>
        GraphicsEditor InsertGraphicsEditor(HttpRequest request, GraphicsEditor original_graphicsEditor);

        /// <summary>
        /// 2.取一筆資料
        /// </summary>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        GraphicsEditor GetGraphicsEditor();

        /// <summary>
        /// 2.1取出單純舊資料導覽圖片1~8不加URL
        /// </summary>
        /// <returns>200 Ok取得一筆資料,404 NotFound</returns>
        GraphicsEditor GetGraphicsEditorNoUrl();

        /// <summary>
        /// 3修改
        /// </summary>
        /// <param name="request">輸入id編號</param>
        /// <param name="graphicsEditor">要修改的內容</param>
        /// <returns>204 No Content , 404 NotFound</returns>
        void UpdateGraphicsEditor(HttpRequest request, GraphicsEditor graphicsEditor);

        /// <summary>
        /// 4新增圖片
        /// </summary>
        /// <param name="request">接收表單內容</param>
        /// <param name="_directoryName"></param>
        /// <returns>imageURL</returns>
        string AddImage(HttpRequest request, string _directoryName);

        /// <summary>
        /// 5清空所有資料表內容
        /// </summary>
        void DeleteAllContents();

        /// <summary>
        /// 5.1清空所有資料表內容(不刪除圖片)
        /// </summary>
        void DeleteAllNoDelPicContents();

        /// <summary>
        /// 抓路由路徑帶出對應的表格名稱與資料匣名稱
        /// </summary>
        /// <param name="_tableName"></param>
        /// <param name="_directoryName"></param>
        void TableNameDirectoryName(out string _tableName, out string _directoryName);

        /// <summary>
        /// 01新增一個內文圖片並回傳return $@"{m_WebSiteImgUrl}/{folder}/{fileName}"; //回傳圖片的URL && 檔名
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="directoryName">目錄名稱</param>
        /// <param name="tableNameImage">編輯器內插入圖片的表格名</param>
        /// <returns>201, GraphicsEditorImage</returns>
        //GraphicsEditorImage AddUploadImage(HttpRequest httpRequest, string directoryName, string tableNameImage);

        ///<summary>
        /// 02新增一筆GraphicsEditorImage內容圖片,有關聯ID, 適用於在修改時使用
        /// </summary>
        /// <param name="httpRequest">用戶端的要求資訊</param>
        /// <param name="Id"></param>
        /// <param name="directoryName">目錄名</param>
        /// <param name="tableNameImage">編輯器內插入圖片的表格名</param>
        /// <returns>201, GraphicsEditorImage</returns>
        //GraphicsEditorImage AddUploadImage(HttpRequest httpRequest, Guid Id, string directoryName, string tableNameImage);



    }
}
