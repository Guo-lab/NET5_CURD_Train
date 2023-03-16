using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using NHibernate;
using NHibernate.Transform;
using ProjectBase.Domain;
using ProjectBase.Utils;
using SharpArch.Domain;
using SharpArch.NHibernate;
using log4net;
using System.Data;
using System.Linq.Expressions;
using System.Data.Common;

namespace ProjectBase.Data
{
    /// <summary>
    /// Utility class for native sql. Mind that use it only in special cases and with cautions for string concatenation.
    /// Watch for SQL injection holes!!!
    /// </summary>
    public class UtilQuery : NHibernateQuery, IUtilQuery
    {
        private static ILog _log = LogManager.GetLogger(typeof (UtilQuery));

        public Object GetScalarBySql(string sql)
        {
            CheckSqlInjection(sql);
            return Session.CreateSQLQuery(sql).UniqueResult();
        }
        public IList<String> GetStringListBySql(string sql)
        {
            CheckSqlInjection(sql);
            return Session.CreateSQLQuery(sql).List<String>();
        }
        public IList<Object[]> GetBySql(string sql)
        {
            CheckSqlInjection(sql);
            return Session.CreateSQLQuery(sql).List<Object[]>();
        }
        public IList<Object[]> GetBySql(string sql, IDictionary<string, Object> namedParameters)
        {
            CheckSqlInjection(sql);
            var q = Session.CreateSQLQuery(sql);
            foreach (var namedParameter in namedParameters)
            {
                q.SetParameter(namedParameter.Key, namedParameter.Value);
            }
            var l = q.List<Object[]>();
            return l;
        }
        /// <summary>
        /// Dto must have corresponding property to each column selected by the sql 
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IList<TDto> GetBySql<TDto>(string sql)
        {
            CheckSqlInjection(sql);
            var q = Session.CreateSQLQuery(sql);
            if (typeof(TDto).IsPrimitive)
            {
                return q.List<TDto>();
            }
            var l=q.SetResultTransformer(
                    Transformers.AliasToBean(typeof(TDto))).List<TDto>();
            return l;
        }
        public IList<TDto> GetBySql<TDto>(string sql,IDictionary<string,Object> namedParameters)
        {
            CheckSqlInjection(sql);
            var q = Session.CreateSQLQuery(sql);
            foreach (var namedParameter in namedParameters)
            {
                q.SetParameter(namedParameter.Key, namedParameter.Value);
            }
            if (typeof(TDto).IsValueType || typeof(TDto).FullName=="System.String")
            {
                return q.List<TDto>();
            }
            var l=q.SetResultTransformer(
                    Transformers.AliasToBean(typeof(TDto))).List<TDto>();
            return l;
        }
        public IList<TDto> GetBySql<TDto>(Pager pager,String countSql,String sql,IDictionary<string,Object> namedParameters)
        {
    	    Check.Require(pager != null, "pager may not be null!");
            CheckSqlInjection(sql);
            ISQLQuery q = Session.CreateSQLQuery(sql);
            ISQLQuery cq = Session.CreateSQLQuery(countSql);
            if(namedParameters!=null){
	            foreach (var namedParameter in namedParameters)
                {
	                q.SetParameter(namedParameter.Key, namedParameter.Value);
	                cq.SetParameter(namedParameter.Key, namedParameter.Value);;
	            }
            }
            pager.ItemCount =(int)cq.UniqueResult();
            if(pager.PageSize>0){
        	    q.SetFirstResult(pager.FromRowIndex).SetMaxResults(pager.PageSize);
            }
            return q.SetResultTransformer(
                    Transformers.AliasToBean<TDto>()).List<TDto>();
        } 
        public IList<T> GetEntityBySql<T>(string sql)
        {
            CheckSqlInjection(sql);
            return Session.CreateSQLQuery(sql).AddEntity(typeof(T)).List<T>();
        }
        public int ExcuteSql(string sql)
        {
            CheckSqlInjection(sql,true);
            return Session.CreateSQLQuery(sql).ExecuteUpdate();
        }
        public int ExcuteSql(String sql,IDictionary<string,Object> namedParameters)
        {
            CheckSqlInjection(sql,true);
            ISQLQuery q = Session.CreateSQLQuery(sql);
            foreach (var namedParameter in namedParameters)
            {
                q.SetParameter(namedParameter.Key, namedParameter.Value);
            }
            return q.ExecuteUpdate();
        }

