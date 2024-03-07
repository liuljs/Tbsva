using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShopping.Helpers;
using WebShopping.Models;

namespace WebShopping.Services
{
    public class ConvertFormsService : IConvertFormsService
    {
        #region DI依賴注入功能
        private IDapperHelper _IDapperHelper;
        public ConvertFormsService(IDapperHelper iDapperHelper)
        {
            _IDapperHelper = iDapperHelper;
        }
        #endregion

        #region  新增實作
        public ConvertForms InsertConvertForms(HttpRequest httpRequest)
        {
            ConvertForms convertForms = Request_data(httpRequest);

            string _sql = @"INSERT INTO [convert_forms]
                                                   ([convertFormsId]
                                                   ,[namez]
                                                   ,[gender]
                                                   ,[enNamez]
                                                   ,[convertz]
                                                   ,[birthz]
                                                   ,[affiliatedAreaz]
                                                   ,[residenceAddress]
                                                   ,[contactAddress]
                                                   ,[contactNumber]
                                                   ,[moblieNumber]
                                                   ,[email]
                                                   ,[sendCertificate]
                                                   ,[sendAddress]
                                                   ,[remarks]
                                                   ,[enabled]
                                                   ,[audit]
                                                   ,[creationDate])
                                             VALUES
                                                   (@convertFormsId,
                                                    @namez,
                                                    @gender,
                                                    @enNamez,
                                                    @convertz,
                                                    @birthz,
                                                    @affiliatedAreaz,
                                                    @residenceAddress,
                                                    @contactAddress,
                                                    @contactNumber,
                                                    @moblieNumber,
                                                    @email,
                                                    @sendCertificate,
                                                    @sendAddress,
                                                    @remarks,
                                                    @enabled,
                                                    @audit,
                                                    @creationDate)
                             SELECT SCOPE_IDENTITY()";  //此方式會傳回在目前工作階段以及目前範圍中，任何資料表產生的最後一個識別值。
            int id = _IDapperHelper.QuerySingle(_sql, convertForms);
            convertForms.id = id;
            return convertForms;
        }
        /// <summary>
        /// 讀取表單資料(新增時用)
        /// </summary>
        /// <param name="httpRequest">讀取表單資料</param>
        /// <returns>ConvertForms</returns>
        private ConvertForms Request_data(HttpRequest httpRequest)
        {
            ConvertForms convertForms = new ConvertForms();
            convertForms.convertFormsId = Guid.NewGuid();                                     //另設 ID 唯一索引用
            convertForms.namez = httpRequest.Form["namez"];
            convertForms.gender = Convert.ToByte(httpRequest.Form["gender"]);
            convertForms.enNamez = httpRequest.Form["enNamez"];
            convertForms.convertz = Convert.ToByte(httpRequest.Form["convertz"]);
            convertForms.birthz = Convert.ToDateTime(httpRequest.Form["birthz"]);
            convertForms.affiliatedAreaz = Convert.ToByte(httpRequest.Form["affiliatedAreaz"]);
            convertForms.residenceAddress = httpRequest.Form["residenceAddress"];
            convertForms.contactAddress = httpRequest.Form["contactAddress"];
            convertForms.contactNumber = httpRequest.Form["contactNumber"];
            convertForms.moblieNumber = httpRequest.Form["moblieNumber"];
            convertForms.email = httpRequest.Form["email"];
            convertForms.sendCertificate = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["sendCertificate"]));
            convertForms.sendAddress = httpRequest.Form["sendAddress"];
            convertForms.remarks = httpRequest.Form["remarks"];
            convertForms.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));
            convertForms.audit = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["audit"]));
            convertForms.creationDate = DateTime.Now;

            return convertForms;
        }
        #endregion

        #region 取得一筆資料
        public ConvertForms GetConvertForms(Guid id)
        {
            ConvertForms convertForms = new ConvertForms();
            convertForms.convertFormsId = id;

            string adminQuery = Auth.Role.IsAdmin ? "" : " AND ENABLED = 1 ";  //前端其實不開放
            string _sql = $"SELECT * FROM [convert_forms] WHERE [convertFormsId] = @convertFormsId {adminQuery}";
            convertForms = _IDapperHelper.QuerySqlFirstOrDefault(_sql, convertForms);

            return convertForms;
        }
        #endregion

        #region 更新一筆資料
        public void UpdateConvertForms(HttpRequest httpRequest, ConvertForms convertForms)
        {
            convertForms = Request_data_mod(httpRequest, convertForms);

            //處理資料庫更新資料
            string _sql = $@"UPDATE [convert_forms]
                                              SET
                                                   [namez] = @namez
                                                  ,[gender] = @gender
                                                  ,[enNamez] = @enNamez
                                                  ,[convertz] = @convertz
                                                  ,[birthz] = @birthz
                                                  ,[affiliatedAreaz] = @affiliatedAreaz
                                                  ,[residenceAddress] = @residenceAddress
                                                  ,[contactAddress] = @contactAddress
                                                  ,[contactNumber] = @contactNumber
                                                  ,[moblieNumber] = @moblieNumber
                                                  ,[email] = @email
                                                  ,[sendCertificate] = @sendCertificate
                                                  ,[sendAddress] = @sendAddress
                                                  ,[remarks] = @remarks
                                                  ,[enabled] = @enabled
                                                  ,[audit] = @audit
                                                  ,[updatedDate] = @updatedDate
                                                    WHERE [convertFormsId] = @convertFormsId";
                                    _IDapperHelper.ExecuteSql(_sql, convertForms);
        }
        private ConvertForms Request_data_mod(HttpRequest httpRequest, ConvertForms convertForms)
        {
            convertForms.namez = httpRequest.Form["namez"];
            convertForms.gender = Convert.ToByte(httpRequest.Form["gender"]);
            convertForms.enNamez = httpRequest.Form["enNamez"];
            convertForms.convertz = Convert.ToByte(httpRequest.Form["convertz"]);
            convertForms.birthz = Convert.ToDateTime(httpRequest.Form["birthz"]);
            convertForms.affiliatedAreaz = Convert.ToByte(httpRequest.Form["affiliatedAreaz"]);
            convertForms.residenceAddress = httpRequest.Form["residenceAddress"];
            convertForms.contactAddress = httpRequest.Form["contactAddress"];
            convertForms.contactNumber = httpRequest.Form["contactNumber"];
            convertForms.moblieNumber = httpRequest.Form["moblieNumber"];
            convertForms.email = httpRequest.Form["email"];
            convertForms.sendCertificate = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["sendCertificate"]));
            convertForms.sendAddress = httpRequest.Form["sendAddress"];
            convertForms.remarks = httpRequest.Form["remarks"];
            convertForms.enabled = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["enabled"]));
            convertForms.audit = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["audit"]));
            convertForms.updatedDate = DateTime.Now;

            return convertForms;
        }
        #endregion

        #region 刪除一筆資料
        public void DeleteConvertForms(ConvertForms convertForms)
        {
            string _sql = @"DELETE FROM [convert_forms] WHERE [convertFormsId] = @convertFormsId";
            _IDapperHelper.ExecuteSql(_sql, convertForms);
        }
        #endregion

        public List<ConvertForms> GetConvertFormsAll(int? count, int? page)
        {
            string page_sql = string.Empty;    //分頁SQL OFFSET 計算要跳過筆數 ROWS  FETCH NEXT 抓出幾筆 ROWS ONLY
            if (count != null && count > 0 && page != null & page > 0)
            {
                int startRowjumpover = 0;  //跳過筆數預設0
                startRowjumpover = Convert.ToInt32((page - 1) * count);  //  <=計算要跳過幾筆
                page_sql = $" OFFSET {startRowjumpover} ROWS FETCH NEXT {count} ROWS ONLY ";
            }

            string adminQuery = Auth.Role.IsAdmin ? "" : " WHERE ENABLED = 1 ";
            string _sql = $"SELECT * FROM [convert_forms] {adminQuery} " +
                                  @"ORDER BY creationDate Desc " +
                                  $" {page_sql} ";

            List<ConvertForms> convertFormsAll = _IDapperHelper.QuerySetSql<ConvertForms>(_sql).ToList();
            return convertFormsAll;
        }

        public int GetConvertFormsTotalCount()
        {
            string _sql = @"SELECT COUNT(id) FROM convert_forms";
            int TotalCount = _IDapperHelper.ExecuteScalar(_sql);
            //int TotalCount = _IDapperHelper.QuerySqlFirstOrDefault<int>(_sql);終於搞懂前面的T
            return TotalCount;
        }

        public void UpdateAuditOption(HttpRequest httpRequest, ConvertForms convertForms)
        {
            convertForms.audit = Convert.ToBoolean(Convert.ToByte(httpRequest.Form["audit"]));
            convertForms.updatedDate = DateTime.Now;
            //處理資料庫更新資料
            string _sql = $@"UPDATE [convert_forms]
                                                   SET
                                                   [audit] = @audit
                                                  ,[updatedDate] = @updatedDate
                                                    WHERE [convertFormsId] = @convertFormsId";
            _IDapperHelper.ExecuteSql(_sql, convertForms);
        }

    }
}