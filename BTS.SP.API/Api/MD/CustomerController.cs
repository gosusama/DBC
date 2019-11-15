using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.Authorize.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ASYNC.DatabaseContext;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Customer")]
    [Route("{id?}")]
    [Authorize]
    public class CustomerController : ApiController
    {
        protected readonly IMdCustomerService _service;
        public CustomerController(IMdCustomerService service)
        {
            _service = service;
        }
        /// <summary>
        /// GET
        /// </summary>
        /// <returns></returns>
        /// 
        /// 
        /// 
        [Route("GetSelectDataFromSQL")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "customer")]
        public IList<ChoiceObj> GetSelectDataFromSQL()
        {
            using (var ctx = new DBCSQL())
            {
                var data = ctx.TDS_Dmkhachhang;
                return data.Where(x => x.Maloaikhach == "NCC").Select(x => new ChoiceObj
                {
                    Value = x.Makhachhang,
                    Text = x.Tenkhachhang
                }).ToList();
            }
        }


        [Route("GetAll_Customer")]
        [CustomAuthorize(Method = "XEM", State = "customer")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll_Customer()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            var countRequest = MemoryCacheHelper.GetValue("countGetAll_CustomerReq");
            int tempCountRequest = 0;

            if (countRequest != null)
            {
                tempCountRequest = (int)MemoryCacheHelper.GetValue("countGetAll_CustomerReq") + 1;
                MemoryCacheHelper.Set("countGetAll_CustomerReq", tempCountRequest);
            }
            else
            {
                MemoryCacheHelper.Add("countGetAll_CustomerReq", 1, DateTimeOffset.UtcNow.AddMinutes(10));
            }

            if (tempCountRequest >= 5)
            {
                var getAll_CustomerResult = MemoryCacheHelper.GetValue("getAll_CustomerResult");
                if (getAll_CustomerResult != null)
                {
                    result = (TransferObj<List<ChoiceObj>>)getAll_CustomerResult;
                }
                else
                {
                    result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaKH, Text = x.MaKH + "|" + x.TenKH, Description = x.TenKH, ExtendValue = x.DiaChi, Id = x.Id }).ToList();
                    MemoryCacheHelper.Add("getAll_CustomerResult", result, DateTimeOffset.UtcNow.AddMinutes(10));
                }
            }
            else
            {
                result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaKH, Text = x.MaKH + "|" + x.TenKH, Description = x.TenKH, ExtendValue = x.DiaChi, Id = x.Id }).ToList();
            }


            return Ok(result);
        }

        [Route("GetSelectData")]
        [CustomAuthorize(Method = "XEM", State = "customer")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            return data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaKH, Text = x.TenKH, Id = x.Id, Description = x.DienThoai, ExtendValue = x.DiaChi }).ToList();
        }
        [Route("GetByUnit/{maDonVi}")]
        [CustomAuthorize(Method = "XEM", State = "customer")]
        public IList<ChoiceObj> GetByUnit(string maDonVi)
        {
            if (string.IsNullOrEmpty(maDonVi)) return null;
            var data = _service.Repository.DbSet;
            return data.Where(x => x.UnitCode == maDonVi).Select(x => new ChoiceObj { Value = x.MaKH, Text = x.TenKH, Id = x.Id }).ToList();
        }


        [Route("PostAsyncFromOracleRoot")]
        [HttpPost]
        [CustomAuthorize(Method = "THEM", State = "customer")]
        public IHttpActionResult PostAsyncFromOracleRoot(MdCustomerVm.Dto khachhang)
        {
            var result = new TransferObj<string>();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            string currentUnitCode = _service.GetCurrentUnitCode();
            MdCustomerVm.Dto dataDongBo = new MdCustomerVm.Dto();
            try
            {
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["LTT.Connection"].ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            OracleCommand cmdRoot = new OracleCommand();
                            cmdRoot.Connection = connection;
                            cmdRoot.InitialLONGFetchSize = 1000;
                            cmdRoot.CommandText = "SELECT * FROM DM_KHACHHANG WHERE (MAKH = '" + khachhang.MaKH + "' OR MATHE = '" + khachhang.MaThe + "') AND UNITCODE = '" + rootUnitcode + "' ";
                            cmdRoot.CommandType = CommandType.Text;
                            OracleDataReader dataReaderRoot = cmdRoot.ExecuteReader();
                            if (!dataReaderRoot.HasRows)
                            {
                                return BadRequest("Không tồn tại khách hàng này");
                            }
                            else
                            {
                                while (dataReaderRoot.Read())
                                {
                                    int TRANGTHAI = 0;
                                    int LOAIKHACHHANG = 0;
                                    decimal SODIEM = 0;
                                    decimal TIENNGUYENGIA = 0;
                                    decimal TIENSALE = 0;
                                    decimal TONGTIEN = 0;
                                    DateTime NGAYDACBIET = DateTime.Now;
                                    DateTime NGAYSINH = DateTime.Now;
                                    dataDongBo.MaKH = dataReaderRoot["MAKH"] != null ? dataReaderRoot["MAKH"].ToString() : "";
                                    dataDongBo.TenKH = dataReaderRoot["TENKH"] != null ? dataReaderRoot["TENKH"].ToString() : "";
                                    dataDongBo.TenKhac = dataDongBo.TenKH;
                                    dataDongBo.DiaChi = dataReaderRoot["DIACHI"] != null ? dataReaderRoot["DIACHI"].ToString() : "";
                                    dataDongBo.TinhThanhPho = dataReaderRoot["TINH/THANHPHO"] != null ? dataReaderRoot["TINH/THANHPHO"].ToString() : "";
                                    dataDongBo.QuanHuyen = dataReaderRoot["QUANHUYEN"] != null ? dataReaderRoot["QUANHUYEN"].ToString() : "";
                                    dataDongBo.MaSoThue = dataReaderRoot["MASOTHUE"] != null ? dataReaderRoot["MASOTHUE"].ToString() : "";
                                    int.TryParse(dataReaderRoot["TRANGTHAI"] != null ? dataReaderRoot["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                    dataDongBo.TrangThai = TRANGTHAI;
                                    dataDongBo.DienThoai = dataReaderRoot["DIENTHOAI"] != null ? dataReaderRoot["DIENTHOAI"].ToString() : "";
                                    dataDongBo.ChungMinhThu = dataReaderRoot["CMTND"] != null ? dataReaderRoot["CMTND"].ToString() : "";
                                    dataDongBo.Email = dataReaderRoot["EMAIL"] != null ? dataReaderRoot["EMAIL"].ToString() : "";
                                    int.TryParse(dataReaderRoot["LOAIKHACHHANG"] != null ? dataReaderRoot["LOAIKHACHHANG"].ToString() : "", out LOAIKHACHHANG);
                                    dataDongBo.LoaiKhachHang = (TypeCustomer)LOAIKHACHHANG;
                                    decimal.TryParse(dataReaderRoot["SODIEM"] != null ? dataReaderRoot["SODIEM"].ToString() : "", out SODIEM);
                                    dataDongBo.SoDiem = SODIEM;
                                    decimal.TryParse(dataReaderRoot["TIENNGUYENGIA"] != null ? dataReaderRoot["TIENNGUYENGIA"].ToString() : "", out TIENNGUYENGIA);
                                    dataDongBo.TienNguyenGia = TIENNGUYENGIA;
                                    decimal.TryParse(dataReaderRoot["TIENSALE"] != null ? dataReaderRoot["TIENSALE"].ToString() : "", out TIENSALE);
                                    dataDongBo.TienSale = TIENSALE;
                                    decimal.TryParse(dataReaderRoot["TONGTIEN"] != null ? dataReaderRoot["TONGTIEN"].ToString() : "", out TONGTIEN);
                                    dataDongBo.TongTien = TONGTIEN;
                                    dataDongBo.MaThe = dataReaderRoot["MATHE"] != null ? dataReaderRoot["MATHE"].ToString() : "";
                                    dataDongBo.GhiChu = dataReaderRoot["MATHE"] != null ? dataReaderRoot["MATHE"].ToString() : "";
                                    dataDongBo.HangKhachHang = dataReaderRoot["HANGKHACHHANG"] != null ? dataReaderRoot["HANGKHACHHANG"].ToString() : "";
                                    dataDongBo.HangKhachHangCu = dataReaderRoot["HANGKHACHHANGCU"] != null ? dataReaderRoot["HANGKHACHHANGCU"].ToString() : "";
                                    DateTime.TryParse(dataReaderRoot["NGAYSINH"] != null ? dataReaderRoot["NGAYSINH"].ToString() : "", out NGAYSINH);
                                    dataDongBo.NgaySinh = NGAYSINH;
                                    DateTime.TryParse(dataReaderRoot["NGAYDACBIET"] != null ? dataReaderRoot["NGAYDACBIET"].ToString() : "", out NGAYDACBIET);
                                    dataDongBo.NgayDacBiet = NGAYDACBIET;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Không tồn tại khách hàng này");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["KNQ.Connection"].ConnectionString))
                {
                    try
                    {
                        if (dataDongBo != null)
                        {
                            connection.Open();
                            if (connection.State == ConnectionState.Open)
                            {
                                OracleCommand cmd = new OracleCommand();
                                cmd.Connection = connection;
                                cmd.InitialLONGFetchSize = 1000;
                                cmd.CommandText = "SELECT * FROM DM_KHACHHANG WHERE (MAKH = '" + khachhang.MaKH + "' OR MATHE = '" + khachhang.MaThe + "') AND UNITCODE = '" + currentUnitCode + "' ";
                                cmd.CommandType = CommandType.Text;
                                OracleDataReader dataReader = cmd.ExecuteReader();
                                if (dataReader.HasRows)
                                {
                                    result.Status = false;
                                    result.Message = "Đã tồn tại khách hàng này tại hệ thống";
                                    return Ok(result);
                                }
                                else
                                {
                                    cmd.CommandText = string.Format(@"INSERT INTO DM_KHACHHANG(ID,MAKH,TENKH,TENKHAC,DIACHI,""TINH/THANHPHO"",QUANHUYEN,MASOTHUE,TRANGTHAI,DIENTHOAI,
                                                                        CMTND,EMAIL,SODIEM,TIENNGUYENGIA,TIENSALE,TONGTIEN,MATHE,GHICHU,HANGKHACHHANG,HANGKHACHHANGCU,NGAYSINH,NGAYDACBIET,
                                                                        TIENNGUYENGIA_CHAMSOC,TONGTIEN_CHAMSOC,NGAYMUAHANG,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE,UNITCODE) 
                                                                        VALUES (:ID,:MAKH,:TENKH,:TENKHAC,:DIACHI,:""TINH/THANHPHO"",:QUANHUYEN,:MASOTHUE,:TRANGTHAI,:DIENTHOAI,:CMTND,:EMAIL,:SODIEM,:TIENNGUYENGIA,
                                                                        :TIENSALE,:TONGTIEN,:MATHE,:GHICHU,:HANGKHACHHANG,:HANGKHACHHANGCU,:NGAYSINH,:NGAYDACBIET,:TIENNGUYENGIA_CHAMSOC,:TONGTIEN_CHAMSOC,:NGAYMUAHANG,
                                                                        :I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE,:UNITCODE)");
                                    cmd.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("MAKH", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaKH;
                                    cmd.Parameters.Add("TENKH", OracleDbType.NVarchar2, 500).Value = dataDongBo.TenKH;
                                    cmd.Parameters.Add("TENKHAC", OracleDbType.NVarchar2, 500).Value = dataDongBo.TenKhac;
                                    cmd.Parameters.Add("DIACHI", OracleDbType.NVarchar2, 500).Value = dataDongBo.DiaChi;
                                    cmd.Parameters.Add("TINH/THANHPHO", OracleDbType.NVarchar2, 50).Value = dataDongBo.TinhThanhPho;
                                    cmd.Parameters.Add("QUANHUYEN", OracleDbType.NVarchar2, 50).Value = dataDongBo.QuanHuyen;
                                    cmd.Parameters.Add("MASOTHUE", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaSoThue;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = dataDongBo.TrangThai;
                                    cmd.Parameters.Add("DIENTHOAI", OracleDbType.NVarchar2, 50).Value = dataDongBo.DienThoai;
                                    cmd.Parameters.Add("CMTND", OracleDbType.NVarchar2, 50).Value = dataDongBo.ChungMinhThu;
                                    cmd.Parameters.Add("EMAIL", OracleDbType.NVarchar2, 100).Value = dataDongBo.Email;
                                    cmd.Parameters.Add("SODIEM", OracleDbType.Decimal).Value = dataDongBo.SoDiem;
                                    cmd.Parameters.Add("TIENNGUYENGIA", OracleDbType.Decimal).Value = dataDongBo.TienNguyenGia;
                                    cmd.Parameters.Add("TIENSALE", OracleDbType.Decimal).Value = dataDongBo.TienSale;
                                    cmd.Parameters.Add("TONGTIEN", OracleDbType.Decimal).Value = dataDongBo.TongTien;
                                    cmd.Parameters.Add("MATHE", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaThe;
                                    cmd.Parameters.Add("GHICHU", OracleDbType.NVarchar2, 500).Value = dataDongBo.GhiChu;
                                    cmd.Parameters.Add("HANGKHACHHANG", OracleDbType.NVarchar2, 50).Value = dataDongBo.HangKhachHang;
                                    cmd.Parameters.Add("HANGKHACHHANGCU", OracleDbType.NVarchar2, 50).Value = dataDongBo.HangKhachHangCu;
                                    cmd.Parameters.Add("NGAYSINH", OracleDbType.Date).Value = dataDongBo.NgaySinh;
                                    cmd.Parameters.Add("NGAYDACBIET", OracleDbType.Date).Value = dataDongBo.NgayDacBiet;
                                    cmd.Parameters.Add("TIENNGUYENGIA_CHAMSOC", OracleDbType.Decimal).Value = dataDongBo.TienNguyenGia_ChamSoc;
                                    cmd.Parameters.Add("TONGTIEN_CHAMSOC", OracleDbType.Decimal).Value = dataDongBo.TongTien_ChamSoc;
                                    cmd.Parameters.Add("NGAYMUAHANG", OracleDbType.Date).Value = dataDongBo.NgayMuaHang;
                                    cmd.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = _service.GetCurrentUnitCode();
                                    OracleTransaction transactionKhachHang;
                                    try
                                    {
                                        transactionKhachHang = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionKhachHang;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionKhachHang.Commit();
                                        if (count > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Đồng bộ thành công";
                                        }
                                        else
                                        {
                                            transactionKhachHang.Rollback();
                                            result.Status = false;
                                            result.Message = "Đồng bộ không thành công";
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    finally
                                    {
                                        connection.Close();
                                    }
                                }
                            }
                        }

                    }
                    catch
                    {
                        return BadRequest("Không tồn tại mặt hàng này");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch
            {
            }
            return Ok(result);
        }

        /// <summary>
        /// GetNewCodeRoot
        /// </summary>
        /// <returns></returns>
        [Route("GetNewCodeRoot")]
        public string GetNewCodeRoot()
        {
            return _service.BuildCodeRoot();
        }

        [Route("PostCustomerToOracleRoot")]
        [CustomAuthorize(Method = "THEM", State = "customer")]
        public async Task<IHttpActionResult> PostCustomerToOracleRoot(MdCustomerVm.Dto instance)
        {
            var result = new TransferObj<MdCustomerVm.Dto>();
            try
            {
                string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
                instance.MaKH = _service.SaveCodeRoot();
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["LTT.Connection"].ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = connection;
                            cmd.InitialLONGFetchSize = 1000;
                            cmd.CommandText = "SELECT MAKH FROM DM_KHACHHANG WHERE MAKH = '" + instance.MaKH + "' OR (DIENTHOAI = '" + instance.DienThoai + "' OR CMTND = '" + instance.ChungMinhThu + "' OR MATHE = '" + instance.MaThe + "') AND UNITCODE = '" + rootUnitcode + "' ";
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                result.Status = false;
                                result.Message = "Đã tồn tại khách hàng này tại hệ thống";
                                return Ok(result);
                            }
                            else
                            {
                                if (instance != null)
                                {
                                    // đồng bộ bảng DM_VATTU
                                    cmd.CommandText = string.Format(@"INSERT INTO DM_KHACHHANG(ID,MAKH,TENKH,TENKHAC,DIACHI,""TINH/THANHPHO"",QUANHUYEN,MASOTHUE,TRANGTHAI,
                                    DIENTHOAI,CMTND,EMAIL,LOAIKHACHHANG,SODIEM,TIENNGUYENGIA,TIENSALE,TONGTIEN,MATHE,NGAYCAPTHE,NGAYHETHAN,
                                    GHICHU,HANGKHACHHANG,HANGKHACHHANGCU,NGAYSINH,NGAYDACBIET,QUENTHE,NGAYCHAMSOC,ISCARE,TIENNGUYENGIA_CHAMSOC,TONGTIEN_CHAMSOC,
                                    GHICHUCU,NGAYMUAHANG,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE,UNITCODE) 
                                    VALUES (:ID,:MAKH,:TENKH,:TENKHAC,:DIACHI,:""TINH/THANHPHO"",:QUANHUYEN,:MASOTHUE,:TRANGTHAI,:DIENTHOAI,:CMTND,:EMAIL,:LOAIKHACHHANG,:SODIEM,
                                    :TIENNGUYENGIA,:TIENSALE,:TONGTIEN,:MATHE,:NGAYCAPTHE,:NGAYHETHAN,:GHICHU,:HANGKHACHHANG,:HANGKHACHHANGCU,:NGAYSINH,:NGAYDACBIET,:QUENTHE,:NGAYCHAMSOC,
                                    :ISCARE,:TIENNGUYENGIA_CHAMSOC,:TONGTIEN_CHAMSOC,:GHICHUCU,:NGAYMUAHANG,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE,:UNITCODE)");
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("MAKH", OracleDbType.NVarchar2, 50).Value = instance.MaKH;
                                    cmd.Parameters.Add("TENKH", OracleDbType.NVarchar2, 500).Value = instance.TenKH;
                                    cmd.Parameters.Add("TENKHAC", OracleDbType.NVarchar2, 500).Value = instance.TenKhac;
                                    cmd.Parameters.Add("DIACHI", OracleDbType.NVarchar2, 500).Value = instance.DiaChi;
                                    cmd.Parameters.Add("TINH/THANHPHO", OracleDbType.NVarchar2, 50).Value = instance.TinhThanhPho;
                                    cmd.Parameters.Add("QUANHUYEN", OracleDbType.NVarchar2, 50).Value = instance.QuanHuyen;
                                    cmd.Parameters.Add("MASOTHUE", OracleDbType.NVarchar2, 50).Value = instance.MaSoThue;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = instance.TrangThai;
                                    cmd.Parameters.Add("DIENTHOAI", OracleDbType.NVarchar2, 50).Value = instance.DienThoai;
                                    cmd.Parameters.Add("CMTND", OracleDbType.NVarchar2, 50).Value = instance.ChungMinhThu;
                                    cmd.Parameters.Add("EMAIL", OracleDbType.NVarchar2, 100).Value = instance.Email;
                                    cmd.Parameters.Add("LOAIKHACHHANG", OracleDbType.Int32).Value = instance.LoaiKhachHang;
                                    cmd.Parameters.Add("SODIEM", OracleDbType.Decimal).Value = instance.SoDiem;
                                    cmd.Parameters.Add("TIENNGUYENGIA", OracleDbType.Decimal).Value = instance.TienNguyenGia;
                                    cmd.Parameters.Add("TIENSALE", OracleDbType.Decimal).Value = instance.TienSale;
                                    cmd.Parameters.Add("TONGTIEN", OracleDbType.Decimal).Value = instance.TongTien;
                                    cmd.Parameters.Add("MATHE", OracleDbType.NVarchar2, 50).Value = instance.MaThe;
                                    cmd.Parameters.Add("NGAYCAPTHE", OracleDbType.Date).Value = instance.NgayCapThe;
                                    cmd.Parameters.Add("NGAYHETHAN", OracleDbType.Date).Value = instance.NgayHetHan;
                                    cmd.Parameters.Add("GHICHU", OracleDbType.NVarchar2, 500).Value = instance.GhiChu;
                                    cmd.Parameters.Add("HANGKHACHHANG", OracleDbType.NVarchar2, 50).Value = instance.HangKhachHang;
                                    cmd.Parameters.Add("HANGKHACHHANGCU", OracleDbType.NVarchar2, 50).Value = instance.HangKhachHangCu;
                                    cmd.Parameters.Add("NGAYSINH", OracleDbType.Date).Value = instance.NgaySinh;
                                    cmd.Parameters.Add("NGAYDACBIET", OracleDbType.Date).Value = instance.NgayDacBiet;
                                    cmd.Parameters.Add("QUENTHE", OracleDbType.Int32).Value = instance.QuenThe;
                                    cmd.Parameters.Add("NGAYCHAMSOC", OracleDbType.Date).Value = instance.NgayChamSoc;
                                    cmd.Parameters.Add("ISCARE", OracleDbType.Int32).Value = instance.IsCare;
                                    cmd.Parameters.Add("TIENNGUYENGIA_CHAMSOC", OracleDbType.Decimal).Value = instance.TienNguyenGia_ChamSoc;
                                    cmd.Parameters.Add("TONGTIEN_CHAMSOC", OracleDbType.Decimal).Value = instance.TongTien_ChamSoc;
                                    cmd.Parameters.Add("GHICHUCU", OracleDbType.NVarchar2, 1000).Value = instance.GhiChuCu;
                                    cmd.Parameters.Add("NGAYMUAHANG", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                                    OracleTransaction transactionKhachHang;
                                    try
                                    {
                                        transactionKhachHang = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionKhachHang;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionKhachHang.Commit();
                                        if (count > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Thêm mới đồng bộ khách hàng thành công";
                                            result.Data = instance;
                                        }
                                        else
                                        {
                                            transactionKhachHang.Rollback();
                                            result.Status = false;
                                            result.Data = null;
                                            result.Message = "Thêm mới đồng bộ khách hàng không thành công";
                                        }
                                    }
                                    catch
                                    {

                                    }
                                    finally
                                    {
                                        connection.Dispose();
                                        connection.Close();
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        return BadRequest("Xảy ra lỗi");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
        }
        /// <summary>
        /// PostSelectDataServerRoot
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [Route("PostSelectDataServerRoot")]
        [CustomAuthorize(Method = "XEM", State = "customer")]
        public IHttpActionResult PostSelectDataServerRoot(JObject jsonData)
        {
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            TransferObj<List<MdCustomerVm.Dto>> result = new TransferObj<List<MdCustomerVm.Dto>>();
            List<MdCustomerVm.Dto> listResult = new List<MdCustomerVm.Dto>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<MdCustomerVm.Dto> filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdCustomerVm.Dto>>();
            listResult = _service.TIMKIEM_KHACHHANG_FROM_ORACLE(filtered.AdvanceData.MaKH, 1, 0, rootUnitcode);
            try
            {
                if (listResult.Count > 0)
                {
                    result.Status = true;
                    result.Message = "Ok";
                    result.Data = listResult;
                }
            }
            catch
            {
                result.Status = false;
                result.Message = "NotFound";
            }
            return Ok(result);
        }
        /// <summary>
        /// POST
        /// </summary>
        /// <returns></returns>
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "customer")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdCustomerVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdCustomer>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdCustomer().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdCustomer>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Query entity
        /// 
        /// 
        /// </summary>
        /// <param name="jsonData">complex data : jsonData.filtered & jsonData.paged</param>
        /// <returns></returns>
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "customer")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var paged = new PagedObj<MdCustomer>();
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdCustomerVm.Search>>();
            try
            {
                paged = ((JObject)postData.paged).ToObject<PagedObj<MdCustomer>>();
            }
            catch (Exception ex)
            {

            }
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdCustomer().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdCustomer().MaKH),
                        Method = OrderMethod.ASC
                    }
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = filterResult.Value;
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("GetNewInstance")]
        public MdCustomer GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
        /// <summary>
        /// Create entity
        /// POST
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(MdCustomer))]
        [Route("Post")]
        [CustomAuthorize(Method = "THEM", State = "customer")]
        public async Task<IHttpActionResult> Post(MdCustomerVm.Dto instance)
        {
            var result = new TransferObj<MdCustomer>();
            var _parentUnitCode = _service.GetParentUnitCode();
            if (instance.IsGenCode)
                instance.MaKH = _service.SaveCode();
            else
            {
                if (instance.MaKH == "")
                {
                    result.Status = false;
                    result.Message = "Mã khách hàng không hợp lệ";
                    return Ok(result);
                }
                else
                {
                    var exist = _service.Repository.DbSet.FirstOrDefault(x => x.MaKH == instance.MaKH && x.UnitCode.StartsWith(_parentUnitCode));
                    if (exist != null)
                    {
                        result.Status = false;
                        result.Message = "Đã tồn tại mã khách hàng này";
                        return Ok(result);
                    }
                }
            }
            try
            {
                if (instance.QuenThe == null) instance.QuenThe = 0;
                var data = Mapper.Map<MdCustomerVm.Dto, MdCustomer>(instance);
                var item = _service.Insert(data);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Message = "Thêm mới thành công";
                result.Data = item;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
            return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
        }

        [ResponseType(typeof(MdCustomer))]
        [CustomAuthorize(Method = "XOA", State = "customer")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdCustomer instance = await _service.Repository.FindAsync(id);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                _service.Delete(instance.Id);
                await _service.UnitOfWork.SaveAsync();
                return Ok(instance);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        /// <summary>
        /// Get by id
        /// GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(MdCustomer))]
        [CustomAuthorize(Method = "XEM", State = "customer")]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        [Route("Update/{id}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "customer")]
        public async Task<IHttpActionResult> Update(string id, MdCustomer instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdCustomer>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                var item = _service.Update(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Data = item;
                result.Message = "Cập nhật thành công";
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }

        [Route("UpdateCustomerToOracleRoot/{id}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "customer")]
        public async Task<IHttpActionResult> UpdateCustomerToOracleRoot(string id, MdCustomerVm.Dto instance)
        {
            var result = new TransferObj<MdCustomerVm.Dto>();
            try
            {
                string parentUnitCode = _service.GetParentUnitCode();
                string unitCode = _service.GetCurrentUnitCode();
                string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["LTT.Connection"].ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            OracleCommand cmdRoot = new OracleCommand();
                            cmdRoot.Connection = connection;
                            cmdRoot.InitialLONGFetchSize = 1000;
                            cmdRoot.CommandText = "SELECT ID,MAKH FROM DM_KHACHHANG WHERE MAKH = '" + instance.MaKH + "' AND UNITCODE = '" + rootUnitcode + "' ";
                            cmdRoot.CommandType = CommandType.Text;
                            OracleDataReader dataReaderRoot = cmdRoot.ExecuteReader();
                            if (!dataReaderRoot.HasRows)
                            {
                                return BadRequest("Không tồn tại khách hàng này");
                            }
                            else
                            {
                                string ID = "";
                                while (dataReaderRoot.Read())
                                {
                                    ID = dataReaderRoot["ID"] != null ? dataReaderRoot["ID"].ToString() : "";
                                }
                                if (!id.Equals(ID))
                                {
                                    return BadRequest();
                                }
                                else
                                {
                                    OracleCommand cmd = new OracleCommand();
                                    cmd.Connection = connection;
                                    cmd.Parameters.Clear();
                                    cmd.CommandText = string.Format(@"UPDATE DM_KHACHHANG SET TENKH=:TENKH,DIACHI=:DIACHI,""TINH/THANHPHO""=:""TINH/THANHPHO"",QUANHUYEN=:QUANHUYEN,
                                    MASOTHUE=:MASOTHUE,TRANGTHAI=:TRANGTHAI,DIENTHOAI=:DIENTHOAI,CMTND=:CMTND,EMAIL=:EMAIL,SODIEM=:SODIEM,TIENNGUYENGIA=:TIENNGUYENGIA,
                                    TIENSALE=:TIENSALE,TONGTIEN=:TONGTIEN,MATHE=:MATHE,NGAYCAPTHE=:NGAYCAPTHE,NGAYHETHAN=:NGAYHETHAN,GHICHU=:GHICHU,HANGKHACHHANG=:HANGKHACHHANG,
                                    HANGKHACHHANGCU=:HANGKHACHHANGCU,NGAYSINH=:NGAYSINH,NGAYDACBIET=:NGAYDACBIET,QUENTHE=:QUENTHE,NGAYCHAMSOC=:NGAYCHAMSOC,
                                    TIENNGUYENGIA_CHAMSOC=:TIENNGUYENGIA_CHAMSOC,TONGTIEN_CHAMSOC=:TONGTIEN_CHAMSOC,GHICHUCU=:GHICHUCU,NGAYMUAHANG=:NGAYMUAHANG,
                                    I_UPDATE_DATE=:I_UPDATE_DATE,I_UPDATE_BY=:I_UPDATE_BY,I_STATE=:I_STATE,UNITCODE=:UNITCODE
                                    WHERE ID = '" + instance.Id + "' AND MAKH = '" + instance.MaKH + "' AND UNITCODE = '" + rootUnitcode + "'");
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("TENKH", OracleDbType.NVarchar2, 500).Value = instance.TenKH;
                                    cmd.Parameters.Add("DIACHI", OracleDbType.NVarchar2, 500).Value = instance.DiaChi;
                                    cmd.Parameters.Add("TINH/THANHPHO", OracleDbType.NVarchar2, 50).Value = instance.TinhThanhPho;
                                    cmd.Parameters.Add("QUANHUYEN", OracleDbType.NVarchar2, 50).Value = instance.QuanHuyen;
                                    cmd.Parameters.Add("MASOTHUE", OracleDbType.NVarchar2, 50).Value = instance.MaSoThue;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = instance.TrangThai;
                                    cmd.Parameters.Add("DIENTHOAI", OracleDbType.NVarchar2, 50).Value = instance.DienThoai;
                                    cmd.Parameters.Add("CMTND", OracleDbType.NVarchar2, 50).Value = instance.ChungMinhThu;
                                    cmd.Parameters.Add("EMAIL", OracleDbType.NVarchar2, 100).Value = instance.Email;
                                    cmd.Parameters.Add("SODIEM", OracleDbType.Decimal).Value = instance.SoDiem;
                                    cmd.Parameters.Add("TIENNGUYENGIA", OracleDbType.Decimal).Value = instance.TienNguyenGia;
                                    cmd.Parameters.Add("TIENSALE", OracleDbType.Decimal).Value = instance.TienSale;
                                    cmd.Parameters.Add("TONGTIEN", OracleDbType.Decimal).Value = instance.TongTien;
                                    cmd.Parameters.Add("MATHE", OracleDbType.NVarchar2, 50).Value = instance.MaThe;
                                    cmd.Parameters.Add("NGAYCAPTHE", OracleDbType.Date).Value = instance.NgayCapThe;
                                    cmd.Parameters.Add("NGAYHETHAN", OracleDbType.Date).Value = instance.NgayHetHan;
                                    cmd.Parameters.Add("GHICHU", OracleDbType.NVarchar2, 500).Value = instance.GhiChu;
                                    cmd.Parameters.Add("HANGKHACHHANG", OracleDbType.NVarchar2, 50).Value = instance.HangKhachHang;
                                    cmd.Parameters.Add("HANGKHACHHANGCU", OracleDbType.NVarchar2, 50).Value = instance.HangKhachHangCu;
                                    cmd.Parameters.Add("NGAYSINH", OracleDbType.Date).Value = instance.NgaySinh;
                                    cmd.Parameters.Add("NGAYDACBIET", OracleDbType.Date).Value = instance.NgayDacBiet;
                                    cmd.Parameters.Add("QUENTHE", OracleDbType.Int32).Value = instance.QuenThe;
                                    cmd.Parameters.Add("NGAYCHAMSOC", OracleDbType.Date).Value = instance.NgayChamSoc;
                                    cmd.Parameters.Add("TIENNGUYENGIA_CHAMSOC", OracleDbType.Decimal).Value = instance.TienNguyenGia_ChamSoc;
                                    cmd.Parameters.Add("TONGTIEN_CHAMSOC", OracleDbType.Decimal).Value = instance.TongTien_ChamSoc;
                                    cmd.Parameters.Add("GHICHUCU", OracleDbType.NVarchar2, 1000).Value = instance.GhiChuCu;
                                    cmd.Parameters.Add("NGAYMUAHANG", OracleDbType.Date).Value = instance.NgayMuaHang;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name + "_" + unitCode;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                                    OracleTransaction transactionCustomer;
                                    try
                                    {
                                        transactionCustomer = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionCustomer;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionCustomer.Commit();
                                        if (count > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Cập nhật đồng bộ thành công";
                                            result.Data = instance;
                                        }
                                        else
                                        {
                                            transactionCustomer.Rollback();
                                            result.Status = false;
                                            result.Message = "Cập nhật đồng bộ không thành công";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        result.Status = false;
                                        result.Data = null;
                                        result.Message = "Cập nhật đồng bộ không thành công";
                                    }
                                    finally
                                    {
                                        connection.Close();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("Không tồn tại mặt hàng này");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
            }
            return Ok(result);
        }

        [Route("GetNewCode")]
        public string GetNewCode()
        {
            return _service.BuildCode();
        }
        [Route("GetForNvByCode/{code}")]
        public async Task<IHttpActionResult> GetForNvByCode(string code)
        {
            MdCustomer result = null;
            var unitCode = _service.GetCurrentUnitCode();
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaKH == code);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }

        [Route("FilterNhaCungCap/{maNhaCungCap}")]
        [ResponseType(typeof(MdCustomer))]
        public MdCustomer FilterTypeMerchandiseCodes(string maNhaCungCap)
        {
            var typeMer = new MdCustomer();
            if (string.IsNullOrEmpty(maNhaCungCap))
            {
                typeMer = null;
            }
            else
            {
                maNhaCungCap = maNhaCungCap.ToUpper();
                maNhaCungCap = maNhaCungCap.Trim();
                var unitCode = _service.GetCurrentUnitCode();
                typeMer = _service.Repository.DbSet.Where(x => x.MaKH == maNhaCungCap).FirstOrDefault(x => x.UnitCode == unitCode);
                if (typeMer != null)
                {
                    return typeMer;
                }
                else
                {
                    typeMer = null;
                }
            }
            return typeMer;
        }

        private bool IsExists(string id)
        {
            return _service.Repository.Find(id) != null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Repository.DataContext.Dispose();
            }
            base.Dispose(disposing);
        }
        [Route("GetAll_DataCity")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll_DataCity()
        {
            var result = new TransferObj();
            var data = _service.UnitOfWork.Repository<MdCity>().DbSet;
            var lstCity = data.Select(x => new ChoiceObj()
            {
                Id = x.CityId,
                Value = x.CityId,
                Text = x.CityName,
            }).ToList();
            if (lstCity.Count > 0)
            {
                result.Data = lstCity;
                result.Status = true;
            }
            else
            {
                result.Data = null;
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetDistrictByCity/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDistrictByCity(string code)
        {
            var result = new TransferObj();
            var data = _service.UnitOfWork.Repository<MdDistricts>().DbSet;
            var lstDistricts = data.Where(x => x.CityId == code).Select(x => new ChoiceObj()
            {
                Parent = x.CityId,
                Id = x.DistrictsId,
                Value = x.DistrictsId,
                Text = x.DistrictsName
            }).ToList();
            if (lstDistricts.Count > 0)
            {
                result.Data = lstDistricts;
                result.Status = true;
            }
            else
            {
                result.Data = null;
                result.Status = false;
            }
            return Ok(result);
        }

        [Route("GetDetails")]
        [HttpPost]
        public async Task<IHttpActionResult> GetDetails(MdCustomerVm.Dto instance)
        {
            var result = new TransferObj<MdCustomer>();
            try
            {
                string parentUnitCode = _service.GetParentUnitCode();
                var data = _service.Repository.DbSet.FirstOrDefault(x => x.MaKH == instance.MaKH && x.UnitCode.StartsWith(parentUnitCode));
                if (data != null)
                {
                    result.Data = data;
                    result.Status = true;
                }
                else
                {
                    result.Data = null;
                    result.Status = false;
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("GetDetailsOracleRoot")]
        [HttpPost]
        public IHttpActionResult GetDetailsOracleRoot(MdCustomerVm.Dto instance)
        {
            var result = new TransferObj<MdCustomerVm.Dto>();
            try
            {
                string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["LTT.Connection"].ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = connection;
                            cmd.InitialLONGFetchSize = 1000;
                            cmd.CommandText = string.Format(@"SELECT ID, MAKH,TENKH,DIACHI,""TINH/THANHPHO"",QUANHUYEN,MASOTHUE,TRANGTHAI,DIENTHOAI,CMTND,EMAIL,
                            SODIEM,TIENNGUYENGIA,TIENSALE,TONGTIEN,MATHE,NGAYCAPTHE,NGAYHETHAN,GHICHU,HANGKHACHHANG,
                            HANGKHACHHANGCU,NGAYSINH,NGAYDACBIET,QUENTHE,NGAYCHAMSOC,TIENNGUYENGIA_CHAMSOC,TONGTIEN_CHAMSOC,GHICHUCU,
                            NGAYMUAHANG FROM DM_KHACHHANG WHERE MAKH = '" + instance.MaKH + "' AND UNITCODE = '" + rootUnitcode + "'");
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader dataReaderCustomer = cmd.ExecuteReader();
                            if (dataReaderCustomer.HasRows)
                            {
                                MdCustomerVm.Dto dto = new MdCustomerVm.Dto();
                                while (dataReaderCustomer.Read())
                                {
                                    int TRANGTHAI = 0;
                                    decimal SODIEM = 0;
                                    decimal TIENNGUYENGIA = 0;
                                    decimal TIENSALE = 0;
                                    decimal TONGTIEN = 0;
                                    dto.Id = dataReaderCustomer["ID"] != null ? dataReaderCustomer["ID"].ToString() : "";
                                    dto.MaKH = dataReaderCustomer["MAKH"] != null ? dataReaderCustomer["MAKH"].ToString() : "";
                                    dto.TenKH = dataReaderCustomer["TENKH"] != null ? dataReaderCustomer["TENKH"].ToString() : "";
                                    dto.DiaChi = dataReaderCustomer["DIACHI"] != null ? dataReaderCustomer["DIACHI"].ToString() : "";
                                    dto.TinhThanhPho = dataReaderCustomer["TINH/THANHPHO"] != null ? dataReaderCustomer["TINH/THANHPHO"].ToString() : "";
                                    dto.QuanHuyen = dataReaderCustomer["QUANHUYEN"] != null ? dataReaderCustomer["QUANHUYEN"].ToString() : "";
                                    dto.MaSoThue = dataReaderCustomer["MASOTHUE"] != null ? dataReaderCustomer["MASOTHUE"].ToString() : "";
                                    if (dataReaderCustomer["TRANGTHAI"] != null)
                                    {
                                        int.TryParse(dataReaderCustomer["TRANGTHAI"].ToString(), out TRANGTHAI);
                                    }
                                    dto.TrangThai = TRANGTHAI;
                                    dto.DienThoai = dataReaderCustomer["DIENTHOAI"] != null ? dataReaderCustomer["DIENTHOAI"].ToString() : "";
                                    dto.ChungMinhThu = dataReaderCustomer["CMTND"] != null ? dataReaderCustomer["CMTND"].ToString() : "";
                                    dto.Email = dataReaderCustomer["EMAIL"] != null ? dataReaderCustomer["EMAIL"].ToString() : "";
                                    if (dataReaderCustomer["SODIEM"] != null)
                                    {
                                        decimal.TryParse(dataReaderCustomer["SODIEM"].ToString(), out SODIEM);
                                    }
                                    dto.SoDiem = SODIEM;
                                    if (dataReaderCustomer["TIENNGUYENGIA"] != null)
                                    {
                                        decimal.TryParse(dataReaderCustomer["TIENNGUYENGIA"].ToString(), out TIENNGUYENGIA);
                                    }
                                    dto.TienNguyenGia = TIENNGUYENGIA;
                                    if (dataReaderCustomer["TIENSALE"] != null)
                                    {
                                        decimal.TryParse(dataReaderCustomer["TIENSALE"].ToString(), out TIENSALE);
                                    }
                                    dto.TienSale = TIENSALE;
                                    if (dataReaderCustomer["TONGTIEN"] != null)
                                    {
                                        decimal.TryParse(dataReaderCustomer["TONGTIEN"].ToString(), out TONGTIEN);
                                    }
                                    dto.TongTien = TONGTIEN;
                                    dto.MaThe = dataReaderCustomer["MATHE"] != null ? dataReaderCustomer["MATHE"].ToString() : "";
                                    DateTime NGAYCAPTHE;
                                    DateTime NGAYHETHAN;
                                    DateTime NGAYSINH;
                                    DateTime NGAYDACBIET;
                                    DateTime NGAYCHAMSOC;
                                    DateTime NGAYMUAHANG;
                                    if (dataReaderCustomer["NGAYCAPTHE"] != null)
                                    {
                                        DateTime.TryParse(dataReaderCustomer["NGAYCAPTHE"].ToString(), out NGAYCAPTHE);
                                        dto.NgayCapThe = NGAYCAPTHE;
                                    }
                                    if (dataReaderCustomer["NGAYHETHAN"] != null)
                                    {
                                        DateTime.TryParse(dataReaderCustomer["NGAYHETHAN"].ToString(), out NGAYHETHAN);
                                        dto.NgayHetHan = NGAYHETHAN;
                                    }
                                    dto.GhiChu = dataReaderCustomer["GHICHU"] != null ? dataReaderCustomer["GHICHU"].ToString() : "";
                                    dto.HangKhachHang = dataReaderCustomer["HANGKHACHHANG"] != null ? dataReaderCustomer["HANGKHACHHANG"].ToString() : "";
                                    dto.HangKhachHangCu = dataReaderCustomer["HANGKHACHHANGCU"] != null ? dataReaderCustomer["HANGKHACHHANGCU"].ToString() : "";
                                    if (dataReaderCustomer["NGAYSINH"] != null)
                                    {
                                        DateTime.TryParse(dataReaderCustomer["NGAYSINH"].ToString(), out NGAYSINH);
                                        dto.NgaySinh = NGAYSINH;
                                    }
                                    if (dataReaderCustomer["NGAYDACBIET"] != null)
                                    {
                                        DateTime.TryParse(dataReaderCustomer["NGAYDACBIET"].ToString(), out NGAYDACBIET);
                                        dto.NgayDacBiet = NGAYDACBIET;
                                    }
                                    int QUENTHE = 0;
                                    if (dataReaderCustomer["QUENTHE"] != null)
                                    {
                                        int.TryParse(dataReaderCustomer["QUENTHE"].ToString(), out QUENTHE);
                                    }
                                    dto.QuenThe = QUENTHE;
                                    if (dataReaderCustomer["NGAYCHAMSOC"] != null)
                                    {
                                        DateTime.TryParse(dataReaderCustomer["NGAYCHAMSOC"].ToString(), out NGAYCHAMSOC);
                                        dto.NgayChamSoc = NGAYCHAMSOC;
                                    }
                                    decimal TIENNGUYENGIA_CHAMSOC = 0;
                                    decimal TONGTIEN_CHAMSOC = 0;
                                    if (dataReaderCustomer["TIENNGUYENGIA_CHAMSOC"] != null)
                                    {
                                        decimal.TryParse(dataReaderCustomer["TIENNGUYENGIA_CHAMSOC"].ToString(), out TIENNGUYENGIA_CHAMSOC);
                                    }
                                    dto.TienNguyenGia_ChamSoc = TIENNGUYENGIA_CHAMSOC;
                                    if (dataReaderCustomer["TONGTIEN_CHAMSOC"] != null)
                                    {
                                        decimal.TryParse(dataReaderCustomer["TONGTIEN_CHAMSOC"].ToString(), out TONGTIEN_CHAMSOC);
                                    }
                                    dto.TongTien_ChamSoc = TONGTIEN_CHAMSOC;
                                    dto.GhiChuCu = dataReaderCustomer["GHICHUCU"] != null ? dataReaderCustomer["GHICHUCU"].ToString() : "";
                                    if (dataReaderCustomer["NGAYMUAHANG"] != null)
                                    {
                                        DateTime.TryParse(dataReaderCustomer["NGAYMUAHANG"].ToString(), out NGAYMUAHANG);
                                        dto.NgayMuaHang = NGAYMUAHANG;
                                    }
                                }
                                result.Data = dto;
                                result.Status = true;
                                result.Message = "Oke";
                            }
                        }
                        else
                        {
                            result.Data = null;
                            result.Status = false;
                            result.Message = "Không có kết nối";
                        }
                    }
                    catch
                    {
                        result.Data = null;
                        result.Status = false;
                        result.Message = "Xảy ra lỗi";
                    }
                    finally
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
