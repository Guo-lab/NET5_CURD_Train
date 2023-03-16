using ProjectBase.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data;
using SharpArch.Domain.DomainModel;

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

        int StatelessDelete<TEntity, TId>(Expression<Func<TEntity, bool>> where) where TEntity: IEntityWithTypedId<TId>;
        int StatelessDelete<TEntity, TId>(TId entityId, Expression<Func<TEntity, bool>> extraWhere) where TEntity : IEntityWithTypedId<TId>;
        TScalar StatelessGetScalar<TEntity, TScalar>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TScalar>> selector, bool unique = false);
        IList<TDto> StatelessGetDtoList<TEntity, TDto>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TDto>> selector);
        IList<TEntity> StatelessGetList<TEntity>(Expression<Func<TEntity, bool>> where);
    }
}
