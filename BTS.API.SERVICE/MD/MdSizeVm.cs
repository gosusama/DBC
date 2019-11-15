using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.MD
{
    public class MdSizeVm
    {
        public class Search : IDataSearch
        {

            public string MaSize { get; set; }
            public string TenSize { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdSize().TenSize);
                }
            }


            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdSize();

                if (!string.IsNullOrEmpty(this.MaSize))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaSize),
                        Value = this.MaSize,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenSize))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenSize),
                        Value = this.TenSize,
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
                MaSize = summary;
                TenSize = summary;
            }
        }

        public class Dto : DataInfoDtoVm
        {
            public string MaSize { get; set; }
            public string TenSize { get; set; }
            public string Id { get; set; }
            public int TrangThai { get; set; }
            public bool IsGenCode { get; set; }
        }
    }
}
