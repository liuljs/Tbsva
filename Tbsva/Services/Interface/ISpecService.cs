using WebShopping.Dtos;
using WebShopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebShopping.Services
{
    public interface ISpecService
    {
        /// <summary>
        /// 取得所有規格
        /// </summary>
        /// <returns></returns>
        List<Sku> GetSpecs(int spu_id, bool IsAdmin);
        /// <summary>
        /// 取得指定規格
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Sku GetSpecInfo(int id, bool IsAdmin);

        /// <summary>
        /// 新增規格
        /// </summary>
        /// <param name="skus"></param>
        int AddData(List<Sku> skus);

        /// <summary>
        /// 更新規格
        /// </summary>
        /// <param name="skus"></param>
        int UpdateData(List<Sku> skus);

        /// <summary>
        /// 刪除規格
        /// </summary>
        /// <param name="id"></param>
        int DeleteData(int id);

    }
}
