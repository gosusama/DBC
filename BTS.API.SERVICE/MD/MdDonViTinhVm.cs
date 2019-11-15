using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System.Collections.Generic;

namespace BTS.API.SERVICE.MD
{
    public class MdDonViTinhVm
    {
        public class Search : IDataSearch
        {

            public string MaDVT { get; set; }
            public string TenDVT { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdDonViTinh().TenDVT);
                }
            }


            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdDonViTinh();

                if (!string.IsNullOrEmpty(this.MaDVT))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaDVT),
                        Value = this.MaDVT,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenDVT))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenDVT),
                        Value = this.TenDVT,
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
                MaDVT = summary;
                TenDVT = summary;
            }
        }
        public class Dto : DataInfoDtoVm
        {
            public string Id { get; set; }
            public string MaDVT { get; set; }
            public string TenDVT { get; set; }
            public bool IsGenCode { get; set; }
        }
    }
}
