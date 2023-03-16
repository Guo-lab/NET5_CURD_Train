using ProjectBase.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data;
namespace ProjectBase.Domain
{
    public interface IUtilQuery
    {
        Object GetScalarBySql(string sql);
        IList<string> GetStringListBySql(string sql);
        IList<Object[]> GetBySql(string sql);
        IList<Object[]> GetBySql(string sql, IDictionary<string, Object> namedParameters);
        IList<TDto> GetBySql<TDto>(string sql);
        IList<TDto> GetBySql<TDto>(string sql, IDictionary<string, Object> namedParameters);
        IList<T> GetEntityBySql<T>(string sql);
        int ExcuteSql(string sql);
        IList<TDto> GetFromDBObj<TDto>(string dbobjectname);
        IList<TDto> GetFromDBObj<TDto>(string dbobjectname, string appendsql);
        void ExecuteProcedureNonQuery(string procedureName, IList<ProcedureParameter> parameters);
        DataSet ExecuteProcedureDataSet(string procedureName, IList<ProcedureParameter> parameters);
        void StatelessExecuteSql(string sql);
        void StatelessExecuteSql(string sql, IDictionary<string, object> namedParameters);
        object StatelessGetScalarBySql(string sql);
        IList<object[]> StatelessGetBySql(string sql);
        IList<object[]> StatelessGetBySql(string sql, IDictionary<string, object> namedParameters);
        TEntity StatelessGetOne<TEntity>(Expression<Func<TEntity, bool>> where);
    }
}
