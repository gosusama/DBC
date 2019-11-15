using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using BTS.API.ENTITY;

namespace BTS.API.SERVICE.MD
{
    public interface IMdShelvesService : IDataInfoService<MdShelves>
    {
        MdShelves CreateNewInstance();
    }
    public class MdShelvesService : DataInfoServiceBase<MdShelves>, IMdShelvesService
    {
        public MdShelvesService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdShelves, bool>> GetKeyFilter(MdShelves instance)
        {
            var _unitCode = GetCurrentUnitCode();
            return x => x.MaKeHang == instance.MaKeHang && x.UnitCode == _unitCode;
        }
        
        public MdShelves CreateNewInstance()
        {
            var _unitCode = GetCurrentUnitCode();
            return new MdShelves()
            {
                MaKeHang = BuildCode_DM(TypeMasterData.KEHANG.ToString(), _unitCode, false)
            };
        }
    }
}
