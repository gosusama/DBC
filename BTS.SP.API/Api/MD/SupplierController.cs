using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ASYNC.DatabaseContext;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.SP.API.Utils;
using System.Data.Entity;
using BTS.API.SERVICE.Authorize.Utils;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Supplier")]
    [Route("{id?}")]
    [Authorize]
    public class SupplierController : ApiController
    {
        protected readonly IMdSupplierService _service;
        public SupplierController(IMdSupplierService service)
        {
            _service = service;
        }
        /// <summary>
        /// GET
        /// </summary>
        /// <returns></returns>
        [Route("PostAsyncFromSql")]
        [HttpPost]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> PostAsyncFromSql(TDS_KhachHangVM.Dto khachhang)
        {
            var result = new TransferObj<MdSupplier>();
            using (var ctx = new DBCSQL())
            {
                try
                {
                    var item = ctx.TDS_Dmkhachhang.FirstOrDefault(x => x.Makhachhang == khachhang.Makhachhang);
                    if (item == null) return BadRequest("Không tồn tại bản ghi này");
                    var exsitItem = _service.Repository.DbSet.FirstOrDefault(x => x.MaNCC == item.Makhachhang);
                    if (exsitItem != null)
                    {
                        result.Status = false;
                        result.Message = "Đã tồn tại bản ghi này tại hệ thống";
                        return BadRequest(result.Message);
                    }
                    if (_service.SyncSQLToOracle(item) != null)
                    {
                        await _service.UnitOfWork.SaveAsync();
                        result.Message = "Đồng bộ thành công";
                        result.Status = true;
                    }
                    else
                    {
                        result.Message = "Lỗi khi đồng bộ";
                        result.Status = false;
                        return BadRequest(result.Message);
                    }
                }
                catch (Exception e)
                {
                    WriteLogs.LogError(e);
                    result.Status = false;
                    result.Message = e.Message;
                }
                return Ok(result);
            }
        }

        [Route("PostSelectDataServerRoot")]
        [CustomAuthorize(Method = "THEM", State = "supplier")]
        public IHttpActionResult PostSelectDataServerRoot(JObject jsonData)
        {
            string unitCode = _service.GetCurrentUnitCode();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            TransferObj<List<MdSupplierVm.Dto>> result = new TransferObj<List<MdSupplierVm.Dto>>();
            List<MdSupplierVm.Dto> listResult = new List<MdSupplierVm.Dto>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<MdSupplierVm.Dto> filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdSupplierVm.Dto>>();
            try
            {
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
                            cmd.CommandText = "DONGBO_TIMKIEM_NHACUNGCAP";
                            cmd.CommandType = CommandType.StoredProcedure;
                            string TIEUCHI = string.Empty;
                            if (filtered.AdvanceData.TieuChiTimKiem.Equals("maNCC")) TIEUCHI = "MANHACUNGCAP";
                            else if (filtered.AdvanceData.TieuChiTimKiem.Equals("tenNCC")) TIEUCHI = "TENNHACUNGCAP";
                            else if (filtered.AdvanceData.TieuChiTimKiem.Equals("maSoThue")) TIEUCHI = "MASOTHUE";
                            else if (filtered.AdvanceData.TieuChiTimKiem.Equals("dienThoai")) TIEUCHI = "DIENTHOAI";
                            else TIEUCHI = "MANHACUNGCAP";
                            cmd.Parameters.Add("P_TIEUCHITIMKIEM", OracleDbType.NVarchar2, 50).Value = TIEUCHI;
                            cmd.Parameters.Add("P_MANHACUNGCAP", OracleDbType.NVarchar2, 50).Value = filtered.AdvanceData.MaNCC;
                            cmd.Parameters.Add("P_TENNHACUNGCAP", OracleDbType.NVarchar2, 500).Value = filtered.AdvanceData.TenNCC;
                            cmd.Parameters.Add("P_MASOTHUE", OracleDbType.NVarchar2, 500).Value = filtered.AdvanceData.MaSoThue;
                            cmd.Parameters.Add("P_DIENTHOAI", OracleDbType.NVarchar2, 50).Value = filtered.AdvanceData.DienThoai;
                            cmd.Parameters.Add("P_UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                            cmd.Parameters.Add("CURSOR_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                            OracleDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    int TRANGTHAI = 0;
                                    MdSupplierVm.Dto _DTO = new MdSupplierVm.Dto();
                                    _DTO.Id = dataReader["ID"] != null ? dataReader["ID"].ToString() : "";
                                    _DTO.MaNCC = dataReader["MANCC"] != null ? dataReader["MANCC"].ToString() : "";
                                    _DTO.TenNCC = dataReader["TENNCC"] != null ? dataReader["TENNCC"].ToString() : "";
                                    _DTO.DiaChi = dataReader["DIACHI"] != null ? dataReader["DIACHI"].ToString() : "";
                                    _DTO.MaSoThue = dataReader["MASOTHUE"] != null ? dataReader["MASOTHUE"].ToString() : "";
                                    _DTO.NguoiLienHe = dataReader["NGUOILIENHE"] != null ? dataReader["NGUOILIENHE"].ToString() : "";
                                    _DTO.DienThoai = dataReader["DIENTHOAI"] != null ? dataReader["DIENTHOAI"].ToString() : "";
                                    _DTO.Fax = dataReader["FAX"] != null ? dataReader["FAX"].ToString() : "";
                                    _DTO.ChucVu = dataReader["CHUCVU"] != null ? dataReader["CHUCVU"].ToString() : "";
                                    _DTO.Email = dataReader["EMAIL"] != null ? dataReader["EMAIL"].ToString() : "";
                                    _DTO.XuatXu = dataReader["XUATXU"] != null ? dataReader["XUATXU"].ToString() : "";
                                    int.TryParse(dataReader["TRANGTHAI"] != null ? dataReader["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                    _DTO.TrangThai = TRANGTHAI;
                                    _DTO.UnitCode = dataReader["UNITCODE"] != null ? dataReader["UNITCODE"].ToString() : "";
                                    _DTO.TaiKhoan_NganHang = dataReader["TAIKHOAN_NGANHANG"] != null ? dataReader["TAIKHOAN_NGANHANG"].ToString() : "";
                                    _DTO.ThongTin_NganHang = dataReader["THONGTIN_NGANHANG"] != null ? dataReader["THONGTIN_NGANHANG"].ToString() : "";
                                    listResult.Add(_DTO);
                                }
                                if (listResult.Count > 0)
                                {
                                    result.Status = true;
                                    result.Message = "Ok";
                                    result.Data = listResult;
                                }
                            }
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "NotFound";
                        }
                    }
                    catch(Exception ex)
                    {
                        result.Status = false;
                        result.Message = "NotFound";
                    }
                    finally
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch
            {
                result.Status = false;
                result.Message = "NotFound";
            }
            return Ok(result);
        }
        public string BuildCodeRoot()
        {
            var result = "";
            var type = TypeMasterData.NCC.ToString();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            string maDonViCha = _service.GetParentUnitCode();
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
                        MdIdBuilder config = new MdIdBuilder();
                        cmd.CommandText = "SELECT ID,TYPE,CODE,\"CURRENT\",UNITCODE FROM MD_ID_BUILDER WHERE TYPE = '" + type + "' AND CODE = '" + type + "' AND UNITCODE = '" + maDonViCha + "' AND ROWNUM = 1";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReaderBuilder = cmd.ExecuteReader();
                        if (dataReaderBuilder.HasRows)
                        {
                            while (dataReaderBuilder.Read())
                            {
                                config = new MdIdBuilder
                                {
                                    Id = dataReaderBuilder["ID"].ToString(),
                                    Type = type,
                                    Code = type,
                                    Current = dataReaderBuilder["CURRENT"].ToString(),
                                    UnitCode = maDonViCha,
                                };
                            }
                        }
                        else
                        {
                            config = new MdIdBuilder
                            {
                                Id = Guid.NewGuid().ToString(),
                                Type = type,
                                Code = type,
                                Current = "0000",
                                UnitCode = maDonViCha,
                            };
                        }
                        var soMa = config.GenerateNumber();
                        config.Current = soMa;
                        result = string.Format("{0}", soMa);
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return result;
        }

        public string SaveCodeRoot()
        {
            var result = "";
            var type = TypeMasterData.NCC.ToString();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            string maDonViCha = _service.GetParentUnitCode();
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
                        MdIdBuilder config = new MdIdBuilder();
                        cmd.CommandText = "SELECT ID,TYPE,CODE,\"CURRENT\",UNITCODE FROM MD_ID_BUILDER WHERE TYPE = '" + type + "' AND CODE = '" + type + "' AND UNITCODE = '" + maDonViCha + "' AND ROWNUM = 1";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReaderBuilder = cmd.ExecuteReader();
                        if (dataReaderBuilder.HasRows)
                        {
                            while (dataReaderBuilder.Read())
                            {
                                config = new MdIdBuilder
                                {
                                    Id = dataReaderBuilder["ID"].ToString(),
                                    Type = type,
                                    Code = type,
                                    Current = dataReaderBuilder["CURRENT"].ToString(),
                                    UnitCode = maDonViCha,
                                };
                                result = config.GenerateNumber();
                                config.Current = result;
                                cmd.CommandText = "UPDATE MD_ID_BUILDER SET \"CURRENT\" = '" + result + "' WHERE TYPE = '" + type + "' AND CODE = '" + type + "' AND UNITCODE = '" + maDonViCha + "' ";
                                cmd.CommandType = CommandType.Text;
                                int countUpdate = cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            config = new MdIdBuilder
                            {
                                Id = Guid.NewGuid().ToString(),
                                Type = type,
                                Code = type,
                                Current = "0000",
                                UnitCode = maDonViCha,
                            };
                            result = config.GenerateNumber();
                            config.Current = result;
                            cmd.CommandText = "INSERT INTO MD_ID_BUILDER(ID,TYPE,CODE,CURRENT,UNITCODE) VALUES ('" + config.Id + "','" + config.Type + "','" + config.Code + "','" + config.Current + "','" + config.UnitCode + "')";
                            cmd.CommandType = CommandType.Text;
                            int count = cmd.ExecuteNonQuery();
                        }
                        result = string.Format("{0}", result);
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return result;
        }

        [Route("GetNewCodeRoot")]
        [HttpGet]
        public string GetNewCodeRoot()
        {
            return BuildCodeRoot();
        }
        [Route("PostNhaCungCapToOracleRoot")]
        [CustomAuthorize(Method = "THEM", State = "supplier")]
        public async Task<IHttpActionResult> PostNhaCungCapToOracleRoot(MdSupplierVm.Dto instance)
        {
            var result = new TransferObj<MdSupplierVm.Dto>();
            try
            {
                string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
                instance.MaNCC = SaveCodeRoot();
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
                            cmd.CommandText = "SELECT MANCC FROM DM_NHACUNGCAP WHERE MANCC = '" + instance.MaNCC + "' AND UNITCODE = '" + rootUnitcode + "' ";
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                result.Status = false;
                                result.Message = "Đã tồn tại mã nhà cung cấp này tại hệ thống";
                                return Ok(result);
                            }
                            else
                            {
                                if (instance != null)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.CommandText = string.Format(@"INSERT INTO DM_NHACUNGCAP(ID,MANCC,TENNCC,DIACHI,MASOTHUE,NGUOILIENHE,TRANGTHAI,DIENTHOAI,FAX,CHUCVU,EMAIL,TAIKHOAN_NGANHANG,THONGTIN_NGANHANG,UNITCODE,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE) VALUES 
                                    (:ID,:MANCC,:TENNCC,:DIACHI,:MASOTHUE,:NGUOILIENHE,:TRANGTHAI,:DIENTHOAI,:FAX,:CHUCVU,:EMAIL,:TAIKHOAN_NGANHANG,:THONGTIN_NGANHANG,:UNITCODE,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE)");
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("ID", OracleDbType.NVarchar2, 50).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("MANCC", OracleDbType.NVarchar2, 50).Value = instance.MaNCC;
                                    cmd.Parameters.Add("TENNCC", OracleDbType.NVarchar2, 500).Value = instance.TenNCC;
                                    cmd.Parameters.Add("DIACHI", OracleDbType.NVarchar2, 500).Value = instance.DiaChi;
                                    cmd.Parameters.Add("MASOTHUE", OracleDbType.NVarchar2, 50).Value = instance.MaSoThue;
                                    cmd.Parameters.Add("NGUOILIENHE", OracleDbType.NVarchar2, 300).Value = instance.NguoiLienHe;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = instance.TrangThai;
                                    cmd.Parameters.Add("DIENTHOAI", OracleDbType.NVarchar2, 50).Value = instance.DienThoai;
                                    cmd.Parameters.Add("FAX", OracleDbType.NVarchar2, 50).Value = instance.Fax;
                                    cmd.Parameters.Add("CHUCVU", OracleDbType.NVarchar2, 50).Value = instance.ChucVu;
                                    cmd.Parameters.Add("EMAIL", OracleDbType.NVarchar2, 50).Value = instance.Email;
                                    cmd.Parameters.Add("TAIKHOAN_NGANHANG", OracleDbType.NVarchar2, 30).Value = instance.TaiKhoan_NganHang;
                                    cmd.Parameters.Add("THONGTIN_NGANHANG", OracleDbType.NVarchar2, 500).Value = instance.ThongTin_NganHang;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                                    cmd.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    OracleTransaction transactionVatTu;
                                    try
                                    {
                                        transactionVatTu = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionVatTu;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionVatTu.Commit();
                                        if (count > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Thêm mới thành công";
                                            result.Data = instance;
                                        }
                                        else
                                        {
                                            transactionVatTu.Rollback();
                                            result.Status = false;
                                            result.Data = null;
                                            result.Message = "Thêm mới không thành công";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        result.Status = false;
                                        result.Data = null;
                                        result.Message = "Thêm mới không thành công";
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

        [Route("UpdateNhaCungCapToOracleRoot/{id}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "supplier")]
        public async Task<IHttpActionResult> UpdateNhaCungCapToOracleRoot(string id, MdSupplierVm.Dto instance)
        {
            var result = new TransferObj<MdSupplierVm.Dto>();
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
                            cmdRoot.CommandText = "SELECT ID,MANCC FROM DM_NHACUNGCAP WHERE MANCC = '" + instance.MaNCC + "' AND UNITCODE = '" + rootUnitcode + "' ";
                            cmdRoot.CommandType = CommandType.Text;
                            OracleDataReader dataReaderRoot = cmdRoot.ExecuteReader();
                            if (!dataReaderRoot.HasRows)
                            {
                                return BadRequest("Không tồn tại nhà cung cấp này");
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
                                    cmd.CommandText = string.Format(@"UPDATE DM_NHACUNGCAP SET TENNCC=:TENNCC,DIACHI=:DIACHI,MASOTHUE=:MASOTHUE,NGUOILIENHE=:NGUOILIENHE,
                                    TRANGTHAI=:TRANGTHAI,DIENTHOAI=:DIENTHOAI,FAX=:FAX,CHUCVU=:CHUCVU,EMAIL=:EMAIL,TAIKHOAN_NGANHANG=:TAIKHOAN_NGANHANG,
                                    THONGTIN_NGANHANG=:THONGTIN_NGANHANG,UNITCODE=:UNITCODE,I_UPDATE_DATE=:I_UPDATE_DATE,I_UPDATE_BY=:I_UPDATE_BY,I_STATE=:I_STATE 
                                    WHERE MANCC = '" + instance.MaNCC + "' AND UNITCODE = '" + rootUnitcode + "'");
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("TENNCC", OracleDbType.NVarchar2, 500).Value = instance.TenNCC;
                                    cmd.Parameters.Add("DIACHI", OracleDbType.NVarchar2, 500).Value = instance.DiaChi;
                                    cmd.Parameters.Add("MASOTHUE", OracleDbType.NVarchar2, 50).Value = instance.MaSoThue;
                                    cmd.Parameters.Add("NGUOILIENHE", OracleDbType.NVarchar2, 300).Value = instance.NguoiLienHe;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = instance.TrangThai;
                                    cmd.Parameters.Add("DIENTHOAI", OracleDbType.NVarchar2, 50).Value = instance.DienThoai;
                                    cmd.Parameters.Add("FAX", OracleDbType.NVarchar2, 50).Value = instance.Fax;
                                    cmd.Parameters.Add("CHUCVU", OracleDbType.NVarchar2, 50).Value = instance.ChucVu;
                                    cmd.Parameters.Add("EMAIL", OracleDbType.NVarchar2, 50).Value = instance.Email;
                                    cmd.Parameters.Add("TAIKHOAN_NGANHANG", OracleDbType.NVarchar2, 30).Value = instance.TaiKhoan_NganHang;
                                    cmd.Parameters.Add("THONGTIN_NGANHANG", OracleDbType.NVarchar2, 500).Value = instance.ThongTin_NganHang;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name + "_" + unitCode;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    OracleTransaction transactionVatTu;
                                    try
                                    {
                                        transactionVatTu = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionVatTu;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionVatTu.Commit();
                                        if (count > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Cập nhật đồng bộ thành công";
                                            result.Data = instance;
                                        }
                                        else
                                        {
                                            transactionVatTu.Rollback();
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

        [Route("PostAsyncSupplierFromOracleRoot")]
        [HttpPost]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> PostAsyncSupplierFromOracleRoot(MdSupplierVm.Dto hangHoa)
        {
            var result = new TransferObj<MdSupplier>();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            string currentUnitCode = _service.GetCurrentUnitCode();
            MdSupplierVm.Dto dataDongBo = new MdSupplierVm.Dto();
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
                            cmdRoot.CommandText = "SELECT ID,MANCC,TENNCC,DIACHI,MASOTHUE,NGUOILIENHE,TRANGTHAI,DIENTHOAI,FAX,CHUCVU,EMAIL,XUATXU,UNITCODE,TAIKHOAN_NGANHANG,THONGTIN_NGANHANG FROM DM_NHACUNGCAP WHERE MANCC = '" + hangHoa.MaNCC + "' AND UNITCODE = '" + rootUnitcode + "' ";
                            cmdRoot.CommandType = CommandType.Text;
                            OracleDataReader dataReaderRoot = cmdRoot.ExecuteReader();
                            if (!dataReaderRoot.HasRows)
                            {
                                return BadRequest("Không tồn tại mặt hàng này");
                            }
                            else
                            {
                                while (dataReaderRoot.Read())
                                {
                                    int TRANGTHAI = 0;
                                    dataDongBo.Id = dataReaderRoot["ID"] != null ? dataReaderRoot["ID"].ToString() : "";
                                    dataDongBo.MaNCC = dataReaderRoot["MANCC"] != null ? dataReaderRoot["MANCC"].ToString() : "";
                                    dataDongBo.TenNCC = dataReaderRoot["TENNCC"] != null ? dataReaderRoot["TENNCC"].ToString() : "";
                                    dataDongBo.DiaChi = dataReaderRoot["DIACHI"] != null ? dataReaderRoot["DIACHI"].ToString() : "";
                                    dataDongBo.MaSoThue = dataReaderRoot["MASOTHUE"] != null ? dataReaderRoot["MASOTHUE"].ToString() : "";
                                    dataDongBo.NguoiLienHe = dataReaderRoot["NGUOILIENHE"] != null ? dataReaderRoot["NGUOILIENHE"].ToString() : "";
                                    dataDongBo.DienThoai = dataReaderRoot["DIENTHOAI"] != null ? dataReaderRoot["DIENTHOAI"].ToString() : "";
                                    dataDongBo.Fax = dataReaderRoot["FAX"] != null ? dataReaderRoot["FAX"].ToString() : "";
                                    dataDongBo.ChucVu = dataReaderRoot["CHUCVU"] != null ? dataReaderRoot["CHUCVU"].ToString() : "";
                                    dataDongBo.Email = dataReaderRoot["EMAIL"] != null ? dataReaderRoot["EMAIL"].ToString() : "";
                                    dataDongBo.XuatXu = dataReaderRoot["XUATXU"] != null ? dataReaderRoot["XUATXU"].ToString() : "";
                                    int.TryParse(dataReaderRoot["TRANGTHAI"] != null ? dataReaderRoot["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                    dataDongBo.TrangThai = TRANGTHAI;
                                    dataDongBo.UnitCode = dataReaderRoot["UNITCODE"] != null ? dataReaderRoot["UNITCODE"].ToString() : "";
                                    dataDongBo.TaiKhoan_NganHang = dataReaderRoot["TAIKHOAN_NGANHANG"] != null ? dataReaderRoot["TAIKHOAN_NGANHANG"].ToString() : "";
                                    dataDongBo.ThongTin_NganHang = dataReaderRoot["THONGTIN_NGANHANG"] != null ? dataReaderRoot["THONGTIN_NGANHANG"].ToString() : "";
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

                using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["KNQ.Connection"].ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = connection;
                            cmd.InitialLONGFetchSize = 1000;
                            cmd.CommandText = "SELECT MANCC FROM DM_NHACUNGCAP WHERE MANCC = '" + hangHoa.MaNCC + "' AND UNITCODE = '" + currentUnitCode + "' ";
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                result.Status = false;
                                result.Message = "Đã tồn tại nhà cung cấp này tại hệ thống";
                                return Ok(result);
                            }
                            else
                            {
                                if (dataDongBo != null)
                                {
                                    cmd.CommandText = "INSERT INTO DM_NHACUNGCAP(ID,MANCC,TENNCC,DIACHI,MASOTHUE,NGUOILIENHE,TRANGTHAI,DIENTHOAI,FAX,CHUCVU,EMAIL,TAIKHOAN_NGANHANG,THONGTIN_NGANHANG,UNITCODE,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE) VALUES (:ID,:MANCC,:TENNCC,:DIACHI,:MASOTHUE,:NGUOILIENHE,:TRANGTHAI,:DIENTHOAI,:FAX,:CHUCVU,:EMAIL,:TAIKHOAN_NGANHANG,:THONGTIN_NGANHANG,:UNITCODE,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE)";
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("ID", OracleDbType.NVarchar2, 50).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("MANCC", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaNCC;
                                    cmd.Parameters.Add("TENNCC", OracleDbType.NVarchar2, 500).Value = dataDongBo.TenNCC;
                                    cmd.Parameters.Add("DIACHI", OracleDbType.NVarchar2, 500).Value = dataDongBo.DiaChi;
                                    cmd.Parameters.Add("MASOTHUE", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaSoThue;
                                    cmd.Parameters.Add("NGUOILIENHE", OracleDbType.NVarchar2, 300).Value = dataDongBo.NguoiLienHe;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = dataDongBo.TrangThai;
                                    cmd.Parameters.Add("DIENTHOAI", OracleDbType.NVarchar2, 50).Value = dataDongBo.DienThoai;
                                    cmd.Parameters.Add("FAX", OracleDbType.NVarchar2, 50).Value = dataDongBo.Fax;
                                    cmd.Parameters.Add("CHUCVU", OracleDbType.NVarchar2, 50).Value = dataDongBo.ChucVu;
                                    cmd.Parameters.Add("EMAIL", OracleDbType.NVarchar2, 50).Value = dataDongBo.Email;
                                    cmd.Parameters.Add("TAIKHOAN_NGANHANG", OracleDbType.NVarchar2, 30).Value = dataDongBo.TaiKhoan_NganHang;
                                    cmd.Parameters.Add("THONGTIN_NGANHANG", OracleDbType.NVarchar2, 500).Value = dataDongBo.ThongTin_NganHang;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = _service.GetCurrentUnitCode() ;
                                    cmd.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    OracleTransaction transactionVatTu;
                                    try
                                    {

                                        transactionVatTu = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionVatTu;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionVatTu.Commit();
                                        if (count > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Đồng bộ thành công";
                                        }
                                        else
                                        {
                                            transactionVatTu.Rollback();
                                            result.Status = false;
                                            result.Message = "Đồng bộ không thành công";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        result.Status = false;
                                        result.Data = null;
                                        result.Message = "Đồng bộ không thành công";
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

        [Route("PostAsyncCompareUpdate")]
        [HttpPost]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> PostAsyncCompareUpdate(MdSupplierVm.Dto hangHoa)
        {
            var result = new TransferObj<MdSupplier>();
            string parentUnitCode = _service.GetParentUnitCode();
            string unitCode = _service.GetCurrentUnitCode();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            MdSupplierVm.Dto dataDongBo = new MdSupplierVm.Dto();
            try
            {
                //check mã đồng bộ ở đơn vị cha
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
                            cmdRoot.CommandText = "SELECT ID,MANCC,TENNCC,DIACHI,MASOTHUE,NGUOILIENHE,TRANGTHAI,DIENTHOAI,FAX,CHUCVU,EMAIL,XUATXU,UNITCODE,TAIKHOAN_NGANHANG,THONGTIN_NGANHANG FROM DM_NHACUNGCAP WHERE MANCC = '" + hangHoa.MaNCC + "' AND UNITCODE = '" + rootUnitcode + "' ";
                            cmdRoot.CommandType = CommandType.Text;
                            OracleDataReader dataReaderRoot = cmdRoot.ExecuteReader();
                            if (!dataReaderRoot.HasRows)
                            {
                                return BadRequest("Không tồn tại nhà cung cấp này");
                            }
                            else
                            {
                                while (dataReaderRoot.Read())
                                {
                                    int TRANGTHAI = 0;
                                    dataDongBo.Id = dataReaderRoot["ID"] != null ? dataReaderRoot["ID"].ToString() : "";
                                    dataDongBo.MaNCC = dataReaderRoot["MANCC"] != null ? dataReaderRoot["MANCC"].ToString() : "";
                                    dataDongBo.TenNCC = dataReaderRoot["TENNCC"] != null ? dataReaderRoot["TENNCC"].ToString() : "";
                                    dataDongBo.DiaChi = dataReaderRoot["DIACHI"] != null ? dataReaderRoot["DIACHI"].ToString() : "";
                                    dataDongBo.MaSoThue = dataReaderRoot["MASOTHUE"] != null ? dataReaderRoot["MASOTHUE"].ToString() : "";
                                    dataDongBo.NguoiLienHe = dataReaderRoot["NGUOILIENHE"] != null ? dataReaderRoot["NGUOILIENHE"].ToString() : "";
                                    dataDongBo.DienThoai = dataReaderRoot["DIENTHOAI"] != null ? dataReaderRoot["DIENTHOAI"].ToString() : "";
                                    dataDongBo.Fax = dataReaderRoot["FAX"] != null ? dataReaderRoot["FAX"].ToString() : "";
                                    dataDongBo.ChucVu = dataReaderRoot["CHUCVU"] != null ? dataReaderRoot["CHUCVU"].ToString() : "";
                                    dataDongBo.Email = dataReaderRoot["EMAIL"] != null ? dataReaderRoot["EMAIL"].ToString() : "";
                                    dataDongBo.XuatXu = dataReaderRoot["XUATXU"] != null ? dataReaderRoot["XUATXU"].ToString() : "";
                                    int.TryParse(dataReaderRoot["TRANGTHAI"] != null ? dataReaderRoot["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                    dataDongBo.TrangThai = TRANGTHAI;
                                    dataDongBo.UnitCode = dataReaderRoot["UNITCODE"] != null ? dataReaderRoot["UNITCODE"].ToString() : "";
                                    dataDongBo.TaiKhoan_NganHang = dataReaderRoot["TAIKHOAN_NGANHANG"] != null ? dataReaderRoot["TAIKHOAN_NGANHANG"].ToString() : "";
                                    dataDongBo.ThongTin_NganHang = dataReaderRoot["THONGTIN_NGANHANG"] != null ? dataReaderRoot["THONGTIN_NGANHANG"].ToString() : "";
                                }
                            }
                        }
                    }
                    catch
                    {
                        return BadRequest("Không tồn tại nhà cung cấp này");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                var exists = _service.Repository.DbSet.FirstOrDefault(x => x.MaNCC == hangHoa.MaNCC && x.UnitCode.Equals(unitCode));
                if (exists != null)
                {
                    using (var connection = new OracleConnection(ConfigurationManager.ConnectionStrings["KNQ.Connection"].ConnectionString))
                    {
                        await connection.OpenAsync();
                        OracleTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        // Assign transaction object for a pending local transaction
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            try
                            {
                                command.CommandType = CommandType.Text;
                                command.CommandText = string.Format(@"DELETE FROM DM_NHACUNGCAP WHERE MAVATTU = '{0}'", exists.MaNCC);
                                command.ExecuteNonQuery();
                                transaction.Commit();

                                if (dataDongBo != null)
                                {
                                    OracleCommand cmd = new OracleCommand();
                                    cmd.Connection = connection;
                                    cmd.CommandText = "INSERT INTO DM_NHACUNGCAP(ID,MANCC,TENNCC,DIACHI,MASOTHUE,NGUOILIENHE,TRANGTHAI,DIENTHOAI,FAX,CHUCVU,EMAIL,TAIKHOAN_NGANHANG,THONGTIN_NGANHANG,UNITCODE,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE) VALUES (:ID,:MANCC,:TENNCC,:DIACHI,:MASOTHUE,:NGUOILIENHE,:TRANGTHAI,:DIENTHOAI,:FAX,:CHUCVU,:EMAIL,:TAIKHOAN_NGANHANG,:THONGTIN_NGANHANG,:UNITCODE,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE)";
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("ID", OracleDbType.NVarchar2, 50).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("MANCC", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaNCC;
                                    cmd.Parameters.Add("TENNCC", OracleDbType.NVarchar2, 500).Value = dataDongBo.TenNCC;
                                    cmd.Parameters.Add("DIACHI", OracleDbType.NVarchar2, 500).Value = dataDongBo.DiaChi;
                                    cmd.Parameters.Add("MASOTHUE", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaSoThue;
                                    cmd.Parameters.Add("NGUOILIENHE", OracleDbType.NVarchar2, 300).Value = dataDongBo.NguoiLienHe;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = dataDongBo.TrangThai;
                                    cmd.Parameters.Add("DIENTHOAI", OracleDbType.NVarchar2, 50).Value = dataDongBo.DienThoai;
                                    cmd.Parameters.Add("FAX", OracleDbType.NVarchar2, 50).Value = dataDongBo.Fax;
                                    cmd.Parameters.Add("CHUCVU", OracleDbType.NVarchar2, 50).Value = dataDongBo.ChucVu;
                                    cmd.Parameters.Add("EMAIL", OracleDbType.NVarchar2, 50).Value = dataDongBo.Email;
                                    cmd.Parameters.Add("TAIKHOAN_NGANHANG", OracleDbType.NVarchar2, 30).Value = dataDongBo.TaiKhoan_NganHang;
                                    cmd.Parameters.Add("THONGTIN_NGANHANG", OracleDbType.NVarchar2, 500).Value = dataDongBo.ThongTin_NganHang;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                                    cmd.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    OracleTransaction transactionVatTu;
                                    try
                                    {

                                        transactionVatTu = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionVatTu;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionVatTu.Commit();
                                        if (count > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Đồng bộ thành công";
                                        }
                                        else
                                        {
                                            transactionVatTu.Rollback();
                                            result.Status = false;
                                            result.Message = "Đồng bộ không thành công";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        result.Status = false;
                                        result.Data = null;
                                        result.Message = "Đồng bộ không thành công";
                                    }
                                    finally
                                    {
                                        connection.Close();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                WriteLogs.LogError(e);
                                transaction.Rollback();
                                throw;
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
                else
                {
                    result.Data = null;
                    result.Message = "Không tìm thấy dữ liệu nhà cung cấp";
                    result.Status = false;
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    result.Message = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    result.Status = false;
                    foreach (var ve in eve.ValidationErrors)
                    {
                        WriteLogs.LogError(e);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                WriteLogs.LogError(ex);
                result.Message = ex.ToString();
                result.Status = false;
            }
            return Ok(result);
        }


        [Route("GetDetailByCode/{code}")]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public IHttpActionResult GetDetailByCode(string code)
        {
            try
            {
                string _ParentUnitCode = _service.GetParentUnitCode();
                MdSupplier instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaNCC == code && x.UnitCode.StartsWith(_ParentUnitCode));
                if (instance == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(instance);
                }
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [Route("GetDetailByCodeRoot/{code}")]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public IHttpActionResult GetDetailByCodeRoot(string code)
        {
            string unitCode = _service.GetCurrentUnitCode();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            TransferObj<MdSupplierVm.Dto> result = new TransferObj<MdSupplierVm.Dto>();
            try
            {
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
                            cmd.CommandText = "SELECT ID,MANCC,TENNCC,DIACHI,MASOTHUE,NGUOILIENHE,TRANGTHAI,DIENTHOAI,FAX,CHUCVU,EMAIL,XUATXU,UNITCODE,TAIKHOAN_NGANHANG,THONGTIN_NGANHANG FROM DM_NHACUNGCAP WHERE MANCC = '" + code + "' AND UNITCODE = '" + rootUnitcode + "' ";
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    int TRANGTHAI = 0;
                                    MdSupplierVm.Dto _DTO = new MdSupplierVm.Dto();
                                    _DTO.Id = dataReader["ID"] != null ? dataReader["ID"].ToString() : "";
                                    _DTO.MaNCC = dataReader["MANCC"] != null ? dataReader["MANCC"].ToString() : "";
                                    _DTO.TenNCC = dataReader["TENNCC"] != null ? dataReader["TENNCC"].ToString() : "";
                                    _DTO.DiaChi = dataReader["DIACHI"] != null ? dataReader["DIACHI"].ToString() : "";
                                    _DTO.MaSoThue = dataReader["MASOTHUE"] != null ? dataReader["MASOTHUE"].ToString() : "";
                                    _DTO.NguoiLienHe = dataReader["NGUOILIENHE"] != null ? dataReader["NGUOILIENHE"].ToString() : "";
                                    _DTO.DienThoai = dataReader["DIENTHOAI"] != null ? dataReader["DIENTHOAI"].ToString() : "";
                                    _DTO.Fax = dataReader["FAX"] != null ? dataReader["FAX"].ToString() : "";
                                    _DTO.ChucVu = dataReader["CHUCVU"] != null ? dataReader["CHUCVU"].ToString() : "";
                                    _DTO.Email = dataReader["EMAIL"] != null ? dataReader["EMAIL"].ToString() : "";
                                    _DTO.XuatXu = dataReader["XUATXU"] != null ? dataReader["XUATXU"].ToString() : "";
                                    int.TryParse(dataReader["TRANGTHAI"] != null ? dataReader["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                    _DTO.TrangThai = TRANGTHAI;
                                    _DTO.UnitCode = dataReader["UNITCODE"] != null ? dataReader["UNITCODE"].ToString() : "";
                                    _DTO.TaiKhoan_NganHang = dataReader["TAIKHOAN_NGANHANG"] != null ? dataReader["TAIKHOAN_NGANHANG"].ToString() : "";
                                    _DTO.ThongTin_NganHang = dataReader["THONGTIN_NGANHANG"] != null ? dataReader["THONGTIN_NGANHANG"].ToString() : "";
                                    result.Status = true;
                                    result.Message = "Ok";
                                    result.Data = _DTO;
                                }
                            }
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "NotFound";
                        }
                    }
                    catch
                    {
                        result.Status = false;
                        result.Message = "NotFound";
                    }
                    finally
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
            catch
            {
                result.Status = false;
                result.Message = "NotFound";
            }
            return Ok(result);
        }

        [Route("GetRootUnitCode")]
        [HttpGet]
        public IHttpActionResult GetRootUnitCode()
        {
            var result = new TransferObj<string>();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            result.Data = rootUnitcode;
            result.Status = true;
            return Ok(result);
        }

        [Route("PostDataSQLQuery")]
        [HttpPost]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> PostDataSQLQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<TDS_KhachHangVM.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<TDS_KhachHangVM.Dto>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
            };
            try
            {
                using (var otherContext = new DBCSQL())
                {
                    var service = new ProcedureService<TDS_Dmkhachhang>();
                    // var filterResult = _service.Filter(filtered, query);
                    var filterResult = service.Filter(filtered, otherContext.TDS_Dmkhachhang, query);
                    if (!filterResult.WasSuccessful)
                    {
                        return NotFound();
                    }
                    result.Data = Mapper.Map<PagedObj<TDS_Dmkhachhang>, PagedObj<TDS_KhachHangVM.Dto>>(filterResult.Value);
                    result.Status = true;
                    return Ok(result);
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }




        [Route("GetNewCodeFromSQL/{maLoaiKhachHang}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetNewCodeFromSQL(string maLoaiKhachHang)
        {
            var result = "";
            using (var ctx = new DBCSQL())
            {
                var dbSet = ctx.TDS_Dmcapma;
                var instanceBuilder = dbSet.SingleOrDefault(x => x.Loaima == "FRM_DMKHACHHANG" && x.Mastart == maLoaiKhachHang);
                if (instanceBuilder == null)
                {
                    return BadRequest("Có vấn đề về sinh mã, không thể tạo mới");
                }
                var number = instanceBuilder.GenerateNumber();
                if (maLoaiKhachHang == "NCC")
                {
                    return Ok(number);
                }
                else
                {
                    return Ok(string.Format("{0}{1}", instanceBuilder.Mastart, number));
                }
            }
        }
        [Route("PostToSQL")]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> PostToSQL(TDS_KhachHangVM.Dto instance)
        {
            var result = new TransferObj<TDS_KhachHangVM.Dto>();
            try
            {
                using (var ctx = new DBCSQL())
                {
                    if (!ctx.Database.Exists())
                    {
                        return BadRequest("Không có kết nối tới database SQL");
                    }
                    var dbSet = ctx.TDS_Dmcapma;
                    var instanceBuilder = dbSet.SingleOrDefault(x => x.Loaima == "FRM_DMKHACHHANG" && x.Mastart == instance.Maloaikhach);
                    if (instanceBuilder == null)
                    {
                        return BadRequest("Có vấn đề về sinh mã, không thể tạo mới");
                    }
                    var number = instanceBuilder.GenerateNumber();
                    if (instance.Maloaikhach == "NCC")
                    {
                        instance.Makhachhang = number;
                    }
                    else
                    {
                        instance.Makhachhang = string.Format("{0}{1}", instanceBuilder.Mastart, number);
                    }

                    instanceBuilder.Macappk = number;

                    var item = Mapper.Map<TDS_KhachHangVM.Dto, TDS_Dmkhachhang>(instance);
                    ctx.TDS_Dmkhachhang.Add(item);
                    await ctx.SaveChangesAsync();
                    result.Status = true;
                }
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
            return Ok(result);
        }
        [Route("PutUpDateToSQLAndSync/{maNhaCungCap}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "supplier")]
        public async Task<IHttpActionResult> PutUpDateToSQLAndSync(string maNhaCungCap, TDS_KhachHangVM.Dto instance)
        {

            if (maNhaCungCap != instance.Makhachhang)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdSupplier>();
            try
            {
                using (var ctx = new DBCSQL())
                {
                    var itemSQL = ctx.TDS_Dmkhachhang.FirstOrDefault(x => x.Makhachhang == maNhaCungCap);
                    if (itemSQL == null)
                    {
                        return NotFound();
                    }
                    //Update to SQl
                    itemSQL.Tenkhachhang = instance.Tenkhachhang;
                    itemSQL.Diachi = instance.Diachi;
                    itemSQL.Dienthoai = instance.Dienthoai;
                    itemSQL.Dienthoai2 = instance.Dienthoai2;
                    itemSQL.Fax = instance.Fax;
                    itemSQL.Email = instance.Email;
                    //Sync From SQl
                    var itemOracle = _service.Repository.DbSet.FirstOrDefault(x => x.MaNCC == maNhaCungCap);
                    if (itemOracle != null)
                    {
                        itemOracle.TenNCC = instance.Tenkhachhang;
                        itemOracle.DiaChi = instance.Diachi;
                        itemOracle.DienThoai = instance.Dienthoai;
                        itemOracle.Fax = instance.Fax;
                        itemOracle.Email = instance.Email;
                        itemOracle.ObjectState = ObjectState.Modified;
                    }
                    ctx.SaveChanges();
                    await _service.UnitOfWork.SaveAsync();
                    result.Message = "Cập nhật thành công";
                    result.Status = true;
                    return Ok(result);
                };
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }
        [Route("GetAll_Supplier")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> GetAll_Supplier()
        {
            TransferObj<List<ChoiceObj>> result = new TransferObj<List<ChoiceObj>>();
            DbSet<MdSupplier> data = _service.Repository.DbSet;
            string maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaNCC, Text = x.MaNCC + "|" + x.TenNCC, Description = x.TenNCC, ExtendValue = x.DiaChi, Infomation = x.TinhThanhPho, Id = x.Id }).ToList();
            return Ok(result);
        }

        [Route("GetAll_SupplierRoot")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public IHttpActionResult GetAll_SupplierRoot()
        {
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            var result = new TransferObj<List<ChoiceObj>>();
            List<ChoiceObj> lstResult = new List<ChoiceObj>();
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
                        cmd.CommandText = "SELECT MANCC,TENNCC FROM DM_NHACUNGCAP WHERE UNITCODE = '" + rootUnitcode + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                ChoiceObj _DTO = new ChoiceObj();
                                string MANCC = dataReader["MANCC"] != null ? dataReader["MANCC"].ToString() : "";
                                string TENNCC = dataReader["TENNCC"] != null ? dataReader["TENNCC"].ToString() : "";
                                _DTO.Value = MANCC;
                                _DTO.Text = MANCC + "|" + TENNCC;
                                _DTO.Description = TENNCC;
                                lstResult.Add(_DTO);
                            }
                        }
                        if (lstResult.Count > 0)
                        {
                            result.Status = true;
                            result.Data = lstResult;
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "NotFound";
                        }
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "NotFound";
                    }
                }
                catch
                {
                    result.Status = false;
                    result.Message = "NotFound";
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return Ok(result);
        }
        [Route("GetByUnit/{maDonVi}")]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public IList<ChoiceObj> GetByUnit(string maDonVi)
        {
            if (string.IsNullOrEmpty(maDonVi)) return null;
            var data = _service.Repository.DbSet;
            return data.Where(x => x.UnitCode == maDonVi).Select(x => new ChoiceObj { Value = x.MaNCC, Text = x.MaNCC + "|" + x.TenNCC, Id = x.Id }).ToList();
        }
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdSupplierVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdSupplier>>();
            var maDonViCha = _service.GetParentUnitCode();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdSupplier().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdSupplier().MaNCC),
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

                result.Data = Mapper.Map<PagedObj<MdSupplier>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        /// <summary>
        /// Query entity
        /// POST
        /// </summary>
        /// <param name="jsonData">complex data : jsonData.filtered & jsonData.paged</param>
        /// <returns></returns>
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdSupplierVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdSupplier>>();
            var maDonViCha = _service.GetParentUnitCode();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdBoHang().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdSupplier().MaNCC),
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
        public MdSupplier GetNewInstance()
        {
            return _service.CreateNewInstance();
        }

        /// <summary>
        /// Create entity
        /// POST
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [Route("Post")]
        [ResponseType(typeof(MdSupplier))]
        [CustomAuthorize(Method = "THEM", State = "supplier")]
        public async Task<IHttpActionResult> Post(MdSupplier instance)
        {
            var result = new TransferObj<MdSupplier>();
            instance.MaNCC = _service.SaveCode();
            try
            {
                instance.ICreateBy = _service.GetClaimsPrincipal().Identity.Name;
                instance.ICreateDate = DateTime.Now;
                var item = _service.Insert(instance);
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

        [ResponseType(typeof(MdSupplier))]
        [CustomAuthorize(Method = "XOA", State = "supplier")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdSupplier instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdSupplier))]
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
        /// Update entity
        /// PUT
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        /// 
        [Route("Update/{id}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "supplier")]
        public async Task<IHttpActionResult> Update(string id, MdSupplier instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdSupplier>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                instance.IUpdateBy = _service.GetClaimsPrincipal().Identity.Name;
                instance.IUpdateDate = DateTime.Now;
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
        [Route("GetNewCode")]
        public string GetNewCode()
        {
            return _service.BuildCode();
        }
        [HttpGet]
        [Route("GetForNvByCode/{code}")]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> GetForNvByCode(string code)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaNCC == code && x.UnitCode == unitCode);
            return Ok(instance);
        }

        [HttpGet]
        [Route("CheckExist/{code}")]
        [ResponseType(typeof(MdSupplier))]
        public MdSupplier CheckExist(string code)
        {
            MdSupplier typeSup = new MdSupplier();
            if (string.IsNullOrEmpty(code))
            {
                typeSup = null;
            }
            else
            {
                string unitCode = _service.GetCurrentUnitCode();
                typeSup = _service.Repository.DbSet.Where(x => x.MaNCC == code.ToUpper().Trim()).FirstOrDefault(x => x.UnitCode == unitCode);
                if (typeSup != null)
                {
                    return typeSup;
                }
                else
                {
                    typeSup = null;
                }
            }
            return typeSup;
        }
        [Route("GetNhaCungCapForNvByCode/{code}")]
        [CustomAuthorize(Method = "XEM", State = "supplier")]
        public async Task<IHttpActionResult> GetNhaCungCapForNvByCode(string code)
        {
            MdSupplier result = null;
            var unitCode = _service.GetCurrentUnitCode();
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaNCC == code && x.UnitCode == unitCode);
            if (instance == null)
            {
                return NotFound();
            }
            result = instance;
            return Ok(result);
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

    }
}
