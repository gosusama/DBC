using BTS.API.ENTITY.NV;
using BTS.API.ENTITY.DCL;

using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using BTS.API.ENTITY;
using System.Data;
using System.IO;
using BTS.API.SERVICE.BuildQuery;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using BTS.API.SERVICE.DCL;
using System.Drawing;
using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.MD;

namespace BTS.API.SERVICE.NV
{
    public interface INvKiemKeService : IDataInfoService<NvKiemKe>
    {
        string BuildCode();
        bool Approval(NvKiemKeVm.Dto instance, string table, string year, int period);
        NvKiemKe UpdateApproval(NvKiemKeVm.Dto instance);
        NvKiemKe InsertPhieu(NvKiemKeVm.Dto instance);
        NvKiemKe UpdatePhieu(NvKiemKeVm.Dto instance);
        MemoryStream ExportExcelTongHop(ParameterKiemKe pi);
        MemoryStream ExportExcelDetail(ParameterKiemKe pi);
        List<NvKiemKeVm.ObjectReport> ReportKiemKe(ParameterKiemKe pi);
        bool DeletePhieu(string id);
        MemoryStream ExportExcelExternalCodeInvertory(List<NvKiemKeVm.ExternalCodeInInventory> pi);
    }
    public class NvKiemKeService : DataInfoServiceBase<NvKiemKe>, INvKiemKeService
    {
        public NvKiemKeService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public string BuildCode()
        {
            string newCode = string.Empty;
            var unitCode = GetCurrentUnitCode();
            var maPhieu = UnitOfWork.Repository<NvKiemKe>().DbSet.OrderByDescending(x => x.MaPhieuKiemKe).Select(x => x.MaPhieuKiemKe).FirstOrDefault();
            if (!string.IsNullOrEmpty(maPhieu))
            {
                int len = maPhieu.Length;
                string soPhieu = maPhieu.Substring(0, 4);
                string ma = maPhieu.Substring(4, 4);
                int count = Int32.Parse(ma);
                count++;
                string newcount = string.Format("{0:0000}", count);
                string yourDateString = DateTime.Now.ToString("ddMMyyyy");
                newCode = (soPhieu + newcount + yourDateString).Trim();
            }
            else
            {
                string soPhieu = "N_KK";
                string ma = "0000";
                int count = Int32.Parse(ma);
                count++;
                string newcount = string.Format("{0:0000}", count);
                string yourDateString = DateTime.Now.ToString("ddMMyyyy");
                newCode = (soPhieu + newcount + yourDateString).Trim();
            }
            return newCode;
        }

        public bool DeletePhieu(string id)
        {
            var instance = UnitOfWork.Repository<NvKiemKe>().DbSet.Where(x => x.Id == id).FirstOrDefault();

            if (instance == null)
            {
                return false;
            }
            var detailData = UnitOfWork.Repository<NvKiemKeChiTiet>().DbSet.Where(o => o.MaPhieuKiemKe == instance.MaPhieuKiemKe).ToList();
            foreach (NvKiemKeChiTiet dt in detailData)
            {
                dt.ObjectState = ObjectState.Deleted;
            }
            UnitOfWork.Repository<NvKiemKe>().Delete(instance);
            return true;
        }

