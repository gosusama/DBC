using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Query.Misc
{
    public class DefaultConverter : IConverter
    {
        public string MapTo(dynamic value)
        {
            var result = value == null ? "" : value.ToString();
            return result;
        }
    }
}