using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class VideoContentService : IVideoContentService
    {
        #region DI依賴注入功能
        private IDapperHelper DapperHelper;
        private IImageFileHelper ImageFileHelper;

        public VideoContentService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        {
            DapperHelper = dapperHelper;
            ImageFileHelper = imageFileHelper;
        }

        //public VideoContentService(IDapperHelper dapperHelper, IImageFileHelper imageFileHelper)
        //{
        //    DapperHelper = dapperHelper ?? throw new ArgumentNullException(nameof(dapperHelper));
        //    ImageFileHelper = imageFileHelper ?? throw new ArgumentNullException(nameof(imageFileHelper));
        //}
        #endregion

        #region  新增實作
        public VideoContent InsertVideoContent(HttpRequest httpRequest)
        {
            VideoContent videoContent = RequestData(httpRequest);
            string _sql = @"Insert Into [VideoContent]
                                             ([id]
                                             ,[VideoCategory_id]
                                             ,[image_name]
                                             ,[title]
                                             ,[brief]
                                             ,[content]
                                             ,[Video_url]
                                             ,[creation_date]
                                             ,[Enabled]
                                             ,[Sort]
                                             ,[first])
                                         VALUES
                                            ( @id,
                                              @VideoCategory_id,
                                              @image_name,
                                              @title,
                                              @brief,
                                              @content,
                                              @Video_url,
                                              @creation_date,
                                              @Enabled,
                                              @Sort,
                                              @first ) ";
            DapperHelper.ExecuteSql(_sql, videoContent);  //1.新增資料

            //處理圖片
            //1.取圖片路徑C:\xxx
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Video);

            //2.存放上傳圖片(1實體檔,2檔名,3路徑)
            ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], videoContent.image_name, _RootPath_ImageFolderPath);

            return videoContent;   
        }
        private VideoContent RequestData(HttpRequest request)
        {
            VideoContent videoContent = new VideoContent();
            videoContent.id = Guid.NewGuid();
            videoContent.VideoCategory_id = Convert.ToInt32(request.Form["VideoCategory_id"]); //關連目錄id
            videoContent.image_name = videoContent.id + ".png";                                                          //圖片檔名使用id來當
            videoContent.title = request.Form["title"];                                                                                 //影片標題
            videoContent.brief = request.Form["brief"];                                                                              //影片簡述_段落1
            videoContent.content = request.Form["content"];                                                                   //影片簡述_段落2
            videoContent.Video_url = request.Form["Video_url"];                                                            //影片連結
            videoContent.creation_date = DateTime.Now;                                                                         //新增時間
            videoContent.Enabled = Convert.ToBoolean(Convert.ToByte(request.Form["Enabled"]));//是否啟用(0/1)

            if (string.IsNullOrWhiteSpace(request.Form["Sort"]))
            {
                videoContent.Sort = Convert.ToInt32(Tools.GetInstance().DefaultValueSort);                 //沒有設定欄位給一個預設值
            }
            else
            {
                videoContent.Sort = Convert.ToInt32(request.Form["Sort"]);
            }
            //是否置頂
            if ("Y" == request.Form["first"])
            {
                videoContent.first = "Y";
            }
            else
            {
                videoContent.first = "N";
            }

            //抓出對應目錄的名稱
            string _sql = @"SELECT name From [VideoCategory] Where [id]=@id";
            VideoCategory videoCategory = new VideoCategory();
            videoCategory.id = videoContent.VideoCategory_id;  //關聯目錄id
            videoCategory = DapperHelper.QuerySqlFirstOrDefault(_sql, videoCategory);  //這裏雖然帶整個類別，但dapper會抓得到id
            videoContent.VideoCategoryName = videoCategory.name;  //將抓出來的目錄名稱帶入VideoCategoryName內容屬性欄位

            return videoContent;
        }
        #endregion

        #region 取得一筆資料
        public VideoContent GetVideoContent(Guid id)
        {
            VideoContent videoContent = new VideoContent();
            videoContent.id = id;
            string adminQuery = Auth.Role.IsAdmin ? "" : " And V2.Enabled = 1 ";

            //增加目錄名稱VideoCategoryName
            string _sql = $"Select V2.*, V1.name AS VideoCategoryName From [VideoContent] V2 " +
                      $"Left Join [VideoCategory] V1 ON V1.id = V2.VideoCategory_id " +
                      $"Where V2.id=@id {adminQuery}";

            //image_name加上http網址
            videoContent = DapperHelper.QuerySqlFirstOrDefault(_sql, videoContent);
            if (videoContent != null)
            {
                videoContent.image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Video, videoContent.image_name);
            }
            return videoContent;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateVideoContent(HttpRequest httpRequest, VideoContent videoContent)
        {
            videoContent = RequestDataMod(httpRequest, videoContent);   //將接收來的參數，和抓到的要修改資料合併
            if (httpRequest.Files.Count > 0)  //檢查有沒有上傳圖片的欄位選項
            {
                if (httpRequest.Files[0].ContentLength > 0)  //檢查上傳檔案大小，確定是有上傳
                {
                    //以下處理圖片
                    //1. 取得要放置的目錄路徑
                    string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Video);

                    //GetVideoContent裏面的image_name有加網址m_ImageFileHelper.GetImageLink，改只取檔案名與副檔
                    string _image_name = Path.GetFileName(videoContent.image_name);
                    //2.存放上傳文章封面圖片(1實體檔,2檔名,3路徑)
                    ImageFileHelper.SaveUploadImageFile(httpRequest.Files[0], _image_name, _RootPath_ImageFolderPath);
                }
            }
            string _sql = @"UPDATE [VideoContent]
                                           SET 
                                               [VideoCategory_id] = @VideoCategory_id
                                              ,[title] = @title
                                              ,[brief] = @brief
                                              ,[content] = @content
                                              ,[Video_url] = @Video_url
                                              ,[updated_date] = @updated_date
                                              ,[Enabled] = @Enabled
                                              ,[Sort] = @Sort
                                              ,[first] = @first
                                         WHERE [id] = @id";
            DapperHelper.ExecuteSql(_sql, videoContent);
        }
        /// <summary>
        /// 讀取表單資料,轉到VideoContent
        /// </summary>
        /// <param name="httpRequest">讀取表單資料(修改時用)</param>
        /// <param name="videoContent">表單傳給類別資料</param>
        /// <returns>VideoContent</returns>
        private VideoContent RequestDataMod(HttpRequest httpRequest, VideoContent videoContent)
        {
            videoContent.VideoCategory_id = Convert.ToInt32(httpRequest.Form["VideoCategory_id"]);  //若這裏有變，目錄VideoCategoryName並未重抓
            videoContent.title = httpRequest.Form["title"];
            videoContent.brief = httpRequest.Form["brief"];
            videoContent.content = httpRequest.Form["content"];
            videoContent.Video_url = httpRequest.Form["Video_url"];
            videoContent.updated_date = DateTime.Now;
            videoContent.Enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["Enabled"]));

            if (string.IsNullOrWhiteSpace(httpRequest.Form["Sort"]))         //空值,或空格，或沒設定此欄位null
            {
                videoContent.Sort = Convert.ToInt32(Tools.GetInstance().DefaultValueSort);                 //沒有設定欄位給一個預設值
            }
            else
            {
                videoContent.Sort = Convert.ToInt32(httpRequest.Form["Sort"]);                                          //排序 
            }
            //是否置頂
            if (httpRequest.Form["first"] == "Y")
            {
                videoContent.first = "Y";
            }
            else
            {
                videoContent.first = "N";
            }
            return videoContent;
        }
        #endregion

        public void DeleteVideoContent(VideoContent videoContent)
        {
            //1. 取得放置圖片的目錄路徑
            string _RootPath_ImageFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Video);

            //刪除內容圖片
            string _image_name = Path.GetFileName(videoContent.image_name);  //取出內容圖片
            string _image_name_FilePath = Path.Combine(_RootPath_ImageFolderPath, _image_name);  //加上檔案路徑
            //刪除封面圖片
            if (File.Exists(_image_name_FilePath))
            {
                File.Delete(_image_name_FilePath);
            }

            //刪除內容資料
            string _sql = @"DELETE FROM [VideoContent] WHERE id = @id";
            DapperHelper.ExecuteSql(_sql, videoContent);
        }
        /// <summary>
        /// 列出多筆資料
        /// </summary>
        /// <param name="VideoCategory_id">所在目錄下</param>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<VideoContent> GetVideoContents(int? VideoCategory_id, int? count, int? page, string keyword)
        {
            string page_sql = string.Empty;   //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string VideoCategory_id_sql = string.Empty;  //所屬的目錄id sql
            string search_sql = string.Empty;  //搜尋欄位組合

            if (count != null && count > 0 && page != null & page >0)
            {
                int startRowjumpover = 0;  //跳過筆數
                startRowjumpover = Convert.ToInt32((page - 1) * count);   // <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover}  ROWS FETCH NEXT {count}  ROWS ONLY ";
            }
            string adminQuery = Auth.Role.IsAdmin ? " where 1=1 " : " where V2.Enabled = 1 ";   //登入取得所有資料:未登入只能取得上線資料
            VideoCategory_id_sql = VideoCategory_id > 0 ? $" and VideoCategory_id = {VideoCategory_id}" : "";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                search_sql = $" and ( V2.title like '%{keyword}%' or V2.brief like '%{keyword}%' or V2.content like '%{keyword}%' ) ";
            }

            string _sql = $"Select V2.*, V1.name AS VideoCategoryName From [VideoContent] V2 " +
                                    $"Left Join [VideoCategory] V1 ON V1.id = V2.VideoCategory_id " +
                                    $"{adminQuery} " +
                                    $"{VideoCategory_id_sql} " +
                                    $"{search_sql} " +
                                    @"Order by V2.[first] Desc, V2.Sort, V2.creation_date Desc " +
                                    $"{page_sql} " ;

            List<VideoContent> videoContents = DapperHelper.QuerySetSql<VideoContent>(_sql).ToList();

            //將資料庫image_name加上URL
            for (int i = 0; i < videoContents.Count; i++)
            {
                videoContents[i].image_name = Tools.GetInstance().GetImageLink(Tools.GetInstance().Video, videoContents[i].image_name);
            }
            return videoContents;
        }
    }
}