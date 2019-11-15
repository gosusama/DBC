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
    public class MdNhomVatTuVm
    {
        public class Search : IDataSearch
        {
            public string MaNhom { get; set; }
            public string TenNhom { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdNhomVatTu().TenNhom);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdNhomVatTu();

                if (!string.IsNullOrEmpty(this.MaLoaiVatTu))
                {
                    var codeTypes = this.MaLoaiVatTu.Split(',').ToList();

                    result.Add(new QueryFilterLinQ
                    {
                        Method = FilterMethod.And,
                        SubFilters = new List<IQueryFilter>()
                        {
                            new QueryFilterLinQ()
                            {
                                Property = ClassHelper.GetProperty(() => refObj.MaLoaiVatTu),
                                Value = codeTypes,
                                Method = FilterMethod.In
                            },
                            new QueryFilterLinQ()
                            {
                                Method = FilterMethod.Or,
                                SubFilters = new List<IQueryFilter>()
                                {
                                    new QueryFilterLinQ
                                    {
                                        Property = ClassHelper.GetProperty(() => refObj.TenNhom),
                                        Value = this.TenNhom,
                                        Method = FilterMethod.Like
                                    },
                                    new QueryFilterLinQ
                                    {
                                        Property = ClassHelper.GetProperty(() => refObj.MaNhom),
                                        Value = this.MaNhom,
                                        Method = FilterMethod.Like
                                    }
                                }
                            }
                        }
                    });
                }
                else
                {
                    result.Add(new QueryFilterLinQ()
                    {
                        Method = FilterMethod.Or,
                        SubFilters = new List<IQueryFilter>()
                                {
                                    new QueryFilterLinQ
                                    {
                                        Property = ClassHelper.GetProperty(() => refObj.TenNhom),
                                        Value = this.TenNhom,
                                        Method = FilterMethod.Like
                                    },
                                    new QueryFilterLinQ
                                    {
                                        Property = ClassHelper.GetProperty(() => refObj.MaNhom),
                                        Value = this.MaNhom,
                                        Method = FilterMethod.Like
                                    }
                                }
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
                MaNhom = summary;
                TenNhom = summary;
            }


        }
        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
            }
            public string MaLoaiVatTu { get; set; }
            public string Id { get; set; }
            public string MaNhom { get; set; }
            public string TenNhom { get; set; }
            public bool IsGenCode { get; set; }
            public int TrangThai { get; set; }
        }
    }
}
