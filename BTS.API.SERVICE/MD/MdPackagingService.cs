using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.MD
{
    public interface IMdPackagingService : IDataInfoService<MdPackaging>
    {
        MdPackaging CreateNewInstance();

    }
    public class MdPackagingService : DataInfoServiceBase<MdPackaging>, IMdPackagingService
    {
        public MdPackagingService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdPackaging, bool>> GetKeyFilter(MdPackaging instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            return x => x.MaBaoBi == instance.MaBaoBi && x.UnitCode.StartsWith(_ParentUnitCode);//&& x.UnitCode == unitCode;
        }
        public override MdPackaging Insert(MdPackaging instance, bool withUnitCode = true)
        {
            var _ParentUnitCode = GetParentUnitCode();
            instance.MaBaoBi = BuildCode_DM(TypeMasterData.BAOBI.ToString(), _ParentUnitCode, true);
            return base.Insert(instance, withUnitCode);
        }
      
        public MdPackaging CreateNewInstance()
        {
            var _ParentUnitCode = GetParentUnitCode();
            return new MdPackaging
            {
                MaBaoBi = BuildCode_DM(TypeMasterData.BAOBI.ToString(), _ParentUnitCode, false),
                TrangThai = 10
            };
        }
    }
}
