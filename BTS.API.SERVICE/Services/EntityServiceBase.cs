
using BTS.API.ENTITY;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;


namespace BTS.API.SERVICE.Services
{
    public class EntityServiceBase<TEntity> : ServiceBase, IEntityService<TEntity>
        where TEntity : EntityBase
    {
        public EntityServiceBase(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            Includes = new List<Expression<Func<TEntity, object>>>();
        }

        public virtual IRepository<TEntity> Repository
        {
            get { return UnitOfWork.Repository<TEntity>(); }
        }

        public virtual List<Expression<Func<TEntity, object>>> Includes { get; private set; }

        public virtual IEntityService<TEntity> Include(Expression<Func<TEntity, object>> include)
        {
            Includes.Add(include);
            ((IQueryable<TEntity>) Repository.DbSet).Include(include);
            return this;
        }
        public virtual ResultObj<PagedObj<TEntity>> Filter<TSearch>(
            FilterObj<TSearch> filtered,
            IQueryBuilder query = null)
            where TSearch : IDataSearch
        {
            query = query ?? new QueryBuilder();
            // load filter
            var advanceData = filtered.AdvanceData;
            if (!filtered.IsAdvance)
            {
                advanceData.LoadGeneralParam(filtered.Summary);
            }
            var filters = advanceData.GetFilters();
            if (filters.Count > 0)
            {
                var newQuery = new QueryFilterLinQ
                {
                    Method = filtered.IsAdvance ? FilterMethod.And : FilterMethod.Or,
                    SubFilters = filters,
                };
                if (query.Filter == null)
                {
                    query.Filter = newQuery;
                }
                else
                {
                    query.Filter.MergeFilter(newQuery);
                }
            }
            var quickFilters = advanceData.GetQuickFilters();
            if (quickFilters != null && quickFilters.Any())
            {
                var newQuery = new QueryFilterLinQ
                {
                    Method = FilterMethod.And,
                    SubFilters = quickFilters,
                };
                if (query.Filter == null)
                {
                    query.Filter = newQuery;
                }
                else
                {
                    query.Filter.MergeFilter(newQuery);
                }
            }
            // load order 
            if (!string.IsNullOrEmpty(filtered.OrderBy))
            {
                query.OrderBy(new QueryOrder
                {
                    Field = filtered.OrderBy,
                    MethodName = filtered.OrderType
                });
            }
            // at lease one order for paging
            if (query.Orders.Count == 0)
            {
                query.OrderBy(new QueryOrder { Field = advanceData.DefaultOrder });
            }
            // query
            var result = new ResultObj<PagedObj<TEntity>>();
            try
            {
                var queryData = UnitOfWork.Repository<TEntity>().QueryPaged(query, Includes);
                result.Value = queryData;
                result.State = ResultState.Success;
            }
            catch (Exception exception)
            {
                result.SetException = exception;
            }
            return result;
        }
  
    }
}