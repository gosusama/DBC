using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Services;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
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
    public interface INvNhapHangMuaService : IDataInfoService<NvVatTuChungTu>
    {
        NvVatTuChungTu InsertPhieu(NvNhapHangMuaVm.Dto instance);
        NvVatTuChungTu UpdatePhieu(NvNhapHangMuaVm.Dto instance);
        StateProcessApproval Approval(string id);
        NvNhapHangMuaVm.ReportModel CreateReport(string id);
        NvNhapHangMuaVm.Dto CreateNewInstance();
        void UpdateAfterApproval(NvVatTuChungTu chungTu);
        bool DeletePhieu(string id);
        MemoryStream ExportExcel(List<NvNhapHangMuaVm.Dto> data, FilterObj<NvNhapHangMuaVm.Search> filter);
        MemoryStream ExportExcelByNhaCungCap(FilterObj<NvNhapHangMuaVm.Search> filter);
        MemoryStream ExportExcelByMerchandise(FilterObj<NvNhapHangMuaVm.Search> filter);
        MemoryStream ExportExcelByMerchandiseType(FilterObj<NvNhapHangMuaVm.Search> filter);
        MemoryStream ExportExcelByMerchandiseGroup(FilterObj<NvNhapHangMuaVm.Search> filter);

        MemoryStream ExportExcelDetails(BTS.API.SERVICE.NV.NvNhapHangMuaVm.ParameterNMua pi);
        List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByMerchandiseByNCC(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByMerchandiseByHang(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByMerchandiseByMG(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByMerchandiseByML(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        List<NvNhapHangMuaVm.ObjectReportCon> CreateReportBuonTraLaiByTongHop(string loaiChungTu, string groupBy, DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        //Excel NMUA ChiTiet
        MemoryStream ExportExcelDetailsCap2(BTS.API.SERVICE.NV.NvNhapHangMuaVm.ParameterNMua pi, string titleExcel);
        //Report NMUA TongHop
        List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByTongHop(string loaiChungTu, string groupBy, DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        //Excel NMUA TongHop
        MemoryStream ExportExcelTongHop(BTS.API.SERVICE.NV.NvNhapHangMuaVm.ObjectReportNMUA pi);
        //DCN
        MemoryStream ExportExcelDCNDetail(NvNhapHangMuaVm.ParameterNMua pc);
        MemoryStream ExportExcelDCNTongHop(NvNhapHangMuaVm.ParameterNMua pc);
        List<NvGiaoDichQuayVm.ObjectReport> ReportDieuChuyenNhan(NvNhapHangMuaVm.ParameterNMua pc);
        //NHẬP KHÁC
        MemoryStream ExportExcelNKhacDetail(NvNhapHangMuaVm.ParameterNMua pc);
        MemoryStream ExportExcelNKhacTongHop(NvNhapHangMuaVm.ParameterNMua pc);
        List<NvGiaoDichQuayVm.ObjectReport> ReportNhapKhac(NvNhapHangMuaVm.ParameterNMua pc);
    }
    public class NvNhapHangMuaService : DataInfoServiceBase<NvVatTuChungTu>, INvNhapHangMuaService
    {
        public NvNhapHangMuaService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public NvNhapHangMuaVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            return new NvNhapHangMuaVm.Dto()
            {
                LoaiPhieu = TypeVoucher.NMUA.ToString(),
                MaChungTu = BuildCode_PTNX(TypeVoucher.NMUA.ToString(), unitCode, false),
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode)

            };
        }
        public bool DeletePhieu(string id)
        {
            Mapper.CreateMap<NvNhapHangMuaVm.Dto, NvVatTuChungTu>();
            Mapper.CreateMap<NvNhapHangMuaVm.DtoDetail, NvVatTuChungTuChiTiet>();
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
            //Delete(id);
            return true;

        }
        public NvVatTuChungTu InsertPhieu(NvNhapHangMuaVm.Dto instance)
        {
            instance.Calc();
            var item = Mapper.Map<NvNhapHangMuaVm.Dto, NvVatTuChungTu>(instance);
            item.Id = Guid.NewGuid().ToString();
            var _ParentUnitCode = GetParentUnitCode();
            var result = AddUnit(item);
            string _unitCode = GetCurrentUnitCode();
            result.MaChungTu = BuildCode_PTNX(TypeVoucher.NMUA.ToString(), _unitCode, true);
            item.GenerateMaChungTuPk();
            result = Insert(result);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var detailData = Mapper.Map<List<NvNhapHangMuaVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            var khoNhap = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);
            int i = 0;
            detailData.ForEach(x =>
            {
                i++;
                var merchandise = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                x.TenHang = merchandise != null ? merchandise.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTu = result.MaChungTu;
                x.MaChungTuPk = result.MaChungTuPk;
                x.Index = i;
                x.VAT = merchandise.MaVatVao;
            });
            InsertGeneralLedger(instance.DataClauseDetails, result);
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            return result;
        }
        public NvNhapHangMuaVm.ReportModel CreateReport(string id)
        {
            var result = new NvNhapHangMuaVm.ReportModel();
            var _ParentUnitCode = GetParentUnitCode();
            Mapper.CreateMap<NvVatTuChungTu, NvNhapHangMuaVm.ReportModel>();
            Mapper.CreateMap<NvVatTuChungTuChiTiet, NvNhapHangMuaVm.ReportDetailModel>();
            Mapper.CreateMap<DclGeneralLedger, NvNhapHangMuaVm.ReportClauseModel>();
            var exsit = FindById(id);
            if (exsit != null)
            {

                result = Mapper.Map<NvVatTuChungTu, NvNhapHangMuaVm.ReportModel>(exsit);
                var userName = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == exsit.ICreateBy && x.UnitCode.StartsWith(_ParentUnitCode)).FirstOrDefault();
                if (userName != null)
                {
                    result.NameNhanVienCreate = userName.TenNhanVien != null ? userName.TenNhanVien : "";
                }
                var detailData = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsit.MaChungTuPk).OrderBy(x => x.Index).ToList();
                foreach (var dt in detailData)
                {
                    var item = UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu.Equals(dt.MaHang) && x.UnitCode.StartsWith(_ParentUnitCode));
                    if (item != null) dt.TenHang = item.TenHang;
                }
                result.DataReportDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapHangMuaVm.ReportDetailModel>>(detailData);

                var dinhKhoan = UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(x => x.MaChungTuPk == exsit.MaChungTuPk).ToList();
                result.DataReportClause = Mapper.Map<List<DclGeneralLedger>, List<NvNhapHangMuaVm.ReportClauseModel>>(dinhKhoan);
                var customer = UnitOfWork.Repository<MdSupplier>().DbSet.FirstOrDefault(x => x.MaNCC == result.MaKhachHang && x.UnitCode.StartsWith(_ParentUnitCode));
                if (customer != null)
                {
                    result.TenKhachHang = customer.TenNCC;
                    result.DienThoai = customer.DienThoai;
                    result.DiaChiKhachHang = customer.DiaChi;
                }
                var kho = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoNhap && x.UnitCode.StartsWith(_ParentUnitCode));
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
                var userName = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name && x.UnitCode.StartsWith(unitCode)).FirstOrDefault();
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
        public void InsertGeneralLedger(List<NvNhapHangMuaVm.DtoClauseDetail> data, NvVatTuChungTu chungTu)
        {
            var generalLedgers = Mapper.Map<List<NvNhapHangMuaVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
            generalLedgers.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = chungTu.MaChungTuPk;
                x.MaChungTu = chungTu.MaChungTu;
                x.LoaiPhieu = chungTu.LoaiPhieu;
                x.TrangThai = chungTu.TrangThai; // Chưa duyệt
                x.NgayCT = chungTu.NgayCT;
                x.NoiDung = chungTu.NoiDung;
                x.UnitCode = x.UnitCode;
            });
            UnitOfWork.Repository<DclGeneralLedger>().InsertRange(generalLedgers);
        }
        public NvVatTuChungTu UpdatePhieu(NvNhapHangMuaVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            instance.Calc();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvNhapHangMuaVm.Dto, NvVatTuChungTu>(instance);
            var detailData = Mapper.Map<List<NvNhapHangMuaVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            masterData.ICreateBy = exsitItem.ICreateBy;
            masterData.ICreateDate = exsitItem.ICreateDate;
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            {
                var detailCollection = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsitItem.MaChungTuPk).OrderBy(x=>x.Index);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            detailData.ForEach(x =>
            {
                var merchandise = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                x.TenHang = merchandise != null ? merchandise.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = exsitItem.MaChungTuPk;
                x.MaChungTu = exsitItem.MaChungTu;
                x.VAT = merchandise.MaVatVao;
            });
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            var result = Update(masterData);
            return result;
        }
        protected override Expression<Func<NvVatTuChungTu, bool>> GetKeyFilter(NvVatTuChungTu instance)
        {
            string _unitCode = GetCurrentUnitCode();
            return x => x.MaChungTuPk == instance.MaChungTuPk && x.UnitCode == _unitCode;
        }

        public StateProcessApproval Approval(string id)
        {
            StateProcessApproval result;
            var unitCode = GetCurrentUnitCode();
            var periods = UnitOfWork.Repository<MdPeriod>().DbSet
                .Where(x => x.UnitCode == unitCode && x.TrangThai == (int)ApprovalState.IsComplete)
                .OrderByDescending(x => new { x.Year, x.Period });
            if (periods != null && periods.Count() > 0)
            {
                var currentPeriods = periods.First();

                var tableName = ProcedureCollection.GetTableName(currentPeriods.Year, currentPeriods.Period);
                if (ProcedureCollection.IncreaseVoucher(tableName, currentPeriods.Year, currentPeriods.Period, id))
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
        public void UpdateAfterApproval(NvVatTuChungTu chungTu)
        {
            var details = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == chungTu.MaChungTuPk);
            var unitCode = GetCurrentUnitCode();
            var _ParentUnitCode = GetParentUnitCode();
            if (chungTu != null)
            {
                if (!string.IsNullOrEmpty(chungTu.SoPhieuDatHang))
                {
                    //change state 
                    var donDatHang = UnitOfWork.Repository<NvDatHang>().DbSet.FirstOrDefault(x => x.SoPhieu == chungTu.SoPhieuDatHang);
                    if (donDatHang != null)
                    {
                        donDatHang.TrangThai = (int)OrderState.IsRecieved;
                        donDatHang.ObjectState = ObjectState.Modified;
                    }
                }
                foreach (var item in details)
                {
                    var itemInMasterData = UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu == item.MaHang && x.UnitCode.StartsWith(_ParentUnitCode));
                    var itemInDetailData = UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu == item.MaHang && x.MaDonVi == unitCode);

                    if (itemInMasterData != null)
                    {
                        itemInMasterData.MaKhachHang = chungTu.MaKhachHang;
                        itemInMasterData.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
                        itemInMasterData.ObjectState = ObjectState.Modified;
                    }
                    if (chungTu.TienChietKhau != null && chungTu.ThanhTienTruocVat != null && chungTu.ThanhTienTruocVat != 0)
                    {
                        decimal chietKhau = chungTu.TienChietKhau.Value / chungTu.ThanhTienTruocVat.Value;

                        item.DonGia = item.DonGia.HasValue ? item.DonGia.Value * (1 - Math.Round(chietKhau, 2)) : 0;
                    }
                    itemInDetailData.ToList().ForEach(d =>
                    {

                        d.GiaMua = item.DonGia.HasValue ? item.DonGia.Value : 0;
                        d.GiaMuaVat = d.GiaMua * (1 + d.TyLeVatVao / 100);

                        if (item.DonGia.HasValue && item.DonGia != 0)
                        {
                            d.TyLeLaiLe = 100 * (d.GiaBanLe - item.DonGia.Value) / item.DonGia.Value;
                            d.TyLeLaiBuon = 100 * (d.GiaBanBuon - item.DonGia.Value) / item.DonGia.Value;
                        }
                        d.ObjectState = ObjectState.Modified;
                    });
                }
            }

        }
        private string _convertToArrayCondition(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            var subStrAray = str.Split(',');
            int length = subStrAray.Length;
            string[] resultArray = new string[length];
            for (int i = 0; i < length; i++)
            {
                resultArray[i] = "'" + subStrAray[i] + "'";
            }
            return String.Join(",", resultArray);
        }
        #region Old Version
        public MemoryStream ExportExcel(List<NvNhapHangMuaVm.Dto> data, FilterObj<NvNhapHangMuaVm.Search> filter)
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU NHẬP HÀNG MUA"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                     string.Format("Từ ngày: {0} Đến ngày: {1}",
                     filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                     filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[3, 1].Value = "STT"; worksheet.Cells[3, 1].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 2].Value = "Ngày"; worksheet.Cells[3, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 3].Value = "Lý do"; worksheet.Cells[3, 3].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 4].Value = "Khách hàng"; worksheet.Cells[3, 4].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells[3, 5].Value = "Nhập kho về"; worksheet.Cells[3, 5].Style.Border.BorderAround(ExcelBorderStyle.Medium);
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
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.MaKhoNhap;
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
        public MemoryStream ExportExcelByMerchandise(FilterObj<NvNhapHangMuaVm.Search> filter)
        {
            var itemCollection = new List<NvNhapHangMuaVm.ObjectReport>();
            List<NvNhapHangMuaVm.ObjectReport> itemCollectionGroup = new List<NvNhapHangMuaVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuNhapGroupByMerchandise(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU NHẬP HÀNG MUA"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                var itemTotal = new NvNhapHangMuaVm.ObjectReport();
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
        public MemoryStream ExportExcelByNhaCungCap(FilterObj<NvNhapHangMuaVm.Search> filter)
        {
            var itemCollection = new List<NvNhapHangMuaVm.ObjectReport>();
            List<NvNhapHangMuaVm.ObjectReport> itemCollectionGroup = new List<NvNhapHangMuaVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuNhapGroupByNhaCungCap(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU NHẬP HÀNG MUA"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                var itemTotal = new NvNhapHangMuaVm.ObjectReport();
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
        public MemoryStream ExportExcelByMerchandiseType(FilterObj<NvNhapHangMuaVm.Search> filter)
        {
            var itemCollection = new List<NvNhapHangMuaVm.ObjectReport>();
            List<NvNhapHangMuaVm.ObjectReport> itemCollectionGroup = new List<NvNhapHangMuaVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuNhapGroupByMerchandiseType(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU NHẬP HÀNG MUA"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}",
                    filter.AdvanceData.TuNgay.HasValue ? filter.AdvanceData.TuNgay.Value.ToString("dd/MM/yyyy") : "",
                    filter.AdvanceData.DenNgay.HasValue ? filter.AdvanceData.DenNgay.Value.ToString("dd/MM/yyyy") : "");
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: Vật tư";
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã loại hàng"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên loại hàng"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tiền CK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tổng tiền"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvNhapHangMuaVm.ObjectReport();
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
        public MemoryStream ExportExcelByMerchandiseGroup(FilterObj<NvNhapHangMuaVm.Search> filter)
        {
            var itemCollection = new List<NvNhapHangMuaVm.ObjectReport>();
            List<NvNhapHangMuaVm.ObjectReport> itemCollectionGroup = new List<NvNhapHangMuaVm.ObjectReport>();
            var unitCode = GetCurrentUnitCode();
            var data = ProcedureCollection.PhieuNhapGroupByMerchandiseGroup(new BTS.API.ENTITY.ERPContext(), unitCode, filter.AdvanceData.TuNgay.Value, filter.AdvanceData.DenNgay.Value);
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU NHẬP HÀNG MUA"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                var itemTotal = new NvNhapHangMuaVm.ObjectReport();
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
        public MemoryStream ExportExcelDetails(BTS.API.SERVICE.NV.NvNhapHangMuaVm.ParameterNMua pi)
        {
            List<NvNhapHangMuaVm.ObjectReportCon> itemCollectionGroup = new List<NvNhapHangMuaVm.ObjectReportCon>();
            var unitCode = GetCurrentUnitCode();
            List<NvNhapHangMuaVm.ObjectReportCon> data = new List<NvNhapHangMuaVm.ObjectReportCon>();
            var dknhom = "";
            var titleCotMa = "";
            var titleCotName = "";
            switch (pi.Option)
            {
                case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.hangHoa:
                    data = CreateReportInventoryByMerchandiseByHang(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    dknhom = " hàng hóa";
                    titleCotMa = "Mã hàng - tên hàng";
                    titleCotName = "hàng";
                    break;
                case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                    data = CreateReportInventoryByMerchandiseByMG(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    dknhom = " nhóm vật tư";
                    titleCotMa = "Mã nhóm hàng ";
                    titleCotName = "Tên nhóm hàng";
                    break;
                case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                    data = CreateReportInventoryByMerchandiseByML(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    dknhom = "loại vật tư";
                    titleCotMa = "Mã Loại hàng";
                    titleCotName = "Tên loại hàng";
                    break;
                case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                    data = CreateReportInventoryByMerchandiseByNCC(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    dknhom = " nhà cung cấp";
                    titleCotMa = "Mã nhà cung cấp";
                    titleCotName = "Tên cung cấp";
                    break;
                default:
                    break;
            }
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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU NHẬP HÀNG MUA"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}", pi.FromDate.Date.ToString(), pi.ToDate.Date.ToString());
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: " + dknhom;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "" + titleCotMa; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "" + titleCotName; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tiền CK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tổng tiền"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvNhapHangMuaVm.ObjectReportCon();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in itemCollectionGroup)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuong;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.TienHang; worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.TienChietKhau; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.TienVat; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.TongTien; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    itemTotal.TienChietKhau = itemTotal.TienChietKhau + item.TienChietKhau;
                    itemTotal.TienVat = itemTotal.TienVat + item.TienVat;
                    itemTotal.TongTien = itemTotal.TongTien + item.TongTien;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, 1].Value = "Tổng";
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong;
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienChietKhau; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienVat; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TongTien; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

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
        public List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByMerchandiseByNCC(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATHEONHACUNGCAPVER2(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                            {
                                Ma = reader["MaCha"].ToString(),
                                Ten = reader["TenCha"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0,
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                            };
                            resultDetail.Add(detailsitem);
                        }

                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return resultDetail;
        }
        public List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByMerchandiseByHang(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATHEOHANG(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var isDonGiaNhap = decimal.TryParse(reader["DonGiaNhap"].ToString(), out donGiaNhap);
                            var isGiaBan = decimal.TryParse(reader["GiaBan"].ToString(), out giaBan);
                            var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                            {
                                Ma = reader["MaCha"].ToString(),
                                Ten = reader["TenCha"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0,
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                GiaBan = isGiaBan ? giaBan : 0,
                                DonGiaNhap = isDonGiaNhap ? donGiaNhap : 0
                            };
                            resultDetail.Add(detailsitem);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return resultDetail;
        }
        public List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByMerchandiseByMG(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATHEONHOMHANGVER2(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                            {
                                Ma = reader["MaCha"].ToString(),
                                Ten = reader["TenCha"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0,
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                            };
                            resultDetail.Add(detailsitem);
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return resultDetail;
        }
        public List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByMerchandiseByML(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATHEOLOAIHANGVER2(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                            {
                                Ma = reader["MaCha"].ToString(),
                                Ten = reader["TenCha"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0,
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                            };
                            resultDetail.Add(detailsitem);
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return resultDetail;
        }
        public List<NvNhapHangMuaVm.ObjectReportCon> CreateReportBuonTraLaiByTongHop(string loaiChungTu, string groupBy, DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pLoaiChungTu = new OracleParameter("pLoaiChungTu", OracleDbType.NVarchar2, loaiChungTu, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "";
                        if (loaiChungTu.Equals(TypeVoucher.NHBANTL.ToString()))
                        {
                            str = "BEGIN TBNETERP.NVPHIEU.NHAPBANBUONTRALAITONGHOP(:pLoaiChungTu,:pGroupBy,:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        }
                        else
                        {
                            str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATONGHOP(:pLoaiChungTu,:pGroupBy,:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        }
                        ctx.Database.ExecuteSqlCommand(str, pLoaiChungTu, pGroupBy, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            // var detailsitem = new NvNhapHangMuaVm.ObjectReportCon();
                            if (groupBy.Equals(InventoryGroupBy.MAVATTU))
                            {
                                decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                                var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                                var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                                var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                                var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                                var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                                var isDonGiaNhap = decimal.TryParse(reader["DonGiaNhap"].ToString(), out donGiaNhap);
                                var isGiaBan = decimal.TryParse(reader["GiaBan"].ToString(), out giaBan);
                                var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                                {
                                    Ma = reader["MaCha"].ToString(),
                                    Ten = reader["TenCha"].ToString(),
                                    SoLuong = isSoLuong ? soLuong : 0,
                                    TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                    TienVat = isTienVat ? tienVat : 0,
                                    TienHang = isTienHang ? tienHang : 0,
                                    TongTien = isTongTien ? tongTien : 0,
                                    MaCha = reader["MaCha"].ToString(),
                                    TenCha = reader["TenCha"].ToString(),
                                };
                                resultDetail.Add(detailsitem);

                            }
                            else
                            {
                                decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                                var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                                var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                                var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                                var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                                var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                                var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                                {
                                    Ma = reader["MaCha"].ToString(),
                                    Ten = reader["TenCha"].ToString(),
                                    SoLuong = isSoLuong ? soLuong : 0,
                                    TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                    TienVat = isTienVat ? tienVat : 0,
                                    TienHang = isTienHang ? tienHang : 0,
                                    TongTien = isTongTien ? tongTien : 0,
                                    MaCha = reader["MaCha"].ToString(),
                                    TenCha = reader["TenCha"].ToString(),
                                };
                                resultDetail.Add(detailsitem);

                            }
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return resultDetail;
        }
        public List<NvNhapHangMuaVm.ObjectReportCha> CreateReportInventoryByMerchandiseByNCCDetailCap2(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NMUACTTHEONHACUNGCAPVER2(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                            DateTime ngayPhatSinh;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var isDonGiaNhap = decimal.TryParse(reader["DonGiaNhap"].ToString(), out donGiaNhap);
                            var isGiaBan = decimal.TryParse(reader["GiaBan"].ToString(), out giaBan);
                            var isNgayPhatSinh = DateTime.TryParse(reader["NgayChungTu"].ToString(), out ngayPhatSinh);
                            var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                            {
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0,
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                GiaBan = isGiaBan ? giaBan : 0,
                                DonGiaNhap = isDonGiaNhap ? donGiaNhap : 0,
                                NgayChungTu = isNgayPhatSinh ? ngayPhatSinh.Day.ToString() + "/" + ngayPhatSinh.Month.ToString() + "/" + ngayPhatSinh.Year.ToString() : " ",
                            };
                            resultDetail.Add(detailsitem);
                        }

                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvNhapHangMuaVm.ObjectReportCha> listCha = new List<NvNhapHangMuaVm.ObjectReportCha>();

                        temp.ToList().ForEach(x =>
                        {
                            NvNhapHangMuaVm.ObjectReportCha model = new NvNhapHangMuaVm.ObjectReportCha();
                            model.Ma = x.Key;

                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }
                            foreach (var itemd in children)
                            {
                                model.TienVat = model.TienVat + itemd.TienVat;
                                model.TienHang = model.TienHang + itemd.TienHang;
                                model.TienChietKhau = model.TienChietKhau + itemd.TienChietKhau;
                                model.SoLuong = model.SoLuong + itemd.SoLuong;
                            }

                            model.DataDetail.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public List<NvNhapHangMuaVm.ObjectReportCha> CreateReportInventoryByMerchandiseDetailCap2(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATHEOHANGCHITIET(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var isDonGiaNhap = decimal.TryParse(reader["DonGiaNhap"].ToString(), out donGiaNhap);
                            var isGiaBan = decimal.TryParse(reader["GiaBan"].ToString(), out giaBan);
                            var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                            {
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0,
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                GiaBan = isGiaBan ? giaBan : 0,
                                DonGiaNhap = isDonGiaNhap ? donGiaNhap : 0,
                                NgayChungTu = reader["NgayChungTu"].ToString()

                            };
                            resultDetail.Add(detailsitem);
                        }

                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvNhapHangMuaVm.ObjectReportCha> listCha = new List<NvNhapHangMuaVm.ObjectReportCha>();

                        temp.ToList().ForEach(x =>
                        {
                            NvNhapHangMuaVm.ObjectReportCha model = new NvNhapHangMuaVm.ObjectReportCha();
                            model.Ma = x.Key;

                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }
                            foreach (var itemd in children)
                            {
                                model.TienVat = model.TienVat + itemd.TienVat;
                                model.TienHang = model.TienHang + itemd.TienHang;
                                model.TienChietKhau = model.TienChietKhau + itemd.TienChietKhau;
                                model.SoLuong = model.SoLuong + itemd.SoLuong;
                            }

                            model.DataDetail.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public List<NvNhapHangMuaVm.ObjectReportCha> CreateReportInventoryByMerchandiseDetailByMGCap2(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUACHITIETTHEONHOMHANGVER2(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                            DateTime ngayPhatSinh;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var isDonGiaNhap = decimal.TryParse(reader["DonGiaNhap"].ToString(), out donGiaNhap);
                            var isGiaBan = decimal.TryParse(reader["GiaBan"].ToString(), out giaBan);
                            var isNgayPhatSinh = DateTime.TryParse(reader["NgayChungTu"].ToString(), out ngayPhatSinh);
                            var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                            {
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0,
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                GiaBan = isGiaBan ? giaBan : 0,
                                DonGiaNhap = isDonGiaNhap ? donGiaNhap : 0,
                                NgayChungTu = isNgayPhatSinh ? ngayPhatSinh.Day.ToString() + "/" + ngayPhatSinh.Month.ToString() + "/" + ngayPhatSinh.Year.ToString() : " ",
                            };
                            resultDetail.Add(detailsitem);
                        }

                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvNhapHangMuaVm.ObjectReportCha> listCha = new List<NvNhapHangMuaVm.ObjectReportCha>();

                        temp.ToList().ForEach(x =>
                        {
                            NvNhapHangMuaVm.ObjectReportCha model = new NvNhapHangMuaVm.ObjectReportCha();
                            model.Ma = x.Key;

                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }
                            foreach (var itemd in children)
                            {
                                model.TienVat = model.TienVat + itemd.TienVat;
                                model.TienHang = model.TienHang + itemd.TienHang;
                                model.TienChietKhau = model.TienChietKhau + itemd.TienChietKhau;
                                model.SoLuong = model.SoLuong + itemd.SoLuong;
                            }

                            model.DataDetail.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public List<NvNhapHangMuaVm.ObjectReportCha> CreateReportInventoryByMerchandiseDetailByMLCap2(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUACHITIETTHEOLOAIHANGVER2(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                            DateTime ngayPhatSinh;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var isDonGiaNhap = decimal.TryParse(reader["DonGiaNhap"].ToString(), out donGiaNhap);
                            var isGiaBan = decimal.TryParse(reader["GiaBan"].ToString(), out giaBan);
                            var isNgayPhatSinh = DateTime.TryParse(reader["NgayChungTu"].ToString(), out ngayPhatSinh);
                            var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                            {
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0,
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                GiaBan = isGiaBan ? giaBan : 0,
                                DonGiaNhap = isDonGiaNhap ? donGiaNhap : 0,
                                NgayChungTu = isNgayPhatSinh ? ngayPhatSinh.Day.ToString() + "/" + ngayPhatSinh.Month.ToString() + "/" + ngayPhatSinh.Year.ToString() : " ",
                            };
                            resultDetail.Add(detailsitem);
                        }

                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvNhapHangMuaVm.ObjectReportCha> listCha = new List<NvNhapHangMuaVm.ObjectReportCha>();

                        temp.ToList().ForEach(x =>
                        {
                            NvNhapHangMuaVm.ObjectReportCha model = new NvNhapHangMuaVm.ObjectReportCha();
                            model.Ma = x.Key;

                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }
                            foreach (var itemd in children)
                            {
                                model.TienVat = model.TienVat + itemd.TienVat;
                                model.TienHang = model.TienHang + itemd.TienHang;
                                model.TienChietKhau = model.TienChietKhau + itemd.TienChietKhau;
                                model.SoLuong = model.SoLuong + itemd.SoLuong;
                            }

                            model.DataDetail.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        #endregion
        //Get from Procedure NMUA ChiTiet
        public List<NvNhapHangMuaVm.ObjectReportCha> CreateReportInventoryChiTiet(string loaiChungTu, string groupBy, DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            var _ParentUnitCode = GetParentUnitCode();
            var currentPeriod = CurrentSetting.GetKhoaSo(unitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);

            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pLoaiChungTu = new OracleParameter("pLoaiChungTu", OracleDbType.NVarchar2, loaiChungTu, ParameterDirection.Input);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);

                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "";
                        if (loaiChungTu.Equals(TypeVoucher.NHBANTL.ToString()))
                        {
                            str = "BEGIN TBNETERP.NVPHIEU.NHAPBANBUONTRALAICHITIET(:pKy,:pLoaiChungTu,:pGroupBy,:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pKy, pLoaiChungTu, pGroupBy, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        }
                        else
                        {
                            str = "BEGIN TBNETERP.NVPHIEU.NHAPMUACHITIET(:pLoaiChungTu,:pGroupBy,:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pLoaiChungTu, pGroupBy, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        }
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            if (groupBy.Equals(TypeVoucher.NHBANTL.ToString()))
                            {
                                decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan, tienVonChuaVat;
                                DateTime ngayPhatSinh;
                                var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                                var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                                var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                                var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                                var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                                var isDonGiaNhap = decimal.TryParse(reader["DonGiaNhap"].ToString(), out donGiaNhap);
                                var isGiaBan = decimal.TryParse(reader["GiaBan"].ToString(), out giaBan);
                                var isNgayPhatSinh = DateTime.TryParse(reader["NgayChungTu"].ToString(), out ngayPhatSinh);
                                var isTienVonChuaVat = decimal.TryParse(reader["VON"].ToString(), out tienVonChuaVat);

                                var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                                {
                                    Ma = reader["MaCon"].ToString(),
                                    Ten = reader["TenCon"].ToString(),
                                    SoLuong = isSoLuong ? soLuong : 0,
                                    TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                    TienVat = isTienVat ? tienVat : 0,
                                    TienHang = isTienHang ? tienHang : 0,
                                    TongTien = isTongTien ? tongTien : 0,
                                    MaCha = reader["MaCha"].ToString(),
                                    TenCha = reader["TenCha"].ToString(),
                                    GiaBan = isGiaBan ? giaBan : 0,
                                    DonGiaNhap = isDonGiaNhap ? donGiaNhap : 0,
                                    TienVonChuaVat = isTienVonChuaVat ? tienVonChuaVat : 0,

                                    NgayChungTu = isNgayPhatSinh ? ngayPhatSinh.Day.ToString() + "/" + ngayPhatSinh.Month.ToString() + "/" + ngayPhatSinh.Year.ToString() : " ",
                                };
                                resultDetail.Add(detailsitem);
                            }

                            else
                            {
                                decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                                DateTime ngayPhatSinh;
                                var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                                var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                                var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                                var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                                var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                                var isDonGiaNhap = decimal.TryParse(reader["DonGiaNhap"].ToString(), out donGiaNhap);
                                var isGiaBan = decimal.TryParse(reader["GiaBan"].ToString(), out giaBan);
                                var isNgayPhatSinh = DateTime.TryParse(reader["NgayChungTu"].ToString(), out ngayPhatSinh);
                                var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                                {
                                    Ma = reader["MaCon"].ToString(),
                                    Ten = reader["TenCon"].ToString(),
                                    SoLuong = isSoLuong ? soLuong : 0,
                                    TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                    TienVat = isTienVat ? tienVat : 0,
                                    TienHang = isTienHang ? tienHang : 0,
                                    TongTien = isTongTien ? tongTien : 0,
                                    MaCha = reader["MaCha"].ToString(),
                                    TenCha = reader["TenCha"].ToString(),
                                    GiaBan = isGiaBan ? giaBan : 0,
                                    DonGiaNhap = isDonGiaNhap ? donGiaNhap : 0,
                                    NgayChungTu = isNgayPhatSinh ? ngayPhatSinh.Day.ToString() + "/" + ngayPhatSinh.Month.ToString() + "/" + ngayPhatSinh.Year.ToString() : " ",
                                };
                                resultDetail.Add(detailsitem);
                            }
                        }


                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvNhapHangMuaVm.ObjectReportCha> listCha = new List<NvNhapHangMuaVm.ObjectReportCha>();

                        temp.ToList().ForEach(x =>
                        {
                            NvNhapHangMuaVm.ObjectReportCha model = new NvNhapHangMuaVm.ObjectReportCha();
                            model.Ma = x.Key;

                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }
                            foreach (var itemd in children)
                            {
                                model.TienVat = model.TienVat + itemd.TienVat;
                                model.TienHang = model.TienHang + itemd.TienHang;
                                model.TienChietKhau = model.TienChietKhau + itemd.TienChietKhau;
                                model.SoLuong = model.SoLuong + itemd.SoLuong;
                            }

                            model.DataDetail.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        //NMUA TongHop
        public MemoryStream ExportExcelTongHop(BTS.API.SERVICE.NV.NvNhapHangMuaVm.ObjectReportNMUA pi)
        {
            string fromDateFomart = pi.FromDay + "/" + pi.FromMonth + "/" + pi.FromYear;
            string toDateFomart = pi.ToDay + "/" + pi.ToMonth + "/" + pi.ToYear;

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
                worksheet.Cells[1, 1].Value = "DANH SÁCH PHIẾU NHẬP HÀNG MUA"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 8].Merge = true;
                worksheet.Cells[3, 1, 3, 8].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: " + pi.GroupType;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[4, 5].Value = "Tiền hàng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tiền CK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tổng tiền"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvNhapHangMuaVm.ObjectReportCon();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in pi.DetailData)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuong;
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.TienHang; worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.TienChietKhau; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.TienVat; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.TongTien; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                    itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                    itemTotal.TienChietKhau = itemTotal.TienChietKhau + item.TienChietKhau;
                    itemTotal.TienVat = itemTotal.TienVat + item.TienVat;
                    itemTotal.TongTien = itemTotal.TongTien + item.TongTien;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, 1].Value = "Tổng";
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuong;
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienHang; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.TienChietKhau; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienVat; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TongTien; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 7].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

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
        //NMUA ChiTiet
        public MemoryStream ExportExcelDetailsCap2(BTS.API.SERVICE.NV.NvNhapHangMuaVm.ParameterNMua pi, string titleExcel)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> itemCollectionGroup = new List<NvNhapHangMuaVm.ObjectReportCha>();
            var unitCode = GetCurrentUnitCode();
            List<NvNhapHangMuaVm.ObjectReportCha> data = new List<NvNhapHangMuaVm.ObjectReportCha>();
            var dknhom = "";
            var titleCotMa = "";
            var titleCotName = "";
            switch (pi.Option)
            {
                case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.hangHoa:
                    //data = CreateReportInventoryByMerchandiseDetailCap2(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(pi.LoaiChungTu, InventoryGroupBy.MAVATTU.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = " nhóm vật tư";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Tên hàng";
                    break;
                case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                    //data = CreateReportInventoryByMerchandiseDetailByMGCap2(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(pi.LoaiChungTu, InventoryGroupBy.MANHOMVATTU.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = " nhóm vật tư";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Tên hàng";
                    break;
                case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                    //data = CreateReportInventoryByMerchandiseDetailByMLCap2(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(pi.LoaiChungTu, InventoryGroupBy.MALOAIVATTU.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = "loại vật tư";
                    titleCotMa = "Mã hàng ";
                    titleCotName = "Tên hàng";
                    break;
                case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                    //data = CreateReportInventoryByMerchandiseByNCCDetailCap2(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(pi.LoaiChungTu, InventoryGroupBy.MAKHACHHANG.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = " nhà cung cấp";
                    titleCotMa = "Mã hàng ";
                    titleCotName = "Tên hàng";
                    break;
                case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.phieu:
                    //data = CreateReportInventoryByMerchandiseByNCCDetailCap2(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(pi.LoaiChungTu, InventoryGroupBy.PHIEU.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = " phiếu";
                    titleCotMa = "Mã hàng ";
                    titleCotName = "Tên hàng";
                    break;

                //case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.hangHoa:
                //    data = CreateReportInventoryByMerchandiseByNCCDetail(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                //    dknhom = " Hàng hóa";
                //    titleCotMa = "Mã hàng - tên hàng";
                //    titleCotName = "Hàng";
                //    break;
                default:
                    break;
            }
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
                worksheet.Cells[1, 1].Value = "" + titleExcel; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}", pi.FromDate.Date.ToString(), pi.ToDate.Date.ToString());
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: " + dknhom;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "" + titleCotMa; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "" + titleCotName; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Ngày chứng từ"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                if (pi.LoaiChungTu.Equals(TypeVoucher.NHBANTL.ToString()))
                {
                    worksheet.Cells[4, 5].Value = "Giá vốn chưa vat"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
                else
                {
                    worksheet.Cells[4, 5].Value = "Số lượng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                worksheet.Cells[4, 6].Value = "Đơn giá nhập"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Giá bán"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tiền hàng"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "Tiền CK"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Tiền VAT"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Value = "Tổng tiền"; worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvNhapHangMuaVm.ObjectReportCon();
                int currentRow = startRow;
                int stt = 0;
                foreach (var items in itemCollectionGroup)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 11].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = items.Ma + " - " + items.Ten;
                    currentRow++;
                    foreach (var item in items.DataDetail)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                        worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                        worksheet.Cells[currentRow, startColumn + 3].Value = item.NgayChungTu;
                        if (pi.LoaiChungTu.Equals(TypeVoucher.NHBANTL.ToString()))
                        {
                            worksheet.Cells[currentRow, startColumn + 4].Value = item.TienVonChuaVat;
                        }
                        else
                        {
                            worksheet.Cells[currentRow, startColumn + 4].Value = item.SoLuong;
                        }
                        worksheet.Cells[currentRow, startColumn + 5].Value = item.DonGiaNhap; worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = item.GiaBan; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = item.TienHang; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = item.TienChietKhau; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = item.TienVat; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = item.TongTien; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        itemTotal.SoLuong = itemTotal.SoLuong + item.SoLuong;
                        if (pi.LoaiChungTu.Equals(TypeVoucher.NHBANTL.ToString()))
                        {
                            itemTotal.TienVonChuaVat = itemTotal.TienVonChuaVat + item.TienVonChuaVat;
                        }
                        itemTotal.TienHang = itemTotal.TienHang + item.TienHang;
                        itemTotal.TienChietKhau = itemTotal.TienChietKhau + item.TienChietKhau;
                        itemTotal.TienVat = itemTotal.TienVat + item.TienVat;
                        itemTotal.TongTien = itemTotal.TongTien + item.TongTien;
                        currentRow++;
                    }
                }

                worksheet.Cells[currentRow, 1, currentRow, startColumn + 3].Merge = true;
                worksheet.Cells[currentRow, 1].Value = "Tổng";
                if (pi.LoaiChungTu.Equals(TypeVoucher.NHBANTL.ToString()))
                {
                    worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.TienVonChuaVat;
                }
                else
                {
                    worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.SoLuong;
                }

                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TienHang; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.TienChietKhau; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.TienVat; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.TongTien; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);


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
        //NMUA TongHop
        public List<NvNhapHangMuaVm.ObjectReportCon> CreateReportInventoryByTongHop(string loaiChungTu, string groupBy, DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<NvNhapHangMuaVm.ObjectReportCha> result = new List<NvNhapHangMuaVm.ObjectReportCha>();
            List<NvNhapHangMuaVm.ObjectReportCon> resultDetail = new List<NvNhapHangMuaVm.ObjectReportCon>();
            unitCode = GetCurrentUnitCode();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        //nhaCungCapCodes = _convertToArrayConditionDetail(nhaCungCapCodes, result);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pLoaiChungTu = new OracleParameter("pLoaiChungTu", OracleDbType.NVarchar2, loaiChungTu, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "";
                        if (loaiChungTu.Equals(TypeVoucher.NHBANTL.ToString()))
                        {
                            str = "BEGIN TBNETERP.NVPHIEU.NHAPBANBUONTRALAITONGHOP(:pLoaiChungTu,:pGroupBy,:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        }
                        else
                        {
                            str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATONGHOP(:pLoaiChungTu,:pGroupBy,:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        }
                        ctx.Database.ExecuteSqlCommand(str, pLoaiChungTu, pGroupBy, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            // var detailsitem = new NvNhapHangMuaVm.ObjectReportCon();
                            if (groupBy.Equals(InventoryGroupBy.MAVATTU))
                            {
                                decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                                var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                                var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                                var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                                var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                                var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                                var isDonGiaNhap = decimal.TryParse(reader["DonGiaNhap"].ToString(), out donGiaNhap);
                                var isGiaBan = decimal.TryParse(reader["GiaBan"].ToString(), out giaBan);
                                var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                                {
                                    Ma = reader["MaCha"].ToString(),
                                    Ten = reader["TenCha"].ToString(),
                                    SoLuong = isSoLuong ? soLuong : 0,
                                    TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                    TienVat = isTienVat ? tienVat : 0,
                                    TienHang = isTienHang ? tienHang : 0,
                                    TongTien = isTongTien ? tongTien : 0,
                                    MaCha = reader["MaCha"].ToString(),
                                    TenCha = reader["TenCha"].ToString(),
                                };
                                resultDetail.Add(detailsitem);

                            }
                            else
                            {
                                decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau, donGiaNhap, giaBan;
                                var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                                var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                                var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                                var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                                var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                                var detailsitem = new NvNhapHangMuaVm.ObjectReportCon()
                                {
                                    Ma = reader["MaCha"].ToString(),
                                    Ten = reader["TenCha"].ToString(),
                                    SoLuong = isSoLuong ? soLuong : 0,
                                    TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                    TienVat = isTienVat ? tienVat : 0,
                                    TienHang = isTienHang ? tienHang : 0,
                                    TongTien = isTongTien ? tongTien : 0,
                                    MaCha = reader["MaCha"].ToString(),
                                    TenCha = reader["TenCha"].ToString(),
                                };
                                resultDetail.Add(detailsitem);

                            }
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return resultDetail;
        }
        #region Lastest Version - Dieu Chuyen Nhan
        public List<NvGiaoDichQuayVm.ObjectReport> ReportDieuChuyenNhan(NvNhapHangMuaVm.ParameterNMua pi)
        {
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);
            switch (pi.RouteType)
            {
                case NvNhapHangMuaVm.TypeDieuChuyenNhan.NHANCHUYENKHO:
                    switch (pi.Option)
                    {
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                            data = ProcedureCollection.DCNTongHop(ky, "MAKHOHANG", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                            data = ProcedureCollection.DCNTongHop(ky, "MALOAIVATTU", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                            data = ProcedureCollection.DCNTongHop(ky, "MANHOMVATTU", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                            data = ProcedureCollection.DCNTongHop(ky, "MAKHACHHANG", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.donVi:
                        default:
                            data = ProcedureCollection.DCNTongHop(ky, "MAVATTU", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
                default:
                    switch (pi.Option)
                    {
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                            data = ProcedureCollection.DCNTongHop(ky, "MAKHOHANG", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                            data = ProcedureCollection.DCNTongHop(ky, "MALOAIVATTU", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                            data = ProcedureCollection.DCNTongHop(ky, "MANHOMVATTU", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                            data = ProcedureCollection.DCNTongHop(ky, "MAKHACHHANG", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCNTongHop(ky, "MAVATTU", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
            }
            return data;
        }
        public MemoryStream ExportExcelDCNTongHop(NvNhapHangMuaVm.ParameterNMua pi)
        {
            List<NvGiaoDichQuayVm.ObjectReport> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReport>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();

            var titleCotName = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);
            switch (pi.RouteType)
            {
                case NvNhapHangMuaVm.TypeDieuChuyenNhan.NHANCHUYENKHO:
                    switch (pi.Option)
                    {
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                            data = ProcedureCollection.DCNTongHop(ky, "MAKHOHANG", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                            data = ProcedureCollection.DCNTongHop(ky, "MALOAIVATTU", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                            data = ProcedureCollection.DCNTongHop(ky, "MANHOMVATTU", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                            data = ProcedureCollection.DCNTongHop(ky, "MAKHACHHANG", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCNTongHop(ky, "MAVATTU", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
                default:
                    switch (pi.Option)
                    {
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                            data = ProcedureCollection.DCNTongHop(ky, "MAKHOHANG", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                            data = ProcedureCollection.DCNTongHop(ky, "MALOAIVATTU", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                            data = ProcedureCollection.DCNTongHop(ky, "MANHOMVATTU", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                            data = ProcedureCollection.DCNTongHop(ky, "MAKHACHHANG", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCNTongHop(ky, "MAVATTU", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
            }
            switch (pi.Option)
            {
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                    titleCotName = "Kho hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                    titleCotName = "Loại hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                    titleCotName = "Nhóm hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                    titleCotName = "Nhà cung cấp";
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
                    case NvNhapHangMuaVm.TypeDieuChuyenNhan.NHANCHUYENKHO:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬN CHUYỂN KHO"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        break;
                    default:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬN SIÊU THỊ THÀNH VIÊN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
        public MemoryStream ExportExcelDCNDetail(NvNhapHangMuaVm.ParameterNMua pi)
        {
            var itemCollection = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCha> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha>();
            var titleCotName = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);

            switch (pi.RouteType)
            {
                case NvNhapHangMuaVm.TypeDieuChuyenNhan.NHANCHUYENKHO:
                    switch (pi.Option)
                    {
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                            data = ProcedureCollection.DCNChiTiet(ky, "MAKHOHANG", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                            data = ProcedureCollection.DCNChiTiet(ky, "MALOAIVATTU", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                            data = ProcedureCollection.DCNChiTiet(ky, "MANHOMVATTU", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                            data = ProcedureCollection.DCNChiTiet(ky, "MAKHACHHANG", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCNChiTiet(ky, "MAVATTU", "AND NVL(t.MADONVIXUAT,'') = NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
                default:
                    switch (pi.Option)
                    {
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                            data = ProcedureCollection.DCNChiTiet(ky, "MAKHOHANG", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                            data = ProcedureCollection.DCNChiTiet(ky, "MALOAIVATTU", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                            data = ProcedureCollection.DCNChiTiet(ky, "MANHOMVATTU", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                            data = ProcedureCollection.DCNChiTiet(ky, "MAKHACHHANG", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                        default:
                            data = ProcedureCollection.DCNChiTiet(ky, "MAVATTU", "AND NVL(t.MADONVIXUAT,'') <> NVL(t.UNITCODE,'')", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                            break;
                    }
                    break;
            }
            switch (pi.Option)
            {
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                    titleCotName = "Kho hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                    titleCotName = "Loại hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                    titleCotName = "Nhóm hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                    titleCotName = "Nhà cung cấp";
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
                    case NvNhapHangMuaVm.TypeDieuChuyenNhan.NHANCHUYENKHO:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬN CHUYỂN KHO"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        break;
                    default:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬN SIÊU THỊ THÀNH VIÊN"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                worksheet.Cells[4, 7].Value = "Giá vốn chưa VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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
        #region Lastest Version -Nhập khác
        public List<NvGiaoDichQuayVm.ObjectReport> ReportNhapKhac(NvNhapHangMuaVm.ParameterNMua pi)
        {
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();
            switch (pi.Option)
            {
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MAKHO.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MALOAIVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MANHOMVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MAKHACHHANG.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                default:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MAVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
            }

            return data;
        }
        public MemoryStream ExportExcelNKhacTongHop(NvNhapHangMuaVm.ParameterNMua pi)
        {
            List<NvGiaoDichQuayVm.ObjectReport> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReport>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();

            var titleCotName = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);
            switch (pi.Option)
            {
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MAKHO.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MALOAIVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MANHOMVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MAKHACHHANG.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
                default:
                    data = ProcedureCollection.NKhacTongHop(pi.LoaiNhapKhac, InventoryGroupBy.MAVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    break;
            }
            switch (pi.Option)
            {
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                    titleCotName = "Kho hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                    titleCotName = "Loại hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                    titleCotName = "Nhóm hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                    titleCotName = "Nhà cung cấp";
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
                switch (pi.LoaiNhapKhac)
                {
                    case "N3":
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬP ĐIỀU CHỈNH"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        break;
                    default:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬP HÀNG XUẤT ÂM"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
        public MemoryStream ExportExcelNKhacDetail(NvNhapHangMuaVm.ParameterNMua pi)
        {
            var itemCollection = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCha> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReportCha>();
            var titleCotName = "";
            var currentPeriod = CurrentSetting.GetKhoaSo(pi.UnitCode);
            var ky = ProcedureCollection.GetTableName(currentPeriod.Year, currentPeriod.Period);

            switch (pi.Option)
            {
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                    data = ProcedureCollection.NKhacChiTiet(pi.LoaiNhapKhac, InventoryGroupBy.MAKHO.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName = "Kho hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                    data = ProcedureCollection.NKhacChiTiet(pi.LoaiNhapKhac, InventoryGroupBy.MALOAIVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName = "Loại hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                    data = ProcedureCollection.NKhacChiTiet(pi.LoaiNhapKhac, InventoryGroupBy.MANHOMVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName = "Nhóm hàng";
                    break;
                case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                    data = ProcedureCollection.NKhacChiTiet(pi.LoaiNhapKhac, InventoryGroupBy.MAKHACHHANG.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName = "Nhà cung cấp";
                    break;
                default:
                    data = ProcedureCollection.NKhacChiTiet(pi.LoaiNhapKhac, InventoryGroupBy.MAVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
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
                switch (pi.LoaiNhapKhac)
                {
                    case "N3":
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬP ĐIỀU CHỈNH"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        break;
                    default:
                        worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬP HÀNG XUẤT ÂM"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
                worksheet.Cells[4, 7].Value = "Giá vốn chưa VAT"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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
