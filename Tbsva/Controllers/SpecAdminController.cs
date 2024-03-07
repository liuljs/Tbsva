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
    /// 後台規格管理
    /// </summary>
    [CustomAuthorize(Role.Admin)]
    public class SpecAdminController : ApiController
    {
        ISpecService m_SpecService;

        private IMapper m_Mapper;

        //private IImageFormatHelper m_ImageFormatHelper;

        public SpecAdminController(ISpecService SpecService, IMapper mapper)
        {
            m_SpecService = SpecService;
            m_Mapper = mapper;
            //m_ImageFormatHelper = imageFormatHelper;
        }

        // GET: api/SpecAdmin
        [HttpGet]
        [Route("SpecAdmin/GetAll/{spu_id}")]
        public IHttpActionResult GetAll(int spu_id)
        {
            var _specs = m_SpecService.GetSpecs(spu_id,true);
            return Ok(_specs);
        }

        // GET: api/SpecAdmin/5
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            Sku _spec = m_SpecService.GetSpecInfo(id, true);

            if (_spec != null)
            {
                return Ok(_spec);
            }
            else
            {
                return Ok();//return NotFound();
            }           
        }


        /// <summary>
        /// 新增多筆規格
        /// </summary>
        /// <param name="skus"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Add(List<Sku> skus)
        {
            try
            {
                int i = m_SpecService.AddData(skus);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, ""));
            }
            catch (Exception ex)
            {
                Helpers.SystemFunctions.WriteLogFile($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\n{ex.Message}\n{ex.StackTrace}");
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// 更新多筆規格
        /// </summary>
        /// <param name="skus"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult Update(List<Sku> skus)
        {
            try
            {
                int i = m_SpecService.UpdateData(skus);

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                Helpers.SystemFunctions.WriteLogFile($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\n{ex.Message}\n{ex.StackTrace}");
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpDelete]
        // DELETE: api/SpecAdmin/5
        public IHttpActionResult Delete(int id)
        {
            int i = m_SpecService.DeleteData(id);
            return StatusCode(HttpStatusCode.NoContent);
        }



    }
}
