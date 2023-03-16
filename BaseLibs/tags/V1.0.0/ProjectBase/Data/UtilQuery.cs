using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Data.SqlClient;
//using Microsoft.Data.SqlClient;

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


        //public ProcedureCall CreateSPCall(String spName)
        //{
        //    ProcedureCall sp = getSession().createStoredProcedureCall(spName);
        //    return sp;
        //}
        //public void ExcuteSPCall(ProcedureCall sp)
        //{
        //    String outputError=(String)sp.getOutputs().getOutputParameterValue(sp.getRegisteredParameters().size()-1);
        //    if(StringUtils.isNotEmpty(outputError))
        //        throw new JDBCException("generated by Procedure call with output parameter indicating sql errors",new SQLException(outputError));
        //}
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

        public void ExecuteProcedureNonQuery(string psProcedure,IList<ProcedureParameter> parrProcedureParameters)
        {
            ISession iSession = this.Session;
            IDbConnection oConn = iSession.Connection;
            IDbCommand oCmd = oConn.CreateCommand();
            oCmd.CommandType = CommandType.StoredProcedure;
            oCmd.CommandText = psProcedure;
            parrProcedureParameters.ToList().ForEach(x=> this.PrepareParameter(oCmd,x.ParameterName,x.DbType,x.Value));

            if (oConn.State == ConnectionState.Closed)
            {
                oConn.Open();
            }
            oCmd.Connection = oConn;
            oCmd.ExecuteNonQuery();
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
            ITransaction transaction = iSession.GetCurrentTransaction();
            if (transaction != null) { 
                transaction.Enlist(cmd);
            }

            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = (SqlCommand)cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }

        private void PrepareParameter(IDbCommand poCmd, String psParameterName, System.Data.DbType poDBType, Object poValue)
        {
            IDbDataParameter oParameter;
            oParameter = poCmd.CreateParameter();
            oParameter.ParameterName = psParameterName;
            oParameter.DbType = poDBType;
            oParameter.Value = poValue;
            poCmd.Parameters.Add(oParameter);
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
