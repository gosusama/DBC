using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.Authorize
{
    public class ClientVm
    {
        public class Search : IDataSearch
        {
            public string Secret { get; set; }
            public string Name { get; set; }
            public ApplicationTypes ApplicationType { get; set; }
            public bool Active { get; set; }
            public int RefreshTokenLifeTime { get; set; }
            public string AllowedOrigin { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new Client().Name);
                }
            }


            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new Client();

                if (!string.IsNullOrEmpty(this.Name))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Name),
                        Value = this.Name,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.AllowedOrigin))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.AllowedOrigin),
                        Value = this.AllowedOrigin,
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
                Name = summary;
                AllowedOrigin = summary;
            }
        }
    }
}
