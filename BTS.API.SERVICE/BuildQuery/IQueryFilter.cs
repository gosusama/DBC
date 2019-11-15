using System.Collections.Generic;
using System.Reflection;
using BTS.API.SERVICE.BuildQuery.Query.Types;

namespace BTS.API.SERVICE.BuildQuery
{
    public interface IQueryFilter
    {
        dynamic PropertyAndValue { set; }

        string Field { get; set; }

        PropertyInfo Property { get; set; }

        bool ValueAsOtherField { get; set; }

        dynamic Value { get; set; }

        FilterMethod Method { get; set; }

        bool IsAlwaysCheckNotNull { get; set; }

        IQueryFilterParam QueryStringParams { get; set; }

        List<IQueryFilter> SubFilters { get; set; }

        bool IsCompleteQuery { get; }

        bool MergeFilter(IQueryFilter filter);

        string Build();

        T ConvertTo<T>(bool keepSub = false) where T : IQueryFilter;
        void MapTo<T>(T filter, bool keepSub = false) where T : IQueryFilter;
    }
}