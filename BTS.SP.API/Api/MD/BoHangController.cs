using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.IO;
using System.Web.Hosting;
using OfficeOpenXml;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.NV;
using BTS.SP.API.Utils;
using OfficeOpenXml.Style;
using BTS.API.SERVICE.Authorize.Utils;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/BoHang")]
    [Route("{id?}")]
    [Authorize]
    public class BoHangController : ApiController
    {
        private readonly IMdBoHangService _service;
        public BoHangController(IMdBoHangService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        [AllowAnonymous]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            return data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj() { Value = x.MaBoHang, Text = x.TenBoHang, Id = x.Id }).ToList();
        }

        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "bohang")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdBoHangVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdBoHang>>();
            filtered.OrderBy = "NgayCT";
            filtered.OrderType = "DESC";
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
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
                        Field = ClassHelper.GetPropertyName(() => new MdBoHang().NgayCT),
                        Method = OrderMethod.DESC
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

        /// <summary>
        /// Create entity
        /// POST
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(MdBoHang))]
        [CustomAuthorize(Method = "THEM", State = "bohang")]
        [Route("Insert")]
        [HttpPost]
        public async Task<IHttpActionResult> Post(MdBoHangVm.Dto instance)
        {
            var result = new TransferObj<MdBoHang>();
            try
            {
                
                var item = _service.InsertDto(instance);
                _service.SaveCode();
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

        [Route("GetDetails/{id}")]
        public TransferObj<MdBoHangVm.Dto> GetDetails(string id)
        {
            var result = new TransferObj<MdBoHangVm.Dto>();
            var temp = new MdBoHangVm.Dto();
            var boHang = _service.FindById(id);
            var chitietBoHang = _service.UnitOfWork.Repository<MdBoHangChiTiet>().DbSet;
            if (boHang != null)
            {
                temp = Mapper.Map<MdBoHang, MdBoHangVm.Dto>(boHang);
                var chiTietBoHang = chitietBoHang.Where(x => x.MaBoHang == boHang.MaBoHang).ToList();
                //foreach (var x in chitietBoHang)
                //{
                    
                //}
                temp.DataDetails = Mapper.Map<List<MdBoHangChiTiet>, List<MdBoHangVm.DtoDetail>>(chiTietBoHang);
                result.Data = temp;
                result.Status = true;
            }
            return result;
        }

        [HttpGet]
        [Route("GetDataByCode/{code}")]
        public TransferObj<MdBoHangVm.Dto> GetDataByCode(string code)
        {
            code = code.ToUpper();
            var result = new TransferObj<MdBoHangVm.Dto>();
            var temp = new MdBoHangVm.Dto();
            var boHang = _service.UnitOfWork.Repository<MdBoHang>().DbSet.FirstOrDefault(x=>x.MaBoHang == code);
            temp = Mapper.Map<MdBoHang, MdBoHangVm.Dto>(boHang);
            if (boHang != null)
            {
                decimal tongle = 0;
                decimal tongbuon = 0;
                temp = Mapper.Map<MdBoHang, MdBoHangVm.Dto>(boHang);
                var chiTietBoHang = _service.UnitOfWork.Repository<MdBoHangChiTiet>().DbSet.Where(x => x.MaBoHang == boHang.MaBoHang).ToList();
                foreach (var x in chiTietBoHang)
                {
                    tongle += (decimal)x.TongBanLe;
                    tongbuon += (decimal) x.TongBanBuon;
                }
                //temp.DataDetails = Mapper.Map<List<MdBoHangChiTiet>, List<MdBoHangVm.DtoDetail>>(chiTietBoHang);
                temp.TongLe = tongle;
                temp.TongBuon = tongbuon;
                result.Data = temp;
                result.Status = true;
            }
            return result;
        }

        [ResponseType(typeof(MdBoHang))]
        [CustomAuthorize(Method = "XOA", State = "bohang")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdBoHang instance = await _service.Repository.FindAsync(id);
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
        /// Update entity
        /// PUT
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "bohang")]
        public async Task<IHttpActionResult> Put(string id, MdBoHangVm.Dto instance)
        {
            var result = new TransferObj<MdBoHang>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                for (int i = 0; i < instance.DataDetails.Count; i++)
                {
                    string maMoi = instance.DataDetails[i].MaHang;
                    var unitcode = _service.GetCurrentUnitCode();
                    var sa = _service.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.FirstOrDefault(x => x.MaVatTu == maMoi  && x.MaDonVi == unitcode);
                    if (sa != null)
                    {
                        instance.DataDetails[i].DonGia = sa.GiaBanLe;
                    }
                }
                var item = _service.UpdateDto(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Message = "Cập nhật thành công";
                result.Data = item;
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }

        public MemoryStream CreateExcelTemplate(MdBoHangVm.Dto param)
        {
            var ms = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                package.Workbook.Worksheets.Add("Sheet1");
                var worksheet = package.Workbook.Worksheets["Sheet1"];
                int startColumn = 1;
                worksheet.Cells[1, 1].Value = "STT"; worksheet.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 2].Value = "Masieuthi"; worksheet.Cells[1, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 3].Value = "Tenviettat"; worksheet.Cells[1, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 4].Value = "Barcode"; worksheet.Cells[1, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 5].Value = "Giabanbuoncovat"; worksheet.Cells[1, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 6].Value = "Giabanlecovat"; worksheet.Cells[1, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 7].Value = "Makhachhang"; worksheet.Cells[1, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, 8].Value = "Soluong"; worksheet.Cells[1, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                int currentRow = 2;
                int stt = 0;
                for (int i = 0; i < param.SoLuongIn; i++)
                {
                    worksheet.Cells[currentRow, startColumn].Value = i + 1;
                    worksheet.Cells[currentRow, startColumn + 1].Value = param.MaBoHang;
                    worksheet.Cells[currentRow, startColumn + 2].Value = UnicodetoTCVN222(param.TenBoHang);
                    worksheet.Cells[currentRow, startColumn + 3].Value = "";
                    worksheet.Cells[currentRow, startColumn + 4].Value = Formattienviet(param.ThanhTien.ToString());
                    worksheet.Cells[currentRow, startColumn + 5].Value = Formattienviet(param.ThanhTien.ToString());
                    worksheet.Cells[currentRow, startColumn + 6].Value = "";
                    worksheet.Cells[currentRow, startColumn + 7].Value = 1;
                    currentRow++;
                }
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                int totalRows = worksheet.Dimension.End.Row;
                int totalCols = worksheet.Dimension.End.Column;
                var dataCells = worksheet.Cells[1, 1, totalRows, totalCols];
                var dataFont = dataCells.Style.Font;
                dataFont.SetFromFont(new Font(".VnTime", 10));
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                HttpContext.Current.Response.Charset = "UTF-8";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                package.SaveAs(ms);
                return ms;
            }
        }

        [Route("GetNewCode")]
        public string GetNewCode()
        {
            return _service.BuildCode();
        }
        public String UnicodetoTCVN222(String strUnicode)
        {
            string strReturn = string.Empty;
            string strTest = "a,à,á,ả,ã,ạ,â,ầ,ấ,ẩ,ẫ,ậ,ă,ằ,ắ,ẳ,ẵ,ặ,e,è,é,ẻ,ẽ,ẹ,ê,ề,ế,ể,ễ,ệ,i,ì,í,ỉ,ĩ,ị,o,ò,ó,ỏ,õ,ọ,ơ,ờ,ớ,ở,ỡ,ợ,ô,ồ,ố,ổ,ỗ,ộ,u,ù,ú,ủ,ũ,ụ,ư,ừ,ứ,ử,ữ,ự,y,ỳ,ý,ỷ,ỹ,ỵ,đ";
            for (int j = 0; j < strUnicode.Length; j++)
            {
                if (strTest.Contains(strUnicode[j].ToString().ToLower()))
                {
                    //convert sang TCVN
                    StringBuilder strB = new StringBuilder(strUnicode[j].ToString().ToLower());
                    StringBuilder strtemp = new StringBuilder(strUnicode[j].ToString().ToLower());
                    #region chuyển mã kí tự unicode thường sang TCVN


                    //                            y          ỳ      ý       ỷ           ỹ       ỵ               
                    char[] Unicode_char = {             '\u1EF3','\u00FD','\u1EF7','\u1EF9','\u1EF5',
                //                            ư          ừ       ứ      ử           ữ      ự
                                            '\u01B0','\u1EEB','\u1EE9','\u1EED','\u1EEF','\u1EF1',
                //                            o          ò       ó      ỏ           õ      ọ
                                                     '\u00F2','\u00F3','\u1ECF','\u00F5','\u1ECD',
                //                            ơ          ờ       ớ      ở           ỡ      ợ
                                            '\u01A1','\u1EDD','\u1EDB','\u1EDF','\u1EE1','\u1EE3',
                //                            ô          ồ       ố      ổ           ỗ      ộ
                                            '\u00F4','\u1ED3','\u1ED1','\u1ED5','\u1ED7','\u1ED9',
                //                            i          ì       í      ỉ           ĩ      ị
                                                     '\u00EC','\u00ED','\u1EC9','\u0129','\u1ECB',
                //                            ê          ề       ế      ể           ễ      ệ
                                            '\u00EA','\u1EC1','\u1EBF','\u1EC3','\u1EC5','\u1EC7',
                //                            e          è       é      ẻ           ẽ      ẹ
                                                     '\u00E8','\u00E9','\u1EBB','\u1EBD','\u1EB9',
                //                            ă          ằ       ắ      ẳ           ẵ      ặ
                                            '\u0103','\u1EB1','\u1EAF','\u1EB3','\u1EB5','\u1EB7',
                //                            a          à       á      ả           ã      ạ
                                                     '\u00E0','\u00E1','\u1EA3','\u00E3','\u1EA1',
                //                            â          ầ       ấ      ẩ           ẫ      ậ
                                            '\u00E2','\u1EA7','\u1EA5','\u1EA9','\u1EAB','\u1EAD',
                //                            u          ù       ú      ủ           ũ      ụ
                                                     '\u00F9','\u00FA','\u1EE7','\u0169','\u1EE5',
                //                            đ
                                            '\u0111'};


                    //                            y          ỳ      ý       ỷ           ỹ       ỵ               
                    char[] TCVN_char = {                '\u00FA', '\u00FD','\u00FB','\u00FC','\u00FE',
                //                            ư          ừ       ứ      ử           ữ      ự
                                            '\u00AD','\u00F5','\u00F8','\u00F6','\u00F7','\u00F9',
                //                            o          ò       ó      ỏ           õ      ọ
                                                     '\u00DF','\u00E3','\u00E1','\u00E2','\u00E4',
                //                            ơ          ờ       ớ      ở           ỡ      ợ
                                            '\u00AC','\u00EA','\u00ED','\u00EB','\u00EC','\u00EE',
                //                            ô          ồ       ố      ổ           ỗ      ộ
                                            '\u00AB','\u00E5','\u00E8','\u00E6','\u00E7','\u00E9',
                //                            i          ì       í      ỉ           ĩ      ị
                                                     '\u00D7','\u00DD','\u00D8','\u00DC','\u00DE',
                //                            ê          ề       ế      ể           ễ      ệ
                                            '\u00AA','\u00D2','\u00D5','\u00D3','\u00D4','\u00D6',
                //                            e          è       é      ẻ           ẽ      ẹ
                                                     '\u00CC','\u00D0','\u00CE','\u00CF','\u00D1',
                //                            ă          ằ       ắ      ẳ           ẵ      ặ
                                            '\u00A8','\u00BB','\u00BE','\u00BC','\u00BD','\u00C6',
                //                            a          à       á      ả           ã      ạ
                                                     '\u00B5','\u00B8','\u00B6','\u00B7','\u00B9',
                //                            â          ầ       ấ      ẩ           ẫ      ậ
                                            '\u00A9','\u00C7','\u00CA','\u00C8','\u00C9','\u00CB',
                //                            u          ù       ú      ủ           ũ      ụ
                                                     '\u00EF','\u00F3','\u00F1','\u00F2','\u00F4',
                //                            đ
                                            '\u00AE'};

                    for (int i = 0; i < Unicode_char.Length; i++)
                    {
                        char a = Unicode_char[i];
                        char b = TCVN_char[i];
                        strB.Replace(a, b);
                        if (strtemp.ToString() != strB.ToString())
                        {
                            break;
                        }
                    }
                    strReturn = strReturn + strB.ToString();
                    #endregion
                }
                else
                {
                    //ko convert
                    strReturn = strReturn + strUnicode[j].ToString();
                }

            }
            return strReturn;
        }
        //WriteDataToExcel
        [Route("WriteDataToExcel")]
        public async Task<IHttpActionResult> WriteDataToExcel(List<MdBoHangVm.Dto> data)
        {
            var result = new TransferObj<MdBoHangVm.Dto>();
            try
            {
                var filenameTemp = "TemPlateBoHang";
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
                var filenameNew = @"TemPlateBoHang.xls";
                FileInfo newFile = new FileInfo(pathAbs + filenameNew);

                if (newFile.Exists) //Kiểm tra file mới này đã tồn tại chưa
                {
                    newFile.Delete();
                    //newFile = new FileInfo(pathAbs + @"Barcode.xls");
                    newFile = new FileInfo(pathAbs + filenameNew);
                }
                using (ExcelPackage package = new ExcelPackage(newFile, tempFile))
                {
                    var worksheet = package.Workbook.Worksheets[1];
                    int index = 0;

                    for (int i = 0; i <data.Count; i++)
                    {
                        for (int j = 0; j < data[i].SoLuong; j++)
                        {
                            worksheet.Cells[index + 2, 1].Value = index + 1;
                            worksheet.Cells[index + 2, 2].Value = data[i].MaBoHang;
                            worksheet.Cells[index + 2, 3].Value = UnicodetoTCVN222(data[i].TenBoHang);
                            worksheet.Cells[index + 2, 4].Value = "";
                            worksheet.Cells[index + 2, 5].Value = Formattienviet(data[i].TongBuon.ToString());
                            worksheet.Cells[index + 2, 6].Value = Formattienviet(data[i].TongLe.ToString());
                            worksheet.Cells[index + 2, 7].Value = "";
                            worksheet.Cells[index + 2, 8].Value = 1;
                            index++;
                        }
                    }
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalCols = worksheet.Dimension.End.Column;
                    var dataCells = worksheet.Cells[2, 1, totalRows, totalCols];
                    var dataFont = dataCells.Style.Font;
                    dataFont.SetFromFont(new Font(".VnTime", 10));

                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                    HttpContext.Current.Response.Charset = "UTF-8";
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    package.SaveAs(newFile);
                }
                result.Status = true;
                result.Message = filenameNew;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Data = null;
                result.Message = ex + "";

            }
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "bohang")]
        public IHttpActionResult PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdBoHangVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdBoHang>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdBoHang().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = unitCode
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdBoHang>, PagedObj<ChoiceObj>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        //
        public static string Formattienviet(string _string)
        {
            string str;
            try
            {
                str = ConvertVNCurencyFormat(decimal.Parse(_string));
            }
            catch
            {
                str = "0";
            }
            return str;
        }
        public static string ConvertVNCurencyFormat(decimal number)
        {
            return number.ToString("C", GetVNeseCultureInfo());
        }

        public static CultureInfo GetVNeseCultureInfo()
        {
            int[] numArray = new int[] { 3 };
            int[] numArray1 = numArray;
            numArray = new int[1];
            int[] numArray2 = numArray;
            CultureInfo cultureInfo = new CultureInfo("vi-VN", true);
            cultureInfo.NumberFormat.CurrencyDecimalDigits = 0;
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
            cultureInfo.NumberFormat.CurrencyGroupSeparator = ",";
            cultureInfo.NumberFormat.CurrencyGroupSizes = numArray1;
            cultureInfo.NumberFormat.CurrencySymbol = "";
            cultureInfo.NumberFormat.NumberDecimalDigits = 2;
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultureInfo.NumberFormat.NumberGroupSeparator = ",";
            cultureInfo.NumberFormat.NumberGroupSizes = numArray1;
            cultureInfo.NumberFormat.PercentDecimalDigits = 1;
            cultureInfo.NumberFormat.PercentGroupSizes = numArray2;
            cultureInfo.NumberFormat.PercentDecimalSeparator = ".";
            cultureInfo.NumberFormat.PercentGroupSeparator = ",";
            cultureInfo.NumberFormat.PercentSymbol = "%";
            return cultureInfo;
        }
        //Đồng bộ
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
