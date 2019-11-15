using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BTS.API.SERVICE.NV
{
    public interface INvXuatBanLeService : IDataInfoService<NvVatTuChungTu>
    {
        NvVatTuChungTu InsertPhieu(NvXuatBanLeVm.Dto instance);
        NvVatTuChungTu UpdatePhieu(NvXuatBanLeVm.Dto instance);
        StateProcessApproval Approval(string id);
        NvXuatBanLeVm.ReportModel CreateReport(string id);
        NvXuatBanLeVm.Dto CreateNewInstance();
    }
    public class NvXuatBanLeService : DataInfoServiceBase<NvVatTuChungTu>, INvXuatBanLeService
    {
        public NvXuatBanLeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public NvXuatBanLeVm.Dto CreateNewInstance()
        {
            var code = BuildCode();
            var unitCode = GetCurrentUnitCode();
            return new NvXuatBanLeVm.Dto()
            {
                LoaiPhieu = TypeVoucher.XBANLE.ToString(),
                MaChungTu = code,
                MaHoaDon = code,
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),

            };
        }

        public NvVatTuChungTu InsertPhieu(NvXuatBanLeVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            instance.Calc(); //Tinh lại cac thuộc tính thứ sinh
            var item = Mapper.Map<NvXuatBanLeVm.Dto, NvVatTuChungTu>(instance);
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            result.MaChungTu = SaveCode();
            item.GenerateMaChungTuPk();
            result = Insert(result);
            var detailData = AutoMapper.Mapper.Map<List<NvXuatBanLeVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            var importWareHouse = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            detailData.ForEach(x => {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTu = result.MaChungTu;
                x.MaChungTuPk = result.MaChungTuPk;
                x.SoLuongCT = x.SoLuong;
                x.SoLuongBaoCT = x.SoLuongBao;
            });
            InsertGeneralLedger(instance.DataClauseDetails, result);
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            return result;
        }
        public NvVatTuChungTu UpdatePhieu(NvXuatBanLeVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            instance.Calc();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
           
            var masterData = Mapper.Map<NvXuatBanLeVm.Dto, NvVatTuChungTu>(instance);
            var detailData = Mapper.Map<List<NvXuatBanLeVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
          
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            {
                var detailCollection = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsitItem.MaChungTuPk);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            detailData.ForEach(x =>
            {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = exsitItem.MaChungTuPk;
                x.MaChungTu = exsitItem.MaChungTu;
                x.SoLuongCT = x.SoLuong;
                x.SoLuongBaoCT = x.SoLuongBao;
            });
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            UpdateGeneralLedger(instance.DataClauseDetails, exsitItem);
             var result = Update(masterData);
            return result;
        }
        public void InsertGeneralLedger(List<NvXuatBanLeVm.DtoClauseDetail> data, NvVatTuChungTu chungTu)
        {
            var generalLedgers = Mapper.Map<List<NvXuatBanLeVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
            generalLedgers.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = chungTu.MaChungTuPk;
                x.MaChungTu = chungTu.MaChungTu;
                x.LoaiPhieu = chungTu.LoaiPhieu;
                x.TrangThai = chungTu.TrangThai; // Chưa duyệt
                x.NgayCT = chungTu.NgayCT;
                x.NoiDung = chungTu.NoiDung;
                x.UnitCode = chungTu.UnitCode;
            });
            UnitOfWork.Repository<DclGeneralLedger>().InsertRange(generalLedgers);
        }
        public void UpdateGeneralLedger(List<NvXuatBanLeVm.DtoClauseDetail> data, NvVatTuChungTu exsitItem)
        {
           
            var generalLedgers = Mapper.Map<List<NvXuatBanLeVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
            {
                var detailCollection = UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(x => x.MaChungTuPk == exsitItem.MaChungTuPk);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            generalLedgers.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = exsitItem.MaChungTuPk;
                x.MaChungTu = exsitItem.MaChungTu;
                x.LoaiPhieu = exsitItem.LoaiPhieu;
                x.TrangThai = exsitItem.TrangThai;
                x.NgayCT = exsitItem.NgayCT;
                x.NoiDung = exsitItem.NoiDung;
                x.UnitCode = exsitItem.UnitCode;
            });
            UnitOfWork.Repository<DclGeneralLedger>().InsertRange(generalLedgers);
        }
        public NvXuatBanLeVm.ReportModel CreateReport(string id)
        {
            var result = new NvXuatBanLeVm.ReportModel();
           
            var exsit = FindById(id);
            if (exsit != null)
            {
                result = Mapper.Map<NvVatTuChungTu, NvXuatBanLeVm.ReportModel>(exsit);
                var nhanVien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == exsit.ICreateBy).FirstOrDefault();
                if (nhanVien != null)
                {
                    
                    result.NameNhanVienCreate = nhanVien.TenNhanVien != null ? nhanVien.TenNhanVien : "";
                }

                var detailData = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsit.MaChungTuPk).ToList();
                result.DataReportDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvXuatBanLeVm.ReportDetailModel>>(detailData);
                var customer = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == result.MaKhachHang);
                if (customer != null)
                {
                    result.TenKhachHang =  customer.TenKH;
                    result.DiaChiKhachHang = customer.DiaChi;
                }
                return result;
            }
            var unitCode = GetCurrentUnitCode();
            var createDate = DateTime.Now;
            result.CreateDay = createDate.Day;
            result.CreateMonth = createDate.Month;
            result.CreateYear = createDate.Year;
            result.TenDonVi = CurrentSetting.GetUnitName(unitCode);
            result.DiaChiDonVi = CurrentSetting.GetUnitAddress(unitCode);
            //GetNhanVien
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                var name = currentUser.Identity.Name;
                var nhanVien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                if (nhanVien != null)
                {
                        result.Username = nhanVien.TenNhanVien;
                }
                else
                {
                    result.Username = "Administrator";
                }
            }
            return null;
        }

        public string BuildCode(TypeVoucher type = TypeVoucher.XBANLE)
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
            result = string.Format("{0}{1}", config.Code, maChungTuGenerate);
            return result;
        }

        public string SaveCode(TypeVoucher type = TypeVoucher.XBANLE)
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
            result = string.Format("{0}{1}", config.Code, config.Current);
            return result;
        }
        public StateProcessApproval Approval(string id)
        {
            StateProcessApproval result;
            var unitCode = GetCurrentUnitCode();
            var periods = CurrentSetting.GetKhoaSo(unitCode);
            if (periods != null)
            {
                var tableName = ProcedureCollection.GetTableName(periods.Year, periods.Period);
                if (ProcedureCollection.DecreaseVoucher(tableName, periods.Year, periods.Period, id))
                {
                    result = StateProcessApproval.Success;
                }
                else
                {
                    result = StateProcessApproval.Failed;
                }
            }
            else
            {
                result = StateProcessApproval.NoPeriod;
            }

            return result;
        }
        protected override Expression<Func<NvVatTuChungTu, bool>> GetKeyFilter(NvVatTuChungTu instance)
        {
            return x => x.MaChungTuPk == instance.MaChungTuPk;
        }
    }
}
