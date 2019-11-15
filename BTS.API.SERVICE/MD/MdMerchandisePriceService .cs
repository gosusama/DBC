using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{

    public interface IMdMerchandisePriceService : IDataInfoService<MdMerchandisePrice>
    {

        //Add function here
    }
    public class MdMerchandisePriceService : DataInfoServiceBase<MdMerchandisePrice>, IMdMerchandisePriceService
    {

        public MdMerchandisePriceService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdMerchandisePrice, bool>> GetKeyFilter(MdMerchandisePrice instance)
        {
            var unitCode = GetCurrentUnitCode();
            return x => x.MaVatTu == instance.MaVatTu &&  x.UnitCode == unitCode;// && x.UnitCode == unitCode;
        }

    }
}
