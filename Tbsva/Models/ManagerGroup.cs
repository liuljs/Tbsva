using System;
using System.Linq;
using System.Web;
using System.Data;
//using ecSqlDBManager;
using System.Data.SqlClient;

namespace WebShoppingAdmin.Models
{
    public class ManagerGroup : BaseModel
    {
        /// <summary>
        /// 輸入群組ID查詢抓出群組，然後在用群組ID搜尋LNK_MODULE記錄，在從記錄搜尋MODULE
        /// </summary>
        /// <param name="id">群組ID</param>
        /// <returns></returns>
        public DataTable Get(Guid? id)
        {
            //String.Format("The current price is {0} per ounce.",  pricePerOunce);  <=  {0}=pricePerOunce這個值
            //1.用搜尋群組資料
            string search_sql = string.Empty;
            if (id != null)
            {
                search_sql = " AND [authId] = @authId";
            }

            //Format(creationDate, N'yyyy-MM-dd HH:mm') 
            string _sql = $@"SELECT [id], [authId], [name], Format(creationDate, N'yyyy-MM-dd HH:mm') AS creationDate 
                                    FROM [MANAGER_GROUP]
                                    WHERE [NAME] <> 'Admin' {search_sql} ";

            SqlCommand sql = new SqlCommand(_sql);

            //SqlCommand sql = new SqlCommand(string.Format(@"
            //SELECT [id], [authId], [name], [creationDate]
            //FROM [MANAGER_GROUP]
            //WHERE [NAME] <> 'Admin' 
            //{0} 
            //ORDER BY [NAME]", id == null ? "" : "AND [authId] = @authId"));

            if (id != null)
                sql.Parameters.AddWithValue("@authId", id);
            DataTable result = db.GetResult(sql);

            //DataTable dt)
            //{
            //    DataTable datatable = new DataTable();
            //    //克隆表結構，表的資料並沒有克隆
            //    datatable = dt.Clone();
            //    foreach (DataColumn col in datatable.Columns)
            //    {
            //        if (col.ColumnName == "ID")
            //        {
            //            //修改列型別
            //            col.DataType = typeof(String);
            //        }
            //    }

            //foreach (DataColumn col in result.Columns)
            //{
            //    if (col.ColumnName == "creationDate")
            //    {
            //        var TTest = col.DataType;
            //        col.DataType = typeof(string);
            //    }
            //}

            //foreach (DataRow dr in result.Rows)
            //{
            //    var date = dr["creationDate"].ToString();
            //    var date2 = Convert.ToDateTime(Convert.ToDateTime(date).ToString("yyyy-MM-dd HH:mm"));
            //    dr["creationDate"] = date2;  //重新寫入時原本欄位還是時間，所以yyyy-MM-dd HH:mm後面秒會是00
            //}

            //for (int i = 0; i < result.Rows.Count; i++)
            //{
            //    //string aa = Convert.ToDateTime(result.Rows[i]["creationDate"].ToString("yyyy-MM-dd"));
            //    result.Rows[i]["date"] = (result.Rows[i]["date"]).GetType(ToString(.ToString("yyyy-MM-dd hh:mm")));
            //}
            result.Columns.Add("auths", typeof(DataTable));

            foreach (DataRow dr in result.Rows)
            {
                string _managerGroupId = dr["authId"].ToString();  //2.調出資料庫群組ID

                //sql = new SqlCommand(string.Format(@"
                //SELECT [B].[CODE],[B].[NAME],[B].[ACT_VIEW],[B].[ACT_EDT],[B].[ACT_DEL]
                //FROM [LNK_MODULE] A
                //LEFT JOIN [MODULE] B
                //ON A.MODULE_ID = B.ID
                //WHERE [A].[MANAGER_GRP_ID] = @ID"));

                //用群組[id]查詢裏面的模組LNK_MODULE為記綠某群組下所有module id
                sql = new SqlCommand(string.Format(@"
                SELECT B.[authId], B.[name], B.[path], B.[belong], B.[view], B.[edit], B.[category]
                FROM [LNK_MODULE] A
                LEFT JOIN [MODULE] B
                ON A.MODULE_ID = B.ID
                WHERE [A].[MANAGER_GRP_ID] = @ID order by authId ASC "));

                sql.Parameters.AddWithValue("@ID", _managerGroupId);
                DataTable _subDataTable = db.GetResult(sql);

                dr["auths"] = _subDataTable;
            }

            return result;
        }

        public void Insert(ManagerGroupInsertParam param)
        {
            try
            {
                db.OpenConn();

                //param.id = Guid.NewGuid(); //新增的群組ID
                param.name = param.name.Trim();

                SqlCommand sql = new SqlCommand(@"
                SELECT [NAME] FROM [MANAGER_GROUP] WHERE [NAME] = @NAME");
                sql.Parameters.AddWithValue("@NAME", param.name);
                DataTable dt = db.GetResult(sql);

                if (dt.Rows.Count > 0)
                    throw new Exception("已經有相同名稱的管理群組");

                //1.新增群組名稱
                sql = new SqlCommand(@"
                INSERT INTO [MANAGER_GROUP] ([authId], [NAME])
                VALUES (@authId, @NAME)");
                sql.Parameters.AddWithValue("@authId", param.id);
                sql.Parameters.AddWithValue("@NAME", param.name);

                db.ExecuteNonCommit(sql);

                //2.新增模組，並使用產生的_moduleId給LNK_MODULE對應MANAGER_GRP_ID
                for (int i = 0; i < param.auth.Count; i++)
                {
                    Guid _moduleId = Guid.NewGuid();

                    sql = new SqlCommand(@"INSERT INTO [module]
                                                                       ([id]
                                                                       ,[authId]
                                                                       ,[name]
                                                                       ,[path]
                                                                       ,[belong]
                                                                       ,[view]
                                                                       ,[edit]
                                                                       ,[category])
                                                                 VALUES
                                                                       (@id,
                                                                        @authId,
                                                                        @name,
                                                                        @path,
                                                                        @belong,
                                                                        @view,
                                                                        @edit,
                                                                       @category)"   );
                    sql.Parameters.AddWithValue("@id", _moduleId);
                    sql.Parameters.AddWithValue("@authId", param.auth[i].authId);
                    sql.Parameters.AddWithValue("@name", param.auth[i].name);
                    sql.Parameters.AddWithValue("@path", param.auth[i].path);
                    sql.Parameters.AddWithValue("@belong", param.auth[i].belong);
                    sql.Parameters.AddWithValue("@view", param.auth[i].view);
                    sql.Parameters.AddWithValue("@edit", param.auth[i].edit);
                    sql.Parameters.AddWithValue("@category", param.auth[i].category);
                    db.ExecuteNonCommit(sql);

                    //3.新增剛剛每一筆模組的ID對應給群組id(MANAGER_GRP_ID)
                    sql = new SqlCommand(@"
                    INSERT INTO [LNK_MODULE] ([MANAGER_GRP_ID], [MODULE_ID])
                    VALUES (@MANAGER_GRP_ID, @MODULE_ID)");
                    sql.Parameters.AddWithValue("@MANAGER_GRP_ID", param.id);
                    sql.Parameters.AddWithValue("@MODULE_ID", _moduleId);

                    db.ExecuteNonCommit(sql);
                }

                EventLog(db, "manager_group", DbAction.INSERT, param.account_id, null, param);

                db.Commit();
                db.CloseConn();
            }
            catch (Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }
        }

        public void Update(ManagerGroupUpdateParam param)
        {
            try
            {
                db.OpenConn();

                param.name = param.name.Trim();

                //0.無此管理群組
                SqlCommand sql = new SqlCommand(@"
                SELECT [NAME]
                FROM [MANAGER_GROUP]                
                WHERE [authId] = @authId");
                sql.Parameters.AddWithValue("@authId", param.id);   //管理群組id
                DataTable dt = db.GetResult(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception($"無此管理群組");

                //1.查出管理群組(拿來做記錄用EventLog)
                sql = new SqlCommand(@"
                SELECT [ID], [authId], [NAME], [creationDate]
                FROM [MANAGER_GROUP]
                WHERE [authId] = @authId");
                sql.Parameters.AddWithValue("@authId", param.id);
                DataTable oldData = db.GetResult(sql);

                //2.使用LNK_MODULE記錄where 群組id所有對應的模組MODULE id
                //調出module id, authId欄
                sql = new SqlCommand(@"
                    SELECT A.[id], A.[authId]
                    FROM [MODULE] A
                    JOIN [LNK_MODULE] B ON A.[ID] = B.MODULE_ID
                    WHERE B.[MANAGER_GRP_ID] = @ID");
                sql.Parameters.AddWithValue("@ID", param.id);   //群組ID
                DataTable _refTable = db.GetResult(sql);

                //3.先更新群組名稱
                sql = new SqlCommand(@"
                UPDATE [MANAGER_GROUP] SET
                    [NAME] = @NAME
                WHERE [authId] = @authId");
                sql.Parameters.AddWithValue("@NAME", param.name);
                sql.Parameters.AddWithValue("@authId", param.id);

                db.ExecuteNonCommit(sql);


                //4.剛剛第2步調出群組下的單一模組開始做更新
                foreach (DataRow dr in _refTable.Rows)
                {
                    string _moduleId = dr["id"].ToString();  //[module].id 從DataTable _refTable調id欄
                    string _authId = dr["authId"].ToString();     //從DataTable _refTable調authId欄

                    //找postman輸入進來的List<Module> auth多筆資料
                    //ManagerGroupUpdateParam=> List<Module> auth=> 裏面的authId
                    //例如param.auth.有三筆選單，然後用authId來比對與原本資料庫抓出來放在DataTable _refTable
                    //拿_refTable _authId欄位比對有等於時代表透過輸入進來的model資料(param.auth)的authId有等於原
                    //資料庫的_authId，確定有後_module資料庫則等於輸入的那筆資料
                    //簡單說輸入進來的資料先搜尋剛剛將資料庫倒到DataTable _refTable裏的資料比對，找到才更新
                    var _module = param.auth.Where(x => x.authId == _authId).FirstOrDefault();

                    //確定輸入的資料有存在資料庫開始做更新
                    //只提供更新view edit
                    if (_module != null)
                    {
                        sql = new SqlCommand(@"
                                            UPDATE [module]
                                               SET  
                                                         [view] = @view
                                                        ,[edit] = @edit
                                                         WHERE  [ID] = @ID "
                        );
                        sql.Parameters.AddWithValue("@ID", _moduleId);
                        sql.Parameters.AddWithValue("@view", _module.view);
                        sql.Parameters.AddWithValue("@edit", _module.edit);

                        db.ExecuteNonCommit(sql);
                    }
                }

                EventLog(db, "manager_group", DbAction.UPDATE, param.account_id, oldData, param);

                db.Commit();
                db.CloseConn();
            }
            catch (Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }
        }

        public void Delete(ManagerGroupDeleteParam param)
        {
            try
            {
                db.OpenConn();

                //0.撈出舊的群組 MANAGER_GROUP 表(記錄用EventLog)
                SqlCommand sql = new SqlCommand(@"
                SELECT
                    [ID], [authId],  [NAME], [creationDate]
                FROM [MANAGER_GROUP]
                WHERE [authId] = @GROUP_ID");
                sql.Parameters.AddWithValue("@GROUP_ID", param.id); //群組ID
                DataTable oldData = db.GetResult(sql);

                //DEL
                //1.MANAGER_GROUP 群組資料
                //2.LNK_MANAGER_GROUP 帳號與群組資料關連
                //3.MODULE 模組資料
                //4.LNK_MODULE 群組id與模組id關連

                //1.刪除 群組 資料MANAGER_GROUP
                sql = new SqlCommand(@"
                DELETE FROM [MANAGER_GROUP]
                WHERE [authId] = @GROUP_ID");
                sql.Parameters.AddWithValue("@GROUP_ID", param.id); //群組ID
                db.ExecuteNonCommit(sql);

                //用群組ID撈出 LNK_MANAGER_GROUP 記錄帳號有那些群組(沒有用到)
                sql = new SqlCommand(@"
                SELECT [MANAGER_ID], [MANAGER_GRP_ID] 
                FROM [LNK_MANAGER_GROUP] 
                WHERE [MANAGER_GRP_ID] = @GROUP_ID");
                sql.Parameters.AddWithValue("@GROUP_ID", param.id); //群組ID
                DataTable oldLnkManagerGroupData = db.GetResult(sql);

                //2.刪除 LNK_MANAGER_GROUP 資料, 因為群組被刪，所以要把記錄帳號下關連那個郡組刪除
                sql = new SqlCommand(@"
                DELETE FROM[LNK_MANAGER_GROUP]                 
                WHERE [MANAGER_GRP_ID] = @GROUP_ID");
                sql.Parameters.AddWithValue("@GROUP_ID", param.id); //群組ID
                db.ExecuteNonCommit(sql);

                //撈出 MODULE 表(沒有用到，這裏弄錯了module.id並不是群組id，所以會抓不到資料)
                sql = new SqlCommand(@"
                SELECT * FROM [MODULE] 
                WHERE [id] = @GROUP_ID");
                sql.Parameters.AddWithValue("@GROUP_ID", param.id); //群組ID
                DataTable oldModuleData = db.GetResult(sql);

                //3.刪除 [MODULE] B 資料 ，需用LNK_MODULE的群組ID抓出要刪除的module id
                sql = new SqlCommand(@"
				DELETE B
                FROM [LNK_MODULE] A
                LEFT JOIN [MODULE] B
                ON A.MODULE_ID = B.ID
                WHERE [A].[MANAGER_GRP_ID] = @GROUP_ID");
                sql.Parameters.AddWithValue("@GROUP_ID", param.id); //群組ID
                db.ExecuteNonCommit(sql);

                //撈出 LNK_MODULE 表(沒有用到)
                sql = new SqlCommand(@"
                SELECT [MANAGER_GRP_ID], [MODULE_ID] 
                FROM [LNK_MODULE] 
                WHERE [MANAGER_GRP_ID] = @GROUP_ID");
                sql.Parameters.AddWithValue("@GROUP_ID", param.id); //群組ID
                DataTable oldLnkModuleData = db.GetResult(sql);

                //4.刪除 LNK_MODULE 資料，看是那個群組的模組要刪
                sql = new SqlCommand(@"
                DELETE [LNK_MODULE]	
                WHERE [MANAGER_GRP_ID] = @GROUP_ID");
                sql.Parameters.AddWithValue("@GROUP_ID", param.id); //群組ID
                db.ExecuteNonCommit(sql);

                EventLog(db, "manager_group", DbAction.DELETE, param.account_id, oldData, null);

                db.Commit();
                db.CloseConn();
            }
            catch (Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }
        }

        public void UpdateLnk(ManagerGroupLnkParam param)
        {
            try
            {
                db.OpenConn();

                //0.使用群組group_id來搜尋裏面記錄的模組id(只是拿來EventLog用)
                SqlCommand sql = new SqlCommand(@"
                SELECT
                    [MANAGER_GRP_ID], [MODULE_ID]
                FROM [LNK_MODULE]
                WHERE [MANAGER_GRP_ID] = @MANAGER_GRP_ID");
                sql.Parameters.AddWithValue("@MANAGER_GRP_ID", param.group_id);
                DataTable oldData = db.GetResult(sql);

                //1.刪除原有的群組下的所有模組id記錄，並沒有刪除module的資料
                sql = new SqlCommand(@"
                DELETE FROM [LNK_MODULE]
                WHERE [MANAGER_GRP_ID] = @MANAGER_GRP_ID");
                sql.Parameters.AddWithValue("@MANAGER_GRP_ID", param.group_id);
                db.ExecuteNonCommit(sql);

                //Guid[] modules 輸入的模組id
                //2.加入輸入的所有模組id，並記在輸入的param.group_id
                if (param.modules != null)
                {
                    sql = new SqlCommand(@"
                    INSERT INTO [LNK_MODULE] ([MANAGER_GRP_ID], [MODULE_ID])
                    VALUES (@MANAGER_GRP_ID, @MODULE_ID)");
                    foreach (var inputModule in param.modules)
                    {
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@MANAGER_GRP_ID", param.group_id);
                        sql.Parameters.AddWithValue("@MODULE_ID", inputModule);

                        db.ExecuteNonCommit(sql);
                    }
                }

                EventLog(db, "lnk_module", DbAction.UPDATE, param.account_id, oldData, param);

                db.Commit();
                db.CloseConn();
            }
            catch (Exception ex)
            {
                if (db.SqlConn.State != ConnectionState.Closed)
                    db.CloseConn();

                throw ex;
            }
        }
    }
}