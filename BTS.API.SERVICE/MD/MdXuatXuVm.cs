using BTS.API.ASYNC.DatabaseContext;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;

namespace BTS.API.SERVICE.MD
{
    public class MdXuatXuVm
    {
        public class Search : IDataSearch
        {
            public string MaXuatXu { get; set; }
            public string TenXuatXu { get; set; }
            public int TrangThai { get; set; }
            public string GhiChu { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdXuatXu().MaXuatXu);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdXuatXu();

                if (!string.IsNullOrEmpty(this.MaXuatXu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaXuatXu),
                        Value = this.MaXuatXu,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenXuatXu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenXuatXu),
                        Value = this.TenXuatXu,
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
                MaXuatXu = summary;
                TenXuatXu = summary;
            }
        }
        public class Dto
        {
            public string Id { get; set; }
            public string MaXuatXu { get; set; }
            public string TenXuatXu { get; set; }
            public int TrangThai { get; set; }
            public string GhiChu { get; set; }
            public bool IsGenCode { get; set; }

        }
    }
}
