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
    public class MdSellingMachineVm
    {
        public class Search : IDataSearch
        {
            public string Code { get; set; }
            public string Name { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdSellingMachine().Code);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdSellingMachine();

                if (!string.IsNullOrEmpty(this.Name))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Code),
                        Value = this.Code,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Name))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Name),
                        Value = this.Name,
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
                Code = summary;
                Name = summary;
            }


        }

        public class Dto : DataInfoDtoVm
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public int TrangThai { get; set; }
            public string HoatDong { get; set; }
        }
    }
}
