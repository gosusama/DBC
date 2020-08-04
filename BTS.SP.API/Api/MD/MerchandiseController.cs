using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.SERVICE;
using BTS.API.SERVICE.NV;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Web.Hosting;
using OfficeOpenXml;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.ENTITY;
using BTS.API.SERVICE.DCL;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Web.Configuration;
using System.Runtime.InteropServices;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.API.ASYNC.DatabaseContext;
using BTS.SP.API.Utils;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Merchandise")]
    [Route("{id?}")]
    [Authorize]
    public class MerchandiseController : ApiController
    {
        private readonly IMdMerchandiseService _service;
        private readonly IMdMerchandisePriceService _servicePrice;
        private readonly IDclCloseoutService _serviceClosed;
        private readonly IMdPeriodService _servicePeriod;
        private readonly INvRetailsService _serviceRetails;
        public MerchandiseController(IMdMerchandiseService service, IMdMerchandisePriceService price, IDclCloseoutService serviceClosed, IMdPeriodService servicePeriod, INvRetailsService serviceRetails)
        {
            _service = service;
            _servicePrice = price;
            _serviceClosed = serviceClosed;
            _servicePeriod = servicePeriod;
            _serviceRetails = serviceRetails;
        }
        /// <summary>
        /// POST
        /// </summary>
        /// <returns></returns>
        [Route("GetNewCodeFromSQL/{maLoai}")]
        [HttpGet]
        public string GetNewCodeFromSQL(string maLoai)
        {
            var result = "";

            using (var ctx = new DBCSQL())
            {
                var dbSet = ctx.TDS_Dmcapma;
                var instanceBuilder = dbSet.FirstOrDefault(x => x.Loaima == "FRM_MATHANG" && x.Mastart == maLoai);
                if (instanceBuilder != null)
                {
                    result = string.Format("{0}{1}", instanceBuilder.Mastart, instanceBuilder.GenerateNumber());
                }
            }
            return result;
        }
        [Route("PostMatHangToSQL")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public async Task<IHttpActionResult> PostMatHangToSQL(TDS_MatHangVm.Dto instance)
        {
            var result = new TransferObj<TDS_MatHangVm.Dto>();
            try
            {
                using (var ctx = new DBCSQL())
                {
                    if (!ctx.Database.Exists())
                    {
                        return BadRequest("Không có kết nối tới database SQL");
                    }
                    var dbSet = ctx.TDS_Dmcapma;
                    var instanceBuilder = dbSet.SingleOrDefault(x => x.Loaima == "FRM_MATHANG" && x.Mastart == instance.Manganh);
                    if (instanceBuilder == null)
                    {
                        return BadRequest("Có vấn đề về sinh mã, không thể tạo mới");
                    }
                    var number = instanceBuilder.GenerateNumber();
                    instance.Masieuthi = string.Format("{0}{1}", instanceBuilder.Mastart, number);
                    instanceBuilder.Macappk = number;

                    var item = Mapper.Map<TDS_MatHangVm.Dto, TDS_Dmmathang>(instance);
                    var itemDetail = Mapper.Map<TDS_MatHangVm.Dto, TDS_Dmgiaban>(instance);
                    if (!string.IsNullOrEmpty(item.Itemcode))
                    {
                        item.Itemcode = _service.SaveCodeCanDienTuFromSQL(ctx);
                    }
                    List<string> listExist = new List<string>();
                    var strBarcode = _service.CheckBarcodeAtSQL(item.Barcode);
                    if (!string.IsNullOrEmpty(strBarcode) && strBarcode.Length >= 8)
                    {
                        var barcodeCollection = strBarcode.Split(';');
                        barcodeCollection = barcodeCollection.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                        foreach (var code in barcodeCollection)
                        {
                            try
                            {
                                var firstOrDefault =
                                  _service.Repository.DbSet.FirstOrDefault(x => x.Barcode.Contains(code));
                                if (firstOrDefault != null)
                                {
                                    string str = firstOrDefault.MaVatTu;
                                    listExist.Add(str);
                                }
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                        if (listExist.Count > 0)
                        {
                            string returnExist = string.Join(",", listExist.ToArray());
                            result.Message = string.Format(@"Mã này bị trùng barcode với mã: {0}", returnExist);
                            result.Status = false;
                            result.Data = null;
                        }
                        else
                        {
                            result.Status = false;
                            result.Data = null;
                            result.Message = "Không tìm thấy mã hoặc barcode trùng";
                        }
                    }
                    else
                    {
                        ctx.TDS_Dmmathang.Add(item);
                        itemDetail.Madonvi = "0001";
                        itemDetail.Masieuthi = item.Masieuthi;
                        ctx.TDS_Dmgiaban.Add(itemDetail);
                        if (await ctx.SaveChangesAsync() > 0)
                        {
                            result.Status = true;
                            result.Message = "Thêm mới thành công";
                            result.Data = null;
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "Lỗi khi lưu cơ sở dữ liệu";
                            result.Data = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                WriteLogs.LogError(e);
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        private string CheckExistBarcode(string barcodes)
        {
            var result = "";
            if (!string.IsNullOrWhiteSpace(barcodes))
            {
                var barcodeCollection = barcodes.Split(';');
                barcodeCollection = barcodeCollection.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                foreach (var code in barcodeCollection)
                {
                    try
                    {
                        using (var ctx = new ERPContext())
                        {
                            var str = string.Format("SELECT Barcode FROM DM_VATTU WHERE Barcode LIKE '%;{0};%'", code);
                            var data = ctx.Database.SqlQuery<string>(str);
                            if (data.Count() > 0)
                            {
                                result = string.Format("{0};{1}", result, code);
                            }
                        }

                    }
                    catch (Exception e)
                    {

                        throw e;
                    }
                }
            }
            return result;
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

        [Route("PostMatHangToOracleRoot")]
        [CustomAuthorize(Method = "THEM", State = "merchandise")]
        public async Task<IHttpActionResult> PostMatHangToOracleRoot(MdMerchandiseVm.Dto instance)
        {
            var result = new TransferObj<MdMerchandiseVm.Dto>();
            try
            {
                string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
                instance.MaVatTu = SaveCodeRoot(instance.MaLoaiVatTu);
                if (!string.IsNullOrEmpty(instance.ItemCode))
                {
                    instance.ItemCode = SaveCodeCanDienTuOracleRoot();
                }
                var strBarcode = CheckExistBarcode(instance.Barcode);
                if (!string.IsNullOrEmpty(strBarcode))
                {
                    result.Status = false;
                    result.Message = "Trùng barcode: " + strBarcode;
                    result.Data = null;
                    return Ok(result);
                }
                else
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
                                cmd.CommandText = "SELECT MAVATTU FROM V_VATTU_GIABAN WHERE MAVATTU = '" + instance.MaVatTu + "' AND MADONVI = '" + rootUnitcode + "' ";
                                cmd.CommandType = CommandType.Text;
                                OracleDataReader dataReader = cmd.ExecuteReader();
                                if (dataReader.HasRows)
                                {
                                    result.Status = false;
                                    result.Message = "Đã tồn tại mặt hàng này tại hệ thống";
                                    return Ok(result);
                                }
                                else
                                {
                                    if (instance != null)
                                    {
                                        // đồng bộ bảng DM_VATTU
                                        cmd.CommandText = "INSERT INTO DM_VATTU(ID,MAVATTU,TENVATTU,TENVIETTAT,DONVITINH,MAKEHANG,MALOAIVATTU,MANHOMVATTU,MABAOBI,MAKHACHHANG,MANCC,GIAMUA,GIABANLE,GIABANBUON,TYLELAIBUON,TYLELAILE,BARCODE,MAKHAC,MAVATVAO,TYLE_VAT_VAO,MAVATRA,TYLE_VAT_RA,GIABANVAT,TRANGTHAI,ITEMCODE,MASIZE,MACOLOR,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE,UNITCODE,CHIETKHAUNCC,SOTONMAX,SOTONMIN) VALUES (:ID,:MAVATTU,:TENVATTU,:TENVIETTAT,:DONVITINH,:MAKEHANG,:MALOAIVATTU,:MANHOMVATTU,:MABAOBI,:MAKHACHHANG,:MANCC,:GIAMUA,:GIABANLE,:GIABANBUON,:TYLELAIBUON,:TYLELAILE,:BARCODE,:MAKHAC,:MAVATVAO,:TYLE_VAT_VAO,:MAVATRA,:TYLE_VAT_RA,:GIABANVAT,:TRANGTHAI,:ITEMCODE,:MASIZE,:MACOLOR,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE,:UNITCODE,:CHIETKHAUNCC,:SOTONMAX,:SOTONMIN)";
                                        cmd.CommandType = CommandType.Text;
                                        cmd.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                        cmd.Parameters.Add("MAVATTU", OracleDbType.NVarchar2, 50).Value = instance.MaVatTu;
                                        cmd.Parameters.Add("TENVATTU", OracleDbType.NVarchar2, 200).Value = instance.TenVatTu;
                                        cmd.Parameters.Add("TENVIETTAT", OracleDbType.NVarchar2, 200).Value = instance.TenVatTu;
                                        cmd.Parameters.Add("DONVITINH", OracleDbType.NVarchar2, 50).Value = instance.DonViTinh;
                                        cmd.Parameters.Add("MAKEHANG", OracleDbType.NVarchar2, 50).Value = instance.MaKeHang;
                                        cmd.Parameters.Add("MALOAIVATTU", OracleDbType.NVarchar2, 50).Value = instance.MaLoaiVatTu;
                                        cmd.Parameters.Add("MANHOMVATTU", OracleDbType.NVarchar2, 50).Value = instance.MaNhomVatTu;
                                        cmd.Parameters.Add("MABAOBI", OracleDbType.NVarchar2, 50).Value = instance.MaBaoBi;
                                        cmd.Parameters.Add("MAKHACHHANG", OracleDbType.NVarchar2, 50).Value = instance.MaKhachHang;
                                        cmd.Parameters.Add("MANCC", OracleDbType.NVarchar2, 50).Value = instance.MaNCC;
                                        cmd.Parameters.Add("GIAMUA", OracleDbType.Decimal).Value = instance.GiaMua;
                                        cmd.Parameters.Add("GIABANLE", OracleDbType.Decimal).Value = instance.GiaBanLe;
                                        cmd.Parameters.Add("GIABANBUON", OracleDbType.Decimal).Value = instance.GiaBanBuon;
                                        cmd.Parameters.Add("TYLELAIBUON", OracleDbType.Decimal).Value = instance.TyLeLaiBuon;
                                        cmd.Parameters.Add("TYLELAILE", OracleDbType.Decimal).Value = instance.TyLeLaiLe;
                                        cmd.Parameters.Add("BARCODE", OracleDbType.NVarchar2, 2000).Value = instance.Barcode;
                                        cmd.Parameters.Add("MAKHAC", OracleDbType.NVarchar2, 50).Value = instance.MaKhac;
                                        cmd.Parameters.Add("MAVATVAO", OracleDbType.NVarchar2, 50).Value = instance.MaVatVao;
                                        cmd.Parameters.Add("TYLE_VAT_VAO", OracleDbType.Decimal).Value = instance.TyLeVatVao;
                                        cmd.Parameters.Add("MAVATRA", OracleDbType.NVarchar2, 50).Value = instance.MaVatRa;
                                        cmd.Parameters.Add("TYLE_VAT_RA", OracleDbType.Decimal).Value = instance.TyLeVatRa;
                                        cmd.Parameters.Add("GIABANVAT", OracleDbType.Decimal).Value = instance.GiaBanLeVat;
                                        cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = instance.TrangThai;
                                        cmd.Parameters.Add("ITEMCODE", OracleDbType.NVarchar2, 50).Value = instance.ItemCode;
                                        cmd.Parameters.Add("MASIZE", OracleDbType.NVarchar2, 50).Value = instance.MaSize;
                                        cmd.Parameters.Add("MACOLOR", OracleDbType.NVarchar2, 50).Value = instance.MaColor;
                                        cmd.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                        cmd.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                        cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                        cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                        cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                        cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                                        cmd.Parameters.Add("CHIETKHAUNCC", OracleDbType.Decimal).Value = 0;
                                        cmd.Parameters.Add("SOTONMAX", OracleDbType.Decimal).Value = 0;
                                        cmd.Parameters.Add("SOTONMIN", OracleDbType.Decimal).Value = 0;
                                        // đồng bộ bảng DM_VATTU_GIACA
                                        OracleCommand cmdGiaBan = new OracleCommand();
                                        cmdGiaBan.Connection = connection;
                                        cmdGiaBan.InitialLONGFetchSize = 1000;
                                        cmdGiaBan.CommandText = "INSERT INTO DM_VATTU_GIACA(ID,MAVATTU,MADONVI,GIAVON,GIAMUAVAT,GIAMUA,GIABANLE,GIABANBUON,TY_LELAI_BUON,TY_LELAI_LE,MAVATVAO,TYLE_VAT_VAO,MAVATRA,TYLE_VAT_RA,GIA_BANLE_VAT,GIA_BANBUON_VAT,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE,UNITCODE,SOTONMAX,SOTONMIN) VALUES (:ID,:MAVATTU,:MADONVI,:GIAVON,:GIAMUAVAT,:GIAMUA,:GIABANLE,:GIABANBUON,:TY_LELAI_BUON,:TY_LELAI_LE,:MAVATVAO,:TYLE_VAT_VAO,:MAVATRA,:TYLE_VAT_RA,:GIA_BANLE_VAT,:GIA_BANBUON_VAT,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE,:UNITCODE,:SOTONMAX,:SOTONMIN)";
                                        cmdGiaBan.CommandType = CommandType.Text;
                                        cmdGiaBan.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                        cmdGiaBan.Parameters.Add("MAVATTU", OracleDbType.NVarchar2, 50).Value = instance.MaVatTu;
                                        cmdGiaBan.Parameters.Add("MADONVI", OracleDbType.NVarchar2, 200).Value = rootUnitcode;
                                        cmdGiaBan.Parameters.Add("GIAVON", OracleDbType.Decimal).Value = instance.GiaVon;
                                        cmdGiaBan.Parameters.Add("GIAMUAVAT", OracleDbType.Decimal).Value = instance.GiaMuaVat;
                                        cmdGiaBan.Parameters.Add("GIAMUA", OracleDbType.Decimal).Value = instance.GiaMua;
                                        cmdGiaBan.Parameters.Add("GIABANLE", OracleDbType.Decimal).Value = instance.GiaBanLe;
                                        cmdGiaBan.Parameters.Add("GIABANBUON", OracleDbType.Decimal).Value = instance.GiaBanBuon;
                                        cmdGiaBan.Parameters.Add("TY_LELAI_BUON", OracleDbType.Decimal).Value = instance.TyLeLaiBuon;
                                        cmdGiaBan.Parameters.Add("TY_LELAI_LE", OracleDbType.Decimal).Value = instance.TyLeLaiLe;
                                        cmdGiaBan.Parameters.Add("MAVATVAO", OracleDbType.NVarchar2, 200).Value = instance.MaVatVao;
                                        cmdGiaBan.Parameters.Add("TYLE_VAT_VAO", OracleDbType.Decimal).Value = instance.TyLeVatVao;
                                        cmdGiaBan.Parameters.Add("MAVATRA", OracleDbType.NVarchar2, 200).Value = instance.MaVatRa;
                                        cmdGiaBan.Parameters.Add("TYLE_VAT_RA", OracleDbType.Decimal).Value = instance.TyLeVatRa;
                                        cmdGiaBan.Parameters.Add("GIA_BANLE_VAT", OracleDbType.Decimal).Value = instance.GiaBanLeVat;
                                        cmdGiaBan.Parameters.Add("GIA_BANBUON_VAT", OracleDbType.Decimal).Value = instance.GiaBanBuonVat;
                                        cmdGiaBan.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                        cmdGiaBan.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                        cmdGiaBan.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                        cmdGiaBan.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                        cmdGiaBan.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                        cmdGiaBan.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                                        cmdGiaBan.Parameters.Add("SOTONMAX", OracleDbType.Decimal).Value = 0;
                                        cmdGiaBan.Parameters.Add("SOTONMIN", OracleDbType.Decimal).Value = 0;
                                        OracleTransaction transactionVatTu;
                                        OracleTransaction transactionGiaBan;
                                        try
                                        {

                                            transactionVatTu = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                            cmd.Transaction = transactionVatTu;
                                            int count = cmd.ExecuteNonQuery();
                                            transactionVatTu.Commit();
                                            transactionGiaBan = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                            cmdGiaBan.Transaction = transactionGiaBan;
                                            int countGiaBan = cmdGiaBan.ExecuteNonQuery();
                                            transactionGiaBan.Commit();
                                            if (count > 0 && countGiaBan > 0)
                                            {
                                                result.Status = true;
                                                result.Message = "Thêm mới thành công";
                                                result.Data = instance;
                                            }
                                            else
                                            {
                                                transactionVatTu.Rollback();
                                                transactionGiaBan.Rollback();
                                                result.Status = false;
                                                result.Data = null;
                                                result.Message = "Thêm mới không thành công";
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
                            return BadRequest("Xảy ra lỗi");
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
        }

        [Route("GetNewCanDienTuFromSQL")]
        public string GetNewCanDienTuFromSQL()
        {
            return _service.BuildCodeCanDienTuFromSQL();
        }
        //log4
        [Route("GetNewCanDienTuOracleRoot")]
        public string GetNewCanDienTuOracleRoot()
        {
            var result = "";
            var type = TypeMasterData.CANDIENTU.ToString();
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
                                Current = "00000",
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
        public string SaveCodeCanDienTuOracleRoot()
        {
            var result = "";
            var type = TypeMasterData.CANDIENTU.ToString();
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
                                Current = "00000",
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

        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdMerchandiseVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdMerchandise>>();
            var maDonViCha = _service.GetParentUnitCode();
            //var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdMerchandise().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdMerchandise().ICreateDate),
                        Method = OrderMethod.DESC
                    }
                }
            };
            try
            {
                if (filtered.IsAdvance)
                {
                    filtered.AdvanceData.LoadGeneralParam(filtered.Summary);
                }
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdMerchandise>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("GetDetailByCode/{code}")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public async Task<IHttpActionResult> GetDetailByCode(string code)
        {
            MdMerchandiseVm.MasterDto result = new MdMerchandiseVm.MasterDto();
            string _ParentUnitCode = _service.GetParentUnitCode();
            MdMerchandise instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaVatTu == code && x.UnitCode.StartsWith(_ParentUnitCode));
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                IQueryable<MdMerchandisePrice> detail = _service.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu == instance.MaVatTu && x.MaDonVi.StartsWith(_ParentUnitCode));
                result = Mapper.Map<MdMerchandise, MdMerchandiseVm.MasterDto>(instance);
                result.TenVatTu = result.TenHang;
                if (result.ItemCode != null && (result.ItemCode.Equals("0") || result.ItemCode.Equals(""))) result.ItemCode = null;
                result.PathImage = WebConfigurationManager.AppSettings["rootUrl"] + "/" + result.PathImage;
                result.DataDetails = Mapper.Map<List<MdMerchandisePrice>, List<MdMerchandiseVm.DtoDetail>>(detail.ToList());
                if (result.DataDetails.Count() == 1)
                {
                    result.GiaBanBuon = result.DataDetails[0].GiaBanBuon;
                    result.GiaBanBuonVat = result.DataDetails[0].GiaBanBuonVat;
                    result.GiaBanLe = result.DataDetails[0].GiaBanLe;
                    result.GiaBanLeVat = result.DataDetails[0].GiaBanLeVat;
                    result.GiaMua = result.DataDetails[0].GiaMua;
                    result.GiaMuaVat = result.DataDetails[0].GiaMuaVat;
                    result.TyLeLaiLe = result.DataDetails[0].TyLeLaiLe;
                    result.TyLeLaiBuon = result.DataDetails[0].TyLeLaiBuon;
                }
                return Ok(result);
            }
            catch (Exception e)
            {

                return InternalServerError();
            }

        }

        [Route("GetDetailByCodeRoot/{code}")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public async Task<IHttpActionResult> GetDetailByCodeRoot(string code)
        {
            string unitCode = _service.GetCurrentUnitCode();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            TransferObj<MdMerchandiseVm.Dto> result = new TransferObj<MdMerchandiseVm.Dto>();
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
                            cmd.CommandText = "SELECT ID,MAVATTU,TENVATTU,TENVIETTAT,BARCODE,DONVITINH,MABAOBI,MAKEHANG,MAKHACHHANG,MALOAIVATTU,MANHOMVATTU,TRANGTHAI,UNITCODE,MADONVI,GIAMUA,GIAMUAVAT,GIABANLE,GIABANBUON,TYLELAIBUON,TYLELAILE,MAVATRA,MAVATVAO,TYLEVATRA,TYLEVATVAO,GIABANLEVAT,GIABANBUONVAT,ITEMCODE FROM V_VATTU_GIABAN WHERE MAVATTU = '" + code + "' AND MADONVI = '" + rootUnitcode + "' ";
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    int TRANGTHAI = 0;
                                    decimal GIAMUA = 0;
                                    decimal GIAMUAVAT = 0;
                                    decimal GIABANLE = 0;
                                    decimal GIABANBUON = 0;
                                    decimal TYLELAIBUON = 0;
                                    decimal TYLELAILE = 0;
                                    decimal TYLEVATRA = 0;
                                    decimal TYLEVATVAO = 0;
                                    decimal GIABANLEVAT = 0;
                                    decimal GIABANBUONVAT = 0;
                                    MdMerchandiseVm.Dto _DTO = new MdMerchandiseVm.Dto();
                                    _DTO.Id = dataReader["ID"] != null ? dataReader["ID"].ToString() : "";
                                    _DTO.MaVatTu = dataReader["MAVATTU"] != null ? dataReader["MAVATTU"].ToString() : "";
                                    _DTO.TenVatTu = dataReader["TENVATTU"] != null ? dataReader["TENVATTU"].ToString() : "";
                                    _DTO.TenVietTat = dataReader["TENVIETTAT"] != null ? dataReader["TENVIETTAT"].ToString() : "";
                                    _DTO.Barcode = dataReader["BARCODE"] != null ? dataReader["BARCODE"].ToString() : "";
                                    _DTO.DonViTinh = dataReader["DONVITINH"] != null ? dataReader["DONVITINH"].ToString() : "";
                                    _DTO.MaBaoBi = dataReader["MABAOBI"] != null ? dataReader["MABAOBI"].ToString() : "";
                                    _DTO.MaKeHang = dataReader["MAKEHANG"] != null ? dataReader["MAKEHANG"].ToString() : "";
                                    _DTO.MaKhachHang = dataReader["MAKHACHHANG"] != null ? dataReader["MAKHACHHANG"].ToString() : "";
                                    _DTO.MaNCC = dataReader["MAKHACHHANG"] != null ? dataReader["MAKHACHHANG"].ToString() : "";
                                    _DTO.MaLoaiVatTu = dataReader["MALOAIVATTU"] != null ? dataReader["MALOAIVATTU"].ToString() : "";
                                    _DTO.MaNhomVatTu = dataReader["MANHOMVATTU"] != null ? dataReader["MANHOMVATTU"].ToString() : "";
                                    int.TryParse(dataReader["TRANGTHAI"] != null ? dataReader["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                    _DTO.TrangThai = TRANGTHAI;
                                    _DTO.MaDonVi = dataReader["MADONVI"] != null ? dataReader["MADONVI"].ToString() : "";
                                    decimal.TryParse(dataReader["GIAMUA"] != null ? dataReader["GIAMUA"].ToString() : "", out GIAMUA);
                                    _DTO.GiaMua = GIAMUA;
                                    decimal.TryParse(dataReader["GIAMUAVAT"] != null ? dataReader["GIAMUAVAT"].ToString() : "", out GIAMUAVAT);
                                    _DTO.GiaMuaVat = GIAMUAVAT;
                                    decimal.TryParse(dataReader["GIABANLE"] != null ? dataReader["GIABANLE"].ToString() : "", out GIABANLE);
                                    _DTO.GiaBanLe = GIABANLE;
                                    decimal.TryParse(dataReader["GIABANBUON"] != null ? dataReader["GIABANBUON"].ToString() : "", out GIABANBUON);
                                    _DTO.GiaBanBuon = GIABANBUON;
                                    decimal.TryParse(dataReader["TYLELAIBUON"] != null ? dataReader["TYLELAIBUON"].ToString() : "", out TYLELAIBUON);
                                    _DTO.TyLeLaiBuon = TYLELAIBUON;
                                    decimal.TryParse(dataReader["TYLELAILE"] != null ? dataReader["TYLELAILE"].ToString() : "", out TYLELAILE);
                                    _DTO.TyLeLaiLe = TYLELAILE;
                                    _DTO.MaVatRa = dataReader["MAVATRA"] != null ? dataReader["MAVATRA"].ToString() : "";
                                    _DTO.MaVatVao = dataReader["MAVATVAO"] != null ? dataReader["MAVATVAO"].ToString() : "";
                                    decimal.TryParse(dataReader["TYLEVATRA"] != null ? dataReader["TYLEVATRA"].ToString() : "", out TYLEVATRA);
                                    _DTO.TyLeVatRa = TYLEVATRA;
                                    decimal.TryParse(dataReader["TYLEVATVAO"] != null ? dataReader["TYLEVATVAO"].ToString() : "", out TYLEVATVAO);
                                    _DTO.TyLeVatVao = TYLEVATVAO;
                                    decimal.TryParse(dataReader["GIABANLEVAT"] != null ? dataReader["GIABANLEVAT"].ToString() : "", out GIABANLEVAT);
                                    _DTO.GiaBanLeVat = GIABANLEVAT;
                                    decimal.TryParse(dataReader["GIABANBUONVAT"] != null ? dataReader["GIABANBUONVAT"].ToString() : "", out GIABANBUONVAT);
                                    _DTO.GiaBanBuonVat = GIABANBUONVAT;
                                    _DTO.ItemCode = dataReader["ITEMCODE"] != null ? (dataReader["ITEMCODE"].ToString() != "0" ? dataReader["ITEMCODE"].ToString() : null) : null;
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
        /// <summary>
        /// Query entity
        /// POST
        /// </summary>
        /// <param name="jsonData">complex data : jsonData.filtered & jsonData.paged</param>
        /// <returns></returns>
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            PagedObj<MdMerchandiseVm.Dto> paged = new PagedObj<MdMerchandiseVm.Dto>();
            TransferObj<PagedObj<MdMerchandiseVm.Dto>> result = new TransferObj<PagedObj<MdMerchandiseVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<MdMerchandiseVm.SearchProcedure> filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdMerchandiseVm.SearchProcedure>>();
            string unitCode = _service.GetCurrentUnitCode();
            try
            {
                paged = ((JObject)postData.paged).ToObject<PagedObj<MdMerchandiseVm.Dto>>();
            }
            catch (Exception e)
            {
                result.Status = false;
            }
            string maKho = "";
            List<InventoryExpImp> xntItems = null;
            QueryBuilder query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1
            };
            if (filtered.AdvanceData.WithGiaVon) maKho = filtered.AdvanceData.MaKho;
            // Sử dụng khi tính lọc giá vốn theo kho
            ProcedureService<MdMerchandiseVm.Dto> service = new ProcedureService<MdMerchandiseVm.Dto>();
            IQueryFilter filterSQl = service.FilterSQL<MdMerchandiseVm.SearchProcedure>(filtered, query);
            if (string.IsNullOrEmpty(filtered.OrderBy))
            {
                filtered.OrderBy = "MAVATTU";
                filtered.OrderType = "DESC";
            }
            string queryStr = filterSQl != null ? string.Format(" AND {0} ", filterSQl.ToString()) : "";
            queryStr = string.Format(" AND MACHA IS NULL {0} ORDER BY {1} {2}", queryStr, filtered.OrderBy, filtered.OrderType);
            //var orderStr = string.Format(" ORDER BY {0} {1}", filtered.OrderBy, filtered.OrderType);
            PagedObj<MdMerchandiseVm.Dto> temp = ProcedureCollection.QueryPageMerchandise(new ERPContext(), paged, filtered.Summary, queryStr, unitCode);
            //New Version
            //var temp = ProcedureCollection.QueryPageMerchandise(new BTS.API.ENTITY.ERPContext(), paged, filtered.Summary, queryStr, orderStr, unitCode);
            if (filtered.AdvanceData.WithGiaVon)
            {
                List<string> merchandiseCodes = temp.Data.Select(x => x.MaHang).ToList();
                String joinMerchandise = String.Join(",", merchandiseCodes);
                xntItems = ProcedureCollection.GetCostOfGoodsSoldByMerchandises(unitCode, maKho, joinMerchandise);
                if (xntItems.Count == 0)
                {
                    result.Message = "Chưa khởi tạo kỳ kế toán";
                }
            }
            try
            {
                result.Data = temp;
                result.Data.Data.ForEach(x =>
                {
                    decimal soTon = 0;
                    if (filtered.AdvanceData.WithGiaVon)
                    {
                        InventoryExpImp xntItem = xntItems.FirstOrDefault(u => u.Code == x.MaHang);
                        if (xntItem != null)
                        {
                            decimal.TryParse(xntItem.ClosingValue.ToString(), out soTon);
                            x.SoLuongTon = soTon;
                            x.GiaVon = xntItem.CostOfCapital;
                            x.GiaVonVat = x.GiaVon * (1 + x.TyLeVatVao / 100);
                        }
                    }
                    MdPackaging baoBi = _service.UnitOfWork.Repository<MdPackaging>().DbSet.FirstOrDefault(u => u.MaBaoBi == x.MaBaoBi);
                    if (baoBi != null)
                    {
                        x.LuongBao = baoBi.SoLuong;
                    }
                });
                result.Status = true;
                if (result.Data.Data.Count == 0)
                {
                    result.Status = true;
                    result.Message = "Không tìm thấy dữ liệu";
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("PostSelectDataQuery")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public IHttpActionResult PostSelectDataQuery(JObject jsonData)
        {
            PagedObj<MdMerchandiseVm.Dto> paged = new PagedObj<MdMerchandiseVm.Dto>();
            TransferObj<PagedObj<MdMerchandiseVm.Dto>> result = new TransferObj<PagedObj<MdMerchandiseVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<MdMerchandiseVm.SearchProcedure> filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdMerchandiseVm.SearchProcedure>>();
            string unitCode = _service.GetCurrentUnitCode();
            try
            {
                paged = ((JObject)postData.paged).ToObject<PagedObj<MdMerchandiseVm.Dto>>();
            }
            catch (Exception e)
            {
                result.Status = false;
            }
            string _parentUnitCode = _service.GetParentUnitCode();
            string maKho = "";
            List<InventoryExpImp> xntItems = null;
            QueryBuilder query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1
            };
            if (filtered.AdvanceData.WithGiaVon) maKho = filtered.AdvanceData.MaKho;
            // Sử dụng khi tính lọc giá vốn theo kho
            ProcedureService<MdMerchandiseVm.Dto> service = new ProcedureService<MdMerchandiseVm.Dto>();
            IQueryFilter filterSQl = service.FilterSQL<MdMerchandiseVm.SearchProcedure>(filtered, query);
            if (string.IsNullOrEmpty(filtered.OrderBy))
            {
                filtered.OrderBy = "MAVATTU";
                filtered.OrderType = "ASC";
            }
            string queryStr = filterSQl != null ? string.Format(" AND {0} ", filterSQl.ToString()) : "";
            queryStr = string.Format(" AND MACHA IS NULL {0} ORDER BY {1} {2}", queryStr, filtered.OrderBy, filtered.OrderType);
            //var orderStr = string.Format(" ORDER BY {0} {1}", filtered.OrderBy, filtered.OrderType);
            PagedObj<MdMerchandiseVm.Dto> temp = ProcedureCollection.QueryPageMerchandise(new ERPContext(), paged, filtered.Summary, queryStr, unitCode);
            //New Version
            //var temp = ProcedureCollection.QueryPageMerchandise(new BTS.API.ENTITY.ERPContext(), paged, filtered.Summary, queryStr, orderStr, unitCode);
            if (filtered.AdvanceData.WithGiaVon)
            {
                List<string> merchandiseCodes = temp.Data.Select(x => x.MaHang).ToList();
                string joinMerchandise = String.Join(",", merchandiseCodes);
                xntItems = ProcedureCollection.GetCostOfGoodsSoldByMerchandises(unitCode, maKho, joinMerchandise);
            }
            try
            {
                result.Data = temp;
                result.Data.Data.ForEach(x =>
                {
                    decimal soTon = 0;
                    if (filtered.AdvanceData.WithGiaVon)
                    {
                        var xntItem = xntItems.FirstOrDefault(u => u.Code == x.MaHang);
                        if (xntItem != null)
                        {
                            decimal.TryParse(xntItem.ClosingValue.ToString(), out soTon);
                            x.SoLuongTon = soTon;
                            x.GiaVon = xntItem.CostOfCapital;
                            x.GiaVonVat = x.GiaVon * (1 + x.TyLeVatVao / 100);
                        }
                    }
                    MdPackaging baoBi = _service.UnitOfWork.Repository<MdPackaging>().DbSet.FirstOrDefault(u => u.MaBaoBi == x.MaBaoBi);
                    if (baoBi != null)
                    {
                        x.LuongBao = baoBi.SoLuong;
                    }
                });
                result.Status = true;
                if (result.Data.Data.Count == 0)
                {
                    result.Status = true;
                    result.Message = "";
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("PostFilterMerchandise")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public IHttpActionResult PostFilterMerchandise(JObject jsonData)
        {
            var result = new TransferObj<List<NvPhieuDatHangVm.DtoDetail>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdMerchandiseVm.FilterForDatHang>>();
            MdMerchandiseVm.FilterQuantity filterQuantity = null;
            filterQuantity = ((JObject)postData.filterQuantity).ToObject<MdMerchandiseVm.FilterQuantity>();
            var unitCode = _service.GetCurrentUnitCode();
            try
            {
                var xntData = ProcedureCollection.Get_Data_DatHang_NhaCungCap(filtered.AdvanceData.MaNhaCungCap, filtered.AdvanceData.MaLoaiVatTu, filtered.AdvanceData.MaNhomVatTu, filtered.AdvanceData.MaKhoHang, filtered.AdvanceData.TuNgay.Value, filtered.AdvanceData.DenNgay.Value, filtered.AdvanceData.NgayChungTu.Value, unitCode, filterQuantity);
                if (xntData.Count > 0)
                {
                    result.Status = true;
                    result.Data = xntData;
                }
                else
                {
                    result.Status = false;
                    result.Data = null;
                }
                return Ok(result);
            }
            catch
            {
                result.Status = false;
                return InternalServerError();
            }
        }
        /// <summary>
        /// GET
        /// </summary>
        /// <returns></returns>
        [Route("GetSelectData")]
        [AllowAnonymous]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            var unitCode = _service.GetCurrentUnitCode();
            return data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaVatTu, Text = x.TenHang, Id = x.Id }).ToList();
        }

        [Route("PostQueryDetail")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public async Task<IHttpActionResult> PostQueryDetail(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdMerchandisePriceVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdMerchandisePrice>>();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
            };
            try
            {
                var filterResult = _servicePrice.Filter(filtered, query);
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
        //logroot
        [Route("PostAsyncFromOracleRoot")]
        [HttpPost]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public async Task<IHttpActionResult> PostAsyncFromOracleRoot(MdMerchandiseVm.Dto hangHoa)
        {
            var result = new TransferObj<MdMerchandise>();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            string currentUnitCode = _service.GetCurrentUnitCode();
            MdMerchandiseVm.Dto dataDongBo = new MdMerchandiseVm.Dto();
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
                            cmdRoot.CommandText = "SELECT MAVATTU,TENVATTU,TENVIETTAT,BARCODE,DONVITINH,MABAOBI,MAKEHANG,MAKHACHHANG,MALOAIVATTU,MANHOMVATTU,TRANGTHAI,UNITCODE,MADONVI,GIAMUA,GIAMUAVAT,GIABANLE,GIABANBUON,TYLELAIBUON,TYLELAILE,MAVATRA,MAVATVAO,TYLEVATRA,TYLEVATVAO,GIABANLEVAT,GIABANBUONVAT,ITEMCODE FROM V_VATTU_GIABAN WHERE MAVATTU = '" + hangHoa.MaVatTu + "' AND MADONVI = '" + rootUnitcode + "' ";
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
                                    decimal GIAMUA = 0;
                                    decimal GIAMUAVAT = 0;
                                    decimal GIABANLE = 0;
                                    decimal GIABANBUON = 0;
                                    decimal TYLELAIBUON = 0;
                                    decimal TYLELAILE = 0;
                                    decimal TYLEVATRA = 0;
                                    decimal TYLEVATVAO = 0;
                                    decimal GIABANLEVAT = 0;
                                    decimal GIABANBUONVAT = 0;
                                    dataDongBo.MaVatTu = dataReaderRoot["MAVATTU"] != null ? dataReaderRoot["MAVATTU"].ToString() : "";
                                    dataDongBo.TenVatTu = dataReaderRoot["TENVATTU"] != null ? dataReaderRoot["TENVATTU"].ToString() : "";
                                    dataDongBo.TenVietTat = dataReaderRoot["TENVIETTAT"] != null ? dataReaderRoot["TENVIETTAT"].ToString() : "";
                                    dataDongBo.Barcode = dataReaderRoot["BARCODE"] != null ? dataReaderRoot["BARCODE"].ToString() : "";
                                    dataDongBo.DonViTinh = dataReaderRoot["DONVITINH"] != null ? dataReaderRoot["DONVITINH"].ToString() : "";
                                    dataDongBo.MaBaoBi = dataReaderRoot["MABAOBI"] != null ? dataReaderRoot["MABAOBI"].ToString() : "";
                                    dataDongBo.MaKeHang = dataReaderRoot["MAKEHANG"] != null ? dataReaderRoot["MAKEHANG"].ToString() : "";
                                    dataDongBo.MaKhachHang = dataReaderRoot["MAKHACHHANG"] != null ? dataReaderRoot["MAKHACHHANG"].ToString() : "";
                                    dataDongBo.MaNCC = dataReaderRoot["MAKHACHHANG"] != null ? dataReaderRoot["MAKHACHHANG"].ToString() : "";
                                    dataDongBo.MaLoaiVatTu = dataReaderRoot["MALOAIVATTU"] != null ? dataReaderRoot["MALOAIVATTU"].ToString() : "";
                                    dataDongBo.MaNhomVatTu = dataReaderRoot["MANHOMVATTU"] != null ? dataReaderRoot["MANHOMVATTU"].ToString() : "";
                                    int.TryParse(dataReaderRoot["TRANGTHAI"] != null ? dataReaderRoot["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                    dataDongBo.TrangThai = TRANGTHAI;
                                    dataDongBo.MaDonVi = dataReaderRoot["MADONVI"] != null ? dataReaderRoot["MADONVI"].ToString() : "";
                                    decimal.TryParse(dataReaderRoot["GIAMUA"] != null ? dataReaderRoot["GIAMUA"].ToString() : "", out GIAMUA);
                                    dataDongBo.GiaMua = GIAMUA;
                                    decimal.TryParse(dataReaderRoot["GIAMUAVAT"] != null ? dataReaderRoot["GIAMUAVAT"].ToString() : "", out GIAMUAVAT);
                                    dataDongBo.GiaMuaVat = GIAMUAVAT;
                                    decimal.TryParse(dataReaderRoot["GIABANLE"] != null ? dataReaderRoot["GIABANLE"].ToString() : "", out GIABANLE);
                                    dataDongBo.GiaBanLe = GIABANLE;
                                    decimal.TryParse(dataReaderRoot["GIABANBUON"] != null ? dataReaderRoot["GIABANBUON"].ToString() : "", out GIABANBUON);
                                    dataDongBo.GiaBanBuon = GIABANBUON;
                                    decimal.TryParse(dataReaderRoot["TYLELAIBUON"] != null ? dataReaderRoot["TYLELAIBUON"].ToString() : "", out TYLELAIBUON);
                                    dataDongBo.TyLeLaiBuon = TYLELAIBUON;
                                    decimal.TryParse(dataReaderRoot["TYLELAILE"] != null ? dataReaderRoot["TYLELAILE"].ToString() : "", out TYLELAILE);
                                    dataDongBo.TyLeLaiLe = TYLELAILE;
                                    dataDongBo.MaVatRa = dataReaderRoot["MAVATRA"] != null ? dataReaderRoot["MAVATRA"].ToString() : "";
                                    dataDongBo.MaVatVao = dataReaderRoot["MAVATVAO"] != null ? dataReaderRoot["MAVATVAO"].ToString() : "";
                                    decimal.TryParse(dataReaderRoot["TYLEVATRA"] != null ? dataReaderRoot["TYLEVATRA"].ToString() : "", out TYLEVATRA);
                                    dataDongBo.TyLeVatRa = TYLEVATRA;
                                    decimal.TryParse(dataReaderRoot["TYLEVATVAO"] != null ? dataReaderRoot["TYLEVATVAO"].ToString() : "", out TYLEVATVAO);
                                    dataDongBo.TyLeVatVao = TYLEVATVAO;
                                    decimal.TryParse(dataReaderRoot["GIABANLEVAT"] != null ? dataReaderRoot["GIABANLEVAT"].ToString() : "", out GIABANLEVAT);
                                    dataDongBo.GiaBanLeVat = GIABANLEVAT;
                                    decimal.TryParse(dataReaderRoot["GIABANBUONVAT"] != null ? dataReaderRoot["GIABANBUONVAT"].ToString() : "", out GIABANBUONVAT);
                                    dataDongBo.GiaBanBuonVat = GIABANBUONVAT;
                                    dataDongBo.ItemCode = dataReaderRoot["ITEMCODE"] != null ? dataReaderRoot["ITEMCODE"].ToString() : "";
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
                            cmd.CommandText = "SELECT MAVATTU FROM V_VATTU_GIABAN WHERE MAVATTU = '" + hangHoa.MaVatTu + "' AND MADONVI = '" + currentUnitCode + "' ";
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                result.Status = false;
                                result.Message = "Đã tồn tại mặt hàng này tại hệ thống";
                                return Ok(result);
                            }
                            else
                            {
                                if (dataDongBo != null)
                                {
                                    // đồng bộ bảng DM_VATTU
                                    cmd.CommandText = "INSERT INTO DM_VATTU(ID,MAVATTU,TENVATTU,TENVIETTAT,DONVITINH,MAKEHANG,MALOAIVATTU,MANHOMVATTU,MABAOBI,MAKHACHHANG,MANCC,GIAMUA,GIABANLE,GIABANBUON,TYLELAIBUON,TYLELAILE,BARCODE,MAKHAC,MAVATVAO,TYLE_VAT_VAO,MAVATRA,TYLE_VAT_RA,GIABANVAT,TRANGTHAI,ITEMCODE,MASIZE,MACOLOR,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE,UNITCODE,CHIETKHAUNCC,SOTONMAX,SOTONMIN) VALUES (:ID,:MAVATTU,:TENVATTU,:TENVIETTAT,:DONVITINH,:MAKEHANG,:MALOAIVATTU,:MANHOMVATTU,:MABAOBI,:MAKHACHHANG,:MANCC,:GIAMUA,:GIABANLE,:GIABANBUON,:TYLELAIBUON,:TYLELAILE,:BARCODE,:MAKHAC,:MAVATVAO,:TYLE_VAT_VAO,:MAVATRA,:TYLE_VAT_RA,:GIABANVAT,:TRANGTHAI,:ITEMCODE,:MASIZE,:MACOLOR,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE,:UNITCODE,:CHIETKHAUNCC,:SOTONMAX,:SOTONMIN)";
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("MAVATTU", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaVatTu;
                                    cmd.Parameters.Add("TENVATTU", OracleDbType.NVarchar2, 200).Value = dataDongBo.TenVatTu;
                                    cmd.Parameters.Add("TENVIETTAT", OracleDbType.NVarchar2, 200).Value = dataDongBo.TenVatTu;
                                    cmd.Parameters.Add("DONVITINH", OracleDbType.NVarchar2, 50).Value = dataDongBo.DonViTinh;
                                    cmd.Parameters.Add("MAKEHANG", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaKeHang;
                                    cmd.Parameters.Add("MALOAIVATTU", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaLoaiVatTu;
                                    cmd.Parameters.Add("MANHOMVATTU", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaNhomVatTu;
                                    cmd.Parameters.Add("MABAOBI", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaBaoBi;
                                    cmd.Parameters.Add("MAKHACHHANG", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaKhachHang;
                                    cmd.Parameters.Add("MANCC", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaNCC;
                                    cmd.Parameters.Add("GIAMUA", OracleDbType.Decimal).Value = dataDongBo.GiaMua;
                                    cmd.Parameters.Add("GIABANLE", OracleDbType.Decimal).Value = dataDongBo.GiaBanLe;
                                    cmd.Parameters.Add("GIABANBUON", OracleDbType.Decimal).Value = dataDongBo.GiaBanBuon;
                                    cmd.Parameters.Add("TYLELAIBUON", OracleDbType.Decimal).Value = dataDongBo.TyLeLaiBuon;
                                    cmd.Parameters.Add("TYLELAILE", OracleDbType.Decimal).Value = dataDongBo.TyLeLaiLe;
                                    cmd.Parameters.Add("BARCODE", OracleDbType.NVarchar2, 2000).Value = dataDongBo.Barcode;
                                    cmd.Parameters.Add("MAKHAC", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaKhac;
                                    cmd.Parameters.Add("MAVATVAO", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaVatVao;
                                    cmd.Parameters.Add("TYLE_VAT_VAO", OracleDbType.Decimal).Value = dataDongBo.TyLeVatVao;
                                    cmd.Parameters.Add("MAVATRA", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaVatRa;
                                    cmd.Parameters.Add("TYLE_VAT_RA", OracleDbType.Decimal).Value = dataDongBo.TyLeVatRa;
                                    cmd.Parameters.Add("GIABANVAT", OracleDbType.Decimal).Value = dataDongBo.GiaBanLeVat;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = dataDongBo.TrangThai;
                                    cmd.Parameters.Add("ITEMCODE", OracleDbType.NVarchar2, 50).Value = dataDongBo.ItemCode;
                                    cmd.Parameters.Add("MASIZE", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaSize;
                                    cmd.Parameters.Add("MACOLOR", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaColor;
                                    cmd.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = _service.GetCurrentUnitCode();
                                    cmd.Parameters.Add("CHIETKHAUNCC", OracleDbType.Decimal).Value = 0;
                                    cmd.Parameters.Add("SOTONMAX", OracleDbType.Decimal).Value = 0;
                                    cmd.Parameters.Add("SOTONMIN", OracleDbType.Decimal).Value = 0;
                                    // đồng bộ bảng DM_VATTU_GIACA
                                    OracleCommand cmdGiaBan = new OracleCommand();
                                    cmdGiaBan.Connection = connection;
                                    cmdGiaBan.InitialLONGFetchSize = 1000;
                                    cmdGiaBan.CommandText = "INSERT INTO DM_VATTU_GIACA(ID,MAVATTU,MADONVI,GIAVON,GIAMUAVAT,GIAMUA,GIABANLE,GIABANBUON,TY_LELAI_BUON,TY_LELAI_LE,MAVATVAO,TYLE_VAT_VAO,MAVATRA,TYLE_VAT_RA,GIA_BANLE_VAT,GIA_BANBUON_VAT,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE,UNITCODE,SOTONMAX,SOTONMIN) VALUES (:ID,:MAVATTU,:MADONVI,:GIAVON,:GIAMUAVAT,:GIAMUA,:GIABANLE,:GIABANBUON,:TY_LELAI_BUON,:TY_LELAI_LE,:MAVATVAO,:TYLE_VAT_VAO,:MAVATRA,:TYLE_VAT_RA,:GIA_BANLE_VAT,:GIA_BANBUON_VAT,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE,:UNITCODE,:SOTONMAX,:SOTONMIN)";
                                    cmdGiaBan.CommandType = CommandType.Text;
                                    cmdGiaBan.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                    cmdGiaBan.Parameters.Add("MAVATTU", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaVatTu;
                                    cmdGiaBan.Parameters.Add("MADONVI", OracleDbType.NVarchar2, 200).Value = _service.GetCurrentUnitCode();
                                    cmdGiaBan.Parameters.Add("GIAVON", OracleDbType.Decimal).Value = dataDongBo.GiaVon;
                                    cmdGiaBan.Parameters.Add("GIAMUAVAT", OracleDbType.Decimal).Value = dataDongBo.GiaMuaVat;
                                    cmdGiaBan.Parameters.Add("GIAMUA", OracleDbType.Decimal).Value = dataDongBo.GiaMua;
                                    cmdGiaBan.Parameters.Add("GIABANLE", OracleDbType.Decimal).Value = dataDongBo.GiaBanLe;
                                    cmdGiaBan.Parameters.Add("GIABANBUON", OracleDbType.Decimal).Value = dataDongBo.GiaBanBuon;
                                    cmdGiaBan.Parameters.Add("TY_LELAI_BUON", OracleDbType.Decimal).Value = dataDongBo.TyLeLaiBuon;
                                    cmdGiaBan.Parameters.Add("TY_LELAI_LE", OracleDbType.Decimal).Value = dataDongBo.TyLeLaiLe;
                                    cmdGiaBan.Parameters.Add("MAVATVAO", OracleDbType.NVarchar2, 200).Value = dataDongBo.MaVatVao;
                                    cmdGiaBan.Parameters.Add("TYLE_VAT_VAO", OracleDbType.Decimal).Value = dataDongBo.TyLeVatVao;
                                    cmdGiaBan.Parameters.Add("MAVATRA", OracleDbType.NVarchar2, 200).Value = dataDongBo.MaVatRa;
                                    cmdGiaBan.Parameters.Add("TYLE_VAT_RA", OracleDbType.Decimal).Value = dataDongBo.TyLeVatRa;
                                    cmdGiaBan.Parameters.Add("GIA_BANLE_VAT", OracleDbType.Decimal).Value = dataDongBo.GiaBanLeVat;
                                    cmdGiaBan.Parameters.Add("GIA_BANBUON_VAT", OracleDbType.Decimal).Value = dataDongBo.GiaBanBuonVat;
                                    cmdGiaBan.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmdGiaBan.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmdGiaBan.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmdGiaBan.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmdGiaBan.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    cmdGiaBan.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = _service.GetCurrentUnitCode();
                                    cmdGiaBan.Parameters.Add("SOTONMAX", OracleDbType.Decimal).Value = 0;
                                    cmdGiaBan.Parameters.Add("SOTONMIN", OracleDbType.Decimal).Value = 0;
                                    OracleTransaction transactionVatTu;
                                    OracleTransaction transactionGiaBan;
                                    try
                                    {

                                        transactionVatTu = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionVatTu;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionVatTu.Commit();
                                        transactionGiaBan = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmdGiaBan.Transaction = transactionGiaBan;
                                        int countGiaBan = cmdGiaBan.ExecuteNonQuery();
                                        transactionGiaBan.Commit();
                                        if (count > 0 && countGiaBan > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Đồng bộ thành công";
                                        }
                                        else
                                        {
                                            transactionVatTu.Rollback();
                                            transactionGiaBan.Rollback();
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
        ////Phạm Tuấn Anh PostAsyncCompareUpdate -- Update lại mới mã đã đồng bộ rồi
        [Route("PostAsyncCompareUpdate")]
        [HttpPost]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public async Task<IHttpActionResult> PostAsyncCompareUpdate(MdMerchandiseVm.Dto hangHoa)
        {
            var result = new TransferObj<MdMerchandise>();
            string parentUnitCode = _service.GetParentUnitCode();
            string unitCode = _service.GetCurrentUnitCode();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            MdMerchandiseVm.Dto dataDongBo = new MdMerchandiseVm.Dto();
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
                            cmdRoot.CommandText = "SELECT MAVATTU,TENVATTU,TENVIETTAT,BARCODE,DONVITINH,MABAOBI,MAKEHANG,MAKHACHHANG,MALOAIVATTU,MANHOMVATTU,TRANGTHAI,UNITCODE,MADONVI,GIAMUA,GIAMUAVAT,GIABANLE,GIABANBUON,TYLELAIBUON,TYLELAILE,MAVATRA,MAVATVAO,TYLEVATRA,TYLEVATVAO,GIABANLEVAT,GIABANBUONVAT,ITEMCODE FROM V_VATTU_GIABAN WHERE MAVATTU = '" + hangHoa.MaVatTu + "' AND MADONVI = '" + rootUnitcode + "' ";
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
                                    decimal GIAMUA = 0;
                                    decimal GIAMUAVAT = 0;
                                    decimal GIABANLE = 0;
                                    decimal GIABANBUON = 0;
                                    decimal TYLELAIBUON = 0;
                                    decimal TYLELAILE = 0;
                                    decimal TYLEVATRA = 0;
                                    decimal TYLEVATVAO = 0;
                                    decimal GIABANLEVAT = 0;
                                    decimal GIABANBUONVAT = 0;
                                    dataDongBo.MaVatTu = dataReaderRoot["MAVATTU"] != null ? dataReaderRoot["MAVATTU"].ToString() : "";
                                    dataDongBo.TenVatTu = dataReaderRoot["TENVATTU"] != null ? dataReaderRoot["TENVATTU"].ToString() : "";
                                    dataDongBo.TenVietTat = dataReaderRoot["TENVIETTAT"] != null ? dataReaderRoot["TENVIETTAT"].ToString() : "";
                                    dataDongBo.Barcode = dataReaderRoot["BARCODE"] != null ? dataReaderRoot["BARCODE"].ToString() : "";
                                    dataDongBo.DonViTinh = dataReaderRoot["DONVITINH"] != null ? dataReaderRoot["DONVITINH"].ToString() : "";
                                    dataDongBo.MaBaoBi = dataReaderRoot["MABAOBI"] != null ? dataReaderRoot["MABAOBI"].ToString() : "";
                                    dataDongBo.MaKeHang = dataReaderRoot["MAKEHANG"] != null ? dataReaderRoot["MAKEHANG"].ToString() : "";
                                    dataDongBo.MaKhachHang = dataReaderRoot["MAKHACHHANG"] != null ? dataReaderRoot["MAKHACHHANG"].ToString() : "";
                                    dataDongBo.MaNCC = dataReaderRoot["MAKHACHHANG"] != null ? dataReaderRoot["MAKHACHHANG"].ToString() : "";
                                    dataDongBo.MaLoaiVatTu = dataReaderRoot["MALOAIVATTU"] != null ? dataReaderRoot["MALOAIVATTU"].ToString() : "";
                                    dataDongBo.MaNhomVatTu = dataReaderRoot["MANHOMVATTU"] != null ? dataReaderRoot["MANHOMVATTU"].ToString() : "";
                                    int.TryParse(dataReaderRoot["TRANGTHAI"] != null ? dataReaderRoot["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                    dataDongBo.TrangThai = TRANGTHAI;
                                    dataDongBo.MaDonVi = dataReaderRoot["MADONVI"] != null ? dataReaderRoot["MADONVI"].ToString() : "";
                                    decimal.TryParse(dataReaderRoot["GIAMUA"] != null ? dataReaderRoot["GIAMUA"].ToString() : "", out GIAMUA);
                                    dataDongBo.GiaMua = GIAMUA;
                                    decimal.TryParse(dataReaderRoot["GIAMUAVAT"] != null ? dataReaderRoot["GIAMUAVAT"].ToString() : "", out GIAMUAVAT);
                                    dataDongBo.GiaMuaVat = GIAMUAVAT;
                                    decimal.TryParse(dataReaderRoot["GIABANLE"] != null ? dataReaderRoot["GIABANLE"].ToString() : "", out GIABANLE);
                                    dataDongBo.GiaBanLe = GIABANLE;
                                    decimal.TryParse(dataReaderRoot["GIABANBUON"] != null ? dataReaderRoot["GIABANBUON"].ToString() : "", out GIABANBUON);
                                    dataDongBo.GiaBanBuon = GIABANBUON;
                                    decimal.TryParse(dataReaderRoot["TYLELAIBUON"] != null ? dataReaderRoot["TYLELAIBUON"].ToString() : "", out TYLELAIBUON);
                                    dataDongBo.TyLeLaiBuon = TYLELAIBUON;
                                    decimal.TryParse(dataReaderRoot["TYLELAILE"] != null ? dataReaderRoot["TYLELAILE"].ToString() : "", out TYLELAILE);
                                    dataDongBo.TyLeLaiLe = TYLELAILE;
                                    dataDongBo.MaVatRa = dataReaderRoot["MAVATRA"] != null ? dataReaderRoot["MAVATRA"].ToString() : "";
                                    dataDongBo.MaVatVao = dataReaderRoot["MAVATVAO"] != null ? dataReaderRoot["MAVATVAO"].ToString() : "";
                                    decimal.TryParse(dataReaderRoot["TYLEVATRA"] != null ? dataReaderRoot["TYLEVATRA"].ToString() : "", out TYLEVATRA);
                                    dataDongBo.TyLeVatRa = TYLEVATRA;
                                    decimal.TryParse(dataReaderRoot["TYLEVATVAO"] != null ? dataReaderRoot["TYLEVATVAO"].ToString() : "", out TYLEVATVAO);
                                    dataDongBo.TyLeVatVao = TYLEVATVAO;
                                    decimal.TryParse(dataReaderRoot["GIABANLEVAT"] != null ? dataReaderRoot["GIABANLEVAT"].ToString() : "", out GIABANLEVAT);
                                    dataDongBo.GiaBanLeVat = GIABANLEVAT;
                                    decimal.TryParse(dataReaderRoot["GIABANBUONVAT"] != null ? dataReaderRoot["GIABANBUONVAT"].ToString() : "", out GIABANBUONVAT);
                                    dataDongBo.GiaBanBuonVat = GIABANBUONVAT;
                                    dataDongBo.ItemCode = dataReaderRoot["ITEMCODE"] != null ? dataReaderRoot["ITEMCODE"].ToString() : "";
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
                var exists = _service.Repository.DbSet.FirstOrDefault(x => x.MaVatTu == hangHoa.MaVatTu && x.UnitCode.Equals(unitCode));
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
                                command.CommandText = string.Format(@"DELETE FROM DM_VATTU WHERE MAVATTU = '{0}'", exists.MaVatTu);
                                command.ExecuteNonQuery();
                                command.CommandText = string.Format(@"DELETE FROM DM_VATTU_GIACA WHERE MAVATTU = '{0}'", exists.MaVatTu);
                                command.ExecuteNonQuery();
                                transaction.Commit();

                                if (dataDongBo != null)
                                {
                                    OracleCommand cmd = new OracleCommand();
                                    cmd.Connection = connection;
                                    // đồng bộ bảng DM_VATTU
                                    cmd.CommandText = "INSERT INTO DM_VATTU(ID,MAVATTU,TENVATTU,TENVIETTAT,DONVITINH,MAKEHANG,MALOAIVATTU,MANHOMVATTU,MABAOBI,MAKHACHHANG,MANCC,GIAMUA,GIABANLE,GIABANBUON,TYLELAIBUON,TYLELAILE,BARCODE,MAKHAC,MAVATVAO,TYLE_VAT_VAO,MAVATRA,TYLE_VAT_RA,GIABANVAT,TRANGTHAI,ITEMCODE,MASIZE,MACOLOR,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE,UNITCODE,CHIETKHAUNCC,SOTONMAX,SOTONMIN) VALUES (:ID,:MAVATTU,:TENVATTU,:TENVIETTAT,:DONVITINH,:MAKEHANG,:MALOAIVATTU,:MANHOMVATTU,:MABAOBI,:MAKHACHHANG,:MANCC,:GIAMUA,:GIABANLE,:GIABANBUON,:TYLELAIBUON,:TYLELAILE,:BARCODE,:MAKHAC,:MAVATVAO,:TYLE_VAT_VAO,:MAVATRA,:TYLE_VAT_RA,:GIABANVAT,:TRANGTHAI,:ITEMCODE,:MASIZE,:MACOLOR,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE,:UNITCODE,:CHIETKHAUNCC,:SOTONMAX,:SOTONMIN)";
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                    cmd.Parameters.Add("MAVATTU", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaVatTu;
                                    cmd.Parameters.Add("TENVATTU", OracleDbType.NVarchar2, 200).Value = dataDongBo.TenVatTu;
                                    cmd.Parameters.Add("TENVIETTAT", OracleDbType.NVarchar2, 200).Value = dataDongBo.TenVatTu;
                                    cmd.Parameters.Add("DONVITINH", OracleDbType.NVarchar2, 50).Value = dataDongBo.DonViTinh;
                                    cmd.Parameters.Add("MAKEHANG", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaKeHang;
                                    cmd.Parameters.Add("MALOAIVATTU", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaLoaiVatTu;
                                    cmd.Parameters.Add("MANHOMVATTU", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaNhomVatTu;
                                    cmd.Parameters.Add("MABAOBI", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaBaoBi;
                                    cmd.Parameters.Add("MAKHACHHANG", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaKhachHang;
                                    cmd.Parameters.Add("MANCC", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaNCC;
                                    cmd.Parameters.Add("GIAMUA", OracleDbType.Decimal).Value = dataDongBo.GiaMua;
                                    cmd.Parameters.Add("GIABANLE", OracleDbType.Decimal).Value = dataDongBo.GiaBanLe;
                                    cmd.Parameters.Add("GIABANBUON", OracleDbType.Decimal).Value = dataDongBo.GiaBanBuon;
                                    cmd.Parameters.Add("TYLELAIBUON", OracleDbType.Decimal).Value = dataDongBo.TyLeLaiBuon;
                                    cmd.Parameters.Add("TYLELAILE", OracleDbType.Decimal).Value = dataDongBo.TyLeLaiLe;
                                    cmd.Parameters.Add("BARCODE", OracleDbType.NVarchar2, 2000).Value = dataDongBo.Barcode;
                                    cmd.Parameters.Add("MAKHAC", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaKhac;
                                    cmd.Parameters.Add("MAVATVAO", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaVatVao;
                                    cmd.Parameters.Add("TYLE_VAT_VAO", OracleDbType.Decimal).Value = dataDongBo.TyLeVatVao;
                                    cmd.Parameters.Add("MAVATRA", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaVatRa;
                                    cmd.Parameters.Add("TYLE_VAT_RA", OracleDbType.Decimal).Value = dataDongBo.TyLeVatRa;
                                    cmd.Parameters.Add("GIABANVAT", OracleDbType.Decimal).Value = dataDongBo.GiaBanLeVat;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = dataDongBo.TrangThai;
                                    cmd.Parameters.Add("ITEMCODE", OracleDbType.NVarchar2, 50).Value = dataDongBo.ItemCode;
                                    cmd.Parameters.Add("MASIZE", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaSize;
                                    cmd.Parameters.Add("MACOLOR", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaColor;
                                    cmd.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = _service.GetCurrentUnitCode();
                                    cmd.Parameters.Add("CHIETKHAUNCC", OracleDbType.Decimal).Value = 0;
                                    cmd.Parameters.Add("SOTONMAX", OracleDbType.Decimal).Value = 0;
                                    cmd.Parameters.Add("SOTONMIN", OracleDbType.Decimal).Value = 0;
                                    // đồng bộ bảng DM_VATTU_GIACA
                                    OracleCommand cmdGiaBan = new OracleCommand();
                                    cmdGiaBan.Connection = connection;
                                    cmdGiaBan.InitialLONGFetchSize = 1000;
                                    cmdGiaBan.CommandText = "INSERT INTO DM_VATTU_GIACA(ID,MAVATTU,MADONVI,GIAVON,GIAMUAVAT,GIAMUA,GIABANLE,GIABANBUON,TY_LELAI_BUON,TY_LELAI_LE,MAVATVAO,TYLE_VAT_VAO,MAVATRA,TYLE_VAT_RA,GIA_BANLE_VAT,GIA_BANBUON_VAT,I_CREATE_DATE,I_CREATE_BY,I_UPDATE_DATE,I_UPDATE_BY,I_STATE,UNITCODE,SOTONMAX,SOTONMIN) VALUES (:ID,:MAVATTU,:MADONVI,:GIAVON,:GIAMUAVAT,:GIAMUA,:GIABANLE,:GIABANBUON,:TY_LELAI_BUON,:TY_LELAI_LE,:MAVATVAO,:TYLE_VAT_VAO,:MAVATRA,:TYLE_VAT_RA,:GIA_BANLE_VAT,:GIA_BANBUON_VAT,:I_CREATE_DATE,:I_CREATE_BY,:I_UPDATE_DATE,:I_UPDATE_BY,:I_STATE,:UNITCODE,:SOTONMAX,:SOTONMIN)";
                                    cmdGiaBan.CommandType = CommandType.Text;
                                    cmdGiaBan.Parameters.Add("ID", OracleDbType.Varchar2, 50).Value = Guid.NewGuid();
                                    cmdGiaBan.Parameters.Add("MAVATTU", OracleDbType.NVarchar2, 50).Value = dataDongBo.MaVatTu;
                                    cmdGiaBan.Parameters.Add("MADONVI", OracleDbType.NVarchar2, 200).Value = _service.GetCurrentUnitCode();
                                    cmdGiaBan.Parameters.Add("GIAVON", OracleDbType.Decimal).Value = dataDongBo.GiaVon;
                                    cmdGiaBan.Parameters.Add("GIAMUAVAT", OracleDbType.Decimal).Value = dataDongBo.GiaMuaVat;
                                    cmdGiaBan.Parameters.Add("GIAMUA", OracleDbType.Decimal).Value = dataDongBo.GiaMua;
                                    cmdGiaBan.Parameters.Add("GIABANLE", OracleDbType.Decimal).Value = dataDongBo.GiaBanLe;
                                    cmdGiaBan.Parameters.Add("GIABANBUON", OracleDbType.Decimal).Value = dataDongBo.GiaBanBuon;
                                    cmdGiaBan.Parameters.Add("TY_LELAI_BUON", OracleDbType.Decimal).Value = dataDongBo.TyLeLaiBuon;
                                    cmdGiaBan.Parameters.Add("TY_LELAI_LE", OracleDbType.Decimal).Value = dataDongBo.TyLeLaiLe;
                                    cmdGiaBan.Parameters.Add("MAVATVAO", OracleDbType.NVarchar2, 200).Value = dataDongBo.MaVatVao;
                                    cmdGiaBan.Parameters.Add("TYLE_VAT_VAO", OracleDbType.Decimal).Value = dataDongBo.TyLeVatVao;
                                    cmdGiaBan.Parameters.Add("MAVATRA", OracleDbType.NVarchar2, 200).Value = dataDongBo.MaVatRa;
                                    cmdGiaBan.Parameters.Add("TYLE_VAT_RA", OracleDbType.Decimal).Value = dataDongBo.TyLeVatRa;
                                    cmdGiaBan.Parameters.Add("GIA_BANLE_VAT", OracleDbType.Decimal).Value = dataDongBo.GiaBanLeVat;
                                    cmdGiaBan.Parameters.Add("GIA_BANBUON_VAT", OracleDbType.Decimal).Value = dataDongBo.GiaBanBuonVat;
                                    cmdGiaBan.Parameters.Add("I_CREATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmdGiaBan.Parameters.Add("I_CREATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmdGiaBan.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmdGiaBan.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name;
                                    cmdGiaBan.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    cmdGiaBan.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = _service.GetCurrentUnitCode();
                                    cmdGiaBan.Parameters.Add("SOTONMAX", OracleDbType.Decimal).Value = 0;
                                    cmdGiaBan.Parameters.Add("SOTONMIN", OracleDbType.Decimal).Value = 0;
                                    OracleTransaction transactionVatTu;
                                    OracleTransaction transactionGiaBan;
                                    try
                                    {

                                        transactionVatTu = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionVatTu;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionVatTu.Commit();
                                        transactionGiaBan = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmdGiaBan.Transaction = transactionGiaBan;
                                        int countGiaBan = cmdGiaBan.ExecuteNonQuery();
                                        transactionGiaBan.Commit();
                                        if (count > 0 && countGiaBan > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Đồng bộ thành công";
                                        }
                                        else
                                        {
                                            transactionVatTu.Rollback();
                                            transactionGiaBan.Rollback();
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
                    result.Message = "Không tìm thấy dữ liệu mã hàng";
                    result.Status = false;
                    //return BadRequest(result.Message);
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
        [Route("GetPriceFromSQL/{maVatTu}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPriceFromSQL(string maVatTu)
        {
            var unitCodeLyThaiTo = "0001";
            using (var ctx = new DBCSQL())
            {
                if (!ctx.Database.Exists())
                {
                    return BadRequest("Không có kết nối tới database SQL");
                }
                var tempResult = ctx.TDS_Dmgiaban.Where(x => x.Madonvi == unitCodeLyThaiTo && x.Masieuthi == maVatTu).ToList();
                var result = Mapper.Map<List<TDS_Dmgiaban>, List<TDS_MatHangVm.Details>>(tempResult);
                if (result == null)
                {
                    return NotFound();
                }
                try
                {
                    return Ok(result);
                }
                catch (Exception e)
                {
                    return InternalServerError();
                }
            }
        }
        [Route("PostSelectDataSQLQuery")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public async Task<IHttpActionResult> PostSelectDataSQLQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<TDS_MatHangVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<TDS_MatHangVm.SearchMatHang>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<TDS_Dmmathang>>();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
            };
            var service = new ProcedureService<TDS_Dmmathang>();
            using (var otherContext = new DBCSQL())
            {
                if (!otherContext.Database.Exists())
                {
                    return BadRequest("Không có kết nối tới database SQL");
                }
                var data = otherContext.TDS_Dmmathang.AsNoTracking().AsQueryable();//.Where(x => x.StateSync != 10); // chưa được đồng bộ
                try
                {
                    var filterResult = service.Filter(filtered, data, query);
                    if (!filterResult.WasSuccessful)
                    {
                        return NotFound();
                    }
                    var tempData = filterResult.Value;
                    result.Data = Mapper.Map<PagedObj<TDS_Dmmathang>, PagedObj<TDS_MatHangVm.Dto>>(tempData);
                    result.Status = true;
                    return Ok(result);
                }
                catch (Exception)
                {
                    return InternalServerError();
                }
            }
        }

        [Route("PostSelectDataServerRoot")]
        [CustomAuthorize(Method = "XEM", State = "merchandise")]
        public IHttpActionResult PostSelectDataServerRoot(JObject jsonData)
        {
            string unitCode = _service.GetCurrentUnitCode();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            TransferObj<List<MdMerchandiseVm.Dto>> result = new TransferObj<List<MdMerchandiseVm.Dto>>();
            List<MdMerchandiseVm.Dto> listResult = new List<MdMerchandiseVm.Dto>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<MdMerchandiseVm.SearchProcedure> filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdMerchandiseVm.SearchProcedure>>();
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
                            cmd.CommandText = "DONGBO_TIMKIEM_HANGHOA";
                            cmd.CommandType = CommandType.StoredProcedure;
                            string TIEUCHI = string.Empty;
                            if (filtered.AdvanceData.TieuChiTimKiem.Equals("maVatTu")) TIEUCHI = "MAVATTU";
                            else if (filtered.AdvanceData.TieuChiTimKiem.Equals("barcode")) TIEUCHI = "BARCODE";
                            else if (filtered.AdvanceData.TieuChiTimKiem.Equals("tenHang")) TIEUCHI = "TENVATTU";
                            else if (filtered.AdvanceData.TieuChiTimKiem.Equals("maKhachHang")) TIEUCHI = "MANHACUNGCAP";
                            else if (filtered.AdvanceData.TieuChiTimKiem.Equals("giaBanLeVat")) TIEUCHI = "GIABANLEVAT";
                            else if (filtered.AdvanceData.TieuChiTimKiem.Equals("giaMuaVat")) TIEUCHI = "GIAMUAVAT";
                            else if (filtered.AdvanceData.TieuChiTimKiem.Equals("tyLeLaiLe")) TIEUCHI = "TYLELAILE";
                            else TIEUCHI = "MAVATTU";
                            cmd.Parameters.Add("P_TIEUCHITIMKIEM", OracleDbType.NVarchar2, 50).Value = TIEUCHI;
                            cmd.Parameters.Add("P_MAVATTU", OracleDbType.NVarchar2, 50).Value = filtered.AdvanceData.MaVatTu;
                            cmd.Parameters.Add("P_BARCODE", OracleDbType.NVarchar2, 2000).Value = filtered.AdvanceData.Barcode;
                            cmd.Parameters.Add("P_TENVATTU", OracleDbType.NVarchar2, 500).Value = filtered.AdvanceData.TenHang;
                            cmd.Parameters.Add("P_MANHACUNGCAP", OracleDbType.NVarchar2, 50).Value = filtered.AdvanceData.MaKhachHang;
                            cmd.Parameters.Add("P_GIABANLEVAT", OracleDbType.NVarchar2).Value = filtered.AdvanceData.GiaBanLeVat;
                            cmd.Parameters.Add("P_GIAMUAVAT", OracleDbType.Decimal).Value = filtered.AdvanceData.GiaMuaVat;
                            cmd.Parameters.Add("P_TYLELAILE", OracleDbType.Decimal).Value = filtered.AdvanceData.TyLeLaiLe;
                            cmd.Parameters.Add("P_UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                            cmd.Parameters.Add("CURSOR_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                            OracleDataReader dataReader = cmd.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    int TRANGTHAI = 0;
                                    decimal GIAMUA = 0;
                                    decimal GIAMUAVAT = 0;
                                    decimal GIABANLE = 0;
                                    decimal GIABANBUON = 0;
                                    decimal TYLELAIBUON = 0;
                                    decimal TYLELAILE = 0;
                                    decimal TYLEVATRA = 0;
                                    decimal TYLEVATVAO = 0;
                                    decimal GIABANLEVAT = 0;
                                    decimal GIABANBUONVAT = 0;
                                    MdMerchandiseVm.Dto _DTO = new MdMerchandiseVm.Dto();
                                    _DTO.Id = dataReader["ID"] != null ? dataReader["ID"].ToString() : "";
                                    _DTO.MaVatTu = dataReader["MAVATTU"] != null ? dataReader["MAVATTU"].ToString() : "";
                                    _DTO.TenVatTu = dataReader["TENVATTU"] != null ? dataReader["TENVATTU"].ToString() : "";
                                    _DTO.TenVietTat = dataReader["TENVIETTAT"] != null ? dataReader["TENVIETTAT"].ToString() : "";
                                    _DTO.Barcode = dataReader["BARCODE"] != null ? dataReader["BARCODE"].ToString() : "";
                                    _DTO.DonViTinh = dataReader["DONVITINH"] != null ? dataReader["DONVITINH"].ToString() : "";
                                    _DTO.MaBaoBi = dataReader["MABAOBI"] != null ? dataReader["MABAOBI"].ToString() : "";
                                    _DTO.MaKeHang = dataReader["MAKEHANG"] != null ? dataReader["MAKEHANG"].ToString() : "";
                                    _DTO.MaKhachHang = dataReader["MAKHACHHANG"] != null ? dataReader["MAKHACHHANG"].ToString() : "";
                                    _DTO.MaNCC = dataReader["MAKHACHHANG"] != null ? dataReader["MAKHACHHANG"].ToString() : "";
                                    _DTO.MaLoaiVatTu = dataReader["MALOAIVATTU"] != null ? dataReader["MALOAIVATTU"].ToString() : "";
                                    _DTO.MaNhomVatTu = dataReader["MANHOMVATTU"] != null ? dataReader["MANHOMVATTU"].ToString() : "";
                                    int.TryParse(dataReader["TRANGTHAI"] != null ? dataReader["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                    _DTO.TrangThai = TRANGTHAI;
                                    _DTO.MaDonVi = dataReader["MADONVI"] != null ? dataReader["MADONVI"].ToString() : "";
                                    decimal.TryParse(dataReader["GIAMUA"] != null ? dataReader["GIAMUA"].ToString() : "", out GIAMUA);
                                    _DTO.GiaMua = GIAMUA;
                                    decimal.TryParse(dataReader["GIAMUAVAT"] != null ? dataReader["GIAMUAVAT"].ToString() : "", out GIAMUAVAT);
                                    _DTO.GiaMuaVat = GIAMUAVAT;
                                    decimal.TryParse(dataReader["GIABANLE"] != null ? dataReader["GIABANLE"].ToString() : "", out GIABANLE);
                                    _DTO.GiaBanLe = GIABANLE;
                                    decimal.TryParse(dataReader["GIABANBUON"] != null ? dataReader["GIABANBUON"].ToString() : "", out GIABANBUON);
                                    _DTO.GiaBanBuon = GIABANBUON;
                                    decimal.TryParse(dataReader["TYLELAIBUON"] != null ? dataReader["TYLELAIBUON"].ToString() : "", out TYLELAIBUON);
                                    _DTO.TyLeLaiBuon = TYLELAIBUON;
                                    decimal.TryParse(dataReader["TYLELAILE"] != null ? dataReader["TYLELAILE"].ToString() : "", out TYLELAILE);
                                    _DTO.TyLeLaiLe = TYLELAILE;
                                    _DTO.MaVatRa = dataReader["MAVATRA"] != null ? dataReader["MAVATRA"].ToString() : "";
                                    _DTO.MaVatVao = dataReader["MAVATVAO"] != null ? dataReader["MAVATVAO"].ToString() : "";
                                    decimal.TryParse(dataReader["TYLEVATRA"] != null ? dataReader["TYLEVATRA"].ToString() : "", out TYLEVATRA);
                                    _DTO.TyLeVatRa = TYLEVATRA;
                                    decimal.TryParse(dataReader["TYLEVATVAO"] != null ? dataReader["TYLEVATVAO"].ToString() : "", out TYLEVATVAO);
                                    _DTO.TyLeVatVao = TYLEVATVAO;
                                    decimal.TryParse(dataReader["GIABANLEVAT"] != null ? dataReader["GIABANLEVAT"].ToString() : "", out GIABANLEVAT);
                                    _DTO.GiaBanLeVat = GIABANLEVAT;
                                    decimal.TryParse(dataReader["GIABANBUONVAT"] != null ? dataReader["GIABANBUONVAT"].ToString() : "", out GIABANBUONVAT);
                                    _DTO.GiaBanBuonVat = GIABANBUONVAT;
                                    _DTO.ItemCode = dataReader["ITEMCODE"] != null ? (dataReader["ITEMCODE"].ToString() != "0" ? dataReader["ITEMCODE"].ToString() : null) : null;
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
        /// <summary>
        /// Get all entity
        /// GET
        /// </summary>
        /// <returns></returns>
        public IList<MdMerchandise> Get()
        {
            var unitCode = _service.GetCurrentUnitCode();
            var data = _service.Repository.DbSet.Where(x => x.UnitCode == unitCode);
            return data.ToList();
        }

        /// <summary>
        /// Get by id
        /// GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(MdMerchandise))]
        [CustomAuthorize(Method = "XEM", State = "mdMerchandise")]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);

            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);


        }
        [Route("UpdateMatHangToOracleRoot/{id}")]
        [HttpPut]
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "merchandise")]
        public async Task<IHttpActionResult> UpdateMatHangToOracleRoot(string id, MdMerchandiseVm.Dto instance)
        {
            var result = new TransferObj<MdMerchandise>();
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
                            cmdRoot.CommandText = "SELECT ID,MAVATTU,TENVATTU,TENVIETTAT,BARCODE FROM V_VATTU_GIABAN WHERE MAVATTU = '" + instance.MaVatTu + "' AND MADONVI = '" + rootUnitcode + "' ";
                            cmdRoot.CommandType = CommandType.Text;
                            OracleDataReader dataReaderRoot = cmdRoot.ExecuteReader();
                            if (!dataReaderRoot.HasRows)
                            {
                                return BadRequest("Không tồn tại mặt hàng này");
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
                                    // đồng bộ bảng DM_VATTU
                                    cmd.CommandText = "UPDATE DM_VATTU SET MAVATTU=:MAVATTU,TENVATTU=:TENVATTU,TENVIETTAT=:TENVIETTAT,DONVITINH=:DONVITINH,MAKEHANG=:MAKEHANG,MALOAIVATTU=:MALOAIVATTU,MANHOMVATTU=:MANHOMVATTU,MABAOBI=:MABAOBI,MAKHACHHANG=:MAKHACHHANG,MANCC=:MANCC,GIAMUA=:GIAMUA,GIABANLE:=GIABANLE,GIABANBUON=:GIABANBUON,TYLELAIBUON=:TYLELAIBUON,TYLELAILE=:TYLELAILE,BARCODE=:BARCODE,MAKHAC=:MAKHAC,MAVATVAO=:MAVATVAO,TYLE_VAT_VAO=:TYLE_VAT_VAO,MAVATRA=:MAVATRA,TYLE_VAT_RA=:TYLE_VAT_RA,GIABANVAT=:GIABANVAT,TRANGTHAI=:TRANGTHAI,ITEMCODE=:ITEMCODE,MASIZE=:MASIZE,MACOLOR=:MACOLOR,I_UPDATE_DATE:=I_UPDATE_DATE,I_UPDATE_BY=:I_UPDATE_BY,I_STATE=:I_STATE,UNITCODE=:UNITCODE,CHIETKHAUNCC=:CHIETKHAUNCC,SOTONMAX=:SOTONMAX,SOTONMIN=:SOTONMIN WHERE MAVATTU = '" + instance.MaVatTu + "' AND UNITCODE = '" + rootUnitcode + "'";
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.Add("MAVATTU", OracleDbType.NVarchar2, 50).Value = instance.MaVatTu;
                                    cmd.Parameters.Add("TENVATTU", OracleDbType.NVarchar2, 200).Value = instance.TenVatTu;
                                    cmd.Parameters.Add("TENVIETTAT", OracleDbType.NVarchar2, 200).Value = instance.TenVatTu;
                                    cmd.Parameters.Add("DONVITINH", OracleDbType.NVarchar2, 50).Value = instance.DonViTinh;
                                    cmd.Parameters.Add("MAKEHANG", OracleDbType.NVarchar2, 50).Value = instance.MaKeHang;
                                    cmd.Parameters.Add("MALOAIVATTU", OracleDbType.NVarchar2, 50).Value = instance.MaLoaiVatTu;
                                    cmd.Parameters.Add("MANHOMVATTU", OracleDbType.NVarchar2, 50).Value = instance.MaNhomVatTu;
                                    cmd.Parameters.Add("MABAOBI", OracleDbType.NVarchar2, 50).Value = instance.MaBaoBi;
                                    cmd.Parameters.Add("MAKHACHHANG", OracleDbType.NVarchar2, 50).Value = instance.MaKhachHang;
                                    cmd.Parameters.Add("MANCC", OracleDbType.NVarchar2, 50).Value = instance.MaNCC;
                                    cmd.Parameters.Add("GIAMUA", OracleDbType.Decimal).Value = instance.GiaMua;
                                    cmd.Parameters.Add("GIABANLE", OracleDbType.Decimal).Value = instance.GiaBanLe;
                                    cmd.Parameters.Add("GIABANBUON", OracleDbType.Decimal).Value = instance.GiaBanBuon;
                                    cmd.Parameters.Add("TYLELAIBUON", OracleDbType.Decimal).Value = instance.TyLeLaiBuon;
                                    cmd.Parameters.Add("TYLELAILE", OracleDbType.Decimal).Value = instance.TyLeLaiLe;
                                    cmd.Parameters.Add("BARCODE", OracleDbType.NVarchar2, 2000).Value = instance.Barcode;
                                    cmd.Parameters.Add("MAKHAC", OracleDbType.NVarchar2, 50).Value = instance.MaKhac;
                                    cmd.Parameters.Add("MAVATVAO", OracleDbType.NVarchar2, 50).Value = instance.MaVatVao;
                                    cmd.Parameters.Add("TYLE_VAT_VAO", OracleDbType.Decimal).Value = instance.TyLeVatVao;
                                    cmd.Parameters.Add("MAVATRA", OracleDbType.NVarchar2, 50).Value = instance.MaVatRa;
                                    cmd.Parameters.Add("TYLE_VAT_RA", OracleDbType.Decimal).Value = instance.TyLeVatRa;
                                    cmd.Parameters.Add("GIABANVAT", OracleDbType.Decimal).Value = instance.GiaBanLeVat;
                                    cmd.Parameters.Add("TRANGTHAI", OracleDbType.Int32).Value = instance.TrangThai;
                                    cmd.Parameters.Add("ITEMCODE", OracleDbType.NVarchar2, 50).Value = instance.ItemCode;
                                    cmd.Parameters.Add("MASIZE", OracleDbType.NVarchar2, 50).Value = instance.MaSize;
                                    cmd.Parameters.Add("MACOLOR", OracleDbType.NVarchar2, 50).Value = instance.MaColor;
                                    cmd.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmd.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name + "|" + unitCode;
                                    cmd.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    cmd.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                                    cmd.Parameters.Add("CHIETKHAUNCC", OracleDbType.Decimal).Value = 0;
                                    cmd.Parameters.Add("SOTONMAX", OracleDbType.Decimal).Value = 0;
                                    cmd.Parameters.Add("SOTONMIN", OracleDbType.Decimal).Value = 0;
                                    // đồng bộ bảng DM_VATTU_GIACA
                                    OracleCommand cmdGiaBan = new OracleCommand();
                                    cmdGiaBan.Connection = connection;
                                    cmdGiaBan.InitialLONGFetchSize = 1000;
                                    cmdGiaBan.CommandText = "UPDATE DM_VATTU_GIACA SET MAVATTU=:MAVATTU,MADONVI=:MADONVI,GIAVON=:GIAVON,GIAMUAVAT=:GIAMUAVAT,GIAMUA=:GIAMUA,GIABANLE=:GIABANLE,GIABANBUON=:GIABANBUON,TY_LELAI_BUON=:TY_LELAI_BUON,TY_LELAI_LE=:TY_LELAI_LE,MAVATVAO=:MAVATVAO,TYLE_VAT_VAO=:TYLE_VAT_VAO,MAVATRA=:MAVATRA,TYLE_VAT_RA=:TYLE_VAT_RA,GIA_BANLE_VAT=:GIA_BANLE_VAT,GIA_BANBUON_VAT=:GIA_BANBUON_VAT,I_UPDATE_DATE=:I_UPDATE_DATE,I_UPDATE_BY=:I_UPDATE_BY,I_STATE=:I_STATE,UNITCODE=:UNITCODE,SOTONMAX=:SOTONMAX,SOTONMIN=:SOTONMIN WHERE MAVATTU = '" + instance.MaVatTu + "' AND MADONVI = '" + rootUnitcode + "'";
                                    cmdGiaBan.CommandType = CommandType.Text;
                                    cmdGiaBan.Parameters.Add("MAVATTU", OracleDbType.NVarchar2, 50).Value = instance.MaVatTu;
                                    cmdGiaBan.Parameters.Add("MADONVI", OracleDbType.NVarchar2, 200).Value = rootUnitcode;
                                    cmdGiaBan.Parameters.Add("GIAVON", OracleDbType.Decimal).Value = instance.GiaVon;
                                    cmdGiaBan.Parameters.Add("GIAMUAVAT", OracleDbType.Decimal).Value = instance.GiaMuaVat;
                                    cmdGiaBan.Parameters.Add("GIAMUA", OracleDbType.Decimal).Value = instance.GiaMua;
                                    cmdGiaBan.Parameters.Add("GIABANLE", OracleDbType.Decimal).Value = instance.GiaBanLe;
                                    cmdGiaBan.Parameters.Add("GIABANBUON", OracleDbType.Decimal).Value = instance.GiaBanBuon;
                                    cmdGiaBan.Parameters.Add("TY_LELAI_BUON", OracleDbType.Decimal).Value = instance.TyLeLaiBuon;
                                    cmdGiaBan.Parameters.Add("TY_LELAI_LE", OracleDbType.Decimal).Value = instance.TyLeLaiLe;
                                    cmdGiaBan.Parameters.Add("MAVATVAO", OracleDbType.NVarchar2, 200).Value = instance.MaVatVao;
                                    cmdGiaBan.Parameters.Add("TYLE_VAT_VAO", OracleDbType.Decimal).Value = instance.TyLeVatVao;
                                    cmdGiaBan.Parameters.Add("MAVATRA", OracleDbType.NVarchar2, 200).Value = instance.MaVatRa;
                                    cmdGiaBan.Parameters.Add("TYLE_VAT_RA", OracleDbType.Decimal).Value = instance.TyLeVatRa;
                                    cmdGiaBan.Parameters.Add("GIA_BANLE_VAT", OracleDbType.Decimal).Value = instance.GiaBanLeVat;
                                    cmdGiaBan.Parameters.Add("GIA_BANBUON_VAT", OracleDbType.Decimal).Value = instance.GiaBanBuonVat;
                                    cmdGiaBan.Parameters.Add("I_UPDATE_DATE", OracleDbType.Date).Value = DateTime.Now;
                                    cmdGiaBan.Parameters.Add("I_UPDATE_BY", OracleDbType.NVarchar2, 50).Value = _service.GetClaimsPrincipal().Identity.Name + "|" + unitCode;
                                    cmdGiaBan.Parameters.Add("I_STATE", OracleDbType.NVarchar2, 50).Value = 10;
                                    cmdGiaBan.Parameters.Add("UNITCODE", OracleDbType.NVarchar2, 50).Value = rootUnitcode;
                                    cmdGiaBan.Parameters.Add("SOTONMAX", OracleDbType.Decimal).Value = 0;
                                    cmdGiaBan.Parameters.Add("SOTONMIN", OracleDbType.Decimal).Value = 0;
                                    OracleTransaction transactionVatTu;
                                    OracleTransaction transactionGiaBan;
                                    try
                                    {
                                        transactionVatTu = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmd.Transaction = transactionVatTu;
                                        int count = cmd.ExecuteNonQuery();
                                        transactionVatTu.Commit();
                                        transactionGiaBan = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                                        cmdGiaBan.Transaction = transactionGiaBan;
                                        int countGiaBan = cmdGiaBan.ExecuteNonQuery();
                                        transactionGiaBan.Commit();
                                        if (count > 0 && countGiaBan > 0)
                                        {
                                            result.Status = true;
                                            result.Message = "Cập nhật đồng bộ thành công";
                                        }
                                        else
                                        {
                                            transactionVatTu.Rollback();
                                            transactionGiaBan.Rollback();
                                            result.Status = false;
                                            result.Message = "Cập nhật đồng bộ không thành công";
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
        /// <summary>
        /// Update entity
        /// PUT
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "merchandise")]
        public async Task<IHttpActionResult> Put(string id, MdMerchandiseVm.MasterDto instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            TransferObj<MdMerchandise> result = new TransferObj<MdMerchandise>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                MdMerchandise item = _service.UpdateDto(instance);
                int rs = await _service.UnitOfWork.SaveAsync();
                if (rs > 0)
                {
                    result.Status = true;
                    result.Data = item;
                    result.Message = "Cập nhật thành công";
                }
                else
                {
                    result.Status = false;
                    result.Message = "Cập nhật thất bại";
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }

        [ResponseType(typeof(void))]
        [Route("UpdateCodeGroup")]
        [CustomAuthorize(Method = "SUA", State = "merchandise")]
        public async Task<IHttpActionResult> UpdateCodeGroup(MdMerchandiseVm.MasterDto instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = new TransferObj<MdMerchandise>();
            try
            {
                var data = _service.ModifieldCodeMerchandise(instance.MaVatTu, instance.Id, instance.MaNhomVatTu);
                if (data == true)
                {
                    result.Status = true;
                    result.Message = "Cập nhật thành công !";
                    return Ok(result);
                }
                else
                {
                    result.Status = false;
                    result.Message = "Không thành công !";
                    return Ok(result);
                }

            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }
        /// <summary>
        /// Create entity
        /// POST
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(MdMerchandise))]
        [Route("Post")]
        [CustomAuthorize(Method = "THEM", State = "merchandise")]
        public async Task<IHttpActionResult> Post(MdMerchandiseVm.MasterDto instance)
        {
            var result = new TransferObj<MdMerchandise>();
            try
            {
                var item = _service.InsertDto(instance);
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

        [Route("UploadMerchandiseImage")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> UploadMerchandiseImage()
        {
            var result = new TransferObj<string>();
            try
            {
                result.Status = true;
                result.Data = _service.UploadImage(false);
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }
        [Route("UploadAvatar")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> UploadAvatar()
        {
            var result = new TransferObj<string>();
            try
            {
                result.Status = true;
                result.Data = _service.UploadImage(true);
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }
        [ResponseType(typeof(MdMerchandise))]
        [CustomAuthorize(Method = "XOA", State = "mdMerchandise")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdMerchandise instance = await _service.Repository.FindAsync(id);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                if (instance.MaCha != null)
                {
                    var lstMerChild = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.Where(x => x.MaCha == instance.MaCha).ToList();
                    if (lstMerChild.Count <= 1)
                    {
                        var mer = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu == instance.MaCha);
                        if (mer != null)
                        {
                            mer.TrangThaiCon = 0;
                            mer.ObjectState = ObjectState.Modified;
                        }
                    }
                }
                _service.Delete(instance.Id);
                await _service.UnitOfWork.SaveAsync();
                return Ok(instance);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("GetForNvByCode/{code}/{wareHouseCode}/{unitCode}")]
        public async Task<IHttpActionResult> GetForNvByCode(string code, string wareHouseCode, string unitCode)
        {
            TransferObj<MdMerchandiseVm.Dto> result = new TransferObj<MdMerchandiseVm.Dto>();
            string _parent = _service.GetParentUnitCode();
            ProcedureService<MdMerchandiseVm.Dto> service = new ProcedureService<MdMerchandiseVm.Dto>();
            IQueryable<MdMerchandiseVm.Dto> data = ProcedureCollection.GetMerchandise(new ERPContext(), code, unitCode);
            if (data != null && data.Count() == 1)
            {
                MdMerchandiseVm.Dto item = data.ToList()[0];
                InventoryExpImp xntItem = ProcedureCollection.GetCostOfGoodsSoldByMerchandise(unitCode, wareHouseCode, item.MaVatTu);
                if (item.MaBaoBi != null)
                {
                    string maBao = item.MaBaoBi;
                    MdPackaging baoBi = _service.UnitOfWork.Repository<MdPackaging>().DbSet.FirstOrDefault(u => u.MaBaoBi == maBao && u.UnitCode.StartsWith(_parent));
                    if (baoBi != null)
                    {
                        item.LuongBao = baoBi.SoLuong;
                    }
                }
                if (xntItem != null)
                {
                    decimal soLuongXuat = 0;
                    using (OracleConnection connection = new OracleConnection(new ERPContext().Database.Connection.ConnectionString))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandText = "SELECT " +
                                                        "NVL(SUM(vtctct.soluong), 0) soluong " +
                                                    "FROM " +
                                                        "vattuchungtu vtct " +
                                                        "INNER JOIN vattuchungtuchitiet vtctct ON vtct.machungtupk = vtctct.machungtupk " +
                                                    "WHERE " +
                                                        "vtct.trangthai = 0 " +
                                                        "AND vtct.ngaychungtu <= sysdate " +
                                                        "AND (vtct.loaichungtu = 'DCX' OR vtct.loaichungtu = 'XKHAC' OR vtct.loaichungtu = 'XBAN') " +
                                                        "AND vtct.makhoxuat = :makhoxuat " +
                                                        "AND vtctct.mahang = :mavattu " +
                                                        "AND vtct.unitcode = :unitcode";
                            command.Parameters.Add(new OracleParameter(":makhoxuat", OracleDbType.NVarchar2, wareHouseCode, ParameterDirection.Input));
                            command.Parameters.Add(new OracleParameter(":mavattu", OracleDbType.NVarchar2, item.MaVatTu, ParameterDirection.Input));
                            command.Parameters.Add(new OracleParameter(":unitcode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input));
                            using (OracleDataReader dataReader = command.ExecuteReader())
                            {
                                if (dataReader.HasRows)
                                {
                                    var SoLuongOrdinal = dataReader.GetOrdinal("soluong");
                                    while (await dataReader.ReadAsync())
                                    {
                                        soLuongXuat = dataReader.GetDecimal(SoLuongOrdinal);
                                    }
                                }
                            }
                        }
                    }

                    item.SoLuongTon = xntItem.ClosingQuantity - soLuongXuat;
                    item.SoLuongNhapTrongKy = xntItem.IncreaseQuantity;
                    item.SoLuongXuatTrongKy = xntItem.DecreaseQuantity;
                    item.GiaVon = xntItem.CostOfCapital == 0 ? 0 : xntItem.CostOfCapital;
                    item.GiaVonVat = item.GiaVon * (1 + item.TyLeVatVao / 100);
                    result.Message = "Ok";
                }
                else
                {
                    result.Message = "NotFound";
                }
                result.Status = true;
                result.Data = item;
                return Ok(result);
            }
            else
            {
                result.Status = false;
                return Ok(result);
            }

        }
        [Route("GetByCodeWithGiaVon/{code}/{makho}/{maDonVi}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetByCodeWithGiaVon(string code, string makho, string maDonVi)
        {
            //buộc phải chọn mã kho
            var service = new ProcedureService<MdMerchandiseVm.Dto>();
            var result = new TransferObj<MdMerchandiseVm.Dto>();
            var unitCode = _service.GetCurrentUnitCode();
            var data = ProcedureCollection.GetMerchandise(new BTS.API.ENTITY.ERPContext(), code, unitCode);
            if (maDonVi == unitCode)
            {
                if (data != null && data.Count() == 1)
                {
                    var items = data.ToList();
                    var xntItem = ProcedureCollection.GetCostOfGoodsSoldByMerchandise(unitCode, makho, items[0].MaHang);
                    if (xntItem != null)
                    {
                        //items[0].GiaVon = (Math.Abs(xntItem.OpeningBalanceQuantity) + Math.Abs(xntItem.IncreaseQuantity)) == 0 ? 0 : (Math.Abs(xntItem.OpeningBalanceValue) + Math.Abs(xntItem.IncreaseValue)) / (Math.Abs(xntItem.OpeningBalanceQuantity) + Math.Abs(xntItem.IncreaseQuantity));
                        items[0].GiaVon = xntItem.CostOfCapital == 0 ? 0 : xntItem.CostOfCapital;
                        items[0].GiaVonVat = items[0].GiaVon * (1 + items[0].TyLeVatVao / 100);
                        items[0].SoLuongTon = xntItem.ClosingQuantity;
                    }
                    if (items[0].MaBaoBi != null)
                    {
                        string maBao = items[0].MaBaoBi;
                        var baoBi = _service.UnitOfWork.Repository<MdPackaging>().DbSet.FirstOrDefault(u => u.MaBaoBi == maBao);

                        if (baoBi != null)
                        {
                            items[0].LuongBao = baoBi.SoLuong;
                        }
                    }
                    result.Data = items[0];
                    result.Status = true;
                    result.Message = "Oke";
                }
                else
                {
                    result.Data = new MdMerchandiseVm.Dto();
                    result.Status = false;
                    result.Message = "Không tìm thấy mã hàng !";
                }
            }
            else
            {
                if (data != null && data.Count() == 1)
                {
                    var items = data.ToList();
                    items[0].GiaVon = items[0].GiaMua;
                    items[0].GiaVonVat = items[0].GiaVon * (1 + items[0].TyLeVatVao / 100);
                    if (items[0].MaBaoBi != null)
                    {
                        string maBao = items[0].MaBaoBi;
                        var baoBi = _service.UnitOfWork.Repository<MdPackaging>().DbSet.FirstOrDefault(u => u.MaBaoBi == maBao);

                        if (baoBi != null)
                        {
                            items[0].LuongBao = baoBi.SoLuong;
                        }
                    }
                    result.Data = items[0];
                    result.Status = true;
                    result.Message = "Oke";

                }
                else
                {
                    result.Data = new MdMerchandiseVm.Dto();
                    result.Status = false;
                    result.Message = "Không tìm thấy mã hàng !";
                }
            }
            return Ok(result);
        }
        [Route("PostInfoCodeByDateTime")]
        public async Task<IHttpActionResult> PostInfoCodeByDateTime(NvXuatBanVm.Dto chungTu)
        {
            MdMerchandiseVm.Dto result = null;
            var unitCode = _service.GetCurrentUnitCode();
            var service = new ProcedureService<MdMerchandiseVm.Dto>();
            var data = ProcedureCollection.GetMerchandise(new BTS.API.ENTITY.ERPContext(), chungTu.MaVatTu, unitCode);
            DateTime ngayGiaVon;
            if (chungTu.TrangThai != 10)
            {
                ngayGiaVon = CurrentSetting.GetKhoaSo(unitCode).FromDate;
            }
            else
            {
                ngayGiaVon = (DateTime)chungTu.NgayDuyetPhieu;
            }
            //{2/8/2017 12:00:00 AM
            string kyKeToan = _servicePeriod.GetKyKeToan(ngayGiaVon);

            if (data != null && data.Count() == 1)
            {
                var items = data.ToList();
                var xntItem = ProcedureCollection.GetCostByPeriodMerchandise(unitCode, kyKeToan, items[0].MaHang);
                if (xntItem != null)
                {
                    //items[0].GiaVon = (Math.Abs(xntItem.OpeningBalanceQuantity) + Math.Abs(xntItem.IncreaseQuantity)) == 0 ? 0 : (Math.Abs(xntItem.OpeningBalanceValue) + Math.Abs(xntItem.IncreaseValue)) / (Math.Abs(xntItem.OpeningBalanceQuantity) + Math.Abs(xntItem.IncreaseQuantity));
                    items[0].GiaVon = xntItem.CostOfCapital == 0 ? 0 : xntItem.CostOfCapital;
                    items[0].GiaVonVat = items[0].GiaVon * (1 + items[0].TyLeVatVao / 100);
                }
                if (items[0].MaBaoBi != null)
                {
                    var baoBi = _service.UnitOfWork.Repository<MdPackaging>().DbSet.FirstOrDefault(u => u.MaBaoBi == items[0].MaBaoBi);

                    if (baoBi != null)
                    {
                        items[0].LuongBao = baoBi.SoLuong;
                    }
                }
                return Ok(items[0]);
            }
            else
            {
                return NotFound();
            }
        }
        [Route("GetByCode/{code}")]
        public async Task<IHttpActionResult> GetByCode(string code)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaVatTu == code);// && x.UnitCode == unitCode);
            if (instance == null)
            {
                return NotFound();
            }

            return Ok(instance);
        }
        [Route("PostItemInventoryByCode")]
        public async Task<IHttpActionResult> PostItemInventoryByCode(InventoryExpImp instance)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var item = ProcedureCollection.GetInventoryItem(instance.Code, instance.WareHouseCode, unitCode);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
        [Route("GetByItemCodeNotNull")]
        public async Task<IHttpActionResult> GetByItemCodeNotNull()
        {
            MdMerchandiseVm.Dto result = null;
            var unitCode = _service.GetCurrentUnitCode();
            //var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaVatTu.ToUpper() == code.ToUpper());//&& x.UnitCode == unitCode);
            var service = new ProcedureService<MdMerchandiseVm.Dto>();
            var data = ProcedureCollection.GetMerchandiseItemCode(new BTS.API.ENTITY.ERPContext(), unitCode);
            if (data != null && data.Count() >= 1)
            {
                var item = data.ToList();
                try
                {
                    var filenameTemp = "macandientu";
                    var pathRelaTemp = string.Format(@"~/Upload/Barcode/");

                    var pathAbsTemp = HostingEnvironment.MapPath(pathRelaTemp);
                    if (pathAbsTemp != null)
                    {
                        var getAbsoluteDirectoryInfoReport = new DirectoryInfo(pathAbsTemp);
                        getAbsoluteDirectoryInfoReport.Create();
                    }
                    var tempFile = new FileInfo(pathAbsTemp + filenameTemp + ".xlsx");
                    var pathRela = string.Format(@"~/Upload/Barcode/");
                    var pathAbs = HostingEnvironment.MapPath(pathRela);
                    var getAbsoluteDirectoryInfo = new DirectoryInfo(pathAbs);
                    getAbsoluteDirectoryInfo.Create();
                    var newFileName = "MaCanDienTu_" + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString();
                    FileInfo newFile =
                        new FileInfo(pathAbs + @"MaCanDienTu.xls");

                    if (newFile.Exists) //Kiểm tra file mới này đã tồn tại chưa
                    {
                        newFile.Delete();
                        newFile = new FileInfo(pathAbs + @"MaCanDienTu.xls");
                    }
                    using (ExcelPackage package = new ExcelPackage(newFile, tempFile))
                    {
                        var worksheet = package.Workbook.Worksheets[1];
                        for (int i = 0; i < item.Count(); i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = i + 1;
                            worksheet.Cells[i + 2, 2].Value = "20";
                            worksheet.Cells[i + 2, 3].Value = item[i].ItemCode.ToString();
                            worksheet.Cells[i + 2, 5].Value = item[i].ItemCode.ToString();
                            worksheet.Cells[i + 2, 6].Value = CurrentSetting.UnicodeToASCII(item[i].TenHang.ToString());
                            worksheet.Cells[i + 2, 20].Value = (item[i].GiaBanLeVat * 1000) + ".00000";

                        }
                        package.SaveAs(newFile);
                    }
                }
                catch (Exception ex)
                {

                }
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }
        [Route("GetPrice/{maVatTu}")]
        public async Task<IHttpActionResult> GetPrice(string maVatTu)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var result = _service.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.FirstOrDefault(x => x.MaVatTu == maVatTu && x.MaDonVi == unitCode);
            if (result == null)
            {
                return NotFound();
            }
            try
            {
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }
        [ResponseType(typeof(void))]
        [Route("PutMerchandisePrice/{id}")]
        public async Task<IHttpActionResult> PutMerchandisePrice(string id, MdMerchandisePrice instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdMerchandisePrice>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                instance.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
                _service.UnitOfWork.Repository<MdMerchandisePrice>().Update(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }
        [Route("GetNewCode/{maLoai}")]
        public string GetNewCode(string maLoai)
        {
            return _service.BuildCode(maLoai);
        }

        public string BuildCodeRoot(string code)
        {
            var result = "";
            var type = TypeMasterData.VATTU.ToString();
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
                        cmd.CommandText = "SELECT MALOAIVATTU,TENLOAIVT,UNITCODE FROM DM_LOAIVATTU WHERE TRANGTHAI = 10 AND MALOAIVATTU = '" + code + "' AND UNITCODE = '" + rootUnitcode + "' AND ROWNUM = 1";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                MdIdBuilder config = new MdIdBuilder();
                                cmd.CommandText = "SELECT ID,TYPE,CODE,\"CURRENT\",UNITCODE FROM MD_ID_BUILDER WHERE TYPE = '" + type + "' AND CODE = '" + dataReader["MALOAIVATTU"] + "' AND UNITCODE = '" + maDonViCha + "' AND ROWNUM = 1";
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
                                            Code = dataReader["MALOAIVATTU"].ToString(),
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
                                        Code = dataReader["MALOAIVATTU"].ToString(),
                                        Current = "000000",
                                        UnitCode = maDonViCha,
                                    };
                                }
                                var soMa = config.GenerateNumber();
                                config.Current = soMa;
                                result = string.Format("{0}{1}", config.Code, soMa);
                            }
                        }
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

        public string SaveCodeRoot(string code)
        {
            var result = "";
            var type = TypeMasterData.VATTU.ToString();
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
                        cmd.CommandText = "SELECT MALOAIVATTU,TENLOAIVT,UNITCODE FROM DM_LOAIVATTU WHERE TRANGTHAI = 10 AND MALOAIVATTU = '" + code + "' AND UNITCODE = '" + rootUnitcode + "' AND ROWNUM = 1";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                MdIdBuilder config = new MdIdBuilder();
                                cmd.CommandText = "SELECT ID,TYPE,CODE,\"CURRENT\",UNITCODE FROM MD_ID_BUILDER WHERE TYPE = '" + type + "' AND CODE = '" + dataReader["MALOAIVATTU"] + "' AND UNITCODE = '" + maDonViCha + "' AND ROWNUM = 1";
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
                                            Code = dataReader["MALOAIVATTU"].ToString(),
                                            Current = dataReaderBuilder["CURRENT"].ToString(),
                                            UnitCode = maDonViCha,
                                        };
                                        result = config.GenerateNumber();
                                        config.Current = result;
                                        cmd.CommandText = "UPDATE MD_ID_BUILDER SET \"CURRENT\" = '" + result + "' WHERE TYPE = '" + type + "' AND CODE = '" + dataReader["MALOAIVATTU"] + "' AND UNITCODE = '" + maDonViCha + "' ";
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
                                        Code = dataReader["MALOAIVATTU"].ToString(),
                                        Current = "000000",
                                        UnitCode = maDonViCha,
                                    };
                                    result = config.GenerateNumber();
                                    config.Current = result;
                                    cmd.CommandText = "INSERT INTO MD_ID_BUILDER(ID,TYPE,CODE,CURRENT,UNITCODE) VALUES ('" + config.Id + "','" + config.Type + "','" + config.Code + "','" + config.Current + "','" + config.UnitCode + "')";
                                    cmd.CommandType = CommandType.Text;
                                    int count = cmd.ExecuteNonQuery();
                                }
                                result = string.Format("{0}{1}", config.Code, result);
                            }
                        }
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

        [Route("GetNewCodeRoot/{maLoai}")]
        public string GetNewCodeRoot(string maLoai)
        {
            return BuildCodeRoot(maLoai);
        }

        //Merchandise Child
        #region Merchandise Child
        [ResponseType(typeof(MdMerchandise))]
        [Route("PostMerchandiseChild")]
        public async Task<IHttpActionResult> PostMerchandiseChild(MdMerchandiseVm.MasterDto instance)
        {
            var result = new TransferObj<MdMerchandise>();
            try
            {
                var item = _service.InsertChild(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Data = item;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
            return Ok(result);
            //return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
        }
        [Route("GetNewCodeChild/{maVatTu}")]
        public string GetNewCodeChild(string maVatTu)
        {
            return _service.BuildCodeChild(maVatTu);
        }
        [Route("GetAllDataChild/{maVatTu}")]
        public async Task<IHttpActionResult> GetAllDataChild(string maVatTu)
        {
            var result = new TransferObj<List<MdMerchandiseVm.MasterDto>>();
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaVatTu == maVatTu);
            if (instance != null)
            {
                try
                {
                    var data = _service.GetAllDataChild(instance.MaVatTu);
                    result.Data = data;
                    result.Status = true;
                    return Ok(result);
                }
                catch (Exception e)
                {
                    result.Status = false;
                    result.Message = e.Message;
                    return Ok(result);
                }

            }
            else
            {
                return NotFound();
            }
        }
        #endregion
        [Route("GetNewCanDienTu")]
        public string GetNewCanDienTu()
        {
            return _service.BuildCodeCanDienTu();
        }
        [Route("WriteDataToExcel")]
        public async Task<IHttpActionResult> WriteDataToExcel(List<MdMerchandiseVm.Dto> data)
        {
            var result = new TransferObj<MdMerchandiseVm.Dto>();
            try
            {
                var filenameTemp = "TemPlateNhapMua";
                var pathRelaTemp = string.Format(@"~/Upload/Barcode/");

                var pathAbsTemp = HostingEnvironment.MapPath(pathRelaTemp);
                if (pathAbsTemp != null)
                {
                    var getAbsoluteDirectoryInfoReport = new DirectoryInfo(pathAbsTemp);
                    getAbsoluteDirectoryInfoReport.Create();
                }
                var tempFile = new FileInfo(pathAbsTemp + filenameTemp + ".xlsx");
                var pathRela = string.Format(@"~/Upload/Barcode/");
                var pathAbs = HostingEnvironment.MapPath(pathRela);
                var getAbsoluteDirectoryInfo = new DirectoryInfo(pathAbs);
                getAbsoluteDirectoryInfo.Create();
                FileInfo newFile =
                    new FileInfo(pathAbs + "Barcode" + @".xls");

                if (newFile.Exists) //Kiểm tra file mới này đã tồn tại chưa
                {
                    newFile.Delete();
                    newFile = new FileInfo(pathAbs + @"Barcode.xls");
                }

                using (ExcelPackage package = new ExcelPackage(newFile, tempFile))
                {
                    var worksheet = package.Workbook.Worksheets[1];
                    int index = 0;
                    for (int i = 0; i < data.Count; i++)
                    {
                        for (int j = 0; j < data[i].SoLuong; j++)
                        {
                            worksheet.Cells[index + 2, 1].Value = index + 1;
                            worksheet.Cells[index + 2, 2].Value = data[i].MaVatTu;
                            worksheet.Cells[index + 2, 3].Value = CurrentSetting.UnicodetoTCVN222(data[i].TenHang);
                            //if(data[i].Barcode.Length>1)
                            worksheet.Cells[index + 2, 4].Value = _getFirstBarcode(data[i].Barcode);
                            worksheet.Cells[index + 2, 5].Value = CurrentSetting.FormatTienViet(data[i].GiaBanBuonVat.ToString()) + " VND";
                            worksheet.Cells[index + 2, 6].Value = CurrentSetting.FormatTienViet(data[i].GiaBanLeVat.ToString()) + " VND";
                            worksheet.Cells[index + 2, 7].Value = data[i].MaKhachHang;
                            worksheet.Cells[index + 2, 8].Value = "1";
                            index++;
                        }
                    }
                    package.SaveAs(newFile);
                    package.Dispose();
                }
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Data = null;
                result.Message = ex + "";
                WriteLogs.LogError(ex);
            }
            return Ok(result);
        }
        [Route("GetDetailInTem/{code}")]
        public string GetDetailInTem(string code)
        {
            var result = new MdMerchandiseVm.MasterDto();
            var unitCode = _service.GetCurrentUnitCode();
            var giaBanLe = _servicePrice.Repository.DbSet.Where(x => x.MaVatTu == code && x.MaDonVi == unitCode).Select(x => x.GiaBanLeVat).FirstOrDefault();
            if (giaBanLe == null)
            {
                return "0";
            }
            return giaBanLe.ToString();
        }
        [Route("GetMerchandiseForActionInventory/{makho}")]
        public HttpResponseMessage GetMerchandiseForActionInventory(string makho)
        {
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
            if (string.IsNullOrEmpty(makho))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }
            var pathRelaTemp = string.Format(@"~/Upload/XML/");
            var pathAbsTemp = HostingEnvironment.MapPath(pathRelaTemp);
            if (pathAbsTemp != null)
            {
                var getAbsoluteDirectoryInfoReport = new DirectoryInfo(pathAbsTemp);
                getAbsoluteDirectoryInfoReport.Create();
            }
            var pathRela = string.Format(@"~/Upload/XML/");
            var pathAbs = HostingEnvironment.MapPath(pathRela);
            var getAbsoluteDirectoryInfo = new DirectoryInfo(pathAbs);
            List<MdMerchandiseVm.MATHANG> lstHangs = new List<MdMerchandiseVm.MATHANG>();
            var result = ProcedureCollection.GetInventoryForActionInventory(unitCode, makho);
            foreach (var soTon in result)
            {
                soTon.Soluong = Convert.ToInt32(soTon.Soluong);
                lstHangs.Add(soTon);
            }
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<MdMerchandiseVm.MATHANG>));
            var path = pathAbs + "Data.xml";
            System.IO.FileStream file = System.IO.File.Create(path);
            writer.Serialize(file, lstHangs);
            response.StatusCode = HttpStatusCode.OK;
            file.Seek(0, SeekOrigin.Begin);
            response.Content = new StreamContent(file);

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Data.xml" };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            return response;
        }
        [Route("FilterMerchandiseCodes/{maVatTu}")]
        [ResponseType(typeof(MdMerchandise))]
        public MdMerchandise FilterMerchandiseCodes(string maVatTu)
        {
            MdMerchandise typeMer = new MdMerchandise();
            if (string.IsNullOrEmpty(maVatTu))
            {
                typeMer = null;
            }
            else
            {
                maVatTu = maVatTu.ToUpper();
                maVatTu = maVatTu.Trim();
                string unitCode = _service.GetCurrentUnitCode();
                typeMer = _service.Repository.DbSet.Where(x => x.MaVatTu == maVatTu).FirstOrDefault(x => x.UnitCode == unitCode);
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
        private string _getFirstBarcode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            string result = "";
            if (str.Length > 0)
            {
                str = str.Substring(1);
                var subStrAray = str.Split(';');
                result = subStrAray[0];
                return result;
            }
            else
            {
                return " ";
            }
        }
        [Route("PostExportExcel")]
        [HttpPost]
        public HttpResponseMessage PostExportExcel(List<MdMerchandiseTypeVm.ObjectTranfer> para)
        {
            List<MdMerchandiseVm.Dto> lstData = new List<MdMerchandiseVm.Dto>();
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                List<string> codeType = new List<string>();
                foreach (var value in para)
                {
                    codeType.Add(value.Value);
                }

                var lstCodeType = String.Join(",", codeType);
                MemoryStream streamData = new MemoryStream();
                var unitCode = _service.GetCurrentUnitCode();
                var service = new ProcedureService<MdMerchandiseVm.Dto>();
                var merchandise = ProcedureCollection.GetMerchandiseByType(new BTS.API.ENTITY.ERPContext(), lstCodeType, unitCode);
                if (merchandise != null)
                {
                    lstData = merchandise.ToList();
                    streamData = _service.ExportExcel(lstData);
                }
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "HangHoa.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostHangTonAm")]
        public async Task<IHttpActionResult> PostHangTonAm()
        {
            var unitCode = _service.GetCurrentUnitCode();
            var warehouse = unitCode + "-K2";
            var data = ProcedureCollection.LayThongTinHangTonAm(unitCode, warehouse);
            if (data != null && data.Count > 0)
            {
                return Ok(data);
            }
            else
            {
                return NotFound();
            }

        }
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }
        public List<MdMerchandise> ResultChildrenVatTu(string code, string name, string[] arrayColor, string[] arraySize, string unitCode)
        {
            List<MdMerchandise> result = new List<MdMerchandise>();
            for (int i = 0; i < arrayColor.Count(); i++)
            {
                string mau = "";
                if (!string.IsNullOrEmpty(arrayColor[i].ToString()))
                {
                    string tmp = arrayColor[i].ToString();
                    tmp = tmp.Trim();
                    mau = _service.UnitOfWork.Repository<MdColor>()
                   .DbSet.FirstOrDefault(x => x.MaColor == tmp && x.UnitCode.StartsWith(unitCode))
                   .TenColor;
                }
                for (int j = 0; j < arraySize.Count(); j++)
                {
                    string size = "";
                    MdMerchandise merchandise = new MdMerchandise();
                    if (!string.IsNullOrEmpty(arraySize[j].ToString()))
                    {
                        string sie = arraySize[j].ToString();
                        sie = sie.Trim();
                        size =
                            _service.UnitOfWork.Repository<MdSize>()
                                .DbSet.FirstOrDefault(x => x.MaSize == sie && x.UnitCode.StartsWith(unitCode))
                                .TenSize;
                    }
                    size = size.Replace("Size", "");
                    size = size.Replace(" ", "");
                    merchandise.MaVatTu = code + "-" + RemoveUnicode(mau.ToUpper()) + "-" + size;
                    merchandise.TenHang = name + " màu " + mau + " Size " + size;
                    merchandise.MaColor = arrayColor[i];
                    merchandise.MaSize = arraySize[j];
                    result.Add(merchandise);
                }
            }
            return result;
        }
        //AnhPT
        [Route("UploadFile/{unitCode}")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UploadFile(string unitCode)
        {
            var result = new TransferObj();
            bool flag = true;
            int countInsertMerchandise = 0, countInsertMerchandisePrice = 0;
            var message = new MdMerchandiseVm.MessageImportExcel();
            List<MdMerchandiseVm.MessageImportExcel> lstMessage = new List<MdMerchandiseVm.MessageImportExcel>();
            string path = _service.GetPhysicalPathImportFile();
            //string path = @"C:/inetpub/wwwroot/wss/VirtualDirectories/BANHANG/Upload/DataHangHoa/";
            HttpRequest request = HttpContext.Current.Request;
            try
            {
                if (request.Files.Count > 0)
                {
                    List<MdMerchandise> lstMerchandise = new List<MdMerchandise>();
                    List<MdMerchandisePrice> lstMerchandisePrice = new List<MdMerchandisePrice>();
                    HttpPostedFile file = request.Files[0];
                    file.SaveAs(path + file.FileName);
                    if (File.Exists(path + file.FileName))
                    {
                        //doc du lieu tu file
                        Excel.Application xlApp;
                        Excel.Workbook xlWorkBook;
                        Excel.Worksheet xlWorkSheet;
                        Excel.Range range;
                        int rw = 0, cl = 0;
                        xlApp = new Excel.Application();
                        string pathFile = path + file.FileName;
                        xlWorkBook = xlApp.Workbooks.Open(@"" + pathFile + "", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                        range = xlWorkSheet.UsedRange;
                        rw = range.Rows.Count;
                        cl = range.Columns.Count;
                        Excel.Range last = xlWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
                        int lastUsedRow = last.Row;
                        for (int rCnt = 2; rCnt <= lastUsedRow; rCnt++) //từ row 2 -- column 1
                        {
                            int cCnt = 1;
                            decimal tyLeVatRa = 0, tyLeVatVao = 0, giaMuaChuaVat = 0, giaBanLeCoVat = 0, giaBanBuonCoVat = 0;
                            decimal giaBanLeChuaVat = 0, giaBanBuonChuaVat = 0, tyLeLaiLe = 0, tyLeLaiBuon = 0, giaVon = 0, giaMuaCoVat = 0;
                            string giaTriVatVao = "", giaTriVatRa = "";
                            //MAVATTU	TENVATTU	VATVAO	VATRA	NHACC	DONVITINH	GIAMUA	GIABANLE	GIABANBUON	MAMAU	MASIZE
                            MdMerchandise merchandise = new MdMerchandise();
                            MdMerchandisePrice merchandisePrice = new MdMerchandisePrice();
                            //mã vật tư
                            merchandise.MaVatTu = string.IsNullOrEmpty((string)(range.Cells[rCnt, cCnt] as Excel.Range).Value2) ? "" : (string)(range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                            if (!string.IsNullOrEmpty(merchandise.MaVatTu))
                            {
                                //check trùng mã 
                                var checkExist = _service.Repository.DbSet.Where(x => x.MaVatTu.Equals(merchandise.MaVatTu) && x.UnitCode.StartsWith(unitCode)).ToList();
                                if (checkExist.Count > 0)
                                {
                                    foreach (var exist in checkExist)
                                    {
                                        //bỏ qua
                                        message.Message1 = "Mã hàng " + exist.MaVatTu + " đã tồn tại trong danh mục !";
                                        flag = false;
                                    }
                                }
                                else
                                {
                                    //tên vật tư
                                    merchandise.TenHang =
                                        string.IsNullOrEmpty(
                                            (string)(range.Cells[rCnt, cCnt + 1] as Excel.Range).Value2)
                                            ? ""
                                            : (string)(range.Cells[rCnt, cCnt + 1] as Excel.Range).Value2.ToString();
                                    //mã vat vào
                                    merchandise.MaVatVao =
                                        string.IsNullOrEmpty(
                                            (string)(range.Cells[rCnt, cCnt + 2] as Excel.Range).Value2)
                                            ? ""
                                            : (string)(range.Cells[rCnt, cCnt + 2] as Excel.Range).Value2.ToString();
                                    if (!string.IsNullOrEmpty(merchandise.MaVatVao) && !string.IsNullOrEmpty(merchandise.MaVatRa))
                                    {
                                        giaTriVatVao =
                                       _service.UnitOfWork.Repository<MdTax>()
                                           .DbSet.FirstOrDefault(x => x.MaLoaiThue == merchandise.MaVatVao && x.UnitCode.StartsWith(unitCode))
                                           .TaxRate.ToString();
                                        if (string.IsNullOrEmpty(giaTriVatVao))
                                        {
                                            message.Message1 = "Mã VAT: " + "'" + merchandise.MaVatVao + "'" +
                                                               " chưa được khai báo";
                                        }
                                        //mã vat ra
                                        merchandise.MaVatRa =
                                            string.IsNullOrEmpty(
                                                (string)(range.Cells[rCnt, cCnt + 3] as Excel.Range).Value2)
                                                ? ""
                                                : (string)(range.Cells[rCnt, cCnt + 3] as Excel.Range).Value2.ToString();
                                        giaTriVatRa =
                                           _service.UnitOfWork.Repository<MdTax>()
                                               .DbSet.FirstOrDefault(x => x.MaLoaiThue == merchandise.MaVatRa && x.UnitCode.StartsWith(unitCode))
                                               .TaxRate.ToString();
                                        if (string.IsNullOrEmpty(giaTriVatVao))
                                        {
                                            message.Message2 = "Mã VAT: " + "'" + merchandise.MaVatRa + "'" +
                                                               " chưa được khai báo";
                                        }
                                    }
                                    merchandise.Barcode =
                                        (DateTime.Now.ToString("ddMMyy") + DateTime.Now.Minute + DateTime.Now.Second +
                                         DateTime.Now.Millisecond + DateTime.Now.Ticks).Substring(0, 13);
                                    //tỷ lệ vat vào
                                    decimal.TryParse(giaTriVatVao, out tyLeVatVao);
                                    merchandise.TyLeVatVao = tyLeVatVao;

                                    //tỷ lệ vat ra
                                    decimal.TryParse(giaTriVatRa, out tyLeVatRa);
                                    merchandise.TyLeVatRa = tyLeVatRa;
                                    //mã nhà cung cấp
                                    merchandise.MaNCC =
                                        string.IsNullOrEmpty(
                                            (string)(range.Cells[rCnt, cCnt + 4] as Excel.Range).Value2)
                                            ? ""
                                            : (string)(range.Cells[rCnt, cCnt + 4] as Excel.Range).Value2.ToString();
                                    merchandise.MaKhachHang = merchandise.MaNCC;
                                    //mã đơn vị tính
                                    merchandise.DonViTinh =
                                        string.IsNullOrEmpty(
                                            (string)(range.Cells[rCnt, cCnt + 5] as Excel.Range).Value2)
                                            ? ""
                                            : (string)(range.Cells[rCnt, cCnt + 5] as Excel.Range).Value2.ToString();
                                    //giá mua chưa vat
                                    string GiaMuaChuaVat = (range.Cells[rCnt, cCnt + 6] as Excel.Range).Value2.ToString();
                                    decimal.TryParse(GiaMuaChuaVat, out giaMuaChuaVat);
                                    //giá mua có vat
                                    decimal.TryParse((giaMuaChuaVat * (1 + tyLeVatVao / 100)).ToString(), out giaMuaCoVat);
                                    //giá bán lẻ có vat
                                    string GiaBanLeCoVat = (string)(range.Cells[rCnt, cCnt + 7] as Excel.Range).Value2.ToString();
                                    decimal.TryParse(GiaBanLeCoVat, out giaBanLeCoVat);
                                    //giá bán buôn
                                    string GiaBanBuonCoVat = (string)(range.Cells[rCnt, cCnt + 8] as Excel.Range).Value2.ToString();
                                    decimal.TryParse(GiaBanBuonCoVat, out giaBanBuonCoVat);
                                    //giá bán lẻ chưa vat
                                    decimal.TryParse((giaBanLeCoVat / (1 + tyLeVatRa / 100)).ToString(), out giaBanLeChuaVat);
                                    //giá bán buôn chưa vat
                                    decimal.TryParse((giaBanBuonCoVat / (1 + tyLeVatRa / 100)).ToString(),
                                        out giaBanBuonChuaVat);
                                    //tỷ lệ lãi lẻ
                                    if (giaMuaChuaVat != 0)
                                    {
                                        decimal.TryParse(
                                            (((giaBanLeChuaVat - giaMuaChuaVat) * 100) / giaMuaChuaVat).ToString(),
                                            out tyLeLaiLe);
                                        decimal.TryParse(
                                            (((giaBanBuonChuaVat - giaMuaChuaVat) * 100) / giaMuaChuaVat).ToString(),
                                            out tyLeLaiBuon);
                                    }
                                    else
                                    {
                                        tyLeLaiLe = 0;
                                        //tỷ lệ lãi buôn
                                        tyLeLaiBuon = 0;
                                    }
                                    merchandise.MaColor =
                                        string.IsNullOrEmpty(
                                            (string)(range.Cells[rCnt, cCnt + 9] as Excel.Range).Value2)
                                            ? ""
                                            : (string)(range.Cells[rCnt, cCnt + 9] as Excel.Range).Value2.ToString();
                                    merchandise.MaSize =
                                        string.IsNullOrEmpty(
                                            (string)(range.Cells[rCnt, cCnt + 10] as Excel.Range).Value2)
                                            ? ""
                                            : (string)(range.Cells[rCnt, cCnt + 10] as Excel.Range).Value2.ToString();
                                    //mã loại vật tư
                                    merchandise.MaLoaiVatTu =
                                        string.IsNullOrEmpty(
                                            (string)(range.Cells[rCnt, cCnt + 11] as Excel.Range).Value2)
                                            ? ""
                                            : (string)(range.Cells[rCnt, cCnt + 11] as Excel.Range).Value2.ToString();
                                    //mã nhóm vật tư
                                    merchandise.MaNhomVatTu =
                                        string.IsNullOrEmpty(
                                            (string)(range.Cells[rCnt, cCnt + 12] as Excel.Range).Value2)
                                            ? ""
                                            : (string)(range.Cells[rCnt, cCnt + 12] as Excel.Range).Value2.ToString();
                                    //
                                    merchandise.TrangThaiCon = 1;
                                    merchandise.TrangThai = 10;
                                    merchandise.UnitCode = unitCode;
                                    //giá
                                    merchandisePrice.MaVatTu = merchandise.MaVatTu;
                                    merchandisePrice.MaDonVi = unitCode;
                                    merchandisePrice.GiaBanBuon = giaBanBuonChuaVat;
                                    merchandisePrice.GiaBanBuonVat = giaBanBuonCoVat;
                                    merchandisePrice.GiaBanLe = giaBanLeChuaVat;
                                    merchandisePrice.GiaBanLeVat = giaBanLeCoVat;
                                    merchandisePrice.GiaMua = giaMuaChuaVat;
                                    merchandisePrice.GiaMuaVat = giaBanLeCoVat;
                                    merchandisePrice.MaVatRa = merchandise.MaVatRa;
                                    merchandisePrice.MaVatVao = merchandise.MaVatVao;
                                    merchandisePrice.TyLeVatVao = tyLeVatVao;
                                    merchandisePrice.TyLeVatRa = tyLeVatRa;
                                    merchandisePrice.GiaMuaVat = giaMuaCoVat;
                                    merchandisePrice.TyLeLaiBuon = tyLeLaiBuon;
                                    merchandisePrice.TyLeLaiLe = tyLeLaiLe;
                                    merchandisePrice.UnitCode = unitCode;

                                    //add Parent
                                    lstMerchandise.Add(merchandise);
                                    lstMerchandisePrice.Add(merchandisePrice);
                                    if (!string.IsNullOrEmpty(merchandise.MaColor) && !string.IsNullOrEmpty(merchandise.MaSize))
                                    {
                                        var arrayColor = merchandise.MaColor.Split(',');
                                        var arraySize = merchandise.MaSize.Split(',');
                                        List<MdMerchandise> children = ResultChildrenVatTu(merchandise.MaVatTu, merchandise.TenHang, arrayColor, arraySize, unitCode);
                                        if (children.Count > 0)
                                        {
                                            message.Message3 = "Khởi tạo " + children.Count + " mã hàng !";
                                            foreach (var dataChld in children)
                                            {
                                                MdMerchandisePrice merchandisePriceChildren = new MdMerchandisePrice();
                                                dataChld.TyLeVatVao = tyLeVatVao;
                                                dataChld.TyLeVatRa = tyLeVatRa;
                                                dataChld.MaNCC = merchandise.MaNCC;
                                                dataChld.MaKhachHang = merchandise.MaNCC;
                                                dataChld.DonViTinh = merchandise.DonViTinh;
                                                dataChld.GiaMua = giaMuaChuaVat;
                                                dataChld.GiaBanLe = giaBanLeCoVat;
                                                dataChld.GiaBanBuon = giaBanBuonCoVat;
                                                dataChld.TrangThaiCon = 0;
                                                dataChld.MaLoaiVatTu = merchandise.MaLoaiVatTu;
                                                dataChld.MaNhomVatTu = merchandise.MaNhomVatTu;
                                                dataChld.Barcode = (DateTime.Now.ToString("ddMMyy") + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Ticks).Substring(0, 13);
                                                dataChld.UnitCode = unitCode;
                                                //giá hàng list vật tư con
                                                merchandisePriceChildren.MaVatTu = dataChld.MaVatTu;
                                                merchandisePriceChildren.MaDonVi = unitCode;
                                                merchandisePriceChildren.GiaBanBuon = giaBanBuonChuaVat;
                                                merchandisePriceChildren.GiaBanBuonVat = giaBanBuonCoVat;
                                                merchandisePriceChildren.GiaBanLe = giaBanLeChuaVat;
                                                merchandisePriceChildren.GiaBanLeVat = giaBanLeCoVat;
                                                merchandisePriceChildren.GiaMua = giaMuaChuaVat;
                                                merchandisePriceChildren.GiaMuaVat = giaBanLeCoVat;
                                                merchandisePriceChildren.MaVatRa = merchandise.MaVatRa;
                                                merchandisePriceChildren.MaVatVao = merchandise.MaVatVao;
                                                merchandisePriceChildren.TyLeVatVao = tyLeVatVao;
                                                merchandisePriceChildren.TyLeVatRa = tyLeVatRa;
                                                merchandisePriceChildren.GiaMuaVat = giaMuaCoVat;
                                                merchandisePriceChildren.TyLeLaiBuon = tyLeLaiBuon;
                                                merchandisePriceChildren.TyLeLaiLe = tyLeLaiLe;
                                                merchandisePriceChildren.UnitCode = unitCode;
                                                lstMerchandise.Add(dataChld);
                                                lstMerchandisePrice.Add(merchandisePriceChildren);
                                            }
                                        }
                                    }
                                }
                            }
                            lstMessage.Add(message);
                        }
                        //insert database
                        if (lstMerchandise.Count > 0 && lstMerchandisePrice.Count > 0 && flag)
                        {
                            foreach (var record in lstMerchandise)
                            {
                                record.ObjectState = ObjectState.Added;
                                var isSave = _service.Insert(record, false);
                                _service.UnitOfWork.Save();
                                countInsertMerchandise++;
                            }
                            foreach (var recordPrice in lstMerchandisePrice)
                            {
                                recordPrice.ObjectState = ObjectState.Added;
                                var isSave = _servicePrice.Insert(recordPrice, false);
                                _servicePrice.UnitOfWork.Save();
                                countInsertMerchandisePrice++;
                            }
                            result.Data = lstMessage;
                            result.Status = true;
                            result.Message = "Import thành công";
                        }
                        else if (!flag)
                        {
                            result.Data = lstMessage;
                            result.Status = false;
                            result.Message = "Trùng mã";
                        }
                        xlWorkBook.Close(true, null, null);
                        xlApp.Quit();
                        Marshal.ReleaseComObject(xlWorkSheet);
                        Marshal.ReleaseComObject(xlWorkBook);
                        Marshal.ReleaseComObject(xlApp);
                    }
                }
            }
            catch (Exception e)
            {
                result.Message = "Xảy ra lỗi !" + e;
                result.Data = null;
                result.Status = false;
                return Ok(result);
            }
            return Ok(result);
        }
        //end
        [Route("ImportExcel")]
        public async Task<IHttpActionResult> ImportExcel()
        {
            try
            {
                var lst = new List<MdMerchandiseVm.Dto>();

                var httpRequest = HttpContext.Current.Request;
                var contents = new List<string>();
                string error = "";
                bool isUpdate = Convert.ToBoolean(httpRequest.Form["isUpdate"]);
                var response = new HttpResponseMessage();
                if (httpRequest.Files.Count < 1)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Content = new StringContent("Lỗi dữ liệu trống");
                    return ResponseMessage(response);
                }

                foreach (string file in httpRequest.Files)
                {

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        ExcelPackage excel = new ExcelPackage();
                        excel.Load(postedFile.InputStream);
                        var currentSheet = excel.Workbook.Worksheets[1];
                        var rows = currentSheet.Dimension.Rows;

                        if (rows < 1)
                        {
                            error = "Lỗi dữ liệu trống";
                            response.Content = new StringContent(error);
                            return ResponseMessage(response);
                        }


                        for (int i = 2; i <= rows; i++)
                        {
                            var obj = new MdMerchandiseVm.Dto();
                            obj.MaLoaiVatTu = currentSheet.Cells[i, 1].Value.ToString();
                            obj.MaNhomVatTu = currentSheet.Cells[i, 2].Value.ToString();
                            obj.TenVatTu = currentSheet.Cells[i, 3].Value.ToString();
                            obj.MaVatVao = currentSheet.Cells[i, 4].Value.ToString();
                            obj.MaVatRa = currentSheet.Cells[i, 5].Value.ToString();
                            obj.MaNCC = currentSheet.Cells[i, 6].Value.ToString();
                            obj.DonViTinh = currentSheet.Cells[i, 7].Value.ToString();
                            obj.GiaMua = Convert.ToDecimal(currentSheet.Cells[i, 8].Value);
                            obj.GiaBanLe = Convert.ToDecimal(currentSheet.Cells[i, 9].Value);
                            obj.GiaBanBuon = Convert.ToDecimal(currentSheet.Cells[i, 10].Value);
                            obj.MaColor = currentSheet.Cells[i, 11].Value.ToString();
                            obj.MaSize = currentSheet.Cells[i, 12].Value.ToString();
                            //if()
                            lst.Add(obj);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            return Ok();
        }
        [Route("ExportTemplateExcel")]
        public HttpResponseMessage ExportTemplateExcel()
        {
            HttpResponseMessage message = new HttpResponseMessage();
            MemoryStream data = new MemoryStream();
            try
            {
                var lstSize = _service.UnitOfWork.Repository<MdSize>().DbSet.Where(o => o.TrangThai == 10).Select(o => new { o.MaSize, o.TenSize });
                var lstMerChandiseType = _service.UnitOfWork.Repository<MdMerchandiseType>().DbSet.Where(o => o.TrangThai == 10).Select(o => new { o.MaLoaiVatTu, o.TenLoaiVatTu });
                var lstMerchandiseGroup = _service.UnitOfWork.Repository<MdNhomVatTu>().DbSet.Where(o => o.TrangThai == 10).Select(o => new { o.MaNhom, o.TenNhom, o.MaLoaiVatTu });
                var lstColor = _service.UnitOfWork.Repository<MdColor>().DbSet.Where(o => o.TrangThai == 10).Select(o => new { o.MaColor, o.TenColor });
                int row;
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    var excel = new MemoryStream();
                    excelPackage.Workbook.Worksheets.Add("DM_VATTU");
                    excelPackage.Workbook.Worksheets.Add("DM_LOAIVATTU");
                    excelPackage.Workbook.Worksheets.Add("DM_NHOMVATTU");
                    excelPackage.Workbook.Worksheets.Add("DM_SIZE");
                    excelPackage.Workbook.Worksheets.Add("DM_MAU");
                    var workSheet_VatTu = excelPackage.Workbook.Worksheets[1];
                    var workSheet_LoaiVatTu = excelPackage.Workbook.Worksheets[2];
                    var workSheet_NhomVatTu = excelPackage.Workbook.Worksheets[3];
                    var workSheet_Size = excelPackage.Workbook.Worksheets[4];
                    var workSheet_Mau = excelPackage.Workbook.Worksheets[5];

                    if (lstSize == null || lstColor == null || lstMerchandiseGroup == null || lstMerChandiseType == null)
                    {
                        message.StatusCode = HttpStatusCode.NotFound;
                        if (lstSize == null)
                        {
                            message.Content = new StringContent("Danh mục size rỗng");
                        }
                        if (lstColor == null)
                        {
                            message.Content = new StringContent("Danh mục màu rỗng");
                        }
                        if (lstMerchandiseGroup == null)
                        {
                            message.Content = new StringContent("Danh mục nhóm vật tư rỗng");
                        }
                        if (lstMerChandiseType == null)
                        {
                            message.Content = new StringContent("Danh mục loại vật tư rỗng");
                        }
                        return message;
                    }

                    if (lstSize != null)
                    {
                        row = 1;
                        workSheet_Size.Cells[row, 1].Value = "Mã Size";
                        workSheet_Size.Cells[row, 2].Value = "Tên Size";

                        foreach (var item in lstSize)
                        {
                            row++;
                            workSheet_Size.Cells[row, 1].Value = item.MaSize;
                            workSheet_Size.Cells[row, 2].Value = item.TenSize;
                        }
                    }

                    if (lstMerChandiseType != null)
                    {
                        row = 1;
                        workSheet_LoaiVatTu.Cells[row, 1].Value = "Mã Loại";
                        workSheet_LoaiVatTu.Cells[row, 2].Value = "Tên Loại";

                        foreach (var item in lstMerChandiseType)
                        {
                            row++;
                            workSheet_LoaiVatTu.Cells[row, 1].Value = item.MaLoaiVatTu;
                            workSheet_LoaiVatTu.Cells[row, 2].Value = item.TenLoaiVatTu;
                        }
                    }

                    if (lstMerchandiseGroup != null)
                    {
                        row = 1;
                        workSheet_NhomVatTu.Cells[row, 1].Value = "Mã Nhóm";
                        workSheet_NhomVatTu.Cells[row, 2].Value = "Tên Nhóm";
                        workSheet_NhomVatTu.Cells[row, 3].Value = "Mã Loại";

                        foreach (var item in lstMerchandiseGroup)
                        {
                            row++;
                            workSheet_NhomVatTu.Cells[row, 1].Value = item.MaNhom;
                            workSheet_NhomVatTu.Cells[row, 2].Value = item.TenNhom;
                            workSheet_NhomVatTu.Cells[row, 3].Value = item.MaLoaiVatTu;
                        }
                    }

                    if (lstColor != null)
                    {
                        row = 1;
                        workSheet_Mau.Cells[row, 1].Value = "Mã Màu";
                        workSheet_Mau.Cells[row, 2].Value = "Tên Màu";
                        foreach (var item in lstColor)
                        {
                            row++;
                            workSheet_Mau.Cells[row, 1].Value = item.MaColor;
                            workSheet_Mau.Cells[row, 2].Value = item.TenColor;
                        }
                    }

                    excelPackage.SaveAs(excel);
                    data = excel;
                }

                message.StatusCode = HttpStatusCode.OK;
                data.Seek(0, SeekOrigin.Begin);
                message.Content = new StreamContent(data);
                message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "TemplateDanhMucVatTu.xlsx" };
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return message;
            }
            catch (Exception e)
            {
                message.StatusCode = HttpStatusCode.InternalServerError;
                message.Content = new StringContent(e.Message);
                return message;
            }
        }

        [Route("ImportExcelPrintItem/{unitCode}")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult ImportExcelPrintItem(string unitCode)
        {
            var result = new TransferObj<List<MdMerchandiseVm.DataPrintItem>>();
            string path = _service.GetPhysicalPathImportFile();
            List<MdMerchandiseVm.DataPrintItem> lstResult = new List<MdMerchandiseVm.DataPrintItem>();
            //string path = @"C:/inetpub/wwwroot/wss/VirtualDirectories/8686/Upload/Barcode.xls";
            HttpRequest request = HttpContext.Current.Request;
            try
            {
                if (request.Files.Count > 0)
                {

                    HttpPostedFile file = request.Files[0];
                    file.SaveAs(path + file.FileName);
                    if (File.Exists(path + file.FileName))
                    {
                        try
                        {
                            //doc du lieu tu file
                            Excel.Application xlApp;
                            Excel.Workbook xlWorkBook;
                            Excel.Worksheet xlWorkSheet;
                            Excel.Range range;
                            int rw = 0, cl = 0;
                            xlApp = new Excel.Application();
                            string pathFile = path + file.FileName;
                            xlWorkBook = xlApp.Workbooks.Open(@"" + pathFile + "", 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                            range = xlWorkSheet.UsedRange;
                            rw = range.Rows.Count;
                            cl = range.Columns.Count;
                            Excel.Range last = xlWorkSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
                            int lastUsedRow = last.Row;
                            for (int rCnt = 2; rCnt <= lastUsedRow; rCnt++) //từ row 2 -- column 1
                            {
                                decimal Soluong = 0;
                                MdMerchandiseVm.DataPrintItem record = new MdMerchandiseVm.DataPrintItem();
                                int cCnt = 1; //bỏ qua ô số thứ tự
                                record.Masieuthi = string.IsNullOrEmpty((string)(range.Cells[rCnt, cCnt] as Excel.Range).Value2) ? "" : (string)(range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                                if (record.Masieuthi != null && record.Masieuthi.Length == 7)
                                {
                                    try
                                    {
                                        var mMerchandise = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu == record.Masieuthi && x.UnitCode.Equals(unitCode));
                                        if (mMerchandise != null)
                                        {
                                            record.Tenviettat = mMerchandise.TenHang;
                                            record.Makhachhang = mMerchandise.MaKhachHang;
                                            record.Barcode = mMerchandise.Barcode;
                                        }
                                        var mMerchandisePrice = _service.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.FirstOrDefault(x => x.MaVatTu == record.Masieuthi && x.MaDonVi.Equals(unitCode));
                                        if (mMerchandisePrice != null)
                                        {
                                            record.Giabanle = mMerchandisePrice.GiaBanLe;
                                            record.Giabanbuon = mMerchandisePrice.GiaBanBuon;
                                            record.Giabanbuoncovat = mMerchandisePrice.GiaBanBuonVat;
                                            record.Giabanlecovat = mMerchandisePrice.GiaBanLeVat;
                                        }
                                        //Tính giá trị khuyến mại
                                        string soLuongExcel = (range.Cells[rCnt, cCnt + 1] as Excel.Range).Value2.ToString();
                                        decimal.TryParse(soLuongExcel, out Soluong);
                                        record.Soluong = Soluong;
                                        lstResult.Add(record);
                                    }
                                    catch
                                    {
                                        return BadRequest("Định dạng file Excel không đúng");
                                    }
                                }
                            }
                            if (lstResult.Count > 0)
                            {
                                result.Status = true;
                                result.Data = lstResult;
                                result.Message = "Đọc dữ liệu thành công !";
                            }
                            else
                            {
                                result.Status = false;
                                result.Data = null;
                                result.Message = "Không đọc được dữ liệu";
                            }
                        }
                        catch
                        {
                            result.Message = "Cấu hình đọc dữ liệu Excel không đúng";
                            result.Data = null;
                            result.Status = false;
                            return Ok(result);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                result.Message = "Xảy ra lỗi !" + e;
                result.Data = null;
                result.Status = false;
                return Ok(result);
            }
            return Ok(result);
        }

        [Route("GetForNvNhapMuaByCode/{code}/{supplierCode}/{unitCode}")]
        public IHttpActionResult GetForNvNhapMuaByCode(string code, string supplierCode, string unitCode)
        {
            TransferObj<MdMerchandiseVm.Dto> result = new TransferObj<MdMerchandiseVm.Dto>();
            string strQuery = @"SELECT * FROM V_VATTU_GIABAN WHERE lower(MAHANG) = lower('" + code + "') AND MAKHACHHANG = '" + supplierCode + "' AND MADONVI = '" + unitCode + "'";
            var data = new ERPContext().Database.SqlQuery<MdMerchandiseVm.Dto>(strQuery).ToList();
            if (data != null && data.Count > 0)
            {
                result.Data = data[0];
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return Ok(result);
        }

        [Route("GetForNvNhapMua/{supplierCode}/{unitCode}/{summary?}")]
        public IHttpActionResult GetForNvNhapMua(string supplierCode, string unitCode, string summary = null)
        {
            TransferObj<List<MdMerchandiseVm.Dto>> result = new TransferObj<List<MdMerchandiseVm.Dto>>();
            string strQuery = @"SELECT * FROM V_VATTU_GIABAN WHERE MAKHACHHANG = '" + supplierCode + "' AND MADONVI = '" + unitCode + "'";
            if (!string.IsNullOrEmpty(summary))
            {
                strQuery += " AND (LOWER(MAHANG) LIKE '%" + summary.ToLower() + "%' OR LOWER(BARCODE) LIKE '%" + summary.ToLower() + "%' OR LOWER(TENHANG) LIKE '%" + summary.ToLower() + "%')";
            }
            var data = new ERPContext().Database.SqlQuery<MdMerchandiseVm.Dto>(strQuery).ToList();
            if (data != null && data.Count > 0)
            {
                result.Data = data;
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return Ok(result);
        }
    }
}
