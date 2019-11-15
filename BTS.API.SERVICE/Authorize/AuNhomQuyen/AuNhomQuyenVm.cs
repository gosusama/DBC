using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System.Collections.Generic;
namespace BTS.API.SERVICE.Authorize.AuNhomQuyen
{
    public class AuNhomQuyenVm
    {
        public class Search : IDataSearch
        {
            public string MANHOMQUYEN { get; set; }
            public string TENNHOMQUYEN { get; set; }
            public string MOTA { get; set; }
            
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new AU_NHOMQUYEN().MANHOMQUYEN);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new AU_NHOMQUYEN();

                if (!string.IsNullOrEmpty(this.MANHOMQUYEN))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MANHOMQUYEN),
                        Value = this.MANHOMQUYEN,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TENNHOMQUYEN))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TENNHOMQUYEN),
                        Value = this.TENNHOMQUYEN,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MOTA))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MOTA),
                        Value = this.MOTA,
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
                MOTA = summary;
                MANHOMQUYEN = summary;
                TENNHOMQUYEN = summary;
            }
        }
    }
}
