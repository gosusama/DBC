using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Query.MiscLinQ
{
    public class TextConverter : IConverter
    {
        public string MapTo(dynamic value)
        {
            var data = string.Format("{0}", value);
            var result = string.IsNullOrWhiteSpace(value) ? "" : string.Format("\"{0}\"", data.ToLower());
            return result;
        }
    }
}