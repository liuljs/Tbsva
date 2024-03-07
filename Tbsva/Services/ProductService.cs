using WebShopping.Connection;
using WebShopping.Helpers;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Configuration;
using System.IO;
using WebShopping.Dtos;
using AutoMapper;
using Newtonsoft.Json;
using static WebShopping.Dtos.EditorDto;

namespace WebShopping.Services
{
    public class ProductService : IProductService
    {
        IDapperHelper m_DapperHelper;

        IImageFileHelper m_ImageFileHelper;

        private IMapper m_Mapper;

        private string m_imageFolder = @"\Admin\backStage\img\products\";

        public ProductService(IDapperHelper dapper, IImageFileHelper imageFile, IMapper mapper)
        {
            m_DapperHelper = dapper;
            m_ImageFileHelper = imageFile;
            m_Mapper = mapper;
        }

        /// <summary>
        /// 前台取得所有產品
        /// 有條件的查詢
        /// </summary>
        /// <returns></returns>
        public List<Spu> GetProducts(QueryProduct query)
        {
            List<Spu> _spus = new List<Spu>();

            string pages = "";
            if (query.count != null && query.page != null && query.count > 0 && query.page > 0)
            {
                int startRowIndex = 0;
                startRowIndex = Convert.ToInt32(query.page - 1) * Convert.ToInt32(query.count);
                pages = $" OFFSET {startRowIndex} ROWS FETCH NEXT {query.count} ROWS ONLY ";
            }

            string strConditionSql_ = string.Empty;
            if (query.cid1 != null)
                strConditionSql_ += $@" AND cid1={query.cid1}";
            if (query.cid2 != null)
                strConditionSql_ += $@" AND cid2={query.cid2}";
            if (query.cid3 != null)
                strConditionSql_ += $@" AND cid3={query.cid3}";
            if (query.recommend != null && Convert.ToBoolean(query.recommend))
                strConditionSql_ += $@" AND recommend=1";
            if (!String.IsNullOrWhiteSpace(query.keyword)) {
                strConditionSql_ += $@" AND title like '%'+@keyword+'%'";
            }

            string sql_ = $@"SELECT s.*,c.cid1,c.cid2,c.cid3
                            ,(SELECT sum(stock_qty) FROM [Sku] where spu_id=s.id)stock_qty
                            ,(SELECT sum(sell_qty) FROM [Sku] where spu_id=s.id)sell_qty
                            FROM [Spu]s left join [spu_category]c on s.id=c.spu_id
                            WHERE [Enabled]=1 
                            And (starts_at IS NULL OR getdate()>=starts_at) 
                            And (ends_at IS NULL OR getdate()<ends_at) 
                            {strConditionSql_} 
                            ORDER BY id desc
                            {pages}";

            _spus = m_DapperHelper.QuerySetSql<QueryProduct, Spu>(sql_, query).ToList();

            return _spus;
        }

