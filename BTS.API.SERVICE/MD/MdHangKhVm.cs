using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System.Collections.Generic;
namespace BTS.API.SERVICE.MD
{
    public class MdHangKhVm
    {
        public class Search : IDataSearch
        {
            public string MaHangKh { get; set; }
            public string TenHangKh { get; set; }
            public int SoDiem { get; set; }

            //public decimal TienGiamGia { get; set; }
            public decimal TyLeGiamGiaSn { get; set; }

            public decimal TyLeGiamGia { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdHangKH().MaHangKh);
                }
            }


            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdHangKH();

                if (!string.IsNullOrEmpty(this.MaHangKh))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaHangKh),
                        Value = this.MaHangKh,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenHangKh))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenHangKh),
                        Value = this.TenHangKh,
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
                MaHangKh = summary;
                TenHangKh = summary;

            }
        }
    }
}