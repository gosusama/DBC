using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Implimentations;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE
{
    public class ProcedureService<TEntity> : RepositoryProcedure<TEntity> where TEntity : class
    {
        public virtual ResultObj<PagedObj<TEntity>> Filter<TSearch>(
    FilterObj<TSearch> filtered , IQueryable<TEntity> dataInit,
    IQueryBuilder query = null)
    where TSearch : IDataSearch
        {
            query = query ?? new QueryBuilder();
            // load filter
            if (true)
            {

            }
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
                var queryData = QueryPaged(query, dataInit, null);
                result.Value = queryData;
                result.State = ResultState.Success;
            }
            catch (Exception exception)
            {
                result.SetException = exception;
            }
            return result;
        }

        public virtual IQueryFilter FilterSQL<TSearch>(
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
                var newQuery = new QueryFilterSQL
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
                var newQuery = new QueryFilterSQL
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

            var result = query.Filter;
            return result;
        }
    }
}
