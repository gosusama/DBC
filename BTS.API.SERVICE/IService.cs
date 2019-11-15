using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE
{
    public interface IService
    {
        IUnitOfWork UnitOfWork { get; }
    }
}