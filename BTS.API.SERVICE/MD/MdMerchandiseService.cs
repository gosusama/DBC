using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Web;
using System.Web.Configuration;
using BTS.API.ASYNC.DatabaseContext;

namespace BTS.API.SERVICE.MD
{

    public interface IMdMerchandiseService : IDataInfoService<MdMerchandise>
    {
        MdMerchandise InsertDto(MdMerchandiseVm.MasterDto instance);
        MdMerchandise UpdateDto(MdMerchandiseVm.MasterDto instance);
        MdMerchandise InsertFromSql(MdMerchandise instance);
        string BuildCode(string code);
        string SaveCode(string code);
        string BuildCodeChild(string code);
        string SaveCodeChild(string code);
        string BuildCodeCanDienTuFromSQL();
        string SaveCodeCanDienTuFromSQL(DBCSQL context);
        string BuildCodeCanDienTu();
        string SaveCodeCanDienTu();
        string CheckBarcodeAtSQL(string barcodes);
        MemoryStream ExportExcel(List<MdMerchandiseVm.Dto> pi);
        bool ModifieldCodeMerchandise(string maVatTu, string id, string maNhomVatTu);
        string UploadImage(bool isAvatar);
        List<MdMerchandiseVm.MasterDto> GetAllDataChild(string maVatTu);
        MdMerchandise InsertChild(MdMerchandiseVm.MasterDto instance);
        //Add function here
    }
    public class MdMerchandiseService : DataInfoServiceBase<MdMerchandise>, IMdMerchandiseService
    {

        public MdMerchandiseService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
      
        private string GetPhysicalPathImage()
        {
            return WebConfigurationManager.AppSettings["rootPhysical"] + "\\Upload\\MerchandiseImage\\";
        }
        protected override Expression<Func<MdMerchandise, bool>> GetKeyFilter(MdMerchandise instance)
        {
            var maDonViCha = GetParentUnitCode();
            var unitCode = GetCurrentUnitCode();
            return x => x.MaVatTu == instance.MaVatTu && x.UnitCode.StartsWith(maDonViCha);
        }

        public bool ModifieldCodeMerchandise(string maVatTu,string id, string maNhomVatTu)
        {
            bool result = false;
            var data = ProcedureCollection.UpdateCodeGroup(id, maVatTu, maNhomVatTu);
            if (data == true)
            {
                result = true;
            }
            return result;
        }

