using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BTS.API.ENTITY;

namespace BTS.API.SERVICE.MD
{
 
    public interface IMdWareHouseService : IDataInfoService<MdWareHouse>
    {
        MdWareHouse UpdateKhoHang(MdWareHouseVm.Dto instance);
        MdWareHouseVm.Dto GetNewInstance();
    }
    public class MdWareHouseService : DataInfoServiceBase<MdWareHouse>, IMdWareHouseService
    {
        public MdWareHouseService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdWareHouse, bool>> GetKeyFilter(MdWareHouse instance)
        {
            var maDonViCha = GetParentUnitCode();
            var unitCode = GetCurrentUnitCode();
            return x => x.MaKho == instance.MaKho && x.UnitCode.StartsWith(maDonViCha);
        }

        public MdWareHouse UpdateKhoHang(MdWareHouseVm.Dto instance)
        {
            var exsitItem = FindById(instance.Id);
            if (exsitItem != null)
            {
                var data = Mapper.Map<MdWareHouseVm.Dto, MdWareHouse>(instance);
                var result = Update(data);
                return result;
            }
            else
            {
                return null;
            }
          
        }

        public MdWareHouseVm.Dto GetNewInstance()
        {
            var _unitCode = GetCurrentUnitCode();
            var khoBanLe = _unitCode + "-K2";
            var khoKM = _unitCode + "-K3";
            bool isHaveKhoBanLe = UnitOfWork.Repository<MdWareHouse>().DbSet.Any(x => x.UnitCode == _unitCode && x.MaKho == khoBanLe);
            bool isHaveKhoKM = UnitOfWork.Repository<MdWareHouse>().DbSet.Any(x => x.UnitCode == _unitCode && x.MaKho == khoKM);
            string MA_DM = _unitCode + "-K";
            return new MdWareHouseVm.Dto()
            {
                MaKho = BuildCode_DM(MA_DM, _unitCode, false),
                IsHaveKhoBanLe  = isHaveKhoBanLe,
                IsHaveKhoKM = isHaveKhoKM,
                UnitCode = _unitCode,
            };
        }
        //public string BuildCode()
        //{
        //    var type = TypeMasterData.KHO.ToString();
        //    var result = "";
        //    var unitCode = GetCurrentUnitCode();
        //    var maDonViCha = GetParentUnitCode();
        //    var idRepo = UnitOfWork.Repository<MdIdBuilder>();
        //    var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
        //    if (config == null)
        //    {
        //        config = new MdIdBuilder
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            Type = type,
        //            Code = unitCode + "-K",
        //            Current = "0",
        //            UnitCode = maDonViCha
        //        };
        //    }
        //    var soMa = config.GenerateNumber();
        //    config.Current = soMa;
        //    result = string.Format("{0}{1}", config.Code, soMa);
        //    return result;
        //}
        //public string SaveCode()
        //{
        //    var type = TypeMasterData.KHO.ToString();
        //    var result = "";
        //    var maDonViCha = GetParentUnitCode();
        //    var unitCode = GetCurrentUnitCode();
        //    var idRepo = UnitOfWork.Repository<MdIdBuilder>();
        //    var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
        //    if (config == null)
        //    {
        //        config = new MdIdBuilder
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            Type = type,
        //            Code = unitCode + "-K",
        //            Current = "000000",
        //            UnitCode = maDonViCha
        //        };
        //        result = config.GenerateNumber();
        //        config.Current = result;
        //        idRepo.Insert(config);
        //    }
        //    else
        //    {
        //        result = config.GenerateNumber();
        //        config.Current = result;
        //        config.ObjectState = ObjectState.Modified;
        //    }
        //    UnitOfWork.Save();
        //    result = string.Format("{0}{1}", config.Code, config.Current);
        //    return result;
        //}
    }
}
