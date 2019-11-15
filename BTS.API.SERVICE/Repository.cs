#region

using BTS.API.ENTITY;
using BTS.API.SERVICE;
using BTS.API.SERVICE.BuildQuery;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;


#endregion

namespace BTS.API.SERVICE
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly IDataContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly Guid _instanceId;
        private readonly List<Expression<Func<TEntity, object>>> _includeProperties;

        public Repository(IDataContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _instanceId = Guid.NewGuid();
        }

        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        public IDataContext DataContext
        {
            get { return _context; }
        }

        public DbSet<TEntity> DbSet
        {
            get { return _dbSet; }
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await _dbSet.FindAsync(cancellationToken, keyValues);
        }

        public virtual IQueryable<TEntity> SqlQuery(string query, params object[] parameters)
        {
            return _dbSet.SqlQuery(query, parameters).AsQueryable();
        }

        public virtual void Insert(TEntity entity)
        {
            ((IObjectState)entity).ObjectState = ObjectState.Added;
            _dbSet.Attach(entity);
            _context.SyncObjectState(entity);
        }

        public virtual void InsertRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Insert(entity);
        }

        public virtual void InsertGraph(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void InsertGraphRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            ((IObjectState)entity).ObjectState = ObjectState.Modified;
            _dbSet.Attach(entity);
            _context.SyncObjectState(entity);
        }

        public virtual void Delete(object id)
        {
            var entity = _dbSet.Find(id);
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            ((IObjectState)entity).ObjectState = ObjectState.Deleted;
            _dbSet.Attach(entity);
            _context.SyncObjectState(entity);
        }



        internal IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includeProperties != null)
                includeProperties.ForEach(i => query = query.Include(i));

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }

        internal async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            return Get(filter, orderBy, includeProperties, page, pageSize).AsEnumerable();
        }


        internal TEntity Find(
            List<Expression<Func<TEntity, object>>> includeProperties = null,
            params object[] keyValues)
        {
            var result = Find(keyValues);
            if (includeProperties != null)
                includeProperties.ForEach(i => ((DataContext)_context).Entry(result).Reference(i));

            return result;
        }

        internal async Task<TEntity> FindAsync(
            List<Expression<Func<TEntity, object>>> includeProperties = null,
            params object[] keyValues)
        {
            var result = Find(includeProperties, keyValues);
            return result;
        }


        public IQueryable<TEntity> Get(IQueryBuilder query, List<Expression<Func<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> result = DbSet.AsQueryable();
            var filterString = query.BuildFilter();
            var orderString = query.BuildOrder();
            if (!string.IsNullOrEmpty(filterString))
                result = result.Where(filterString, query.Filter.QueryStringParams.Params.ToArray());
            if (!string.IsNullOrEmpty(orderString))
                result = result.OrderBy(orderString);
            if (includes != null)
                includes.ForEach(i => result = result.Include(i));
            return result;
        }


        public List<TEntity> Query(IQueryBuilder query, List<Expression<Func<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> data = Get(query, includes);
            if (query.Skip > 0)
            {
                data = data.Skip(query.Skip);
            }
            if (query.Take > 0)
            {
                data = data.Take(query.Take);
            }
            var result = data.ToList();
            return result;
        }

        public PagedObj<TEntity> QueryPaged(IQueryBuilder query, List<Expression<Func<TEntity, object>>> includes = null)
        {
            var result = new PagedObj<TEntity>
            {
                ItemsPerPage = query.Take,
                FromItem = query.Skip + 1,
            };
            IQueryable<TEntity> data = Get(query, includes);
            result.TotalItems = data.Count();
            if (query.Skip > 0)
            {
                data = data.Skip(query.Skip);
            }
            if (query.Take > 0)
            {
                data = data.Take(query.Take);
            }
            result.Data.AddRange(data.ToList());
            return result;
        }

        public async Task<List<TEntity>> QueryAsync(IQueryBuilder query,
            List<Expression<Func<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> data = Get(query, includes);
            if (query.Skip > 0)
            {
                data = data.Skip(query.Skip);
            }
            if (query.Take > 0)
            {
                data = data.Take(query.Take);
            }
            var result = data.ToListAsync();
            return await result;
        }
        public async Task<PagedObj<TEntity>> QueryPagedAsync(IQueryBuilder query,
            List<Expression<Func<TEntity, object>>> includes = null)
        {
            var result = new PagedObj<TEntity> { ItemsPerPage = query.Take };
            IQueryable<TEntity> data = Get(query, includes);
            result.TotalItems = await data.CountAsync();
            if (query.Skip > 0)
            {
                data = data.Skip(query.Skip);
            }
            if (query.Take > 0)
            {
                data = data.Take(query.Take);
            }
            result.Data.AddRange(await data.ToListAsync());
            return result;
        }

        protected int ExecuteSqlCommand(string sqlCommand)
        {
            var result = ((DataContext)_context).Database.ExecuteSqlCommand(sqlCommand);
            return result;
        }

        protected List<T> ExecuteSqlQuery<T>(string sqlCommand)
        {
            var result = ((DataContext)_context).Database.SqlQuery<T>(sqlCommand);
            return result.ToList();
        }

        protected async Task<int> ExecuteSqlCommandAsync(string sqlCommand)
        {
            var result = ((DataContext)_context).Database.ExecuteSqlCommand(sqlCommand);
            return result;
        }

        protected async Task<List<T>> ExecuteSqlQueryAsync<T>(string sqlCommand)
        {
            var result = ((DataContext)_context).Database.SqlQuery<T>(sqlCommand);
            return await result.ToListAsync();
        }
    }
}