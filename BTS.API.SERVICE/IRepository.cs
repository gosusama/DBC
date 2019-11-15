using BTS.API.ENTITY;
using BTS.API.SERVICE;
using BTS.API.SERVICE.BuildQuery;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;


namespace BTS.API.SERVICE
{
    public interface IRepository<TEntity> where TEntity : EntityBase
    {
        Guid InstanceId { get; }
        IDataContext DataContext { get; }
        DbSet<TEntity> DbSet { get; }

        TEntity Find(params object[] keyValues);
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        IQueryable<TEntity> SqlQuery(string query, params object[] parameters);
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        void InsertGraph(TEntity entity);
        void InsertGraphRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entity);
        List<TEntity> Query(IQueryBuilder query, List<Expression<Func<TEntity, object>>> includes = null);
        PagedObj<TEntity> QueryPaged(IQueryBuilder query, List<Expression<Func<TEntity, object>>> includes = null);
        Task<List<TEntity>> QueryAsync(IQueryBuilder query, List<Expression<Func<TEntity, object>>> includes = null);
        Task<PagedObj<TEntity>> QueryPagedAsync(IQueryBuilder query, List<Expression<Func<TEntity, object>>> includes = null);
    }
}