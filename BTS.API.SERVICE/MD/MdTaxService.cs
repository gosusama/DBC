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
    public interface IMdTaxService : IDataInfoService<MdTax>
    {
        //Add function here
    }
    public class MdTaxService : DataInfoServiceBase<MdTax>, IMdTaxService
    {
        public MdTaxService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdTax, bool>> GetKeyFilter(MdTax instance)
        {
            var unitCode = GetCurrentUnitCode();
            var maDonViCha = GetParentUnitCode();
            return x => x.MaLoaiThue == instance.MaLoaiThue && x.UnitCode.StartsWith(maDonViCha);
        }
    }
}
