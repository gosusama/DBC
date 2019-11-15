using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System.Collections.Generic;
namespace BTS.API.SERVICE.MD
{
    public class MdDepartmentVm
    {
        public class Search : IDataSearch
        {
            public string MaPhong { get; set; }
            public string TenPhong { get; set; }
            public string ThongTinBoSung { get; set; }

           
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdDepartment().MaPhong);
                }
            }


            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdDepartment();

                if (!string.IsNullOrEmpty(this.MaPhong))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaPhong),
                        Value = this.MaPhong,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenPhong))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenPhong),
                        Value = this.TenPhong,
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
                MaPhong = summary;
                TenPhong = summary;

            }
        }
    }
}
