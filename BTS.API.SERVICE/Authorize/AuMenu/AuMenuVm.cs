using System.Collections.Generic;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
namespace BTS.API.SERVICE.Authorize.AuMenu
{
    public class AuMenuVm
    {
        public class Search : IDataSearch
        {
            public string MenuIdCha { get; set; }
            public string MenuId { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public int Sort { get; set; }
            public int TrangThai { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new AU_MENU().MenuId);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new AU_MENU();

                if (!string.IsNullOrEmpty(this.MenuId))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MenuId),
                        Value = this.MenuId,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Title))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Title),
                        Value = this.Title,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MenuIdCha))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MenuIdCha),
                        Value = this.MenuIdCha,
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
                MenuIdCha = summary;
                MenuId = summary;
                Title = summary;
            }
        }
        public class Menu
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Parent { get; set; }
            public List<Menu> Children { get; set; }
        }
    }
}
