using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System.Linq.Expressions;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Dashboard;
using System.Data.Entity;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Oracle.ManagedDataAccess.Types;

namespace BTS.API.SERVICE.Dashboard
{
    
    public interface IDashboardService : IDataInfoService<NvVatTuChungTu>
    {
        List<DashboardVm.RetailRevenue> GetRevenue();
        List<DashboardVm.TransactionAmmount> GetTransactionSummary();
        List<DashboardVm.BestMerchandise> GetBestOfFiveMerchandiseSelled();
        List<DashboardVm.BestMerchandise> GetMatHangBanCham();
        List<DashboardVm.BestMerchandise> GetDoanhThuLoaiHang();
        List<DashboardVm.BestMerchandise> GetDoanhThuNhomHang();
        List<DashboardVm.InventoryMerchandise> GetTonMatHang(string maVatTu);
        decimal GetAmmountImportToDay();
        decimal GetAmmountExportToDay();
        int GetCountImportTransactionNotApproved();
        int GetCountExportTransactionNotApproved();
        //Add function here
    }
    public class DashboardService : DataInfoServiceBase<NvVatTuChungTu>, IDashboardService
    {

        public DashboardService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public decimal GetAmmountExportToDay()
        {
            var _unitCode = GetCurrentUnitCode();
            var _ammount = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => x.UnitCode.Equals(_unitCode) && x.LoaiPhieu.Equals(TypeVoucher.XBAN.ToString()) && x.TrangThai == (int)OrderState.IsComplete && DbFunctions.TruncateTime(x.NgayDuyetPhieu) == DbFunctions.TruncateTime(DateTime.Now)).Sum(x => x.ThanhTienSauVat);
            var _ammountBLe = UnitOfWork.Repository<NvGiaoDichQuay>().DbSet.Where(x => x.MaDonVi.Equals(_unitCode) && x.LoaiGiaoDich == 1 && DbFunctions.TruncateTime(x.NgayPhatSinh) == DbFunctions.TruncateTime(DateTime.Now)).Sum(x => x.TTienCoVat);
            decimal result = (_ammount.HasValue ? _ammount.Value : 0) + (_ammountBLe.HasValue ? _ammountBLe.Value : 0);
            return result;
        }

