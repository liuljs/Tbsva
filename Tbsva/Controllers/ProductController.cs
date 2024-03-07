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

namespace WebShopping.Controllers
{
    /// <summary>
    /// 前台產品功能
    /// </summary>
    [CustomAuthorize(Role.User)]
    public class ProductController : ApiController
    {
        IProductService m_ProductService;

        private IMapper m_Mapper;

        private IImageFormatHelper m_ImageFormatHelper;

        public ProductController(IProductService productService, IMapper mapper, IImageFormatHelper imageFormatHelper)
        {
            m_ProductService = productService;
            m_Mapper = mapper;
            m_ImageFormatHelper = imageFormatHelper;
        }

        // GET: api/Product
        [AllowAnonymous]
        [HttpPost]
        [Route("Product/GetProducts")]
        public IHttpActionResult Get(QueryProduct query)
        {
            var _spus = m_ProductService.GetProducts(query);
            return Ok(SwitchSpuData(_spus));
        }

        // GET: api/Product/5
        [AllowAnonymous]
        public IHttpActionResult Get(int id)
        {
            Spu _spu = m_ProductService.GetProductInfo(id, false);

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

        /// <summary>
        /// 會員提出產品問題
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Product/AddQuestion")]
        public IHttpActionResult AddQuestion(ProductQuestion question)
        {
            question.member_id = new Guid(User.Identity.Name);
            int i = m_ProductService.AddQuestion(question);
            return Ok();
        }

        /// <summary>
        /// 訪客查看產品上的QnA
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Product/GetProductsQnA")]
        [AllowAnonymous]
        public IHttpActionResult GetProductsQnA(QueryProductAnswer query)
        {
            var spu_id = query.spu_id;
            if (spu_id == null) return Ok("產品號為必填");
            List<Spu_QnA> result = m_ProductService.GetProductsQnA(null, spu_id, true);
            return Ok(result);
        }

        /// <summary>
        /// 會員查自己問過的QnA
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Product/GetMamberQnA")]
        public IHttpActionResult GetMamberQnA()
        {
            var member_id = new Guid(User.Identity.Name);
            List<Spu_QnA> result = m_ProductService.GetProductsQnA(member_id, null, false);
            return Ok(result);
        }

        private List<SpuGetDto> SwitchSpuData(List<Spu> spus)
        {
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

    }
}
