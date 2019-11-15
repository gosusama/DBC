using BTS.API.ENTITY;
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


namespace BTS.API.SERVICE
{
    public class RepositoryProcedure<TEntity> where TEntity : class 
    {

        public IQueryable<TEntity> Get(IQueryBuilder query, IQueryable<TEntity> dataInit, List<Expression<Func<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> result = dataInit;
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


        public List<TEntity> Query(IQueryBuilder query, IQueryable<TEntity> dataInit, List<Expression<Func<TEntity, object>>> includes = null)
        {
            IQueryable<TEntity> data = Get(query, dataInit, includes);
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

        public PagedObj<TEntity> QueryPaged(IQueryBuilder query, IQueryable<TEntity> dataInit, List<Expression<Func<TEntity, object>>> includes = null)
        {
            var result = new PagedObj<TEntity>
            {
                ItemsPerPage = query.Take,
                FromItem = query.Skip + 1,
            };
            IQueryable<TEntity> data = Get(query, dataInit, includes);
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

    }
}