        /// <summary>
        /// 後台取得所有產品
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<Spu> GetProductsAdmin(QueryProduct query)
        {
            List<Spu> _spus = new List<Spu>();

            string pages = "";
            if (query.count != null && query.page != null && query.count > 0 && query.page > 0)
            {
                int startRowIndex = 0;
                startRowIndex = Convert.ToInt32(query.page - 1) * Convert.ToInt32(query.count);
                pages = $" OFFSET {startRowIndex} ROWS FETCH NEXT {query.count} ROWS ONLY ";
            }

            string strConditionSql_ = string.Empty;
            if (query.id != null)
                strConditionSql_ += $@" AND s.id={query.id}";
            if (query.cid1 != null)
                strConditionSql_ += $@" AND cid1={query.cid1}";
            if (query.cid2 != null)
                strConditionSql_ += $@" AND cid2={query.cid2}";
            if (query.cid3 != null)
                strConditionSql_ += $@" AND cid3={query.cid3}";
            if (query.recommend != null && Convert.ToBoolean(query.recommend))
                strConditionSql_ += $@" AND recommend=1";
            if (!String.IsNullOrWhiteSpace(query.keyword))
            {
                strConditionSql_ += $@" AND title like '%'+@keyword+'%'";
            }
            switch (query.cond)
            {//1.已上架,2.已售完,3.停售,4.未上架
                case "1": strConditionSql_ += " AND ((starts_at=null or getdate()>=starts_at) AND (ends_at=null or getdate()<ends_at)) AND Enabled=1 "; break;
                case "2": strConditionSql_ += " AND (SELECT sum(stock_qty) FROM [Sku] where spu_id=s.id)<=0 "; break;
                case "3": strConditionSql_ += " AND sell_stop=1 "; break;
                case "4": strConditionSql_ += " AND ((getdate()<starts_at) OR (getdate()>ends_at)) OR Enabled=0 "; break;
            }

            string sql_ = $@"SELECT s.*,c.cid1,c.cid2,c.cid3
                            ,(SELECT sum(stock_qty) FROM [Sku] where spu_id=s.id)stock_qty
                            ,(SELECT sum(sell_qty) FROM [Sku] where spu_id=s.id)sell_qty
                            FROM [Spu] s left join spu_category c on s.id=c.spu_id 
                            WHERE s.[id]>0 
                            {strConditionSql_}
                            ORDER BY id desc
                            {pages}";

            _spus = m_DapperHelper.QuerySetSql<QueryProduct, Spu>(sql_, query).ToList();
            switch (query.cond) {
                case "4"://因日期未上架的Enabled也設為0
                    foreach (var s in _spus.FindAll(a => a.Enabled == 1))
                        s.Enabled = 0;
                    break;
            }

            return _spus;
        }

        public Spu GetProductInfo(int id, bool IsAdmin)
        {
            Spu _spu = new Spu() { Id = id };

            string adminQuery = IsAdmin ? "" : " AND [Enabled]=1 And (starts_at IS NULL OR getdate()>=starts_at) And (ends_at IS NULL OR getdate()<ends_at) ";

            string _sql = $"SELECT * FROM [Spu] WHERE [ID]=@ID {adminQuery} ";
            _spu = m_DapperHelper.QuerySqlFirstOrDefault(_sql, _spu);

            if (_spu != null) {
                _sql = "SELECT * FROM [spu_detail] WHERE [spu_id]=@spu_id";
                _spu.Detail = m_DapperHelper.QuerySqlFirstOrDefault(_sql, new SpuDetail() { Spu_Id = id });

                _sql = "SELECT * FROM [spu_category] WHERE [spu_id]=@spu_id";
                _spu.ProductCategory = m_DapperHelper.QuerySqlFirstOrDefault(_sql, new SpuCategory() { Spu_Id = id });

                _sql = $"SELECT * FROM [spu_logistics] WHERE [spu_id]={id}";
                if (!IsAdmin) _sql += $" AND Enable=1";
                _spu.Logistics = m_DapperHelper.QuerySetSql<SpuLogistics>(_sql).ToList();

                _sql = $"SELECT * FROM [sku] WHERE [spu_id]={id}";
                if (!IsAdmin) _sql += $" AND Enabled=1";
                _spu.SellInfos = m_DapperHelper.QuerySetSql<Sku>(_sql).ToList();

                _sql = $"SELECT * FROM [spu_introduction_image] WHERE [spu_id]={id}";
                if (!IsAdmin) _sql += $" AND DEL=0";
                _spu.IntroductionImage=m_DapperHelper.QuerySetSql<SpuIntroductionImage>(_sql).ToList();
            }

            return _spu;
        }

