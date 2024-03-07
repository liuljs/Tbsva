using AutoMapper;
using WebShopping.Auth;
using WebShopping.Dtos;
using WebShopping.Models;
using WebShopping.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebShopping.Controllers
{
    /// <summary>
    /// 產品類別管理
    /// </summary>
    [AllowAnonymous]
    public class CategoryController : ApiController
    {
        private ICategoryService m_CategoryService;

        private IMapper m_Mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            m_CategoryService = categoryService;
            m_Mapper = mapper;
        }

        public IHttpActionResult Get(int id)
        {
            return Ok();//return NotFound();
        }

        public IHttpActionResult GetCategorys()
        {
            List<Category> _categories = m_CategoryService.GetCategorts(false);

            if (_categories.Count > 0)
            {
                List<CategoryGetDto> _categoryGetDtos = m_Mapper.Map<List<CategoryGetDto>>(_categories); // 轉換型別

                return Ok(_categoryGetDtos);
            }
            else
            {
                return Ok();//return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult AddMainCategory([FromBody] MainCategoryInsertDto categoryDto)
        {
            if (ModelState.IsValid)
            {
                Category _category = new Category();
                _category.Name = categoryDto.Name.Trim();
                _category.Enable = OpenStatus.Close;

                if (m_CategoryService.ExistSameName(_category, "[CATEGORY]") == true)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "其它主類別已經有相同名稱"));
                }

                m_CategoryService.AddMainCategory(_category);

                return StatusCode(HttpStatusCode.Created);               
            }
            else
            {
                var _errMsg = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Where(x => !string.IsNullOrEmpty(x.ErrorMessage))
                                    .Select(x => x.ErrorMessage));
                return Content(HttpStatusCode.BadRequest, _errMsg);
            }
        }

    }
}
