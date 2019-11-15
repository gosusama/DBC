using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Query.Misc
{
    public class DateTimeConverter : IConverter
    {
        public string MapTo(dynamic value)
        {
            var data = (DateTime) value();
            var result = string.Format(@"convert(datetime, '{0}', {1})",
                data.ToString("dd/MM/yyyy"),
                103);
            return result;
        }
    }
}