using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Query.Misc
{
    public class BooleanConverter : IConverter
    {
        public string MapTo(dynamic value)
        {
            var data = (bool) value();
            var result = data ? "1" : "0";
            return result;
        }
    }
}