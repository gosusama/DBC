using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{

    public interface IMdCurrencyService : IDataInfoService<MdCurrency>
    {
        //Add function here
    }
    public class MdCurrencyService : DataInfoServiceBase<MdCurrency>, IMdCurrencyService
    {
        public MdCurrencyService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdCurrency, bool>> GetKeyFilter(MdCurrency instance)
        {
            var unitCode = GetCurrentUnitCode();
            return x => x.MaNgoaiTe == instance.MaNgoaiTe && x.UnitCode == unitCode;
        }
    }
}
