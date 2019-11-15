using BTS.API.ENTITY.DCL;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using BTS.API.ENTITY;
using System.Data.Common;
using Oracle.ManagedDataAccess.Types;
using System.IO;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using BTS.API.SERVICE.NV;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using BTS.API.ENTITY.Md;

namespace BTS.API.SERVICE.DCL
{
    public interface IXuatNhapTonChiTietService : IDetailInfoServiceBase<DclCloseout>
    {
        List<InventoryDetailItem> CreateReportXNTNewTongHop(ParameterInventory pi);
        MemoryStream ExportExcelXNTNewTongHop(ParameterInventory pi);
        List<InventoryDetailItemCha> CreateReportXNTNewChiTiet(ParameterInventory pi);
        MemoryStream ExportExcelXNTNewChiTiet(ParameterInventory pi);
        string GetTableName(int year, int period);
    }
    public class XuatNhapTonChiTietService : DetailInfoServiceBase<DclCloseout>, IXuatNhapTonChiTietService
    {
        public XuatNhapTonChiTietService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public List<InventoryDetailItem> CreateReportXNTNewTongHop(ParameterInventory pi)
        {
            List<InventoryDetailItem> data = new List<InventoryDetailItem>();
            DateTime beginDay, endDay;
            string ky = string.Empty;
            beginDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 0, 0, 0);
            endDay = new DateTime(pi.ToDate.Year, pi.ToDate.Month, pi.ToDate.Day, 23, 59, 59);

            var period = UnitOfWork.Repository<MdPeriod>().DbSet.FirstOrDefault(x => x.FromDate >= beginDay && x.FromDate <= endDay);
            if (period != null)
            {
                ky = ProcedureCollection.GetTableName(period.Year, period.Period);
            }
            switch (pi.GroupBy)
            {
                case TypeGroupInventory.TYPE:
                    data = ProcedureCollection.ReportXNTNew_TongHop(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MALOAIVATTU.ToString(), pi.NhaCungCapCodes);
                    break;
                case TypeGroupInventory.GROUP:
                    data = ProcedureCollection.ReportXNTNew_TongHop(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MANHOMVATTU.ToString(), pi.NhaCungCapCodes);
                    break;
                case TypeGroupInventory.NHACUNGCAP:
                    data = ProcedureCollection.ReportXNTNew_TongHop(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MANHACUNGCAP.ToString(), pi.NhaCungCapCodes);
                    break;
                case TypeGroupInventory.WAREHOUSE:
                    data = ProcedureCollection.ReportXNTNew_TongHop(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MAKHO.ToString(), pi.NhaCungCapCodes);
                    break;
                default:
                    data = ProcedureCollection.ReportXNTNew_TongHop(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MAVATTU.ToString(), pi.NhaCungCapCodes);
                    break;
            }
            return data;
        }

