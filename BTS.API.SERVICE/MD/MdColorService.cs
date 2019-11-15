using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BTS.API.SERVICE.MD
{
    public interface IMdColorService : IDataInfoService<MdColor>
    {
        //Add function here
        string BuildCode();
        string SaveCode();
        MdColor CreateNewInstance();
    }
    public class MdColorService : DataInfoServiceBase<MdColor>, IMdColorService
    {
        public MdColorService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdColor, bool>> GetKeyFilter(MdColor instance)
        {
            var unitCode = GetCurrentUnitCode();
            var maDonViCha = GetParentUnitCode();
            return x => x.MaColor == instance.MaColor && x.UnitCode.StartsWith(maDonViCha);
        }
        public MdColor CreateNewInstance()
        {
            return new MdColor()
            {
                MaColor = BuildCode(),
                TrangThai = 10
            };
        }
        public string BuildCode()
        {
            var type = TypeMasterData.COLOR.ToString();
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
                    Code = "C",
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
            var type = TypeMasterData.COLOR.ToString();
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
                    Code = "C",
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
