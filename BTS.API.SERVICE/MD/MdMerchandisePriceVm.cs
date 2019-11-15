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
    public class MdMerchandisePriceVm
    {
        public class Search : IDataSearch
        {
            public string MaVatTu { get; set; }
            public string MaDonVi { get; set; }
           

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdMerchandisePrice().UnitCode);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdMerchandisePrice();

                if (!string.IsNullOrEmpty(this.MaVatTu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaVatTu),
                        Value = this.MaVatTu,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaDonVi))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.UnitCode),
                        Value = this.MaDonVi,
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
                MaVatTu = summary;
                MaDonVi = summary;
            }


        }
    }
}
