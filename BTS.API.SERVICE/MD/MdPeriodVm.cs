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
    public class MdPeriodVm
    {
        public class Search : IDataSearch
        {
            public int Period { get; set; }
            public string Name { get; set; }

            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public int Year { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdPeriod().Period);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdPeriod();

                if (this.Period > 0)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Period),
                        Value = this.Period,
                        Method = FilterMethod.EqualTo
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
                if (this.Year > 0)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Year),
                        Value = this.Year,
                        Method = FilterMethod.EqualTo
                    });
                }

                if (this.FromDate.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.FromDate),
                        Value = this.FromDate.Value,
                        Method = FilterMethod.GreaterThanOrEqualTo
                    });
                }
                if (this.ToDate.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.ToDate),
                        Value = this.ToDate.Value.AddDays(1),
                        Method = FilterMethod.LessThan
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
                int _period, _year;
                if(int.TryParse(summary, out _period))
                {
                    Period = _period;
                }
                if (int.TryParse(summary, out _year))
                {
                    Year = _year;
                }
                Name = summary;
            }


        }

        public class Dto : DataInfoDtoVm
        {
            public int Period { get; set; }
            public string Name { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public int Year { get; set; }
            public int TrangThai { get; set; }
            public string TableName { get; set; }
        }
        public class ResponseData
        {
            public decimal SoLuongTonKhoXuat { get; set; }
            public decimal SoLuongTonKhoNhap { get; set; }
        }
        public class RequestCreatePeriod
        {
            public int Year { get; set; }
        }
    }
}
