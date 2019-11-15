using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Web;

namespace BTS.API.SERVICE.NV
{
    public interface INvNhapHangBanTraLaiService : IDataInfoService<NvVatTuChungTu>
    {
        NvVatTuChungTu InsertPhieu(NvNhapHangBanTraLaiVm.Dto instance);
        NvVatTuChungTu UpdatePhieu(NvNhapHangBanTraLaiVm.Dto instance);
        StateProcessApproval Approval(NvVatTuChungTu chungTu);
        NvNhapHangBanTraLaiVm.ReportModel CreateReport(string id);
        NvNhapHangBanTraLaiVm.Dto CreateNewInstance();
        bool DeletePhieu(string id);
    }
    public class NvNhapHangBanTraLaiService : DataInfoServiceBase<NvVatTuChungTu>, INvNhapHangBanTraLaiService
    {
        public NvNhapHangBanTraLaiService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public bool DeletePhieu(string id)
        {
            var insatance = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => x.Id == id).FirstOrDefault();
            if (insatance == null)
            {
                return false;
            }
            var detailData = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(o => o.MaChungTuPk == insatance.MaChungTuPk).ToList();
            foreach (NvVatTuChungTuChiTiet dt in detailData)
            {
                dt.ObjectState = ObjectState.Deleted;
            }
            return true;

        }
        public NvNhapHangBanTraLaiVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            var code = BuildCode_PTNX(TypeVoucher.NHBANTL.ToString(), unitCode, false);
            return new NvNhapHangBanTraLaiVm.Dto()
            {
                LoaiPhieu = TypeVoucher.NHBANTL.ToString(),
                MaChungTu = code, 
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode)
            };
        }

      
        public StateProcessApproval Approval(NvVatTuChungTu chungTu)
        {
            StateProcessApproval result;
            var unitCode = GetCurrentUnitCode();

            var periods = CurrentSetting.GetKhoaSo(unitCode);
            if (periods != null)
            {
                var tableName = ProcedureCollection.GetTableName(periods.Year, periods.Period);
                if (ProcedureCollection.IncreaseVoucher(tableName, periods.Year, periods.Period, chungTu.Id))
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
        public NvVatTuChungTu InsertPhieu(NvNhapHangBanTraLaiVm.Dto instance)
        {
            var _parentUnitCode = GetParentUnitCode();
            instance.Calc();
            var item = Mapper.Map<NvNhapHangBanTraLaiVm.Dto, NvVatTuChungTu>(instance);
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            string _unitCode = GetCurrentUnitCode();
            result.MaChungTu = BuildCode_PTNX(TypeVoucher.NHBANTL.ToString(), _unitCode, true);
            result.LoaiPhieu = TypeVoucher.NHBANTL.ToString();
            result.NgayCT = DateTime.Now;
            result.TrangThai = 0;
            item.GenerateMaChungTuPk();
            result = Insert(result);
            var detailData = Mapper.Map<List<NvNhapHangBanTraLaiVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            var wareHouse = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            detailData.ForEach(x =>
            {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_parentUnitCode));
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTu = result.MaChungTu;
                x.MaChungTuPk = result.MaChungTuPk;
                x.Index = x.Index;
                x.VAT = hang.MaVatVao;
            });
            InsertGeneralLedger(instance.DataClauseDetails, result);
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            return result;

        }
        public NvVatTuChungTu UpdatePhieu(NvNhapHangBanTraLaiVm.Dto instance)
        {
            var _parentUnitCode = GetParentUnitCode();
            instance.Calc();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvNhapHangBanTraLaiVm.Dto, NvVatTuChungTu>(instance);
            var detailData = Mapper.Map<List<NvNhapHangBanTraLaiVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
          
            {
                var detailCollection = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsitItem.MaChungTuPk);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            detailData.ForEach(x =>
            {
                var merchandise = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_parentUnitCode));
                x.TenHang = merchandise != null ? merchandise.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = exsitItem.MaChungTuPk;
                x.MaChungTu = exsitItem.MaChungTu;
                x.SoLuongCT = x.SoLuong;
                x.Index = x.Index;
            });
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            UpdateGeneralLedger(instance.DataClauseDetails, exsitItem);
            var result = Update(masterData);
            return result;
        }
        public void InsertGeneralLedger(List<NvNhapHangBanTraLaiVm.DtoClauseDetail> data, NvVatTuChungTu chungTu)
        {
            var generalLedgers = Mapper.Map<List<NvNhapHangBanTraLaiVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
            generalLedgers.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = chungTu.MaChungTuPk;
                x.MaChungTu = chungTu.MaChungTu;
                x.LoaiPhieu = chungTu.LoaiPhieu;
                x.TrangThai = chungTu.TrangThai;
                x.NgayCT = chungTu.NgayCT;
                x.NoiDung = chungTu.NoiDung;
                x.UnitCode = x.UnitCode;
            });
            UnitOfWork.Repository<DclGeneralLedger>().InsertRange(generalLedgers);
        }
        public void UpdateGeneralLedger(List<NvNhapHangBanTraLaiVm.DtoClauseDetail> data, NvVatTuChungTu exsitItem)
        {
            var generalLedgers = Mapper.Map<List<NvNhapHangBanTraLaiVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
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

        public NvNhapHangBanTraLaiVm.ReportModel CreateReport(string id)
        {
            var result = new NvNhapHangBanTraLaiVm.ReportModel();
            var exsit = FindById(id);
            if (exsit != null)
            {
                result = Mapper.Map<NvVatTuChungTu, NvNhapHangBanTraLaiVm.ReportModel>(exsit);
                var detailData = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsit.MaChungTuPk).ToList();
                result.DataReportDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapHangBanTraLaiVm.ReportDetailModel>>(detailData);
                var customer = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == result.MaKhachHang);
                if (customer != null)
                {
                    result.TenKhachHang = customer.TenKH;
                    result.DienThoai = customer.DienThoai;
                    result.DiaChiKhachHang = customer.DiaChi;
                }
                var kho = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);
                if (kho != null)
                {
                    result.TenKho = kho.TenKho;
                }
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
                var userName = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                if (userName != null)
                {
                        result.Username = userName.TenNhanVien;
                }
                else
                {
                    result.Username = "Administrator";
                }
            }
            return result;
        }
        protected override Expression<Func<NvVatTuChungTu, bool>> GetKeyFilter(NvVatTuChungTu instance)
        {
            string _unitCode = GetCurrentUnitCode();
            return x => x.MaChungTuPk == instance.MaChungTuPk && x.UnitCode == _unitCode;
        }


    }
}
