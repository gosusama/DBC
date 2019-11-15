using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BTS.API.SERVICE.NV
{
    public interface INvKhuyenMaiVoucherService : IDataInfoService<NvChuongTrinhKhuyenMai>
    {
        NvKhuyenMaiVoucherVm.Dto CreateNewInstance();
        NvChuongTrinhKhuyenMai InsertPhieu(NvKhuyenMaiVoucherVm.Dto instance);
        NvChuongTrinhKhuyenMai UpdatePhieu(NvKhuyenMaiVoucherVm.Dto instance);
        bool DeletePhieu(string id);
    }
    public class NvKhuyenMaiVoucherService : DataInfoServiceBase<NvChuongTrinhKhuyenMai>, INvKhuyenMaiVoucherService
    {
        public NvKhuyenMaiVoucherService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public string BuildCode(TypeVoucher type = TypeVoucher.CTKM)
        {
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var strType = type.ToString();
            var config = idRepo.DbSet.Where(x => x.Type == strType).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type.ToString(),
                    Code = type.ToString(),
                    Current = "0",
                };
            }
            var maChungTuGenerate = config.GenerateNumber();
            config.Current = maChungTuGenerate;
            result = string.Format("{0}", maChungTuGenerate);
            return result;
        }
        public bool DeletePhieu(string id)
        {
            var insatance = UnitOfWork.Repository<NvChuongTrinhKhuyenMai>().DbSet.Where(x => x.Id == id).FirstOrDefault();
            if (insatance == null)
            {
                return false;
            }
            var detailData = UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().DbSet.Where(o => o.MaChuongTrinh == insatance.MaChuongTrinh).ToList();
            foreach (NvChuongTrinhKhuyenMaiChiTiet dt in detailData)
            {
                dt.ObjectState = ObjectState.Deleted;
            }
            return true;

        }
        public string SaveCode(TypeVoucher type = TypeVoucher.CTKM)
        {
            var result = "";
            var strType = type.ToString();
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == strType).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = strType,
                    Code = strType,
                    Current = "0",
                };
                config.Current = config.GenerateNumber();
                idRepo.Insert(config);
            }
            else
            {
                config.Current = config.GenerateNumber();
                config.ObjectState = ObjectState.Modified;
            }
            result = string.Format("{0}", config.Current);
            return result;
        }
        public NvChuongTrinhKhuyenMai InsertPhieu(NvKhuyenMaiVoucherVm.Dto instance)
        {
            var item = AutoMapper.Mapper.Map<NvKhuyenMaiVoucherVm.Dto, NvChuongTrinhKhuyenMai>(instance);
            item.Id = Guid.NewGuid().ToString();
            item.MaChuongTrinh = SaveCode();
            item.TrangThaiSuDung = (int)ApprovalState.IsComplete;
            var result = Insert(item);
            return result;
        }
        public NvChuongTrinhKhuyenMai UpdatePhieu(NvKhuyenMaiVoucherVm.Dto instance)
        {
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvKhuyenMaiVoucherVm.Dto, NvChuongTrinhKhuyenMai>(instance);

            var result = Update(masterData);
            return result;
        }
        protected override Expression<Func<NvChuongTrinhKhuyenMai, bool>> GetKeyFilter(NvChuongTrinhKhuyenMai instance)
        {
            var _parentUnitCode = GetParentUnitCode();
            return x => x.MaChuongTrinh == instance.MaChuongTrinh && x.MaGiamGia == instance.MaGiamGia && x.UnitCode.StartsWith(_parentUnitCode);
        }
        public NvKhuyenMaiVoucherVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            return new NvKhuyenMaiVoucherVm.Dto()
            {
                LoaiKhuyenMai = (int)LoaiKhuyenMai.Voucher,
                MaChuongTrinh = BuildCode(),
                TrangThai = 10,
                //NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
               // NgayDieuDong = DateTime.Now
            };
        }

    }
}
