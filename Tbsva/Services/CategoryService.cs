using WebShopping.Connection;
using WebShopping.Helpers;
using WebShopping.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebShopping.Services
{
    public class CategoryService : ICategoryService
    {
        IDapperHelper m_DapperHelper;

        public CategoryService(IDapperHelper dapperHelper)
        {
            m_DapperHelper = dapperHelper;
        }

        public List<Category> GetCategorts(bool IsAdmin)
        {
            List<Category> _categories = new List<Category>();

            string _sql = @"SELECT * FROM [CATEGORY] A
                            LEFT JOIN [SUBCATEGORY1] B
                            ON A.[ID] = B.[PARENT_ID] And B.enable=1
                            LEFT JOIN [SUBCATEGORY2] C
                            ON B.[ID] = C.[PARENT_ID] And C.enable=1
                            Where A.Enable=1"; 
            if (IsAdmin) {
                _sql = @"SELECT * FROM [CATEGORY] A
                            LEFT JOIN [SUBCATEGORY1] B
                            ON A.[ID] = B.[PARENT_ID]
                            LEFT JOIN [SUBCATEGORY2] C
                            ON B.[ID] = C.[PARENT_ID] ";
            }

            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    var _categoryDictionary = new Dictionary<int, Category>();

                    var _subcategoryDictionary = new Dictionary<int, SubCategory1>();

                    _categories = _cn.Query<Category, SubCategory1, SubCategory2, Category>(
                               _sql,
                               (category, subCategory1, subCategory2) =>
                               {
                                   Category _categoryEntry;
                                   SubCategory1 _subCategoryEntry = new SubCategory1();

                                   if (!_categoryDictionary.TryGetValue(category.Id, out _categoryEntry))
                                   {
                                       _categoryEntry = category;
                                       _categoryEntry.SubCategories = new List<SubCategory1>();
                                       _categoryDictionary.Add(_categoryEntry.Id, _categoryEntry);
                                   }

                                   if (subCategory1 != null && !_subcategoryDictionary.TryGetValue(subCategory1.Id, out
                                       _subCategoryEntry))
                                   {
                                       _subCategoryEntry = subCategory1;
                                       _subCategoryEntry.SubCategories = new List<SubCategory2>();
                                       _subcategoryDictionary.Add(subCategory1.Id, _subCategoryEntry);
                                   }

                                   if (subCategory1 != null)
                                   {
                                       if (!_categoryEntry.SubCategories.Exists(x => x.Id == subCategory1.Id))
                                       {
                                           _categoryEntry.SubCategories.Add(subCategory1);
                                       }

                                       if (subCategory2 != null)
                                       {
                                           _subCategoryEntry.SubCategories.Add(subCategory2);
                                       }
                                   }

                                   return _categoryEntry;
                               },
                               splitOn: "id").Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _categories;
        }

        public void AddMainCategory(Category category)
        {
            string _sql = @"INSERT INTO [CATEGORY] ([NAME],[ENABLE])
                            VALUES (@NAME,@ENABLE)";

            m_DapperHelper.ExecuteSql(_sql, category);

            _sql = @"SELECT [ID] FROM [CATEGORY] WHERE NAME = @NAME";

            int _rootId = m_DapperHelper.QuerySqlFirstOrDefault<Category, int>(_sql, category);

            SubCategory1 _subCategory1 = new SubCategory1();
            _subCategory1.Name = "其它";
            _subCategory1.Parent_id = _rootId;
            _subCategory1.Enable = OpenStatus.Close;

            _sql = @"INSERT INTO [SUBCATEGORY1] ([NAME],[PARENT_ID],[ENABLE])
                            VALUES (@NAME,@PARENT_ID,@ENABLE)";

            m_DapperHelper.ExecuteSql(_sql, _subCategory1);
        }

        public void UpdateMainCategory(Category category)
        {
            string _sql = @"UPDATE [CATEGORY] SET [NAME]=@NAME,[ENABLE]=@ENABLE,[UPDATED_AT]=@UPDATED_AT WHERE ID=@ID";

            m_DapperHelper.ExecuteSql(_sql, category);
        }

        public void AddSubCategory<T>(T category, string tableName)
        {
            string _sql = $@"INSERT INTO {tableName} ([PARENT_ID], [NAME], [ENABLE])
                            VALUES (@PARENT_ID, @NAME, @ENABLE)";

            m_DapperHelper.ExecuteSql(_sql, category);
        }

        public void AddSubCategory<T>(IEnumerable<T> category, string tableName)
        {
            string _sql = $@"INSERT INTO {tableName} ([PARENT_ID], [NAME], [ENABLE])
                            VALUES (@PARENT_ID, @NAME, @ENABLE)";

            m_DapperHelper.ExecuteSql(_sql, category);
        }

        public void UpdateSubCategory<T>(T category, string tableName)
        {
            string _sql = $@"UPDATE {tableName} SET [NAME]=@NAME,[ENABLE]=@ENABLE,[UPDATED_AT]=@UPDATED_AT WHERE [ID]=@ID";

            m_DapperHelper.ExecuteSql(_sql, category);
        }

        public void DeleteCategory<T>(T category, string tableName)
        {
            string _sql = $@"DELETE FROM {tableName} WHERE [ID]=@ID";

            m_DapperHelper.ExecuteSql(_sql, category);
        }

        public bool ExistSameName<T>(T category, string tableName, bool isUpdate = false)
        {
            bool _isSame = false;

            string _updateSql = isUpdate == true ? "AND ID <> ID" : string.Empty;

            string _sql = $@"SELECT [NAME] FROM {tableName} WHERE NAME=@NAME {_updateSql}";

            var _list = m_DapperHelper.QuerySetSql(_sql, category);

            if (_list.Count() > 0)
                _isSame = true;

            return _isSame;
        }

        public bool ExistId<T>(T category, string tableName)
        {
            bool _isExist = false;

            string _sql = $@"SELECT [ID] FROM {tableName} WHERE [ID]=@ID";

            var _list = m_DapperHelper.QuerySetSql(_sql, category);

            if (_list.Count() > 0)
                _isExist = true;

            return _isExist;
        }       
    }
}