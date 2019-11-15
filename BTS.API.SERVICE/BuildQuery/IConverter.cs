using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery
{
    public interface IConverter
    {
        string MapTo(dynamic value);
    }
}
