using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{

    public interface IMdCustomerService : IDataInfoService<MdCustomer>
    {
        string BuildCode();
        string SaveCode();
        string BuildCodeRoot();
        string SaveCodeRoot();
        MdCustomer CreateNewInstance();
        MdCustomer UpdateCustomerAfterPurchase(decimal tienNguyenGia, decimal tienSale, decimal tongTien, decimal soDiem, int quenThe, string idCustomer);
        MdCustomer TakeCareOfCustomer(MdCustomer instance);
        List<MdCustomerVm.Dto> TIMKIEM_KHACHHANG_FROM_ORACLE(string P_KEYSEARCH, int P_USE_TIMKIEM_ALL, int P_DIEUKIEN_TIMKIEM, string UNITCODE);
        //Add function here
    }
    public class MdCustomerService : DataInfoServiceBase<MdCustomer>, IMdCustomerService
    {
        public MdCustomerService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdCustomer, bool>> GetKeyFilter(MdCustomer instance)
        {
            var maDonViCha = GetParentUnitCode();
            return x => x.MaKH == instance.MaKH && x.UnitCode.StartsWith(maDonViCha);
        }

        public List<MdCustomerVm.Dto> TIMKIEM_KHACHHANG_FROM_ORACLE(string P_KEYSEARCH, int P_USE_TIMKIEM_ALL, int P_DIEUKIEN_TIMKIEM, string UNITCODE)
        {
            List<MdCustomerVm.Dto> result = new List<MdCustomerVm.Dto>();
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
                        cmd.CommandText = "TIMKIEM_KHACHHANG";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_KEYSEARCH", OracleDbType.Varchar2).Value = P_KEYSEARCH;
                        cmd.Parameters.Add("P_UNITCODE", OracleDbType.Varchar2).Value = UNITCODE;
                        cmd.Parameters.Add("P_USE_TIMKIEM_ALL", OracleDbType.Int32).Value = P_USE_TIMKIEM_ALL;
                        cmd.Parameters.Add("P_DIEUKIEN_TIMKIEM", OracleDbType.Int32).Value = P_DIEUKIEN_TIMKIEM;
                        cmd.Parameters.Add("CURSOR_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                MdCustomerVm.Dto dto = new MdCustomerVm.Dto();
                                decimal SODIEM = 0;
                                decimal TONGTIEN = 0;
                                dto.MaKH = dataReader["MAKH"] != null ? dataReader["MAKH"].ToString().Trim() : "";
                                dto.TenKH = dataReader["TENKH"] != null ? dataReader["TENKH"].ToString().Trim() : "";
                                dto.DiaChi = dataReader["DIACHI"] != null ? dataReader["DIACHI"].ToString().Trim() : "";
                                dto.DienThoai = dataReader["DIENTHOAI"] != null ? dataReader["DIENTHOAI"].ToString().Trim() : "";
                                dto.ChungMinhThu = dataReader["CMTND"] != null ? dataReader["CMTND"].ToString().Trim() : "";
                                dto.Email = dataReader["EMAIL"] != null ? dataReader["EMAIL"].ToString().Trim() : "";
                                if (dataReader["SODIEM"] != null)
                                {
                                    decimal.TryParse(dataReader["SODIEM"].ToString(), out SODIEM);
                                }
                                dto.SoDiem = SODIEM;
                                if (dataReader["TONGTIEN"] != null)
                                {
                                    decimal.TryParse(dataReader["TONGTIEN"].ToString(), out TONGTIEN);
                                }
                                dto.TongTien = TONGTIEN;
                                if (dataReader["NGAYCAPTHE"] != null)
                                {
                                    DateTime? NGAYCAPTHE = string.IsNullOrEmpty(dataReader["NGAYCAPTHE"].ToString()) ? (DateTime?)null : DateTime.Parse(dataReader["NGAYCAPTHE"].ToString());
                                    dto.NgayCapThe = NGAYCAPTHE;
                                }
                                if (dataReader["NGAYHETHAN"] != null)
                                {
                                    DateTime? NGAYHETHAN = string.IsNullOrEmpty(dataReader["NGAYHETHAN"].ToString()) ? (DateTime?)null : DateTime.Parse(dataReader["NGAYHETHAN"].ToString());
                                    dto.NgayHetHan = NGAYHETHAN;
                                }
                                if (dataReader["NGAYSINH"] != null)
                                {
                                    DateTime? NGAYSINH = string.IsNullOrEmpty(dataReader["NGAYSINH"].ToString()) ? (DateTime?)null : DateTime.Parse(dataReader["NGAYSINH"].ToString());
                                    dto.NgaySinh = NGAYSINH;
                                }
                                dto.HangKhachHang = dataReader["HANGKHACHHANG"] != null ? dataReader["HANGKHACHHANG"].ToString().Trim() : "";
                                dto.HangKhachHangCu = dataReader["HANGKHACHHANGCU"] != null ? dataReader["HANGKHACHHANGCU"].ToString().Trim() : "";
                                result.Add(dto);
                            }
                        }
                    }
                }
                catch
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

        public string BuildCodeRoot()
        {
            var result = "";
            var type = TypeMasterData.KH.ToString();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            string maDonViCha = GetParentUnitCode();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["LTT.Connection"].ConnectionString))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        MdIdBuilder config = new MdIdBuilder();
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = connection;
                        cmd.InitialLONGFetchSize = 1000;
                        cmd.CommandText = "SELECT ID,TYPE,CODE,\"CURRENT\",UNITCODE FROM MD_ID_BUILDER WHERE TYPE = '" + type + "' AND CODE = '" + type + "' AND UNITCODE = '" + maDonViCha + "' AND ROWNUM = 1";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                config = new MdIdBuilder
                                {
                                    Id = dataReader["ID"].ToString(),
                                    Type = type,
                                    Code = type,
                                    Current = dataReader["CURRENT"].ToString(),
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
                                Current = "0",
                                UnitCode = maDonViCha,
                            };
                        }
                        var soMa = config.GenerateNumber();
                        config.Current = soMa;
                        result = string.Format("{0}{1}", config.Code, soMa);
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
            var type = TypeMasterData.KH.ToString();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            string maDonViCha = GetParentUnitCode();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["LTT.Connection"].ConnectionString))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        MdIdBuilder config = new MdIdBuilder();
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = connection;
                        cmd.InitialLONGFetchSize = 1000;
                        cmd.CommandText = "SELECT ID,TYPE,CODE,\"CURRENT\",UNITCODE FROM MD_ID_BUILDER WHERE TYPE = '" + type + "' AND CODE = '" + type + "' AND UNITCODE = '" + maDonViCha + "' AND ROWNUM = 1";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                config = new MdIdBuilder
                                {
                                    Id = dataReader["ID"].ToString(),
                                    Type = type,
                                    Code = type,
                                    Current = dataReader["CURRENT"].ToString(),
                                    UnitCode = maDonViCha
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
                                Current = "0",
                                UnitCode = maDonViCha
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

        public MdCustomer CreateNewInstance()
        {
            return new MdCustomer()
            {
                MaKH = BuildCode()
            };
        }

        public string BuildCode()
        {
            var maDonViCha = GetParentUnitCode();
            var unitCode = GetCurrentUnitCode();
            var type = TypeMasterData.KH.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode.StartsWith(maDonViCha)).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    UnitCode = maDonViCha,
                    Current = "0000",
                };
            }
            var soMa = config.GenerateNumber();
            config.Current = soMa;
            result = string.Format("{0}{1}", config.Code, soMa);

            return result;
        }

        public string SaveCode()
        {
            var maDonViCha = GetParentUnitCode();
            var type = TypeMasterData.KH.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode.StartsWith(maDonViCha)).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    UnitCode = maDonViCha,
                    Current = "0000",
                };
                result = config.GenerateNumber();
                config.Current = result;
                idRepo.Insert(config);
            }
            else
            {
                result = config.GenerateNumber();
                config.Current = result;
                config.ObjectState = ObjectState.Modified;
            }
            result = string.Format("{0}{1}", config.Code, config.Current);
            return result;

        }
        public MdCustomer TakeCareOfCustomer(MdCustomer instance)
        {
            try
            {
                var existItem = FindById(instance.Id);
                existItem.IsCare = true;
                existItem.TongTien_ChamSoc = existItem.TongTien;
                existItem.TienNguyenGia_ChamSoc = existItem.TienNguyenGia;
                existItem.NgayChamSoc = DateTime.Now;
                var result = Update(existItem);
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public MdCustomer UpdateCustomerAfterPurchase(decimal tienNguyenGia, decimal tienSale, decimal tongTien, decimal soDiem, int quenThe, string idCustomer)
        {
            MdCustomer result = new MdCustomer();
            decimal exsTienNguyenGia = 0, exsTienSale = 0, exsTongTien = 0, exsSoDiem = 0;
            //var exsitItem = FindById(idCustomer);
            var _parentUnitCode = GetParentUnitCode();
            var exsitItem = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.Id == idCustomer && x.UnitCode.StartsWith(_parentUnitCode));
            decimal.TryParse(exsitItem.TienNguyenGia.ToString(), result: out exsTienNguyenGia);
            decimal.TryParse(exsitItem.TienSale.ToString(), result: out exsTienSale);
            decimal.TryParse(exsitItem.TongTien.ToString(), result: out exsTongTien);
            decimal.TryParse(exsitItem.SoDiem.ToString(), result: out exsSoDiem);
            exsitItem.TienNguyenGia = exsTienNguyenGia + tienNguyenGia;
            exsitItem.TienSale = exsTienSale + tienSale;
            exsitItem.TongTien = exsTongTien + tongTien;
            exsitItem.SoDiem = exsSoDiem + soDiem;
            exsitItem.NgayMuaHang = DateTime.Now;
            exsitItem.QuenThe = quenThe;
            var item = UnitOfWork.Repository<MdHangKH>().DbSet.Where(x => x.SoTien <= exsitItem.SoDiem && x.UnitCode.StartsWith(_parentUnitCode)).OrderByDescending(x => x.SoTien).ToList();
            if (item.Count > 0)
            {
                var rank = item[0];
                if (rank.MaHangKh != exsitItem.HangKhachHang)
                {
                    exsitItem.HangKhachHangCu = exsitItem.HangKhachHang;
                    exsitItem.HangKhachHang = rank.MaHangKh;
                    exsitItem.NgayCapThe = DateTime.Now.Date;
                }
            }
            if (exsitItem != null)
            {
                result = Update(exsitItem);
            }
            return result;
        }
    }
}
