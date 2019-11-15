using BTS.API.ASYNC.DatabaseContext;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;

namespace BTS.API.SERVICE.MD
{
    public class MdCityVm
    {
        public class Search : IDataSearch
        {
            public string CityId { get; set; }
            public string CityName { get; set; }
            public Nullable<int> Level { get; set; }
            public int Status { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdCity().CityId);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdCity();

                if (!string.IsNullOrEmpty(this.CityId))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.CityId),
                        Value = this.CityId,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.CityName))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.CityName),
                        Value = this.CityName,
                        Method = FilterMethod.Like
                    });
                }
                return result;
            }
            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }

            public void LoadGeneralParam(string summary)
            {
                CityId = summary;
                CityName = summary;
            }
        }
    }
}
