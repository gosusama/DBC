using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Query.MiscLinQ
{
    public class BooleanConverter : IConverter
    {
        public string MapTo(dynamic value)
        {
            var data = (bool) value();
            var result = data.ToString();
            return result;
        }
    }
}