        /// <summary>
        /// 新增產品
        /// </summary>
        /// <param name="productAdminDto"></param>
        /// <param name="request"></param>
        public void AddProductData(ProductAdminDto productAdminDto, HttpRequest request)
        {
            Spu _spu = m_Mapper.Map<Spu>(productAdminDto);
            //_spu.Logistics = m_Mapper.Map<List<SpuLogistics>>(productAdminInsertDto.Logistics);
           
            SetImageFileName(_spu, request.Files,eMode.Insert);

            SetOnAndOffShelvesSql(out string _ifSql1, out string _ifSql2
                , _spu.Starts_At.Date.Year, _spu.Ends_At.Date.Year
                , _spu.Marketing_Starts_At.Date.Year, _spu.Marketing_Ends_At.Date.Year);

            //1.基本資料(spu) | 7.行銷資訊(sku_marketing)
            string _sql = $@"INSERT INTO [SPU] ([TITLE],[RECOMMEND],[SELL_STOP],[VIEW_STOCK],[VIEW_SELL_NUM],[PRESERVE_STATUS],[PRODUCT_STATUS],    
                            [PRODUCT_COVER],[PRODUCT01],[PRODUCT02],[PRODUCT03],[PRODUCT04],[PRODUCT05],[PRODUCT06],[SPEC],[DESCRIBE],[PRICE],[marketing_title],[enabled]{_ifSql1}) VALUES
                            (@TITLE,@RECOMMEND,@SELL_STOP,@VIEW_STOCK,@VIEW_SELL_NUM,@PRESERVE_STATUS,@PRODUCT_STATUS,@PRODUCT_COVER,@PRODUCT01,
                            @PRODUCT02,@PRODUCT03,@PRODUCT04,@PRODUCT05,@PRODUCT06,@SPEC,@DESCRIBE,@PRICE,@marketing_title,@enabled{_ifSql2})";

            m_DapperHelper.ExecuteSql(_sql, _spu);

            //取得新增商品的Id
            _sql = @"SELECT TOP 1 ID FROM [SPU] ORDER BY [ID] DESC";
            int _spuId = m_DapperHelper.QuerySqlFirstOrDefault<int>(_sql);

            //實作存圖片
            UpdateUploadImage(request.Files, _spuId);

            //2.產品介紹(spu_detail)
            _spu.Detail.Spu_Id = _spuId;
            _sql = @"INSERT INTO [SPU_DETAIL] ([SPU_ID],[TITLE1],[INTRODUCTION1],[TITLE2],[INTRODUCTION2]) VALUES    
                     (@SPU_ID,@TITLE1,@INTRODUCTION1,@TITLE2,@INTRODUCTION2)";
            m_DapperHelper.ExecuteSql(_sql, _spu.Detail);

            //3.產品目錄關連表(spu_category)
            _spu.ProductCategory.Spu_Id = _spuId;
            _sql = @"INSERT INTO [dbo].[spu_category]([spu_id],[cid1],[cid2],[cid3]) VALUES
                    (@spu_id,@cid1,@cid2,@cid3)";
            m_DapperHelper.ExecuteSql(_sql, _spu.ProductCategory);

            //4.運費資訊(spu_logistics)
            _spu.Logistics.ForEach(x => x.Spu_Id = _spuId);
            _sql = @"INSERT INTO [SPU_LOGISTICS] VALUES (@SPU_ID,@CODE,@SHIPPING_FEE,@ENABLE)";
            m_DapperHelper.ExecuteSql(_sql, _spu.Logistics);

            //5.規格表(sku) | 6.庫存(stock) | 7.行銷資訊(sku_marketing)
            _spu.SellInfos.ForEach(x => x.Spu_Id = _spuId);
            _sql = @"INSERT INTO [sku]([spu_id],[title],[sell_price],[enabled],[stock_qty],[start_stock_qty],[safety_stock_qty],[discount_percent],[discount_price]) VALUES (@spu_id,@title,@sell_price,@enabled,@stock_qty,@stock_qty,@safety_stock_qty,@discount_percent,@discount_price)";
            m_DapperHelper.ExecuteSql(_sql, _spu.SellInfos);