        /// <summary>
        /// Dto defines which columns will be returned by the dbobject
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="dbobjectname">db view or function or stored procedures which return recordsets</param>
        /// <returns></returns>
        public IList<TDto> GetFromDBObj<TDto>(string dbobjectname)
        {
            return GetFromDBObj<TDto>(dbobjectname, null);
        }
        /// <summary>
        /// Dto defines which columns will be returned by the dbobject,and you can append a sql string to do filtering 
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="dbobjectname"></param>
        /// <param name="appendsql"></param>
        /// <returns></returns>
        public IList<TDto> GetFromDBObj<TDto>(string dbobjectname,string appendsql)
        {
            CheckSqlInjection(appendsql);
            var props = typeof (TDto).GetProperties();
            var selectlist = "";
            Array.ForEach(props, item => selectlist = selectlist+"["+item.Name+"],");
            selectlist = selectlist.Remove(selectlist.Length - 1);
            var sql = "select " + selectlist + " from " + dbobjectname+" ";
            if(!string.IsNullOrEmpty(appendsql))
            {
                sql = sql + appendsql;
            }
            var t = Session.CreateSQLQuery(sql).SetResultTransformer(
                    Transformers.AliasToBean(typeof (TDto))).List<TDto>();
            return t;
        }
        public DataSet ExecuteProcedureDataSet(string procedureName, IList<ProcedureParameter> parameters)
        {
            ISession iSession = this.Session;
            DbConnection conn = iSession.Connection;
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procedureName;
            parameters.ToList().ForEach(x => this.PrepareParameter(cmd, x.ParameterName, x.DbType, x.Value));

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            iSession.GetCurrentTransaction().Enlist(cmd);
            
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = (SqlCommand)cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }

        public void ExecuteProcedureNonQuery(string procedureName,IList<ProcedureParameter> parametes)
        {
            ISession iSession = this.Session;
            DbConnection conn = iSession.Connection;
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procedureName;
            parametes.ToList().ForEach(x=> this.PrepareParameter(cmd,x.ParameterName,x.DbType,x.Value));

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            iSession.GetCurrentTransaction().Enlist(cmd);
            cmd.ExecuteNonQuery();
        }

        private void PrepareParameter(IDbCommand cmd, string parameterName, DbType dbType, object value)
        {
            IDbDataParameter parameter;
            parameter = cmd.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = dbType;
            parameter.Value = value;
            cmd.Parameters.Add(parameter);
        }


        public void StatelessExecuteSql(string sql)
        {
            CheckSqlInjection(sql,true);
            IStatelessSession session = GetStatelessSession();
            try
            {
                session.CreateSQLQuery(sql).ExecuteUpdate();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                session.Close();
            }
        }
        public void StatelessExecuteSql(String sql,IDictionary<string, Object> namedParameters)
        {
            CheckSqlInjection(sql,true);
            IStatelessSession session = GetStatelessSession();
            try
            {
                var q = session.CreateSQLQuery(sql);
                foreach (var namedParameter in namedParameters)
                {
                    q.SetParameter(namedParameter.Key, namedParameter.Value);
                }
                q.ExecuteUpdate();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                session.Close();
            }
        }
        public object StatelessGetScalarBySql(string sql)
        {
            CheckSqlInjection(sql);
            IStatelessSession session = GetStatelessSession();
            try{
                return session.CreateSQLQuery(sql).UniqueResult();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                session.Close();
            }
        }
        public IList<object[]> StatelessGetBySql(string sql)
        {
            CheckSqlInjection(sql);
            IStatelessSession session = GetStatelessSession();
            try{
                return session.CreateSQLQuery(sql).List<object[]>();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                session.Close();
            }
        }
        public IList<object[]> StatelessGetBySql(string sql, IDictionary<string, Object> namedParameters)
        {
            CheckSqlInjection(sql);
            IStatelessSession session = GetStatelessSession();
            try{
                var q = session.CreateSQLQuery(sql);
                foreach (var namedParameter in namedParameters)
                {
                    q.SetParameter(namedParameter.Key, namedParameter.Value);
                }
                var l = q.List<object[]>();
                return l;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                session.Close();
            }
        }
        public TEntity StatelessGetOne<TEntity>(Expression<Func<TEntity, bool>> where)
        {
            IStatelessSession session = GetStatelessSession();
            try
            {
                return session.Query<TEntity>().Where(where).SingleOrDefault();
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                session.Close();
            }
        }
        private IStatelessSession GetStatelessSession()
        {
            return NHibernateSessionModified.SessionFactories[NHibernateSession.DefaultFactoryKey].OpenStatelessSession();
        }
        private static void CheckSqlInjection(string sql,bool checkChange=false)
        {
            if(!_log.IsWarnEnabled) return;

            if (checkChange){
                sql = sql.ToLower();
                var words =new string[]{"delete","update","insert","create","drop","alter"};
                Array.ForEach(words,word=>
                                        {
                                            if(sql.Contains(word))
                                            {
                                                _log.Warn("changing word found in native sql:" + sql);
                                            }
                                        });
                
            }
            if(sql.Contains('\''))
                _log.Warn("single quote found in native sql:"+sql);
        }

    }


}
