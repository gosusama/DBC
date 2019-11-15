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
    public class MdTaxVm
    {
        public class Search : IDataSearch
        {

            public string MaLoaiThue { get; set; }
            public string LoaiThue { get; set; }

            public string TaiKhoanKt { get; set; }

            public decimal TaxRate { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdTax().LoaiThue);
                }
            }


            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdTax();

                if (!string.IsNullOrEmpty(this.MaLoaiThue))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaLoaiThue),
                        Value = this.MaLoaiThue,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.LoaiThue))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.LoaiThue),
                        Value = this.LoaiThue,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TaiKhoanKt))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TaiKhoanKt),
                        Value = this.TaiKhoanKt,
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
                MaLoaiThue = summary;
                LoaiThue = summary;
                TaiKhoanKt = summary;
            }
        }
    }
    public class TDS_VatVm
    {
        public string Mavat { get; set; }
        public string Tenvat { get; set; }
        public int? Vat { get; set; }
        public string Congthuc { get; set; }
        public int? Loaivat { get; set; }
        public string Manguoitao { get; set; }
        public DateTime? Ngaytao { get; set; }
        public DateTime Ngayphatsinh { get; set; }
        public int? Khongchiuthue { get; set; }
        public string Doanhso { get; set; }
    }
}