            //8.產品特色圖片(spu_introduction_image)[參照關於我們的作法]
            //取得內容_productAdminInsertDto.Detail
            string[] tmps = new String[] { _spu.Detail.Introduction1, _spu.Detail.Introduction2 };
            foreach (string tmp in tmps) {
                string json = tmp;
                //測試用內容
                //json = "{\"ops\":[{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"創辦人賦予年輕的象徵，因伴隨而存在，因存在而依賴。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"希望使用者用了這個品牌後能會心一笑有戀愛的感覺， all代表想將全部最好的用在產品上帶給使用者感受得到，而清潔是我們的專業。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\\n\"},{\"insert\":{\"image\":\"http://tcayla.webshopping.vip/backStage/img/abouts/2021-06-09_14-34-20_591.png\"}},{\"attributes\":{\"align\":\"center\"},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"謝謝親愛的朋友們，體驗CAYLA的香氛產品。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"香氛能使忙碌一天的妳/你感到放鬆，洗完澡會發現皮膚滋潤中帶點彈力。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"如果您使用了泡泡面膜，可能會感到驚訝，因為臉上產生了細緻的泡泡，不過請放心的等待10分鐘， 讓泡沫將髒污帶出，用水清洗後會發現，臉上的毛細孔正在呼吸呢～\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"CAYLA的產品多樣化，每次嘗試都會有不同的感受呢！\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"希望這個產品能讓您，擁有幸福的一天!!\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\"},\"insert\":\"\\n\"}]}";
                try
                {
                    Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
                    foreach (Op o in myDeserializedClass.ops)
                    {
                        string imgstr = $"{o.insert}";
                        if (imgstr.IndexOf("image") > -1)
                        {
                            var img = JsonConvert.DeserializeObject<Img>(imgstr);
                            string[] clearImgurl = img.image.Split('/');
                            _sql = @"INSERT INTO [spu_introduction_image]([image_name],[spu_id]) VALUES (@image_name,@spu_id)";
                            var spu_image = new SpuIntroductionImage()
                            {
                                image_name = clearImgurl[clearImgurl.Length-1],
                                Spu_Id = _spuId
                            };
                            m_DapperHelper.ExecuteSql(_sql, spu_image);
                        }
                    }
                }
                catch(Exception ex)
                {
                    ;
                }
            }
        }

        private void SetOnAndOffShelvesSql(out string ifSql1, out string ifSql2, int startYear, int endYear, int marketingStartYear, int marketingEndYear)
        {
            ifSql1 = string.Empty;
            ifSql2 = string.Empty;

            if (startYear != 1)
            {
                ifSql1 += @",[STARTS_AT]";
                ifSql2 += @",@STARTS_AT";
            }

            if (endYear != 1)
            {
                ifSql1 += @",[ENDS_AT]";
                ifSql2 += @",@ENDS_AT";
            }

            if (marketingStartYear != 1)
            {
                ifSql1 += @",[MARKETING_STARTS_AT]";
                ifSql2 += @",@MARKETING_STARTS_AT";
            }

            if (marketingEndYear != 1)
            {
                ifSql1 += @",[MARKETING_ENDS_AT]";
                ifSql2 += @",@MARKETING_ENDS_AT";
            }
        }

        enum eMode
        {
            Insert,
            Update
        }
        private void SetImageFileName(Spu spu, HttpFileCollection fileCollection,eMode mode)
        {
            if (mode == eMode.Insert)
            {
                for (int i = 0; i < fileCollection.Count; i++)
                {
                    var _file = fileCollection[i];

                    switch (i)
                    {
                        case 0:
                            spu.Product_Cover = _file.FileName;
                            break;
                        case 1:
                            spu.Product01 = _file.FileName;
                            break;
                        case 2:
                            spu.Product02 = _file.FileName;
                            break;
                        case 3:
                            spu.Product03 = _file.FileName;
                            break;
                        case 4:
                            spu.Product04 = _file.FileName;
                            break;
                        case 5:
                            spu.Product05 = _file.FileName;
                            break;
                        case 6:
                            spu.Product06 = _file.FileName;
                            break;
                    }
                }
            }
            else
            {
                foreach (string key in fileCollection.AllKeys)
                {
                    var _file = fileCollection[key];
                    if(_file!=null)// && _file.ContentLength>0
                        switch (key.ToLower())
                        {
                            case "product_cover":
                                spu.Product_Cover = _file.FileName;
                                break;
                            case "product01":
                                spu.Product01 = _file.FileName;
                                break;
                            case "product02":
                                spu.Product02 = _file.FileName;
                                break;
                            case "product03":
                                spu.Product03 = _file.FileName;
                                break;
                            case "product04":
                                spu.Product04 = _file.FileName;
                                break;
                            case "product05":
                                spu.Product05 = _file.FileName;
                                break;
                            case "product06":
                                spu.Product06 = _file.FileName;
                                break;
                        }
                }
            }
        }
        /// <summary>
        /// 更新上傳內容圖片
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public void UpdateUploadImage(HttpFileCollection fileCollection, int spu_Id)
        {
            //取得圖片存放目錄            
            string _rootPath = Path.GetDirectoryName(Path.GetDirectoryName(HttpContext.Current.Server.MapPath("/")));
            string _saveFolderPath = m_ImageFileHelper.GetImageFolderPath(_rootPath, $"{m_imageFolder}{spu_Id}\\");

            //存放上傳圖片檔案
            for (int i = 0; i < fileCollection.Count; i++)
            {
                var _file = fileCollection[i];
                if (_file.ContentLength > 0)
                    m_ImageFileHelper.SaveUploadImageFile(_file, _file.FileName, _saveFolderPath);
            }
        }

