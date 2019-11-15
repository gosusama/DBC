using System;
using System.Collections.Generic;
using System.Reflection;
using BTS.API.SERVICE.BuildQuery.Query.Types;

namespace BTS.API.SERVICE.BuildQuery
{
    public abstract class QueryFilterBase : IQueryFilter
    {
        protected QueryFilterBase()
        {
            IsUseProperty = false;
            SubFilters = new List<IQueryFilter>();
            QueryStringParams = new QueryFilterParam();
        }

        public abstract string Field { get; set; }

        public abstract string Build();

        public abstract bool MergeFilter(IQueryFilter filter);

        public dynamic PropertyAndValue
        {
            set
            {
                if (value != null)
                {
                    var property = (PropertyInfo) value.Property;
                    var val = value.Value;
                    if (property == null || val == null)
                    {
                        throw new Exception("Empty Property or Value");
                    }
                    Property = property;
                    Value = val;
                }
            }
        }

        private PropertyInfo _property;

        public PropertyInfo Property
        {
            get { return _property; }
            set
            {
                _property = value;
                IsUseProperty = _property != null;
            }
        }

        public bool ValueAsOtherField { get; set; }

        public object Value { get; set; }

        public FilterMethod Method { get; set; }

        public bool IsAlwaysCheckNotNull { get; set; }

        public IQueryFilterParam QueryStringParams { get; set; }

        public List<IQueryFilter> SubFilters { get; set; }

        public bool IsUseProperty { get; set; }

        public bool IsLogicOperator
        {
            get
            {
                var result = false;
                switch (Method)
                {
                    case FilterMethod.Not:
                        result = true;
                        break;
                    case FilterMethod.Or:
                        result = true;
                        break;
                    case FilterMethod.And:
                        result = true;
                        break;
                }
                return result;
            }
        }

        public bool IsNotUseValueOperator
        {
            get
            {
                var result = false;
                switch (Method)
                {
                    case FilterMethod.Null:
                        result = true;
                        break;
                    case FilterMethod.NotNull:
                        result = true;
                        break;
                }
                return result;
            }
        }

        public bool IsCompleteQuery
        {
            get
            {
                var result = Method == FilterMethod.All
                             || (SubFilters != null && SubFilters.Count > 0)
                             || (!string.IsNullOrEmpty(Field) &&
                                 (Value != null || IsNotUseValueOperator));
                return result;
            }
        }

        public T ConvertTo<T>(bool keepSub = false) where T : IQueryFilter
        {
            var result = Activator.CreateInstance<T>();
            MapTo(result, keepSub);
            return result;
        }

        public void MapTo<T>(T filter, bool keepSub = false) where T : IQueryFilter
        {
            if (filter != null)
            {
                filter.Field = Field;
                filter.Property = Property;
                filter.Value = Value;
                filter.Method = Method;
                filter.SubFilters = new List<IQueryFilter>();
                if (keepSub && typeof (T) == GetType())
                    SubFilters.ForEach(item => filter.SubFilters.Add(item));
                else
                    SubFilters.ForEach(item => filter.SubFilters.Add(item.ConvertTo<T>()));
            }
        }
    }
}