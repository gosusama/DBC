using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

namespace BTS.API.SERVICE.BuildQuery
{
    public interface IQueryBuilder
    {
        string CommandString { get; set; }

        bool TakeAll { get; set; }

        int Skip { get; set; }

        int Take { get; set; }

        dynamic ExtensionObject { get; set; }

        bool IsQueryLocal { get; set; }

        IQueryFilter Filter { get; set; }

        List<IQueryOrder> Orders { get; set; }

        IQueryBuilder OrderBy(IQueryOrder orderBy);

        string BuildFilter();

        string BuildOrder();

        string Build();

        TBuilder ConvertTo<TBuilder, TFilter, TOrder>(bool keepFilterAndOrder = false)
            where TBuilder : IQueryBuilder
            where TFilter : IQueryFilter
            where TOrder : IQueryOrder;

        void MapTo<TBuilder, TFilter, TOrder>(TBuilder query, bool keepFilterAndOrder = false)
            where TBuilder : IQueryBuilder
            where TFilter : IQueryFilter
            where TOrder : IQueryOrder;
    }
}