        public bool Approval(NvKiemKeVm.Dto instance,string table, string year, int period)
        {
            var pTableName = new OracleParameter("pTableName", OracleDbType.NVarchar2, table, ParameterDirection.Input);
            var pYear = new OracleParameter("pYear", OracleDbType.Decimal, year, ParameterDirection.Input);
            var pPeriod = new OracleParameter("period", OracleDbType.Decimal, period, ParameterDirection.Input);
           
            using (var ctx = new ERPContext())
            {
                try
                {
                    var nhapKiemKe = UnitOfWork.Repository<NvKiemKeChiTiet>()
                        .DbSet.Where(x => x.MaPhieuKiemKe == instance.MaPhieuKiemKe).ToList();
                    foreach (var record in nhapKiemKe)
                    {
                        string id = record.Id;
                        //SoLuongChenhLech < 0 -- NHập kiểm kê
                        if (record.SoLuongChenhLech < 0)
                        {
                            var pId = new OracleParameter("pId", OracleDbType.NVarchar2, id, ParameterDirection.Input);
                            var str = "BEGIN TBNETERP.XNT.XNT_TANGPHIEU_KIEMKE(:pTableName, :year, :period, :pId); END;";
                            ctx.Database.ExecuteSqlCommand(str, pTableName, pYear, pPeriod, pId);
                        }
                        else
                        {
                            var pId = new OracleParameter("pId", OracleDbType.NVarchar2, id, ParameterDirection.Input);
                            var str = "BEGIN TBNETERP.XNT.XNT_GIAMPHIEU_KIEMKE(:pTableName, :year, :period, :pId); END;";
                            ctx.Database.ExecuteSqlCommand(str, pTableName, pYear, pPeriod, pId);
                        }

                    }
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }


        public NvKiemKe InsertPhieu(NvKiemKeVm.Dto instance)
        {
            var unitCode = GetCurrentUnitCode();
            var item = AutoMapper.Mapper.Map<NvKiemKeVm.Dto, NvKiemKe>(instance);
            item.Id = Guid.NewGuid().ToString();
            item.MaDonVi = unitCode;
            item.TrangThai = 20;
            var result = Insert(item);
            var detailData = AutoMapper.Mapper.Map<List<NvKiemKeVm.DtoDetails>, List<NvKiemKeChiTiet>>(instance.DataDetails);

            detailData.ForEach(x => {
                x.Id = Guid.NewGuid().ToString();
                x.MaPhieuKiemKe = item.MaPhieuKiemKe;
                x.SoLuongChenhLech = x.SoLuongTonMay - x.SoLuongKiemKe;
                x.TienTonMay = x.SoLuongTonMay * x.GiaVon;
                x.TienKiemKe = x.SoLuongKiemKe * x.GiaVon;
                x.TienChenhLech = x.TienTonMay - x.TienKiemKe;
                x.MaDonVi = unitCode;
                x.KhoKiemKe = instance.KhoKiemKe;
            });
            UnitOfWork.Repository<NvKiemKeChiTiet>().InsertRange(detailData);
            return result;
        }

        public NvKiemKe UpdatePhieu(NvKiemKeVm.Dto instance)
        {
            var unitCode = GetCurrentUnitCode();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvKiemKeVm.Dto, NvKiemKe>(instance);
            masterData.TrangThai = 30;
            var detailData = Mapper.Map<List<NvKiemKeVm.DtoDetails>, List<NvKiemKeChiTiet>>(instance.DataDetails);
            {
                var detailCollection = UnitOfWork.Repository<NvKiemKeChiTiet>().DbSet.Where(x => x.MaPhieuKiemKe == exsitItem.MaPhieuKiemKe);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            detailData.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaPhieuKiemKe = masterData.MaPhieuKiemKe;
                x.SoLuongChenhLech = x.SoLuongTonMay - x.SoLuongKiemKe;
                x.TienTonMay = x.SoLuongTonMay*x.GiaVon;
                x.TienKiemKe = x.SoLuongKiemKe * x.GiaVon;
                x.TienChenhLech = x.TienTonMay - x.TienKiemKe;
                x.MaDonVi = unitCode;
                x.KhoKiemKe = masterData.KhoKiemKe;
            });
            UnitOfWork.Repository<NvKiemKeChiTiet>().InsertRange(detailData);
            var result = Update(masterData);
            return result;
        }

        public NvKiemKe UpdateApproval(NvKiemKeVm.Dto instance)
        {
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvKiemKeVm.Dto, NvKiemKe>(instance);
            masterData.TrangThai = 10;
            var result = Update(masterData);
            return result;
        }

        protected override Expression<Func<NvKiemKe, bool>> GetKeyFilter(NvKiemKe instance)
        {
            return x => x.MaPhieuKiemKe == instance.MaPhieuKiemKe;
        }
        #region Lastest Version - Kiem Ke
        public MemoryStream ExportExcelTongHop(ParameterKiemKe pi)
        {
            List<NvKiemKeVm.ObjectReport> itemCollectionGroup = new List<NvKiemKeVm.ObjectReport>();
            List<BTS.API.SERVICE.NV.NvKiemKeVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvKiemKeVm.ObjectReport>();

            var titleCotName = "";
            var dieuKienLoc = "";
            switch (pi.ReportType)
            {
                case TypeReportKiemKe.BAOCAOTHUA:
                    dieuKienLoc = "AND NVL(ct.SOLUONGCHENHLECH,0) < 0";
                    titleCotName += "(Báo cáo thừa) ";
                    break;
                case TypeReportKiemKe.BAOCAOTHIEU:
                    dieuKienLoc = "AND NVL(ct.SOLUONGCHENHLECH,0) > 0";
                    break;
                default:
                    dieuKienLoc = " ";
                    titleCotName += "(Báo cáo thiếu) ";
                    break;
            }
            switch (pi.GroupBy)
            {
                case TypeGroupKiemKe.TYPE:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MALOAIVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Loại hàng";
                    break;
                case TypeGroupKiemKe.WAREHOUSE:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MAKHOHANG", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Kho hàng";
                    break;
                case TypeGroupKiemKe.GROUP:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MANHOMVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Nhóm hàng";
                    break;
                case TypeGroupKiemKe.NHACUNGCAP:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MANHACUNGCAP", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Nhà cung cấp";
                    break;
                case TypeGroupKiemKe.KEHANG:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MAKEHANG", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Kệ hàng";
                    break;
                case TypeGroupKiemKe.MERCHANDISE:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MAVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Mặt hàng";
                    break;
                default:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MAVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Mặt hàng";
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
                int startRow = 6;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 10].Merge = true;
                worksheet.Cells[1, 1].Value = "NHẬP XUẤT KIỂM KÊ"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 10].Merge = true;
                worksheet.Cells[3, 1, 3, 10].Merge = true;
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 5, 4].Merge = true;
                worksheet.Cells[4, 5, 5, 5].Merge = true;
                worksheet.Cells[4, 6, 5, 6].Merge = true;
                worksheet.Cells[4, 7, 4, 8].Merge = true;
                worksheet.Cells[4, 9, 4, 10].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: " + titleCotName;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Danh sách"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Giá vốn"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "SL Máy"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "SL KK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Thừa"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "Thiếu"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "SL"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Giá trị"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "SL"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Giá trị"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvKiemKeVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in itemCollectionGroup)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.GiaVon; worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.SoLuongTonMay; 
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.SoLuongKiemKe; 
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.SoLuongThua; 
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.GiaTriThua; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 8].Value = item.SoLuongThieu;
                    worksheet.Cells[currentRow, startColumn + 9].Value = item.GiaTriThieu; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 9].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.GiaVon += item.GiaVon;
                    itemTotal.SoLuongTonMay += item.SoLuongTonMay;
                    itemTotal.SoLuongKiemKe += item.SoLuongKiemKe;
                    itemTotal.SoLuongThua += item.SoLuongThua;
                    itemTotal.GiaTriThua += item.SoLuongThua;
                    itemTotal.SoLuongThieu += item.SoLuongThieu;
                    itemTotal.GiaTriThieu += item.GiaTriThieu;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.GiaVon; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.SoLuongTonMay; 
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.SoLuongKiemKe; 
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.SoLuongThua; 
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.GiaTriThua; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.SoLuongThieu; 
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.GiaTriThieu; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn +9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

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

                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelDetail(ParameterKiemKe pi)
        {
            var itemCollection = new List<NvKiemKeVm.ObjectReportCha>();
            List<NvKiemKeVm.ObjectReportCha> itemCollectionGroup = new List<NvKiemKeVm.ObjectReportCha>();
            List<BTS.API.SERVICE.NV.NvKiemKeVm.ObjectReportCha> data = new List<BTS.API.SERVICE.NV.NvKiemKeVm.ObjectReportCha>();
            var titleCotName = "";
            var dieuKienLoc = "";
            switch (pi.ReportType)
            {
                case TypeReportKiemKe.BAOCAOTHUA:
                    dieuKienLoc = "AND NVL(ct.SOLUONGCHENHLECH,0) < 0";
                    titleCotName += "(Báo cáo thừa) ";
                    break;
                case TypeReportKiemKe.BAOCAOTHIEU:
                    dieuKienLoc = "AND NVL(ct.SOLUONGCHENHLECH,0) > 0";
                    titleCotName += "(Báo cáo thiếu) ";
                    break;
                default:
                    dieuKienLoc = " ";
                    break;
            }
            switch (pi.GroupBy)
            {
                case TypeGroupKiemKe.TYPE:
                    data = ProcedureCollection.KKChiTiet(dieuKienLoc, "MALOAIVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Loại hàng";
                    break;
                case TypeGroupKiemKe.WAREHOUSE:
                    data = ProcedureCollection.KKChiTiet(dieuKienLoc, "MAKHOHANG", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Kho hàng";
                    break;
                case TypeGroupKiemKe.GROUP:
                    data = ProcedureCollection.KKChiTiet(dieuKienLoc, "MANHOMVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Nhóm hàng";
                    break;
                case TypeGroupKiemKe.NHACUNGCAP:
                    data = ProcedureCollection.KKChiTiet(dieuKienLoc, "MANHACUNGCAP", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Nhà cung cấp";
                    break;
                case TypeGroupKiemKe.KEHANG:
                    data = ProcedureCollection.KKChiTiet(dieuKienLoc, "MAKEHANG", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Kệ hàng";
                    break;
                case TypeGroupKiemKe.MERCHANDISE:
                    data = ProcedureCollection.KKChiTiet(dieuKienLoc, "MAVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Mặt hàng";
                    break;
                default:
                    data = ProcedureCollection.KKChiTiet(dieuKienLoc, "MAVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Mặt hàng";
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
                int startRow = 6;
                int startColumn = 1;

                ///Header
                ///
                worksheet.Cells[1, 1, 1, 11].Merge = true;
                worksheet.Cells[1, 1].Value = "NHẬP XUẤT KIỂM KÊ"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 5, 4].Merge = true;
                worksheet.Cells[4, 5, 5, 5].Merge = true;
                worksheet.Cells[4, 6, 5, 6].Merge = true;
                worksheet.Cells[4, 11, 5, 11].Merge = true;
                worksheet.Cells[4, 12, 5, 12].Merge = true;
                worksheet.Cells[4, 7, 4, 8].Merge = true;
                worksheet.Cells[4, 9, 4, 10].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: " + titleCotName;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Danh sách"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Giá vốn"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "SL Máy"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "SL KK"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Thừa"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "Thiếu"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Value = "Ngày KK"; worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 12].Value = "Barcode"; worksheet.Cells[4, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "SL"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Giá trị"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "SL"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Giá trị"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvKiemKeVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;

                foreach (var item in itemCollectionGroup)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 12].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Ma + " - " + item.Ten;
                    currentRow++;
                    foreach (var itemdetails in item.DataDetails)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetails.Ma;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetails.Ten;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetails.GiaVon; worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetails.SoLuongTonMay;
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetails.SoLuongKiemKe;
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetails.SoLuongThua;
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetails.GiaTriThua; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetails.SoLuongThieu;
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetails.GiaTriThieu; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = string.Format("{0}/{1}/{2}",itemdetails.NgayKiemKe.Day,itemdetails.NgayKiemKe.Month,itemdetails.NgayKiemKe.Year);
                        worksheet.Cells[currentRow, startColumn + 11].Value = itemdetails.Barcode;
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 11].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        itemTotal.GiaVon += itemdetails.GiaVon;
                        itemTotal.SoLuongTonMay += itemdetails.SoLuongTonMay;
                        itemTotal.SoLuongKiemKe += itemdetails.SoLuongKiemKe;
                        itemTotal.SoLuongThua += itemdetails.SoLuongThua;
                        itemTotal.GiaTriThua += itemdetails.SoLuongThua;
                        itemTotal.SoLuongThieu += itemdetails.SoLuongThieu;
                        itemTotal.GiaTriThieu += itemdetails.GiaTriThieu;
                        currentRow++;
                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.GiaVon; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.SoLuongTonMay; 
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.SoLuongKiemKe; 
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.SoLuongThua; 
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.GiaTriThua; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.SoLuongThieu; 
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.GiaTriThieu; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn +11].Style.Border.BorderAround(ExcelBorderStyle.Thin);

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
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public List<NvKiemKeVm.ObjectReport> ReportKiemKe(ParameterKiemKe pi)
        {
            List<BTS.API.SERVICE.NV.NvKiemKeVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvKiemKeVm.ObjectReport>();
            var titleCotName = "";
            var dieuKienLoc = "";
            switch (pi.ReportType)
            {
                case TypeReportKiemKe.BAOCAOTHUA:
                    dieuKienLoc = "AND NVL(ct.SOLUONGCHENHLECH,0) < 0";
                    titleCotName += "(Báo cáo thừa) ";
                    break;
                case TypeReportKiemKe.BAOCAOTHIEU:
                    dieuKienLoc = "AND NVL(ct.SOLUONGCHENHLECH,0) > 0";
                    titleCotName += "(Báo cáo thiếu) ";
                    break;
                default:
                    dieuKienLoc = " ";
                    break;
            }
            switch (pi.GroupBy)
            {
                case TypeGroupKiemKe.TYPE:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MALOAIVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Loại hàng";
                    break;
                case TypeGroupKiemKe.WAREHOUSE:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MAKHOHANG", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Kho hàng";
                    break;
                case TypeGroupKiemKe.GROUP:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MANHOMVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Nhóm hàng";
                    break;
                case TypeGroupKiemKe.NHACUNGCAP:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MANHACUNGCAP", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Nhà cung cấp";
                    break;
                case TypeGroupKiemKe.KEHANG:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MAKEHANG", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Kệ hàng";
                    break;
                case TypeGroupKiemKe.MERCHANDISE:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MAVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Mặt hàng";
                    break;
                default:
                    data = ProcedureCollection.KKTongHop(dieuKienLoc, "MAVATTU", pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.KeHangCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
                    titleCotName += "Mặt hàng";
                    break;
            }

            return data;
        }
        #endregion

        public MemoryStream ExportExcelExternalCodeInvertory(List<NvKiemKeVm.ExternalCodeInInventory> pi)
        {
            var ms = new MemoryStream();
            if (pi != null)
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    package.Workbook.Worksheets.Add("Data");
                    var worksheet = package.Workbook.Worksheets[1];
                    int startRow = 3;
                    int startColumn = 1;

                    worksheet.Cells[1, 1, 1, 18].Merge = true;
                    worksheet.Cells[1, 1].Value = "Mã hàng chưa kiểm kê";
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 1].Value = string.Format("Ngày tạo: {0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 1, 2, 18].Merge = true;
                    worksheet.Cells[3, 1].Value = "STT";
                    worksheet.Cells[3, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 2].Value = "Mã vật tư";
                    worksheet.Cells[3, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 3].Value = "Tên vật tư";
                    worksheet.Cells[3, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 4].Value = "Barcode";
                    worksheet.Cells[3, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 5].Value = "Mã kệ";
                    worksheet.Cells[3, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 6].Value = "Mã loại";
                    worksheet.Cells[3, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 7].Value = "Mã nhóm";
                    worksheet.Cells[3, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 8].Value = "NCC";
                    worksheet.Cells[3, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 9].Value = "Mã kho";
                    worksheet.Cells[3, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 10].Value = "Tồn đầu SL";
                    worksheet.Cells[3, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 11].Value = "Tồn đầu GT";
                    worksheet.Cells[3, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 12].Value = "Nhập SL";
                    worksheet.Cells[3, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 13].Value = "Nhập GT";
                    worksheet.Cells[3, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 14].Value = "Xuất SL";
                    worksheet.Cells[3, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 15].Value = "Xuất GT";
                    worksheet.Cells[3, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 16].Value = "Tồn cuối SL";
                    worksheet.Cells[3, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 17].Value = "Tồn cuối GT";
                    worksheet.Cells[3, 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 18].Value = "Giá vốn";
                    worksheet.Cells[3, 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    int currentRow = startRow;
                    int stt = 0;

                    foreach (var item in pi)
                    {
                        currentRow++;
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = item.MaVatTu;
                        worksheet.Cells[currentRow, startColumn + 2].Value = item.TenVatTu;
                        worksheet.Cells[currentRow, startColumn + 3].Value = item.Barcode;
                        worksheet.Cells[currentRow, startColumn + 4].Value = item.MaKeHang;
                        worksheet.Cells[currentRow, startColumn + 5].Value = item.MaLoaiVatTu;
                        worksheet.Cells[currentRow, startColumn + 6].Value = item.MaNhomVatTu;
                        worksheet.Cells[currentRow, startColumn + 7].Value = item.MaKhachHang;
                        worksheet.Cells[currentRow, startColumn + 8].Value = item.MaKho;
                        worksheet.Cells[currentRow, startColumn + 9].Value = item.TonDauKySl;
                        worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = item.TonDauKyGt;
                        worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 11].Value = item.NhapSl;
                        worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 12].Value = item.NhapGt;
                        worksheet.Cells[currentRow, 13].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 13].Value = item.XuatSl;
                        worksheet.Cells[currentRow, 14].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 14].Value = item.XuatGt;
                        worksheet.Cells[currentRow, 15].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 15].Value = item.TonCuoiKySl;
                        worksheet.Cells[currentRow, 16].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 16].Value = item.TonCuoiKyGt;
                        worksheet.Cells[currentRow, 17].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 17].Value = item.GiaVon;
                        worksheet.Cells[currentRow, 18].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 17].Style.Border.BorderAround(
                            ExcelBorderStyle.Dotted);
                    }
                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    //worksheet.Column(4).AutoFit();
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
                    worksheet.Column(16).AutoFit();
                    worksheet.Column(17).AutoFit();
                    worksheet.Column(18).AutoFit();
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalCols = worksheet.Dimension.End.Column;
                    var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                    var dataFont = dataCells.Style.Font;
                    dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 14));
                    package.SaveAs(ms);
                    return ms;
                }
            }
            else
            {
                return null;
            }
        }

    }
}
