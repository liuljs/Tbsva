using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class AddTbsvaService : IAddTbsvaService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        public AddTbsvaService(IDapperHelper iDapperHelper)
        {
            _IDapperHelper = iDapperHelper;
        }
        #endregion

        #region  新增實作
        public AddTbsva InsertAddTbsva(HttpRequest httpRequest)
        {
            AddTbsva addTbsva = Request_data(httpRequest);
            string _sql = @"INSERT INTO [addTbsva]
                                                   ([addtbsvaId]
                                                   ,[namez]
                                                   ,[gender]
                                                   ,[birthz]
                                                   ,[affiliatedAreaz]
                                                   ,[contactAddress]
                                                   ,[contactNumber]
                                                   ,[moblieNumber]
                                                   ,[email]
                                                   ,[contentz]
                                                   ,[notez]
                                                   ,[enabled]
                                                   ,[audit]
                                                   ,[creationDate])
                                             VALUES
                                                   (@addtbsvaId,
                                                   @namez,
                                                   @gender,
                                                   @birthz,
                                                   @affiliatedAreaz,
                                                   @contactAddress,
                                                   @contactNumber,
                                                   @moblieNumber,
                                                   @email,
                                                   @contentz,
                                                   @notez,
                                                   @enabled,
                                                   @audit,
                                                   @creationDate )
                                                    SELECT SCOPE_IDENTITY()";   //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, addTbsva);
            addTbsva.id = id;
            return addTbsva;
        }
        /// <summary>
        /// 讀取表單資料寫入類別(新增時用)
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns>AddTbsva addTbsva</returns>
        private AddTbsva Request_data(HttpRequest httpRequest)
        {
            AddTbsva addTbsva = new AddTbsva();                 //設定空類別準備接收資料
            addTbsva.addtbsvaId = Guid.NewGuid();               //另設 ID 唯一索引用
            addTbsva.namez = httpRequest.Form["namez"];
            addTbsva.gender = Convert.ToByte(httpRequest.Form["gender"]);       //tinyint
            addTbsva.birthz = Convert.ToDateTime(httpRequest.Form["birthz"]);
            addTbsva.affiliatedAreaz = Convert.ToByte(httpRequest.Form["affiliatedAreaz"]);  //tinyint
            //https://docs.microsoft.com/zh-tw/dotnet/framework/data/adonet/sql-server-data-type-mappings
            addTbsva.contactAddress = httpRequest.Form["contactAddress"];
            addTbsva.contactNumber = httpRequest.Form["contactNumber"];
            addTbsva.moblieNumber = httpRequest.Form["moblieNumber"];
            addTbsva.email = httpRequest.Form["email"];
            addTbsva.contentz = httpRequest.Form["contentz"];
            addTbsva.notez = httpRequest.Form["notez"];
            addTbsva.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));
            addTbsva.audit = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["audit"]));
            addTbsva.creationDate = DateTime.Now;

            return addTbsva;
        }
        #endregion

        #region 取得一筆資料
        public AddTbsva GetAddTbsva(Guid id)
        {
            AddTbsva addTbsva = new AddTbsva();
            addTbsva.addtbsvaId = id;

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1";  //前端只顯示1
            string _sql = $"SELECT * FROM [addTbsva] WHERE [addtbsvaId] = @addtbsvaId {adminQuery}" ;
            addTbsva = _IDapperHelper.QuerySqlFirstOrDefault(_sql, addTbsva);

            return addTbsva;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateAddTbsva(HttpRequest httpRequest, AddTbsva addTbsva)
        {
            addTbsva = Request_data_mod(httpRequest, addTbsva);

            //處理資料庫更新資料
            string _sql = @"UPDATE [addTbsva]
                                                    SET
                                                       [namez] = @namez
                                                      ,[gender] = @gender
                                                      ,[birthz] = @birthz
                                                      ,[affiliatedAreaz] = @affiliatedAreaz
                                                      ,[contactAddress] = @contactAddress
                                                      ,[contactNumber] = @contactNumber
                                                      ,[moblieNumber] = @moblieNumber
                                                      ,[email] = @email
                                                      ,[contentz] = @contentz
                                                      ,[notez] = @notez
                                                      ,[enabled] = @enabled
                                                      ,[audit] = @audit
                                                      ,[updatedDate] = @updatedDate
                                                  WHERE [addtbsvaId] = @addtbsvaId";
                                            _IDapperHelper.ExecuteSql(_sql, addTbsva);
        }
        private AddTbsva Request_data_mod(HttpRequest httpRequest, AddTbsva addTbsva)
        {
            addTbsva.namez = httpRequest.Form["namez"];
            addTbsva.gender = Convert.ToByte(httpRequest.Form["gender"]);       //tinyint
            addTbsva.birthz = Convert.ToDateTime(httpRequest.Form["birthz"]);
            addTbsva.affiliatedAreaz = Convert.ToByte(httpRequest.Form["affiliatedAreaz"]);  //tinyint
            addTbsva.contactAddress = httpRequest.Form["contactAddress"];
            addTbsva.contactNumber = httpRequest.Form["contactNumber"];
            addTbsva.moblieNumber = httpRequest.Form["moblieNumber"];
            addTbsva.email = httpRequest.Form["email"];
            addTbsva.contentz = httpRequest.Form["contentz"];
            addTbsva.notez = httpRequest.Form["notez"];
            addTbsva.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));
            addTbsva.audit = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["audit"]));
            addTbsva.updatedDate = DateTime.Now;

            return addTbsva;
        }
        #endregion

        #region 刪除一筆資料
        public void DeleteAddTbsva(AddTbsva addTbsva)
        {
            string _sql = @"DELETE FROM [addTbsva] WHERE [addtbsvaId] = @addtbsvaId";
            _IDapperHelper.ExecuteSql(_sql, addTbsva);
        }
        #endregion

        #region 取得加入台密總明細項目(前後端enabled區別)
        public List<AddTbsva> GetAddTbsvasAll(string namez, int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            string search_sql = string.Empty;

            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }

            //後台登入不限條件 : enabled 狀態 1顯示 0不顯示 , audit審核 0未通過 1通過
            string adminQuery = Auth.Role.IsAdmin ? " where 1=1 " : " WHERE ENABLED = 1 And audit = 1 ";
            if (!string.IsNullOrWhiteSpace(namez))
            {
                search_sql += $" And namez like '%{namez}%' ";
            }
            string _sql = $"SELECT * FROM [addTbsva] {adminQuery} " +
                                  $" {search_sql} " +
                                  @" ORDER BY [creationDate] Desc " +
                                  $" {page_sql} ";

            List<AddTbsva> addTbsvas = _IDapperHelper.QuerySetSql<AddTbsva>(_sql).ToList();

            return addTbsvas;
        }
        #endregion

        #region 取得加入台密總總筆數(前後端enabled區別)
        public int GetAddTbsvaTotalCount()
        {
            string adminQuery = Auth.Role.IsAdmin ? "" : " WHERE ENABLED = 1 ";
            string _sql = $@"SELECT COUNT(id) FROM [addTbsva] {adminQuery} ";

            int TotalCount = _IDapperHelper.ExecuteScalar(_sql);
            return TotalCount;
        }
        #endregion

        #region 更新加入台密總的審核
        public void UpdateAuditOption(HttpRequest httpRequest, AddTbsva addTbsva)
        {
            addTbsva.audit = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["audit"]));
            addTbsva.updatedDate = DateTime.Now;
            //處理資料庫更新資料
            string _sql = $@"UPDATE [addTbsva]
                                                   SET
                                                   [audit] = @audit
                                                  ,[updatedDate] = @updatedDate
                                                    WHERE [addtbsvaId] = @addtbsvaId";
            _IDapperHelper.ExecuteSql(_sql, addTbsva);
        }
        #endregion
    }
}