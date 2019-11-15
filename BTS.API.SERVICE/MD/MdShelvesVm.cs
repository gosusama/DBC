using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery.Query.Types;

namespace BTS.API.SERVICE.MD
{
    public class MdShelvesVm
    {
        public class Search : IDataSearch
        {
            public string MaKeHang { get; set; }
            public string TenKeHang { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdShelves().TenKeHang); 
                }
            }


            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdShelves();
                
                if (!string.IsNullOrEmpty(this.MaKeHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKeHang),
                        Value = this.MaKeHang,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenKeHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenKeHang),
                        Value = this.TenKeHang,
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
                MaKeHang = summary;
                TenKeHang = summary;
            }
        }

    }
}
