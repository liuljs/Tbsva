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
    [CustomAuthorize(Role.Admin)]
    public class CategoryAdminController : ApiController
    {
        private ICategoryService m_CategoryService;

        private IMapper m_Mapper;

        public CategoryAdminController(ICategoryService categoryService, IMapper mapper)
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
            List<Category> _categories = m_CategoryService.GetCategorts(true);

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

        [HttpPost]
        [Route("CategoryAdmin/EditFirstCategory")]
        public IHttpActionResult EditFirstCategory([FromBody] EditCategoryDto editCategoryDto)
        {
            if (editCategoryDto.SubCategories.Count == 0)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "請至少設定一個次分類"));
            }

            if (ModelState.IsValid)
            {
                Category _category = m_Mapper.Map<Category>(editCategoryDto);
                _category.Updated_At = DateTime.Now;
                string _categoryTableName = "[CATEGORY]";
                string _subCategoryTableName = "[SUBCATEGORY1]";

                //判斷主類別是否已經有相同名稱
                if (m_CategoryService.ExistSameName(_category, _categoryTableName, true) == true)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "其它主類別已經有相同名稱"));
                }

                if (m_CategoryService.ExistId(_category, _categoryTableName) == false)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "主類別的ID不存在"));
                }

                var _isSameSubCategoryName = editCategoryDto.SubCategories.Select(x => x.Name.Trim()).GroupBy(y => y).Any(z => z.Count() > 1);

                if (_isSameSubCategoryName)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "參數次類別有相同名稱"));
                }              

                for (int i = 0; i < editCategoryDto.SubCategories.Count; i++)
                {
                    EditSubCategoryDto _editFirstCategoryDto = editCategoryDto.SubCategories[i];

                    if (_editFirstCategoryDto.EditAction != ActionStatus.Delete)
                    {
                        bool _isUpdate = _editFirstCategoryDto.EditAction == ActionStatus.Update ? true : false;

                        _editFirstCategoryDto.Name = _editFirstCategoryDto.Name.Trim();
                        //判斷次類別是否已經有相同名稱
                        if (m_CategoryService.ExistSameName(_editFirstCategoryDto, _subCategoryTableName, _isUpdate) == true)
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "其它次類別已經有相同名稱"));
                        }
                    }
                }

                m_CategoryService.UpdateMainCategory(_category); //更新主類別資料

                for (int i = 0; i < editCategoryDto.SubCategories.Count; i++)
                {
                    EditSubCategoryDto _editFirstCategoryDto = editCategoryDto.SubCategories[i];
                    SubCategory1 _subCategory1 = m_Mapper.Map<SubCategory1>(_editFirstCategoryDto);

                    switch (_editFirstCategoryDto.EditAction)
                    {
                        case ActionStatus.Insert:
                            _subCategory1.Parent_id = _category.Id;
                            m_CategoryService.AddSubCategory(_subCategory1, _subCategoryTableName);
                            break;
                        case ActionStatus.Update:
                            _subCategory1.Updated_At = DateTime.Now;
                            m_CategoryService.UpdateSubCategory(_subCategory1, _subCategoryTableName);
                            break;
                        case ActionStatus.Delete:
                            m_CategoryService.DeleteCategory(_subCategory1, _subCategoryTableName);
                            break;
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);
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

        [HttpPost]
        [Route("CategoryAdmin/EditSecondCategory")]
        public IHttpActionResult EditSecondCategory([FromBody] EditFirstCategoryDto editFirstCategoryDto)
        {
            if (ModelState.IsValid)
            {               
                string _subCategory1TableName = "[SUBCATEGORY1]";
                string _subCategory2TableName = "[SUBCATEGORY2]";

                if (m_CategoryService.ExistId(editFirstCategoryDto, _subCategory1TableName) == false)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "次類別的ID不存在"));
                }
              
                var _isSameSubCategoryName = editFirstCategoryDto.SubCategories.Select(x => x.Name.Trim()).GroupBy(y => y).Any(z => z.Count() > 1);

                if (_isSameSubCategoryName)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "參數次類別有相同名稱"));
                }

                for (int i = 0; i < editFirstCategoryDto.SubCategories.Count; i++)
                {
                    EditSubCategoryDto _editSecondCategoryDto = editFirstCategoryDto.SubCategories[i];

                    if (_editSecondCategoryDto.EditAction != ActionStatus.Delete)
                    {
                        bool _isUpdate = _editSecondCategoryDto.EditAction == ActionStatus.Update ? true : false;

                        _editSecondCategoryDto.Name = _editSecondCategoryDto.Name.Trim();
                        //判斷子分類是否已經有相同名稱
                        if (m_CategoryService.ExistSameName(_editSecondCategoryDto, _subCategory2TableName, _isUpdate) == true)
                        {
                            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "其它子分類已經有相同名稱"));
                        }
                    }
                }

                for (int i = 0; i < editFirstCategoryDto.SubCategories.Count; i++)
                {
                    EditSubCategoryDto _editSecondCategoryDto = editFirstCategoryDto.SubCategories[i];
                    SubCategory2 _subCategory2 = m_Mapper.Map<SubCategory2>(_editSecondCategoryDto);

                    switch (_editSecondCategoryDto.EditAction)
                    {
                        case ActionStatus.Insert:
                            _subCategory2.Parent_id = editFirstCategoryDto.Id;
                            m_CategoryService.AddSubCategory(_subCategory2, _subCategory2TableName);
                            break;
                        case ActionStatus.Update:
                            _subCategory2.Updated_At = DateTime.Now;
                            m_CategoryService.UpdateSubCategory(_subCategory2, _subCategory2TableName);
                            break;
                        case ActionStatus.Delete:
                            m_CategoryService.DeleteCategory(_subCategory2, _subCategory2TableName);
                            break;
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);
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
      
        [HttpDelete]
        public IHttpActionResult DeleteMainCategory(int id)
        {
            Category _category = new Category();
            _category.Id = id;
            string _categoryTableName = "[CATEGORY]";

            if (m_CategoryService.ExistId(_category, _categoryTableName) == false)
            {
                return Ok();//return NotFound();
            }

            m_CategoryService.DeleteCategory(_category, _categoryTableName);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("CategoryAdmin/DeleteSubCategory/{id}")]
        public IHttpActionResult DeleteSubCategory(int id)
        {
            SubCategory1 _subCategory1 = new SubCategory1();
            _subCategory1.Id = id;
            string _subCategory1TableName = "[SUBCATEGORY1]";

            if (m_CategoryService.ExistId(_subCategory1, _subCategory1TableName) == false)
            {
                return Ok();//return NotFound();
            }

            m_CategoryService.DeleteCategory(_subCategory1, _subCategory1TableName);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