        //private string GetSaveImageFolderPath(string productFolder)
        //{
        //    //取得圖片存放目錄
        //    string _rootPath = Path.GetDirectoryName(Path.GetDirectoryName(HttpContext.Current.Server.MapPath("/")));
        //    string _saveFolderPath = m_ImageFileHelper.GetImageFolderPath(_rootPath, $"{m_imageFolder}{productFolder}");

        //    return _saveFolderPath;
        //}

        /// <summary>
        /// 新增圖片
        /// </summary>
        /// <param name="request">內容相關資訊</param>
        /// <returns></returns>
        public string AddImage(HttpRequest request)
        {
            var formData_ = request.Form;

            string strFileName_ = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_fff")}.png";
            string strPathName_ = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Products); //圖片存放路徑

            var file = request.Files[0];
            if (file.ContentLength > 0)
            {
                file.SaveAs($"{strPathName_}{strFileName_}");
            }
            else
            {
                throw new FileNotFoundException();
            }

            string strImage_Link_ = Tools.GetInstance().GetImageLink(Tools.GetInstance().Products, strFileName_); //回傳圖片的URL && 檔名

            return strImage_Link_;
        }

        /// <summary>
        /// 更新單筆產品
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="news">更新的資料類別</param>
        public void UpdateData(ProductAdminDto productAdminDto, Spu original_spu, HttpRequest request)
        {
            Spu _spu = m_Mapper.Map<Spu>(productAdminDto);

            SetImageFileName(_spu, request.Files,eMode.Update);

            string pic = string.Empty;

            //是否更新首圖 
            if (!String.IsNullOrWhiteSpace(_spu.Product_Cover)) pic += ",[PRODUCT_COVER]=@PRODUCT_COVER";
            //是否更新子圖
            //,[PRODUCT01]=@PRODUCT01,[PRODUCT02]=@PRODUCT02,[PRODUCT03]=@PRODUCT03,[PRODUCT04]=@PRODUCT04,[PRODUCT05]=@PRODUCT05,[PRODUCT06]=@PRODUCT06,
            if (_spu.Product01 != null) pic += ",[Product01]=@Product01"; if (request["Product01"] == "null") pic += ",[Product01]=NULL";
            if (_spu.Product02 != null) pic += ",[Product02]=@Product02"; if (request["Product02"] == "null") pic += ",[Product02]=NULL";
            if (_spu.Product03 != null) pic += ",[Product03]=@Product03"; if (request["Product03"] == "null") pic += ",[Product03]=NULL";
            if (_spu.Product04 != null) pic += ",[Product04]=@Product04"; if (request["Product04"] == "null") pic += ",[Product04]=NULL";
            if (_spu.Product05 != null) pic += ",[Product05]=@Product05"; if (request["Product05"] == "null") pic += ",[Product05]=NULL";
            if (_spu.Product06 != null) pic += ",[Product06]=@Product06"; if (request["Product06"] == "null") pic += ",[Product06]=NULL";

            //行銷日期 | 上下架日期
            string dateInfo = string.Empty;

            if (Helpers.Tools.Formatter.IsDate(productAdminDto.Marketing_Starts_At))
                dateInfo += ",[marketing_starts_at]=@marketing_starts_at";
            else
                dateInfo += ",[marketing_starts_at]=NULL";

            if (Helpers.Tools.Formatter.IsDate(productAdminDto.Marketing_Ends_At))
                dateInfo += ",[marketing_ends_at]=@marketing_ends_at";
            else
                dateInfo += ",[marketing_ends_at]=NULL";

            if (Helpers.Tools.Formatter.IsDate(productAdminDto.StartsAt))
                dateInfo += ",[starts_at]=@starts_at";
            else
                dateInfo += ",[starts_at]=NULL";

            if (Helpers.Tools.Formatter.IsDate(productAdminDto.EndsAt))
                dateInfo += ",[ends_at]=@ends_at";
            else
                dateInfo += ",[ends_at]=NULL";



            //1.基本資料(spu) | 7.行銷資訊(sku_marketing)
            string _sql = $@"UPDATE [SPU] SET [TITLE]=@TITLE
            ,[RECOMMEND]=@RECOMMEND,[SELL_STOP]=@SELL_STOP,[VIEW_STOCK]=@VIEW_STOCK,[VIEW_SELL_NUM]=@VIEW_SELL_NUM,[PRESERVE_STATUS]=@PRESERVE_STATUS,[PRODUCT_STATUS]=@PRODUCT_STATUS 
            {pic} 
            ,[SPEC]=@SPEC,[DESCRIBE]=@DESCRIBE,[PRICE]=@PRICE,[marketing_title]=@marketing_title {dateInfo} 
            ,[enabled]=@enabled,updated_at=getdate() 
            WHERE [ID]=@ID";
            m_DapperHelper.ExecuteSql(_sql, _spu);

            //實作存圖片
            UpdateUploadImage(request.Files, _spu.Id);

            //2.產品介紹(spu_detail)
            _spu.Detail.Spu_Id = _spu.Id;
            _sql = @"UPDATE [SPU_DETAIL] SET [TITLE1]=@TITLE1,[INTRODUCTION1]=@INTRODUCTION1,[TITLE2]=@TITLE2,[INTRODUCTION2]=@INTRODUCTION2 
                     WHERE [ID]=@ID";
            m_DapperHelper.ExecuteSql(_sql, _spu.Detail);

            //3.產品目錄關連表(spu_category)
            _spu.ProductCategory.Spu_Id = _spu.Id;
            _sql = @"UPDATE [spu_category] SET [cid1]=@cid1,[cid2]=@cid2,[cid3]=@cid3 
                    WHERE [SPU_ID]=@SPU_ID";
            m_DapperHelper.ExecuteSql(_sql, _spu.ProductCategory);

            //4.運費資訊(spu_logistics)
            _spu.Logistics.ForEach(x => x.Spu_Id = _spu.Id);
            _sql = @"UPDATE [SPU_LOGISTICS] SET [CODE]=@CODE,[SHIPPING_FEE]=@SHIPPING_FEE,[ENABLE]=@ENABLE
                    WHERE [ID]=@ID";
            m_DapperHelper.ExecuteSql(_sql, _spu.Logistics);

            //5.規格表(sku) | 6.庫存(stock) | 7.行銷資訊(sku_marketing)
            _spu.SellInfos.ForEach(x => x.Spu_Id = _spu.Id);
            //刪除(規格表)
            foreach (var item in _spu.SellInfos)
            {
                var matchingRecord = original_spu.SellInfos.Where(x => x.Id == item.Id).FirstOrDefault();
                original_spu.SellInfos.Remove(matchingRecord);
            }
            foreach (var item in original_spu.SellInfos) {
                _sql = $"DELETE [sku] WHERE [ID]=@ID";
                m_DapperHelper.ExecuteSql(_sql, item);
            }

            //新增|修改(規格表)
            _sql = @"IF(@ID=0)
                    INSERT INTO [sku]([spu_id],[title],[sell_price],[enabled],[stock_qty],[start_stock_qty],[safety_stock_qty],[discount_percent],[discount_price]) VALUES (@spu_id,@title,@sell_price,@enabled,@stock_qty,@stock_qty,@safety_stock_qty,@discount_percent,@discount_price)
                    ELSE
                    UPDATE [sku] SET [title]=@title,[sell_price]=@sell_price,[enabled]=@enabled,[safety_stock_qty]=@safety_stock_qty,[discount_percent]=@discount_percent,[discount_price]=@discount_price,updated_at=getdate() WHERE [ID]=@ID";
            m_DapperHelper.ExecuteSql(_sql, _spu.SellInfos);





            //8.產品特色圖片(spu_introduction_image)[參照關於我們的作法]
            //取得內容_productAdminInsertDto.Detail
            m_DapperHelper.ExecuteSql($"UPDATE spu_introduction_image SET DEL=1 WHERE SPU_ID={_spu.Id}");//先標註待刪除
            string[] tmps = new String[] { _spu.Detail.Introduction1, _spu.Detail.Introduction2 };
            foreach (string tmp in tmps)
            {
                string json = tmp;
                //測試用內容
                //json = "{\"ops\":[{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"創辦人賦予年輕的象徵，因伴隨而存在，因存在而依賴。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"希望使用者用了這個品牌後能會心一笑有戀愛的感覺， all代表想將全部最好的用在產品上帶給使用者感受得到，而清潔是我們的專業。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\\n\"},{\"insert\":{\"image\":\"http://tcayla.webshopping.vip/backStage/img/abouts/2021-06-09_14-34-20_591.png\"}},{\"attributes\":{\"align\":\"center\"},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"謝謝親愛的朋友們，體驗CAYLA的香氛產品。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"香氛能使忙碌一天的妳/你感到放鬆，洗完澡會發現皮膚滋潤中帶點彈力。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"如果您使用了泡泡面膜，可能會感到驚訝，因為臉上產生了細緻的泡泡，不過請放心的等待10分鐘， 讓泡沫將髒污帶出，用水清洗後會發現，臉上的毛細孔正在呼吸呢～\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"CAYLA的產品多樣化，每次嘗試都會有不同的感受呢！\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"希望這個產品能讓您，擁有幸福的一天!!\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\"},\"insert\":\"\\n\"}]}";
                try
                {
                    Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
                    foreach (Op o in myDeserializedClass.ops)
                    {
                        string imgstr = $"{o.insert}";
                        if (imgstr.IndexOf("image") > -1)
                        {
                            var img = JsonConvert.DeserializeObject<Img>(imgstr);
                            string[] clearImgurl = img.image.Split('/');
                            _sql = @"IF NOT EXISTS(SELECT * FROM spu_introduction_image WHERE image_name=@image_name AND spu_id=@spu_id)
                                    INSERT INTO [spu_introduction_image]([image_name],[spu_id]) VALUES (@image_name,@spu_id)
                                    ELSE
                                    UPDATE [spu_introduction_image] SET DEL=0 WHERE image_name=@image_name AND spu_id=@spu_id";
                            var spu_image = new SpuIntroductionImage()
                            {
                                Spu_Id=_spu.Id,
                                image_name = clearImgurl[clearImgurl.Length - 1]
                            };
                            m_DapperHelper.ExecuteSql(_sql, spu_image);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ;
                }
            }
            //刪除前，清除實體檔
            //取得這筆消息相關內容圖片檔名
            List<string> _contentImageNameList = m_DapperHelper.QuerySetSql<SpuIntroductionImage, string>($"SELECT image_name FROM spu_introduction_image WHERE SPU_ID={_spu.Id} AND DEL=1",new SpuIntroductionImage()).ToList();
            //取得圖片存放目錄            
            string _saveFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Products);
            string _contentImageFilePath = string.Empty;
            for (int i = 0; i < _contentImageNameList.Count; i++)
            {
                _contentImageFilePath = Path.Combine(_saveFolderPath, _contentImageNameList[i]);

                if (File.Exists(_contentImageFilePath))
                {
                    File.Delete(_contentImageFilePath);
                }
            }
            m_DapperHelper.ExecuteSql($"DELETE spu_introduction_image WHERE SPU_ID={_spu.Id} AND DEL=1");//刪除標註


        }

        /// <summary>
        /// 刪除圖片
        /// </summary>
        /// <param name="request">內容相關資訊</param>
        /// <returns></returns>
        public int DeleteImage(SpuIntroductionImage _spu)
        {
            int result = 0;

            //取得圖片存放目錄            
            string _saveFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Products);

            //刪除內容圖檔
            string _contentImageFilePath = Path.Combine(_saveFolderPath, _spu.image_name);

            if (File.Exists(_contentImageFilePath))
            {
                File.Delete(_contentImageFilePath);
            }
            string sql = "DELETE [spu_introduction_image] WHERE image_name=@image_name AND spu_id=@spu_id";
            result = m_DapperHelper.ExecuteSql(sql, _spu);

            return result;
        }

        /// <summary>
        /// 刪除一筆產品和圖片檔案
        /// </summary>
        /// <param name="id">消息ID</param>
        public void DeleteProductData(Spu spu)
        {
            //取得圖片存放目錄            
            string _saveFolderPath = Tools.GetInstance().GetImagePathName(Tools.GetInstance().Products);

            //取得這筆消息相關內容圖片檔名
            List<string> _contentImageNameList = new List<string>()
            {
                $@"{spu.Id}\{spu.Product_Cover}",
                $@"{spu.Id}\{spu.Product01}",$@"{spu.Id}\{spu.Product02}",$@"{spu.Id}\{spu.Product03}",$@"{spu.Id}\{spu.Product04}",$@"{spu.Id}\{spu.Product05}",$@"{spu.Id}\{spu.Product06}"
            };

            var list = spu.IntroductionImage;
            foreach (var img in list) {
                _contentImageNameList.Add(img.image_name);
            }

            string _contentImageFilePath = string.Empty;

            //刪除內容圖檔
            for (int i = 0; i < _contentImageNameList.Count; i++)
            {
                _contentImageFilePath = Path.Combine(_saveFolderPath, _contentImageNameList[i]);

                if (File.Exists(_contentImageFilePath))
                {
                    File.Delete(_contentImageFilePath);
                }
            }

            string fold = Path.Combine(_saveFolderPath, spu.Id.ToString());
            if (Directory.Exists(fold))
            {
                Directory.Delete(fold, true);
            }

            string _sql = @"DELETE FROM [SPU] WHERE ID = @ID";

            m_DapperHelper.ExecuteSql(_sql, spu);
        }

        /// <summary>
        /// 會員對產品提出問題
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public int AddQuestion(ProductQuestion question)
        {
            string _sql = $@"Insert into [spu_qna] (spu_id,member_id,quection,Is_View)
                            Values(@spu_id,@member_id,@quection,0)";
            int i = m_DapperHelper.ExecuteSql(_sql, question);
            return i;
        }

        /// <summary>
        /// 管理者回覆產品的問題
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public int Answer(ProductAnswer answer)
        {
            string _sql = $@"Update [spu_qna] SET manager_id=@manager_id,answer=@answer,Is_View=@Is_View,updated_at=getdate()
                            Where id=@id";
            int i = m_DapperHelper.ExecuteSql(_sql, answer);
            return i;
        }

        /// <summary>
        /// 取得產品問答
        /// </summary>
        /// <param name="member_id"></param>
        /// <param name="spu_id"></param>
        /// <param name="Is_View">前台產品下看問答時，Is_View要傳1</param>
        /// <returns></returns>
        public List<Spu_QnA> GetProductsQnA(Guid? member_id, int? spu_id, bool Is_View)
        {
            List<Spu_QnA> qnAs = new List<Spu_QnA>();
            QueryProductAnswer query = new QueryProductAnswer()
            {
                member_id = member_id,
                spu_id = spu_id,
                Is_View= Is_View
            };

            string sql_ = $@"SELECT s.*,m.account FROM [spu_qna] s left join member m on s.member_id=m.id Where s.Id>0 ";
            if (member_id != null)
                sql_ += " And member_id=@member_id";//會員查看自己的問題
            if (spu_id != null)
                sql_ += " And spu_id=@spu_id";//管理者/會員查看某產品問題
            if(Is_View)
                sql_ += " And Is_View=@Is_View";//前台產品下看問答時，Is_View要傳1

            sql_ += " Order by s.id desc";

            qnAs = m_DapperHelper.QuerySetSql<QueryProductAnswer, Spu_QnA>(sql_, query).ToList();

            return qnAs;
        }
    }
}