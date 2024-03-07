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
    public interface IProductService
    {
        /// <summary>
        /// 前台取得所有商品
        /// </summary>
        /// <returns></returns>
        List<Spu> GetProducts(QueryProduct query);

        /// <summary>
        /// 後台取得所有商品
        /// </summary>
        /// <returns></returns>
        List<Spu> GetProductsAdmin(QueryProduct query);

        /// <summary>
        /// 取得指定產品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Spu GetProductInfo(int id, bool IsAdmin);

        /// <summary>
        /// 新增產品
        /// </summary>
        /// <param name="productAdminInsertDto"></param>
        /// <param name="httpRequest"></param>
        void AddProductData(ProductAdminDto productAdminInsertDto, HttpRequest httpRequest);
        /// <summary>
        /// 新增1筆圖片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        string AddImage(HttpRequest request);

        /// <summary>
        /// 更新產品
        /// </summary>
        /// <param name="productAdminDto"></param>
        /// <param name="request"></param>
        void UpdateData(ProductAdminDto productAdminDto, Spu _spu, HttpRequest request);

        /// <summary>
        /// 刪除產品
        /// </summary>
        /// <param name="spu"></param>
        void DeleteProductData(Spu spu);

        /// <summary>
        /// 刪除產品說明的圖片
        /// </summary>
        /// <param name="_spu"></param>
        /// <returns></returns>
        int DeleteImage(SpuIntroductionImage _spu);

        /// <summary>
        /// 消費者對產品提出問題
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        int AddQuestion(ProductQuestion question);

        /// <summary>
        /// 管理者回覆產品的問題
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        int Answer(ProductAnswer answer);

        /// <summary>
        /// 取得產品問答
        /// </summary>
        /// <param name="member_id"></param>
        /// <param name="spu_id"></param>
        /// <param name="Is_View">前台產品下看問答時，Is_View要傳1</param>
        /// <returns></returns>
        List<Spu_QnA> GetProductsQnA(Guid? member_id, int? spu_id, bool Is_View);
    }
}
