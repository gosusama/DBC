using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System.Collections.Generic;
namespace BTS.API.SERVICE.MD
{
    public class MdColorVm
    {
        public class Search : IDataSearch
        {

            public string MaColor { get; set; }
            public string TenColor { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdColor().MaColor);
                }
            }


            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdColor();

                if (!string.IsNullOrEmpty(this.MaColor))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaColor),
                        Value = this.MaColor,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenColor))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenColor),
                        Value = this.TenColor,
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
                MaColor = summary;
                TenColor = summary;
            }
        }
        public class Dto : DataInfoDtoVm
        {
            public string MaColor { get; set; }
            public string TenColor { get; set; }
            public int TrangThai { get; set; }
            public bool IsGenCode { get; set; }
        }
    }
}
