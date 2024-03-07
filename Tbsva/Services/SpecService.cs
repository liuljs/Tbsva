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
    public class SpecService : ISpecService
    {
        IDapperHelper m_DapperHelper;

        //IImageFileHelper m_ImageFileHelper;

        private IMapper m_Mapper;

        //private string m_imageFolder = @"\Admin\backStage\img\Specs\";

        public SpecService(IDapperHelper dapper, IMapper mapper)
        {
            m_DapperHelper = dapper;
            //m_ImageFileHelper = imageFile;
            m_Mapper = mapper;
        }

        public List<Sku> GetSpecs(int spu_id,bool IsAdmin)
        {
            List<Sku> _spus = new List<Sku>();
            SpecDto spec = new SpecDto() { Spu_Id = spu_id };
            string adminQuery = IsAdmin ? "" : " AND [Enabled]=1 ";
            string _sql = $"SELECT * FROM [sku] WHERE [spu_id]=@spu_id {adminQuery}";
            _spus = m_DapperHelper.QuerySetSql<SpecDto, Sku>(_sql, spec).ToList();
            return _spus;
        }

        public Sku GetSpecInfo(int id, bool IsAdmin)
        {
            SpecDto spec = new SpecDto() { Id = id };
            string adminQuery = IsAdmin ? "" : " AND [Enabled]=1 ";
            string _sql = $"SELECT * FROM [sku] WHERE [id]=@id {adminQuery}";

            var sku = m_DapperHelper.QuerySqlFirstOrDefault<SpecDto, Sku>(_sql, spec);
            return sku;
        }

        /// <summary>
        /// 新增規格
        /// </summary>
        /// <param name="spec"></param>
        public int AddData(List<Sku> skus)
        {
            //TODO:還沒處理到行銷
            string _sql = @"INSERT INTO [sku]([spu_id],[title],[sell_price],[enabled],[stock_qty],[start_stock_qty],[safety_stock_qty]) VALUES (@spu_id,@title,@sell_price,@enabled,@stock_qty,@stock_qty,@safety_stock_qty)";
            int i = m_DapperHelper.ExecuteSql(_sql, skus);
            return i;
        }

        /// <summary>
        /// 更新單筆規格
        /// </summary>
        /// <param name="request">用戶端的要求資訊</param>
        /// <param name="news">更新的資料類別</param>
        public int UpdateData(List<Sku> skus)
        {
            //TODO:還沒處理到行銷
            string _sql = @"UPDATE [sku] SET [title]=@title,[sell_price]=@sell_price,[enabled]=@enabled,[safety_stock_qty]=@safety_stock_qty 
                     WHERE [ID]=@ID AND [SPU_ID]=@SPU_ID";
            int i = m_DapperHelper.ExecuteSql(_sql, skus);
            return i;
        }

        /// <summary>
        /// 刪除一筆規格
        /// </summary>
        /// <param name="spec"></param>
        public int DeleteData(int id)
        {
            string _sql = $"DELETE [sku] WHERE [ID]={id}";
            int i = m_DapperHelper.ExecuteSql(_sql);
            return i;
        }
    }
}