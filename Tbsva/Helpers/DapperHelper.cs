using WebShopping.Connection;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebShopping.Helpers
{
    public class DapperHelper : IDapperHelper
    {
        public T QuerySqlFirstOrDefault<T>(string sql)
        {
            T _retData;

            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _retData = _cn.QueryFirstOrDefault<T>(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _retData;
        }

        public T QuerySqlFirstOrDefault<T>(string sql, T paramData)
        {
            T _retData;

            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _retData = _cn.QueryFirstOrDefault<T>(sql, paramData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _retData;
        }

        public T QuerySqlFirstOrDefault<T>(string sql, object paramData)
        {
            T _retData;

            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _retData = _cn.QueryFirstOrDefault<T>(sql, paramData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _retData;
        }


        public TReturn QuerySqlFirstOrDefault<T1, TReturn>(string sql, T1 paramData)
        {
            TReturn _retData;

            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _retData = _cn.QueryFirstOrDefault<TReturn>(sql, paramData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _retData;
        }

        public IEnumerable<T> QuerySetSql<T>(string sql)
        {
            IEnumerable<T> _retSetData;

            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _retSetData = _cn.Query<T>(sql).ToList();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _retSetData;
        }

        public IEnumerable<T> QuerySetSql<T>(string sql, T paramData)
        {
            IEnumerable<T> _retSetData;

            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _retSetData = _cn.Query<T>(sql, paramData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _retSetData;
        }
        //public TReturn QuerySetSql<T, TReturn>(string sql, T paramData)
        //{
        //    TReturn _retSetData;

        //    try
        //    {
        //        using (var _cn = new ConnectionFactory().CreateConnection())
        //        {
        //            _retSetData = _cn.Query<TReturn>(sql, paramData);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }

        //    return _retSetData;
        //}

        public IEnumerable<TReturn> QuerySetSql<T1, TReturn>(string sql, T1 paramData)
        {
            IEnumerable<TReturn> _retSetData;

            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _retSetData = _cn.Query<TReturn>(sql, paramData);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _retSetData;
        }

        public int ExecuteSql(string sql)
        {
            int result = 0;
            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _cn.Open();

                    using (var transaction = _cn.BeginTransaction())
                    {
                        result = _cn.Execute(sql, transaction: transaction);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int ExecuteSql<T>(string sql, T paramData)
        {
            int result = 0;
            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _cn.Open();

                    using (var transaction = _cn.BeginTransaction())
                    {
                        result = _cn.Execute(sql, paramData, transaction: transaction);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int ExecuteSql<T>(string sql, IEnumerable<T> paramData)
        {
            int result = 0;
            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _cn.Open();

                    using (var transaction = _cn.BeginTransaction())
                    {
                        result = _cn.Execute(sql, paramData, transaction: transaction);
                       
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public int QuerySingle<T>(string sql, T paramData)
        {
            var result = 0;
            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    _cn.Open();

                    using (var transaction = _cn.BeginTransaction())
                    {
                        result = _cn.QuerySingle<int>(sql, paramData, transaction: transaction);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    
        public int ExecuteScalar(string sql)
        {
            int result = 0;
            try
            {
                using (var _cn = new ConnectionFactory().CreateConnection())
                {
                    result = _cn.ExecuteScalar<int>(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }


    }
}