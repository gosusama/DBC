using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System.Collections.Generic;
namespace BTS.API.SERVICE.MD
{
    public class MdChietKhauKhVm
    {
    public class Search : IDataSearch
        {
            public string MaChietKhau { get; set; }
           
            public decimal TienTu { get; set; }

            public decimal TienDen { get; set; }

            public decimal TyLeChietKhau { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdChietKhauKH().MaChietKhau);
                }
            }


            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdChietKhauKH();

                if (!string.IsNullOrEmpty(this.MaChietKhau))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaChietKhau),
                        Value = this.MaChietKhau,
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
                MaChietKhau = summary;
                

            }
        }
    }
}
