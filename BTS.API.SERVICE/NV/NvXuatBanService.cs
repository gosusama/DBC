using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Common;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BTS.API.SERVICE.NV
{
    public interface INvXuatBanService : IDataInfoService<NvVatTuChungTu>
    {
        NvVatTuChungTu InsertPhieu(NvXuatBanVm.Dto instance);
        NvVatTuChungTu UpdatePhieu(NvXuatBanVm.Dto instance);
        NvVatTuChungTu UpdateStatus(NvXuatBanVm.Dto instance);
        StateProcessApproval Approval(NvVatTuChungTu chungTu);
        NvXuatBanVm.ReportModel CreateReport(string id);
        NvXuatBanVm.Dto CreateNewInstance();
        MemoryStream ExportExcel(List<NvXuatBanVm.Dto> data, FilterObj<NvXuatBanVm.Search> filter);
        MemoryStream ExportExcelByNhaCungCap(FilterObj<NvXuatBanVm.Search> filter);
        MemoryStream ExportExcelByMerchandise(FilterObj<NvXuatBanVm.Search> filter);
        MemoryStream ExportExcelByMerchandiseType(FilterObj<NvXuatBanVm.Search> filter);
        MemoryStream ExportExcelByMerchandiseGroup(FilterObj<NvXuatBanVm.Search> filter);

        MemoryStream ExportExcelDetail(ParameterXuatBan pc);
        //MemoryStream ExportExcelDetailForMerchandise(ParameterXuatBan pc);
        MemoryStream ExportExcelXBTongHop(ParameterXuatBan pc);
        List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportXuatBan(ParameterXuatBan pc);
        MemoryStream ExportExcelXKDetail(ParameterXuatBan pc);
        MemoryStream ExportExcelXKTongHop(ParameterXuatBan pc);
        List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportXuatKhac(ParameterXuatBan pc);
        MemoryStream ExportExcelDCXDetail(ParameterXuatBan pc);
        MemoryStream ExportExcelDCXTongHop(ParameterXuatBan pc);
        List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportDieuChuyenXuat(ParameterXuatBan pc);
        bool DeletePhieu(string id);

    }
    public class NvXuatBanService : DataInfoServiceBase<NvVatTuChungTu>, INvXuatBanService
    {
        public NvXuatBanService(IUnitOfWork unitOfWork) : base(unitOfWork)
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
        public NvXuatBanVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            var code = BuildCode_PTNX(TypeVoucher.XBAN.ToString(), unitCode, false);
            return new NvXuatBanVm.Dto()
            {
                LoaiPhieu = TypeVoucher.XBAN.ToString(),
                MaChungTu = code,
                MaHoaDon = code,
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
            };
        }

        public NvVatTuChungTu InsertPhieu(NvXuatBanVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            instance.Calc(); //Tinh lại cac thuộc tính thứ sinh
            NvVatTuChungTu item = new NvVatTuChungTu();
            List<NvVatTuChungTuChiTiet> detailData = new List<NvVatTuChungTuChiTiet>();
            item.LoaiPhieu = instance.LoaiPhieu;
            item.MaChungTuPk = instance.MaChungTuPk;
            item.MaChungTu = instance.MaChungTu;
            item.NgayCT = instance.NgayCT;
            item.NgayDuyetPhieu = instance.NgayDuyetPhieu;
            item.MaKhoXuat = instance.MaKhoXuat;
            item.SoPhieuDatHang = instance.SoPhieuDatHang;
            item.MaKhoNhap = instance.MaKhoNhap;
            item.MaHoaDon = instance.MaHoaDon;
            item.NgayHoaDon = instance.NgayCT;
            item.MaDonViNhan = instance.MaDonViNhan;
            item.MaDonViXuat = GetCurrentUnitCode();
            item.MaKhachHang = instance.MaKhachHang;
            item.MaHang = instance.MaVatTu;
            item.MaSoThue = instance.MaSoThue;
            item.TenNgh = instance.TenNgh;
            item.NoiDung = instance.NoiDung;
            item.MaLyDo = instance.MaLyDo;
            item.ThanhTienTruocVat = instance.ThanhTienTruocVat != null ? instance.ThanhTienTruocVat : 0;
            item.TienChietKhau = instance.TienChietKhau != null ? instance.TienChietKhau : 0;
            item.TienVat = instance.TienVat != null ? instance.TienVat : 0;
            item.TongTienGiamGia = instance.TongTienGiamGia != null ? instance.TongTienGiamGia : 0;
            item.ThanhTienSauVat = instance.ThanhTienSauVat != null ? instance.ThanhTienSauVat : 0;
            item.VAT = instance.VAT;
            item.TrangThai = 0;
            item.TrangThaiThanhToan = instance.TrangThaiThanhToan;
            item.TienMat = instance.TienMat != null ? instance.TienMat : 0;
            item.TienThe = instance.TienThe != null ? instance.TienThe : 0;
            item.TienCOD = instance.TienCOD != null ? instance.TienCOD : 0;
            item.MaNhanVien = instance.MaNhanVien;
            item.TenNn = instance.TenNn;
            item.TienNoCu = instance.TienNoCu != null ? instance.TienNoCu : 0;
            item.TienThanhToan = instance.TienThanhToan != null ? instance.TienThanhToan : 0;
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            string _unitCode = GetCurrentUnitCode();
            result.MaChungTu = BuildCode_PTNX(TypeVoucher.XBAN.ToString(), _unitCode, true);
            result.GenerateMaChungTuPk();
            item.ICreateBy = GetClaimsPrincipal().Identity.Name;
            item.ICreateDate = DateTime.Now;
            result = Insert(result);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var merchandisePriceCollection = UnitOfWork.Repository<MdMerchandisePrice>().DbSet;
            int index = 0;
            if (instance.DataDetails.Count > 0)
            {
                foreach (var row in instance.DataDetails)
                {
                    decimal giaMuaCoVat = 0;
                    NvVatTuChungTuChiTiet chungtu = new NvVatTuChungTuChiTiet();
                    var merchandise =
                        merchandiseCollection.FirstOrDefault(
                            u => u.MaVatTu == row.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                    var merchandisePrice =
                        merchandisePriceCollection.FirstOrDefault(
                            u => u.MaVatTu == row.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                    chungtu.Id = Guid.NewGuid().ToString();
                    chungtu.TenHang = merchandise.TenHang;
                    chungtu.MaKhachHang = merchandise.MaKhachHang;
                    chungtu.MaChungTu = result.MaChungTu;
                    chungtu.MaChungTuPk = result.MaChungTuPk;
                    chungtu.MaHang = row.MaHang;
                    chungtu.TenHang = row.TenHang;
                    chungtu.MaBaoBi = row.MaBaoBi;
                    chungtu.DonViTinh = row.DonViTinh;
                    chungtu.Barcode = row.Barcode;
                    chungtu.VAT = merchandise.MaVatRa;
                    chungtu.LuongBao = row.LuongBao;
                    chungtu.SoLuongLe = row.SoLuongLe;
                    chungtu.SoLuongBaoCT = row.SoLuongBaoCT;
                    chungtu.SoLuongCT = row.SoLuongCT;
                    chungtu.SoLuongBao = row.SoLuongBao;
                    chungtu.SoLuong = row.SoLuongLe;
                    chungtu.DonGia = row.DonGia;
                    chungtu.ThanhTien = row.ThanhTien;
                    chungtu.TienGiamGia = row.TienGiamGia;
                    chungtu.GiaVon = row.GiaVon;
                    if (merchandisePrice != null)
                    {
                        decimal.TryParse(merchandisePrice.GiaMuaVat.ToString(), out giaMuaCoVat);
                    }
                    else
                    {
                        giaMuaCoVat = 0;
                    }
                    chungtu.GiaMuaCoVat = giaMuaCoVat;
                    chungtu.Index = ++index;
                    detailData.Add(chungtu);
                }
            }
            else
            {
                return null;
            }
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            return result;
        }
        public NvVatTuChungTu UpdatePhieu(NvXuatBanVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            NvVatTuChungTu masterData = new NvVatTuChungTu();
            List<NvVatTuChungTuChiTiet> detailData = new List<NvVatTuChungTuChiTiet>();
            masterData.LoaiPhieu = instance.LoaiPhieu;
            masterData.MaChungTuPk = instance.MaChungTuPk;
            masterData.MaChungTu = instance.MaChungTu;
            masterData.NgayCT = instance.NgayCT;
            masterData.NgayDuyetPhieu = instance.NgayDuyetPhieu;
            masterData.MaKhoXuat = instance.MaKhoXuat;
            masterData.SoPhieuDatHang = instance.SoPhieuDatHang;
            masterData.MaKhoNhap = instance.MaKhoNhap;
            masterData.MaHoaDon = instance.MaHoaDon;
            masterData.NgayHoaDon = instance.NgayCT;
            masterData.MaDonViNhan = instance.MaDonViNhan;
            masterData.MaDonViXuat = GetCurrentUnitCode();
            masterData.MaKhachHang = instance.MaKhachHang;
            masterData.MaHang = instance.MaVatTu;
            masterData.MaSoThue = instance.MaSoThue;
            masterData.TenNgh = instance.TenNgh;
            masterData.NoiDung = instance.NoiDung;
            masterData.MaLyDo = instance.MaLyDo;
            masterData.ThanhTienTruocVat = instance.ThanhTienTruocVat != null ? instance.ThanhTienTruocVat : 0;
            masterData.TienChietKhau = instance.TienChietKhau != null ? instance.TienChietKhau : 0;
            masterData.TienVat = instance.TienVat != null ? instance.TienVat : 0;
            masterData.TongTienGiamGia = instance.TongTienGiamGia != null ? instance.TongTienGiamGia : 0;
            masterData.ThanhTienSauVat = instance.ThanhTienSauVat != null ? instance.ThanhTienSauVat : 0;
            masterData.VAT = instance.VAT;
            masterData.TrangThai = instance.TrangThai;
            masterData.TrangThaiThanhToan = instance.TrangThaiThanhToan;
            masterData.TienMat = instance.TienMat != null ? instance.TienMat : 0;
            masterData.TienThe = instance.TienThe != null ? instance.TienThe : 0;
            masterData.TienCOD = instance.TienCOD != null ? instance.TienCOD : 0;
            masterData.MaNhanVien = instance.MaNhanVien;
            masterData.TenNn = instance.TenNn;
            masterData.TienNoCu = instance.TienNoCu != null ? instance.TienNoCu : 0;
            masterData.TienThanhToan = instance.TienThanhToan != null ? instance.TienThanhToan : 0;
            masterData.Id = exsitItem.Id;
            masterData.ICreateBy = exsitItem.ICreateBy;
            masterData.ICreateDate = exsitItem.ICreateDate;
            masterData.IUpdateBy = GetClaimsPrincipal().Identity.Name;
            masterData.IUpdateDate = DateTime.Now;
            masterData.UnitCode = GetCurrentUnitCode();
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var merchandisePriceCollection = UnitOfWork.Repository<MdMerchandisePrice>().DbSet;
            if (instance.DataDetails.Count > 0)
            {
                foreach (var row in instance.DataDetails)
                {
                    decimal giaMuaCoVat = 0;
                    NvVatTuChungTuChiTiet chungtu = new NvVatTuChungTuChiTiet();
                    var merchandise =
                        merchandiseCollection.FirstOrDefault(
                            u => u.MaVatTu == row.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                    var merchandisePrice =
                        merchandisePriceCollection.FirstOrDefault(
                            u => u.MaVatTu == row.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                    chungtu.Id = Guid.NewGuid().ToString();
                    chungtu.TenHang = merchandise.TenHang;
                    chungtu.MaKhachHang = merchandise.MaKhachHang;
                    chungtu.MaChungTu = masterData.MaChungTu;
                    chungtu.MaChungTuPk = masterData.MaChungTuPk;
                    chungtu.MaHang = row.MaHang;
                    chungtu.TenHang = row.TenHang;
                    chungtu.MaBaoBi = row.MaBaoBi;
                    chungtu.DonViTinh = row.DonViTinh;
                    chungtu.Barcode = row.Barcode;
                    chungtu.VAT = merchandise.MaVatRa;
                    chungtu.LuongBao = row.LuongBao;
                    chungtu.SoLuongLe = row.SoLuongLe;
                    chungtu.SoLuongBaoCT = row.SoLuongBaoCT;
                    chungtu.SoLuongCT = row.SoLuongCT;
                    chungtu.SoLuongBao = row.SoLuongBao;
                    chungtu.SoLuong = row.SoLuongLe;
                    chungtu.DonGia = row.DonGia;
                    chungtu.ThanhTien = row.ThanhTien;
                    chungtu.TienGiamGia = row.TienGiamGia;
                    chungtu.GiaVon = row.GiaVon;
                    if (merchandisePrice != null)
                    {
                        decimal.TryParse(merchandisePrice.GiaMuaVat.ToString(), out giaMuaCoVat);
                    }
                    else
                    {
                        giaMuaCoVat = 0;
                    }
                    chungtu.GiaMuaCoVat = giaMuaCoVat;
                    detailData.Add(chungtu);
                }
            }
            else
            {
                return null;
            }
            //xóa dữ liệu
            var detailCollection = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsitItem.MaChungTuPk);
            detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            //end
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            var result = Update(masterData);
            return result;
        }
        public NvVatTuChungTu UpdateStatus(NvXuatBanVm.Dto instance)
        {
            var result = new NvVatTuChungTu();
            var vatTuChungTuCollection = UnitOfWork.Repository<NvVatTuChungTu>().DbSet;
            if (instance != null)
            {
                var masterData = vatTuChungTuCollection.FirstOrDefault(x => x.Id == instance.Id);
                masterData.TienThe = instance.TienThe;
                masterData.TienCOD = instance.TienCOD;
                masterData.TienMat = instance.TienMat;
                masterData.TrangThaiThanhToan = instance.TrangThaiThanhToan;
                result = Update(masterData);
            }
            else
            {
                result = null;
            }
            return result;
        }

        public void InsertGeneralLedger(List<NvXuatBanVm.DtoClauseDetail> data, NvVatTuChungTu chungTu)
        {
            var generalLedgers = Mapper.Map<List<NvXuatBanVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
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
        public void UpdateGeneralLedger(List<NvXuatBanVm.DtoClauseDetail> data, NvVatTuChungTu exsitItem)
        {
           
            var generalLedgers = Mapper.Map<List<NvXuatBanVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
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
        public NvXuatBanVm.ReportModel CreateReport(string id)
        {
            var result = new NvXuatBanVm.ReportModel();
            var _ParentUnitCode = GetParentUnitCode();
            
            var exsit = FindById(id);
            if (exsit != null)
            {
                result = Mapper.Map<NvVatTuChungTu, NvXuatBanVm.ReportModel>(exsit);
                var nhanvien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == exsit.ICreateBy && x.UnitCode.StartsWith(_ParentUnitCode)).FirstOrDefault();
                if (nhanvien != null)
                {
                    result.NameNhanVienCreate = nhanvien.TenNhanVien != null ? nhanvien.TenNhanVien : "";
                    result.TenNhanVien = nhanvien.TenNhanVien;
                    result.MaNhanVien = nhanvien.MaNhanVien;
                }
                var detailData = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsit.MaChungTuPk).OrderBy(x => x.Index).ToList();
                foreach (var dt in detailData)
                {
                    //dt.DonGia -= (dt.TienGiamGia / dt.SoLuong);
                    var item = UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu.Equals(dt.MaHang) && x.UnitCode.StartsWith(_ParentUnitCode));
                    if (item != null) dt.TenHang = item.TenHang;
                }
                result.DataReportDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvXuatBanVm.ReportDetailModel>>(detailData);

                var customer = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == result.MaKhachHang && x.UnitCode.StartsWith(_ParentUnitCode));
                if (customer != null)
                {
                    result.TenKhachHang = customer.TenKH;
                    result.DienThoai = customer.DienThoai;
                    result.DiaChiKhachHang = customer.DiaChi;
                }
                var kho = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoXuat && x.UnitCode.StartsWith(_ParentUnitCode));
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
                var nhanvien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                if (nhanvien != null)
                {
                    result.Username = nhanvien.TenNhanVien;


                }
                else
                {
                    result.Username = "Administrator";
                }
            }
            //
            
            return result;
        }

        public StateProcessApproval Approval(NvVatTuChungTu chungTu)
        {
            var unitCode = GetCurrentUnitCode();
            //Process XNT...
            StateProcessApproval result;
            var periods = CurrentSetting.GetKhoaSo(unitCode);
            if (periods != null)
            {
                var tableName = ProcedureCollection.GetTableName(periods.Year, periods.Period);
                if (ProcedureCollection.DecreaseVoucher(tableName, periods.Year, periods.Period, chungTu.Id))
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
            string _unitCode = GetCurrentUnitCode();
            return x => x.MaChungTuPk == instance.MaChungTuPk && x.UnitCode == _unitCode;
        }
        #region Old Version
        public MemoryStream ExportExcel(List<NvXuatBanVm.Dto> data, FilterObj<NvXuatBanVm.Search> filter)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();

                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 4;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 7].Merge = true;
                worksheet.Cells[2, 1, 2, 7].Merge = true;
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU XUẤT BÁN BUÔN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                     string.Format("Từ ngày: {0} Đến ngày: {1}",
                     filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                     filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[3, 1].Value = "STT"; worksheet.Cells[3, 1].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 2].Value = "Ngày"; worksheet.Cells[3, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 3].Value = "Lý do"; worksheet.Cells[3, 3].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 4].Value = "Khách hàng"; worksheet.Cells[3, 4].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 5].Value = "Kho xuất"; worksheet.Cells[3, 5].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 6].Value = "Tổng tiền"; worksheet.Cells[3, 6].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 7].Value = "Trạng thái"; worksheet.Cells[3, 7].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                decimal itemTotal = 0;
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in data)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.NgayCT.HasValue ? item.NgayCT.Value.ToString("dd/MM/yyyy") : "";
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.NoiDung;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.MaKhachHang;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.MaKhoXuat;
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.ThanhTienSauVat; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value =
                        item.TrangThai == 10 ? "Hoàn thành" : "Chưa duyệt";
                    worksheet.Cells[currentRow, startColumn + 6].Style.Font.Color.SetColor(item.TrangThai == 10 ? Color.DarkBlue : Color.Black);
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 6].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal += item.ThanhTienSauVat;
                    currentRow = currentRow + 1;
                }
                worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
                worksheet.Cells[1, 1].Value = "TỔNG"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();

                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }

        public MemoryStream ExportExcelByMerchandise(FilterObj<NvXuatBanVm.Search> filter)
        {
            var itemCollection = new List<NvXuatBanVm.ObjectReport>();
            List<NvXuatBanVm.ObjectReport> itemCollectionGroup = new List<NvXuatBanVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuXuatBanGroupByMerchandise(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
            if (data != null)
            {
                itemCollectionGroup = data.ToList();
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 5;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU XUẤT BÁN BUÔN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}",
                    filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                    filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Vật tư";
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã hàng"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên hàng"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tiền CK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tổng tiền"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvXuatBanVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in itemCollectionGroup)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuong;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.TienHang; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.TienChietKhau; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.TienVat; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.TongTien; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    itemTotal.TienChietKhau = itemTotal.TienChietKhau + item.TienChietKhau;
                    itemTotal.TienVat = itemTotal.TienVat + item.TienVat;
                    itemTotal.TongTien = itemTotal.TongTien + item.TongTien;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienChietKhau; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienVat; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TongTien; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByNhaCungCap(FilterObj<NvXuatBanVm.Search> filter)
        {
            var itemCollection = new List<NvXuatBanVm.ObjectReport>();
            List<NvXuatBanVm.ObjectReport> itemCollectionGroup = new List<NvXuatBanVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuXuatBanGroupByNhaCungCap(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
            if (data != null)
            {
                itemCollectionGroup = data.ToList();
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 5;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU XUẤT BÁN BUÔN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}",
                    filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                    filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Nhà cung cấp";
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã NCC"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên NCC"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tiền CK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tổng tiền"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvXuatBanVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in itemCollectionGroup)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuong;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.TienHang; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.TienChietKhau; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.TienVat; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.TongTien; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    itemTotal.TienChietKhau = itemTotal.TienChietKhau + item.TienChietKhau;
                    itemTotal.TienVat = itemTotal.TienVat + item.TienVat;
                    itemTotal.TongTien = itemTotal.TongTien + item.TongTien;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienChietKhau; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienVat; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TongTien; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByMerchandiseType(FilterObj<NvXuatBanVm.Search> filter)
        {
            var itemCollection = new List<NvXuatBanVm.ObjectReport>();
            List<NvXuatBanVm.ObjectReport> itemCollectionGroup = new List<NvXuatBanVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuXuatBanGroupByMerchandiseType(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
            if (data != null)
            {
                itemCollectionGroup = data.ToList();
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 5;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU XUẤT BÁN BUÔN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}",
                    filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                    filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Loại vật tư";
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã loại hàng"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên loại hàng"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tiền CK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tổng tiền"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvXuatBanVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in itemCollectionGroup)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuong;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.TienHang; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.TienChietKhau; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.TienVat; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.TongTien; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    itemTotal.TienChietKhau = itemTotal.TienChietKhau + item.TienChietKhau;
                    itemTotal.TienVat = itemTotal.TienVat + item.TienVat;
                    itemTotal.TongTien = itemTotal.TongTien + item.TongTien;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienVat; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienChietKhau; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienVat; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TongTien; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByMerchandiseGroup(FilterObj<NvXuatBanVm.Search> filter)
        {
            var itemCollection = new List<NvXuatBanVm.ObjectReport>();
            List<NvXuatBanVm.ObjectReport> itemCollectionGroup = new List<NvXuatBanVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuXuatBanGroupByMerchandiseGroup(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
            if (data != null)
            {
                itemCollectionGroup = data.ToList();
            }



            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 5;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU XUẤT BÁN BUÔN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}",
                    filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                    filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Vật tư";
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã nhóm hàng"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên nhóm hàng"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tiền CK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tổng tiền"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvXuatBanVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in itemCollectionGroup)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuong;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.TienHang; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.TienChietKhau; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.TienVat; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.TongTien; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    itemTotal.TienChietKhau = itemTotal.TienChietKhau + item.TienChietKhau;
                    itemTotal.TienVat = itemTotal.TienVat + item.TienVat;
                    itemTotal.TongTien = itemTotal.TongTien + item.TongTien;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienVat; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienChietKhau; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienVat; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TongTien; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        //public MemoryStream ExportExcelDetailForMerchandise(ParameterXuatBan pi)
        //{
        //    var itemCollection = new List<NvGiaoDichQuayVm.ObjectReportCha>();
        //    List<NvGiaoDichQuayVm.ObjectReportCha> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportCha>();
        //    List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha>();
        //    var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
        //    var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);
        //    data = ProcedureCollection.XBChiTietGroupByM(ky, "MAVATTU",pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
        //    if (data != null)
        //    {
        //        itemCollectionGroup.AddRange(data.ToList());
        //    }

        //    using (ExcelPackage package = new ExcelPackage())
        //    {

        //        var ms = new MemoryStream();
        //        package.Workbook.Worksheets.Add("Data");
        //        var worksheet = package.Workbook.Worksheets[1];
        //        int startRow = 5;
        //        int startColumn = 1;

        //        ///Header
        //        ///
        //        worksheet.Cells[1, 1, 1, 12].Merge = true;
        //        worksheet.Cells[1, 1].Value = "BẢNG KÊ XUẤT BÁN BUÔN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        worksheet.Cells[2, 1].Value =
        //            string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
        //            pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
        //        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        worksheet.Cells[2, 1, 2, 12].Merge = true;
        //        worksheet.Cells[3, 1, 3, 12].Merge = true;
        //        worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Vật tư";
        //        worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 3].Value = "Danh sách"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 5].Value = "Vốn"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 6].Value = "Tiền thuế"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 7].Value = "T.Doanh thu"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 8].Value = "T.Tiền bán"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 9].Value = "T.Tiền KM"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 10].Value = "Lãi bán lẻ"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 11].Value = "Tỉ lệ lãi"; worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        worksheet.Cells[4, 12].Value = "Ghi chú"; worksheet.Cells[4, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //        var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
        //        int currentRow = startRow;
        //        int stt = 0;

        //        foreach (var item in itemCollectionGroup)
        //        {
        //            stt = 0;
        //            worksheet.Cells[currentRow, 1, currentRow, 12].Merge = true;
        //            worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //            worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
        //            worksheet.Cells[currentRow, startColumn].Value = item.Ma + " - " + item.Ten;
        //            currentRow++;
        //            foreach (var itemdetails in item.DataDetails)
        //            {
        //                ++stt;
        //                worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
        //                worksheet.Cells[currentRow, startColumn + 1].Value = itemdetails.Ma;
        //                worksheet.Cells[currentRow, startColumn + 2].Value = itemdetails.Ten;
        //                worksheet.Cells[currentRow, startColumn + 3].Value = itemdetails.SoLuongBan;
        //                worksheet.Cells[currentRow, startColumn + 4].Value = itemdetails.Von; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
        //                worksheet.Cells[currentRow, startColumn + 5].Value = itemdetails.TienThue; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
        //                worksheet.Cells[currentRow, startColumn + 6].Value = itemdetails.DoanhThu; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
        //                worksheet.Cells[currentRow, startColumn + 7].Value = itemdetails.TienBan; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
        //                worksheet.Cells[currentRow, startColumn + 8].Value = itemdetails.TienKhuyenMai; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
        //                worksheet.Cells[currentRow, startColumn + 9].Value = itemdetails.LaiBanLe; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
        //                worksheet.Cells[currentRow, startColumn + 10].Value = itemdetails.Von == 0 ? 0 : itemdetails.LaiBanLe / itemdetails.Von; worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
        //                worksheet.Cells[currentRow, startColumn + 11].Value = itemdetails.Von == 0 ? "Có thể chưa nhập" : "";
        //                worksheet.Cells[currentRow, 1, currentRow, startColumn + 11].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
        //                itemTotal.SoLuongBan = itemTotal.SoLuongBan + itemdetails.SoLuongBan;
        //                itemTotal.Von = itemTotal.Von + itemdetails.Von;
        //                itemTotal.TienThue = itemTotal.TienThue + itemdetails.TienThue;
        //                itemTotal.DoanhThu = itemTotal.DoanhThu + itemdetails.DoanhThu;
        //                itemTotal.TienBan = itemTotal.TienBan + itemdetails.TienBan;
        //                itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + itemdetails.TienKhuyenMai;
        //                itemTotal.LaiBanLe = itemTotal.LaiBanLe + itemdetails.LaiBanLe;
        //                currentRow++;
        //            }
        //        }
        //        worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
        //        worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuongBan;
        //        worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.Von; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
        //        worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
        //        worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
        //        worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
        //        worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
        //        worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.LaiBanLe; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
        //        worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.Von == 0 ? 0 : itemTotal.LaiBanLe / itemTotal.Von; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
        //        worksheet.Cells[currentRow, 1, currentRow, startColumn + 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //        worksheet.Column(1).AutoFit();
        //        worksheet.Column(2).AutoFit();
        //        worksheet.Column(3).AutoFit();
        //        worksheet.Column(4).AutoFit();
        //        worksheet.Column(5).AutoFit();
        //        worksheet.Column(6).AutoFit();
        //        worksheet.Column(7).AutoFit();
        //        worksheet.Column(8).AutoFit();
        //        worksheet.Column(9).AutoFit();
        //        worksheet.Column(10).AutoFit();
        //        worksheet.Column(11).AutoFit();
        //        worksheet.Column(12).AutoFit();
        //        int totalRows = worksheet.Dimension.End.Row;
        //        int totalCols = worksheet.Dimension.End.Column;
        //        var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
        //        var dataFont = dataCells.Style.Font;
        //        dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
        //        package.SaveAs(ms);
        //        return ms;
        //    }
        //}
        #endregion
        #region Lastest Version - Xuat Ban Buon
        public MemoryStream ExportExcelXBTongHop(ParameterXuatBan pi)
        {
            List<NvGiaoDichQuayVm.ObjectReportLevel2> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportLevel2>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportLevel2> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportLevel2>();

            var titleCotName = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);
            data = ReportXuatBan(pi);
            if (data != null)
            {
                itemCollectionGroup.AddRange(data.ToList());
            }
            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MALOAIVATTU:
                    titleCotName = "Loại hàng";
                    break;
                case TypeGroupXuatBan.MAKHO:
                    titleCotName = "Kho hàng";
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    titleCotName = "Nhóm hàng";
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    titleCotName = "Nhà cung cấp";
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    titleCotName = "Khách hàng";
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    titleCotName = "Giao dịch";
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    titleCotName = "Đơn vị nhận";
                    break;
                case TypeGroupXuatBan.MADONVIXUAT:
                    titleCotName = "Đơn vị xuất";
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    titleCotName = "Loại thuế";
                    break;
                case TypeGroupXuatBan.MAXUATXU:
                    titleCotName = "Xuất xứ";
                    break;
                default:
                    titleCotName = "Mặt hàng";
                    break;
            }
            var _UnitCode = GetCurrentUnitCode();
            string userName = string.Empty;
            //GetNhanVien
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                var name = currentUser.Identity.Name;
                var nhanVien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name && x.UnitCode.StartsWith(_UnitCode)).FirstOrDefault();
                if (nhanVien != null)
                {
                    userName = nhanVien.TenNhanVien;
                }
                else
                {
                    userName = "Administrator";
                }
            }

            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 7;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 6].Merge = true; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[2, 1, 2, 6].Merge = true; worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[1, 7, 1, 13].Merge = true; worksheet.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[2, 7, 2, 13].Merge = true; worksheet.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[1, 1].Value = CurrentSetting.GetUnitName(pi.UnitCode);
                worksheet.Cells[2, 1].Value = CurrentSetting.GetUnitAddress(pi.UnitCode);
                worksheet.Cells[1, 7].Value = string.Format("Ngày in: {0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                worksheet.Cells[2, 7].Value = "Người in: " + userName;
                worksheet.Cells[3, 1, 3, 13].Merge = true;
                worksheet.Cells[3, 1].Value = "BẢNG KÊ XUẤT BÁN BUÔN"; worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; worksheet.Cells[3, 1].Style.Font.Bold = true;
                worksheet.Cells[4, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[4, 1, 4, 13].Merge = true;
                worksheet.Cells[5, 1, 5, 13].Merge = true;

                worksheet.Cells[5, 1].Value = "Điều kiện, Nhóm theo: " + titleCotName;
                worksheet.Cells[6, 1].Value = "STT"; worksheet.Cells[6, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 2].Value = "Mã"; worksheet.Cells[6, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 3].Value = "Danh sách"; worksheet.Cells[6, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 4].Value = "Số lượng"; worksheet.Cells[6, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 5].Value = "Vốn chưa VAT"; worksheet.Cells[6, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 6].Value = "Tiền thuế"; worksheet.Cells[6, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 7].Value = "T.Tiền KM"; worksheet.Cells[6, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 8].Value = "T.Tiền Mặt"; worksheet.Cells[6, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 9].Value = "T.Tiền Chuyển khoản"; worksheet.Cells[6, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 10].Value = "T.Tiền COD"; worksheet.Cells[6, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 11].Value = "T.Doanh thu"; worksheet.Cells[6, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 12].Value = "T.Tiền bán"; worksheet.Cells[6, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 13].Value = "Lãi bán lẻ"; worksheet.Cells[6, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 14].Value = "Tỉ lệ lãi"; worksheet.Cells[6, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 15].Value = "Ghi chú"; worksheet.Cells[6, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;
                foreach (var itemdetails in itemCollectionGroup)
                {
                    string tenDonVi = string.Empty;
                    var unit = UnitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == itemdetails.Ma);
                    if (unit != null) tenDonVi = unit.TenDonVi;
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 18].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = itemdetails.Ma + " - " + tenDonVi;
                    currentRow++;
                    foreach (var item in itemdetails.DataDetails)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                        worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                        worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuongBan;
                        worksheet.Cells[currentRow, startColumn + 4].Value = item.Von; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 5].Value = item.TienThue; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = item.TienKhuyenMai; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = item.TienMat; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = item.TienChuyenKhoan; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = item.TienCod; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = item.DoanhThu; worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 11].Value = item.TienBan; worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 12].Value = item.LaiBanLe; worksheet.Cells[currentRow, 13].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 13].Value = item.Von == 0 ? 0 : item.LaiBanLe / item.Von; worksheet.Cells[currentRow, 14].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 14].Value = "";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 14].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        itemTotal.SoLuongBan = itemTotal.SoLuongBan + item.SoLuongBan;
                        itemTotal.Von = itemTotal.Von + item.Von;
                        itemTotal.TienThue = itemTotal.TienThue + item.TienThue;
                        itemTotal.DoanhThu = itemTotal.DoanhThu + item.DoanhThu;
                        itemTotal.TienBan = itemTotal.TienBan + item.TienBan;
                        itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + item.TienKhuyenMai;
                        itemTotal.TienChuyenKhoan += item.TienChuyenKhoan;
                        itemTotal.TienCod += item.TienCod;
                        itemTotal.TienMat += item.TienMat;
                        itemTotal.LaiBanLe = itemTotal.LaiBanLe + item.LaiBanLe;
                        currentRow++;
                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuongBan;
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.Von; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TienMat; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.TienChuyenKhoan; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.TienCod; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";

                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 11].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 11].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 12].Value = itemTotal.LaiBanLe; worksheet.Cells[currentRow, startColumn + 12].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 13].Value = itemTotal.Von == 0 ? 0 : itemTotal.LaiBanLe / itemTotal.Von; worksheet.Cells[currentRow, startColumn + 13].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();
                worksheet.Column(13).AutoFit();
                worksheet.Column(14).AutoFit();
                worksheet.Column(15).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelDetail(ParameterXuatBan pi)
        {
            var itemCollection = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCha> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha>();
            var titleCotMa = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);

            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MADONVIXUAT:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MADONVIXUAT.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Đơn vị xuất";
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MADONVINHAN.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Đơn vị nhận";
                    break;
                case TypeGroupXuatBan.MAKHO:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MAKHO.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Kho hàng";
                    break;
                case TypeGroupXuatBan.MALOAIVATTU:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MALOAIVATTU.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Loại hàng";
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MANHOMVATTU.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Nhóm hàng";
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Nhà cung cấp";
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MAKHACHHANG.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Khách hàng";
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MAGIAODICH.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Giao dịch";
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MALOAITHUE.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Loại thuế";
                    break;
                case TypeGroupXuatBan.MAXUATXU:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MAXUATXU.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Xuất xứ";
                    break;
                default:
                    data = ProcedureCollection.XBChiTiet(ky, InventoryGroupBy.MAVATTU.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    titleCotMa = "Mặt hàng";
                    break;
            }
            if (data != null)
            {
                itemCollectionGroup.AddRange(data.ToList());
            }

            using (ExcelPackage package = new ExcelPackage())
            {

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 5;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 12].Merge = true;
                worksheet.Cells[1, 1].Value = "BẢNG KÊ XUẤT BÁN BUÔN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 13].Merge = true;
                worksheet.Cells[3, 1, 3, 13].Merge = true;
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 14].Merge = true;
                worksheet.Cells[3, 1, 3, 14].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + titleCotMa;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Barcode"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Mã"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Danh sách"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Ngày"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Số lượng"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Vốn chưa VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tiền thuế"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "T.Doanh thu"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "T.Tiền bán"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Value = "T.Tiền KM"; worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 12].Value = "Lãi bán lẻ"; worksheet.Cells[4, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 13].Value = "Tỉ lệ lãi"; worksheet.Cells[4, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 14].Value = "Ghi chú"; worksheet.Cells[4, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;

                foreach (var item in itemCollectionGroup)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 14].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.MaDonVi + " : " + item.Ma + " - " + item.Ten;
                    currentRow++;
                    foreach (var itemdetails in item.DataDetails)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetails.Barcode;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetails.Ma;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetails.Ten;
                        worksheet.Cells[currentRow, startColumn + 4].Value = string.Format("{0}/{1}/{2}", itemdetails.NgayGiaoDich.Day, itemdetails.NgayGiaoDich.Month, itemdetails.NgayGiaoDich.Year);
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetails.SoLuongBan;
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetails.Von; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetails.TienThue; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetails.DoanhThu; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetails.TienBan; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetails.TienKhuyenMai; worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 11].Value = itemdetails.LaiBanLe; worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 12].Value = itemdetails.Von == 0 ? 0 : itemdetails.LaiBanLe / itemdetails.Von; worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 13].Value = itemdetails.Von == 0 ? "Có thể chưa nhập" : "";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 13].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        itemTotal.SoLuongBan = itemTotal.SoLuongBan + itemdetails.SoLuongBan;
                        itemTotal.Von = itemTotal.Von + itemdetails.Von;
                        itemTotal.TienThue = itemTotal.TienThue + itemdetails.TienThue;
                        itemTotal.DoanhThu = itemTotal.DoanhThu + itemdetails.DoanhThu;
                        itemTotal.TienBan = itemTotal.TienBan + itemdetails.TienBan;
                        itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + itemdetails.TienKhuyenMai;
                        itemTotal.LaiBanLe = itemTotal.LaiBanLe + itemdetails.LaiBanLe;
                        currentRow++;
                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 4].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.SoLuongBan;
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.Von; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 11].Value = itemTotal.LaiBanLe; worksheet.Cells[currentRow, startColumn + 11].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 12].Value = itemTotal.Von == 0 ? 0 : itemTotal.LaiBanLe / itemTotal.Von; worksheet.Cells[currentRow, startColumn + 12].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportXuatBan(ParameterXuatBan pi)
        {
            List<NvGiaoDichQuayVm.ObjectReportLevel2> result = new List<NvGiaoDichQuayVm.ObjectReportLevel2>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);
            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MADONVIXUAT:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MADONVIXUAT.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MADONVINHAN.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                case TypeGroupXuatBan.MALOAIVATTU:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MALOAIVATTU.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                case TypeGroupXuatBan.MAKHO:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MAKHO.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MANHOMVATTU.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MAKHACHHANG.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MAGIAODICH.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MALOAITHUE.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                case TypeGroupXuatBan.MAXUATXU:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MAXUATXU.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
                default:
                    data = ProcedureCollection.XBTongHop(ky, InventoryGroupBy.MAVATTU.ToString(), pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate, pi.IsPay);
                    break;
            }
            if (data.Count > 0)
            {
                var groupBy = data.GroupBy(x => x.MaDonVi).ToList();
                foreach (var item in groupBy)
                {
                    NvGiaoDichQuayVm.ObjectReportLevel2 obj = new NvGiaoDichQuayVm.ObjectReportLevel2();
                    obj.Ma = item.Key;
                    List<NvGiaoDichQuayVm.ObjectReport> lst = data.Where(x => x.MaDonVi == obj.Ma).ToList();
                    var groupByDkLoc = lst.GroupBy(x => new { x.Ma, x.Ten, x.MaDonVi })
                                            .Select(group => new NvGiaoDichQuayVm.ObjectReport()
                                            {
                                                Ma = group.Key.Ma,
                                                Ten = group.Key.Ten,
                                                MaDonVi = group.Key.MaDonVi,
                                                TienChuyenKhoan = group.Sum(a => a.TienChuyenKhoan),
                                                TienCod = group.Sum(a => a.TienCod),
                                                TienMat = group.Sum(a => a.TienMat),
                                                SoLuongBan = group.Sum(a => a.SoLuongBan),
                                                VonChuaVat = group.Sum(a => a.VonChuaVat),
                                                Von = group.Sum(a => a.Von),
                                                TienThue = group.Sum(a => a.TienThue),
                                                DoanhThu = group.Sum(a => a.DoanhThu),
                                                TienBan = group.Sum(a => a.TienBan),
                                                TienKhuyenMai = group.Sum(a => a.TienKhuyenMai),
                                                LaiBanLe = group.Sum(a => a.LaiBanLe),
                                            }).ToList();
                    obj.DataDetails.AddRange(groupByDkLoc);
                    result.Add(obj);
                }
            }
            return result;
        }
        #endregion
        #region Lastest Version - Xuat Khac
        public MemoryStream ExportExcelXKTongHop(ParameterXuatBan pi)
        {
            List<NvGiaoDichQuayVm.ObjectReport> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReport>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();

            var titleCotName = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);

            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MALOAIVATTU:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MALOAIVATTU.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MANHOMVATTU.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MAKHACHHANG.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MAKHO:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MAKHO.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MAGIAODICH.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MADONVI.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MALOAITHUE.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                default:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MAVATTU.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
            }

            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MALOAIVATTU:
                    titleCotName = "Loại hàng";
                    break;
                case TypeGroupXuatBan.MAKHO:
                    titleCotName = "Kho hàng";
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    titleCotName = "Nhóm hàng";
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    titleCotName = "Nhà cung cấp";
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    titleCotName = "Giao dịch";
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    titleCotName = "Đơn vị";
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    titleCotName = "Khách hàng";
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    titleCotName = "Loại thuế";
                    break;
                default:
                    titleCotName = "Mặt hàng";
                    break;
            }
            if (data != null)
            {
                itemCollectionGroup.AddRange(data.ToList());
            }

            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 5;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                var rt = UnitOfWork.Repository<MdTypeReason>().DbSet.Where(x => x.MaLyDo == pi.ReasonType).FirstOrDefault();
                if (rt != null)
                    worksheet.Cells[1, 1].Value = "BẢNG KÊ " + rt.TenLyDo; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: " + titleCotName;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Danh sách"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tiền CK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tổng tiền"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in itemCollectionGroup)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuongBan;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.DoanhThu; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.TienKhuyenMai; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.TienThue; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.TienBan; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.SoLuongBan = itemTotal.SoLuongBan + item.SoLuongBan;
                    itemTotal.DoanhThu = itemTotal.DoanhThu + item.DoanhThu;
                    itemTotal.TienThue = itemTotal.TienThue + item.TienThue;
                    itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + item.TienKhuyenMai;
                    itemTotal.TienBan = itemTotal.TienBan + item.TienBan;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuongBan;
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelXKDetail(ParameterXuatBan pi)
        {
            var itemCollection = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCha> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha>();
            var titleCotName = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);


            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MADONVIXUAT:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MADONVIXUAT.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MALOAIVATTU:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MALOAIVATTU.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MANHOMVATTU.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MAKHACHHANG.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MAKHO:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MAKHO.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MAGIAODICH.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MADONVI.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MALOAITHUE.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                default:
                    data = ProcedureCollection.XKChiTiet(ky, InventoryGroupBy.MAVATTU.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
            }

            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MADONVIXUAT:
                    titleCotName = "Đơn vị xuất";
                    break;
                case TypeGroupXuatBan.MALOAIVATTU:
                    titleCotName = "Loại hàng";
                    break;
                case TypeGroupXuatBan.MAKHO:
                    titleCotName = "Kho hàng";
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    titleCotName = "Nhóm hàng";
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    titleCotName = "Nhà cung cấp";
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    titleCotName = "Giao dịch";
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    titleCotName = "Đơn vị nhận";
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    titleCotName = "Khách hàng";
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    titleCotName = "Loại thuế";
                    break;
                default:
                    titleCotName = "Mặt hàng";
                    break;

            }
            if (data != null)
            {
                itemCollectionGroup.AddRange(data.ToList());
            }

            using (ExcelPackage package = new ExcelPackage())
            {

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 5;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 10].Merge = true;
                var rt = UnitOfWork.Repository<MdTypeReason>().DbSet.Where(x => x.MaLyDo == pi.ReasonType).FirstOrDefault();
                if (rt != null)
                    worksheet.Cells[1, 1].Value = "BẢNG KÊ " + rt.TenLyDo; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + titleCotName;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Barcode"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Mã"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Danh sách"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Ngày giao dịch"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Số lượng"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Vốn chưa VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tiền hàng"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "Tiền CK"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Tiền VAT"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Value = "Tổng tiền"; worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;

                foreach (var item in itemCollectionGroup)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 11].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Ma + " - " + item.Ten;
                    currentRow++;
                    foreach (var itemdetails in item.DataDetails)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetails.Barcode;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetails.Ma;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetails.Ten;
                        worksheet.Cells[currentRow, startColumn + 4].Value = string.Format("{0}/{1}/{2}", itemdetails.NgayGiaoDich.Day, itemdetails.NgayGiaoDich.Month, itemdetails.NgayGiaoDich.Year);
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetails.SoLuongBan;
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetails.Von; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetails.DoanhThu; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetails.TienKhuyenMai; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetails.TienThue; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetails.TienBan; worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        itemTotal.Von = itemTotal.Von + itemdetails.Von;
                        itemTotal.SoLuongBan = itemTotal.SoLuongBan + itemdetails.SoLuongBan;
                        itemTotal.DoanhThu = itemTotal.DoanhThu + itemdetails.DoanhThu;
                        itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + itemdetails.TienKhuyenMai;
                        itemTotal.TienThue = itemTotal.TienThue + itemdetails.TienThue;
                        itemTotal.TienBan = itemTotal.TienBan + itemdetails.TienBan;
                        currentRow++;
                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 4].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.SoLuongBan;
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.Von; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportXuatKhac(ParameterXuatBan pi)
        {
            List<NvGiaoDichQuayVm.ObjectReportLevel2> result = new List<NvGiaoDichQuayVm.ObjectReportLevel2>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);

            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MADONVIXUAT:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MADONVIXUAT.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MALOAIVATTU:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MALOAIVATTU.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MANHOMVATTU.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MAKHACHHANG.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MAKHO:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MAKHO.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MAGIAODICH.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MADONVI.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MALOAITHUE.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                default:
                    data = ProcedureCollection.XKTongHop(ky, InventoryGroupBy.MAVATTU.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
            }

            if (data.Count > 0)
            {
                var groupBy = data.GroupBy(x => x.MaDonVi).ToList();
                foreach (var item in groupBy)
                {
                    NvGiaoDichQuayVm.ObjectReportLevel2 obj = new NvGiaoDichQuayVm.ObjectReportLevel2();
                    obj.Ma = item.Key;
                    List<NvGiaoDichQuayVm.ObjectReport> lst = data.Where(x => x.MaDonVi == obj.Ma).ToList();
                    var groupByDkLoc = lst.GroupBy(x => new { x.Ma, x.Ten, x.MaDonVi })
                                            .Select(group => new NvGiaoDichQuayVm.ObjectReport()
                                            {
                                                Ma = group.Key.Ma,
                                                Ten = group.Key.Ten,
                                                MaDonVi = group.Key.MaDonVi,
                                                SoLuongBan = group.Sum(a => a.SoLuongBan),
                                                VonChuaVat = group.Sum(a => a.VonChuaVat),
                                                Von = group.Sum(a => a.Von),
                                                TienThue = group.Sum(a => a.TienThue),
                                                DoanhThu = group.Sum(a => a.DoanhThu),
                                                TienBan = group.Sum(a => a.TienBan),
                                                TienKhuyenMai = group.Sum(a => a.TienKhuyenMai),
                                                LaiBanLe = group.Sum(a => a.LaiBanLe),
                                            }).ToList();
                    obj.DataDetails.AddRange(groupByDkLoc);
                    result.Add(obj);
                }
            }
            return result;
        }
        #endregion  
        #region Lastest Version - Dieu Chuyen Xuat
        public List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportDieuChuyenXuat(ParameterXuatBan pi)
        {
            List<NvGiaoDichQuayVm.ObjectReportLevel2> result = new List<NvGiaoDichQuayVm.ObjectReportLevel2>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);
            switch (pi.RouteType)
            {
                case TypeReasonDieuChuyen.XUATCHUYENKHO:
                    switch (pi.GroupBy)
                    {
                        case TypeGroupXuatBan.MALOAIVATTU:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MALOAIVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHOMVATTU:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MANHOMVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHACUNGCAP:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHACHHANG:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAKHACHHANG.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHO:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAKHO.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAGIAODICH:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAGIAODICH.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MALOAITHUE:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MALOAITHUE.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MADONVINHAN:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MADONVI.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
                default:
                    switch (pi.GroupBy)
                    {
                        case TypeGroupXuatBan.MALOAIVATTU:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MALOAIVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHOMVATTU:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MANHOMVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHACUNGCAP:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHACHHANG:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAKHACHHANG.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHO:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAKHO.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAGIAODICH:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAGIAODICH.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MALOAITHUE:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MALOAITHUE.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MADONVINHAN:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MADONVI.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
            }
            if (data.Count > 0)
            {
                var groupBy = data.GroupBy(x => x.MaDonVi).ToList();
                foreach (var item in groupBy)
                {
                    NvGiaoDichQuayVm.ObjectReportLevel2 obj = new NvGiaoDichQuayVm.ObjectReportLevel2();
                    obj.Ma = item.Key;
                    List<NvGiaoDichQuayVm.ObjectReport> lst = data.Where(x => x.MaDonVi == obj.Ma).ToList();
                    var groupByDkLoc = lst.GroupBy(x => new { x.Ma, x.Ten, x.MaDonVi })
                                            .Select(group => new NvGiaoDichQuayVm.ObjectReport()
                                            {
                                                Ma = group.Key.Ma,
                                                Ten = group.Key.Ten,
                                                MaDonVi = group.Key.MaDonVi,
                                                SoLuongBan = group.Sum(a => a.SoLuongBan),
                                                VonChuaVat = group.Sum(a => a.VonChuaVat),
                                                Von = group.Sum(a => a.Von),
                                                TienThue = group.Sum(a => a.TienThue),
                                                DoanhThu = group.Sum(a => a.DoanhThu),
                                                TienBan = group.Sum(a => a.TienBan),
                                                TienKhuyenMai = group.Sum(a => a.TienKhuyenMai),
                                                LaiBanLe = group.Sum(a => a.LaiBanLe),
                                            }).ToList();
                    obj.DataDetails.AddRange(groupByDkLoc);
                    result.Add(obj);
                }
            }
            return result;
        }
        public MemoryStream ExportExcelDCXTongHop(ParameterXuatBan pi)
        {
            List<NvGiaoDichQuayVm.ObjectReport> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReport>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();

            var titleCotName = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);
            switch (pi.RouteType)
            {
                case TypeReasonDieuChuyen.XUATCHUYENKHO:
                    switch (pi.GroupBy)
                    {
                        case TypeGroupXuatBan.MALOAIVATTU:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MALOAIVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHOMVATTU:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MANHOMVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHACUNGCAP:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHACHHANG:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAKHACHHANG.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHO:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAKHO.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAGIAODICH:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAGIAODICH.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MALOAITHUE:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MALOAITHUE.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MADONVINHAN:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MADONVI.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
                default:
                    switch (pi.GroupBy)
                    {
                        case TypeGroupXuatBan.MALOAIVATTU:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MALOAIVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHOMVATTU:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MANHOMVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHACUNGCAP:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHACHHANG:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAKHACHHANG.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHO:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAKHO.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAGIAODICH:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAGIAODICH.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MALOAITHUE:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MALOAITHUE.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MADONVINHAN:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MADONVI.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCXTongHop(ky, InventoryGroupBy.MAVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
            }

            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MALOAIVATTU:
                    titleCotName = "Loại hàng";
                    break;
                case TypeGroupXuatBan.MAKHO:
                    titleCotName = "Kho hàng";
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    titleCotName = "Nhóm hàng";
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    titleCotName = "Nhà cung cấp";
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    titleCotName = "Giao dịch";
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    titleCotName = "Đơn vị";
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    titleCotName = "Khách hàng";
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    titleCotName = "Loại thuế";
                    break;
                default:
                    titleCotName = "Mặt hàng";
                    break;
            }
            if (data != null)
            {
                itemCollectionGroup.AddRange(data.ToList());
            }

            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 5;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                switch (pi.RouteType)
                {
                    case TypeReasonDieuChuyen.XUATCHUYENKHO:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ XUẤT CHUYỂN KHO"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        break;
                    default:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ XUẤT SIÊU THỊ THÀNH VIÊN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        break;
                }
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: " + titleCotName;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Danh sách"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tiền CK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tổng tiền"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in itemCollectionGroup)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuongBan;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.DoanhThu; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.TienKhuyenMai; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.TienThue; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.TienBan; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.SoLuongBan = itemTotal.SoLuongBan + item.SoLuongBan;
                    itemTotal.DoanhThu = itemTotal.DoanhThu + item.DoanhThu;
                    itemTotal.TienThue = itemTotal.TienThue + item.TienThue;
                    itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + item.TienKhuyenMai;
                    itemTotal.TienBan = itemTotal.TienBan + item.TienBan;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuongBan;
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelDCXDetail(ParameterXuatBan pi)
        {
            var itemCollection = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCha> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha>();
            var titleCotName = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);

            switch (pi.RouteType)
            {
                case TypeReasonDieuChuyen.XUATCHUYENKHO:
                    switch (pi.GroupBy)
                    {
                        case TypeGroupXuatBan.MADONVIXUAT:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MADONVIXUAT.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MALOAIVATTU:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MALOAIVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHOMVATTU:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MANHOMVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHACUNGCAP:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHACHHANG:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MAKHACHHANG.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHO:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MAKHO.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAGIAODICH:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MAGIAODICH.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MALOAITHUE:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MALOAITHUE.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MADONVINHAN:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MADONVI.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MAVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') = NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
                default:
                    switch (pi.GroupBy)
                    {
                        case TypeGroupXuatBan.MADONVIXUAT:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MADONVIXUAT.ToString(), pi.ReasonType, pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MALOAIVATTU:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MALOAIVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHOMVATTU:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MANHOMVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MANHACUNGCAP:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MANHACUNGCAP.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHACHHANG:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MAKHACHHANG.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAKHO:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MAKHO.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MAGIAODICH:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MAGIAODICH.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MALOAITHUE:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MALOAITHUE.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case TypeGroupXuatBan.MADONVINHAN:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MADONVI.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCXChiTiet(ky, InventoryGroupBy.MAVATTU.ToString(), "AND NVL(t.MADONVINHAN,'') <> NVL(t.UNITCODE,'')", pi.UnitUserCodes, pi.TaxCodes, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.CustomerCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
            }
            switch (pi.GroupBy)
            {
                case TypeGroupXuatBan.MALOAIVATTU:
                    titleCotName = "Loại hàng";
                    break;
                case TypeGroupXuatBan.MAKHO:
                    titleCotName = "Kho hàng";
                    break;
                case TypeGroupXuatBan.MANHOMVATTU:
                    titleCotName = "Nhóm hàng";
                    break;
                case TypeGroupXuatBan.MANHACUNGCAP:
                    titleCotName = "Nhà cung cấp";
                    break;
                case TypeGroupXuatBan.MAGIAODICH:
                    titleCotName = "Giao dịch";
                    break;
                case TypeGroupXuatBan.MADONVINHAN:
                    titleCotName = "Đơn vị";
                    break;
                case TypeGroupXuatBan.MAKHACHHANG:
                    titleCotName = "Khách hàng";
                    break;
                case TypeGroupXuatBan.MALOAITHUE:
                    titleCotName = "Loại thuế";
                    break;
                default:
                    titleCotName = "Mặt hàng";
                    break;
            }
            if (data != null)
            {
                itemCollectionGroup.AddRange(data.ToList());
            }

            using (ExcelPackage package = new ExcelPackage())
            {

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 5;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 10].Merge = true;
                switch (pi.RouteType)
                {
                    case TypeReasonDieuChuyen.XUATCHUYENKHO:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ XUẤT CHUYỂN KHO"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        break;
                    default:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ XUẤT SIÊU THỊ THÀNH VIÊN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        break;
                }
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + titleCotName;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Barcode"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Mã"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Danh sách"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Ngày giao dịch"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Số lượng"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Vốn chưa VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tiền hàng"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "Tiền CK"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Tiền VAT"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Value = "Tổng tiền"; worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;

                foreach (var item in itemCollectionGroup)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 11].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Ma + " - " + item.Ten;
                    currentRow++;
                    foreach (var itemdetails in item.DataDetails)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetails.Barcode;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetails.Ma;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetails.Ten;
                        worksheet.Cells[currentRow, startColumn + 4].Value = string.Format("{0}/{1}/{2}", itemdetails.NgayGiaoDich.Day, itemdetails.NgayGiaoDich.Month, itemdetails.NgayGiaoDich.Year);
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetails.SoLuongBan;
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetails.Von; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetails.DoanhThu; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetails.TienKhuyenMai; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetails.TienThue; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetails.TienBan; worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        itemTotal.Von = itemTotal.Von + itemdetails.Von;
                        itemTotal.SoLuongBan = itemTotal.SoLuongBan + itemdetails.SoLuongBan;
                        itemTotal.DoanhThu = itemTotal.DoanhThu + itemdetails.DoanhThu;
                        itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + itemdetails.TienKhuyenMai;
                        itemTotal.TienThue = itemTotal.TienThue + itemdetails.TienThue;
                        itemTotal.TienBan = itemTotal.TienBan + itemdetails.TienBan;
                        currentRow++;
                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 4].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.SoLuongBan;
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.Von; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        #endregion  
    }
}
