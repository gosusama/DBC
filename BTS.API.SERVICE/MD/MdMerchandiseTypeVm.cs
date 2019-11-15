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
    public class MdMerchandiseTypeVm
    {
        public class Search : IDataSearch
        {
            public string MaLoaiVatTu { get; set; }
            public string TenLoaiVatTu { get; set; }

            //public string MANHACC { get; set; }

            //public string TkNo1 { get; set; }
            //public string TkNo2 { get; set; }
            //public string TkNo3 { get; set; }
            //public string TkCo1 { get; set; }
            //public string TkCo2 { get; set; }
            //public string TkCo3 { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdMerchandiseType().TenLoaiVatTu);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdMerchandiseType();

                if (!string.IsNullOrEmpty(this.MaLoaiVatTu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaLoaiVatTu),
                        Value = this.MaLoaiVatTu,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenLoaiVatTu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenLoaiVatTu),
                        Value = this.TenLoaiVatTu,
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
                MaLoaiVatTu = summary;
                TenLoaiVatTu = summary;
            }


        }
        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
            }
            public string Id { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string TenLoaiVatTu { get; set; }
            public bool IsGenCode { get; set; }
            public int TrangThai { get; set; }
        }
        public class ObjectTranfer
        {
            public string Id { get; set; }
            public string Value { get; set; }
            public string ExtendValue { get; set; }
            public string Text { get; set; }
        }
    }
}
