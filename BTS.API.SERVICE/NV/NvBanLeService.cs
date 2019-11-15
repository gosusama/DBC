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
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
namespace BTS.API.SERVICE.NV
{
    public interface INvRetailsService : IDataInfoService<NvGiaoDichQuay>
    {
        NvGiaoDichQuay InsertPhieu(NvGiaoDichQuayVm.DataDto instance);
        NvGiaoDichQuayVm.DataDto SetCustomer(NvGiaoDichQuayVm.DataDto instance);
        List<NvGiaoDichQuayVm.DataDetails> DataDetails(NvGiaoDichQuayVm.DataDto instance);
        MdMerchandiseVm.DtoAndPromotion GetDataPromotionByMerchandise(MdMerchandiseVm.DtoAndPromotion para);
        void RUNSTORE_TANGGIAM_TON(string MaGiaoDichQuayPk, int LoaiGiaoDich);
        List<string[]> GetInventory(string code);
        List<NvGiaoDichQuayVm.ReportHistoryBuyOfCustomer> ReportHistoryBuyOfCustomer(string para);
    }
    public class NvBanLeService : DataInfoServiceBase<NvGiaoDichQuay>, INvRetailsService
    {
        public NvBanLeService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public void RUNSTORE_TANGGIAM_TON(string MaGiaoDichQuayPk, int LoaiGiaoDich)
        {
            //chạy store trừ xuất nhập tồn                   
            try
            {
                using (var context = new ERPContext())
                {
                    string query = "";
                    var _unitCode = GetCurrentUnitCode();
                    NvGiaoDichQuay gdquay = context.NvGiaoDichQuays.Where(x => x.MaGiaoDichQuayPK == MaGiaoDichQuayPk).SingleOrDefault();
                    string ID = gdquay.Id;
                    var pP_TABLENAME = new OracleParameter(":P_TABLENAME", OracleDbType.Varchar2, CurrentSetting.GetKhoaSo(_unitCode).GetTableName(), ParameterDirection.Input);
                    var pP_NAM = new OracleParameter(":P_NAM", OracleDbType.Varchar2, CurrentSetting.GetKhoaSo(_unitCode).Year, ParameterDirection.Input);
                    var pP_KY = new OracleParameter(":P_KY", OracleDbType.Decimal, CurrentSetting.GetKhoaSo(_unitCode).Period, ParameterDirection.Input);
                    var pP_ID = new OracleParameter(":P_ID", OracleDbType.Varchar2, ID, ParameterDirection.Input);
                    query = "BEGIN TBNETERP.XNT.XNT_GIAM_PHIEU(:pP_TABLENAME,:pP_NAM,:pP_KY,:pP_ID); END;";

                    if (gdquay.LoaiGiaoDich == 2)
                    {
                        string IDTraLai = gdquay.MaGiaoDichQuayPK;
                        var pP_IDTraLai = new OracleParameter(":P_ID", OracleDbType.Varchar2, IDTraLai, ParameterDirection.Input);
                        query = "BEGIN TBNETERP.XNT.XNT_TANG_PHIEU(:pP_TABLENAME,:pP_NAM,:pP_KY,:pP_IDTraLai); END;";
                        context.Database.ExecuteSqlCommand(query, pP_TABLENAME, pP_NAM, pP_KY, pP_IDTraLai);
                    }
                    else
                    {
                        context.Database.ExecuteSqlCommand(query, pP_TABLENAME, pP_NAM, pP_KY, pP_ID);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public List<string[]> GetInventory(string code)
        {
            List<string[]> list = new List<string[]>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var unitCode = GetCurrentUnitCode();
                        var pP_TABLENAME = new OracleParameter(":P_TABLENAME", OracleDbType.Varchar2, CurrentSetting.GetKhoaSo(unitCode).GetTableName(), ParameterDirection.Input);
                        var pP_UNITCODE = new OracleParameter(":P_UNITCODE", OracleDbType.Varchar2, unitCode, ParameterDirection.Input);
                        var pP_MAVATTU = new OracleParameter(":P_MAVATTU", OracleDbType.Varchar2, code, ParameterDirection.Input);
                        var pCUR = new OracleParameter(":CUR", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.INVENTORE_MULTI_STORE(:P_MAVATTU, :P_UNITCODE, :P_TABLENAME,:CUR); END;";
                        ctx.Database.ExecuteSqlCommand(str, pP_MAVATTU, pP_UNITCODE, pP_TABLENAME, pCUR);
                        OracleDataReader reader = ((OracleRefCursor)pCUR.Value).GetDataReader();

                        bool hasNameColumn = false;
                        while (reader.Read())
                        {
                            var it = new InventoryUniCode();
                            int fieldCount = reader.FieldCount;
                            string[] obj = new string[fieldCount];
                            if (!hasNameColumn)
                            {
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    var colName = reader.GetName(i);
                                    obj[i] = colName;
                                }
                                hasNameColumn = true;
                                list.Add(obj);
                                obj = new string[fieldCount];
                            }
                            for (int i = 0; i < fieldCount; i++)
                            {
                                var colName = reader.GetName(i);
                                obj[i] = reader[colName].ToString();
                            }
                            list.Add(obj);
                        }
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
                return list;
            }
        }
        //lấy dữ liệu mua hàng của khách group by theo ngày mua hàng
        public List<NvGiaoDichQuayVm.ReportHistoryBuyOfCustomer> ReportHistoryBuyOfCustomer(string para)
        {
            List<NvGiaoDichQuayVm.ReportHistoryBuyOfCustomer> result =  new List<NvGiaoDichQuayVm.ReportHistoryBuyOfCustomer>();
            List<NvGiaoDichQuayVm.ReportHistoryBuyOfCustomerLevel2> data = new List<NvGiaoDichQuayVm.ReportHistoryBuyOfCustomerLevel2>();
            var unitCode = GetCurrentUnitCode();
            DateTime fromDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime toDate = DateTime.Now.Date;
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pMaKhachHang = new OracleParameter("pMaKhachHang", OracleDbType.NVarchar2, para, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.HISTORYBUYOFCUSTOMER(:pFromDate,:pToDate,:pUnitCode,:pMaKhachHang, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pFromDate, pToDate, pUnitCode, pMaKhachHang, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, tongBan, tienKhuyenMai, tienVoucher, tienChietKhau,tienCod,tienThe,tienMat;
                            DateTime ngayMuaHang;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isTienBan = decimal.TryParse(reader["TTIENCOVAT"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isTienChietKhau = decimal.TryParse(reader["TIENCHIETKHAU"].ToString(), out tienChietKhau);
                            var isNgayMuaHang = DateTime.TryParse(reader["NGAYGIAODICH"].ToString(), out ngayMuaHang);
                            var isTienVoucher = decimal.TryParse(reader["TIENVOUCHER"].ToString(), out tienVoucher);
                            var isTienCod = decimal.TryParse(reader["TIENCOD"].ToString(), out tienCod);
                            var isTienThe = decimal.TryParse(reader["TIENTHE"].ToString(), out tienThe);
                            var isTienMat = decimal.TryParse(reader["TIENMAT"].ToString(), out tienMat);
                            var item = new NvGiaoDichQuayVm.ReportHistoryBuyOfCustomerLevel2()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                TienVoucher = isTienVoucher ? Decimal.Parse(reader["TIENVOUCHER"].ToString()) : 0,
                                TienChuyenKhoan = isTienThe ? Decimal.Parse(reader["TIENTHE"].ToString()) : 0,
                                TienCod = isTienCod ? Decimal.Parse(reader["TIENCOD"].ToString()) : 0 ,
                                TienMat = isTienMat ? Decimal.Parse(reader["TIENMAT"].ToString()) : 0 ,
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                NgayMuaHang = isNgayMuaHang ? ngayMuaHang : new DateTime(0001, 1, 1)
                            };
                            data.Add(item);
                        }
                        dbContextTransaction.Commit();
                        if (data.Count > 0)
                        {
                            var groupBy = data.GroupBy(x => x.NgayMuaHang).ToList();
                            groupBy.ForEach(x =>
                            {
                                NvGiaoDichQuayVm.ReportHistoryBuyOfCustomer obj = new NvGiaoDichQuayVm.ReportHistoryBuyOfCustomer();
                                obj.NgayMuaHang = x.Key;
                                var lst = data.Where(y => y.NgayMuaHang == x.Key).ToList();
                                obj.DataDetails.AddRange(lst);
                                result.Add(obj);
                            });
                        }
                        return result;
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
        }

        public MdMerchandiseVm.DtoAndPromotion GetDataPromotionByMerchandise(MdMerchandiseVm.DtoAndPromotion para)
        {
            try
            {
                para.IsTichDiem = false;
                var _unitCode = GetCurrentUnitCode();
                var data = new MdMerchandiseVm.DtoAndPromotion();
                var resultPromotion = new List<NvChuongTrinhKhuyenMaiVm.Promotion>();
                var kmTichDiemTheoGio = new NvChuongTrinhKhuyenMai();
                var kmTichDiem = new NvChuongTrinhKhuyenMai();
                var nowDay = DateTime.Now;
                var date = new DateTime(nowDay.Year, nowDay.Month, nowDay.Day, 0, 0, 0);
                var resultTichDiem = UnitOfWork.Repository<NvChuongTrinhKhuyenMai>().DbSet.Where(x => x.LoaiKhuyenMai == 6 && x.TuNgay <= date && x.DenNgay >= date && x.UnitCode.StartsWith(_unitCode) && x.TrangThai == 10).ToList();
                if (resultTichDiem.Count > 0)
                {
                    int getHour = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                    string[] tugio = resultTichDiem[0].TuGio.Split(':');
                    string[] dengio = resultTichDiem[0].DenGio.Split(':');
                    int minuteTuGio = Int32.Parse(tugio[0]) * 60 + Int32.Parse(tugio[1]);
                    int minuteDenGio = Int32.Parse(dengio[0]) * 60 + Int32.Parse(dengio[1]);
                    if (minuteTuGio <= getHour && getHour <= minuteDenGio)
                    {
                        kmTichDiemTheoGio = resultTichDiem[0];
                    }
                    else
                    {
                        kmTichDiem = resultTichDiem[0];
                    }
                }
                using (BTS.API.ENTITY.ERPContext ctx = new BTS.API.ENTITY.ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        string maHang = para.MaVatTu;
                        var pNgay = new OracleParameter("p_Ngay", OracleDbType.Date, DateTime.Now.Date, ParameterDirection.Input);
                        var pMaVatTu = new OracleParameter("p_maVatTu", OracleDbType.NVarchar2, maHang, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("p_unitCode", OracleDbType.NVarchar2, _unitCode, ParameterDirection.Input);
                        var pDataPromotion = new OracleParameter("p_dataPromotion", OracleDbType.RefCursor, ParameterDirection.Output);
                        string strQuerry = "BEGIN TBNETERP.KHUYENMAI.KHUYENMAI_HANG_THEOMAHANG(:pNgay,:pMaVatTu,:pUnitCode,:datapromotion); END;";
                        var khuyenMai = ctx.Database.SqlQuery<NvChuongTrinhKhuyenMaiVm.Promotion>(strQuerry, pNgay, pMaVatTu, pUnitCode, pDataPromotion);
                        try
                        {
                            int count = 0;
                            decimal tienKhuyenMai = 0;
                            resultPromotion = khuyenMai.ToList();
                            //kiểm tra riêng km tích điểm
                            if (kmTichDiem.MaChuongTrinh != null && kmTichDiem.LoaiKhuyenMai != null)
                            {
                                para.TyLeKhuyenMai_TichDiem = kmTichDiem.TyLeKhuyenMai;
                                para.GiaTriKhuyenMai_TichDiem = kmTichDiem.GiaTriKhuyenMai;
                                para.IsTichDiem = true;
                            }
                            else if (kmTichDiemTheoGio.MaChuongTrinh != null && kmTichDiemTheoGio.LoaiKhuyenMai != null && kmTichDiemTheoGio.TuGio != null)
                            {
                                para.TyLeKhuyenMai_TichDiem = kmTichDiemTheoGio.TyLeKhuyenMai;
                                para.GiaTriKhuyenMai_TichDiem = kmTichDiemTheoGio.GiaTriKhuyenMai;
                                para.IsTichDiem = true;
                            }
                            //kiểm tra khuyến mại từ giờ đến giờ
                            if (resultPromotion.Count != 0)
                            {
                                foreach (var record in resultPromotion)
                                {
                                    MdMerchandiseVm.ListDataDetails detailsKhuyenMai = new MdMerchandiseVm.ListDataDetails();
                                    if (string.IsNullOrEmpty(record.TuGio) || string.IsNullOrEmpty(record.DenGio))
                                    {
                                        if (record.LoaiKhuyenMai == LoaiKhuyenMai.DongGia)
                                        {
                                            para.TyLeKhuyenMai_DongGia = record.TyLeKhuyenMai;
                                            para.GiaTriKhuyenMai_DongGia = record.GiaTriKhuyenMaiChildren;
                                            para.LoaiKhuyenMai = LoaiKhuyenMai.DongGia.ToString();
                                        }
                                        else if (record.LoaiKhuyenMai == LoaiKhuyenMai.ChietKhau)
                                        {
                                            para.TyLeKhuyenMai_ChietKhau = record.TyLeKhuyenMaiChildren;
                                            para.GiaTriKhuyenMai_ChietKhau = record.GiaTriKhuyenMaiChildren;
                                            para.LoaiKhuyenMai = LoaiKhuyenMai.ChietKhau.ToString();
                                        }
                                        else if (record.LoaiKhuyenMai == LoaiKhuyenMai.TinhTien)
                                        {
                                            para.TyLeBatDau_TinhTien = record.TyLeBatDau;
                                            para.TyLeKhuyenMai_TinhTien = record.TyLeKhuyenMai;
                                            para.GiaTriKhuyenMai_TinhTien = record.GiaTriKhuyenMaiChildren;
                                            para.LoaiKhuyenMai = LoaiKhuyenMai.TinhTien.ToString();
                                        }
                                        else if (record.LoaiKhuyenMai == LoaiKhuyenMai.Buy1Get1)
                                        {
                                            para.MaHang_Km_Buy1Get1 = record.MaHang_Km_Buy1Get1;
                                            para.TenHang_Km_Buy1Get1 = record.TenHang_Km_Buy1Get1;
                                            para.SoLuong_Km_Buy1Get1 = record.SoLuong_Km_Buy1Get1;
                                            para.LoaiKhuyenMai = LoaiKhuyenMai.Buy1Get1.ToString();
                                        }
                                        else if (record.LoaiKhuyenMai == LoaiKhuyenMai.Combo)
                                        {
                                            para.GiaTriKhuyenMai_Combo = record.GiaTriKhuyenMai; // tiền sau khi đã trừ k/m
                                            para.LoaiKhuyenMai = LoaiKhuyenMai.Combo.ToString();
                                            para.SoLuongKhuyenMai_Combo = record.SoLuong_KhuyenMai;
                                            detailsKhuyenMai.MaVatTuCon = record.MaHang_Km_Buy1Get1;
                                            detailsKhuyenMai.TenVatTuCon = record.TenHang_Km_Buy1Get1;
                                            para.ListMaHangKhuyenMai.Add(detailsKhuyenMai);
                                        }
                                    }
                                    else
                                    {
                                        //nếu từ giờ đến giờ thì kiểm tra còn tron khung giờ khuyến mại không
                                        int getHour = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                                        string[] tugio = record.TuGio.Split(':');
                                        string[] dengio = record.DenGio.Split(':');
                                        int minuteTuGio = Int32.Parse(tugio[0]) * 60 + Int32.Parse(tugio[1]);
                                        int minuteDenGio = Int32.Parse(dengio[0]) * 60 + Int32.Parse(dengio[1]);
                                        if (minuteTuGio <= getHour && getHour <= minuteDenGio)
                                        {
                                            if (record.LoaiKhuyenMai == LoaiKhuyenMai.DongGia)
                                            {
                                                para.TuGio = record.TuGio;
                                                para.DenGio = record.DenGio;
                                                para.TyLeKhuyenMai_DongGia = record.TyLeKhuyenMai; //tỷ lệ đồng giá
                                                para.GiaTriKhuyenMai_DongGia = record.GiaTriKhuyenMaiChildren; //tiền km đồng giá
                                                para.LoaiKhuyenMai = LoaiKhuyenMai.DongGia.ToString();
                                            }
                                            else if (record.LoaiKhuyenMai == LoaiKhuyenMai.ChietKhau)
                                            {
                                                para.TuGio = record.TuGio;
                                                para.DenGio = record.DenGio;
                                                para.TyLeKhuyenMai_ChietKhau = record.TyLeKhuyenMaiChildren; //tỷ lệ khuyến mại
                                                para.GiaTriKhuyenMai_ChietKhau = record.GiaTriKhuyenMaiChildren; // tiền sau khi đã trừ k/m
                                                para.LoaiKhuyenMai = LoaiKhuyenMai.ChietKhau.ToString();
                                            }
                                            else if (record.LoaiKhuyenMai == LoaiKhuyenMai.TinhTien)
                                            {
                                                para.TyLeKhuyenMai_TinhTien = record.TyLeKhuyenMai;
                                                para.GiaTriKhuyenMai_TinhTien = record.GiaTriKhuyenMaiChildren;
                                                para.LoaiKhuyenMai = LoaiKhuyenMai.TinhTien.ToString();
                                            }
                                            else if (record.LoaiKhuyenMai == LoaiKhuyenMai.Buy1Get1)
                                            {
                                                para.MaHang_Km_Buy1Get1 = record.MaHang_Km_Buy1Get1;
                                                para.TenHang_Km_Buy1Get1 = record.TenHang_Km_Buy1Get1;
                                                para.SoLuong_Km_Buy1Get1 = record.SoLuong_Km_Buy1Get1;
                                                para.LoaiKhuyenMai = LoaiKhuyenMai.Buy1Get1.ToString();
                                            }
                                            else if (record.LoaiKhuyenMai == LoaiKhuyenMai.Combo)
                                            {
                                                para.TuGio = record.TuGio;
                                                para.DenGio = record.DenGio;
                                                para.GiaTriKhuyenMai_Combo = record.GiaTriKhuyenMai; // tiền sau khi đã trừ k/m
                                                para.LoaiKhuyenMai = LoaiKhuyenMai.Combo.ToString();
                                                para.SoLuongKhuyenMai_Combo = record.SoLuong_KhuyenMai;
                                                detailsKhuyenMai.MaVatTuCon = record.MaHang_Km_Buy1Get1;
                                                detailsKhuyenMai.TenVatTuCon = record.TenHang_Km_Buy1Get1;
                                                para.ListMaHangKhuyenMai.Add(detailsKhuyenMai);
                                            }
                                        }
                                    }
                                    para.MaChuongTrinhKhuyenMai = record.MaChuongTrinh;
                                    para.SoLuong = record.SoLuong;
                                    para.SoLuong_KhuyenMai = record.SoLuong_KhuyenMai;
                                    para.MaKhoKhuyenMai = record.MaKhoKhuyenMai;
                                    para.TienHangKhuyenMai = tienKhuyenMai;
                                    count++;
                                }
                            }
                            if (count > 1)
                            {
                                para.LogKhuyenMaiError = count;
                                para.NoiDungKhuyenMai = "Cảnh báo ! Mã hàng đang được kích hoạt trong 2 chương trình khuyến mại";
                            }
                            else if (count == 0)
                            {
                                para.LogKhuyenMaiError = 1;
                                para.NoiDungKhuyenMai = "Mã hàng này không được khuyến mại";
                            }
                            else
                            {
                                para.LogKhuyenMaiError = 1;
                                para.NoiDungKhuyenMai = "Mã hàng này được khuyến mại trong chương trình mã : " + resultPromotion[0].MaChuongTrinh;
                            }
                            data = para;
                        }
                        catch
                        {
                            data = para;
                        }
                    }
                }
                return data;
            }
            catch
            {
                return null;
            }
        }
        public NvGiaoDichQuay InsertPhieu(NvGiaoDichQuayVm.DataDto instance)
        {
            decimal tyLeKhuyenMai = 0, tienKhuyenMai = 0;
            var unitCode = GetCurrentUnitCode();
            var item = AutoMapper.Mapper.Map<NvGiaoDichQuayVm.DataDto, NvGiaoDichQuay>(instance);
            item.Id = Guid.NewGuid().ToString();
            item.MaGiaoDich = instance.MaGiaoDich;
            item.MaGiaoDichQuayPK = unitCode + '.' + instance.MaGiaoDich;
            item.MaDonVi = unitCode;
            item.LoaiGiaoDich = instance.LoaiGiaoDich;
            item.NgayTao = DateTime.Now;
            item.NgayPhatSinh = instance.NgayPhatSinh;
            item.MaNguoiTao = instance.MaNhanVien;
            item.NguoiTao = instance.TenNhanVien;
            item.MaVoucher = instance.Voucher;
            item.TienKhachDua = instance.TienKhachDua;
            item.TienVoucher = instance.TienVoucher;
            item.TienTheVip = instance.TienTheVip;
            item.TienTraLai = instance.TienThua;
            item.TienThe = instance.TienThe;
            item.TienMat = instance.TienMat;
            item.TienCOD = instance.TienCOD;
            item.TTienCoVat = instance.SumTienHang;
            item.ThoiGian = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
            item.MaKhachHang = instance.Makh;
            item.UnitCode = unitCode;
            item.IState = "Complete";
            item.ICreateDate = DateTime.Now;
            item.ICreateBy = instance.UserName;
            item.MaQuayBan = "QUAYBANLE";
            //tính tỷ lệ khuyến mại voucher ra phần trăm theo tỷ lệ các mặt hàng trong danh sách bán
            if (item.TienVoucher > 0)
            {
                decimal tTienMin = 0;
                decimal tTienTemp = 0;
                int number = instance.DataDetails.Count;
                if (number > 0)
                {
                    //tỷ lệ từng mặt hàng
                    decimal.TryParse(instance.DataDetails[0].TTienCoVat.ToString(),out tTienMin);
                    foreach (var record in instance.DataDetails)
                    {
                        decimal.TryParse(record.TTienCoVat.ToString(), out tTienTemp);
                        if (tTienTemp < tTienMin) tTienMin = tTienTemp;
                    }
                    //lấy được tiền nhỏ nhất rồi thì đem chia giá từng mặt hàng để ra tỷ lệ theo %
                    foreach (var record in instance.DataDetails)
                    {
                        decimal tTyLeKhuyenMaiVoucher = 0;
                        decimal.TryParse((100 * (record.TTienCoVat.Value / tTienMin) / number).ToString(), out tTyLeKhuyenMaiVoucher);
                        record.TyLeVoucher = tTyLeKhuyenMaiVoucher;
                    }
                }
            }
            var result = item;
            Insert(result);
            var dataDetails = AutoMapper.Mapper.Map<List<NvGiaoDichQuayVm.DataDetails>, List<NvGiaoDichQuayChiTiet>>(instance.DataDetails);
            dataDetails.ForEach(x =>
            {
                decimal.TryParse(x.TyLeKhuyenMai.ToString(), out tyLeKhuyenMai);
                decimal.TryParse(x.TienKhuyenMai.ToString(), out tienKhuyenMai);
                x.Id = Guid.NewGuid().ToString();
                x.MaGDQuayPK = item.MaGiaoDichQuayPK;
                x.MaKhoHang = unitCode + "-K2";
                x.MaDonVi = unitCode;
                x.MaVatTu = x.MaVatTu;
                x.Barcode = x.Barcode;
                x.TenDayDu = x.TenDayDu;
                x.NguoiTao = item.NguoiTao;
                x.MaBoPK = x.MaBoPK;
                x.NgayTao = DateTime.Now;
                x.NgayPhatSinh = item.NgayPhatSinh;
                x.SoLuong = x.SoLuong;
                x.TTienCoVat = x.TTienCoVat;
                x.VatBan = x.VatBan;
                x.GiaBanLeCoVat = x.GiaBanLeCoVat;
                x.MaKhachHang = instance.Makh;
                x.MaChuongTrinhKhuyenMai = x.MaChuongTrinhKhuyenMai;
                x.TyLeLaiLe = x.TyLeLaiLe;
                x.GiaVon = x.GiaVon;
                x.LoaiKhuyenMai = x.LoaiKhuyenMai;
                x.TienKhuyenMai = tienKhuyenMai;
                x.TyLeKhuyenMai = tyLeKhuyenMai;
                x.TienVoucher = (item.TienVoucher * x.TyLeVoucher)/100;
            });
            UnitOfWork.Repository<NvGiaoDichQuayChiTiet>().InsertRange(dataDetails);
            return result;
        }
        public NvGiaoDichQuayVm.DataDto SetCustomer(NvGiaoDichQuayVm.DataDto instance)
        {
            var customer = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == instance.MaKhachHang);
            if (customer != null)
            {
                instance.DiaChi = customer.DiaChi;
                instance.DienThoai = customer.DienThoai;
                instance.NgaySinh = customer.NgaySinh;
                instance.NgayDacBiet = customer.NgayDacBiet;
                instance.Email = customer.Email;
                instance.TenKhachHang = customer.TenKH;
                instance.TenKhac = customer.TenKhac;
                instance.MaThe = customer.MaThe;
                instance.QuenThe = customer.QuenThe;
            }
            return instance;
        }
        public List<NvGiaoDichQuayVm.DataDetails> DataDetails(NvGiaoDichQuayVm.DataDto instance)
        {
            var data =
                UnitOfWork.Repository<NvGiaoDichQuayChiTiet>()
                    .DbSet.Where(x => x.MaGDQuayPK == instance.MaGiaoDichQuayPk)
                    .ToList();
            var dataDetails = AutoMapper.Mapper.Map<List<NvGiaoDichQuayChiTiet>, List<NvGiaoDichQuayVm.DataDetails>>(data);
            return dataDetails;
        }
    }
}
