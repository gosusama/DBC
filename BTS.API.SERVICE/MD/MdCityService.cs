using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{
    public interface IMdCityService : IDataInfoService<MdCity>
    {
    }
    public class MdCityService : DataInfoServiceBase<MdCity>,IMdCityService
    {
        public MdCityService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdCity, bool>> GetKeyFilter(MdCity instance)
        {
            var _parent = GetParentUnitCode();
            return x => x.CityId == instance.CityId && x.UnitCode.StartsWith(_parent);
        }
    }
}
