using BTS.API.ENTITY;
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
    public interface IMdSizeService : IDataInfoService<MdSize>
    {
        //Add function here
        string BuildCode();
        string SaveCode();
        MdSize CreateNewInstance();
    }
    public class MdSizeService : DataInfoServiceBase<MdSize>, IMdSizeService
    {
        public MdSizeService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdSize, bool>> GetKeyFilter(MdSize instance)
        {
            var unitCode = GetCurrentUnitCode();
            var maDonViCha = GetParentUnitCode();
            return x => x.MaSize == instance.MaSize && x.UnitCode.StartsWith(maDonViCha);
        }
        public MdSize CreateNewInstance()
        {
            return new MdSize()
            {
                MaSize = BuildCode(),
                TrangThai = 10
            };
        }
        public string BuildCode()
        {
            var type = TypeMasterData.SIZE.ToString();
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
                    Code = "S",
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
            var type = TypeMasterData.SIZE.ToString();
            var result = "";
            var maDonViCha = GetParentUnitCode();

            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var unit = GetCurrentUnitCode();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = "S",
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
