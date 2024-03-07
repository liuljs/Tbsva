using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class VideoCategoryService : IVideoCategoryService
    {
        #region DI依賴注入功能
        //Why use DI?
        //https://medium.com/wenchin-rolls-around/%E6%B7%BA%E5%85%A5%E6%B7%BA%E5%87%BA-dependency-injection-ea672ba033ca
        //如果你跟我一樣還沒有小孩，體驗可能沒那麼深刻，那我們看下一個例子。
        //如何稱呼男/女友？你都叫他他的名字還是「寶貝」？
        //如果叫的是「寶貝」，那恭喜你，你是 DI 大師
        //插座（介面）允許不同電器的插頭（實作）
        //緊耦合：就像廉價旅館直接用電線連進牆壁上一個洞的吹風機，無法替換電器、難以修改
        //鬆耦合：就像插座，可以替換吹風機、筆電插頭等，符合 SOLID 裡面的里氏替換原則(Liskov Substitution Principle)
        private IDapperHelper dapperHelper;
        public VideoCategoryService(IDapperHelper dapperHelper)
        {
            this.dapperHelper = dapperHelper;
        }
        #endregion

        #region 新增
        public VideoCategory InsertVideoCategory(HttpRequest request)
        {
            VideoCategory videoCategory = new VideoCategory();  //產生一個空類別
            videoCategory = RequestData(videoCategory, request);
            string _sql = @"INSERT INTO [VideoCategory]
                                            ([name]
                                           ,[content]
                                           ,[Enabled]
                                           ,[Sort])
                                     VALUES
                                               (@name
                                               ,@content
                                               ,@Enabled
                                               ,@Sort )
                                                Select scope_identity()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = dapperHelper.QuerySingle(_sql, videoCategory); //需使用QuerySingle，因Execute所傳回是新增成功數值
            videoCategory.id = id;

            //自動帶id或輸入id
            if (string.IsNullOrWhiteSpace(request.Form["Sort"]))          //空值,或空格，或沒設定此欄位null
            {
                videoCategory.Sort = id;     //沒有Sort值時給剛新增的id值
            }
            else
            {
                videoCategory.Sort = Convert.ToInt32(request.Form["Sort"]);        //排序 
            }
            _sql = @"UPDATE [VideoCategory] 
                                SET [Sort] = @Sort 
                             Where [id] = @id ";
            dapperHelper.ExecuteSql(_sql, videoCategory);         //更新排序

            return videoCategory;
        }
        /// <summary>
        /// 讀取表單資料
        /// </summary>
        /// <param name="videoCategory">目錄類別資料</param>
        /// <param name="_request">讀取表單資料</param>
        /// <returns></returns>
        private VideoCategory RequestData(VideoCategory videoCategory, HttpRequest _request)
        {
            //VideoCategory videoCategory = new VideoCategory();  //Error CS0136 RequestData上一行已宣告
            videoCategory.name = _request.Form["name"];
            videoCategory.content = _request.Form["content"];
            videoCategory.Enabled = Convert.ToBoolean(Convert.ToByte(_request.Form["Enabled"]));  //是否啟用(0/1)
            videoCategory.Sort = Convert.ToInt32(_request.Form["Sort"]);

            return videoCategory;
        }
        #endregion

        #region 取得一筆資料
        public VideoCategory GetVideoCategory(int id)
        {
            VideoCategory videoCategory = new VideoCategory();              //先設定一個空類別
            videoCategory.id = id;                      //將輸入的id傳給此類別videoCategory.id

            string adminQuery = Auth.Role.IsAdmin ? "" : " and Enabled = 1 ";
            string _sql = $"SELECT * FROM [VideoCategory] Where [id]=@id {adminQuery} ";

            videoCategory = dapperHelper.QuerySqlFirstOrDefault(_sql, videoCategory);

            return videoCategory;
        }
        #endregion

        #region 取得所有目錄資料
        public List<VideoCategory> GetVideoCategories()
        {
            string adminQuery = Auth.Role.IsAdmin ? "" : " Where Enabled = 1 ";
            string _sql = $"SELECT * FROM [VideoCategory] {adminQuery} Order by Sort ";

            List<VideoCategory> videoCategories = dapperHelper.QuerySetSql<VideoCategory>(_sql).ToList();

            return videoCategories;
        }
        #endregion

        #region 修改資料
        public void UpdateVideoCategory(HttpRequest request, VideoCategory videoCategory)
        {
            videoCategory.name = request.Form["name"];
            videoCategory.content = request.Form["content"];
            videoCategory.Enabled = Convert.ToBoolean(Convert.ToByte(request.Form["Enabled"]));
            videoCategory.Sort = Convert.ToInt16(request.Form["Sort"]);

            //$@"" 用法 @純字串 $可以設定變數{adminQuery} 
            string _sql = @"UPDATE [VideoCategory]
                                        SET [name] = @name,
                                               [content] = @content,
                                               [updated_date] = getdate(),
                                               [Enabled] = @Enabled,
                                               [Sort] = @Sort
                                        Where [id] = @id ";
            //執行更新
            int result = dapperHelper.ExecuteSql(_sql, videoCategory);
        }
        #endregion

        #region 刪除一筆資料
        public void DeleteVideoCategory(VideoCategory videoCategory)
        {
            string _sql = @"Delete From [VideoCategory] Where id = @id";
            dapperHelper.ExecuteSql(_sql, videoCategory);
        }
        #endregion

    }
}