        public MemoryStream ExportExcelXNTNewTongHop(ParameterInventory para)
        {
            var pi = CreateReportXNTNewTongHop(para);
            string groupType = string.Empty;
            switch (para.GroupBy)
            {
                case TypeGroupInventory.GROUP:
                    groupType = "Nhóm vật tư";
                    break;
                case TypeGroupInventory.MERCHANDISE:
                    groupType = "Vật tư";
                    break;
                case TypeGroupInventory.NHACUNGCAP:
                    groupType = "Nhà cung cấp";
                    break;
                case TypeGroupInventory.TYPE:
                    groupType = "Loại vật tư";
                    break;
                case TypeGroupInventory.WAREHOUSE:
                    groupType = "Kho vật tư";
                    break;
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = para.FromDate.Day + "/" + para.FromDate.Month + "/" + para.FromDate.Year;
                string toDateFomart = para.ToDate.Day + "/" + para.ToDate.Month + "/" + para.ToDate.Year;
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;

                worksheet.Cells[1, 1, 1, 26].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo xuất nhập tồn chi tiết theo " + groupType; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 26].Merge = true;
                worksheet.Cells[3, 1, 3, 26].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + groupType;
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 5, 4].Merge = true;

                worksheet.Cells[4, 5, 4, 6].Merge = true;
                worksheet.Cells[4, 7, 4, 8].Merge = true;
                worksheet.Cells[4, 9, 4, 10].Merge = true;
                worksheet.Cells[4, 11, 4, 12].Merge = true;
                worksheet.Cells[4, 13, 4, 14].Merge = true;
                worksheet.Cells[4, 15, 4, 16].Merge = true;
                worksheet.Cells[4, 17, 4, 18].Merge = true;
                worksheet.Cells[4, 19, 4, 20].Merge = true;
                worksheet.Cells[4, 21, 4, 22].Merge = true;
                worksheet.Cells[4, 23, 4, 24].Merge = true;
                worksheet.Cells[4, 25, 4, 26].Merge = true;
                worksheet.Cells[4, 27, 4, 28].Merge = true;
                worksheet.Cells[4, 29, 4, 30].Merge = true;
                worksheet.Cells[4, 31, 4, 32].Merge = true;
                worksheet.Cells[4, 33, 4, 34].Merge = true;
                worksheet.Cells[4, 35, 4, 36].Merge = true;
                worksheet.Cells[4, 37, 4, 38].Merge = true;
                worksheet.Cells[4, 39, 4, 40].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1, 5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                switch (para.GroupBy)
                {
                    case TypeGroupInventory.GROUP:
                        worksheet.Cells[4, 2].Value = "Nhóm vật tư"; worksheet.Cells[4, 2, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Tên nhóm"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                    case TypeGroupInventory.MERCHANDISE:
                        worksheet.Cells[4, 2].Value = "Mã vật tư"; worksheet.Cells[4, 2, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Tên vật tư"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                    case TypeGroupInventory.NHACUNGCAP:
                        worksheet.Cells[4, 2].Value = "Mã nhà cung cấp"; worksheet.Cells[4, 2, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Tên nhà cung cấp"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                    case TypeGroupInventory.TYPE:
                        worksheet.Cells[4, 2].Value = "Loại vật tư"; worksheet.Cells[4, 2, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Tên loại"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                    case TypeGroupInventory.WAREHOUSE:
                        worksheet.Cells[4, 2].Value = "Kho vật tư"; worksheet.Cells[4, 2, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Tên kho"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                }
                worksheet.Cells[4, 5].Value = "Tồn đầu kỳ"; worksheet.Cells[4, 5, 4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Nhập mua"; worksheet.Cells[4, 7, 4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "Nhập bán lẻ TL"; worksheet.Cells[4, 9, 4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Value = "Nhập chuyển kho"; worksheet.Cells[4, 11, 4, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 13].Value = "Nhập ST thành viên"; worksheet.Cells[4, 13, 4, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 15].Value = "Xuất ST thành viên"; worksheet.Cells[4, 15, 4, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 17].Value = "Nhập bán buôn TL"; worksheet.Cells[4, 17, 4, 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 19].Value = "Nhập điều chỉnh"; worksheet.Cells[4, 19, 4, 20].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 21].Value = "Nhập hàng âm"; worksheet.Cells[4, 21, 4, 22].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 23].Value = "Xuất bán lẻ"; worksheet.Cells[4, 23, 4, 24].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 25].Value = "Xuất bán buôn"; worksheet.Cells[4, 25, 4, 26].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 27].Value = "Xuất chuyển kho"; worksheet.Cells[4, 27, 4, 28].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 29].Value = "Xuất điều chỉnh"; worksheet.Cells[4, 29, 4, 30].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 31].Value = "Xuất trả NCC"; worksheet.Cells[4, 31, 4, 32].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 33].Value = "Xuất hủy hàng hỏng"; worksheet.Cells[4, 33, 4, 34].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 35].Value = "Nhập kiểm kê"; worksheet.Cells[4, 35, 4, 36].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 37].Value = "Xuất kiểm kê"; worksheet.Cells[4, 37, 4, 38].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 39].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 39, 4, 40].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 5].Value = "Số lượng"; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Giá trị"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Số lượng"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Giá trị"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "Số lượng"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Giá trị"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Value = "Số lượng"; worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 12].Value = "Giá trị"; worksheet.Cells[5, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 13].Value = "Số lượng"; worksheet.Cells[5, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 14].Value = "Giá trị"; worksheet.Cells[5, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 15].Value = "Số lượng"; worksheet.Cells[5, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 16].Value = "Giá trị"; worksheet.Cells[5, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 17].Value = "Số lượng"; worksheet.Cells[5, 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 18].Value = "Giá trị"; worksheet.Cells[5, 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 19].Value = "Số lượng"; worksheet.Cells[5, 19].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 20].Value = "Giá trị"; worksheet.Cells[5, 20].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 21].Value = "Số lượng"; worksheet.Cells[5, 21].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 22].Value = "Giá trị"; worksheet.Cells[5, 22].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 23].Value = "Số lượng"; worksheet.Cells[5, 23].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 24].Value = "Giá trị"; worksheet.Cells[5, 24].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 25].Value = "Số lượng"; worksheet.Cells[5, 25].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 26].Value = "Giá trị"; worksheet.Cells[5, 26].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 27].Value = "Số lượng"; worksheet.Cells[5, 27].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 28].Value = "Giá trị"; worksheet.Cells[5, 28].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 29].Value = "Số lượng"; worksheet.Cells[5, 29].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 30].Value = "Giá trị"; worksheet.Cells[5, 30].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 31].Value = "Số lượng"; worksheet.Cells[5, 31].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 32].Value = "Giá trị"; worksheet.Cells[5, 32].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 33].Value = "Số lượng"; worksheet.Cells[5, 33].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 34].Value = "Giá trị"; worksheet.Cells[5, 34].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 35].Value = "Số lượng"; worksheet.Cells[5, 35].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 36].Value = "Giá trị"; worksheet.Cells[5, 36].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 37].Value = "Số lượng"; worksheet.Cells[5, 37].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 38].Value = "Giá trị"; worksheet.Cells[5, 38].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 39].Value = "Số lượng"; worksheet.Cells[5, 39].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 40].Value = "Giá trị"; worksheet.Cells[5, 40].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                int currentRow = startRow;
                int stt = 0;
                foreach (var itemdetail in pi)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Ma;
                    worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Ten;
                    worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.TonDauKy_Sl;
                    worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.TonDauKy_Gt;
                    worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.Nmua_Sl;
                    worksheet.Cells[currentRow, startColumn + 7].Value = itemdetail.Nmua_Gt;
                    worksheet.Cells[currentRow, startColumn + 8].Value = itemdetail.XBanLeTL_Sl;
                    worksheet.Cells[currentRow, startColumn + 9].Value = itemdetail.XBanLeTL_Gt;
                    worksheet.Cells[currentRow, startColumn + 10].Value = itemdetail.NhapChuyenKho_Sl;
                    worksheet.Cells[currentRow, startColumn + 11].Value = itemdetail.NhapChuyenKho_Gt;
                    worksheet.Cells[currentRow, startColumn + 12].Value = itemdetail.NhapSTThanhVien_Sl;
                    worksheet.Cells[currentRow, startColumn + 13].Value = itemdetail.NhapSTThanhVien_Gt;
                    worksheet.Cells[currentRow, startColumn + 14].Value = itemdetail.XuatSTThanhVien_Sl;
                    worksheet.Cells[currentRow, startColumn + 15].Value = itemdetail.XuatSTThanhVien_Gt;
                    worksheet.Cells[currentRow, startColumn + 16].Value = itemdetail.NhapBanTL_Sl;
                    worksheet.Cells[currentRow, startColumn + 17].Value = itemdetail.NhapBanTL_Gt;
                    worksheet.Cells[currentRow, startColumn + 18].Value = itemdetail.NhapDieuChinh_Sl;
                    worksheet.Cells[currentRow, startColumn + 19].Value = itemdetail.NhapDieuChinh_Gt;
                    worksheet.Cells[currentRow, startColumn + 20].Value = itemdetail.NhapHangAm_Sl;
                    worksheet.Cells[currentRow, startColumn + 21].Value = itemdetail.NhapHangAm_Gt;
                    worksheet.Cells[currentRow, startColumn + 22].Value = itemdetail.XBanLeQuay_Sl;
                    worksheet.Cells[currentRow, startColumn + 23].Value = itemdetail.XBanLeQuay_Gt;
                    worksheet.Cells[currentRow, startColumn + 24].Value = itemdetail.XBanBuon_Sl;
                    worksheet.Cells[currentRow, startColumn + 25].Value = itemdetail.XBanBuon_Gt;
                    worksheet.Cells[currentRow, startColumn + 26].Value = itemdetail.XuatChuyenKho_Sl;
                    worksheet.Cells[currentRow, startColumn + 27].Value = itemdetail.XuatChuyenKho_Gt;
                    worksheet.Cells[currentRow, startColumn + 28].Value = itemdetail.XuatDC_Sl;
                    worksheet.Cells[currentRow, startColumn + 29].Value = itemdetail.XuatDC_Gt;
                    worksheet.Cells[currentRow, startColumn + 30].Value = itemdetail.XuatTraNCC_Sl;
                    worksheet.Cells[currentRow, startColumn + 31].Value = itemdetail.XuatTraNCC_Gt;
                    worksheet.Cells[currentRow, startColumn + 32].Value = itemdetail.XuatHuyHH_Sl;
                    worksheet.Cells[currentRow, startColumn + 33].Value = itemdetail.XuatHuyHH_Gt;
                    worksheet.Cells[currentRow, startColumn + 34].Value = itemdetail.NhapKiemKe_Sl;
                    worksheet.Cells[currentRow, startColumn + 35].Value = itemdetail.NhapKiemKe_Gt;
                    worksheet.Cells[currentRow, startColumn + 36].Value = itemdetail.XuatKiemKe_Sl;
                    worksheet.Cells[currentRow, startColumn + 37].Value = itemdetail.XuatKiemKe_Gt;
                    worksheet.Cells[currentRow, startColumn + 38].Value = itemdetail.TonCuoiKy_Sl;
                    worksheet.Cells[currentRow, startColumn + 39].Value = itemdetail.TonCuoiKy_Gt;
                    currentRow++;
                }
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
                worksheet.Column(16).AutoFit();
                worksheet.Column(17).AutoFit();
                worksheet.Column(18).AutoFit();
                worksheet.Column(19).AutoFit();
                worksheet.Column(20).AutoFit();
                worksheet.Column(21).AutoFit();
                worksheet.Column(22).AutoFit();
                worksheet.Column(23).AutoFit();
                worksheet.Column(24).AutoFit();
                worksheet.Column(25).AutoFit();
                worksheet.Column(26).AutoFit();
                worksheet.Column(27).AutoFit();
                worksheet.Column(28).AutoFit();
                worksheet.Column(29).AutoFit();
                worksheet.Column(30).AutoFit();
                worksheet.Column(31).AutoFit();
                worksheet.Column(32).AutoFit();
                worksheet.Column(33).AutoFit();
                worksheet.Column(34).AutoFit();
                worksheet.Column(35).AutoFit();
                worksheet.Column(36).AutoFit();
                worksheet.Column(37).AutoFit();
                worksheet.Column(38).AutoFit();
                worksheet.Column(39).AutoFit();

                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;

            }
        }

        public List<InventoryDetailItemCha> CreateReportXNTNewChiTiet(ParameterInventory pi)
        {
            List<InventoryDetailItemCha> data = new List<InventoryDetailItemCha>();
            DateTime beginDay, endDay;
            string ky = string.Empty;
            beginDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 0, 0, 0);
            endDay = new DateTime(pi.ToDate.Year, pi.ToDate.Month, pi.ToDate.Day, 23, 59, 59);

            var period = UnitOfWork.Repository<MdPeriod>().DbSet.FirstOrDefault(x => x.FromDate >= beginDay && x.FromDate <= endDay);
            if (period != null)
            {
                ky = ProcedureCollection.GetTableName(period.Year, period.Period);
            }
            switch (pi.GroupBy)
            {
                case TypeGroupInventory.TYPE:
                    data = ProcedureCollection.ReportXNTNew_ChiTiet(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MALOAIVATTU.ToString(), pi.NhaCungCapCodes);
                    break;
                case TypeGroupInventory.GROUP:
                    data = ProcedureCollection.ReportXNTNew_ChiTiet(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MANHOMVATTU.ToString(), pi.NhaCungCapCodes);
                    break;
                case TypeGroupInventory.NHACUNGCAP:
                    data = ProcedureCollection.ReportXNTNew_ChiTiet(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MANHACUNGCAP.ToString(), pi.NhaCungCapCodes);
                    break;
                case TypeGroupInventory.WAREHOUSE:
                    data = ProcedureCollection.ReportXNTNew_ChiTiet(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MAKHO.ToString(), pi.NhaCungCapCodes);
                    break;
                default:
                    data = ProcedureCollection.ReportXNTNew_ChiTiet(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, InventoryGroupBy.MAVATTU.ToString(), pi.NhaCungCapCodes);
                    break;
            }
            return data;
        }

        public MemoryStream ExportExcelXNTNewChiTiet(ParameterInventory para)
        {
            var pi = CreateReportXNTNewChiTiet(para);
            string groupType = string.Empty;
            switch (para.GroupBy)
            {
                case TypeGroupInventory.GROUP:
                    groupType = "Nhóm vật tư";
                    break;
                case TypeGroupInventory.MERCHANDISE:
                    groupType = "Vật tư";
                    break;
                case TypeGroupInventory.NHACUNGCAP:
                    groupType = "Nhà cung cấp";
                    break;
                case TypeGroupInventory.TYPE:
                    groupType = "Loại vật tư";
                    break;
                case TypeGroupInventory.WAREHOUSE:
                    groupType = "Kho vật tư";
                    break;
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = para.FromDate.Day + "/" + para.FromDate.Month + "/" + para.FromDate.Year;
                string toDateFomart = para.ToDate.Day + "/" + para.ToDate.Month + "/" + para.ToDate.Year;
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;

                worksheet.Cells[1, 1, 1, 26].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo xuất nhập tồn chi tiết theo " + groupType; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 26].Merge = true;
                worksheet.Cells[3, 1, 3, 26].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + groupType;
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 5, 4].Merge = true;

                worksheet.Cells[4, 5, 4, 6].Merge = true;
                worksheet.Cells[4, 7, 4, 8].Merge = true;
                worksheet.Cells[4, 9, 4, 10].Merge = true;
                worksheet.Cells[4, 11, 4, 12].Merge = true;
                worksheet.Cells[4, 13, 4, 14].Merge = true;
                worksheet.Cells[4, 15, 4, 16].Merge = true;
                worksheet.Cells[4, 17, 4, 18].Merge = true;
                worksheet.Cells[4, 19, 4, 20].Merge = true;
                worksheet.Cells[4, 21, 4, 22].Merge = true;
                worksheet.Cells[4, 23, 4, 24].Merge = true;
                worksheet.Cells[4, 25, 4, 26].Merge = true;
                worksheet.Cells[4, 27, 4, 28].Merge = true;
                worksheet.Cells[4, 29, 4, 30].Merge = true;
                worksheet.Cells[4, 31, 4, 32].Merge = true;
                worksheet.Cells[4, 33, 4, 34].Merge = true;
                worksheet.Cells[4, 35, 4, 36].Merge = true;
                worksheet.Cells[4, 37, 4, 38].Merge = true;
                worksheet.Cells[4, 39, 4, 40].Merge = true;



                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1, 5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                switch (para.GroupBy)
                {
                    case TypeGroupInventory.GROUP:
                        worksheet.Cells[4, 2].Value = "Mã nhóm vật tư"; worksheet.Cells[4, 2, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Mã vật tư"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 4].Value = "Tên vật tư"; worksheet.Cells[4, 4, 5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                    case TypeGroupInventory.MERCHANDISE:
                        worksheet.Cells[4, 2].Value = "Mã vật tư"; worksheet.Cells[4, 2, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Mã vật tư"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 4].Value = "Tên vật tư"; worksheet.Cells[4, 4, 5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                    case TypeGroupInventory.NHACUNGCAP:
                        worksheet.Cells[4, 2].Value = "Mã nhà cung cấp"; worksheet.Cells[4, 2, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Mã vật tư"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 4].Value = "Tên vật tư"; worksheet.Cells[4, 4, 5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                    case TypeGroupInventory.TYPE:
                        worksheet.Cells[4, 2].Value = "Mã loại vật tư"; worksheet.Cells[4, 2, 5,2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Mã vật tư"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 4].Value = "Tên vật tư"; worksheet.Cells[4, 4, 5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                    case TypeGroupInventory.WAREHOUSE:
                        worksheet.Cells[4, 2].Value = "Mã kho"; worksheet.Cells[4, 2, 5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 3].Value = "Mã vật tư"; worksheet.Cells[4, 3, 5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[4, 4].Value = "Tên vật tư"; worksheet.Cells[4, 4, 5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        break;
                }
                
                worksheet.Cells[4, 5].Value = "Tồn đầu kỳ"; worksheet.Cells[4, 5, 4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Nhập mua"; worksheet.Cells[4, 7, 4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "Nhập bán lẻ TL"; worksheet.Cells[4, 9, 4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Value = "Nhập chuyển kho"; worksheet.Cells[4, 11, 4, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 13].Value = "Nhập ST thành viên"; worksheet.Cells[4, 13, 4, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 15].Value = "Xuất ST thành viên"; worksheet.Cells[4, 15, 4, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 17].Value = "Nhập bán buôn TL"; worksheet.Cells[4, 17, 4, 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 19].Value = "Nhập điều chỉnh"; worksheet.Cells[4, 19, 4, 20].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 21].Value = "Nhập hàng âm"; worksheet.Cells[4, 21, 4, 22].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 23].Value = "Xuất bán lẻ"; worksheet.Cells[4, 23, 4, 24].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 25].Value = "Xuất bán buôn"; worksheet.Cells[4, 25, 4, 26].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 27].Value = "Xuất chuyển kho"; worksheet.Cells[4, 27, 4, 28].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 29].Value = "Xuất điều chỉnh"; worksheet.Cells[4, 29, 4, 30].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 31].Value = "Xuất trả NCC"; worksheet.Cells[4, 31, 4, 32].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 33].Value = "Xuất hủy hàng hỏng"; worksheet.Cells[4, 33, 4, 34].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 35].Value = "Nhập kiểm kê"; worksheet.Cells[4, 35, 4, 36].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 37].Value = "Xuất kiểm kê"; worksheet.Cells[4, 37, 4, 38].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 39].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 39, 4, 40].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 5].Value = "Số lượng"; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Giá trị"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Số lượng"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Giá trị"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "Số lượng"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Giá trị"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Value = "Số lượng"; worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 12].Value = "Giá trị"; worksheet.Cells[5, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 13].Value = "Số lượng"; worksheet.Cells[5, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 14].Value = "Giá trị"; worksheet.Cells[5, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 15].Value = "Số lượng"; worksheet.Cells[5, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 16].Value = "Giá trị"; worksheet.Cells[5, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 17].Value = "Số lượng"; worksheet.Cells[5, 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 18].Value = "Giá trị"; worksheet.Cells[5, 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 19].Value = "Số lượng"; worksheet.Cells[5, 19].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 20].Value = "Giá trị"; worksheet.Cells[5, 20].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 21].Value = "Số lượng"; worksheet.Cells[5, 21].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 22].Value = "Giá trị"; worksheet.Cells[5, 22].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 23].Value = "Số lượng"; worksheet.Cells[5, 23].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 24].Value = "Giá trị"; worksheet.Cells[5, 24].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 25].Value = "Số lượng"; worksheet.Cells[5, 25].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 26].Value = "Giá trị"; worksheet.Cells[5, 26].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 27].Value = "Số lượng"; worksheet.Cells[5, 27].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 28].Value = "Giá trị"; worksheet.Cells[5, 28].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 29].Value = "Số lượng"; worksheet.Cells[5, 29].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 30].Value = "Giá trị"; worksheet.Cells[5, 30].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 31].Value = "Số lượng"; worksheet.Cells[5, 31].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 32].Value = "Giá trị"; worksheet.Cells[5, 32].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 33].Value = "Số lượng"; worksheet.Cells[5, 33].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 34].Value = "Giá trị"; worksheet.Cells[5, 34].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 35].Value = "Số lượng"; worksheet.Cells[5, 35].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 36].Value = "Giá trị"; worksheet.Cells[5, 36].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 37].Value = "Số lượng"; worksheet.Cells[5, 37].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 38].Value = "Giá trị"; worksheet.Cells[5, 38].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 39].Value = "Số lượng"; worksheet.Cells[5, 39].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 40].Value = "Giá trị"; worksheet.Cells[5, 40].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                int currentRow = startRow;
                int stt = 0;
                foreach (var item in pi)
                {
                    worksheet.Cells[currentRow, 1, currentRow, 40].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Ma + " - " + item.Ten;
                    currentRow++;
                    stt = 0;
                    foreach (var itemdetail in item.DataDetails)
                    {
                        stt++;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.MaCha;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Ten;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.Ma;
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.TonDauKy_Sl;
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.TonDauKy_Gt;
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.Nmua_Sl;
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetail.Nmua_Gt;
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetail.XBanLeTL_Sl;
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetail.XBanLeTL_Gt;
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetail.NhapChuyenKho_Sl;
                        worksheet.Cells[currentRow, startColumn + 11].Value = itemdetail.NhapChuyenKho_Gt;
                        worksheet.Cells[currentRow, startColumn + 12].Value = itemdetail.NhapSTThanhVien_Sl;
                        worksheet.Cells[currentRow, startColumn + 13].Value = itemdetail.NhapSTThanhVien_Gt;
                        worksheet.Cells[currentRow, startColumn + 14].Value = itemdetail.XuatSTThanhVien_Sl;
                        worksheet.Cells[currentRow, startColumn + 15].Value = itemdetail.XuatSTThanhVien_Gt;
                        worksheet.Cells[currentRow, startColumn + 16].Value = itemdetail.NhapBanTL_Sl;
                        worksheet.Cells[currentRow, startColumn + 17].Value = itemdetail.NhapBanTL_Gt;
                        worksheet.Cells[currentRow, startColumn + 18].Value = itemdetail.NhapDieuChinh_Sl;
                        worksheet.Cells[currentRow, startColumn + 19].Value = itemdetail.NhapDieuChinh_Gt;
                        worksheet.Cells[currentRow, startColumn + 20].Value = itemdetail.NhapHangAm_Sl;
                        worksheet.Cells[currentRow, startColumn + 21].Value = itemdetail.NhapHangAm_Gt;
                        worksheet.Cells[currentRow, startColumn + 22].Value = itemdetail.XBanLeQuay_Sl;
                        worksheet.Cells[currentRow, startColumn + 23].Value = itemdetail.XBanLeQuay_Gt;
                        worksheet.Cells[currentRow, startColumn + 24].Value = itemdetail.XBanBuon_Sl;
                        worksheet.Cells[currentRow, startColumn + 25].Value = itemdetail.XBanBuon_Gt;
                        worksheet.Cells[currentRow, startColumn + 26].Value = itemdetail.XuatChuyenKho_Sl;
                        worksheet.Cells[currentRow, startColumn + 27].Value = itemdetail.XuatChuyenKho_Gt;
                        worksheet.Cells[currentRow, startColumn + 28].Value = itemdetail.XuatDC_Sl;
                        worksheet.Cells[currentRow, startColumn + 29].Value = itemdetail.XuatDC_Gt;
                        worksheet.Cells[currentRow, startColumn + 30].Value = itemdetail.XuatTraNCC_Sl;
                        worksheet.Cells[currentRow, startColumn + 31].Value = itemdetail.XuatTraNCC_Gt;
                        worksheet.Cells[currentRow, startColumn + 32].Value = itemdetail.XuatHuyHH_Sl;
                        worksheet.Cells[currentRow, startColumn + 33].Value = itemdetail.XuatHuyHH_Gt;
                        worksheet.Cells[currentRow, startColumn + 34].Value = itemdetail.NhapKiemKe_Sl;
                        worksheet.Cells[currentRow, startColumn + 35].Value = itemdetail.NhapKiemKe_Gt;
                        worksheet.Cells[currentRow, startColumn + 36].Value = itemdetail.XuatKiemKe_Sl;
                        worksheet.Cells[currentRow, startColumn + 37].Value = itemdetail.XuatKiemKe_Gt;
                        worksheet.Cells[currentRow, startColumn + 38].Value = itemdetail.TonCuoiKy_Sl;
                        worksheet.Cells[currentRow, startColumn + 39].Value = itemdetail.TonCuoiKy_Gt;
                        currentRow++;
                    }
                }
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
                worksheet.Column(16).AutoFit();
                worksheet.Column(17).AutoFit();
                worksheet.Column(18).AutoFit();
                worksheet.Column(19).AutoFit();
                worksheet.Column(20).AutoFit();
                worksheet.Column(21).AutoFit();
                worksheet.Column(22).AutoFit();
                worksheet.Column(23).AutoFit();
                worksheet.Column(24).AutoFit();
                worksheet.Column(25).AutoFit();
                worksheet.Column(26).AutoFit();
                worksheet.Column(27).AutoFit();
                worksheet.Column(28).AutoFit();
                worksheet.Column(29).AutoFit();
                worksheet.Column(30).AutoFit();
                worksheet.Column(31).AutoFit();
                worksheet.Column(32).AutoFit();
                worksheet.Column(33).AutoFit();
                worksheet.Column(34).AutoFit();
                worksheet.Column(35).AutoFit();
                worksheet.Column(36).AutoFit();
                worksheet.Column(37).AutoFit();
                worksheet.Column(38).AutoFit();
                worksheet.Column(39).AutoFit();

                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;

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
        private string _convertToArrayConditionDetail(string str, List<InventoryExcel> result)
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
                InventoryExcel model = new InventoryExcel();
                resultArray[i] = "'" + subStrAray[i] + "'";
                model.Code = subStrAray[i];
                result.Add(model);
            }
            return String.Join(",", resultArray);
        }
        public string GetTableName(int year, int period)
        {
            return string.Format("XNT_{0}_KY_{1}", year, period);
        }

    }
}
