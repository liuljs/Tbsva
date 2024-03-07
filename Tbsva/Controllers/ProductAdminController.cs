using AutoMapper;
using WebShopping.Dtos;
using WebShopping.Helpers;
using WebShopping.Models;
using WebShopping.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using WebShopping.Auth;
using System.Text;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 後台產品管理
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    public class ProductAdminController : ApiController
    {
        IProductService m_ProductService;

        private IMapper m_Mapper;

        private IImageFormatHelper m_ImageFormatHelper;

        public ProductAdminController(IProductService productService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            m_ProductService = productService;
            m_Mapper = mapper;
            m_ImageFormatHelper = imageFormatHelper;
        }

        // GET: api/ProductAdmin
        /// <summary>
        /// 取得所有產品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ProductAdmin/GetProducts")]
        public IHttpActionResult Get(QueryProduct query)
        {
            //HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件
            //string cond = _request["cond"];
            ////1.已上架,2.已售完,3.停售,4.未上架

            //int? count = null;
            //if(!string.IsNullOrWhiteSpace(_request["count"]))count = Convert.ToInt32(_request["count"]);

            //int? page = null;
            //if (!string.IsNullOrWhiteSpace(_request["page"])) page = Convert.ToInt32(_request["page"]);

            //var _spus = m_ProductService.GetProductsAdmin(cond, count, page);
            var _spus = m_ProductService.GetProductsAdmin(query);
            return Ok(SwitchSpuData(_spus));
        }

        ///// <summary>
        ///// 取得所有產品
        ///// </summary>
        ///// <param name="cond"></param>
        ///// <param name="count"></param>
        ///// <param name="page"></param>
        ///// <returns></returns>
        //public IHttpActionResult Get()
        //{
        //    //已上架
        //    //已售完
        //    //停售
        //    //未上架
        //    var _spus = m_ProductService.GetProductsAdmin(cond, count, page);
        //    return Ok(SwitchSpuData(_spus));
        //}

        // GET: api/ProductAdmin/5
        /// <summary>
        /// 取得一筆指定產品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Get(int id)
        {
            Spu _spu = m_ProductService.GetProductInfo(id, true);

            if (_spu != null)
            {
                SpuGetDto _spuGetDto = SwitchSpuData(_spu);
                return Ok(_spuGetDto);
            }
            else
            {
                return Ok();//return NotFound();
            }           
        }

        private List<SpuGetDto> SwitchSpuData(List<Spu> spus) {
            List<SpuGetDto> _spuGetDtos = new List<SpuGetDto>();
            foreach (Spu spu in spus)
                _spuGetDtos.Add((SwitchSpuData(spu)));
            return _spuGetDtos;

        }
        private SpuGetDto SwitchSpuData(Spu spu)
        {
            SpuGetDto _spuGetDto = m_Mapper.Map<SpuGetDto>(spu);
            return _spuGetDto;
        }

        /// <summary>
        /// 將新增產品的內容，初始化成物件
        /// </summary>
        /// <param name="_request"></param>
        /// <returns></returns>
        private ProductAdminDto InitFormdata(HttpRequest _request)
        {
            ProductAdminDto _productAdminInsertDto = new ProductAdminDto();
            if (String.IsNullOrEmpty(_request.Form.Get("data")))
            {//form-data
                _productAdminInsertDto.Title = string.IsNullOrWhiteSpace(_request.Form.Get("title")) ? string.Empty : _request.Form.Get("title");
                _productAdminInsertDto.Enabled = string.IsNullOrWhiteSpace(_request.Form.Get("Enabled")) ? (byte)0 : Convert.ToByte(_request.Form.Get("Enabled"));
                _productAdminInsertDto.Recommend = int.TryParse(_request.Form.Get("recommend"), out int _recommend) ? _recommend : 0;
                _productAdminInsertDto.SellStop = int.TryParse(_request.Form.Get("sellStop"), out int _sellStop) ? _sellStop : 0;
                _productAdminInsertDto.ViewStock = int.TryParse(_request.Form.Get("viewStock"), out int _viewStock) ? _viewStock : 0;
                _productAdminInsertDto.ViewSellNum = int.TryParse(_request.Form.Get("viewSellNum"), out int _viewSellNum) ? _viewSellNum : 0;
                _productAdminInsertDto.PreserveStatus = int.TryParse(_request.Form.Get("preserveStatus"), out int _preserveStatus) ? _preserveStatus : -1;
                _productAdminInsertDto.ProductStatus = int.TryParse(_request.Form.Get("productStatus"), out int _productStatus) ? _productStatus : 0;
                _productAdminInsertDto.StartsAt = string.IsNullOrWhiteSpace(_request.Form.Get("startsAt")) ? string.Empty : _request.Form.Get("startsAt");
                _productAdminInsertDto.EndsAt = string.IsNullOrWhiteSpace(_request.Form.Get("endsAt")) ? string.Empty : _request.Form.Get("endsAt");
                _productAdminInsertDto.Spec = string.IsNullOrWhiteSpace(_request.Form.Get("spec")) ? string.Empty : _request.Form.Get("spec");
                _productAdminInsertDto.Describe = string.IsNullOrWhiteSpace(_request.Form.Get("describe")) ? string.Empty : _request.Form.Get("describe");
                _productAdminInsertDto.Price = int.TryParse(_request.Form.Get("price"), out int _price) ? _price : -1;

                //產品規格|庫存|行銷
                string _sellInfos = string.IsNullOrWhiteSpace(_request.Form.Get("SellInfos")) ? string.Empty : _request.Form.Get("SellInfos");
                //json 轉規格表(+行銷折價)
                _productAdminInsertDto.SellInfos = JsonConvert.DeserializeObject<List<SellInfoInsertDto>>(_sellInfos);
                //_productAdminInsertDto.SellInfos = new List<SellInfoInsertDto>() {
                //    new SellInfoInsertDto(){ Title="規格1",SellPrice=90,Enabled=1,StockQty=50,SafetyStockQty=0,DiscountPercent=0,DiscountPrice=0 }
                //    ,new SellInfoInsertDto(){ Title="規格2",SellPrice=130,Enabled=1,StockQty=80,SafetyStockQty=0,DiscountPercent=0,DiscountPrice=0 }
                //};

                //行銷資訊
                _productAdminInsertDto.Marketing_Title = string.IsNullOrWhiteSpace(_request.Form.Get("marketing_Title")) ? string.Empty : _request.Form.Get("marketing_Title");
                _productAdminInsertDto.Marketing_Starts_At = string.IsNullOrWhiteSpace(_request.Form.Get("marketing_Starts_At")) ? string.Empty : _request.Form.Get("marketing_Starts_At");
                _productAdminInsertDto.Marketing_Ends_At = string.IsNullOrWhiteSpace(_request.Form.Get("marketing_Ends_At")) ? string.Empty : _request.Form.Get("marketing_Ends_At");
                //string _marketingInfo = string.IsNullOrWhiteSpace(_request.Form.Get("marketingInfo")) ? string.Empty : _request.Form.Get("marketingInfo");

                //json 轉產品介紹(spu_detail)
                string _Detail = string.IsNullOrWhiteSpace(_request.Form.Get("Detail")) ? string.Empty : _request.Form.Get("Detail");
                _productAdminInsertDto.Detail = JsonConvert.DeserializeObject<DetailInsertDto>(_Detail);

                //json 轉產品分類
                string _productCategory = string.IsNullOrWhiteSpace(_request.Form.Get("productCategory")) ? string.Empty : _request.Form.Get("productCategory");
                _productAdminInsertDto.ProductCategory = JsonConvert.DeserializeObject<ProductCategoryInsertDto>(_productCategory);

                //json 轉運費資料
                string _logistics = string.IsNullOrWhiteSpace(_request.Form.Get("logistics")) ? string.Empty : _request.Form.Get("logistics");
                _productAdminInsertDto.Logistics = JsonConvert.DeserializeObject<List<LogisticsInsertDto>>(_logistics);

                //TODO:抓取轉json結果
                //_productAdminInsertDto.Detail.Introduction1 = "{\"ops\":[{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"創辦人賦予年輕的象徵，因伴隨而存在，因存在而依賴。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"希望使用者用了這個品牌後能會心一笑有戀愛的感覺， all代表想將全部最好的用在產品上帶給使用者感受得到，而清潔是我們的專業。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\\n\"},{\"insert\":{\"image\":\"http://tcayla.webshopping.vip/backStage/img/abouts/2021-06-09_14-34-20_591.png\"}},{\"attributes\":{\"align\":\"center\"},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"謝謝親愛的朋友們，體驗CAYLA的香氛產品。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"香氛能使忙碌一天的妳/你感到放鬆，洗完澡會發現皮膚滋潤中帶點彈力。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"如果您使用了泡泡面膜，可能會感到驚訝，因為臉上產生了細緻的泡泡，不過請放心的等待10分鐘， 讓泡沫將髒污帶出，用水清洗後會發現，臉上的毛細孔正在呼吸呢～\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"CAYLA的產品多樣化，每次嘗試都會有不同的感受呢！\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"希望這個產品能讓您，擁有幸福的一天!!\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\"},\"insert\":\"\\n\"}]}";
                //_productAdminInsertDto.Detail.Introduction2 = "{\"ops\":[{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"創辦人賦予年輕的象徵，因伴隨而存在，因存在而依賴。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"希望使用者用了這個品牌後能會心一笑有戀愛的感覺， all代表想將全部最好的用在產品上帶給使用者感受得到，而清潔是我們的專業。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\\n\"},{\"insert\":{\"image\":\"http://tcayla.webshopping.vip/backStage/img/abouts/2021-06-09_14-34-20_591.png\"}},{\"attributes\":{\"align\":\"center\"},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"謝謝親愛的朋友們，體驗CAYLA的香氛產品。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"attributes\":{\"color\":\"#333333\"},\"insert\":\"香氛能使忙碌一天的妳/你感到放鬆，洗完澡會發現皮膚滋潤中帶點彈力。\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"如果您使用了泡泡面膜，可能會感到驚訝，因為臉上產生了細緻的泡泡，不過請放心的等待10分鐘， 讓泡沫將髒污帶出，用水清洗後會發現，臉上的毛細孔正在呼吸呢～\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"CAYLA的產品多樣化，每次嘗試都會有不同的感受呢！\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\",\"header\":5},\"insert\":\"\\n\"},{\"insert\":\"希望這個產品能讓您，擁有幸福的一天!!\"},{\"attributes\":{\"align\":\"center\",\"header\":2},\"insert\":\"\\n\"},{\"attributes\":{\"align\":\"center\"},\"insert\":\"\\n\"}]}";

                Helpers.SystemFunctions.WriteLogFile($"\n\n{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}收到Form-data\n{Helpers.SystemFunctions.GetFormData(_request)}");

                string json = JsonConvert.SerializeObject(_productAdminInsertDto);

                //TODO:測用轉型用
                //var m = m_Mapper.Map<Spu>(_productAdminInsertDto);
            }
            else
            {//json
                Helpers.SystemFunctions.WriteLogFile($"\n\n{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}收到json-data\n{_request.Form.Get("data")}");
                _productAdminInsertDto = JsonConvert.DeserializeObject<ProductAdminDto>(_request.Form.Get("data"));
            }

            return _productAdminInsertDto;
        }

        /// <summary>
        /// 新增產品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AddNewProduct()
        {
            try
            {
                HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件

                ProductAdminDto _productAdminInsertDto = InitFormdata(_request);

                m_ProductService.AddProductData(_productAdminInsertDto, _request);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, ""));
            }
            catch (Exception ex)
            {
                Helpers.SystemFunctions.WriteLogFile($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\n{ex.Message}\n{ex.StackTrace}");
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
      
        /// <summary>
        /// 更新一筆產品
        /// </summary>
        /// <param name="id">產品ID</param>
        /// <returns>204</returns>
        [HttpPut]
        public IHttpActionResult Update(int id)
        {
            try
            {
                Spu _spu = m_ProductService.GetProductInfo(id, true);

                if (_spu != null)
                {
                    HttpRequest _request = HttpContext.Current.Request; //取得使用者要求的Request物件

                    ProductAdminDto _productAdminDto = InitFormdata(_request);
                    _productAdminDto.Detail.Id = _spu.Detail.Id;
                    _productAdminDto.Id = id;
                    m_ProductService.UpdateData(_productAdminDto, _spu, _request);

                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return Ok();//return NotFound();
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// 刪除一筆產品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        // DELETE: api/ProductAdmin/5
        public IHttpActionResult Delete(int id)
        {
            Spu _spu = m_ProductService.GetProductInfo(id, true);

            if (_spu != null)
            {
                m_ProductService.DeleteProductData(_spu);

                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return Ok();//return NotFound();
            }
        }

        [Route("ProductAdmin/DeleteImage")]
        [HttpDelete]
        // DELETE: api/ProductAdmin/5
        public IHttpActionResult DeleteImage()
        {
            SpuIntroductionImage _spu = new SpuIntroductionImage()
            {
                Spu_Id = Convert.ToInt32(HttpContext.Current.Request.Form.Get("Spu_Id")),
                image_name = HttpContext.Current.Request.Form.Get("image_name")
            };

            if (_spu != null)
            {
                m_ProductService.DeleteImage(_spu);

                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return Ok();//return NotFound();
            }
        }

        [Route("ProductAdmin/AddImage")]
        [HttpPost]
        /// <summary>
        /// 新增產品說明的圖片(1筆) 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IHttpActionResult AddImage()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            HttpRequest _request = HttpContext.Current.Request;

            if (_request.Files.Count == 0)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "上傳圖片檔案"));
            }

            if (m_ImageFormatHelper.CheckImageMIME(_request.Files) == false)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "圖片格式錯誤"));
            }

            try
            {
                var _result = m_ProductService.AddImage(_request); //新增圖片
                SendProductAddImageDto dto_ = ReturnAddImageItmes(_result);
                //return Ok(dto_);
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, dto_));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// 轉換成AddImage回傳的類別
        /// </summary>
        /// <param name="p_strImgLink"></param>
        /// <returns></returns>
        private SendProductAddImageDto ReturnAddImageItmes(string p_strImgLink)
        {
            SendProductAddImageDto dto_ = new SendProductAddImageDto();
            dto_.Image_Link = p_strImgLink;

            return dto_;
        }

        /// <summary>
        /// 管理者回覆產品問答
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("ProductAdmin/Answer")]
        public IHttpActionResult Answer(ProductAnswer answer)
        {
            answer.manager_id = new Guid(User.Identity.Name);
            int i = m_ProductService.Answer(answer);
            return Ok();
        }

        /// <summary>
        /// 管理者查看產品的QnA
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ProductAdmin/GetProductsQnA")]
        public IHttpActionResult GetProductsQnA(QueryProductAnswer query)
        {
            var spu_id = query.spu_id;
            List<Spu_QnA> qnAs = m_ProductService.GetProductsQnA(null, spu_id, false);
            List<Spu_QnA_Send> result = m_Mapper.Map<List<Spu_QnA_Send>>(qnAs);//轉換型別
            return Ok(result);
        }

    }
}
