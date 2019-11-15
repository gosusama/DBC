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
    public class MdCurrencyVm
    {
      
        public class Search : IDataSearch
        {
            public string MaNgoaiTe { get; set; }
            public string TenNgoaiTe { get; set; }


            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdCurrency().TenNgoaiTe);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdCurrency();

                if (!string.IsNullOrEmpty(this.MaNgoaiTe))//MaTk
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaNgoaiTe),
                        Value = this.MaNgoaiTe,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenNgoaiTe))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenNgoaiTe),
                        Value = this.TenNgoaiTe,
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
                MaNgoaiTe = summary;//MaTk
                TenNgoaiTe = summary;
               
            }


        }
    }
}
