using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{
    public interface IMdDonViTinhService : IDataInfoService<MdDonViTinh>
    {
        //Add function here
        string BuildCode();
        string SaveCode();
        MdDonViTinh CreateNewInstance();
    }
    public class MdDonViTinhService : DataInfoServiceBase<MdDonViTinh>, IMdDonViTinhService
    {
        public MdDonViTinhService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdDonViTinh, bool>> GetKeyFilter(MdDonViTinh instance)
        {
            var unitCode = GetCurrentUnitCode();
            var maDonViCha = GetParentUnitCode();
            return x => x.MaDVT == instance.MaDVT && x.UnitCode.StartsWith(maDonViCha);
        }
        public MdDonViTinh CreateNewInstance()
        {
            return new MdDonViTinh()
            {
                MaDVT = BuildCode(),
                TrangThai = 10
            };
        }
        public string BuildCode()
        {
            var type = TypeMasterData.DVT.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var maDonViCha = GetParentUnitCode();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    Current = "0000",
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
            var type = TypeMasterData.DVT.ToString();
            var result = "";


            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var maDonViCha = GetParentUnitCode();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    Current = "0000",
                    UnitCode = maDonViCha
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
