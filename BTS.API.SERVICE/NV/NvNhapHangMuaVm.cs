using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.ENTITY.Md;
using System.Web;
using System.Security.Claims;

namespace BTS.API.SERVICE.NV
{
    public class NvNhapHangMuaVm
    {
        public class Search : IDataSearch
        {
            public string MaChungTu { get; set; }
            public string MaKhachHang { get; set; }
            public string NoiDung { get; set; }
            public string MaSoThue { get; set; }
            public string MaKhoNhap { get; set; }
            public string MaHoaDon { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public int TrangThai { get; set; }
            public string ICreateBy { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new NvVatTuChungTu().MaChungTu);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new NvVatTuChungTu();

                if (!string.IsNullOrEmpty(this.MaChungTu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaChungTu),
                        Value = this.MaChungTu,
                        Method = FilterMethod.Like
                    });
                }
                if (this.TrangThai != -1)// không tồn tại trạng thái
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TrangThai),
                        Value = this.TrangThai,
                        Method = FilterMethod.EqualTo
                    });
                }
                if (!string.IsNullOrEmpty(this.MaKhachHang))
                {
                    var codes = this.MaKhachHang.Split(',').ToList();
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhachHang),
                        Value = codes,
                        Method = FilterMethod.In
                    });
                }
                if (!string.IsNullOrEmpty(this.NoiDung))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NoiDung),
                        Value = this.NoiDung,
                        Method = FilterMethod.Like
                    });
                }

                if (!string.IsNullOrEmpty(this.MaSoThue))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaSoThue),
                        Value = this.MaSoThue,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaKhoNhap))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhoNhap),
                        Value = this.MaKhoNhap,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaHoaDon))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaHoaDon),
                        Value = this.MaHoaDon,
                        Method = FilterMethod.Like
                    });
                }
                if (this.TuNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayCT),
                        Value = this.TuNgay.Value,
                        Method = FilterMethod.GreaterThanOrEqualTo
                    });
                }
                if (this.DenNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayCT),
                        Value = this.DenNgay.Value.AddDays(1),
                        Method = FilterMethod.LessThan
                    });
                }
                if (!string.IsNullOrEmpty(this.ICreateBy))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.ICreateBy),
                        Value = this.ICreateBy,
                        Method = FilterMethod.Like
                    });
                }
                return result;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }

            public void LoadGeneralParam(string summary)
            {
                MaKhachHang = summary;
                MaChungTu = summary;
                NoiDung = summary;
                TrangThai = -1;
                try
                {
                    TrangThai = int.Parse(summary);
                }
                catch
                {
                }
            }
        }

        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
                DataClauseDetails = new List<DtoClauseDetail>();
                DataDetails = new List<DtoDetail>();
                LstModifield = new List<ListModifield>();
            }
            public string MaChungTu { get; set; }//
            public string MaHoaDon { get; set; }
            public string TenNgh { get; set; }
            public string MaLyDo { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNn { get; set; }
            public string MaChungTuPk { get; set; }
            public string SoPhieuDatHang { get; set; }
            public string MaKhachHang { get; set; }
            public string LoaiPhieu { get; set; }
            public string MaDonViXuat { get; set; }
            public string MaDonViNhan { get; set; }
            public string NoiDung { get; set; }
            public string MaSoThue { get; set; }
            public string MaKhoNhap { get; set; }
            public decimal ThanhTienTruocVat { get; set; }
            public decimal ThanhTienSauVat { get; set; }
            public decimal TienVat { get; set; }
            public string TkCo { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal ChietKhau
            {
                get
                {
                    if (ThanhTienTruocVat != 0)
                    {
                        return (TienChietKhau / ThanhTienTruocVat) * 100;
                    }
                    return 0;
                }
            }
            public int TrangThai { get; set; }
            public int TrangThaiThanhToan { get; set; }
            public DateTime? NgayDuyetPhieu { get; set; }
            public DateTime? NgayCT { get; set; }
            public DateTime? NgayHoaDon { get; set; }

            public string VAT { get; set; }
            public decimal GiaMuaCoVat { get; set; }
            public List<DtoDetail> DataDetails { get; set; }
            public List<DtoClauseDetail> DataClauseDetails { get; set; }
            public List<ListModifield> LstModifield { get; set; }
            public void Calc()
            {
            }
            public void CalcResult()
            {
                //ChietKhau = (TienChietKhau / ThanhTienTruocVat) * 100;

            }
        }

        public class ReportModel
        {
            public ReportModel()
            {
                DataReportDetails = new List<ReportDetailModel>();
                DataReportClause = new List<ReportClauseModel>();
            }
            public string Id { get; set; }
            public string MaChungTu { get; set; }//
            public string MaHoaDon { get; set; }
            public string MaChungTuPk { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string DienThoai { get; set; }
            public string NoiDung { get; set; }
            public string SoPhieuDatHang { get; set; }
            public string MaSoThue { get; set; }
            public string DiaChiKhachHang { get; set; }
            public string MaKhoNhap { get; set; }
            public string TenKho { get; set; }
            public string TienChietKhau { get; set; }
            public decimal ThanhTienTruocVat { get; set; }
            public decimal ThanhTienSauVat { get; set; }
            public decimal TienVat { get; set; }
            public string TkCo { get; set; }
            public int TrangThai { get; set; }
            public DateTime? NgayCT { get; set; }
            public string VAT { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public string Username { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public string NameNhanVienCreate { get; set; }
            public List<ReportDetailModel> DataReportDetails { get; set; }
            public List<ReportClauseModel> DataReportClause { get; set; }
            public void CalcResult()
            {

            }
        }
        public class ReportDetailModel
        {
            public string MaChungTu { get; set; }//
            public string MaChungTuPk { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string DonViTinh { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal LuongBao { get; set; }

        }
        public class ReportClauseModel
        {
            public string MaChungTu { get; set; }//
            public string MaChungTuPk { get; set; }
            public string LoaiPhieu { get; set; }//
            public string TkCo { get; set; }
            public string TkNo { get; set; }
            public decimal SoTien { get; set; }
            public string DoiTuongNo { get; set; }
            public string DoiTuongCo { get; set; }
        }
        public class ListModifield
        {
            public string MaHang { get; set; }
            public decimal DonGia { get; set; } // là giá mua trong nghiệp vụ sửa phiếu nhập mua
            public decimal GiaMuaCoVat { get; set; } // là giá mua(VAT trong nghiệp vụ sửa phiếu nhập mua
            public decimal GiaBanLe { get; set; }
            public decimal GiaBanLeVat { get; set; }
            public decimal GiaBanBuon { get; set; }
            public decimal GiaBanBuonVat { get; set; }
            public decimal TyLeLaiLe { get; set; }
            public decimal TyLeLaiBuon { get; set; }
            public decimal TyLeVatRa { get; set; }
            public decimal TyLeVatVao { get; set; }
        }

        public class DtoDetail
        {
            public string Id { get; set; }
            public string MaChungTu { get; set; }
            public string MaChungTuPk { get; set; }
            public string MaBaoBi { get; set; }
            public string Barcode { get; set; }
            public string MaKhachHang { get; set; }
            public string DonViTinh { get; set; }
            public decimal SoLuongLe { get; set; }
            public decimal LuongBao { get; set; }
            public decimal SoLuongBao { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal GiaBanLe { get; set; }
            public decimal GiaBanLeVat { get; set; }
            public decimal GiaMuaCoVat { get; set; }
            public decimal ThanhTienVAT { get; set; }
            public decimal TyLeLaiLe { get; set; }
            public decimal TyLeLaiBuon { get; set; }
            public decimal TyLeVatVao { get; set; }
            public decimal TyLeVatRa { get; set; }
            public int Index { get; set; }
            public decimal chietkhau { get; set; }
            public string MaLoaiHang { get; set; }
            public string TenLoaiHang { get; set; }
            public decimal SoLuongBaoCT { get; set; }
            public decimal SoLuongCT { get; set; }
            public decimal TienGiamGia { get; set; }
            public decimal GiaVon { get; set; }
            public string MaVatRa { get; set; }
            public string MaVatVao { get; set; }

        }
        public class ExportExcelByMerchandiseTypeDetail
        {
            public string Id { get; set; }
            public string MaChungTu { get; set; }
            public string MaLoaiHang { get; set; }
            public string MaChungTuPk { get; set; }
            public string MaBaoBi { get; set; }
            public string Barcode { get; set; }
            public string MaKhachHang { get; set; }
            public string DonViTinh { get; set; }
            public decimal SoLuongLe { get; set; }
            public decimal LuongBao { get; set; }
            public decimal SoLuongBao { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal GiaBanLe { get; set; }
            public decimal TyLeVatVao { get; set; }
            public int Index { get; set; }
            public decimal chietkhau { get; set; }
        }

        public class DtoClauseDetail
        {
            public string Id { get; set; }
            public string MaChungTu { get; set; }//
            public string MaChungTuPk { get; set; }
            public string LoaiPhieu { get; set; }//
            public string TkCo { get; set; }
            public string TkNo { get; set; }
            public decimal SoTien { get; set; }
            public string DoiTuongNo { get; set; }
            public string DoiTuongCo { get; set; }
        }

        public class ObjectReport
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public decimal SoLuong { get; set; }
            public decimal TienHang { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal TienVat { get; set; }
            public decimal TongTien { get; set; }
        }
        public class ObjectReportCha
        {
            public ObjectReportCha()
            {
                DataDetail = new List<ObjectReportCon>();
            }
            public string Ma { get; set; }
            public string Ten { get; set; }
            public decimal SoLuong { get; set; }
            public decimal TienHang { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal TienVat { get; set; }
            public decimal TongTien { get; set; }
            public decimal GiaBan { get; set; }
            public decimal DonGiaNhap { get; set; }
            public List<ObjectReportCon> DataDetail;
        }
        public class ObjectReportCon
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public string MaCha { get; set; }
            public string TenCha { get; set; }
            public decimal SoLuong { get; set; }
            public decimal TienHang { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal TienVat { get; set; }
            public decimal TongTien { get; set; }
            public decimal GiaBan { get; set; }
            public decimal TienVonChuaVat { get; set; }
            public decimal DonGiaNhap { get; set; }
            public string NgayChungTu { get; set; }
            public virtual string GetParentUnitCode()
            {
                if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
                {
                    var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                    var parentunit = currentUser.Claims.FirstOrDefault(x => x.Type == "parentUnitCode");
                    if (parentunit != null) return parentunit.Value;
                }
                return "";
            }

            public void MapSupplierName(IUnitOfWork unitOfWork)
            {
                var _ParentUnitCode = GetParentUnitCode();
                var customer = unitOfWork.Repository<MdSupplier>().DbSet.FirstOrDefault(x => x.MaNCC == this.Ma && x.UnitCode.StartsWith(_ParentUnitCode));
                if (customer != null)
                {
                    this.Ten = customer.TenNCC;
                }
            }

            public void MapCustomerName(IUnitOfWork unitOfWork)
            {
                var _ParentUnitCode = GetParentUnitCode();
                var customer = unitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == this.Ma && x.UnitCode.StartsWith(_ParentUnitCode));
                if (customer != null)
                {
                    this.Ten = customer.TenKH;
                }
            }
            public void MapWareHouseName(IUnitOfWork unitOfWork)
            {
                var _ParentUnitCode = GetParentUnitCode();
                var wareHouse = unitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == this.Ma && x.UnitCode.StartsWith(_ParentUnitCode));
                if (wareHouse != null)
                {
                    this.Ten = wareHouse.TenKho;
                }
            }
            public void MapGroupName(IUnitOfWork unitOfWork)
            {
                var _ParentUnitCode = GetParentUnitCode();
                var group = unitOfWork.Repository<MdNhomVatTu>().DbSet.FirstOrDefault(x => x.MaNhom == this.Ma && x.UnitCode.StartsWith(_ParentUnitCode));
                if (group != null)
                {
                    this.Ten = group.TenNhom;
                }
            }
            public void MapTypeName(IUnitOfWork unitOfWork)
            {
                var _ParentUnitCode = GetParentUnitCode();
                var type = unitOfWork.Repository<MdMerchandiseType>().DbSet.FirstOrDefault(x => x.MaLoaiVatTu == this.Ma && x.UnitCode.StartsWith(_ParentUnitCode));
                if (type != null)
                {
                    this.Ten = type.TenLoaiVatTu;
                }
            }
            public void MapMerchandiseName(IUnitOfWork unitOfWork)
            {
                var _ParentUnitCode = GetParentUnitCode();
                var type = unitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu == this.Ma && x.UnitCode.StartsWith(_ParentUnitCode));
                if (type != null)
                {
                    this.Ten = type.TenHang;
                }
            }
        }
        public class ObjectReportNMUA
        {
            public ObjectReportNMUA()
            {
                DetailData = new List<ObjectReportCon>();
            }
            public string UnitCode { get; set; }
            public DateTime? ToDate { get; set; }
            public DateTime? FromDate { get; set; }
            public string UnitUserName { get; set; }

            public string GroupType { get; set; }
            public int FromDay { get; set; }
            public int FromMonth { get; set; }
            public int FromYear { get; set; }
            public int ToDay { get; set; }
            public int ToMonth { get; set; }
            public int ToYear { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public string Username { get; set; }
            public List<ObjectReportCon> DetailData { get; set; }
            public void MapUnitUserName(IUnitOfWork unitOfWork)
            {
                var unitUser = unitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == this.UnitCode);
                if (unitUser != null)
                {
                    this.UnitUserName = unitUser.TenDonVi;
                }
            }
            public void CreateDateNow()
            {
                var createDate = DateTime.Now;
                this.CreateDay = createDate.Day;
                this.CreateMonth = createDate.Month;
                this.CreateYear = createDate.Year;
            }

        }
        public class ParameterNMua
        {
            public string UnitCode { get; set; }
            public DateTime ToDate { get; set; }
            public DateTime FromDate { get; set; }
            public string UnitUserCodes { get; set; }
            public string TaxCodes { get; set; }
            public string WareHouseCodes { get; set; }
            public string MerchandiseTypeCodes { get; set; }
            public string MerchandiseCodes { get; set; }
            public string MerchandiseGroupCodes { get; set; }
            public string NhaCungCapCodes { get; set; }
            public TypeGroupInventoryNMua Option { get; set; }
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
            public PHUONGTHUCNHAP PhuongThucNhap { get; set; }
            public TypeDieuChuyenNhan RouteType { get; set; }
            public string LoaiNhapKhac { get; set; }
            public string LoaiChungTu { get; set; }
        }
        public enum TypeGroupInventoryNMua
        {
            kho,
            phieu,
            hangHoa,
            loaiHang,
            nhomHang,
            nhaCungCap,
            donVi,
            loaiThue
        }
        public enum TypeDieuChuyenNhan
        {
            NHANCHUYENKHO = 1,
            NHANSIEUTHITHANHVIEN = 2
        }

        public class ObjectImportExcel
        {
            public string Id { get; set; }
            public string MaChungTu { get; set; }
            public string MaChungTuPk { get; set; }
            public string MaBaoBi { get; set; }
            public string Barcode { get; set; }
            public string MaKhachHang { get; set; }
            public string DonViTinh { get; set; }
            public decimal GiaMuaCoVat { get; set; }
            public decimal SoLuongLe { get; set; }
            public decimal LuongBao { get; set; }
            public decimal SoLuongBao { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal ThanhTienVAT { get; set; }
            public decimal GiaBanLe { get; set; }
            public decimal TyLeVatVao { get; set; }
            public int Index { get; set; }
            public decimal chietkhau { get; set; }
            public string MaLoaiHang { get; set; }
            public string TenLoaiHang { get; set; }
            public decimal TyLeLaile { get; set; }
            public bool Exist { get; set; }
        }
    }
}
