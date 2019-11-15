using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.Authorize.AuThamSoHeThong
{
    public interface IAuThamSoHeThongService : IDataInfoService<AU_THAMSOHETHONG>
    {
    }
    public class AuThamSoHeThongService : DataInfoServiceBase<AU_THAMSOHETHONG>, IAuThamSoHeThongService
    {
        public AuThamSoHeThongService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
