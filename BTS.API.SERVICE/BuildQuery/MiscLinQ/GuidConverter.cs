using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Query.MiscLinQ
{
    public class GuidConverter : IConverter
    {
        public string MapTo(dynamic value)
        {
            var data = (Guid)value();
            var result = data.ToString();
            return result;
        }
    }
}