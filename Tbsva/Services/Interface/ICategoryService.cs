using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopping.Services
{
    public interface ICategoryService
    {
        List<Category> GetCategorts(bool IsAdmin);

        void AddMainCategory(Category category);

        void UpdateMainCategory(Category category);

        void AddSubCategory<T>(T category, string tableName);

        void AddSubCategory<T>(IEnumerable<T> category, string tableName);

        void UpdateSubCategory<T>(T category, string tableName);

        void DeleteCategory<T>(T category, string tableName);

        bool ExistSameName<T>(T category,string tableName,bool isUpdate = false);

        bool ExistId<T>(T category, string tableName);
    }
}
