using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BTS.API.SERVICE.BuildQuery
{
    public class QueryBuilder : IQueryBuilder
    {
        public QueryBuilder()
        {
            Orders = new List<IQueryOrder>();
        }

        public string CommandString { get; set; }

        public bool TakeAll { get; set; }


        private int _skip;

        public int Skip
        {
            get { return TakeAll ? 0 : _skip; }
            set { _skip = value; }
        }

        private int _take;

        public int Take
        {
            get { return TakeAll ? 0 : _take; }
            set { _take = value; }
        }

        public dynamic ExtensionObject { get; set; }

        public bool IsQueryLocal { get; set; }

        public IQueryFilter Filter { get; set; }

        public IQueryOrder AddOrder
        {
            set { }
        }

        public List<IQueryOrder> Orders { get; set; }

        public IQueryBuilder OrderBy(IQueryOrder orderBy)
        {
            if (orderBy != null && Orders.All(item => item.Field != orderBy.Field))
            {
                Orders.Add(orderBy);
            }
            return this;
        }

        public string BuildFilter()
        {
            var result = Filter != null && Filter.IsCompleteQuery
                ? Filter.Build()
                : "";
            return result;
        }

        public string BuildOrder()
        {
            string result = "";
            if (Orders.Count > 0)
            {
                foreach (var item in Orders)
                {
                    result += string.Format("{0},", item);
                }
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        public string Build()
        {
            string result = string.Format("{0} {1}", BuildFilter(), BuildOrder());
            return result;
        }

        public TBuilder ConvertTo<TBuilder, TFilter, TOrder>(bool keepFilterAndOrder = false)
            where TBuilder : IQueryBuilder
            where TFilter : IQueryFilter
            where TOrder : IQueryOrder
        {
            var result = Activator.CreateInstance<TBuilder>();
            MapTo<TBuilder, TFilter, TOrder>(result, keepFilterAndOrder);
            return result;
        }

        public void MapTo<TBuilder, TFilter, TOrder>(TBuilder query, bool keepFilterAndOrder = false)
            where TBuilder : IQueryBuilder
            where TFilter : IQueryFilter
            where TOrder : IQueryOrder
        {
            if (query != null)
            {
                query.CommandString = CommandString;
                query.Skip = Skip;
                query.Take = Take;
                query.ExtensionObject = ExtensionObject;
                query.IsQueryLocal = IsQueryLocal;
                if (keepFilterAndOrder)
                {
                    query.Filter = Filter;
                    query.Orders = Orders;
                }
                else
                {
                    query.Filter = Filter.ConvertTo<TFilter>();
                    query.Orders = Orders.Select(item => item.ConvertTo<TOrder>() as IQueryOrder).ToList();
                }
            }
        }

        public override string ToString()
        {
            string result = string.IsNullOrEmpty(CommandString) ? Build() : CommandString;
            return result;
        }
    }
}