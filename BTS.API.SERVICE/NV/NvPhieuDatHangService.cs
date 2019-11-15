using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.Services;
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
    public interface INvPhieuDatHangService : IDataInfoService<NvDatHang>
    {
        NvDatHang InsertPhieu(NvPhieuDatHangVm.Dto instance);
        NvDatHang UpdatePhieu(NvPhieuDatHangVm.Dto instance);
        List<NvPhieuDatHangVm.DtoDetail> MergerPhieu(List<string> soPhieu);
        NvPhieuDatHangVm.ReportModel CreateReport(string id);
        NvPhieuDatHangVm.Dto CreateNewInstance();
        List<BTS.API.SERVICE.NV.NvPhieuDatHangVm.DatHangExpImpModel> ReportDatHangTongHop(NvPhieuDatHangVm.ParameterDatHang pi);
        MemoryStream ReportDatHangChiTiet(NvPhieuDatHangVm.ParameterDatHang pi);
        NvDatHang UpdateXuatBan(string soPhieuPK);
        bool DeletePhieu(string id);

    }
    public class NvPhieuDatHangService : DataInfoServiceBase<NvDatHang>, INvPhieuDatHangService
    {
        public NvPhieuDatHangService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public bool DeletePhieu(string id)
        {
            var insatance = UnitOfWork.Repository<NvDatHang>().DbSet.Where(x => x.Id == id).FirstOrDefault();
            if (insatance == null)
            {
                return false;
            }
            var detailData = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(o => o.SoPhieuPk == insatance.SoPhieuPk).ToList();
            foreach (NvDatHangChiTiet dt in detailData)
            {
                dt.ObjectState = ObjectState.Deleted;
            }
            return true;

        }
        public NvPhieuDatHangVm.Dto CreateNewInstance()
        {
            string _unitCode = GetCurrentUnitCode();
            var code = BuildCode_PTNX(TypeVoucher.DH.ToString(), _unitCode, false);
            return new NvPhieuDatHangVm.Dto()
            {
                Loai = (int)LoaiDatHang.KHACHHANG,
                SoPhieu = code,
                Ngay = DateTime.Now,
                TrangThai = 1,
                TrangThaiTt = 0,
                HinhThucTt = "tienMat",
            };
        }

        public List<NvPhieuDatHangVm.DtoDetail> MergerPhieu(List<string> soPhieu)
        {
            List<NvPhieuDatHangVm.DtoDetail> tempResult = new List<NvPhieuDatHangVm.DtoDetail>();
            foreach (var sp in soPhieu)
            {
                var phieu = Repository.DbSet.FirstOrDefault(x => x.SoPhieu == sp && x.TrangThai == (int)OrderState.IsApproval);
                if (phieu != null)
                {
                    var chiTietPhieu = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieu == phieu.SoPhieu);
                    var data = Mapper.Map<List<NvDatHangChiTiet>, List<NvPhieuDatHangVm.DtoDetail>>(chiTietPhieu.ToList());
                    tempResult.AddRange(data);
                }
            }
            var result = tempResult.GroupBy(x => x.MaHang).Select(u => new NvPhieuDatHangVm.DtoDetail()
            {
                SoLuong = u.Sum(x => x.SoLuong),
                SoLuongBao = u.Sum(x => x.SoLuongBao),
                SoLuongBaoDuyet = u.Sum(x => x.SoLuongBaoDuyet),
                MaBaoBi = u.First().MaBaoBi,
                SoLuongDuyet = u.Sum(x => x.SoLuongDuyet),
                SoLuongLe = u.Sum(x => x.SoLuongLe),
                SoLuongLeDuyet = u.Sum(x => x.SoLuongLeDuyet),
                LuongBao = u.Sum(x => x.LuongBao),
                DonGia = u.First().DonGia,
                DonGiaDuyet = u.First().DonGiaDuyet,
                DonViTinh = u.First().DonViTinh,
                TenHang = u.First().TenHang,
                Barcode = u.First().Barcode,
                MaHd = u.First().MaHd,
                MaHang = u.First().MaHang,
                ThanhTien = u.Sum(x => x.SoLuong) * u.First().DonGia,

            });
            return result.ToList();
        }

        public NvDatHang InsertPhieu(NvPhieuDatHangVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            instance.Calc(); //Tinh lại cac thuộc tính thứ sinh
            var item = AutoMapper.Mapper.Map<NvPhieuDatHangVm.Dto, NvDatHang>(instance);
            item.IsBanBuon = instance.IsBuon ? (int)LoaiDonDatHang.BANBUON : (int)LoaiDonDatHang.BANLE;
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            string _unitCode = GetCurrentUnitCode();
            result.SoPhieu = BuildCode_PTNX(TypeVoucher.DH.ToString(), _unitCode, true);
            result.GenerateMaChungTuPk();
            result = Insert(result);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var dataFilter = instance.DataDetails.Where(x => x.SoLuong > 0).ToList();
            var dataDetails = AutoMapper.Mapper.Map<List<NvPhieuDatHangVm.DtoDetail>, List<NvDatHangChiTiet>>(dataFilter);
            dataDetails.ForEach(x =>
            {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.SoPhieu = result.SoPhieu;
                x.SoPhieuPk = result.SoPhieuPk;
            });
            UnitOfWork.Repository<NvDatHangChiTiet>().InsertRange(dataDetails);
            return result;
        }
        public NvDatHang UpdatePhieu(NvPhieuDatHangVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            instance.Calc();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)OrderState.IsComplete || exsitItem.TrangThai == (int)OrderState.IsRecieved) return null;
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var masterData = Mapper.Map<NvPhieuDatHangVm.Dto, NvDatHang>(instance);
            var contract = UnitOfWork.Repository<MdContract>().DbSet.FirstOrDefault(x => x.MaHd == masterData.MaHd);
            if (contract != null)
            {
                masterData.MaKhachHang = contract.KhachHang;
            }
            var dataFilter = instance.DataDetails.Where(x => x.SoLuong > 0).ToList();
            var detailData = Mapper.Map<List<NvPhieuDatHangVm.DtoDetail>, List<NvDatHangChiTiet>>(dataFilter);
            {
                var detailCollection = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieuPk == exsitItem.SoPhieuPk);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            detailData.ForEach(x =>
            {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang && u.UnitCode.StartsWith(_ParentUnitCode));
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.SoPhieu = exsitItem.SoPhieu;
                x.SoPhieuPk = exsitItem.SoPhieuPk;
                x.MaHd = exsitItem.MaHd;
            });
            UnitOfWork.Repository<NvDatHangChiTiet>().InsertRange(detailData);
            var result = Update(masterData);
            return result;
        }
        public NvDatHang UpdateXuatBan(string soPhieuPK)
        {
            var exsitItem = Repository.DbSet.Where(x=>x.SoPhieuPk == soPhieuPK).FirstOrDefault();
            if(exsitItem == null)
            {
                return null;
            }
            else
            {
                exsitItem.TrangThai = (int)TrangThaiDonHang.THANHCONG;
                var result = Update(exsitItem);
                return result;
            }
        }

        public NvPhieuDatHangVm.ReportModel CreateReport(string id)
        {
            var result = new NvPhieuDatHangVm.ReportModel();
            var exitItem = FindById(id);
            if (exitItem != null)
            {
                result = Mapper.Map<NvDatHang, NvPhieuDatHangVm.ReportModel>(exitItem);
                var detailData = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieuPk == exitItem.SoPhieuPk).ToList();
                result.DataReportDetails = Mapper.Map<List<NvDatHangChiTiet>, List<NvPhieuDatHangVm.ReportDetailModel>>(detailData);
                var customer = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == result.MaKhachHang);
                if (customer != null)
                {
                    result.DienThoai = customer.DienThoai;
                    result.TenKhachHang = customer.TenKH;
                }
            }
            var createDate = DateTime.Now;
            result.CreateDay = createDate.Day;
            result.CreateMonth = createDate.Month;
            result.CreateYear = createDate.Year;
            var unitCode = GetCurrentUnitCode();
            result.TenDonVi = CurrentSetting.GetUnitName(unitCode);
            result.DiaChiDonVi = CurrentSetting.GetUnitAddress(unitCode);
            //GetNhanVien
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                var name = currentUser.Identity.Name;
                var userName = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
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

        public List<NvPhieuDatHangVm.DatHangExpImpModel> ReportDatHangTongHop(NvPhieuDatHangVm.ParameterDatHang pi)
        {
            List<NvPhieuDatHangVm.DatHangExpImpModel> data = new List<NvPhieuDatHangVm.DatHangExpImpModel>();
            DateTime beginDay, endDay;
            string ky = string.Empty;
            beginDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 0, 0, 0);
            endDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 23, 59, 59);
            string merchandiseCodes = "";
            string nhaCungCapCodes = "";
            string NhanVienCodes = "";
            string groupby = "";
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseCodes = _convertToArrayCondition(pi.MerchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(pi.NhaCungCapCodes);
                        NhanVienCodes = _convertToArrayCondition(pi.NhanVienCodes);
                        groupby = pi.GroupBy.ToString();
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupby, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pNhanVienCode = new OracleParameter("pNhanVienCode", OracleDbType.NVarchar2, NhanVienCodes, ParameterDirection.Input);
                        var pTrangThai = new OracleParameter("pTrangThai", OracleDbType.NVarchar2, pi.TrangThaiDatHang, ParameterDirection.Input);

                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, pi.UnitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, pi.ToDate, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, pi.FromDate, ParameterDirection.Input);

                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.DATHANG.DATHANGTONGHOP(:pGroupBy, :pTrangThai, :pNhanVienCode, :pMerchandiseCode, :pNhaCungCapCode,:pUnitCode, :pToDate,:pFromDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy, pTrangThai, pNhanVienCode, pMerchandiseCode, pNhaCungCapCode, pUnitCode, pToDate, pFromDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, donGia;
                            var isSoLuong = decimal.TryParse(reader["SOLUONG"].ToString(), out soLuong);
                            var isDonGia = decimal.TryParse(reader["DONGIA"].ToString(), out donGia);
                            var item = new NvPhieuDatHangVm.DatHangExpImpModel();
                            switch ((int)pi.GroupBy)
                            {
                                case 1:
                                    item = new NvPhieuDatHangVm.DatHangExpImpModel()
                                    {
                                        Code = reader["TRANGTHAI"].ToString(),
                                        Name = _parseTrangThai(reader["TRANGTHAI"].ToString()),
                                        SoLuong = isSoLuong ? soLuong : 0,
                                        DonGia = isDonGia ? donGia : 0,
                                        ThanhTien = (isSoLuong ? soLuong : 0) * (isDonGia ? donGia : 0),
                                    };
                                    data.Add(item);
                                    break;
                                case 2:
                                    item = new NvPhieuDatHangVm.DatHangExpImpModel()
                                    {
                                        //SoPhieu = reader["SOPHIEU"].ToString(),
                                        SoLuong = isSoLuong ? soLuong : 0,
                                        DonGia = isDonGia ? donGia : 0,
                                        ThanhTien = (isSoLuong ? soLuong : 0 ) * (isDonGia ? donGia : 0),
                                        Code = reader["MANHANVIEN"].ToString(),
                                        Name = reader["TENNHANVIEN"].ToString(),
                                    };
                                    data.Add(item);
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    item = new NvPhieuDatHangVm.DatHangExpImpModel()
                                    {
                                        //SoPhieu = reader["SOPHIEU"].ToString(),
                                        SoLuong = isSoLuong ? soLuong : 0,
                                        DonGia = isDonGia ? donGia : 0,
                                        ThanhTien = (isSoLuong ? soLuong : 0) * (isDonGia ? donGia : 0),
                                        Code = reader["MAKHACHHANG"].ToString(),
                                        Name = reader["Tenkh"].ToString(),
                                    };
                                    data.Add(item);
                                    break;

                            }
                        }
                        dbContextTransaction.Commit();
                        return data;
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }

        public List<NvPhieuDatHangVm.DatHangExpImpChiTiet> GetDataReportDatHangChiTiet(NvPhieuDatHangVm.ParameterDatHang pi)
        {
            List<NvPhieuDatHangVm.DatHangExpImpChiTiet> data = new List<NvPhieuDatHangVm.DatHangExpImpChiTiet>();
            List<NvPhieuDatHangVm.DatHangExpImpDetailChiTiet> dataTemp = new List<NvPhieuDatHangVm.DatHangExpImpDetailChiTiet>();

            DateTime beginDay, endDay;
            string ky = string.Empty;
            beginDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 0, 0, 0);
            endDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 23, 59, 59);
            string merchandiseCodes = "";
            string nhaCungCapCodes = "";
            string NhanVienCodes = "";
            string groupby = "";
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseCodes = _convertToArrayCondition(pi.MerchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(pi.NhaCungCapCodes);
                        NhanVienCodes = _convertToArrayCondition(pi.NhanVienCodes);
                        groupby = pi.GroupBy.ToString();
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupby, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pNhanVienCode = new OracleParameter("pNhanVienCode", OracleDbType.NVarchar2, NhanVienCodes, ParameterDirection.Input);
                        var pTrangThai = new OracleParameter("pTrangThai", OracleDbType.NVarchar2, pi.TrangThaiDatHang, ParameterDirection.Input);

                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, pi.UnitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, pi.ToDate, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, pi.FromDate, ParameterDirection.Input);

                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.DATHANG.DATHANGCHITIET(:pGroupBy, :pTrangThai, :pNhanVienCode, :pMerchandiseCode, :pNhaCungCapCode,:pUnitCode, :pToDate,:pFromDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy, pTrangThai, pNhanVienCode, pMerchandiseCode, pNhaCungCapCode, pUnitCode, pToDate, pFromDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, donGia;
                            var isSoLuong = decimal.TryParse(reader["SOLUONG"].ToString(), out soLuong);
                            var isDonGia = decimal.TryParse(reader["DONGIA"].ToString(), out donGia);
                            var item = new NvPhieuDatHangVm.DatHangExpImpDetailChiTiet();
                            switch ((int)pi.GroupBy)
                            {
                                case 1:
                                    item = new NvPhieuDatHangVm.DatHangExpImpDetailChiTiet()
                                    {
                                        Code = reader["TRANGTHAI"].ToString(),
                                        Name = _parseTrangThai(reader["TRANGTHAI"].ToString()),
                                        MaHang = reader["MAHANG"].ToString(),
                                        TenHang = reader["TENHANG"].ToString(),
                                        SoLuong = isSoLuong ? soLuong : 0,
                                        DonGia = isDonGia ? donGia : 0,
                                        ThanhTien = (isSoLuong ? soLuong : 0) * (isDonGia ? donGia : 0),
                                    };
                                    dataTemp.Add(item);
                                    break;
                                case 2:
                                    item = new NvPhieuDatHangVm.DatHangExpImpDetailChiTiet()
                                    {
                                        //SoPhieu = reader["SOPHIEU"].ToString(),
                                        SoLuong = isSoLuong ? soLuong : 0,
                                        DonGia = isDonGia ? donGia : 0,
                                        MaHang = reader["MAHANG"].ToString(),
                                        TenHang = reader["TENHANG"].ToString(),
                                        ThanhTien = (isSoLuong ? soLuong : 0) * (isDonGia ? donGia : 0),
                                        Code = reader["MANHANVIEN"].ToString(),
                                        Name = reader["TENNHANVIEN"].ToString(),
                                    };
                                    dataTemp.Add(item);
                                    break;
                                case 3:
                                    break;
                                case 4:
                                    item = new NvPhieuDatHangVm.DatHangExpImpDetailChiTiet()
                                    {
                                        //SoPhieu = reader["SOPHIEU"].ToString(),
                                        SoLuong = isSoLuong ? soLuong : 0,
                                        DonGia = isDonGia ? donGia : 0,
                                        MaHang = reader["MAHANG"].ToString(),
                                        TenHang = reader["TENHANG"].ToString(),
                                        ThanhTien = (isSoLuong ? soLuong : 0) * (isDonGia ? donGia : 0),
                                        Code = reader["MAKHACHHANG"].ToString(),
                                        Name = reader["Tenkh"].ToString(),
                                    };
                                    dataTemp.Add(item);
                                    break;
                            }
                        }
                        var lstTemp = dataTemp.GroupBy(x => x.Code);
                        lstTemp.ToList().ForEach(x => {
                            NvPhieuDatHangVm.DatHangExpImpChiTiet model = new NvPhieuDatHangVm.DatHangExpImpChiTiet();
                            model.DataDetail = new List<NvPhieuDatHangVm.DatHangExpImpDetailChiTiet>();
                            model.Code = x.Key;
                            var children = dataTemp.Where(i => i.Code == x.Key).ToList();
                            if(children[0]!=null)
                            {
                                model.Name = children[0].Name;
                            }
                            model.DataDetail.AddRange(children);
                            data.Add(model);
                        }); 
                        dbContextTransaction.Commit();
                        return data;
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }

        public MemoryStream ReportDatHangChiTiet(NvPhieuDatHangVm.ParameterDatHang pi)
        {
            List<NvPhieuDatHangVm.DatHangExpImpChiTiet> data = GetDataReportDatHangChiTiet(pi);
            DateTime beginDay = new DateTime(pi.ToDate.Year, pi.ToDate.Month, pi.ToDate.Day, 0, 0, 0);
            DateTime endDay = new DateTime(pi.FromDate.Year, pi.FromDate.Month, pi.FromDate.Day, 0, 0, 0);
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int startRow = 4;
                int startColumn = 1;
                worksheet.Cells[1, 1, 1, 7].Merge = true;
                worksheet.Cells[1, 1].Value = "Báo cáo Đặt hàng"; worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1].Value = string.Format("Từ Ngày: {0}/{1}/{2} Đến Ngày: {3}/{4}/{5}", beginDay.Day, beginDay.Month, beginDay.Year, endDay.Day, endDay.Month, endDay.Year);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 7].Merge = true;
                worksheet.Cells[3, 1, 3, 7].Merge = true;
                worksheet.Cells[3, 1].Value = "Điều kiện, Nhóm theo" ;
                worksheet.Cells[4, 1].Value = "STT"; worksheet.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 2].Value = "Mã" ; worksheet.Cells[5, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 3].Value = "Tên hàng"; worksheet.Cells[5, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 4].Value = "Tên" ; worksheet.Cells[5, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 5].Value = "Đơn giá"; worksheet.Cells[5, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 6].Value = "Số lượng"; worksheet.Cells[5, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[4, 7].Value = "Thành tiền"; worksheet.Cells[5, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                var itemTotal = new NvPhieuDatHangVm.DatHangExpImpChiTiet();
                int currentRow = startRow;
                int stt = 0;
                foreach (var item in data)
                {
                    stt = 0;
                    worksheet.Cells[currentRow, 1, currentRow, 7].Merge = true;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, startColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 245, 76));
                    worksheet.Cells[currentRow, startColumn].Value = item.Code + " - " + item.Name;
                    currentRow++;
                    foreach (var itemdetail in item.DataDetail)
                    {
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = itemdetail.Code;
                        worksheet.Cells[currentRow, startColumn + 2].Value = itemdetail.MaHang;
                        worksheet.Cells[currentRow, startColumn + 3].Value = itemdetail.TenHang;
                        //worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.Name;
                        worksheet.Cells[currentRow, startColumn + 4].Value = itemdetail.DonGia; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 5].Value = itemdetail.SoLuong; worksheet.Cells[currentRow, startColumn + 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 6].Value = itemdetail.ThanhTien; worksheet.Cells[currentRow, startColumn + 6].Style.Numberformat.Format = "#,##0.00";

                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 6].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                        itemTotal.SoLuong = itemTotal.SoLuong + itemdetail.SoLuong;
                        itemTotal.ThanhTien = itemTotal.ThanhTien + itemdetail.ThanhTien;

                        currentRow++;

                    }
                }
                worksheet.Cells[currentRow, 1, currentRow, startColumn + 4].Merge = true;
                worksheet.Cells[currentRow, startColumn].Value = "TỔNG CỘNG"; worksheet.Cells[currentRow, startColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, startColumn + 5].Value = itemTotal.SoLuong; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[currentRow, startColumn + 6].Value = itemTotal.ThanhTien; worksheet.Cells[currentRow, startColumn + 4].Style.Numberformat.Format = "#,##0.00";

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


        public static string _parseTrangThai(string tt)
        {
            string result = "";
            switch(Int32.Parse( tt))
            {
                case 1:
                    result = "Mới";
                    break;
                case 2:
                    result = "Đang xác nhận";
                    break;
                case 3:
                    result = "Đã xác nhận";
                    break;
                case 4:
                    result = "Đơn đang chuyển";
                    break;
                case 5:
                    result = "Đơn thành công";
                    break;
                case 6:
                    result = "Đơn thất bại";
                    break;
                case 7:
                    result = "Đơn chuyển hoàn";
                    break;
                case 8:
                    result = "Đơn trả lại";
                    break;
                case 9:
                    result = "Đơn đổi";
                    break;
                case 10:
                    result = "Đơn hủy";
                    break;
                case 11:
                    result = "Đơn hết";
                    break;

            }
            return result;
        }


        public static string _convertToArrayCondition(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
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

        protected override Expression<Func<NvDatHang, bool>> GetKeyFilter(NvDatHang instance)
        {
            string _unitCode = GetCurrentUnitCode();
            return x => x.SoPhieu == instance.SoPhieu && x.UnitCode == _unitCode;
        }
    }
}
