using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Web;

namespace BTS.API.SERVICE.NV
{
    public interface INvXuatKhacService : IDataInfoService<NvVatTuChungTu>
    {
        NvVatTuChungTu InsertPhieu(NvXuatKhacVm.Dto instance);
        NvVatTuChungTu UpdatePhieu(NvXuatKhacVm.Dto instance);
        NvXuatKhacVm.ReportModel CreateReport(string id);
        StateProcessApproval Approval(string id);
        NvXuatKhacVm.Dto CreateNewInstance();
        bool DeletePhieu(string id);
    }
    public class NvXuatKhacService : DataInfoServiceBase<NvVatTuChungTu>, INvXuatKhacService
    {
        public NvXuatKhacService(IUnitOfWork unitOfWork) : base(unitOfWork)
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
        public NvXuatKhacVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            var code = BuildCode_PTNX(TypeVoucher.XKHAC.ToString(), unitCode, false);
            return new NvXuatKhacVm.Dto()
            {
                LoaiPhieu = TypeVoucher.XKHAC.ToString(),
                MaChungTu = code,
                MaHoaDon = code,
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
            };
        }

        public NvVatTuChungTu InsertPhieu(NvXuatKhacVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var item = Mapper.Map<NvXuatKhacVm.Dto, NvVatTuChungTu>(instance);
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            string _unitCode = GetCurrentUnitCode();
            result.MaChungTu = BuildCode_PTNX(TypeVoucher.XKHAC.ToString(), _unitCode, true);
            item.GenerateMaChungTuPk();
            result = Insert(result);
            var detailData = AutoMapper.Mapper.Map<List<NvXuatKhacVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            var importWareHouse = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            detailData.ForEach(x =>
            {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTu = result.MaChungTu;
                x.MaChungTuPk = result.MaChungTuPk;
                x.SoLuongCT = x.SoLuong;
                x.SoLuongBaoCT = x.SoLuongBao;
                x.GiaVon = x.DonGia;
            });
            InsertGeneralLedger(instance.DataClauseDetails, result);
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            return result;
        }
        public NvVatTuChungTu UpdatePhieu(NvXuatKhacVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            instance.Calc();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
           
            var masterData = Mapper.Map<NvXuatKhacVm.Dto, NvVatTuChungTu>(instance);
            var detailData = Mapper.Map<List<NvXuatKhacVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);

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
        public void InsertGeneralLedger(List<NvXuatKhacVm.DtoClauseDetail> data, NvVatTuChungTu chungTu)
        {
            var generalLedgers = Mapper.Map<List<NvXuatKhacVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
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
        public void UpdateGeneralLedger(List<NvXuatKhacVm.DtoClauseDetail> data, NvVatTuChungTu exsitItem)
        {
            var generalLedgers = Mapper.Map<List<NvXuatKhacVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
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
        public NvXuatKhacVm.ReportModel CreateReport(string id)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var result = new NvXuatKhacVm.ReportModel();
            decimal sum = 0;
            var exsit = FindById(id);
            if (exsit != null)
            {
                result = Mapper.Map<NvVatTuChungTu, NvXuatKhacVm.ReportModel>(exsit);
                var nhanvien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == exsit.ICreateBy).FirstOrDefault();
                if (nhanvien != null)
                {

                    result.NameNhanVienCreate = nhanvien.TenNhanVien != null ? nhanvien.TenNhanVien : "";
                }
                var detailData = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsit.MaChungTuPk).ToList();
                result.DataReportDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvXuatKhacVm.ReportDetailModel>>(detailData);
                var sup = UnitOfWork.Repository<MdSupplier>().DbSet.FirstOrDefault(x => x.MaNCC == result.MaKhachHang);
                var cus = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == result.MaKhachHang);
                if (sup != null)
                {
                    result.TenKhachHang = sup.TenNCC;
                    result.DiaChiKhachHang = sup.DiaChi;
                }
                else if (cus != null)
                {
                    result.TenKhachHang = cus.TenKH;
                    result.DiaChiKhachHang = cus.DiaChi;
                }
                var typeR = UnitOfWork.Repository<MdTypeReason>().DbSet.FirstOrDefault(x => x.MaLyDo == result.MaLyDo);
                if(typeR != null)
                {
                    result.TenLyDo = typeR.TenLyDo;
                }
                if (exsit.TrangThai != 10)
                {
                    List<NvXuatKhacVm.DtoDetail> listDetails = new List<NvXuatKhacVm.DtoDetail>();
                    var ky = CurrentSetting.GetKhoaSo(exsit.UnitCode);
                    var tableName = ProcedureCollection.GetTableName(ky.Year, ky.Period);
                    var MaKho = exsit.MaKhoXuat;
                    //string kyKeToan = _servicePeriod.GetKyKeToan((DateTime)phieu.NgayCT);
                    foreach (var value in result.DataReportDetails)
                    {
                        List<MdMerchandiseVm.DataXNT> data = ProcedureCollection.GetDataInventoryByCondition(exsit.UnitCode, MaKho, value.MaHang, tableName, _ParentUnitCode);
                        value.GiaVon = value.GiaVon;
                        value.DonGia = value.GiaVon;
                        value.ThanhTien = value.DonGia * value.SoLuong;
                        sum += (decimal)value.ThanhTien;
                    }
                    var tyLe = UnitOfWork.Repository<MdTax>().DbSet.Where(x => x.MaLoaiThue == exsit.VAT).Select(x => x.TaxRate).FirstOrDefault();
                    if (tyLe != null)
                    {
                        result.TienVat = sum * (tyLe / 100);

                    }
                    else
                    {
                        result.TienVat = 0;
                    }

                    result.ThanhTienTruocVat = sum;
                    result.ThanhTienSauVat = result.ThanhTienTruocVat + result.TienVat;
                }
                var warehouses = UnitOfWork.Repository<MdWareHouse>().DbSet;
                var exportWareHouse = warehouses.FirstOrDefault(x => x.MaKho == result.MaKhoXuat);                
                result.TenKhoXuat = exportWareHouse != null ? exportWareHouse.TenKho : "";
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
