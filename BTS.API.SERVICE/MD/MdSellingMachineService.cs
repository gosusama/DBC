using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.MD
{
   
    public interface IMdSellingMachineService : IDataInfoService<MdSellingMachine>
    {
    }
    public class MdSellingMachineService : DataInfoServiceBase<MdSellingMachine>, IMdSellingMachineService
    {
        public MdSellingMachineService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdSellingMachine, bool>> GetKeyFilter(MdSellingMachine instance)
        {
            var unitCode = GetCurrentUnitCode();
            return x => x.Code == instance.Code && x.UnitCode == unitCode;
        }

    }
}