        public MemoryStream ExportExcel(List<MdMerchandiseVm.Dto> pi)
        {
            var ms = new MemoryStream();
            if (pi != null)
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    package.Workbook.Worksheets.Add("Data");
                    var worksheet = package.Workbook.Worksheets[1];
                    int startRow = 3;
                    int startColumn = 1;

                    worksheet.Cells[1, 1, 1, 16].Merge = true;
                    worksheet.Cells[1, 1].Value = "Danh sách hàng hóa";
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 1].Value = string.Format("Ngày tạo: {0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[2, 1, 2, 16].Merge = true;
                    worksheet.Cells[3, 1].Value = "STT";
                    worksheet.Cells[3, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 2].Value = "Mã vật tư";
                    worksheet.Cells[3, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 3].Value = "Tên vật tư";
                    worksheet.Cells[3, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 4].Value = "Barcode";
                    worksheet.Cells[3, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 5].Value = "Mã kệ";
                    worksheet.Cells[3, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 6].Value = "Mã loại";
                    worksheet.Cells[3, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 7].Value = "Mã nhóm";
                    worksheet.Cells[3, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 8].Value = "Nhà cung cấp";
                    worksheet.Cells[3, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 9].Value = "Giá mua";
                    worksheet.Cells[3, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 10].Value = "Giá mua VAT";
                    worksheet.Cells[3, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 11].Value = "Giá bán lẻ VAT";
                    worksheet.Cells[3, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 12].Value = "Giá bán buôn";
                    worksheet.Cells[3, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 13].Value = "Tỷ lệ VAT ra";
                    worksheet.Cells[3, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 14].Value = "Tỷ lệ VAT vào";
                    worksheet.Cells[3, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[3, 15].Value = "Đơn vị tính";
                    worksheet.Cells[3, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    int currentRow = startRow;
                    int stt = 0;

                    foreach (var item in pi)
                    {
                        currentRow++;
                        ++stt;
                        worksheet.Cells[currentRow, startColumn].Value = stt.ToString();
                        worksheet.Cells[currentRow, startColumn + 1].Value = item.MaVatTu;
                        worksheet.Cells[currentRow, startColumn + 2].Value = item.TenVatTu;
                        worksheet.Cells[currentRow, startColumn + 3].Value = item.Barcode;
                        worksheet.Cells[currentRow, startColumn + 4].Value = item.MaKeHang;
                        worksheet.Cells[currentRow, startColumn + 5].Value = item.MaLoaiVatTu;
                        worksheet.Cells[currentRow, startColumn + 6].Value = item.MaNhomVatTu;
                        worksheet.Cells[currentRow, startColumn + 7].Value = item.MaKhachHang;
                        worksheet.Cells[currentRow, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 8].Value = item.GiaMua;
                        worksheet.Cells[currentRow, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 9].Value = item.GiaMuaVat;
                        worksheet.Cells[currentRow, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 10].Value = item.GiaBanLeVat;
                        worksheet.Cells[currentRow, 11].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 11].Value = item.GiaBanBuon;
                        worksheet.Cells[currentRow, 12].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 12].Value = item.TyLeVatRa;
                        worksheet.Cells[currentRow, 13].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[currentRow, startColumn + 13].Value = item.TyLeVatVao;
                        worksheet.Cells[currentRow, startColumn + 14].Value = item.DonViTinh;
                        worksheet.Cells[currentRow, 1, currentRow, startColumn + 14].Style.Border.BorderAround(ExcelBorderStyle.Dotted);

                    }
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
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalCols = worksheet.Dimension.End.Column;
                    var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                    var dataFont = dataCells.Style.Font;
                    dataFont.SetFromFont(new System.Drawing.Font("Times New Roman", 13));
                    package.SaveAs(ms);
                    return ms;
                }
            }
            else
            {
                return null;
            }
        }

        public MdMerchandise InsertDto(MdMerchandiseVm.MasterDto instance)
        {
            if (instance.UseGenCode) instance.MaVatTu = SaveCode(instance.MaLoaiVatTu);
            var strBarcode = isExistBarCode(instance.Barcode);
            if (!string.IsNullOrEmpty(strBarcode))
            {
                throw new Exception("Barcode trùng:" + strBarcode);
            }
            var mer = UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu == instance.MaCha);
            if (mer != null)
            {
                mer.TrangThaiCon = 10;
                mer.ObjectState = ObjectState.Modified;
            }
            if (!string.IsNullOrEmpty(instance.ItemCode))
            {
                instance.ItemCode = SaveCodeCanDienTu();
            }
            instance.MaNCC = instance.MaKhachHang;
            instance.TenHang = instance.TenVatTu;
            instance.MaKhac = instance.MaVatTu;
            var masterData = Mapper.Map<MdMerchandiseVm.MasterDto, MdMerchandise>(instance);
            MdMerchandisePrice detailData = new MdMerchandisePrice();
            detailData.MaVatTu = instance.MaVatTu;
            detailData.MaDonVi = GetCurrentUnitCode();
            detailData.GiaBanBuon = instance.GiaBanBuon;
            detailData.GiaBanBuonVat = instance.GiaBanBuonVat;
            detailData.GiaBanLe = instance.GiaBanLe;
            detailData.GiaBanLeVat = instance.GiaBanLeVat;
            detailData.GiaMua = instance.GiaMua;
            detailData.GiaMuaVat = instance.GiaMuaVat;
            detailData.GiaVon = instance.GiaVon;
            detailData.TyLeLaiBuon = instance.TyLeLaiBuon;
            detailData.TyLeVatRa = instance.TyLeVatRa;
            detailData.TyLeVatVao = instance.TyLeVatVao;
            detailData.TyLeLaiLe = instance.TyLeLaiLe;
            detailData.SoTonMax = 0;
            detailData.SoTonMin = 0;
            detailData.UnitCode = GetCurrentUnitCode();
            detailData.MaVatRa = instance.MaVatRa;
            detailData.MaVatVao = instance.MaVatVao;
            detailData.IState = "50";
            detailData.Id = Guid.NewGuid().ToString();
            detailData.ICreateBy = GetClaimsPrincipal().Identity.Name;
            detailData.ICreateDate = DateTime.Now;
            detailData.IUpdateBy = GetClaimsPrincipal().Identity.Name;
            detailData.IUpdateDate = DateTime.Now;
            string path = GetPhysicalPathImage() + masterData.MaVatTu + "\\";
            masterData.PathImage = "/Upload/MerchandiseImage/"+ masterData.MaVatTu + "/";
            if (!string.IsNullOrEmpty(instance.AvatarName))
            {
                FileStream fs = new FileStream(path + instance.AvatarName, FileMode.Open, FileAccess.Read);
                byte[] ImageData = new byte[fs.Length];
                fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();
                masterData.Avatar = ImageData;
            }
            masterData.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
            masterData.ICreateBy = GetClaimsPrincipal().Identity.Name;
            masterData.ICreateDate = DateTime.Now;
            var result = Insert(masterData);
            UnitOfWork.Repository<MdMerchandisePrice>().Insert(detailData);
            return result;
        }
        public MdMerchandise InsertChild(MdMerchandiseVm.MasterDto instance)
        {
            if(instance.UseGenCode) instance.MaVatTu = SaveCodeChild(instance.MaCha);
            var strBarcode = isExistBarCode(instance.Barcode);
            if (!string.IsNullOrEmpty(strBarcode))
            {
                throw new Exception("Barcode trùng:" + strBarcode);
            }
            if (!string.IsNullOrEmpty(instance.ItemCode))
            {
                instance.ItemCode = SaveCodeCanDienTu();
            }
            var masterData = Mapper.Map<MdMerchandiseVm.MasterDto, MdMerchandise>(instance);
            var detailData = Mapper.Map<List<MdMerchandiseVm.DtoDetail>, List<MdMerchandisePrice>>(instance.DataDetails);
            if (detailData.Count == 0)
            {
                detailData.Add(new MdMerchandisePrice()
                {
                    GiaBanBuon = 0,
                    GiaBanBuonVat = 0,
                    GiaBanLe = 0,
                    GiaBanLeVat = 0,
                    GiaMua = 0,
                    GiaMuaVat = 0,
                    GiaVon = 0,
                    TyLeLaiBuon = 0,
                    TyLeVatRa = 0,
                    TyLeVatVao = 0,
                    TyLeLaiLe = 0,
                    SoTonMax = 0,
                    SoTonMin = 0,

                });
            }
            if (detailData.GroupBy(x => x.MaDonVi).Any(x => x.Count() > 1))
                throw new Exception("Không thể tồn tại nhiều giá trên cùng đơn vị!");

            detailData.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaVatTu = masterData.MaVatTu;
                x.MaDonVi = GetCurrentUnitCode();
            });

            string path = GetPhysicalPathImage() + masterData.MaVatTu + "\\";
            masterData.PathImage = "/Upload/MerchandiseImage/" + masterData.MaVatTu + "/";
            if (!string.IsNullOrEmpty(instance.AvatarName))
            {
                FileStream fs = new FileStream(path + instance.AvatarName, FileMode.Open, FileAccess.Read);
                byte[] ImageData = new byte[fs.Length];
                fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();
                masterData.Avatar = ImageData;
            }
            masterData.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
            var result = Insert(masterData);
            UnitOfWork.Repository<MdMerchandisePrice>().InsertRange(detailData);
            return result;
        }
        public string UploadImage(bool isAvatar)
        {
            string result = string.Empty; ;
            try
            {
                var ctx = new ERPContext();
                string path = GetPhysicalPathImage();

                HttpRequest request = HttpContext.Current.Request;
                var maVatTu = request.Form["maVatTu"];
                path += maVatTu + "\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                try
                {
                    if (isAvatar)
                    {
                        HttpPostedFile file = request.Files[0];
                        List<string> tmp = file.FileName.Split('.').ToList();
                        string extension = tmp.Count > 0 ? tmp[1] : "jpg";
                        string fileName = string.Format("{0}.{1}", maVatTu, extension);
                        file.SaveAs(path + fileName);
                        result = fileName;
                    }
                    else
                    {
                        for (int i = 0; i < request.Files.Count; i++)
                        {
                            HttpPostedFile file = request.Files[i];
                            List<string> tmp = file.FileName.Split('.').ToList();
                            string extension = tmp.Count > 0 ? tmp[1] : "jpg";
                            //file.ContentType
                            string fileName = string.Format("{0}_{1}{2}{3}.{4}", maVatTu, DateTime.Now.Minute, DateTime.Now.Second,
                                                                                            DateTime.Now.Millisecond, extension);
                            file.SaveAs(path + fileName);
                            result += fileName + ",";
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public MdMerchandise UpdateDto(MdMerchandiseVm.MasterDto instance)
        {
            string unitCode = GetCurrentUnitCode();
            MdMerchandise existItem = FindById(instance.Id);
            MdMerchandise masterData = Mapper.Map<MdMerchandiseVm.MasterDto, MdMerchandise>(instance);
            List<MdMerchandisePrice> detailData = Mapper.Map<List<MdMerchandiseVm.DtoDetail>, List<MdMerchandisePrice>>(instance.DataDetails);
            {
                List<MdMerchandisePrice> detail = UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu == masterData.MaVatTu && x.MaDonVi == unitCode).ToList();
                detail.ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            {
                if (detailData.GroupBy(x => x.MaDonVi).Any(x => x.Count() > 1))
                    throw new Exception("Không thể tồn tại nhiều giá trên cùng đơn vị!");
            }
            {
                detailData.ForEach(x =>
                {
                    x.MaVatTu = masterData.MaVatTu;
                    x.Id = Guid.NewGuid().ToString();
                });
            }
            masterData.PathImage = "/Upload/MerchandiseImage/" + masterData.MaVatTu + "/";
            string path = GetPhysicalPathImage() + masterData.MaVatTu + "\\";
            if (!string.IsNullOrEmpty(instance.AvatarName))
            {
                FileStream fs = new FileStream(path + instance.AvatarName, FileMode.Open, FileAccess.Read);
                byte[] ImageData = new byte[fs.Length];
                fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();
                masterData.Avatar = ImageData;
            }
            masterData.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
            masterData.ICreateBy = existItem.ICreateBy;
            masterData.ICreateDate = existItem.ICreateDate;
            masterData.IUpdateBy = GetClaimsPrincipal().Identity.Name;
            masterData.IUpdateDate = DateTime.Now;
            MdMerchandise result = Update(masterData);
            UnitOfWork.Repository<MdMerchandisePrice>().InsertRange(detailData);
            return result;
        }
        public MdMerchandise InsertFromSql(MdMerchandise instance)
        {
            var strBarcode = isExistBarCode(instance.Barcode);
            if (!string.IsNullOrEmpty(strBarcode))
            {
                throw new Exception("Barcode trùng:"+ strBarcode);
            }
            return Insert(instance);
        }
        public string BuildCodeCanDienTu()
        {
            var type = TypeMasterData.CANDIENTU.ToString();
            var result = "";

            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    Current = "00000",
                };
            }
            var soMa = config.GenerateNumber();
            config.Current = soMa;
            result = string.Format("{0}", soMa);
            return result;
        }
        public string SaveCodeCanDienTu()
        {
            var type = TypeMasterData.CANDIENTU.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    Current = "00000",
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
            result = string.Format("{0}", result);
            return result;
        }
       
        public string BuildCodeCanDienTuFromSQL()
        {
            var type = TypeMasterData.CANDIENTU.ToString();
            var result = "";
            using (var ctx = new DBCSQL())
            {
                var dbSet = ctx.TDS_Dmcapma;
                var instanceBuilder = dbSet.FirstOrDefault(x => x.Loaima == type);
                if (instanceBuilder != null)
                {
                    result = string.Format("{0}{1}", instanceBuilder.Mastart, instanceBuilder.GenerateNumber());
                }
            }

            return result;
        }
        public string SaveCodeCanDienTuFromSQL(DBCSQL context)
        {
            var type = TypeMasterData.CANDIENTU.ToString();
            var result = "";

                var dbSet = context.TDS_Dmcapma;
                var instanceBuilder = dbSet.FirstOrDefault(x => x.Loaima == type);
                if (instanceBuilder != null)
                {
                    var newNumber = instanceBuilder.GenerateNumber();
                    result = string.Format("{0}{1}", instanceBuilder.Mastart, newNumber);
                    instanceBuilder.Macappk = newNumber;
                }
            return result;
        }
        public string BuildCode(string code)
        {
            var type = TypeMasterData.VATTU.ToString();
            var maDonViCha = GetParentUnitCode();
            var result = "";
            var loaiVatTu = UnitOfWork.Repository<MdMerchandiseType>().DbSet.First(x => x.MaLoaiVatTu == code && x.UnitCode.StartsWith(maDonViCha));
            if (loaiVatTu != null)
            {

                var idRepo = UnitOfWork.Repository<MdIdBuilder>();
                var config = idRepo.DbSet.Where(x => x.Type == type && x.Code == loaiVatTu.MaLoaiVatTu && x.UnitCode == maDonViCha).FirstOrDefault();
                if (config == null)
                {
                    config = new MdIdBuilder
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = type,
                        Code = loaiVatTu.MaLoaiVatTu,
                        Current = "000000",
                        UnitCode = maDonViCha,
                    };
                }
                var soMa = config.GenerateNumber();
                config.Current = soMa;
                result = string.Format("{0}{1}", config.Code, soMa);
            }
            return result;
        }

        public string SaveCode(string code)
        {
            var type = TypeMasterData.VATTU.ToString();
            var maDonViCha = GetParentUnitCode();
            var result = "";
            var loaiVatTu = UnitOfWork.Repository<MdMerchandiseType>().DbSet.First(x => x.MaLoaiVatTu == code && x.UnitCode.StartsWith(maDonViCha));
            if (loaiVatTu != null)
            {

                var idRepo = UnitOfWork.Repository<MdIdBuilder>();
                var config = idRepo.DbSet.Where(x => x.Type == type && x.Code == loaiVatTu.MaLoaiVatTu && x.UnitCode == maDonViCha).FirstOrDefault();
                if (config == null)
                {
                    config = new MdIdBuilder
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = type,
                        Code = loaiVatTu.MaLoaiVatTu,
                        Current = "000000",
                        UnitCode = maDonViCha,
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

                result = string.Format("{0}{1}", config.Code, result);
            }
            return result;
        }

        public string BuildCodeChild(string code)
        {
            var type = TypeMasterData.VATTU.ToString();
            var result = "";
            var vatTu = UnitOfWork.Repository<MdMerchandise>().DbSet.First(x => x.MaVatTu == code);
            if (vatTu != null)
            {

                var idRepo = UnitOfWork.Repository<MdIdBuilder>();
                var config = idRepo.DbSet.Where(x => x.Type == type && x.Code == vatTu.MaVatTu).FirstOrDefault();
                if (config == null)
                {
                    config = new MdIdBuilder
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = type,
                        Code = vatTu.MaVatTu,
                        Current = "00",
                    };
                }
                var soMa = config.GenerateNumber();
                config.Current = soMa;
                result = string.Format("{0}{1}", config.Code, soMa);
            }
            return result;
        }
        public string SaveCodeChild(string code)
        {
            var type = TypeMasterData.VATTU.ToString();
            var result = "";
            var vatTu = UnitOfWork.Repository<MdMerchandise>().DbSet.First(x => x.MaVatTu == code);
            if (vatTu != null)
            {

                var idRepo = UnitOfWork.Repository<MdIdBuilder>();
                var config = idRepo.DbSet.Where(x => x.Type == type && x.Code == vatTu.MaVatTu).FirstOrDefault();
                if (config == null)
                {
                    config = new MdIdBuilder
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = type,
                        Code = vatTu.MaVatTu,
                        Current = "00",
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

                result = string.Format("{0}{1}", config.Code, result);
            }
            return result;
        }
        private string isExistBarCode(string barcodes)
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
                            if (data.Count() > 0 )
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
        private bool isExistMaKhac(string makhac)
        {
            bool ok = false;
            var hangHoas = UnitOfWork.Repository<MdMerchandise>().DbSet;
            if (!string.IsNullOrWhiteSpace(makhac))
            {
                var maKhacCollection = makhac.Split(';');
                maKhacCollection = maKhacCollection.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                foreach (var code in maKhacCollection)
                {
                    foreach (var item in hangHoas)
                    {

                        if (!string.IsNullOrWhiteSpace(item.MaKhac))
                        {
                            var mkCollection = item.MaKhac.Split(';');
                            if (mkCollection.Contains(code)) return true;
                        }

                    }
                }
            }
            return ok;
        }

        public List<MdMerchandise> GetMerByID(string id)
        {
            //MdMerchandiseService _service = new MdMerchandiseService(new IUnitOfWork());
            var unitCode = GetCurrentUnitCode();
            var data = Repository.DbSet.Where(x => x.UnitCode == unitCode);
            ERPContext db = new ERPContext();

            var lst = db.MdMerchandises.SqlQuery("SELECT * from TABLE( SEARCH_VATTU.GETROW_VATTU('" + id.ToString() + "'))").ToList();

            if (lst == null)
            {
                return new List<MdMerchandise>();
            }
            return lst;


        }

        public string CheckBarcodeAtSQL(string barcodes)
        {
            var result = "";


            using (var ctx = new DBCSQL())
            {

                if (!string.IsNullOrWhiteSpace(barcodes))
                {
                    var barcodeCollection = barcodes.Split(';');
                    barcodeCollection = barcodeCollection.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    foreach (var code in barcodeCollection)
                    {
                        try
                        {
                            var str = string.Format("SELECT * FROM TDS_Dmmathang WHERE Barcode LIKE '%;{0};%'", code);
                            var data = ctx.TDS_Dmmathang.SqlQuery(str);
                            if (data.Count() > 0)
                            {
                                result = string.Format("{0};{1}", result, code);
                            }
                        }
                        catch (Exception e)
                        {

                            throw e;
                        }
                    }
                }
            }
            return result;
        }
        public List<MdMerchandiseVm.MasterDto> GetAllDataChild(string maVatTu)
        {
            try
            {
                List<MdMerchandiseVm.MasterDto> result = new List<MdMerchandiseVm.MasterDto>();
                var unitCode = GetCurrentUnitCode();
                List<MdMerchandise> instance = Repository.DbSet.Where(x => x.MaCha == maVatTu && x.UnitCode == unitCode).ToList();
                if (instance.Count > 0)
                {
                    instance.ForEach(x => {
                        MdMerchandiseVm.MasterDto masterData = new MdMerchandiseVm.MasterDto();
                        masterData = Mapper.Map<MdMerchandise, MdMerchandiseVm.MasterDto>(x);
                        var detailData = UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(y => y.MaVatTu == x.MaVatTu && x.UnitCode == unitCode).ToList();
                        masterData.DataDetails = Mapper.Map<List<MdMerchandisePrice>, List<MdMerchandiseVm.DtoDetail>>(detailData);
                        result.Add(masterData);
                    });
                }
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
