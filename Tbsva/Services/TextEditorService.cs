using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class TextEditorService : ITextEditorService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        private IImageFileHelper m_ImageFileHelper;

        public TextEditorService(IDapperHelper iDapperHelper, IImageFileHelper imageFileHelper)
        {
            _IDapperHelper = iDapperHelper;
            m_ImageFileHelper = imageFileHelper;
        }
        #endregion

        #region 新增一筆資料
        public TextEditor InsertTextEditor(string tableName, HttpRequest request)
        {
            TextEditor textEditor = new TextEditor();
            textEditor.textEditorId = Guid.NewGuid();                //內容類別另設 ID 唯一索引用
            textEditor.title = request.Form["title"];
            textEditor.subTitle = request.Form["subTitle"];
            textEditor.other = request.Form["other"];
            textEditor.content = request.Form["content"];         //內容資訊Delta 物件類似 JSON 格式的物件
            textEditor.creationDate = DateTime.Now;
            string _sql = $@"INSERT INTO [{tableName}]
                                       ( [TEXTEDITORID]
                                        ,[TITLE]
                                        ,[SUBTITLE]
                                        ,[OTHER]
                                        ,[CONTENT]
                                        ,[CREATIONDATE])
                                       VALUES
                                       ( @TEXTEDITORID,
                                         @TITLE,
                                         @SUBTITLE,
                                         @OTHER,
                                         @CONTENT,
                                         @CREATIONDATE )
                                          SELECT SCOPE_IDENTITY()";  
            //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。

            int id = _IDapperHelper.QuerySingle(_sql, textEditor);
            textEditor.id = id;
            return textEditor;   //這裏的資料是來自上面表單收集來的
        }
        #endregion


        #region 刪除所有資料
        public void DeleteAllContents(string tableName)
        {
            string _sql = $@"TRUNCATE TABLE [{tableName}]";
            _IDapperHelper.ExecuteSql(_sql);
        }
        #endregion


        #region 編輯時插入圖片小圖
        public string AddImage(string directoryName, HttpRequest request)
        {
            //設定圖片檔名
            string _strFileName = Guid.NewGuid() + ".png";
            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(directoryName);

            //2上傳圖片(1實體圖片檔, 2檔名, 3放圖路徑)
            m_ImageFileHelper.SaveUploadImageFile(request.Files[0], _strFileName, _RootPath_ImageFolderPath);

            //3回傳http圖片網址GetImageLink(1.指定目錄,2檔名)
            string _imageUrl = Tools.GetInstance().GetImageLink(directoryName, _strFileName);

            return _imageUrl;
        }
        #endregion


        #region 取出資料
        public TextEditor GetTextEditor(string tableName)
        {
            TextEditor textEditor = new TextEditor();
            string _sql = $"SELECT TOP 1 * FROM [{tableName}] ORDER BY CREATIONDATE DESC";

            textEditor = _IDapperHelper.QuerySqlFirstOrDefault<TextEditor>(_sql);

            return textEditor;
        }
        #endregion

    }
}