using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS.API.SERVICE.Services;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery.Query.Types;

namespace BTS.API.SERVICE.MD
{
    public class MdTypeReasonVm
    {
        public class Search : IDataSearch
        {
            public string MaLyDo { get; set; }
            public string TenLyDo { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdTypeReason().MaLyDo);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdTypeReason();

                if (!string.IsNullOrEmpty(this.MaLyDo))//MaTk
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaLyDo),
                        Value = this.MaLyDo,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenLyDo))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenLyDo),
                        Value = this.TenLyDo,
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
                TenLyDo = summary;
                MaLyDo = summary;
            }
        }
    }
}