        public decimal GetAmmountImportToDay()
        {
            var _unitCode = GetCurrentUnitCode();
            var _ammount = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => x.UnitCode.Equals(_unitCode) && x.LoaiPhieu.Equals(TypeVoucher.NMUA.ToString()) && x.TrangThai == (int)OrderState.IsComplete && DbFunctions.TruncateTime(x.NgayDuyetPhieu) == DbFunctions.TruncateTime(DateTime.Now)).Sum(x => x.ThanhTienSauVat);
            return _ammount.HasValue ? _ammount.Value : 0;
        }

        public List<DashboardVm.BestMerchandise> GetBestOfFiveMerchandiseSelled()
        {
            List<DashboardVm.BestMerchandise> result = new List<DashboardVm.BestMerchandise>();
            var _unitCode = GetCurrentUnitCode();
            
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("P_UNITCODE", OracleDbType.NVarchar2, _unitCode, ParameterDirection.Input);
                            var pTuNgay = new OracleParameter("P_TUNGAY", OracleDbType.Date, DateTime.Now.Date.AddDays(-7), ParameterDirection.Input);
                            var pDenNgay = new OracleParameter("P_DENNGAY", OracleDbType.Date, DateTime.Now.Date, ParameterDirection.Input);
                            var pReturnData = new OracleParameter("CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.THONGKE_VATTU_BANCHAY(:P_UNITCODE,:P_TUNGAY, :P_DENNGAY, :CUR); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, pReturnData);
                            OracleDataReader reader = ((OracleRefCursor)pReturnData.Value).GetDataReader();
                            while (reader.Read())
                            {
                                var item = new DashboardVm.BestMerchandise()
                                {
                                    MaVatTu = reader["MAVATTU"].ToString(),
                                    TenVatTu = reader["TENVATTU"].ToString(),
                                    GiaTri = !string.IsNullOrEmpty(reader["THANHTIEN"].ToString()) ? Decimal.Parse(reader["THANHTIEN"].ToString()) : 0,
                                };
                                result.Add(item);
                            }
                            return result;
                        }
                        catch
                        {
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DashboardVm.BestMerchandise> GetDoanhThuLoaiHang()
        {
            //DashboardVm.Parameter para
            List<DashboardVm.BestMerchandise> result = new List<DashboardVm.BestMerchandise>();
            var _unitCode = GetCurrentUnitCode();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("P_UNITCODE", OracleDbType.NVarchar2, _unitCode, ParameterDirection.Input);
                            //var pTuNgay = new OracleParameter("P_TUNGAY", OracleDbType.Date, para .TuNgay.Date, ParameterDirection.Input);
                            //var pDenNgay = new OracleParameter("P_DENNGAY", OracleDbType.Date, para.DenNgay.Date, ParameterDirection.Input);
                            var pTuNgay = new OracleParameter("P_TUNGAY", OracleDbType.Date, DateTime.Now.Date.AddDays(-7), ParameterDirection.Input);
                            var pDenNgay = new OracleParameter("P_DENNGAY", OracleDbType.Date, DateTime.Now.Date, ParameterDirection.Input);
                            var pReturnData = new OracleParameter("CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.THONGKE_DOANHTHU_LOAIHANG(:P_UNITCODE,:P_TUNGAY, :P_DENNGAY, :CUR); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, pReturnData);
                            OracleDataReader reader = ((OracleRefCursor)pReturnData.Value).GetDataReader();
                            while (reader.Read())
                            {
                                var item = new DashboardVm.BestMerchandise()
                                {
                                    MaVatTu = reader["MAVATTU"].ToString(),
                                    TenVatTu = reader["TENVATTU"].ToString(),
                                    GiaTri = !string.IsNullOrEmpty(reader["THANHTIEN"].ToString()) ? Decimal.Parse(reader["THANHTIEN"].ToString()) : 0,
                                };
                                result.Add(item);
                            }
                            return result;
                        }
                        catch
                        {
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int GetCountExportTransactionNotApproved()
        {
            var _unitCode = GetCurrentUnitCode();
            var _ammount = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => x.UnitCode.Equals(_unitCode) && x.LoaiPhieu.Equals(TypeVoucher.XBAN.ToString()) && x.TrangThai == (int)OrderState.IsNotApproval).Count();
            return _ammount;
        }

        public int GetCountImportTransactionNotApproved()
        {
            var _unitCode = GetCurrentUnitCode();
            var _ammount = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => x.UnitCode.Equals(_unitCode) && x.LoaiPhieu.Equals(TypeVoucher.NMUA.ToString()) && x.TrangThai == (int)OrderState.IsNotApproval).Count();
            return _ammount;
        }

        public List<DashboardVm.RetailRevenue> GetRevenue()
        {
            List<DashboardVm.RetailRevenue> result = new List<DashboardVm.RetailRevenue>();
            var _unitCode = GetCurrentUnitCode();
            try
            {
                DashboardVm.RetailRevenue item = new DashboardVm.RetailRevenue();
                item.UnitCode = _unitCode;
                var _donVi = UnitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == _unitCode);
                item.TenDonVi = _donVi == null ? string.Empty : _donVi.TenDonVi;
                var lst = UnitOfWork.Repository<ThongKe>().DbSet.Where(x => x.Loai.Equals(LoaiThongKe.XBANLE.ToString()) && x.UnitCode.Equals(item.UnitCode)).OrderByDescending(x => x.TuNgay).Take(6).ToList();
                for(int i = lst.Count -1; i >=0 ; i--)
                {
                    var x = lst[i];
                    string _ngayCTMobile = string.Format("{0}/{1}", x.TuNgay.Value.Day, x.TuNgay.Value.Month);
                    item.DataDetails.Add(new DashboardVm.RetailRevenueDetail() { NgayCT = x.TuNgay, DoanhThu = x.GiaTri,NgayCTMobile = _ngayCTMobile });
                }
                var _revenueToday = UnitOfWork.Repository<NvGiaoDichQuay>().DbSet.Where(x => x.LoaiGiaoDich == 1 && x.MaDonVi.Equals(item.UnitCode) && DbFunctions.TruncateTime(x.NgayPhatSinh) == DbFunctions.TruncateTime(DateTime.Now)).Sum(x=>x.TTienCoVat);
                item.DataDetails.Add(new DashboardVm.RetailRevenueDetail() { NgayCT = DateTime.Now,NgayCTMobile = string.Format("{0}/{1}", DateTime.Now.Day, DateTime.Now.Month), DoanhThu = _revenueToday.HasValue?_revenueToday.Value:0 });
                result.Add(item);
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }

        public List<DashboardVm.BestMerchandise> GetDoanhThuNhomHang()
        {
            List<DashboardVm.BestMerchandise> result = new List<DashboardVm.BestMerchandise>();
            var _unitCode = GetCurrentUnitCode();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("P_UNITCODE", OracleDbType.NVarchar2, _unitCode, ParameterDirection.Input);
                            //var pTuNgay = new OracleParameter("P_TUNGAY", OracleDbType.Date, para.TuNgay.Date, ParameterDirection.Input);
                            //var pDenNgay = new OracleParameter("P_DENNGAY", OracleDbType.Date, para.DenNgay.Date, ParameterDirection.Input);
                            var pTuNgay = new OracleParameter("P_TUNGAY", OracleDbType.Date, DateTime.Now.Date.AddDays(-7), ParameterDirection.Input);
                            var pDenNgay = new OracleParameter("P_DENNGAY", OracleDbType.Date, DateTime.Now.Date, ParameterDirection.Input);
                            var pReturnData = new OracleParameter("CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.THONGKE_DOANHTHU_NHOMHANG(:P_UNITCODE,:P_TUNGAY, :P_DENNGAY, :CUR); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, pReturnData);
                            OracleDataReader reader = ((OracleRefCursor)pReturnData.Value).GetDataReader();
                            while (reader.Read())
                            {
                                var item = new DashboardVm.BestMerchandise()
                                {
                                    MaVatTu = reader["MAVATTU"].ToString(),
                                    TenVatTu = reader["TENVATTU"].ToString(),
                                    GiaTri = !string.IsNullOrEmpty(reader["THANHTIEN"].ToString()) ? Decimal.Parse(reader["THANHTIEN"].ToString()) : 0,
                                };
                                result.Add(item);
                            }
                            return result;
                        }
                        catch
                        {
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DashboardVm.TransactionAmmount> GetTransactionSummary()
        {
            List<DashboardVm.TransactionAmmount> result = new List<DashboardVm.TransactionAmmount>();
            List<DashboardVm.TransactionAmmountDetail> lstDetail = new List<DashboardVm.TransactionAmmountDetail>();
            var _unitCode = GetCurrentUnitCode();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("P_UNITCODE", OracleDbType.NVarchar2, _unitCode, ParameterDirection.Input);
                            //var pTuNgay = new OracleParameter("P_TUNGAY", OracleDbType.Date, para .TuNgay.Date, ParameterDirection.Input);
                            //var pDenNgay = new OracleParameter("P_DENNGAY", OracleDbType.Date, para.DenNgay.Date, ParameterDirection.Input);
                            var pTuNgay = new OracleParameter("P_TUNGAY", OracleDbType.Date, DateTime.Now.Date.AddDays(-7), ParameterDirection.Input);
                            var pDenNgay = new OracleParameter("P_DENNGAY", OracleDbType.Date, DateTime.Now.Date, ParameterDirection.Input);
                            var pReturnData = new OracleParameter("CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.THONGKE_NMUA_XBAN(:P_UNITCODE,:P_TUNGAY, :P_DENNGAY, :CUR); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, pReturnData);
                            OracleDataReader reader = ((OracleRefCursor)pReturnData.Value).GetDataReader();
                            while (reader.Read())
                            {
                                DateTime _date = !string.IsNullOrEmpty(reader["NGAYCHUNGTU"].ToString()) ? DateTime.Parse(reader["NGAYCHUNGTU"].ToString()) : new DateTime(1, 0, 0);
                                var item = new DashboardVm.TransactionAmmountDetail()
                                {
                                    UnitCode = reader["UNITCODE"].ToString(),
                                    LoaiNhapXuat = !string.IsNullOrEmpty(reader["LOAINHAPXUAT"].ToString()) ? int.Parse(reader["LOAINHAPXUAT"].ToString()) : 0,
                                    LoaiChungTu = reader["LOAICHUNGTU"].ToString(),
                                    TenChungTu = reader["TENCHUNGTU"].ToString(),
                                    NgayChungTu = string.Format("{0}/{1}/{2}",_date.Day, _date.Month,_date.Year),
                                    GiaTri = !string.IsNullOrEmpty(reader["SOTIEN"].ToString()) ? Decimal.Parse(reader["SOTIEN"].ToString()) : 0,
                                };
                                lstDetail.Add(item);
                            }
                            var tmp = lstDetail.GroupBy(x => x.NgayChungTu).ToList();
                            tmp.ForEach(x =>
                            {
                                DashboardVm.TransactionAmmount obj = new DashboardVm.TransactionAmmount();
                                obj.NgayChungTu = x.Key;
                                lstDetail.ForEach(y =>
                                {
                                    if (y.NgayChungTu == x.Key)
                                        obj.DataDetails.Add(y);
                                });
                                result.Add(obj);
                            });
                            return result;
                        }
                        catch
                        {
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DashboardVm.BestMerchandise> GetMatHangBanCham()
        {
            List<DashboardVm.BestMerchandise> result = new List<DashboardVm.BestMerchandise>();
            var _unitCode = GetCurrentUnitCode();

            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("P_UNITCODE", OracleDbType.NVarchar2, _unitCode, ParameterDirection.Input);
                            var pTuNgay = new OracleParameter("P_TUNGAY", OracleDbType.Date, DateTime.Now.Date.AddDays(-7), ParameterDirection.Input);
                            var pDenNgay = new OracleParameter("P_DENNGAY", OracleDbType.Date, DateTime.Now.Date, ParameterDirection.Input);
                            var pSoLuong = new OracleParameter("P_SOLUONG", OracleDbType.Decimal, 20, ParameterDirection.Input);
                            var pReturnData = new OracleParameter("CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.THONGKE_VATTU_BANCHAM(:P_UNITCODE,:P_TUNGAY, :P_DENNGAY, :P_SOLUONG, :CUR); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, pSoLuong, pReturnData);
                            OracleDataReader reader = ((OracleRefCursor)pReturnData.Value).GetDataReader();
                            while (reader.Read())
                            {
                                var item = new DashboardVm.BestMerchandise()
                                {
                                    MaVatTu = reader["MAVATTU"].ToString(),
                                    TenVatTu = reader["TENVATTU"].ToString(),
                                    GiaTri = !string.IsNullOrEmpty(reader["SOLUONG"].ToString()) ? Decimal.Parse(reader["SOLUONG"].ToString()) : 0,
                                };
                                result.Add(item);
                            }
                            return result;
                        }
                        catch
                        {
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DashboardVm.InventoryMerchandise> GetTonMatHang(string maVatTu)
        {
            List<DashboardVm.InventoryMerchandise> result = new List<DashboardVm.InventoryMerchandise>();
            var _unitCode = GetCurrentUnitCode();

            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("P_UNITCODE", OracleDbType.NVarchar2, _unitCode, ParameterDirection.Input);
                            var pTuNgay = new OracleParameter("P_TUNGAY", OracleDbType.Date, new DateTime(2018,5,5), ParameterDirection.Input);
                            var pMaVatTu = new OracleParameter("P_MAVATTU", OracleDbType.NVarchar2, maVatTu, ParameterDirection.Input);
                            var pReturnData = new OracleParameter("CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.THONGKE_KIEMTRATONHANG(:P_UNITCODE,:P_TUNGAY, :P_MAVATTU, :CUR); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pMaVatTu, pReturnData);
                            OracleDataReader reader = ((OracleRefCursor)pReturnData.Value).GetDataReader();
                            while (reader.Read())
                            {
                                var item = new DashboardVm.InventoryMerchandise()
                                {
                                    MaVatTu = reader["MAVATTU"].ToString(),
                                    TenVatTu = reader["TENVATTU"].ToString(),
                                    GiaTri = !string.IsNullOrEmpty(reader["SOLUONG"].ToString()) ? Decimal.Parse(reader["SOLUONG"].ToString()) : 0,
                                    MaKho = reader["MAKHO"].ToString(),
                                    TenKho = reader["TENKHO"].ToString(),
                                    UnitCode = reader["UNITCODE"].ToString(),
                                };
                                result.Add(item);
                            }
                            return result;
                        }
                        catch
                        {
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
