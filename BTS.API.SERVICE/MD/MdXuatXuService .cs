using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{
    public interface IMdXuatXuService:IDataInfoService<MdXuatXu>
    {
        string BuildCode();
        string SaveCode();
    }
    public class MdXuatXuService:DataInfoServiceBase<MdXuatXu>,IMdXuatXuService
    {
        public MdXuatXuService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdXuatXu, bool>> GetKeyFilter(MdXuatXu instance)
        {
            var _parent = GetParentUnitCode();
            return x => x.MaXuatXu == instance.MaXuatXu && x.UnitCode.StartsWith(_parent);
        }
        public string BuildCode()
        {
            var maDonViCha = GetParentUnitCode();
            var type = TypeMasterData.XUATXU.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = "XUATXU",
                    Current = "00",
                    UnitCode = maDonViCha,
                };
            }
            var soMa = config.GenerateNumber();
            config.Current = soMa;
            result = string.Format("{0}{1}", config.Code, soMa);
            return result;
        }
        public string SaveCode()
        {
            var maDonViCha = GetParentUnitCode();
            var type = TypeMasterData.XUATXU.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = "XUATXU",
                    Current = "00",
                    UnitCode = maDonViCha,
                };
                result = config.GenerateNumber();
                config.Current = result;
                idRepo.Insert(config);
            }
            else
            {
                result = config.GenerateNumber();
                config.Current = result;
                config.ObjectState = ObjectState.Modified;
            }
            result = string.Format("{0}{1}", config.Code, config.Current);
            return result;
        }
    }
}
