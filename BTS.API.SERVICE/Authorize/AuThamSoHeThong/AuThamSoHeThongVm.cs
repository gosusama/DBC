using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System.Collections.Generic;
namespace BTS.API.SERVICE.Authorize.AuThamSoHeThong
{
    public class AuThamSoHeThongVm
    {
        public class Search : IDataSearch
        {
            public string MaThamSo { get; set; }
            public string TenThamSo { get; set; }
            public int GiaTriThamSo { get; set; }
            public int KieuDuLieu { get; set; }
            public int IsEdit { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new AU_THAMSOHETHONG().MaThamSo);
                }
            }
            public void LoadGeneralParam(string summary)
            {
                MaThamSo = summary;
                TenThamSo = summary;
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new AU_THAMSOHETHONG();
                if (!string.IsNullOrEmpty(this.MaThamSo))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaThamSo),
                        Value = this.MaThamSo,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenThamSo))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenThamSo),
                        Value = this.TenThamSo,
                        Method = FilterMethod.Like
                    });
                }
                return result;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }
        }

        public class Dto
        {
            public string MaThamSo { get; set; }
            public string TenThamSo { get; set; }
            public int GiaTriThamSo { get; set; }
            public int KieuDuLieu { get; set; }
            public int IsEdit { get; set; }
        }
    }
}
