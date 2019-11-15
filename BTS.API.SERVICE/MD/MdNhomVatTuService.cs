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
    public interface IMdNhomVatTuService : IDataInfoService<MdNhomVatTu>
    {
        string BuildCode();
        string SaveCode();

        //Add function here
    }
    public class MdNhomVatTuService : DataInfoServiceBase<MdNhomVatTu>, IMdNhomVatTuService
    {
        public MdNhomVatTuService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdNhomVatTu, bool>> GetKeyFilter(MdNhomVatTu instance)
        {
            var unitCode = GetCurrentUnitCode();
            var maDonViCha = GetParentUnitCode();
            return x => x.MaNhom == instance.MaNhom && x.UnitCode.StartsWith(maDonViCha);
        }
        public string BuildCode()
        {
            var type = TypeMasterData.MN.ToString();
            var result = "";
            var maDonViCha = GetParentUnitCode();
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var strType = type.ToString();
            var config = idRepo.DbSet.Where(x => x.Type == strType && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = strType,
                    Code = strType,
                    Current = "000000",
                    UnitCode = maDonViCha
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
            var type = TypeMasterData.MN.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var strType = type.ToString();
            var config = idRepo.DbSet.Where(x => x.Type == strType && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = strType,
                    Code = strType,
                    Current = "000000",
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
            UnitOfWork.Save();
            result = string.Format("{0}{1}", config.Code, config.Current);
            return result;
        }
    }
}
