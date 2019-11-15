using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Implimentations
{
    public class QueryFilterParamSQL : IQueryFilterParam
    {
        public QueryFilterParamSQL()
        {
            Params = new List<object>();
        }

        public int Count { get; set; }
        public List<object> Params { get; set; }

        public string GetNextParam(object param = null)
        {
            var result = "";
            if (param != null)
            {
                if (Params.Contains(param))
                {
                    result = string.Format("'{0}'", Params.IndexOf(param));
                }
                else
                {
                    result = string.Format("'{0}'", Count++);
                    Params.Add(param);
                }
            }
            else
            {
                result = string.Format("'{0}'", Count++);
            }
            return result;
        }
    }
}
