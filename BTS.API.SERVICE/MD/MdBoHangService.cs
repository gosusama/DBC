using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{
    public interface IMdBoHangService:IDataInfoService<MdBoHang>
    {
        MdBoHang InsertDto(MdBoHangVm.Dto instance);
        MdBoHang UpdateDto(MdBoHangVm.Dto instance);
        string BuildCode();
        string SaveCode();
    }
    public class MdBoHangService:DataInfoServiceBase<MdBoHang>,IMdBoHangService
    {
        public MdBoHangService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdBoHang, bool>> GetKeyFilter(MdBoHang instance)
        {
            var _parent = GetParentUnitCode();
            return x => x.MaBoHang == instance.MaBoHang && x.UnitCode.StartsWith(_parent);
        }

        public MdBoHang InsertDto(MdBoHangVm.Dto instance)
        {
            var item = AutoMapper.Mapper.Map<MdBoHangVm.Dto, MdBoHang>(instance);
            item.GhiChu = instance.GhiChu;
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            result = Insert(result);
            var detail = Mapper.Map<List<MdBoHangVm.DtoDetail>, List<MdBoHangChiTiet>>(instance.DataDetails);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            detail.ForEach(x => { 
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang);
                x.Id = Guid.NewGuid().ToString();
                x.MaBoHang = result.MaBoHang;
            });
            UnitOfWork.Repository<MdBoHangChiTiet>().InsertRange(detail);

            return result;
        }
        public MdBoHang UpdateDto(MdBoHangVm.Dto instance)
        {
            MdBoHang result = null;
            var item = FindById(instance.Id);
            if (item != null)
            {
                result = Update(Mapper.Map<MdBoHangVm.Dto, MdBoHang>(instance));
                {//delete
                    var deleteList = UnitOfWork.Repository<MdBoHangChiTiet>().DbSet.Where(x => x.MaBoHang == result.MaBoHang).ToList();
                    deleteList.ForEach(x => x.ObjectState = ObjectState.Deleted);
                }
                {//insert data details
                    var merchandiseCollection = UnitOfWork.Repository<MdBoHangChiTiet>().DbSet;
                    var detail = Mapper.Map<List<MdBoHangVm.DtoDetail>, List<MdBoHangChiTiet>>(instance.DataDetails);
                    detail.ForEach(x =>
                    {
                        var hang = merchandiseCollection.FirstOrDefault(u => u.MaHang == x.MaHang);
                        x.Id = Guid.NewGuid().ToString();
                        x.MaBoHang = result.MaBoHang;
                    });
                    UnitOfWork.Repository<MdBoHangChiTiet>().InsertRange(detail);
                }
            }
            return result;
        }
        public string BuildCode()
        {
            var maDonViCha = GetParentUnitCode();
            var type = TypeMasterData.BOHANG.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = "BH",
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
            var maDonViCha = GetParentUnitCode();
            var type = TypeMasterData.BOHANG.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = "BH",
                    Current = "0000",
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
