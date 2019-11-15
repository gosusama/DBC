using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.NV
{
    public interface INvKhuyenMaiTichDiemService : IDataInfoService<NvChuongTrinhKhuyenMai>
    {
        NvKhuyenMaiTichDiemVm.Dto CreateNewInstance();
        NvChuongTrinhKhuyenMai InsertPhieu(NvKhuyenMaiTichDiemVm.Dto instance);
        NvChuongTrinhKhuyenMai UpdatePhieu(NvKhuyenMaiTichDiemVm.Dto instance);

    }
    public class NvKhuyenMaiTichDiemService : DataInfoServiceBase<NvChuongTrinhKhuyenMai>, INvKhuyenMaiTichDiemService
    {
        public NvKhuyenMaiTichDiemService(IUnitOfWork unitOfWork) : base(unitOfWork)
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
        public NvChuongTrinhKhuyenMai InsertPhieu(NvKhuyenMaiTichDiemVm.Dto instance)
        {
            var item = AutoMapper.Mapper.Map<NvKhuyenMaiTichDiemVm.Dto, NvChuongTrinhKhuyenMai>(instance);
            item.Id = Guid.NewGuid().ToString();
            item.MaChuongTrinh = SaveCode();
            var result = Insert(item);
            return result;
        }
        public NvChuongTrinhKhuyenMai UpdatePhieu(NvKhuyenMaiTichDiemVm.Dto instance)
        {
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvKhuyenMaiTichDiemVm.Dto, NvChuongTrinhKhuyenMai>(instance);

            var result = Update(masterData);
            return result;
        }
        protected override Expression<Func<NvChuongTrinhKhuyenMai, bool>> GetKeyFilter(NvChuongTrinhKhuyenMai instance)
        {
            return x => x.MaChuongTrinh == instance.MaChuongTrinh;
        }
        public NvKhuyenMaiTichDiemVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            return new NvKhuyenMaiTichDiemVm.Dto()
            {
                LoaiKhuyenMai = (int)LoaiKhuyenMai.TichDiem,
                MaChuongTrinh = BuildCode(),
                TrangThai = 10,
                //NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
               // NgayDieuDong = DateTime.Now
            };
        }

    }
}
