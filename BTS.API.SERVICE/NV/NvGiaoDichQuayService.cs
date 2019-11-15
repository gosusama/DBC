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
using System.Web;
using System.Security.Claims;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Md;

namespace BTS.API.SERVICE.NV
{
    public interface INvGiaoDichQuayService : IDataInfoService<NvGiaoDichQuay>
    {
        List<NvGiaoDichQuayVm.ReportGDQDetailLevel2> CreatePrintTranferCashieer(DateTime fromDate, DateTime toDate, string unitCode, string shallingMachine, string cashieer);

        List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportGiaoDichQuay(ParameterCashier pc);
        MemoryStream ExportExcelGDQTongHop(ParameterCashier pc);
        MemoryStream ExportExcelDetail(ParameterCashier pc);
        List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportNhapBLeTraLai(ParameterCashier pc);
        MemoryStream ExportExcelNhapBLeTraLaiTongHop(ParameterCashier pc);
        MemoryStream ExportExcelNhapBLeTraLaiDetail(ParameterCashier pc);

    }
    public class NvGiaoDichQuayService : DataInfoServiceBase<NvGiaoDichQuay>, INvGiaoDichQuayService
    {
        public NvGiaoDichQuayService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<NvGiaoDichQuay, bool>> GetKeyFilter(NvGiaoDichQuay instance)
        {
            return x => x.MaGiaoDichQuayPK == instance.MaGiaoDichQuayPK;
        }
        public List<NvGiaoDichQuayVm.ReportGDQDetailLevel2> CreatePrintTranferCashieer(DateTime fromDate, DateTime toDate, string unitCode, string shallingMachine, string cashieer)
        {
            List<NvGiaoDichQuayVm.ReportGDQDetail> detailData = new List<NvGiaoDichQuayVm.ReportGDQDetail>();
            List<NvGiaoDichQuayVm.ReportGDQDetailLevel2> result = new List<NvGiaoDichQuayVm.ReportGDQDetailLevel2>();
            //unitCode = _convertToArrayCondition(unitCode);
            shallingMachine = _convertToArrayCondition(shallingMachine);
            cashieer = _convertToArrayCondition(cashieer);
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pMaNguoiTao = new OracleParameter("pMaNguoiTao", OracleDbType.NVarchar2, cashieer, ParameterDirection.Input);
                        var pMaQuayBan = new OracleParameter("pMaQuayBan", OracleDbType.NVarchar2, shallingMachine, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.GIAODICHQUAY.BAOCAO_GIAODICHQUAY(:pFromDate, :pToDate, :pUnitCode,:pMaQuayBan,:pMaNguoiTao, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pFromDate, pToDate, pUnitCode, pMaQuayBan, pMaNguoiTao, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ReTongBan, ReTongTraLai;
                            var isTongBan = decimal.TryParse(reader["TONGBAN"].ToString(), out ReTongBan);
                            var isTongTraLai = decimal.TryParse(reader["TONGTRALAI"].ToString(), out ReTongTraLai);
                            var item = new NvGiaoDichQuayVm.ReportGDQDetail()
                            {
                                MaDonVi = reader["MADONVI"].ToString(),
                                MaNguoiTao = reader["MANGUOITAO"].ToString(),
                                MaQuayBan = reader["MAQUAYBAN"].ToString(),
                                NguoiTao = reader["NGUOITAO"].ToString(),
                                TongBan = isTongBan ? ReTongBan : 0,
                                TongTraLai = isTongTraLai ? ReTongTraLai : 0,
                                ThucThu = (isTongBan ? ReTongBan : 0) - (isTongTraLai ? ReTongTraLai : 0)
                            };
                            //result.
                            detailData.Add(item);
                        }
                        dbContextTransaction.Commit();
                        var groupBy = detailData.GroupBy(x => x.MaDonVi).ToList();
                        foreach (var item in groupBy)
                        {
                            NvGiaoDichQuayVm.ReportGDQDetailLevel2 obj = new NvGiaoDichQuayVm.ReportGDQDetailLevel2();
                            obj.MaDonVi = item.Key;
                            List<NvGiaoDichQuayVm.ReportGDQDetail> lst = detailData.Where(x => x.MaDonVi == obj.MaDonVi).ToList();
                            obj.DataDetails.AddRange(lst);
                            result.Add(obj);
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
        #region Export Excel in BaoCaoTheoNhanVien
        public MemoryStream ExportExcelGDQTongHop(ParameterCashier pi)
        {
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
            List<NvGiaoDichQuayVm.ObjectReportLevel2> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportLevel2>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();

            var titleCotName = "";
            data = ProcedureCollection.GDQTongHop(pi.GroupBy.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate);

            var groupBy = data.GroupBy(x => x.MaDonVi).ToList();
            List<NvGiaoDichQuayVm.ObjectReportLevel2> result = new List<NvGiaoDichQuayVm.ObjectReportLevel2>();
            foreach (var item in groupBy)
            {
                NvGiaoDichQuayVm.ObjectReportLevel2 obj = new NvGiaoDichQuayVm.ObjectReportLevel2();
                obj.Ma = item.Key;
                var donVi = UnitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == obj.Ma);
                if (donVi != null)
                    obj.Ten = donVi.TenDonVi;
                List<NvGiaoDichQuayVm.ObjectReport> lst = data.Where(x => x.MaDonVi == obj.Ma).ToList();
                var groupByDkLoc = lst.GroupBy(x => new { x.Ma, x.Ten, x.MaDonVi })
                                        .Select(group => new NvGiaoDichQuayVm.ObjectReport()
                                        {
                                            Ma = group.Key.Ma,
                                            Ten = group.Key.Ten,
                                            MaDonVi = group.Key.MaDonVi,
                                            TienVoucher = group.Sum(a => a.TienVoucher),
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
            if (data != null)
            {
                itemCollectionGroup.AddRange(result);
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
                worksheet.Cells[3, 1].Value = "BẢNG KÊ XUẤT BÁN LẺ"; worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; worksheet.Cells[3, 1].Style.Font.Bold = true;
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
                worksheet.Cells[6, 6].Value = "Vốn"; worksheet.Cells[6, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 7].Value = "Tiền thuế"; worksheet.Cells[6, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 8].Value = "T.Tiền KM"; worksheet.Cells[6, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 9].Value = "T.Tiền Chiết khấu"; worksheet.Cells[6, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 10].Value = "T.Tiền Voucher"; worksheet.Cells[6, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 11].Value = "T.Tiền Mặt"; worksheet.Cells[6, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 12].Value = "T.Tiền Chuyển khoản"; worksheet.Cells[6, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 13].Value = "T.Tiền COD"; worksheet.Cells[6, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 14].Value = "T.Doanh thu"; worksheet.Cells[6, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 15].Value = "T.Tiền bán"; worksheet.Cells[6, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 16].Value = "Lãi bán lẻ"; worksheet.Cells[6, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 17].Value = "Tỉ lệ lãi"; worksheet.Cells[6, 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 18].Value = "Ghi chú"; worksheet.Cells[6, 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;
                foreach (var itemdetails in itemCollectionGroup)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 18].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = itemdetails.Ma + " - " + itemdetails.Ten;
                    currentRow++;
                    foreach (var item in itemdetails.DataDetails)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = item.Ma;
                        worksheet.Cells[currentRow, startColumn + 2].Value = item.Ten;
                        worksheet.Cells[currentRow, startColumn + 3].Value = item.SoLuongBan;
                        worksheet.Cells[currentRow, startColumn + 4].Value = item.VonChuaVat; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 5].Value = item.Von; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = item.TienThue; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = item.TienKhuyenMai; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = item.TienChietKhau; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = item.TienVoucher; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = item.TienMat; worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 11].Value = item.TienChuyenKhoan; worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 12].Value = item.TienCod; worksheet.Cells[currentRow, 13].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 13].Value = item.DoanhThu; worksheet.Cells[currentRow, 14].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 14].Value = item.TienBan; worksheet.Cells[currentRow, 15].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 15].Value = item.LaiBanLe; worksheet.Cells[currentRow, 16].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 16].Value = item.Von == 0 ? 0 : item.LaiBanLe / item.Von; worksheet.Cells[currentRow, 17].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 17].Value = /*item.Von == 0 ? "Có thể chưa nhập" : */"";
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 12].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        itemTotal.SoLuongBan = itemTotal.SoLuongBan + item.SoLuongBan;
                        itemTotal.VonChuaVat = itemTotal.VonChuaVat + item.VonChuaVat;
                        itemTotal.Von = itemTotal.Von + item.Von;
                        itemTotal.TienThue = itemTotal.TienThue + item.TienThue;
                        itemTotal.DoanhThu = itemTotal.DoanhThu + item.DoanhThu;
                        itemTotal.TienBan = itemTotal.TienBan + item.TienBan;
                        itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + item.TienKhuyenMai;
                        itemTotal.TienChietKhau += item.TienChietKhau;
                        itemTotal.TienVoucher += item.TienVoucher;
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
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.VonChuaVat; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.Von; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.TienChietKhau; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.TienVoucher; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.TienMat; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 11].Value = itemTotal.TienChuyenKhoan; worksheet.Cells[currentRow, startColumn + 11].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 12].Value = itemTotal.TienCod; worksheet.Cells[currentRow, startColumn + 12].Style.Numberformat.Format = "#,##0.00";

                worksheet.Cells[currentRow, startColumn + 13].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 13].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 14].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 14].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 15].Value = itemTotal.LaiBanLe; worksheet.Cells[currentRow, startColumn + 15].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 16].Value = itemTotal.Von == 0 ? 0 : itemTotal.LaiBanLe / itemTotal.Von; worksheet.Cells[currentRow, startColumn + 16].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

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
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public MemoryStream ExportExcelDetail(ParameterCashier pi)
        {
            string userName = string.Empty;
            //GetNhanVien
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                var _UnitCode = GetCurrentUnitCode();
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
            List<NvGiaoDichQuayVm.ObjectReportCha> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCha> data = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            var titleCotMa = "";
            data = ProcedureCollection.GDQChiTiet(pi.GroupBy.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
            if (data != null)
            {
                itemCollectionGroup.AddRange(data.ToList());
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
                worksheet.Cells[1, 1, 1, 7].Merge = true; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[2, 1, 2, 7].Merge = true; worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[1, 8, 1, 14].Merge = true; worksheet.Cells[1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[2, 8, 2, 14].Merge = true; worksheet.Cells[2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[1, 1].Value = CurrentSetting.GetUnitName(pi.UnitCode);
                worksheet.Cells[2, 1].Value = CurrentSetting.GetUnitAddress(pi.UnitCode);
                worksheet.Cells[1, 8].Value = string.Format("Ngày in: {0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                worksheet.Cells[2, 8].Value = "Người in: " + userName;
                worksheet.Cells[3, 1, 3, 14].Merge = true;
                worksheet.Cells[3, 1].Value = "BẢNG KÊ XUẤT BÁN LẺ"; worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; worksheet.Cells[3, 1].Style.Font.Bold = true;
                worksheet.Cells[4, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[4, 1, 4, 14].Merge = true;
                worksheet.Cells[5, 1, 5, 14].Merge = true;

                worksheet.Cells[5, 1].Value = "Điều kiện, Nhóm theo: " + titleCotMa;
                worksheet.Cells[6, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 3].Value = "Danh sách"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 4].Value = "Ngày"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 5].Value = "Số lượng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 6].Value = "Vốn chưa VAT"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 7].Value = "Vốn"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 8].Value = "Tiền thuế"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 9].Value = "T.Tiền KM"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 10].Value = "T.Tiền C.Khấu"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 11].Value = "T.Tiền Voucher"; worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 12].Value = "T.Doanh thu"; worksheet.Cells[4, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 13].Value = "T.Tiền bán"; worksheet.Cells[4, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 14].Value = "Lãi bán lẻ"; worksheet.Cells[4, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 15].Value = "Tỉ lệ lãi"; worksheet.Cells[4, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 16].Value = "Ghi chú"; worksheet.Cells[4, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[6, 17].Value = "Barcode"; worksheet.Cells[4, 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;

                foreach (var item in itemCollectionGroup)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 17].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.MaDonVi + ": " + item.Ma + " - " + item.Ten;
                    currentRow++;
                    foreach (var itemdetails in item.DataDetails)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetails.Ma;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetails.Ten;
                        worksheet.Cells[currentRow, startColumn + 3].Value = string.Format("{0}/{1}/{2}", itemdetails.NgayGiaoDich.Day, itemdetails.NgayGiaoDich.Month, itemdetails.NgayGiaoDich.Year);
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetails.SoLuongBan;
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetails.VonChuaVat; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetails.Von; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetails.TienThue; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetails.TienKhuyenMai; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetails.TienChietKhau; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetails.TienVoucher; worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 11].Value = itemdetails.DoanhThu; worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 12].Value = itemdetails.TienBan; worksheet.Cells[currentRow, 13].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 13].Value = itemdetails.LaiBanLe; worksheet.Cells[currentRow, 14].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 14].Value = itemdetails.Von == 0 ? 0 : itemdetails.LaiBanLe / itemdetails.Von; worksheet.Cells[currentRow, 15].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 15].Value = /*itemdetails.Von == 0 ? "Có thể chưa nhập" :*/ "";
                        worksheet.Cells[currentRow, startColumn + 16].Value = itemdetails.Barcode;
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 14].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        itemTotal.SoLuongBan = itemTotal.SoLuongBan + itemdetails.SoLuongBan;
                        itemTotal.VonChuaVat = itemTotal.VonChuaVat + itemdetails.VonChuaVat;
                        itemTotal.Von = itemTotal.Von + itemdetails.Von;
                        itemTotal.TienThue = itemTotal.TienThue + itemdetails.TienThue;
                        itemTotal.DoanhThu = itemTotal.DoanhThu + itemdetails.DoanhThu;
                        itemTotal.TienBan = itemTotal.TienBan + itemdetails.TienBan;
                        itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + itemdetails.TienKhuyenMai;
                        itemTotal.TienChietKhau += itemdetails.TienChietKhau;
                        itemTotal.TienVoucher += itemdetails.TienVoucher;
                        itemTotal.LaiBanLe = itemTotal.LaiBanLe + itemdetails.LaiBanLe;
                        currentRow++;
                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 3].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.SoLuongBan;
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.VonChuaVat; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.Von; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.TienChietKhau; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.TienVoucher; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 11].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 11].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 12].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 12].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 13].Value = itemTotal.LaiBanLe; worksheet.Cells[currentRow, startColumn + 13].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 14].Value = itemTotal.Von == 0 ? 0 : itemTotal.LaiBanLe / itemTotal.Von; worksheet.Cells[currentRow, startColumn + 14].Style.Numberformat.Format = "#,##0.00";
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
                worksheet.Column(16).AutoFit();
                worksheet.Column(17).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 10));
                package.SaveAs(ms);
                return ms;
            }
        }
        public List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportGiaoDichQuay(ParameterCashier pi)
        {
            List<NvGiaoDichQuayVm.ObjectReportLevel2> result = new List<NvGiaoDichQuayVm.ObjectReportLevel2>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();
            data = ProcedureCollection.GDQTongHop(pi.GroupBy.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.XuatXuCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
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
                                            TienVoucher = group.Sum(a => a.TienVoucher),
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
            return result;
        }
        public List<NvGiaoDichQuayVm.ObjectReportLevel2> ReportNhapBLeTraLai(ParameterCashier pi)
        {
            List<NvGiaoDichQuayVm.ObjectReportLevel2> result = new List<NvGiaoDichQuayVm.ObjectReportLevel2>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();
            data = ProcedureCollection.NhapBanLeTraLaiTongHop(pi.GroupBy.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
            var groupBy = data.GroupBy(x => x.MaDonVi).ToList();
            foreach (var item in groupBy)
            {
                NvGiaoDichQuayVm.ObjectReportLevel2 obj = new NvGiaoDichQuayVm.ObjectReportLevel2();
                obj.Ma = item.Key;
                List<NvGiaoDichQuayVm.ObjectReport> lst = data.Where(x => x.MaDonVi == obj.Ma).ToList();
                obj.DataDetails.AddRange(lst);
                result.Add(obj);
            }
            return result;
        }
        public MemoryStream ExportExcelNhapBLeTraLaiTongHop(ParameterCashier pi)
        {
            List<NvGiaoDichQuayVm.ObjectReport> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReport>();
            List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport> data = new List<BTS.API.SERVICE.NV.NvGiaoDichQuayVm.ObjectReport>();

            var titleCotName = "";
            data = ProcedureCollection.NhapBanLeTraLaiTongHop(pi.GroupBy.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
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
                worksheet.Cells[1, 1, 1, 13].Merge = true;
                worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬP BÁN LẺ TRẢ LẠI"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 13].Merge = true;
                worksheet.Cells[3, 1, 3, 13].Merge = true;

                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo: " + titleCotName;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Danh sách"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Số lượng"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Vốn chưa VAT"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Vốn"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Tiền thuế"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "T.Doanh thu"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "T.Tiền bán"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "T.Tiền KM"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Value = "Lãi bán lẻ"; worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 12].Value = "Tỉ lệ lãi"; worksheet.Cells[4, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 13].Value = "Ghi chú"; worksheet.Cells[4, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
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
                    worksheet.Cells[currentRow, startColumn + 4].Value = item.VonChuaVat; worksheet.Cells[currentRow, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 5].Value = item.Von; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 6].Value = item.TienThue; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 7].Value = item.DoanhThu; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 8].Value = item.TienBan; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 9].Value = item.TienKhuyenMai; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 10].Value = item.LaiBanLe; worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 11].Value = item.Von == 0 ? 0 : item.LaiBanLe / item.Von; worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[currentRow, startColumn + 12].Value = item.Von == 0 ? "Có thể chưa nhập" : "";
                    worksheet.Cells[currentRow, 1, currentRow, startColumn + 12].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                    itemTotal.SoLuongBan = itemTotal.SoLuongBan + item.SoLuongBan;
                    itemTotal.VonChuaVat = itemTotal.VonChuaVat + item.VonChuaVat;
                    itemTotal.Von = itemTotal.Von + item.Von;
                    itemTotal.TienThue = itemTotal.TienThue + item.TienThue;
                    itemTotal.DoanhThu = itemTotal.DoanhThu + item.DoanhThu;
                    itemTotal.TienBan = itemTotal.TienBan + item.TienBan;
                    itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + item.TienKhuyenMai;
                    itemTotal.LaiBanLe = itemTotal.LaiBanLe + item.LaiBanLe;
                    currentRow++;
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 2].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 3].Value = itemTotal.SoLuongBan;
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.VonChuaVat; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.Von; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.LaiBanLe; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 11].Value = itemTotal.Von == 0 ? 0 : itemTotal.LaiBanLe / itemTotal.Von; worksheet.Cells[currentRow, startColumn + 11].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);

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
        public MemoryStream ExportExcelNhapBLeTraLaiDetail(ParameterCashier pi)
        {
            List<NvGiaoDichQuayVm.ObjectReportCha> itemCollectionGroup = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCha> data = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            var titleCotMa = "";
            data = ProcedureCollection.NhapBanLeTraLaiChiTiet(pi.GroupBy.ToString(), pi.WareHouseCodes, pi.MerchandiseTypeCodes, pi.MerchandiseGroupCodes, pi.MerchandiseCodes, pi.NhaCungCapCodes, pi.UnitCode, pi.FromDate, pi.ToDate);
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
                worksheet.Cells[1, 1, 1, 14].Merge = true;
                worksheet.Cells[1, 1].Value = "BẢNG KÊ NHẬP BÁN LẺ TRẢ LẠI"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value =
                    string.Format("Từ ngày: {0}/{1}/{2} Đến ngày: {3}/{4}/{5}",
                    pi.FromDate.Day, pi.FromDate.Month, pi.FromDate.Year, pi.ToDate.Day, pi.ToDate.Month, pi.ToDate.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 14].Merge = true;
                worksheet.Cells[3, 1, 3, 14].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo:" + titleCotMa;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã"; worksheet.Cells[4, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Danh sách"; worksheet.Cells[4, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Ngày"; worksheet.Cells[4, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Số lượng"; worksheet.Cells[4, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Vốn chưa VAT"; worksheet.Cells[4, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Vốn"; worksheet.Cells[4, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 8].Value = "Tiền thuế"; worksheet.Cells[4, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 9].Value = "T.Doanh thu"; worksheet.Cells[4, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 10].Value = "T.Tiền bán"; worksheet.Cells[4, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 11].Value = "T.Tiền KM"; worksheet.Cells[4, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 12].Value = "Lãi bán lẻ"; worksheet.Cells[4, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 13].Value = "Tỉ lệ lãi"; worksheet.Cells[4, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 14].Value = "Ghi chú"; worksheet.Cells[4, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 15].Value = "Barcode"; worksheet.Cells[4, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                var itemTotal = new NvGiaoDichQuayVm.ObjectReport();
                int currentRow = startRow;
                int stt = 0;

                foreach (var item in itemCollectionGroup)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 15].Merge = true;
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
                        worksheet.Cells[currentRow, startColumn + 3].Value = string.Format("{0}/{1}/{2}", itemdetails.NgayGiaoDich.Day, itemdetails.NgayGiaoDich.Month, itemdetails.NgayGiaoDich.Year);
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetails.SoLuongBan;
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetails.VonChuaVat; worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetails.Von; worksheet.Cells[currentRow, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 7].Value = itemdetails.TienThue; worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = itemdetails.DoanhThu; worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = itemdetails.TienBan; worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = itemdetails.TienKhuyenMai; worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 11].Value = itemdetails.LaiBanLe; worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 12].Value = itemdetails.Von == 0 ? 0 : itemdetails.LaiBanLe / itemdetails.Von; worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 13].Value = itemdetails.Von == 0 ? "Có thể chưa nhập" : "";
                        worksheet.Cells[currentRow, startColumn + 14].Value = itemdetails.Barcode;
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 14].Style.Border.BorderAround(ExcelBorderStyle.Dotted);
                        itemTotal.SoLuongBan = itemTotal.SoLuongBan + itemdetails.SoLuongBan;
                        itemTotal.VonChuaVat = itemTotal.VonChuaVat + itemdetails.VonChuaVat;
                        itemTotal.Von = itemTotal.Von + itemdetails.Von;
                        itemTotal.TienThue = itemTotal.TienThue + itemdetails.TienThue;
                        itemTotal.DoanhThu = itemTotal.DoanhThu + itemdetails.DoanhThu;
                        itemTotal.TienBan = itemTotal.TienBan + itemdetails.TienBan;
                        itemTotal.TienKhuyenMai = itemTotal.TienKhuyenMai + itemdetails.TienKhuyenMai;
                        itemTotal.LaiBanLe = itemTotal.LaiBanLe + itemdetails.LaiBanLe;
                        currentRow++;
                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 3].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 4].Value = itemTotal.SoLuongBan;
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.VonChuaVat; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.Von; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 7].Value = itemTotal.TienThue; worksheet.Cells[currentRow, startColumn + 7].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 8].Value = itemTotal.DoanhThu; worksheet.Cells[currentRow, startColumn + 8].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 9].Value = itemTotal.TienBan; worksheet.Cells[currentRow, startColumn + 9].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 10].Value = itemTotal.TienKhuyenMai; worksheet.Cells[currentRow, startColumn + 10].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 11].Value = itemTotal.LaiBanLe; worksheet.Cells[currentRow, startColumn + 11].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 12].Value = itemTotal.Von == 0 ? 0 : itemTotal.LaiBanLe / itemTotal.Von; worksheet.Cells[currentRow, startColumn + 12].Style.Numberformat.Format = "#,##0.00";
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
        #endregion
    }
}
