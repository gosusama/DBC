using BTS.API.SERVICE.BuildQuery.Query.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Implimentations
{
    public class QueryFilterSQL : QueryFilterBase, IQueryFilter
    {
        public QueryFilterSQL()
        {
            IsUseProperty = false;
            SubFilters = new List<IQueryFilter>();
            QueryStringParams = new QueryFilterParamSQL();
        }

        private string _field;

        public override string Field
        {
            get
            {
                var result = _field;
                if (IsUseProperty)
                {
                    result = Property.Name;
                }
                return result;
            }
            set { _field = value; }
        }


        private string _switchLogic()
        {
            var result = "";
            if (SubFilters != null && SubFilters.Count > 0)
            {
                switch (Method)
                {
                    case FilterMethod.Not:
                        if (SubFilters != null && SubFilters.Count > 0)
                        {
                            var subQuery = SubFilters[0].Build();
                            result = string.IsNullOrEmpty(subQuery)
                                ? ""
                                : string.Format(" NOT ({0})", subQuery);
                        }
                        else
                        {
                            result = "";
                        }
                        break;

                    case FilterMethod.And:
                        result = _buildLogic("AND");
                        break;
                    case FilterMethod.Or:
                        result = _buildLogic("OR");
                        break;
                }
            }
            return result;
        }

        private string _buildLogic(string _key)
        {
            string result = "";
            int iCount = 0;
            foreach (var item in SubFilters)
            {
                item.QueryStringParams = QueryStringParams;
                var cmd = item.Build();
                if (!string.IsNullOrEmpty(cmd))
                {
                    iCount++;
                    result += string.Format("({0}) {1}", cmd, _key);
                }
            }
            if (iCount > 0)
            {
                // thừa 1 lần _key
                result = result.Substring(0, result.Length - _key.Length);
            }
            if (!string.IsNullOrWhiteSpace(result))
                result = string.Format("({0})", result);
            return result;
        }

        private string _switchCompareLogic()
        {
            var result = "";
            switch (Method)
            {
                case FilterMethod.Null:
                    result = string.Format("{0} IS NULL", Field);
                    break;
                case FilterMethod.NotNull:
                    result = string.Format("{0} IS NOT NULL", Field);
                    break;
            }
            return result;
        }

        private string _switchCompareValue()
        {
            var result = "";
            switch (Method)
            {
                case FilterMethod.All:
                    result = "";
                    break;

                case FilterMethod.EqualTo:
                    result = string.Format("LOWER({0}) = LOWER('{1}')", Field, GetNextParam());
                    break;
                case FilterMethod.NotEqualTo:
                    result = string.Format("{0} <> '{1}'", Field, GetNextParam());
                    break;
                case FilterMethod.LessThan:
                    result = string.Format("{0} < '{1}'", Field, GetNextParam());
                    break;
                case FilterMethod.NotLessThan:
                    result = string.Format("{0} !< '{1}'", Field, GetNextParam());
                    break;
                case FilterMethod.GreaterThan:
                    result = string.Format("{0} > '{1}'", Field, GetNextParam());
                    break;
                case FilterMethod.NotGreaterThan:
                    result = string.Format("{0} !> '{1}'", Field, GetNextParam());
                    break;
                case FilterMethod.LessThanOrEqualTo:
                    result = string.Format("{0} <= '{1}'", Field, GetNextParam());
                    break;
                case FilterMethod.GreaterThanOrEqualTo:
                    result = string.Format("{0} >= '{1}'", Field, GetNextParam());
                    break;


                //case FilterMethod.StartsWith:
                //    IsAlwaysCheckNotNull = true;
                //    result = string.Format("{0}.ToLower().StartsWith({1})", Field,
                //        GetNextParam(Value.ToString().ToLower()));
                //    break;
                case FilterMethod.Like:
                    IsAlwaysCheckNotNull = true;
                    result = string.Format("LOWER({0}) LIKE '%{1}%'", Field,
                        GetNextParam(Value.ToString().ToLower()));
                    break;
                case FilterMethod.NotLike:
                    IsAlwaysCheckNotNull = true;
                    result = string.Format("LOWER({0}) NOT LIKE {1}", Field,
                        GetNextParam(Value.ToString().ToLower()));
                    break;
                case FilterMethod.In:
                    result = string.Format("{0} IN ({1})", Field, GetNextParam());
                    break;
                case FilterMethod.NotIn:
                    result = string.Format("{0} NOT IN ({1})", Field, GetNextParam());
                    break;
            }
            if (!string.IsNullOrEmpty(result) && IsAlwaysCheckNotNull)
            {
                result = string.Format("({0} IS NOT NULL AND {1})", Field, result);
            }
            return result;
        }

        private object GetNextParam(object value = null)
        {
            value = value ?? Value;
            var result = value;
            if (ValueAsOtherField)
            {
                if (value is PropertyInfo)
                {
                    result = Property.Name;
                }
                else
                {
                    result = value;
                }
            }
            else
            {
                result = QueryStringParams.GetNextParam(value);
            }
            return result;
        }

        public override bool MergeFilter(IQueryFilter filter)
        {
            var result = false;
            if (filter != null && filter.IsCompleteQuery)
            {
                if (IsCompleteQuery)
                {
                    var originQuery = ConvertTo<QueryFilterSQL>(true);
                    this.Method = FilterMethod.And;
                    this.SubFilters = new List<IQueryFilter>
                    {
                        originQuery,
                        filter
                    };
                }
                else
                {
                    filter.MapTo(this, true);
                }
                result = true;
            }
            return result;
        }

        public override string Build()
        {
            string result = "";
            if (IsCompleteQuery)
            {
                if (IsLogicOperator)
                {
                    // not use "Attribute"
                    result = _switchLogic();
                }
                else if (!string.IsNullOrEmpty(Field))
                {
                    // use "Attribute"
                    if (IsNotUseValueOperator)
                    {
                        // not use "Value"
                        result = _switchCompareLogic();
                    }
                    else if (Value != null)
                    {
                        // use "Value"
                        result = _switchCompareValue();
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            return Build();
        }
    }
}
