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
using BTS.API.ENTITY.Authorize;

namespace BTS.API.SERVICE.DCL
{
    public interface IDclCloseoutService : IDetailInfoServiceBase<DclCloseout>
    {
        void ProcedureCloseInventory(string preTableName, string nextTableName, string unitCode, int year, int period);
        void CreateTableXNT_KhoaSo(string preTableName, string tableName, string unitCode, int year, int period);
        List<InventoryExpImp> CreateReportInventoryByPeriod(string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, int year, int period);
        List<InventoryExpImp> CreateReportInventoryByDay(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<CashierVm> CreateReportCashierByStaff(DateTime fromDate, DateTime toDate, string unitCode);
        #region OLD VERSION
        List<InventoryExpImp> CreateReportIventoryItemByWareHouse(DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<InventoryExpImp> CreateReportIventoryItemByType(DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<InventoryExpImp> CreateReportIventoryItemByGroup(DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<InventoryExpImp> CreateReportIventoryItemByMerchandise(DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<InventoryExpImp> CreateReportInventoryByCustomer(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        List<InventoryExpImp> CreateReportInventoryByWareHouse(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<InventoryExpImp> CreateReportInventoryByType(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<InventoryExpImp> CreateReportInventoryByGroup(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<InventoryExpImp> CreateReportInventoryByMerchandise(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<InventoryExpImp> CreateReportInventoryByMerchandiseByNCC(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        List<InventoryExcel> CreateReportInventoryByMerchandiseByNCCDetail(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        List<InventoryExcel> CreateReportInventoryByMerchandiseByMerchandiseTypeDetail(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        List<InventoryExcel> CreateReportInventoryByMerchandiseByMerchandiseGroupDetail(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        MemoryStream ExportExcelXNTByMerchandise(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes);
        MemoryStream ExportExcelXNTByMerchandiseByNCC(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        MemoryStream ExportExcelXNTByMerchandiseType(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        MemoryStream ExportExcelXNTByMerchandiseGroup(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        #endregion
        //Report XNT TỔng hợp
        List<InventoryExpImpLevel2> CreateReportInventoryTongHop(string groupBy, DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes);
        //Excel XNT Chi tiết
        MemoryStream ExportExcelXNTDetail(ParameterInventory pi);
        //Excel XNT Tổng hợp
        MemoryStream ExportExcelXNTTongHop(InventoryReport pi);

        MemoryStream ExcelXNTExcelPTA(InventoryReportExcel pi);
        //Report Tồn TỔng hợp
        List<InventoryExpImpLevel2> ReportTonTongHop(ParameterInventory pi);
        //Excel Tồn Chi tiết
        MemoryStream ExportExcelTonChiTiet(ParameterInventory pi);
        //Excel Tồn Tổng hợp
        MemoryStream ExportExcelTonTongHop(InventoryReport pi);
    }
    public class DclCloseoutService : DetailInfoServiceBase<DclCloseout>, IDclCloseoutService
    {
        public DclCloseoutService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        protected override Expression<Func<DclCloseout, bool>> GetKeyFilter(DclCloseout instance)
        {
            return x => x.MaKhoaSo == instance.MaKhoaSo;
        }
        public void CreateTableXNT_KhoaSo(string preTableName, string tableName, string unitCode, int year, int period)
        {
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {

                        var pPreTableName = new OracleParameter("pPreTableName", OracleDbType.NVarchar2, preTableName, ParameterDirection.Input);
                        var ptableName = new OracleParameter("pNextTableName", OracleDbType.NVarchar2, tableName, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pYear = new OracleParameter("pYear", OracleDbType.Decimal, year, ParameterDirection.Input);
                        var pPeriod = new OracleParameter("period", OracleDbType.Decimal, period, ParameterDirection.Input);
                        var str = "BEGIN TBNETERP.XNT.XNT_CREATE_TABLE_TONKY(:pPreTableName, :pNextTableName, :unitCode , :year, :period); END;";
                        ctx.Database.ExecuteSqlCommand(str, pPreTableName, ptableName, pUnitCode, pYear, pPeriod);
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
        }
        public List<InventoryExpImpLevel2> CreateReportInventoryTongHop(string groupBy, DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExpImpLevel2> result = new List<InventoryExpImpLevel2>();
            List<InventoryExpImp> data = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);

                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);

                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_TONGHOP(:pGroupBy,:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode,:pNhaCungCapCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString(),
                                Name = reader["Ten"].ToString(),
                                UnitCode = reader["MaDonVi"].ToString(),
                            };
                            data.Add(item);
                        }
                        dbContextTransaction.Commit();
                        if (data.Count > 0)
                        {
                            var group = data.GroupBy(x => x.UnitCode).ToList();
                            foreach (var item in group)
                            {
                                InventoryExpImpLevel2 obj = new InventoryExpImpLevel2();
                                obj.Ma = item.Key;
                                List<InventoryExpImp> list = data.Where(x => x.UnitCode == item.Key).ToList();
                                obj.DataDetails.AddRange(list);
                                result.Add(obj);
                            }
                        }
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public MemoryStream ExportExcelXNTDetail(ParameterInventory pi)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExpImp> itemCollectionGroup = new List<InventoryExpImp>();
            List<InventoryExcel> data = new List<InventoryExcel>();
            var dknhom = "";
            var titleCotMa = "";
            var titleCotName = "";
            switch (pi.GroupBy)
            {
                case TypeGroupInventory.MADONVI:
                    //data = CreateReportInventoryByMerchandiseByMerchandiseGroupDetail(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(InventoryGroupBy.MADONVI.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = " đơn vị";
                    titleCotMa = "Mã đơn vị";
                    titleCotName = "Đơn vị";
                    break;
                case TypeGroupInventory.GROUP:
                    //data = CreateReportInventoryByMerchandiseByMerchandiseGroupDetail(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(InventoryGroupBy.MANHOMVATTU.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = " nhóm vật tư";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Nhóm hàng";
                    break;
                case TypeGroupInventory.TYPE:
                    //data = CreateReportInventoryByMerchandiseByMerchandiseTypeDetail(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(InventoryGroupBy.MALOAIVATTU.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = "loại vật tư";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Loại hàng";
                    break;
                case TypeGroupInventory.NHACUNGCAP:
                    //data = CreateReportInventoryByMerchandiseByNCCDetail(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(InventoryGroupBy.MAKHACHHANG.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = " nhà cung cấp";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Nhà cung cấp";

                    break;
                case TypeGroupInventory.WAREHOUSE:
                    //data = CreateReportInventoryByMerchandiseByKhoDetail(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(InventoryGroupBy.MAKHO.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = " kho";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Kho";
                    break;
                case TypeGroupInventory.MERCHANDISE:
                    //data = CreateReportInventoryByMerchandiseByMerchandise(pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);
                    data = CreateReportInventoryChiTiet(InventoryGroupBy.MAVATTU.ToString(), pi.FromDate, pi.ToDate, pi.UnitCode, pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes);

                    dknhom = " Hàng hóa";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Hàng";
                    break;
            }
            if (data != null)
            {
                result.AddRange(data.ToList());
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = pi.FromDate.Day + "/" + pi.FromDate.Month + "/" + pi.FromDate.Year;
                string toDateFomart = pi.ToDate.Day + "/" + pi.ToDate.Month + "/" + pi.ToDate.Year;

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 13].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo xuất nhập tồn"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value = string.Format("Từ ngày: {0} Đến ngày: {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 13].Merge = true;
                worksheet.Cells[3, 1, 3, 13].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + dknhom;
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 5, 4].Merge = true;
                worksheet.Cells[4, 5, 5, 5].Merge = true;
                worksheet.Cells[4, 6, 4, 7].Merge = true;
                worksheet.Cells[4, 8, 4, 9].Merge = true;
                worksheet.Cells[4, 10, 4, 11].Merge = true;
                worksheet.Cells[4, 12, 4, 13].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Barcode"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "" + titleCotMa; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tên hàng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "" + titleCotName; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tồn đầu kỳ"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Nhập trong kỳ"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Xuất trong kỳ"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 12].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Số lượng"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Giá trị"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Số lượng"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "Giá trị"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Số lượng"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Value = "Giá trị"; worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 12].Value = "Số lượng"; worksheet.Cells[5, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 13].Value = "Giá trị"; worksheet.Cells[5, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                var itemTotal = new InventoryExpImp();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in result)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 13].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.UnitCode + ": " +item.Code + " - " + item.Name;
                    currentRow++;
                    foreach (var itemdetail in item.DetailData)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Barcode;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Code;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.TenVatTu;
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.Name;
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetail.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetail.IncreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetail.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetail.DecreaseValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 11].Value = itemdetail.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 12].Value = itemdetail.ClosingValue; worksheet.Cells[currentRow, startColumn + 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 12].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                        itemTotal.OpeningBalanceQuantity = itemTotal.OpeningBalanceQuantity + itemdetail.OpeningBalanceQuantity;
                        itemTotal.OpeningBalanceValue = itemTotal.OpeningBalanceValue + itemdetail.OpeningBalanceValue;
                        itemTotal.IncreaseQuantity = itemTotal.IncreaseQuantity + itemdetail.IncreaseQuantity;
                        itemTotal.IncreaseValue = itemTotal.IncreaseValue + itemdetail.IncreaseValue;
                        itemTotal.DecreaseQuantity = itemTotal.DecreaseQuantity + itemdetail.DecreaseQuantity;
                        itemTotal.DecreaseValue = itemTotal.DecreaseValue + itemdetail.DecreaseValue;
                        itemTotal.ClosingQuantity = itemTotal.ClosingQuantity + itemdetail.ClosingQuantity;
                        itemTotal.ClosingValue = itemTotal.ClosingValue + itemdetail.ClosingValue;
                        currentRow++;
                    }

                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 3].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.IncreaseValue; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.DecreaseValue; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 11].Value = itemTotal.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 12].Value = itemTotal.ClosingValue; worksheet.Cells[currentRow, startColumn + 11].Style.Numberformat.Format = "#,##0.00";

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
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;

            }
        }

        public MemoryStream ExcelXNTExcelPTA(InventoryReportExcel pi)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExpImp> itemCollectionGroup = new List<InventoryExpImp>();
            List<InventoryExcel> data = new List<InventoryExcel>();
            if (data != null)
            {
                result.AddRange(data.ToList());
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = pi.FromDay + "/" + pi.FromMonth + "/" + pi.FromYear;
                string toDateFomart = pi.ToDay + "/" + pi.ToMonth + "/" + pi.ToYear;

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 11].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo xuất nhập tồn"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + pi.GroupType;
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 4, 5].Merge = true;
                worksheet.Cells[4, 6, 4, 7].Merge = true;
                worksheet.Cells[4, 8, 4, 9].Merge = true;
                worksheet.Cells[4, 10, 4, 11].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tồn đầu kỳ"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Nhập trong kỳ"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Xuất trong kỳ"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);





                worksheet.Cells[5, 4].Value = "Số lượng"; worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Value = "Giá trị"; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Số lượng"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Giá trị"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Số lượng"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "Giá trị"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Số lượng"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Value = "Giá trị"; worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                var itemTotal = new InventoryExpImp();
                int currentRow = startRow;
                int stt = 0;
                foreach (var itemdetail in pi.DetailData)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Code;
                    worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Name;
                    worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = itemdetail.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 8].Value = itemdetail.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 9].Value = itemdetail.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 10].Value = itemdetail.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                    itemTotal.OpeningBalanceQuantity = itemTotal.OpeningBalanceQuantity + itemdetail.OpeningBalanceQuantity;
                    itemTotal.OpeningBalanceValue = itemTotal.OpeningBalanceValue + itemdetail.OpeningBalanceValue;
                    itemTotal.IncreaseQuantity = itemTotal.IncreaseQuantity + itemdetail.IncreaseQuantity;
                    itemTotal.IncreaseValue = itemTotal.IncreaseValue + itemdetail.IncreaseValue;
                    itemTotal.DecreaseQuantity = itemTotal.DecreaseQuantity + itemdetail.DecreaseQuantity;
                    itemTotal.DecreaseValue = itemTotal.DecreaseValue + itemdetail.DecreaseValue;
                    itemTotal.ClosingQuantity = itemTotal.ClosingQuantity + itemdetail.ClosingQuantity;
                    itemTotal.ClosingValue = itemTotal.ClosingValue + itemdetail.ClosingValue;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";

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
        public MemoryStream ExportExcelXNTTongHop(InventoryReport pi)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExpImp> itemCollectionGroup = new List<InventoryExpImp>();
            List<InventoryExcel> data = new List<InventoryExcel>();
            if (data != null)
            {
                result.AddRange(data.ToList());
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = pi.FromDay + "/" + pi.FromMonth + "/" + pi.FromYear;
                string toDateFomart = pi.ToDay + "/" + pi.ToMonth + "/" + pi.ToYear;

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 11].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo xuất nhập tồn"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + pi.GroupType;
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 4, 5].Merge = true;
                worksheet.Cells[4, 6, 4, 7].Merge = true;
                worksheet.Cells[4, 8, 4, 9].Merge = true;
                worksheet.Cells[4, 10, 4, 11].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tồn đầu kỳ"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Nhập trong kỳ"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Xuất trong kỳ"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                
                worksheet.Cells[5, 4].Value = "Số lượng"; worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Value = "Giá trị"; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Số lượng"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Giá trị"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Số lượng"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "Giá trị"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Số lượng"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Value = "Giá trị"; worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                var itemTotal = new InventoryExpImp();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in pi.DetailData)
                {
                    string tenDonVi = string.Empty;
                    var unit = UnitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == item.Ma);
                    if (unit != null) tenDonVi = unit.TenDonVi;
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Ma + " - " + tenDonVi;
                    currentRow++;
                    foreach (var itemdetail in item.DataDetails)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Code;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Name;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetail.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetail.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetail.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetail.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                        itemTotal.OpeningBalanceQuantity = itemTotal.OpeningBalanceQuantity + itemdetail.OpeningBalanceQuantity;
                        itemTotal.OpeningBalanceValue = itemTotal.OpeningBalanceValue + itemdetail.OpeningBalanceValue;
                        itemTotal.IncreaseQuantity = itemTotal.IncreaseQuantity + itemdetail.IncreaseQuantity;
                        itemTotal.IncreaseValue = itemTotal.IncreaseValue + itemdetail.IncreaseValue;
                        itemTotal.DecreaseQuantity = itemTotal.DecreaseQuantity + itemdetail.DecreaseQuantity;
                        itemTotal.DecreaseValue = itemTotal.DecreaseValue + itemdetail.DecreaseValue;
                        itemTotal.ClosingQuantity = itemTotal.ClosingQuantity + itemdetail.ClosingQuantity;
                        itemTotal.ClosingValue = itemTotal.ClosingValue + itemdetail.ClosingValue;
                        currentRow++;
                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";

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
        //log 16-05-2017
        //báo cáo tồn kho 
        public List<InventoryExpImpLevel2> ReportTonTongHop(ParameterInventory pi)
        {
            List<InventoryExpImpLevel2> data = new List<InventoryExpImpLevel2>();
            DateTime beginDay, endDay;
            string ky = string.Empty;
            beginDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 0, 0, 0);
            endDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 23, 59, 59);

            var period = UnitOfWork.Repository<MdPeriod>().DbSet.FirstOrDefault(x => x.FromDate >= beginDay && x.FromDate <= endDay);
            if (period != null)
            {
                ky = ProcedureCollection.GetTableName(period.Year, period.Period);
            }
            switch (pi.GroupBy)
            {
                case TypeGroupInventory.MADONVI:
                    data = ProcedureCollection.TonKhoTongHop(ky, pi.TypeValue.ToString(), InventoryGroupBy.MADONVI.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);
                    break;
                case TypeGroupInventory.TYPE:
                    data = ProcedureCollection.TonKhoTongHop(ky, pi.TypeValue.ToString(), InventoryGroupBy.MALOAIVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);
                    break;
                case TypeGroupInventory.GROUP:
                    data = ProcedureCollection.TonKhoTongHop(ky, pi.TypeValue.ToString(), InventoryGroupBy.MANHOMVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);
                    break;
                case TypeGroupInventory.NHACUNGCAP:
                    data = ProcedureCollection.TonKhoTongHop(ky, pi.TypeValue.ToString(), InventoryGroupBy.MAKHACHHANG.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);
                    break;
                case TypeGroupInventory.WAREHOUSE:
                    data = ProcedureCollection.TonKhoTongHop(ky, pi.TypeValue.ToString(), InventoryGroupBy.MAKHO.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);
                    break;
                default:
                    data = ProcedureCollection.TonKhoTongHop(ky, pi.TypeValue.ToString(), InventoryGroupBy.MAVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);
                    break;
            }
            
            return data;
        }
     
        public MemoryStream ExportExcelTonTongHop(InventoryReport pi)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = pi.FromDay + "/" + pi.FromMonth + "/" + pi.FromYear;

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 11].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo tồn kho"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Ngày: {0}", fromDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 5].Merge = true;
                worksheet.Cells[3, 1, 3, 5].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + pi.GroupType;
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 4, 5].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 4].Value = "Số lượng"; worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Value = "Giá trị"; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                var itemTotal = new InventoryExpImp();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in pi.DetailData)
                {
                    string tenDonVi = string.Empty;
                    var unit = UnitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == item.Ma);
                    if (unit != null) tenDonVi = unit.TenDonVi;
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 5].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Ma + " - " + tenDonVi;
                    currentRow++;
                    foreach (var itemdetail in item.DataDetails)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Code;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Name;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.ClosingValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 4].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                        itemTotal.ClosingQuantity = itemTotal.ClosingQuantity + itemdetail.ClosingQuantity;
                        itemTotal.ClosingValue = itemTotal.ClosingValue + itemdetail.ClosingValue;
                        currentRow++;
                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.ClosingValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";

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
        public MemoryStream ExportExcelTonChiTiet(ParameterInventory pi)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExpImp> itemCollectionGroup = new List<InventoryExpImp>();
            List<InventoryExcel> data = new List<InventoryExcel>();
            var dknhom = "";
            var titleCotMa = "";
            var titleCotName = "";
            string ky = string.Empty;
            DateTime beginDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 0, 0, 0);
            DateTime endDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 23, 59, 59);

            var period = UnitOfWork.Repository<MdPeriod>().DbSet.FirstOrDefault(x => x.FromDate >= beginDay && x.FromDate <= endDay);
            if (period != null)
            {
                ky = ProcedureCollection.GetTableName(period.Year, period.Period);
            }
            switch (pi.GroupBy)
            {
                case TypeGroupInventory.GROUP:
                    data = ProcedureCollection.TonKhoChiTiet(ky, pi.TypeValue.ToString(), InventoryGroupBy.MANHOMVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);
                    dknhom = " nhóm vật tư";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Nhóm hàng";
                    break;
                case TypeGroupInventory.TYPE:
                    data = ProcedureCollection.TonKhoChiTiet(ky, pi.TypeValue.ToString(), InventoryGroupBy.MALOAIVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);

                    dknhom = "loại vật tư";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Loại hàng";
                    break;
                case TypeGroupInventory.NHACUNGCAP:
                    data = ProcedureCollection.TonKhoChiTiet(ky, pi.TypeValue.ToString(), InventoryGroupBy.MAKHACHHANG.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);
                    dknhom = " nhà cung cấp";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Nhà cung cấp";

                    break;
                case TypeGroupInventory.WAREHOUSE:
                    data = ProcedureCollection.TonKhoChiTiet(ky, pi.TypeValue.ToString(), InventoryGroupBy.MAKHO.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);

                    dknhom = " kho";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Kho";
                    break;
                case TypeGroupInventory.MERCHANDISE:
                    data = ProcedureCollection.TonKhoChiTiet(ky, pi.TypeValue.ToString(), InventoryGroupBy.MAVATTU.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode);
                    dknhom = " Hàng hóa";
                    titleCotMa = "Mã hàng";
                    titleCotName = "Hàng";
                    break;
            }
            if (data != null)
            {
                result.AddRange(data.ToList());
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = pi.FromDate.Day + "/" + pi.FromDate.Month + "/" + pi.FromDate.Year;
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 7].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo tồn kho"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value = string.Format("Ngày: {0}", fromDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 7].Merge = true;
                worksheet.Cells[3, 1, 3, 7].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + dknhom;
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 5, 4].Merge = true;
                worksheet.Cells[4, 5, 5, 5].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Barcode"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "" + titleCotMa; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tên hàng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "" + titleCotName; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Số lượng"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Giá trị"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                var itemTotal = new InventoryExpImp();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in result)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.UnitCode + ": "+ item.Code + " - " + item.Name;
                    currentRow++;
                    foreach (var itemdetail in item.DetailData)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Barcode;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Code;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.TenVatTu;
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.Name;
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.ClosingValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 6].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                        itemTotal.ClosingQuantity = itemTotal.ClosingQuantity + itemdetail.ClosingQuantity;
                        itemTotal.ClosingValue = itemTotal.ClosingValue + itemdetail.ClosingValue;
                        currentRow++;
                    }

                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 3].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.ClosingValue; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";

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
        #region Old Version
        /// <summary>
        /// Khóa sổ
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="preTableName"></param>
        /// <param name="nextTableName"></param>
        /// <param name="unitCode"></param>
        /// <param name="year"></param>
        /// <param name="period"></param>
        /// <param name="typeVoucher"></param>
        public void ProcedureCloseInventory(string preTableName, string nextTableName, string unitCode, int year, int period)
        {
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var pPreTableName = new OracleParameter("pPreTableName", OracleDbType.NVarchar2, preTableName, ParameterDirection.Input);
                        var pNextTableName = new OracleParameter("pNextTableName", OracleDbType.NVarchar2, nextTableName, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pYear = new OracleParameter("pYear", OracleDbType.Decimal, year, ParameterDirection.Input);
                        var pPeriod = new OracleParameter("period", OracleDbType.Decimal, period, ParameterDirection.Input);
                        var str = "BEGIN TBNETERP.XNT.XNT_KHOASO(:pPreTableName, :pNextTableName, :unitCode , :year, :period); END;";
                        ctx.Database.ExecuteSqlCommand(str, pPreTableName, pNextTableName, pUnitCode, pYear, pPeriod);
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
        }
        public List<InventoryExpImp> CreateReportInventoryByPeriod(string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, int year, int period)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var tableName = GetTableName(year, period);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pTableName = new OracleParameter("pTableName", OracleDbType.NVarchar2, tableName, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pYear = new OracleParameter("pYear", OracleDbType.Decimal, year, ParameterDirection.Input);
                        var pPeriod = new OracleParameter("period", OracleDbType.Decimal, period, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pTableName, :pUnitCode, :year, :period, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pTableName, pUnitCode, pYear, pPeriod, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                Code = reader["Code"].ToString(),
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                Name = reader["Name"].ToString(),
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Unit = reader["Unit"].ToString(),
                                UnitCode = reader["UnitCode"].ToString(),
                                WareHouseCode = reader["WareHouseCode"].ToString()
                            };
                            result.Add(item);
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
            return result;
        }

        public List<InventoryExpImp> CreateReportInventoryByDay(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NGAY(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                Code = reader["Code"].ToString(),
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                Name = reader["Name"].ToString(),
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Unit = reader["Unit"].ToString(),
                                UnitCode = reader["UnitCode"].ToString(),
                                WareHouseCode = reader["WareHouseCode"].ToString()
                            };
                            result.Add(item);
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
            return result;
        }

        public List<InventoryExpImp> CreateReportInventoryByWareHouse(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NGAY_THEOKHO(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString()
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExpImp> CreateReportInventoryByType(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NGAY_THEOLOAI(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString()
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExpImp> CreateReportInventoryByGroup(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NGAY_THEONHOM(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString()
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExpImp> CreateReportInventoryByMerchandise(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NGAY_THEOMAVATTU(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString(),
                                Name = reader["Ten"].ToString()
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExpImp> CreateReportIventoryItemByWareHouse(DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var period = ctx.MdPeriods.FirstOrDefault(x => x.ToDate == toDate.Date && x.UnitCode == unitCode && x.TrangThai == (int)ApprovalState.IsComplete);
                        if (period == null)
                        {
                            return result;
                        }
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var tableName = GetTableName(period.Year, period.Period);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTableName = new OracleParameter("pTable", OracleDbType.NVarchar2, tableName, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.TONKHO.BAOCAO_TONKHO_THEOKHO(:pTableName, :pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pTableName, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);

                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                Code = reader["Code"].ToString(),
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExpImp> CreateReportIventoryItemByType(DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var period = ctx.MdPeriods.FirstOrDefault(x => x.ToDate == toDate.Date && x.UnitCode == unitCode && x.TrangThai == (int)ApprovalState.IsComplete);
                        if (period == null)
                        {
                            return result;
                        }
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var tableName = GetTableName(period.Year, period.Period);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTableName = new OracleParameter("pTable", OracleDbType.NVarchar2, tableName, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.TONKHO.BAOCAO_TONKHO_THEOLOAI(:pTableName, :pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pTableName, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);

                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                Code = reader["Code"].ToString(),
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExpImp> CreateReportIventoryItemByGroup(DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var period = ctx.MdPeriods.FirstOrDefault(x => x.ToDate == toDate.Date && x.UnitCode == unitCode && x.TrangThai == (int)ApprovalState.IsComplete);
                        if (period == null)
                        {
                            return result;
                        }
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var tableName = GetTableName(period.Year, period.Period);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTableName = new OracleParameter("pTable", OracleDbType.NVarchar2, tableName, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.TONKHO.BAOCAO_TONKHO_THEONHOM(:pTableName, :pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pTableName, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);

                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                Code = reader["Code"].ToString(),
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExpImp> CreateReportIventoryItemByMerchandise(DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var period = ctx.MdPeriods.FirstOrDefault(x => x.ToDate == toDate.Date && x.UnitCode == unitCode && x.TrangThai == (int)ApprovalState.IsComplete);
                        if (period == null)
                        {
                            return result;
                        }
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var tableName = GetTableName(period.Year, period.Period);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTableName = new OracleParameter("pTable", OracleDbType.NVarchar2, tableName, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.TONKHO.BAOCAO_TONKHO_THEOMAVATTU(:pTableName, :pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pTableName, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);

                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                Code = reader["Code"].ToString(),
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExpImp> CreateReportInventoryByCustomer(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pCustomersCode = new OracleParameter("pCustomersCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NGAY_THEONHACUNGCAP(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pCustomersCode,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pCustomersCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString()
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExpImp> CreateReportInventoryByMerchandiseByNCC(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NCC_THEOMAVATTU(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString() + " - " + reader["TenVT"].ToString(),
                                Name = reader["Ten"].ToString()
                            };
                            result.Add(item);
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
            return result;
        }
        public List<CashierVm> CreateReportCashierByStaff(DateTime fromDate, DateTime toDate, string unitCode)
        {
            List<CashierVm> result = new List<CashierVm>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.THUNGAN.BC_TONGTIEN_GIAODICH_QUAY(:pFromDate, :pToDate, :pUnitCode, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pFromDate, pToDate, pUnitCode, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal tongBan, tongTra, thucThu;
                            var isSumOut = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isSumIn = decimal.TryParse(reader["TONGTRA"].ToString(), out tongTra);
                            var isSum = decimal.TryParse(reader["THUCTHU"].ToString(), out thucThu);
                            var item = new CashierVm()
                            {
                                NguoiTao = reader["NGUOITAO"].ToString(),
                                MaMayBan = reader["MAMAYBAN"].ToString(),
                                TongBan = isSumOut ? tongBan : 0,
                                TongTra = isSumIn ? tongTra : 0,
                                ThucThu = isSum ? thucThu : 0
                            };
                            result.Add(item);
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
            return result;
        }
        public List<InventoryExcel> CreateReportInventoryByMerchandiseByNCCDetail(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExcelItem> resultDetail = new List<InventoryExcelItem>();
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
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NCC_THEOMAVATTU(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var detailsitem = new InventoryExcelItem()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString(),
                                Name = reader["Ten"].ToString(),
                                CodeParent = reader["CodeNCC"].ToString(),
                                TenVatTu = reader["TenVT"].ToString()

                            };
                            resultDetail.Add(detailsitem);
                        }

                        var temp = resultDetail.GroupBy(x => x.CodeParent);
                        List<InventoryExcel> listCha = new List<InventoryExcel>();

                        temp.ToList().ForEach(x =>
                        {
                            InventoryExcel model = new InventoryExcel();
                            model.Code = x.Key;

                            var children = resultDetail.Where(i => i.CodeParent == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Name = children[0].Name;
                            }

                            model.DetailData.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);


                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public List<InventoryExcel> CreateReportInventoryByMerchandiseByMerchandiseTypeDetail(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExcelItem> resultDetail = new List<InventoryExcelItem>();

            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        //merchandiseTypeCodes = _convertToArrayConditionDetail(merchandiseTypeCodes, result);
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
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_MT_THEOMAVATTU(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var detailsitem = new InventoryExcelItem()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString(),
                                Name = reader["Ten"].ToString(),
                                CodeParent = reader["CodeNCC"].ToString(),
                                TenVatTu = reader["TenVT"].ToString()

                            };
                            //var item = new InventoryExcel()
                            //{
                            //    Code = reader["Code"].ToString(),
                            //    Name = reader["Ten"].ToString()


                            //};
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => x.CodeParent);
                        List<InventoryExcel> listCha = new List<InventoryExcel>();

                        temp.ToList().ForEach(x =>
                        {
                            InventoryExcel model = new InventoryExcel();
                            model.Code = x.Key;
                            var children = resultDetail.Where(i => i.CodeParent == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Name = children[0].Name;
                            }

                            model.DetailData.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);

                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public List<InventoryExcel> CreateReportInventoryByMerchandiseByMerchandiseGroupDetail(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExcelItem> resultDetail = new List<InventoryExcelItem>();

            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        //merchandiseGroupCodes = _convertToArrayConditionDetail(merchandiseGroupCodes, result);
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
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_MG_THEOMAVATTU(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var detailsitem = new InventoryExcelItem()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString(),
                                Name = reader["Ten"].ToString(),
                                CodeParent = reader["CodeNCC"].ToString(),
                                TenVatTu = reader["TenVT"].ToString()

                            };
                            foreach (var itemresult in result)
                            {
                                if (itemresult.Code == detailsitem.CodeParent)
                                {
                                    itemresult.DetailData.Add(detailsitem);
                                    itemresult.Name = detailsitem.Name;
                                }
                            }
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => x.CodeParent);
                        List<InventoryExcel> listCha = new List<InventoryExcel>();

                        temp.ToList().ForEach(x =>
                        {
                            InventoryExcel model = new InventoryExcel();
                            model.Code = x.Key;
                            var children = resultDetail.Where(i => i.CodeParent == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Name = children[0].Name;
                            }

                            model.DetailData.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public List<InventoryExcel> CreateReportInventoryByMerchandiseByKhoDetail(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExcelItem> resultDetail = new List<InventoryExcelItem>();

            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        //merchandiseGroupCodes = _convertToArrayConditionDetail(merchandiseGroupCodes, result);
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
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_KHO_THEOMAVATTU(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var detailsitem = new InventoryExcelItem()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString(),
                                Name = reader["Ten"].ToString(),
                                CodeParent = reader["CodeNCC"].ToString(),
                                TenVatTu = reader["TenVT"].ToString()

                            };
                            foreach (var itemresult in result)
                            {
                                if (itemresult.Code == detailsitem.CodeParent)
                                {
                                    itemresult.DetailData.Add(detailsitem);
                                    itemresult.Name = detailsitem.Name;
                                }
                            }
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => x.CodeParent);
                        List<InventoryExcel> listCha = new List<InventoryExcel>();

                        temp.ToList().ForEach(x =>
                        {
                            InventoryExcel model = new InventoryExcel();
                            model.Code = x.Key;
                            var children = resultDetail.Where(i => i.CodeParent == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Name = children[0].Name;
                            }

                            model.DetailData.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public List<InventoryExcel> CreateReportInventoryByMerchandiseByMerchandise(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExcelItem> resultDetail = new List<InventoryExcelItem>();

            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        //merchandiseGroupCodes = _convertToArrayConditionDetail(merchandiseGroupCodes, result);
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
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NGAY_THEOMAVATTU(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var detailsitem = new InventoryExcelItem()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString(),
                                Name = reader["Ten"].ToString(),
                                CodeParent = reader["CodeNCC"].ToString(),
                                TenVatTu = reader["TenVT"].ToString()

                            };
                            foreach (var itemresult in result)
                            {
                                if (itemresult.Code == detailsitem.CodeParent)
                                {
                                    itemresult.DetailData.Add(detailsitem);
                                    itemresult.Name = detailsitem.Name;
                                }
                            }
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => x.CodeParent);
                        List<InventoryExcel> listCha = new List<InventoryExcel>();

                        temp.ToList().ForEach(x =>
                        {
                            InventoryExcel model = new InventoryExcel();
                            model.Code = x.Key;
                            var children = resultDetail.Where(i => i.CodeParent == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Name = children[0].Name;
                            }

                            model.DetailData.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public List<InventoryExcel> CreateReportInventoryChiTiet(string groupBy, DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExcelItem> resultDetail = new List<InventoryExcelItem>();

            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        //merchandiseGroupCodes = _convertToArrayConditionDetail(merchandiseGroupCodes, result);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_CHITIET(:pGroupBy,:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            string code = reader["Code"] != null ? reader["Code"].ToString() : "";
                            string tenVT = reader["TenVT"] != null ? reader["TenVT"].ToString() : "";
                            string ten = reader["Ten"] != null ? reader["Ten"].ToString() : "";
                            string barcode = reader["Barcode"] != null ? reader["Barcode"].ToString() : "";
                            string nhacungcap = reader["CodeNCC"] != null ? reader["CodeNCC"].ToString() : "";
                            string unitcode = reader["MADONVI"] != null ? reader["MADONVI"].ToString() : "";
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var detailsitem = new InventoryExcelItem()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                CodeParent = nhacungcap,
                                Code = code,
                                Name = ten,
                                TenVatTu = tenVT,
                                Barcode = barcode,
                                UnitCode = unitCode
                            };
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => new { x.CodeParent,x.UnitCode });
                        List<InventoryExcel> listCha = new List<InventoryExcel>();

                        temp.ToList().ForEach(x =>
                        {
                            InventoryExcel model = new InventoryExcel();
                            model.Code = x.Key.CodeParent;
                            model.UnitCode = x.Key.UnitCode;
                            var children = resultDetail.Where(i => i.CodeParent == x.Key.CodeParent && i.UnitCode == x.Key.UnitCode).ToList();
                            if (children[0] != null)
                            {
                                model.Name = children[0].Name;
                            }

                            model.DetailData.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public MemoryStream ExportExcelXNTByMerchandise(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            var result = new TransferObj<InventoryReport>();
            List<InventoryExpImp> itemCollectionGroup = new List<InventoryExpImp>();
            var data = CreateReportInventoryByMerchandise(fromDate, toDate, unitCode, wareHouseCodes, merchandiseTypeCodes, merchandiseGroupCodes, merchandiseCodes);
            if (data != null)
            {
                itemCollectionGroup.AddRange(data.ToList());
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = fromDate.Day + "/" + fromDate.Month + "/" + fromDate.Year;
                string toDateFomart = toDate.Day + "/" + toDate.Month + "/" + toDate.Year;

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 11].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo xuất nhập tồn"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:  Vật tư";
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 4, 5].Merge = true;
                worksheet.Cells[4, 6, 4, 7].Merge = true;
                worksheet.Cells[4, 8, 4, 9].Merge = true;
                worksheet.Cells[4, 10, 4, 11].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã hàng"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên hàng"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tồn đầu kỳ"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Nhập trong kỳ"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Xuất trong kỳ"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 4].Value = "Số lượng"; worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Value = "Giá trị"; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Số lượng"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Giá trị"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Số lượng"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "Giá trị"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Số lượng"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Value = "Giá trị"; worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                var itemTotal = new InventoryExpImp();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in itemCollectionGroup)
                {
                    ++stt;
                    worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                    worksheet.Cells[currentRow, startColumn + 1].Value = item.Code;
                    worksheet.Cells[currentRow, startColumn + 2].Value = item.Name;
                    worksheet.Cells[currentRow, startColumn + 3].Value = item.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 8].Value = item.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 9].Value = item.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 10].Value = item.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                    itemTotal.OpeningBalanceQuantity = itemTotal.OpeningBalanceQuantity + item.OpeningBalanceQuantity;
                    itemTotal.OpeningBalanceValue = itemTotal.OpeningBalanceValue + item.OpeningBalanceValue;
                    itemTotal.IncreaseQuantity = itemTotal.IncreaseQuantity + item.IncreaseQuantity;
                    itemTotal.IncreaseValue = itemTotal.IncreaseValue + item.IncreaseValue;
                    itemTotal.DecreaseQuantity = itemTotal.DecreaseQuantity + item.DecreaseQuantity;
                    itemTotal.DecreaseValue = itemTotal.DecreaseValue + item.DecreaseValue;
                    itemTotal.ClosingQuantity = itemTotal.ClosingQuantity + item.ClosingQuantity;
                    itemTotal.ClosingValue = itemTotal.ClosingValue + item.ClosingValue;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";

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
        public MemoryStream ExportExcelXNTByMerchandiseByNCC(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExpImp> itemCollectionGroup = new List<InventoryExpImp>();
            var data = CreateReportInventoryByMerchandiseByNCCDetail(fromDate, toDate, unitCode, wareHouseCodes, merchandiseTypeCodes, merchandiseGroupCodes, merchandiseCodes, nhaCungCapCodes);
            if (data != null)
            {
                result.AddRange(data.ToList());
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = fromDate.Day + "/" + fromDate.Month + "/" + fromDate.Year;
                string toDateFomart = toDate.Day + "/" + toDate.Month + "/" + toDate.Year;

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 11].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo xuất nhập tồn"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: nhóm Vật tư";
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 4, 5].Merge = true;
                worksheet.Cells[4, 6, 4, 7].Merge = true;
                worksheet.Cells[4, 8, 4, 9].Merge = true;
                worksheet.Cells[4, 10, 4, 11].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã nhóm hàng"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên nhóm hàng"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tồn đầu kỳ"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Nhập trong kỳ"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Xuất trong kỳ"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);





                worksheet.Cells[5, 4].Value = "Số lượng"; worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Value = "Giá trị"; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Số lượng"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Giá trị"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Số lượng"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "Giá trị"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Số lượng"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Value = "Giá trị"; worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                var itemTotal = new InventoryExpImp();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in result)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 11].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Code + " - " + item.Name;
                    currentRow++;
                    foreach (var itemdetail in item.DetailData)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Code;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Name;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetail.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetail.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetail.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetail.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                        itemTotal.OpeningBalanceQuantity = itemTotal.OpeningBalanceQuantity + itemdetail.OpeningBalanceQuantity;
                        itemTotal.OpeningBalanceValue = itemTotal.OpeningBalanceValue + itemdetail.OpeningBalanceValue;
                        itemTotal.IncreaseQuantity = itemTotal.IncreaseQuantity + itemdetail.IncreaseQuantity;
                        itemTotal.IncreaseValue = itemTotal.IncreaseValue + itemdetail.IncreaseValue;
                        itemTotal.DecreaseQuantity = itemTotal.DecreaseQuantity + itemdetail.DecreaseQuantity;
                        itemTotal.DecreaseValue = itemTotal.DecreaseValue + itemdetail.DecreaseValue;
                        itemTotal.ClosingQuantity = itemTotal.ClosingQuantity + itemdetail.ClosingQuantity;
                        itemTotal.ClosingValue = itemTotal.ClosingValue + itemdetail.ClosingValue;
                        currentRow++;
                    }

                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";

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
        public MemoryStream ExportExcelXNTByMerchandiseType(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExpImp> itemCollectionGroup = new List<InventoryExpImp>();
            var data = CreateReportInventoryByMerchandiseByMerchandiseTypeDetail(fromDate, toDate, unitCode, wareHouseCodes, merchandiseTypeCodes, merchandiseGroupCodes, merchandiseCodes, nhaCungCapCodes);
            if (data != null)
            {
                result.AddRange(data.ToList());
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = fromDate.Day + "/" + fromDate.Month + "/" + fromDate.Year;
                string toDateFomart = toDate.Day + "/" + toDate.Month + "/" + toDate.Year;

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 11].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo xuất nhập tồn"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: nhóm Vật tư";
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 4, 5].Merge = true;
                worksheet.Cells[4, 6, 4, 7].Merge = true;
                worksheet.Cells[4, 8, 4, 9].Merge = true;
                worksheet.Cells[4, 10, 4, 11].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã nhóm hàng"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên nhóm hàng"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tồn đầu kỳ"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Nhập trong kỳ"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Xuất trong kỳ"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);





                worksheet.Cells[5, 4].Value = "Số lượng"; worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Value = "Giá trị"; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Số lượng"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Giá trị"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Số lượng"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "Giá trị"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Số lượng"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Value = "Giá trị"; worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                var itemTotal = new InventoryExpImp();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in result)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 11].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Code + " - " + item.Name;
                    currentRow++;
                    foreach (var itemdetail in item.DetailData)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Code;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Name;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetail.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetail.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetail.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetail.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                        itemTotal.OpeningBalanceQuantity = itemTotal.OpeningBalanceQuantity + itemdetail.OpeningBalanceQuantity;
                        itemTotal.OpeningBalanceValue = itemTotal.OpeningBalanceValue + itemdetail.OpeningBalanceValue;
                        itemTotal.IncreaseQuantity = itemTotal.IncreaseQuantity + itemdetail.IncreaseQuantity;
                        itemTotal.IncreaseValue = itemTotal.IncreaseValue + itemdetail.IncreaseValue;
                        itemTotal.DecreaseQuantity = itemTotal.DecreaseQuantity + itemdetail.DecreaseQuantity;
                        itemTotal.DecreaseValue = itemTotal.DecreaseValue + itemdetail.DecreaseValue;
                        itemTotal.ClosingQuantity = itemTotal.ClosingQuantity + itemdetail.ClosingQuantity;
                        itemTotal.ClosingValue = itemTotal.ClosingValue + itemdetail.ClosingValue;
                        currentRow++;
                    }

                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";

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
        public MemoryStream ExportExcelXNTByMerchandiseGroup(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExpImp> itemCollectionGroup = new List<InventoryExpImp>();
            var data = CreateReportInventoryByMerchandiseByMerchandiseGroupDetail(fromDate, toDate, unitCode, wareHouseCodes, merchandiseTypeCodes, merchandiseGroupCodes, merchandiseCodes, nhaCungCapCodes);
            if (data != null)
            {
                result.AddRange(data.ToList());
            }
            using (ExcelPackage package = new ExcelPackage())
            {
                string fromDateFomart = fromDate.Day + "/" + fromDate.Month + "/" + fromDate.Year;
                string toDateFomart = toDate.Day + "/" + toDate.Month + "/" + toDate.Year;

                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 6;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 11].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo xuất nhập tồn"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0} Đến ngày: {1}", fromDateFomart, toDateFomart);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 11].Merge = true;
                worksheet.Cells[3, 1, 3, 11].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: nhóm Vật tư";
                worksheet.Cells[4, 1, 5, 1].Merge = true;
                worksheet.Cells[4, 2, 5, 2].Merge = true;
                worksheet.Cells[4, 3, 5, 3].Merge = true;
                worksheet.Cells[4, 4, 4, 5].Merge = true;
                worksheet.Cells[4, 6, 4, 7].Merge = true;
                worksheet.Cells[4, 8, 4, 9].Merge = true;
                worksheet.Cells[4, 10, 4, 11].Merge = true;

                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã nhóm hàng"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên nhóm hàng"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tồn đầu kỳ"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Nhập trong kỳ"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Xuất trong kỳ"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "Tồn cuối kỳ"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);





                worksheet.Cells[5, 4].Value = "Số lượng"; worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 5].Value = "Giá trị"; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 6].Value = "Số lượng"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 7].Value = "Giá trị"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 8].Value = "Số lượng"; worksheet.Cells[5, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 9].Value = "Giá trị"; worksheet.Cells[5, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 10].Value = "Số lượng"; worksheet.Cells[5, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[5, 11].Value = "Giá trị"; worksheet.Cells[5, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 1, 5, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                var itemTotal = new InventoryExpImp();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in result)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 11].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Code + " - " + item.Name;
                    currentRow++;
                    foreach (var itemdetail in item.DetailData)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Code;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.Name;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetail.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetail.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetail.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetail.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 10].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                        itemTotal.OpeningBalanceQuantity = itemTotal.OpeningBalanceQuantity + itemdetail.OpeningBalanceQuantity;
                        itemTotal.OpeningBalanceValue = itemTotal.OpeningBalanceValue + itemdetail.OpeningBalanceValue;
                        itemTotal.IncreaseQuantity = itemTotal.IncreaseQuantity + itemdetail.IncreaseQuantity;
                        itemTotal.IncreaseValue = itemTotal.IncreaseValue + itemdetail.IncreaseValue;
                        itemTotal.DecreaseQuantity = itemTotal.DecreaseQuantity + itemdetail.DecreaseQuantity;
                        itemTotal.DecreaseValue = itemTotal.DecreaseValue + itemdetail.DecreaseValue;
                        itemTotal.ClosingQuantity = itemTotal.ClosingQuantity + itemdetail.ClosingQuantity;
                        itemTotal.ClosingValue = itemTotal.ClosingValue + itemdetail.ClosingValue;
                        currentRow++;
                    }

                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.OpeningBalanceQuantity; worksheet.Cells[currentRow, startColumn + 3].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.OpeningBalanceValue; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.IncreaseQuantity; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.IncreaseValue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.DecreaseQuantity; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.DecreaseValue; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.ClosingQuantity; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.ClosingValue; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";

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

        /// <summary>
        /// Trả và tên bảng VD(XNT_2016_KY_1)
        /// </summary>
        /// <param name="year">Năm (VD: 2016)</param>
        /// <param name="period">Kỳ: (VD: 1)</param>
        /// <returns></returns>
        public string GetTableName(int year, int period)
        {
            return string.Format("XNT_{0}_KY_{1}", year, period);
        }



    }
}
