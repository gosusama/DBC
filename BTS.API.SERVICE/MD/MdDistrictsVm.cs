using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;

namespace BTS.API.SERVICE.MD
{
    public class MdDistrictsVm
    {
        public class Search : IDataSearch
        {
            public string CityId { get; set; }
            public string DistrictsId { get; set; }
            public string DistrictsName { get; set; }
            public Nullable<int> Level { get; set; }
            public int Status { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdDistricts().CityId);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdDistricts();

                if (!string.IsNullOrEmpty(this.CityId))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.CityId),
                        Value = this.CityId,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.DistrictsId))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.DistrictsId),
                        Value = this.DistrictsId,
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
                DistrictsId = summary;
                DistrictsName = summary;
            }
        }
    }
}
