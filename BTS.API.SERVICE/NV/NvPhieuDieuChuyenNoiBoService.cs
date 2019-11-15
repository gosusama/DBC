using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Data.Entity;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;

namespace BTS.API.SERVICE.NV
{
    public interface INvPhieuDieuChuyenNoiBoService : IDataInfoService<NvVatTuChungTu>
    {
        NvPhieuDieuChuyenNoiBoVm.Dto CreateNewInstance(string maChungTu);
        NvPhieuDieuChuyenNoiBoVm.Dto CreateNewInstance();
        NvPhieuDieuChuyenNoiBoVm.Dto CreateNewReciveInstance();
        NvVatTuChungTu InsertPhieu(NvPhieuDieuChuyenNoiBoVm.Dto instance);
        NvVatTuChungTu InsertPhieuNhan(NvPhieuDieuChuyenNoiBoVm.Dto instance);
        NvPhieuDieuChuyenNoiBoVm.ReportModel CreateReport(string id);
        NvPhieuDieuChuyenNoiBoVm.ReportModel CreateReportReceive(string id);
        NvVatTuChungTu UpdatePhieu(NvPhieuDieuChuyenNoiBoVm.Dto instance);
        NvPhieuDieuChuyenNoiBoVm.Dto MapDtoRecieve(NvVatTuChungTu instance, NvPhieuDieuChuyenNoiBoVm.Dto dto);
        MemoryStream ExportExcel(List<NvPhieuDieuChuyenNoiBoVm.Dto> data, FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter, TypeVoucher type);
        MemoryStream ExportExcelByNhaCungCap(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter);
        MemoryStream ExportExcelByMerchandise(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter);
        MemoryStream ExportExcelByMerchandiseType(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter);
        MemoryStream ExportExcelByMerchandiseGroup(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter);
        MemoryStream ExportExcelByNhaCungCapReceive(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter);
        MemoryStream ExportExcelByMerchandiseReceive(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter);
        MemoryStream ExportExcelByMerchandiseTypeReceive(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter);
        MemoryStream ExportExcelByMerchandiseGroupReceive(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter);
        bool DeletePhieu(string id);
        StateProcessApproval Approval(string id, string type);
        bool CheckConnectServer(string unitCode);
        string GetStringConnectByUniCode(string unitCode);
        bool InsertVoucherToUnitCode(NvVatTuChungTu instance);
    }
    public class NvPhieuDieuChuyenNoiBoService : DataInfoServiceBase<NvVatTuChungTu>, INvPhieuDieuChuyenNoiBoService
    {
        public NvPhieuDieuChuyenNoiBoService(IUnitOfWork unitOfWork)
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
        public NvPhieuDieuChuyenNoiBoVm.Dto CreateNewInstance(string maChungTu)
        {
            var unitCode = GetCurrentUnitCode();
            NvPhieuDieuChuyenNoiBoVm.Dto result = new NvPhieuDieuChuyenNoiBoVm.Dto();
            NvVatTuChungTu phieuNhap = Repository.DbSet.FirstOrDefault(x => x.MaChungTu == maChungTu);
            if (phieuNhap != null)
            {
                var chiTiet = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieuNhap.MaChungTuPk);
                result = Mapper.Map<NvVatTuChungTu, NvPhieuDieuChuyenNoiBoVm.Dto>(phieuNhap);
                result.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvPhieuDieuChuyenNoiBoVm.DtoDetail>>(chiTiet.ToList());
                result.LoaiPhieu = TypeVoucher.DCX.ToString();
                result.MaChungTu = BuildCode_PTNX(TypeVoucher.DCX.ToString(), unitCode, false);
                result.NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode);
                result.NgayDieuDong = DateTime.Now;
                result.MaKhoNhap = "";
                result.MaKhoXuat = "";
            }
            return result;
        }
        public NvPhieuDieuChuyenNoiBoVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            return new NvPhieuDieuChuyenNoiBoVm.Dto()
            {
                LoaiPhieu = TypeVoucher.DCX.ToString(),
                MaChungTu = BuildCode_PTNX(TypeVoucher.DCX.ToString(), unitCode, false),
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
                NgayDieuDong = DateTime.Now
            };
        }
        public NvPhieuDieuChuyenNoiBoVm.Dto CreateNewReciveInstance()
        {
            var unitCode = GetCurrentUnitCode();
            return new NvPhieuDieuChuyenNoiBoVm.Dto()
            {
                LoaiPhieu = TypeVoucher.DCN.ToString(),
                MaChungTu = BuildCode_PTNX(TypeVoucher.DCN.ToString(), unitCode, false),
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
                NgayDieuDong = DateTime.Now
            };
        }
        public NvVatTuChungTu InsertPhieu(NvPhieuDieuChuyenNoiBoVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var item = AutoMapper.Mapper.Map<NvPhieuDieuChuyenNoiBoVm.Dto, NvVatTuChungTu>(instance);
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            string _unitCode = GetCurrentUnitCode();
            result.MaChungTu = BuildCode_PTNX(TypeVoucher.DCX.ToString(), _unitCode, true);
            result.MaDonViXuat = GetCurrentUnitCode();
            item.GenerateMaChungTuPk();
            result = Insert(result);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var dataDetails = Mapper.Map<List<NvPhieuDieuChuyenNoiBoVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            dataDetails.ForEach(x =>
            {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTu = result.MaChungTu;
                x.MaChungTuPk = result.MaChungTuPk;
                x.SoLuongCT = x.SoLuong;
                x.SoLuongBaoCT = x.SoLuongBao;
                x.VAT = hang != null ? hang.MaVatVao : "";
                x.GiaMuaCoVat = x.DonGia * (1 + (hang.TyLeVatVao / 100));
            });

            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(dataDetails);
            return result;
        }
        public NvVatTuChungTu UpdatePhieu(NvPhieuDieuChuyenNoiBoVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvPhieuDieuChuyenNoiBoVm.Dto, NvVatTuChungTu>(instance);
            var detailData = Mapper.Map<List<NvPhieuDieuChuyenNoiBoVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
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
                x.VAT = hang != null ? hang.MaVatVao : "";
                x.GiaMuaCoVat = x.DonGia * (1 + (hang.TyLeVatVao / 100));
            });
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            var result = Update(masterData);
            return result;
        }
        public NvPhieuDieuChuyenNoiBoVm.ReportModel CreateReport(string id)
        {
            NvPhieuDieuChuyenNoiBoVm.ReportModel result = new NvPhieuDieuChuyenNoiBoVm.ReportModel();
            NvVatTuChungTu exsitItem = FindById(id);
            if (exsitItem != null)
            {
                result = Mapper.Map<NvVatTuChungTu, NvPhieuDieuChuyenNoiBoVm.ReportModel>(exsitItem);
                AU_NGUOIDUNG nhanVien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == exsitItem.ICreateBy).FirstOrDefault();
                List<NvVatTuChungTuChiTiet> chiTietPhieu = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsitItem.MaChungTuPk).ToList();
                result.DataReportDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvPhieuDieuChuyenNoiBoVm.ReportDetailModel>>(chiTietPhieu);
                if (nhanVien != null)
                {
                    result.NameNhanVienCreate = nhanVien.TenNhanVien != null ? nhanVien.TenNhanVien : "";
                }
                string unitcode = GetCurrentUnitCode();
                foreach (NvPhieuDieuChuyenNoiBoVm.ReportDetailModel ct in result.DataReportDetails)
                {
                    if (ct.Barcode != null)
                    {
                        string[] str = ct.Barcode.Split(';');
                        if (str.Length > 6)
                        {
                            ct.Barcode = string.Format(";{0};{1};{2};{3};{4};", str[0], str[1], str[2], str[3], str[4]);
                        }
                    }
                    MdMerchandisePrice chiTietGia = UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu == ct.MaHang && x.MaDonVi == unitcode).FirstOrDefault();
                    if (chiTietGia != null)
                    {
                        ct.GiaBanBuonVat = chiTietGia.GiaBanBuonVat;
                        ct.GiaBanLeVat = chiTietGia.GiaBanLeVat;
                    }
                }
                string unitCode = GetCurrentUnitCode();
                DbSet<MdCustomer> khachHang = UnitOfWork.Repository<MdCustomer>().DbSet;
                MdCustomer KH = khachHang.FirstOrDefault(x => x.MaKH == exsitItem.MaKhachHang);
                DbSet<MdWareHouse> warehouses = UnitOfWork.Repository<MdWareHouse>().DbSet;
                MdWareHouse importWareHouse = warehouses.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);
                MdWareHouse exportWareHouse = warehouses.FirstOrDefault(x => x.MaKho == result.MaKhoXuat);
                result.TenKhoXuat = exportWareHouse != null ? exportWareHouse.TenKho : "";
                result.TenKhoNhap = importWareHouse != null ? importWareHouse.TenKho : "";
                result.DiaChi = exportWareHouse != null ? exportWareHouse.DiaChi : "";
                result.TenDonVi = CurrentSetting.GetUnitName(unitCode);
                result.DiaChiDonVi = CurrentSetting.GetUnitAddress(unitCode);
                result.TenDonViNhan = UnitOfWork.Repository<AU_DONVI>().DbSet.Where(x => x.MaDonVi == result.MaDonViNhan).Select(x => x.TenDonVi).FirstOrDefault().ToString();
                //GetNhanVien
            }
            DateTime createDate = DateTime.Now;
            result.CreateDay = createDate.Day;
            result.CreateMonth = createDate.Month;
            result.CreateYear = createDate.Year;
            return result;
        }
        public NvPhieuDieuChuyenNoiBoVm.ReportModel CreateReportReceive(string id)
        {
            NvPhieuDieuChuyenNoiBoVm.ReportModel result = new NvPhieuDieuChuyenNoiBoVm.ReportModel();
            NvVatTuChungTu exsitItem = FindById(id);
            if (exsitItem != null)
            {
                result = Mapper.Map<NvVatTuChungTu, NvPhieuDieuChuyenNoiBoVm.ReportModel>(exsitItem);
                AU_NGUOIDUNG userName = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == exsitItem.ICreateBy).FirstOrDefault();
                List<NvVatTuChungTuChiTiet> chiTietPhieu = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsitItem.MaChungTuPk).ToList();
                result.DataReportDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvPhieuDieuChuyenNoiBoVm.ReportDetailModel>>(chiTietPhieu);
                if (userName != null)
                {
                    result.NameNhanVienCreate = userName.TenNhanVien != null ? userName.TenNhanVien : "";
                }
                string unitcode = GetCurrentUnitCode();
                foreach (NvPhieuDieuChuyenNoiBoVm.ReportDetailModel ct in result.DataReportDetails)
                {
                    if (ct.Barcode != null)
                    {
                        string[] str = ct.Barcode.Split(';');
                        if (str.Length > 6)
                        {
                            ct.Barcode = string.Format(";{0};{1};{2};{3};{4};", str[0], str[1], str[2], str[3], str[4]);
                        }
                    }
                    MdMerchandisePrice chiTietGia = UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu == ct.MaHang && x.MaDonVi == unitcode).FirstOrDefault();
                    if (chiTietGia != null)
                    {
                        ct.GiaBanBuonVat = chiTietGia.GiaBanBuonVat;
                        ct.GiaBanLeVat = chiTietGia.GiaBanLeVat;
                    }
                    var price = UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu == ct.MaHang).FirstOrDefault();
                    if (price != null) ct.GiaBanLeVat = price.GiaBanLeVat;
                }
                string unitCode = GetCurrentUnitCode();
                DbSet<MdCustomer> khachHang = UnitOfWork.Repository<MdCustomer>().DbSet;
                MdCustomer KH = khachHang.FirstOrDefault(x => x.MaKH == exsitItem.MaKhachHang);
                DbSet<MdWareHouse> warehouses = UnitOfWork.Repository<MdWareHouse>().DbSet;
                MdWareHouse importWareHouse = warehouses.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);
                MdWareHouse exportWareHouse = warehouses.FirstOrDefault(x => x.MaKho == result.MaKhoXuat);
                result.TenKhoXuat = exportWareHouse != null ? exportWareHouse.TenKho : "";
                result.TenKhoNhap = importWareHouse != null ? importWareHouse.TenKho : "";
                result.DiaChi = exportWareHouse != null ? exportWareHouse.DiaChi : "";
                result.TenDonViXuat = UnitOfWork.Repository<AU_DONVI>().DbSet.Where(x => x.MaDonVi == result.MaDonViXuat).Select(x => x.TenDonVi).FirstOrDefault().ToString();
                result.TenDonVi = CurrentSetting.GetUnitName(unitCode);
                result.DiaChiDonVi = CurrentSetting.GetUnitAddress(unitCode);
                //GetNhanVien
                if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
                {
                    ClaimsPrincipal currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                    string name = currentUser.Identity.Name;
                    AU_NGUOIDUNG nhanVien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                    if (nhanVien != null)
                    {
                        result.Username = nhanVien.TenNhanVien;
                    }
                    else
                    {
                        result.Username = "Administrator";
                    }
                }
            }
            DateTime createDate = DateTime.Now;
            result.CreateDay = createDate.Day;
            result.CreateMonth = createDate.Month;
            result.CreateYear = createDate.Year;
            return result;
        }
        protected override Expression<Func<NvVatTuChungTu, bool>> GetKeyFilter(NvVatTuChungTu instance)
        {
            return x => x.MaChungTuPk == instance.MaChungTuPk;
        }
        public NvVatTuChungTu InsertPhieuNhan(NvPhieuDieuChuyenNoiBoVm.Dto instance)
        {
            var item = Mapper.Map<NvPhieuDieuChuyenNoiBoVm.Dto, NvVatTuChungTu>(instance);
            var unitCode = GetCurrentUnitCode();
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            result.LenhDieuDong = instance.MaChungTu;
            result.LoaiPhieu = TypeVoucher.DCN.ToString();
            string _unitCode = GetCurrentUnitCode();
            result.MaChungTu = BuildCode_PTNX(TypeVoucher.DCN.ToString(), _unitCode, true);
            result.MaDonViNhan = unitCode;
            item.GenerateMaChungTuPk();
            result = Insert(result);
            var dataDetails = Mapper.Map<List<NvPhieuDieuChuyenNoiBoVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            dataDetails.ForEach(x =>
            {
                var VATTU = UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(a => a.MaVatTu == x.MaHang && a.UnitCode == unitCode);
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTu = result.MaChungTu;
                x.MaChungTuPk = result.MaChungTuPk;
                x.SoLuongBaoCT = x.SoLuongBao;
                x.SoLuongCT = x.SoLuong;
                x.SoLuongLeCT = x.SoLuongLe;
                x.VAT = VATTU != null ? VATTU.MaVatVao : "";
                x.GiaMuaCoVat = x.DonGia * (1 + (VATTU.TyLeVatVao / 100));
            });
            var exsitItem = FindById(instance.Id);
            if (exsitItem != null)
            {
                exsitItem.TrangThai = 10; //Đã duyệt
                exsitItem.ObjectState = ObjectState.Modified;
            }
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(dataDetails);
            return result;
        }
        public StateProcessApproval Approval(string id, string type)
        {
            StateProcessApproval result;
            var unitCode = GetCurrentUnitCode();
            var periods = CurrentSetting.GetKhoaSo(unitCode);
            if (periods != null)
            {
                var tableName = ProcedureCollection.GetTableName(periods.Year, periods.Period);
                Func<string, int, int, string, bool> func = null;
                if (type == TypeVoucher.DCX.ToString())
                {
                    func = ProcedureCollection.DecreaseVoucher;
                }
                else
                {
                    func = ProcedureCollection.IncreaseVoucher;
                }
                if (func(tableName, periods.Year, periods.Period, id))
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

        public NvPhieuDieuChuyenNoiBoVm.Dto MapDtoRecieve(NvVatTuChungTu instance, NvPhieuDieuChuyenNoiBoVm.Dto dto)
        {
            var unitCode = GetCurrentUnitCode();
            var result = Mapper.Map<NvVatTuChungTu, NvPhieuDieuChuyenNoiBoVm.Dto>(instance);
            result.MaHoaDon = instance.MaChungTu;
            var detailData = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == dto.MaChungTuPk);
            if (detailData != null && detailData.Count() > 0)
            {
                result.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvPhieuDieuChuyenNoiBoVm.DtoDetail>>(detailData.ToList());
                foreach (var item in dto.DataDetails)
                {
                    var VATTU = UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(a => a.MaVatTu == item.MaHang && a.UnitCode == unitCode);
                    var exsit = result.DataDetails.FirstOrDefault(x => x.Id == item.Id);
                    if (exsit != null)
                    {
                        exsit.SoLuong = item.SoLuong;
                        exsit.SoLuongBao = item.SoLuongBao;
                        exsit.ThanhTien = item.SoLuong * item.DonGia;
                        exsit.Vat = VATTU != null ? VATTU.MaVatVao : "";
                    }
                }
            }
            result.TrangThai = 0;
            return result;
        }

        public MemoryStream ExportExcel(List<NvPhieuDieuChuyenNoiBoVm.Dto> data, FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter, TypeVoucher type)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                var loaiPhieu = type == TypeVoucher.DCX ? "XUẤT" : "NHẬP";
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 4;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 8].Merge = true;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU " + loaiPhieu + " ĐIỀU CHUYỂN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                     string.Format("Từ ngày: {0} Đến ngày: {1}",
                     filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                     filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[3, 1].Value = "STT"; worksheet.Cells[3, 1].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 2].Value = "Ngày"; worksheet.Cells[3, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 3].Value = "Đơn vị xuất"; worksheet.Cells[3, 3].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 4].Value = "Đơn vị nhận"; worksheet.Cells[3, 4].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 5].Value = "Kho xuất"; worksheet.Cells[3, 5].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 6].Value = "Kho nhận"; worksheet.Cells[3, 6].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 7].Value = "Tổng tiền"; worksheet.Cells[3, 7].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 8].Value = "Trạng thái"; worksheet.Cells[3, 8].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                decimal itemTotal = 0;
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in data)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.NgayCT.HasValue ? item.NgayCT.Value.ToString("dd/MM/yyyy") : "";
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.MaDonViXuat;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.MaDonViNhan;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.MaKhoXuat;
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.MaKhoNhap;
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.ThanhTienSauVat; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value =
                        item.TrangThai == 10 ? "Hoàn thành" : "Chưa duyệt";
                    worksheet.Cells[currentRow, startColumn + 8].Style.Font.Color.SetColor(item.TrangThai == 10 ? Color.DarkBlue : Color.Black);
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 8].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal += item.ThanhTienSauVat;
                    currentRow = currentRow + 1;
                }
                worksheet.Cells[currentRow, 1, currentRow, 6].Merge = true;
                worksheet.Cells[currentRow, 1].Value = "TỔNG"; worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByMerchandise(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter)
        {
            var itemCollection = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> itemCollectionGroup = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuDieuChuyenGroupByMerchandise(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU ĐIỀU CHUYỂN XUẤT HÀNG"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                var itemTotal = new NvPhieuDieuChuyenNoiBoVm.ObjectReport();
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
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByNhaCungCap(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter)
        {
            var itemCollection = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> itemCollectionGroup = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuDieuChuyenGroupByNhaCungCap(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU ĐIỀU CHUYỂN XUẤT HÀNG"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                var itemTotal = new NvPhieuDieuChuyenNoiBoVm.ObjectReport();
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
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByMerchandiseType(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter)
        {
            var itemCollection = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> itemCollectionGroup = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuDieuChuyenGroupByMerchandiseType(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU ĐIỀU CHUYỂN XUẤT HÀNG"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}",
                    filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                    filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Loại vật tư";
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã loại"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên loại"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvPhieuDieuChuyenNoiBoVm.ObjectReport();
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
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByMerchandiseGroup(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter)
        {
            var itemCollection = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> itemCollectionGroup = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuDieuChuyenGroupByMerchandiseGroup(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU ĐIỀU CHUYỂN XUẤT HÀNG"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}",
                    filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                    filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Nhóm vật tư";
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã nhóm"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên nhóm"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvPhieuDieuChuyenNoiBoVm.ObjectReport();
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
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByMerchandiseReceive(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter)
        {
            var itemCollection = new List<NvNhapHangMuaVm.ObjectReport>();
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> itemCollectionGroup = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuDieuChuyenNhanGroupByMerchandise(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU ĐIỀU CHUYỂN NHẬP HÀNG"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                var itemTotal = new NvPhieuDieuChuyenNoiBoVm.ObjectReport();
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
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByNhaCungCapReceive(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter)
        {
            var itemCollection = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> itemCollectionGroup = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuDieuChuyenNhanGroupByNhaCungCap(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU ĐIỀU CHUYỂN NHẬP HÀNG"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                var itemTotal = new NvPhieuDieuChuyenNoiBoVm.ObjectReport();
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
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByMerchandiseTypeReceive(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter)
        {
            var itemCollection = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> itemCollectionGroup = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuDieuChuyenNhanGroupByMerchandiseType(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU ĐIỀU CHUYỂN NHẬP HÀNG"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}",
                    filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                    filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Loại vật tư";
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã loại"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên loại"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvPhieuDieuChuyenNoiBoVm.ObjectReport();
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
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelByMerchandiseGroupReceive(FilterObj<NvPhieuDieuChuyenNoiBoVm.Search> filter)
        {
            var itemCollection = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> itemCollectionGroup = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuDieuChuyenNhanGroupByMerchandiseGroup(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU ĐIỀU CHUYỂN NHẬP HÀNG"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}",
                    filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                    filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Nhóm vật tư";
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã nhóm"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên nhóm"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvPhieuDieuChuyenNoiBoVm.ObjectReport();
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
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);


                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public string GetStringConnectByUniCode(string unitCode)
        {
            string connectString = null;
            if (!string.IsNullOrEmpty(unitCode) && unitCode != "")
            {
                switch (unitCode.ToUpper())
                {
                    case "DV1-CH1":
                        {
                            connectString = ConfigurationManager.ConnectionStrings["TUSON.Connection"].ConnectionString.ToString();
                            break;
                        }
                    case "DV1-CH2":
                        {
                            connectString = ConfigurationManager.ConnectionStrings["LTT.Connection"].ConnectionString.ToString();
                            break;
                        }
                    case "DV1-CH4":
                        {
                            connectString = ConfigurationManager.ConnectionStrings["QUEVO.Connection"].ConnectionString.ToString();
                            break;
                        }
                    case "DV1-CH5":
                        {
                            connectString = ConfigurationManager.ConnectionStrings["LACVE.Connection"].ConnectionString.ToString();
                            break;
                        }
                    case "DV1-CH6":
                        {
                            connectString = ConfigurationManager.ConnectionStrings["GIABINH.Connection"].ConnectionString.ToString();
                            break;
                        }
                }
            }
            return connectString;
        }

        public bool CheckConnectServer(string unitCode)
        {
            bool result = false;
            string connectString = "";
            if (!string.IsNullOrEmpty(unitCode) && unitCode != "")
            {
                connectString = GetStringConnectByUniCode(unitCode);
                if (!string.IsNullOrEmpty(connectString))
                {
                    
                    using (OracleConnection connection = new OracleConnection(connectString))
                    {
                        try
                        {
                            connection.Open();
                            if (connection.State == ConnectionState.Open)
                            {
                                result = true;
                            }
                        }
                        catch
                        {
                            result = false;
                        }
                        finally
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool InsertVoucherToUnitCode(NvVatTuChungTu instance)
        {
            bool result = false;
            string connectString = null;
            int countGiaBan = 0;
            string currentUnitCode = GetCurrentUnitCode();
            if (!string.IsNullOrEmpty(instance.MaDonViNhan))
            {
                connectString = GetStringConnectByUniCode(instance.MaDonViNhan);
                if (!string.IsNullOrEmpty(connectString))
                {
                    using (OracleConnection connection = new OracleConnection(connectString))
                    {
                        try
                        {
                            connection.Open();
                            if (connection.State == ConnectionState.Open)
                            {
                                //Tạo lại mã chứng từ theo tự sinh của đơn vị
                                List<NvVatTuChungTuChiTiet> chungTuChiTiet = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk.Equals(instance.MaChungTuPk) && x.MaChungTu.Equals(instance.MaChungTu)).ToList();
                                if (chungTuChiTiet.Count > 0)
                                {
                                    instance.ICreateBy = currentUnitCode + "." + instance.ICreateBy;
                                    instance.ICreateDate = DateTime.Now;
                                    instance.IState = "DONGBO";
                                    instance.LoaiPhieu = TypeVoucher.DCN.ToString();
                                    instance.TrangThai = 0;
                                    instance.NgayDuyetPhieu = null;
                                    instance.SoHd = currentUnitCode + "." + instance.MaChungTu;
                                    instance.LenhDieuDong = currentUnitCode + "." + instance.MaChungTu;
                                    instance.MaChungTu = SaveCodeRoot(connectString, instance.MaDonViNhan);
                                    instance.MaChungTuPk = instance.MaDonViNhan + "." + instance.MaChungTu;
                                    instance.MaDonViNhan = instance.MaDonViNhan;
                                    instance.MaDonViXuat = currentUnitCode;
                                    instance.UnitCode = instance.MaDonViNhan;
                                    
                                    // đồng bộ bảng NvVatTuChungTu
                                    OracleCommand cmd = new OracleCommand();
                                    cmd.Connection = connection;
                                    cmd.InitialLONGFetchSize = 1000;
                                    cmd.CommandText = "INSERT INTO VATTUCHUNGTU(ID,LOAICHUNGTU,MACHUNGTUPK,MACHUNGTU,NGAYCHUNGTU,NGAYDUYETPHIEU,MAKHOXUAT,MAKHONHAP,SOHOADON,MADONVINHAN,MADONVIXUAT,NOIDUNG,LENHDIEUDONG,THANHTIENTRUOCVAT,TIENCHIETKHAU,TIENVAT,TONGTIENGIAMGIA,THANHTIENSAUVAT,VAT,NGAYDIEUDONG,TRANGTHAI,MANHANVIEN,I_CREATE_DATE,I_CREATE_BY,I_STATE,UNITCODE) VALUES (:ID,:LOAICHUNGTU,:MACHUNGTUPK,:MACHUNGTU,:NGAYCHUNGTU,:NGAYDUYETPHIEU,:MAKHOXUAT,:MAKHONHAP,:SOHOADON,:MADONVINHAN,:MADONVIXUAT,:NOIDUNG,:LENHDIEUDONG,:THANHTIENTRUOCVAT,:TIENCHIETKHAU,:TIENVAT,:TONGTIENGIAMGIA,:THANHTIENSAUVAT,:VAT,:NGAYDIEUDONG,:TRANGTHAI,:MANHANVIEN,:I_CREATE_DATE,:I_CREATE_BY,:I_STATE,:UNITCODE)";
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("LOAICHUNGTU", OracleDbType.NVarchar2, 50).Value = instance.LoaiPhieu;
                                    cmd.Parameters.Add("MACHUNGTUPK", OracleDbType.NVarchar2, 50).Value = instance.MaChungTuPk;
                                    cmd.Parameters.Add("MACHUNGTU", OracleDbType.NVarchar2, 50).Value = instance.MaChungTu;
                                    cmd.Parameters.Add("NGAYCHUNGTU", OracleDbType.Date).Value = instance.NgayCT;
                                    cmd.Parameters.Add("NGAYDUYETPHIEU", OracleDbType.Date).Value = instance.NgayDuyetPhieu;
                                    cmd.Parameters.Add("MAKHOXUAT", OracleDbType.NVarchar2, 50).Value = instance.MaKhoXuat;
                                    cmd.Parameters.Add("MAKHONHAP", OracleDbType.NVarchar2, 50).Value = instance.MaKhoNhap;
                                    cmd.Parameters.Add("SOHOADON", OracleDbType.NVarchar2, 50).Value = instance.SoHd;
                                    cmd.Parameters.Add("MADONVINHAN", OracleDbType.NVarchar2, 50).Value = instance.MaDonViNhan;
                                    cmd.Parameters.Add("MADONVIXUAT", OracleDbType.NVarchar2, 50).Value = instance.MaDonViXuat;
                                    cmd.Parameters.Add("NOIDUNG", OracleDbType.NVarchar2, 400).Value = instance.NoiDung;
                                    cmd.Parameters.Add("LENHDIEUDONG", OracleDbType.NVarchar2, 400).Value = instance.LenhDieuDong;
                                    cmd.Parameters.Add("THANHTIENTRUOCVAT", OracleDbType.Decimal).Value = instance.ThanhTienTruocVat;
                                    cmd.Parameters.Add("TIENCHIETKHAU", OracleDbType.Decimal).Value = instance.TienChietKhau;
                                    cmd.Parameters.Add("TIENVAT", OracleDbType.Decimal).Value = instance.TienVat;
                                    cmd.Parameters.Add("TONGTIENGIAMGIA", OracleDbType.Decimal).Value = instance.TongTienGiamGia;
                                    cmd.Parameters.Add("THANHTIENSAUVAT", OracleDbType.Decimal).Value = instance.ThanhTienSauVat;
                                    cmd.Parameters.Add("VAT", OracleDbType.NVarchar2, 50).Value = instance.VAT;
                                    cmd.Parameters.Add("NGAYDIEUDONG", OracleDbType.Date).Value = instance.NgayDieuDong;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = instance.TrangThai;
                                    cmd.Parameters.Add("MANHANVIEN", OracleDbType.NVarchar2, 50).Value = instance.ICreateBy;
                                    cmd.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = instance.ICreateDate;
                                    cmd.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = instance.ICreateBy;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = instance.IState;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = instance.UnitCode;
                                    
                                    OracleCommand cmdChiTiet = new OracleCommand();
                                    cmdChiTiet.Connection = connection;
                                    cmd.InitialLONGFetchSize = 1000;
                                    OracleTransaction transactionChiTiet;
                                    transactionChiTiet = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                    cmdChiTiet.Transaction = transactionChiTiet;
                                    foreach (NvVatTuChungTuChiTiet row in chungTuChiTiet)
                                    {
                                        // đồng bộ bảng NvVatTuChungTuChiTiet
                                        cmdChiTiet.CommandText = "INSERT INTO VATTUCHUNGTUCHITIET(ID,MACHUNGTU,MACHUNGTUPK,MAHANG,TENHANG,MAKHACHHANG,DONVITINH,BARCODE,LUONGQUYCACH,SOLUONGLE,SOLUONGLECT,SOLUONGBAOCT,SOLUONGCT,SOLUONGBAO,SOLUONG,DONGIA,THANHTIEN,TIENGIAMGIA,GIAVON,\"INDEX\",GIAMUACOVAT,VAT) VALUES (:ID,:MACHUNGTU,:MACHUNGTUPK,:MAHANG,:TENHANG,:MAKHACHHANG,:DONVITINH,:BARCODE,:LUONGQUYCACH,:SOLUONGLE,:SOLUONGLECT,:SOLUONGBAOCT,:SOLUONGCT,:SOLUONGBAO,:SOLUONG,:DONGIA,:THANHTIEN,:TIENGIAMGIA,:GIAVON,:\"INDEX\",:GIAMUACOVAT,:VAT)";
                                        cmdChiTiet.CommandType = CommandType.Text;
                                        cmdChiTiet.Parameters.Clear();
                                        cmdChiTiet.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                        cmdChiTiet.Parameters.Add("MACHUNGTU", OracleDbType.NVarchar2, 50).Value = instance.MaChungTu;
                                        cmdChiTiet.Parameters.Add("MACHUNGTUPK", OracleDbType.NVarchar2, 50).Value = instance.MaChungTuPk;
                                        cmdChiTiet.Parameters.Add("MAHANG", OracleDbType.NVarchar2, 50).Value = row.MaHang;
                                        cmdChiTiet.Parameters.Add("TENHANG", OracleDbType.NVarchar2, 300).Value = row.TenHang;
                                        cmdChiTiet.Parameters.Add("MAKHACHHANG", OracleDbType.NVarchar2, 50).Value = row.MaKhachHang;
                                        cmdChiTiet.Parameters.Add("DONVITINH", OracleDbType.NVarchar2, 50).Value = row.DonViTinh;
                                        cmdChiTiet.Parameters.Add("BARCODE", OracleDbType.NVarchar2, 2000).Value = row.Barcode;
                                        cmdChiTiet.Parameters.Add("LUONGQUYCACH", OracleDbType.Decimal).Value = row.LuongBao;
                                        cmdChiTiet.Parameters.Add("SOLUONGLE", OracleDbType.Decimal).Value = row.SoLuongLe;
                                        cmdChiTiet.Parameters.Add("SOLUONGLECT", OracleDbType.Decimal).Value = row.SoLuongLeCT;
                                        cmdChiTiet.Parameters.Add("SOLUONGBAOCT", OracleDbType.Decimal).Value = row.SoLuongBaoCT;
                                        cmdChiTiet.Parameters.Add("SOLUONGCT", OracleDbType.Decimal).Value = row.SoLuongCT;
                                        cmdChiTiet.Parameters.Add("SOLUONGBAO", OracleDbType.Decimal).Value = row.SoLuongBao;
                                        cmdChiTiet.Parameters.Add("SOLUONG", OracleDbType.Decimal).Value = row.SoLuong;
                                        cmdChiTiet.Parameters.Add("DONGIA", OracleDbType.Decimal).Value = row.DonGia;
                                        cmdChiTiet.Parameters.Add("THANHTIEN", OracleDbType.Decimal).Value = row.ThanhTien;
                                        cmdChiTiet.Parameters.Add("TIENGIAMGIA", OracleDbType.Decimal).Value = row.TienGiamGia;
                                        cmdChiTiet.Parameters.Add("GIAVON", OracleDbType.Decimal).Value = row.GiaVon;
                                        cmdChiTiet.Parameters.Add("\"INDEX\"", OracleDbType.Int32).Value = row.Index;
                                        cmdChiTiet.Parameters.Add("GIAMUACOVAT", OracleDbType.Decimal).Value = row.GiaMuaCoVat;
                                        cmdChiTiet.Parameters.Add("VAT", OracleDbType.NVarchar2, 50).Value = row.VAT;
                                        cmdChiTiet.ExecuteNonQuery();
                                        countGiaBan ++;
                                    }
                                    if(chungTuChiTiet.Count == countGiaBan)
                                    {
                                        transactionChiTiet.Commit();
                                        OracleTransaction transactionChungTu;
                                        try
                                        {
                                            transactionChungTu = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                            cmd.Transaction = transactionChungTu;
                                            int count = cmd.ExecuteNonQuery();
                                            transactionChungTu.Commit();
                                            if (count > 0 && countGiaBan > 0)
                                            {
                                                result = true;
                                            }
                                            else
                                            {
                                                transactionChungTu.Rollback();
                                                transactionChiTiet.Rollback();
                                                result = false;
                                            }
                                        }
                                        catch
                                        {
                                            result = false;
                                        }
                                        finally
                                        {
                                            connection.Close();
                                            connection.Dispose();
                                        }
                                    }
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            result = false;
                        }
                        finally
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Tạo mã phiếu nhận theo số tự tăng mã phiếu nhận tại đơn vị điều chuyển đến
        /// </summary>
        /// <param name="connectString"></param>
        /// <returns></returns>
        public string BuildCodeRoot(string connectString, string maDonViNhan)
        {
            var result = "";
            var type = TypeVoucher.DCN.ToString();
            using (OracleConnection connection = new OracleConnection(connectString))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = connection;
                        cmd.InitialLONGFetchSize = 1000;
                        cmd.CommandText = "SELECT ID,TYPE,CODE,\"CURRENT\",UNITCODE FROM MD_ID_BUILDER WHERE TYPE = '" + type + "' AND CODE = '" + type + "' AND UNITCODE = '" + maDonViNhan + "' AND ROWNUM = 1";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        MdIdBuilder config = new MdIdBuilder();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                config = new MdIdBuilder
                                {
                                    Id = dataReader["ID"].ToString(),
                                    Type = dataReader["TYPE"].ToString(),
                                    Code = dataReader["CODE"].ToString(),
                                    Current = dataReader["CURRENT"].ToString(),
                                    UnitCode = maDonViNhan,
                                };
                            }
                        }
                        else
                        {
                            config = new MdIdBuilder
                            {
                                Id = Guid.NewGuid().ToString(),
                                Type = type,
                                Code = type,
                                Current = "0000",
                                UnitCode = maDonViNhan,
                            };
                        }
                        var soMa = config.GenerateNumber();
                        config.Current = soMa;
                        result = string.Format("{0}{1}", config.Code, soMa);
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return result;
        }

        public string SaveCodeRoot(string connectString, string maDonViNhan)
        {
            var result = "";
            var type = TypeVoucher.DCN.ToString();
            using (OracleConnection connection = new OracleConnection(connectString))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = connection;
                        cmd.InitialLONGFetchSize = 1000;
                        cmd.CommandText = "SELECT ID,TYPE,CODE,\"CURRENT\",UNITCODE FROM MD_ID_BUILDER WHERE TYPE = '" + type + "' AND CODE = '" + type + "' AND UNITCODE = '" + maDonViNhan + "' AND ROWNUM = 1";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        MdIdBuilder config = new MdIdBuilder();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                config = new MdIdBuilder
                                {
                                    Id = dataReader["ID"].ToString(),
                                    Type = type,
                                    Code = dataReader["CODE"].ToString(),
                                    Current = dataReader["CURRENT"].ToString(),
                                    UnitCode = maDonViNhan
                                };
                                result = config.GenerateNumber();
                                config.Current = result;
                                cmd.CommandText = "UPDATE MD_ID_BUILDER SET \"CURRENT\" = '" + result + "' WHERE TYPE = '" + type + "' AND CODE = '" + dataReader["CODE"] + "' AND UNITCODE = '" + maDonViNhan + "' ";
                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            config = new MdIdBuilder
                            {
                                Id = Guid.NewGuid().ToString(),
                                Type = type,
                                Code = type,
                                Current = "0000",
                                UnitCode = maDonViNhan
                            };
                            result = config.GenerateNumber();
                            config.Current = result;
                            cmd.CommandText = "INSERT INTO MD_ID_BUILDER(ID,TYPE,CODE,CURRENT,UNITCODE) VALUES ('" + config.Id + "','" + config.Type + "','" + config.Code + "','" + config.Current + "','" + config.UnitCode + "')";
                            cmd.CommandType = CommandType.Text;
                            cmd.ExecuteNonQuery();
                        }
                        result = string.Format("{0}{1}", config.Code, result);
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return result;
        }
        //end
    }
}
