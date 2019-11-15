using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.NV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BTS.API.ENTITY.Md;
using System.Web;
using System.IO;
using System.Web.Hosting;
using OfficeOpenXml;
using System.Text;
using System.Globalization;
using System.Drawing;
using System.Net.Http.Headers;
using BTS.API.SERVICE.MD;
using System.Security.Claims;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize.Utils;
using System.Runtime.InteropServices;
using BTS.SP.API.Utils;
using OfficeOpenXml.Style;
namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/NhapHangMua")]
    [Route("{id?}")]
    [Authorize]
    public class NhapHangMuaController : ApiController
    {
        private readonly INvNhapHangMuaService _service;
        private readonly IDclGeneralLedgerService _serviceGeneral;
        private readonly IMdPeriodService _servicePeriod;
        private readonly INvCongNoService _serviceCongNo;
        private readonly IMdMerchandisePriceService _servicePrice;
        public NhapHangMuaController(INvNhapHangMuaService service, IDclGeneralLedgerService service2, IMdPeriodService servicePeriod, INvCongNoService serviceCongNo, IMdMerchandisePriceService servicePrice)
        {
            _service = service;
            _serviceGeneral = service2;
            _servicePeriod = servicePeriod;
            _serviceCongNo = serviceCongNo;
            _servicePrice = servicePrice;
        }
        #region VUDQ_IMPORT_EXCEL
        [Route("ImportExcelNhapHangMua/{unitCode}")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ImportExcelNhapHangMua(string unitCode)
        {
            string parent = "";
            string url = "";
            var result = new TransferObj();
            if (!string.IsNullOrEmpty(unitCode))
            {
                var parentUnitCode = _service.UnitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == unitCode);
                if (parentUnitCode != null) parent = parentUnitCode.MaDonViCha;
            }
            string path = _service.GetPhysicalPathImportFile();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //string path = @"C:/inetpub/wwwroot/wss/VirtualDirectories/BANHANG/Upload/DataHangHoa/";
            HttpRequest request = HttpContext.Current.Request;
            try
            {
                if (request.Files.Count > 0)
                {
                    List<NvNhapHangMuaVm.ObjectImportExcel> lstImportExcel = new List<NvNhapHangMuaVm.ObjectImportExcel>();
                    HttpPostedFile file = request.Files[0];
                    file.SaveAs(path + file.FileName);
                    file.InputStream.Close();
                    if (File.Exists(path + file.FileName))
                    {
                        url = File.Exists(path + file.FileName).ToString();
                        //doc du lieu tu file
                        Microsoft.Office.Interop.Excel.Application xlApp;
                        Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                        Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                        Microsoft.Office.Interop.Excel.Range range;
                        int rw = 0, cl = 0;
                        xlApp = new Microsoft.Office.Interop.Excel.Application();
                        string pathFile = path + file.FileName;
                        xlWorkBook = xlApp.Workbooks.Open(@"" + pathFile + "", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                        xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                        range = xlWorkSheet.UsedRange;
                        rw = range.Rows.Count;
                        cl = range.Columns.Count;
                        Microsoft.Office.Interop.Excel.Range last = xlWorkSheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
                        int lastUsedRow = last.Row;
                        for (int rCnt = 2; rCnt <= lastUsedRow; rCnt++) //từ row 2 -- column 1
                        {
                            decimal slLe = 0, slBao = 0, slTong = 0, giaMuaChuaVAT = 0, giaMuaCoVAT = 0, VAT = 0;
                            NvNhapHangMuaVm.ObjectImportExcel record = new NvNhapHangMuaVm.ObjectImportExcel();
                            int cCnt = 1; //bỏ qua ô số thứ tự
                            record.MaHang = string.IsNullOrEmpty((string)(range.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value2) ? "" : (string)(range.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value2;
                            record.TenHang = string.IsNullOrEmpty((string)(range.Cells[rCnt, cCnt + 1] as Microsoft.Office.Interop.Excel.Range).Value2) ? "" : (string)(range.Cells[rCnt, cCnt + 1] as Microsoft.Office.Interop.Excel.Range).Value2;
                            record.DonViTinh = string.IsNullOrEmpty((string)(range.Cells[rCnt, cCnt + 2] as Microsoft.Office.Interop.Excel.Range).Value2) ? "" : (string)(range.Cells[rCnt, cCnt + 2] as Microsoft.Office.Interop.Excel.Range).Value2;
                            record.MaBaoBi = string.IsNullOrEmpty((string)(range.Cells[rCnt, cCnt + 3] as Microsoft.Office.Interop.Excel.Range).Value2) ? "" : (string)(range.Cells[rCnt, cCnt + 3] as Microsoft.Office.Interop.Excel.Range).Value2;
                            string slBaostr = Convert.ToString((range.Cells[rCnt, cCnt + 5] as Microsoft.Office.Interop.Excel.Range).Value2);
                            decimal.TryParse(slBaostr, out slBao);
                            record.SoLuongBao = slBao;
                            string slLestr = Convert.ToString((range.Cells[rCnt, cCnt + 6] as Microsoft.Office.Interop.Excel.Range).Value2);
                            decimal.TryParse(slLestr, out slLe);
                            record.SoLuongLe = slLe;
                            var objBaoBi = _service.UnitOfWork.Repository<MdPackaging>()
                                .DbSet.FirstOrDefault(x => x.MaBaoBi == record.MaBaoBi);

                            record.SoLuong = objBaoBi == null
                                ? record.SoLuongLe
                                : (record.SoLuongBao * objBaoBi.SoLuong + record.SoLuongLe);
                            string giaMuaChuaVATstr = Convert.ToString((range.Cells[rCnt, cCnt + 8] as Microsoft.Office.Interop.Excel.Range).Value2);

                            decimal.TryParse(giaMuaChuaVATstr, out giaMuaChuaVAT);
                            record.DonGia = giaMuaChuaVAT;
                            string VATstr = Convert.ToString((range.Cells[rCnt, cCnt + 9] as Microsoft.Office.Interop.Excel.Range));
                            decimal.TryParse(VATstr, out VAT);
                            record.TyLeVatVao = VAT;
                            string giaMuaCoVATstr = Convert.ToString((range.Cells[rCnt, cCnt + 10] as Microsoft.Office.Interop.Excel.Range).Value2);

                            decimal.TryParse(giaMuaCoVATstr, out giaMuaCoVAT);
                            record.GiaMuaCoVat = giaMuaCoVAT;

                            record.ThanhTien = record.DonGia * record.SoLuong;
                            record.ThanhTienVAT = record.GiaMuaCoVat * record.SoLuong;

                            //var objHang= _service.UnitOfWork.Repository<MdMerchandise>()
                            //        .DbSet.FirstOrDefault(x => x.MaVatTu == record.MaHang );
                            IQueryable<MdMerchandiseVm.Dto> data = ProcedureCollection.GetMerchandise(new ERPContext(), record.MaHang, unitCode);
                            try
                            {
                                MdMerchandiseVm.Dto objHang = data.ToList()[0];
                                record.Barcode = objHang == null ? "" : objHang.Barcode;
                                record.TenHang = objHang == null ? "" : objHang.TenHang;
                                record.Exist = true;
                                record.DonViTinh = objHang == null ? "" : objHang.DonViTinh;
                                if (giaMuaChuaVATstr == null)
                                {
                                    record.DonGia = objHang == null ? 0 : objHang.GiaMua;
                                    record.GiaMuaCoVat = objHang == null ? 0 : objHang.GiaMua * (1 + 1 / objHang.TyLeVatVao);
                                    record.TyLeVatVao = objHang == null ? 0 : objHang.TyLeVatVao;
                                    record.TyLeLaile = objHang == null ? 0 : objHang.TyLeLaiLe;
                                    record.GiaBanLe = objHang == null ? 0 : objHang.GiaBanLe;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (record.TenHang == null)
                                {
                                    record.Exist = false;
                                }
                            }
                            lstImportExcel.Add(record);
                        }
                        if (lstImportExcel.Count > 0)
                        {
                            result.Status = true;
                            result.Data = lstImportExcel;
                            result.Message = "Import thành công " + lstImportExcel.Count + " mã hàng khuyến mãi !";
                        }
                        else
                        {
                            result.Status = false;
                            result.Data = null;
                            result.Message = "Không đọc được dữ liệu";
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
                result.Message = e.ToString();
                result.Data = null;
                result.Status = false;
                WriteLogs.LogError(e);
                return Ok(result);
            }
            return Ok(result);
        }

        #endregion
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "phieuNhapHangMua")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            TransferObj<PagedObj<NvNhapHangMuaVm.Dto>> result = new TransferObj<PagedObj<NvNhapHangMuaVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvNhapHangMuaVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangMuaVm.Search>>();
            PagedObj<NvVatTuChungTu> paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            filtered.OrderBy = "NgayCT";
            filtered.OrderType = "DECS";
            string unitCode = _service.GetCurrentUnitCode();
            QueryBuilder query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.NMUA.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvVatTuChungTu().ICreateDate),
                        Method = OrderMethod.DESC

                    }
                }
            };
            try
            {
                ResultObj<PagedObj<NvVatTuChungTu>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = Mapper.Map<PagedObj<NvVatTuChungTu>, PagedObj<NvNhapHangMuaVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                throw e;
            }
        }

        [Route("PostPrint")]
        public async Task<IHttpActionResult> PostPrint(JObject jsonData)
        {
            var result = new List<NvNhapHangMuaVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangMuaVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.NMUA.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvVatTuChungTu().ICreateDate),
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
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvNhapHangMuaVm.Dto>>(filterResult.Value.Data);
                return Ok(result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                return InternalServerError();
            }
        }
        [Route("PostPrintDetail")]
        public async Task<IHttpActionResult> PostPrintDetail(JObject jsonData)
        {
            var result = new List<NvNhapHangMuaVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangMuaVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.NMUA.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And,
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvVatTuChungTu().ICreateDate),
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

                result = Mapper.Map<List<NvVatTuChungTu>, List<NvNhapHangMuaVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapHangMuaVm.DtoDetail>>(details.ToList());
                    }
                    {
                        var details = _service.UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataClauseDetails = Mapper.Map<List<DclGeneralLedger>, List<NvNhapHangMuaVm.DtoClauseDetail>>(details.ToList());
                    }

                });
                return Ok(result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                throw e;
            }
        }

        [Route("Post")]
        [HttpPost]
        [CustomAuthorize(Method = "THEM", State = "phieuNhapHangMua")]
        public async Task<IHttpActionResult> Post(NvNhapHangMuaVm.Dto instance)
        {
            string parentUnitCode = _service.GetParentUnitCode();
            TransferObj<NvVatTuChungTu> result = new TransferObj<NvVatTuChungTu>();
            try
            {
                if (_service.ValidateNgayCT(instance.NgayCT.Value))
                {
                    NvVatTuChungTu item = _service.InsertPhieu(instance);
                    if (item != null)
                    {
                        if (await _service.UnitOfWork.SaveAsync() > 0)
                        {
                            result.Data = item;
                            result.Message = "Thêm mới thành công";
                            result.Status = true;
                            //update giá trong danh mục hàng hóa
                            if (instance.DataDetails.Count > 0)
                            {
                                foreach (NvNhapHangMuaVm.DtoDetail row in instance.DataDetails)
                                {
                                    decimal giaMua, giaMuaCoVat, tyLeLaiLe, tyLeLaiBuon, tyLeVatRa = 0;
                                    MdMerchandisePrice price = _servicePrice.Repository.DbSet.FirstOrDefault((x => x.MaVatTu == row.MaHang && x.MaDonVi.StartsWith(parentUnitCode)));
                                    if (price != null)
                                    {
                                        decimal.TryParse(row.DonGia.ToString(), out giaMua);
                                        decimal.TryParse(row.GiaMuaCoVat.ToString(), out giaMuaCoVat);
                                        decimal.TryParse(row.TyLeLaiLe.ToString(), out tyLeLaiLe);
                                        decimal.TryParse(row.TyLeLaiBuon.ToString(), out tyLeLaiBuon);
                                        decimal.TryParse(row.TyLeVatRa.ToString(), out tyLeVatRa);
                                        price.GiaMuaVat = giaMuaCoVat;
                                        price.TyLeLaiLe = 100 * (price.GiaBanLe - giaMua) / giaMua;
                                        price.TyLeLaiBuon = 100 * (price.GiaBanBuon - giaMua) / giaMua;
                                        price.IUpdateBy = _service.GetClaimsPrincipal().Identity.Name;
                                        price.UnitCode = _service.GetCurrentUnitCode();
                                        price.IState = instance.MaChungTu;
                                        price.ObjectState = ObjectState.Modified;
                                        _servicePrice.UnitOfWork.Repository<MdMerchandisePrice>().Update(price);
                                    }
                                }
                                if (await _servicePrice.UnitOfWork.SaveAsync() > 0)
                                {
                                    result.Message = "Thêm mới thành công";
                                }
                            }
                            //end
                        }
                        else
                        {
                            result.Data = null;
                            result.Message = "Xảy ra lỗi khi thêm mới";
                            result.Status = false;
                            return BadRequest(result.Message);
                        }
                    }
                    else
                    {
                        result.Data = null;
                        result.Message = "Không tìm thấy dòng chi tiết phiếu";
                        result.Status = false;
                        return BadRequest(result.Message);
                    }
                    return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
                }
                else
                {
                    result.Message = "Ngày chứng từ sai!";
                    result.Status = false;
                    return BadRequest(result.Message);
                }
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }

            return Ok(result);
        }

        #region Old Version
        [Route("PostExportExcelDetail")]
        public HttpResponseMessage PostExportExcelDetail(NvNhapHangMuaVm.ParameterNMua para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _service.ExportExcelDetails(para);
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.Option)
                {
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonTongTheoNhaCungCap.xlsx" };
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonTongTheoLoaiHang.xlsx" };
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonTongTheoNhomHang.xlsx" };
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.hangHoa:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonTongTheoHangHoa.xlsx" };
                        break;

                };
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonChiTiet.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcelByMerchandiseType")]
        public HttpResponseMessage PostExportExcelByMerchandiseType(JObject jsonData)
        {
            var result = new List<NvNhapHangMuaVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangMuaVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.NMUA.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvVatTuChungTu().ICreateDate),
                        Method = OrderMethod.DESC

                    }
                }
            };
            try
            {
                filtered.AdvanceData.TuNgay = filtered.AdvanceData.TuNgay.HasValue ? filtered.AdvanceData.TuNgay.Value : new DateTime(2015, 2, 2);
                filtered.AdvanceData.DenNgay = filtered.AdvanceData.DenNgay.HasValue ? filtered.AdvanceData.DenNgay.Value : new DateTime(2018, 2, 2);
                //var filterResult = _service.Filter(filtered, query);
                //if (!filterResult.WasSuccessful)
                //{
                //    return null;
                //}
                //result = Mapper.Map<List<NvVatTuChungTu>, List<NvNhapHangMuaVm.Dto>>(filterResult.Value.Data);

                //result.ForEach(x =>
                //{
                //    {
                //        var details = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                //        x.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapHangMuaVm.DtoDetail>>(details.ToList());
                //        foreach(var item in x.DataDetails)
                //        {
                //            var hanghoadamap = new MdMerchandiseVm.Dto();
                //            var loaihanghoadamap = new MdMerchandiseTypeVm.Dto();
                //            var hanghoa = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(v => v.MaVatTu == item.MaHang);
                //            if (hanghoa != null)
                //            {
                //                var loaihanghoa = _service.UnitOfWork.Repository<MdMerchandiseType>().DbSet.FirstOrDefault(v => v.MaLoaiVatTu == hanghoa.MaLoaiVatTu);
                //                if (loaihanghoa != null)
                //                    loaihanghoadamap = Mapper.Map<MdMerchandiseType, MdMerchandiseTypeVm.Dto>(loaihanghoa);
                //                item.MaLoaiHang = loaihanghoadamap.MaLoaiVatTu;
                //                item.TenLoaiHang = loaihanghoadamap.TenLoaiVatTu;
                //                item.chietkhau = x.ChietKhau;
                //                item.TyLeVatVao = (x.ThanhTienSauVat - x.ThanhTienTruocVat);
                //            }
                //        }
                //    }
                //});

                var streamData = _service.ExportExcelByMerchandiseType(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuNhapTheoLoaiHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        [Route("PostExportExcelByMerchandiseGroup")]
        public HttpResponseMessage PostExportExcelByMerchandiseGroup(JObject jsonData)
        {
            var result = new List<NvNhapHangMuaVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangMuaVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.NMUA.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvVatTuChungTu().ICreateDate),
                        Method = OrderMethod.DESC

                    }
                }
            };
            try
            {
                filtered.AdvanceData.TuNgay = filtered.AdvanceData.TuNgay.HasValue ? filtered.AdvanceData.TuNgay.Value : new DateTime(2015, 2, 2);
                filtered.AdvanceData.DenNgay = filtered.AdvanceData.DenNgay.HasValue ? filtered.AdvanceData.DenNgay.Value : new DateTime(2018, 2, 2);
                var streamData = _service.ExportExcelByMerchandiseGroup(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuNhapTheoNhomHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcel")]
        public HttpResponseMessage PostExportExcel(JObject jsonData)
        {
            var result = new List<NvNhapHangMuaVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangMuaVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.NMUA.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvVatTuChungTu().ICreateDate),
                        Method = OrderMethod.DESC

                    }
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return null;
                }
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvNhapHangMuaVm.Dto>>(filterResult.Value.Data);
                var streamData = _service.ExportExcel(result, filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuNhapMua.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        [Route("PostExportExcelByMerchandise")]
        public HttpResponseMessage PostExportExcelByMerchandise(JObject jsonData)
        {
            var result = new List<NvNhapHangMuaVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangMuaVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                filtered.AdvanceData.TuNgay = filtered.AdvanceData.TuNgay.HasValue ? filtered.AdvanceData.TuNgay.Value : new DateTime(2015, 1, 1);
                filtered.AdvanceData.DenNgay = filtered.AdvanceData.DenNgay.HasValue ? filtered.AdvanceData.DenNgay.Value : new DateTime(2018, 1, 1);
                var streamData = _service.ExportExcelByMerchandise(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuNhapTheoHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        [Route("PostExportExcelByNhaCungCap")]
        public HttpResponseMessage PostExportExcelByNhaCungCap(JObject jsonData)
        {
            var result = new List<NvNhapHangMuaVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangMuaVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.NMUA.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvVatTuChungTu().ICreateDate),
                        Method = OrderMethod.DESC

                    }
                }
            };
            try
            {
                filtered.AdvanceData.TuNgay = filtered.AdvanceData.TuNgay.HasValue ? filtered.AdvanceData.TuNgay.Value : new DateTime(2015, 2, 2);
                filtered.AdvanceData.DenNgay = filtered.AdvanceData.DenNgay.HasValue ? filtered.AdvanceData.DenNgay.Value : new DateTime(2018, 2, 2);
                var streamData = _service.ExportExcelByNhaCungCap(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuNhapTheoNCC.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }


        [Route("WriteDataToExcel")]
        public async Task<IHttpActionResult> WriteDataToExcel(NvNhapHangMuaVm.Dto data)
        {
            var result = new TransferObj<NvNhapHangMuaVm.Dto>();
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
                var filenameNew = @"Barcode.xls";
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

                    for (int i = 0; i < data.DataDetails.Count; i++)
                    {
                        for (int j = 0; j < data.DataDetails[i].SoLuong; j++)
                        {
                            worksheet.Cells[index + 2, 1].Value = index + 1;
                            worksheet.Cells[index + 2, 2].Value = data.DataDetails[i].MaHang;
                            worksheet.Cells[index + 2, 3].Value = UnicodetoTCVN222(data.DataDetails[i].TenHang);
                            worksheet.Cells[index + 2, 4].Value = data.DataDetails[i].Barcode;
                            worksheet.Cells[index + 2, 5].Value = Formattienviet(data.DataDetails[i].GiaBanLe.ToString()) + " VND";
                            worksheet.Cells[index + 2, 6].Value = Formattienviet(data.DataDetails[i].GiaBanLe.ToString()) + " VND";
                            worksheet.Cells[index + 2, 7].Value = data.MaKhachHang;
                            worksheet.Cells[index + 2, 8].Value = "1";
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
                WriteLogs.LogError(ex);
            }
            return Ok(result);
        }

        //Phạm Tuấn Anh - In tem theo mã kệ - số lượng chỉ bằng 1
        [Route("WriteDataToExcelByShelves")]
        public async Task<IHttpActionResult> WriteDataToExcelByShelves(NvNhapHangMuaVm.Dto data)
        {
            TransferObj<NvNhapHangMuaVm.Dto> result = new TransferObj<NvNhapHangMuaVm.Dto>();
            try
            {
                string filenameTemp = "TemPlateNhapMua";
                string pathRelaTemp = string.Format(@"~/Upload/Barcode/");
                string pathAbsTemp = HostingEnvironment.MapPath(pathRelaTemp);
                if (pathAbsTemp != null)
                {
                    DirectoryInfo getAbsoluteDirectoryInfoReport = new DirectoryInfo(pathAbsTemp);
                    getAbsoluteDirectoryInfoReport.Create();
                }
                FileInfo tempFile = new FileInfo(pathAbsTemp + filenameTemp + ".xlsx");
                string pathRela = string.Format(@"~/Upload/Barcode/");
                string pathAbs = HostingEnvironment.MapPath(pathRela);
                DirectoryInfo getAbsoluteDirectoryInfo = new DirectoryInfo(pathAbs);
                getAbsoluteDirectoryInfo.Create();
                string filenameNew = @"Barcode.xls";
                FileInfo newFile = new FileInfo(pathAbs + filenameNew);
                if (newFile.Exists) //Kiểm tra file mới này đã tồn tại chưa
                {
                    newFile.Delete();
                    //newFile = new FileInfo(pathAbs + @"Barcode.xls");
                    newFile = new FileInfo(pathAbs + filenameNew);
                }
                using (ExcelPackage package = new ExcelPackage(newFile, tempFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int startColumn = 1;
                    int currentRow = 2;
                    int index = 1;
                    for (int i = 0; i < data.DataDetails.Count; i++)
                    {
                        worksheet.Cells[currentRow, startColumn].Value = index;
                        worksheet.Cells[currentRow, startColumn + 1].Value = data.DataDetails[i].MaHang;
                        worksheet.Cells[currentRow, startColumn + 2].Value = UnicodetoTCVN222(data.DataDetails[i].TenHang);
                        worksheet.Cells[currentRow, startColumn + 3].Value = data.DataDetails[i].Barcode;
                        worksheet.Cells[currentRow, startColumn + 4].Value = Formattienviet(data.DataDetails[i].GiaBanLe.ToString()) + " VND";
                        worksheet.Cells[currentRow, startColumn + 5].Value = Formattienviet(data.DataDetails[i].GiaBanLe.ToString()) + " VND";
                        worksheet.Cells[currentRow, startColumn + 6].Value = data.MaKhachHang;
                        worksheet.Cells[currentRow, startColumn + 7].Value = 1;
                        currentRow++;
                        index++;
                    }
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalCols = worksheet.Dimension.End.Column;
                    ExcelRange dataCells = worksheet.Cells[2, 1, totalRows, totalCols];
                    ExcelFont dataFont = dataCells.Style.Font;
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
                WriteLogs.LogError(ex);
            }
            return Ok(result);
        }
        #endregion
        //Excel Chi Tiet
        [Route("PostExportExcelDetailCap2")]
        public HttpResponseMessage PostExportExcelDetailCap2(NvNhapHangMuaVm.ParameterNMua para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            string filename = "";
            string titleExcel = "DANH SÁCH PHIẾU ";
            try
            {
                MemoryStream streamData = new MemoryStream();
                switch (para.PhuongThucNhap)
                {
                    case PHUONGTHUCNHAP.NHAPMUA:
                        para.LoaiChungTu = TypeVoucher.NMUA.ToString();
                        filename = filename + "NhapMua";
                        titleExcel = titleExcel + "NHẬP HÀNG MUA";
                        break;
                    case PHUONGTHUCNHAP.NHAPBUONTRALAI:
                        para.LoaiChungTu = TypeVoucher.NHBANTL.ToString();
                        titleExcel = titleExcel + "BÁN BUÔN TRẢ LẠI";

                        filename = filename + "NhapBanBuonTraLai";
                        break;
                    case PHUONGTHUCNHAP.NHANDIEUCHUYEN:
                        para.LoaiChungTu = TypeVoucher.DCN.ToString();
                        titleExcel = titleExcel + "NHẬN ĐIỀU CHUYỂN";
                        filename = filename + "NhanDieuChuyen";
                        break;
                }
                streamData = _service.ExportExcelDetailsCap2(para, titleExcel);
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.Option)
                {
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                        filename = filename + "TheoNhaCungCap";
                        //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "NhapMuaChiTietTheoNhaCungCap.xlsx" };
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                        filename = filename + "TheoLoaiHang";
                        //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "NhapMuaChiTietTheoLoaiHang.xlsx" };
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                        filename = filename + "TheoNhomHang";
                        //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "NhapMuaChiTietTheoNhomHang.xlsx" };
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.hangHoa:
                        filename = filename + "TheoHangHoa";
                        //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "NhapMuaChiTietTheoHangHoa.xlsx" };
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.phieu:
                        filename = filename + "TheoPhieu";
                        //response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "NhapMuaChiTietTheoHangHoa.xlsx" };
                        break;

                };
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = filename + ".xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;

            }
            catch (Exception ex)
            {
                WriteLogs.LogError(ex);
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        //Report Tong Hop
        [Route("PostReportNhapMua")]
        public async Task<IHttpActionResult> PostReportNhapMua(NvNhapHangMuaVm.ParameterNMua para)
        {
            var result = new TransferObj<BTS.API.SERVICE.NV.NvNhapHangMuaVm.ObjectReportNMUA>();
            try
            {
                var reporter = new BTS.API.SERVICE.NV.NvNhapHangMuaVm.ObjectReportNMUA();
                var data = new List<BTS.API.SERVICE.NV.NvNhapHangMuaVm.ObjectReportCon>();
                reporter.UnitCode = _servicePeriod.GetCurrentUnitCode();
                var unitCode = _servicePeriod.GetCurrentUnitCode();
                reporter.CreateDateNow();
                reporter.FromDay = para.FromDate.Day;
                reporter.FromMonth = para.FromDate.Month;
                reporter.FromYear = para.FromDate.Year;
                reporter.ToDay = para.ToDate.Day;
                reporter.ToMonth = para.ToDate.Month;
                reporter.ToYear = para.ToDate.Year;
                //GetNhanVien
                if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
                {
                    var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                    var name = currentUser.Identity.Name;
                    var nhanVien = _service.UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                    if (nhanVien != null)
                    {
                        reporter.Username = nhanVien.TenNhanVien;
                    }
                    else
                    {
                        reporter.Username = "Administrator";
                    }
                }
                switch (para.PhuongThucNhap)
                {
                    case PHUONGTHUCNHAP.NHAPBUONTRALAI:
                        para.LoaiChungTu = TypeVoucher.NHBANTL.ToString();
                        break;
                    //case PHUONGTHUCNHAP.NHAPMUA:
                    default:
                        para.LoaiChungTu = TypeVoucher.NMUA.ToString();
                        break;
                }
                switch (para.Option)
                {
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                        //data = _service.CreateReportInventoryByMerchandiseByNCC(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        data = _service.CreateReportInventoryByTongHop(para.LoaiChungTu, InventoryGroupBy.MAKHACHHANG.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        reporter.DetailData = data.ToList();
                        reporter.DetailData.ForEach(x => x.MapSupplierName(_service.UnitOfWork));
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Nhà cung cấp";
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                        //data = _service.CreateReportInventoryByMerchandiseByML(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        data = _service.CreateReportInventoryByTongHop(para.LoaiChungTu, InventoryGroupBy.MALOAIVATTU.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        reporter.DetailData = data.ToList();
                        reporter.DetailData.ForEach(x => x.MapTypeName(_service.UnitOfWork));
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Loại hàng";
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                        //data = _service.CreateReportInventoryByMerchandiseByMG(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        data = _service.CreateReportInventoryByTongHop(para.LoaiChungTu, InventoryGroupBy.MANHOMVATTU.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        reporter.DetailData = data.ToList();
                        reporter.DetailData.ForEach(x => x.MapGroupName(_service.UnitOfWork));
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Nhóm hàng";
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.hangHoa:
                        //data = _service.CreateReportInventoryByMerchandiseByHang(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        data = _service.CreateReportInventoryByTongHop(para.LoaiChungTu, InventoryGroupBy.MAVATTU.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        reporter.DetailData = data.ToList();
                        reporter.DetailData.ForEach(x => x.MapMerchandiseName(_service.UnitOfWork));
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Hàng hóa";
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.phieu:
                        //data = _service.CreateReportInventoryByMerchandiseByHang(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        data = _service.CreateReportInventoryByTongHop(para.LoaiChungTu, InventoryGroupBy.PHIEU.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        reporter.DetailData = data.ToList();
                        reporter.DetailData.ForEach(x => x.MapMerchandiseName(_service.UnitOfWork));
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Phiếu";
                        break;
                    case BTS.API.SERVICE.NV.NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                        //data = _service.CreateReportInventoryByMerchandiseByHang(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        data = _service.CreateReportInventoryByTongHop(para.LoaiChungTu, InventoryGroupBy.MAKHO.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        reporter.DetailData = data.ToList();
                        reporter.DetailData.ForEach(x => x.MapMerchandiseName(_service.UnitOfWork));
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Kho";
                        break;
                    default:
                        //data = _service.CreateReportInventoryByDay(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                        break;
                }
                result.Data = reporter;
                result.Status = true;
                result.Message = "Xuất báo cáo thành công";
                return Ok(result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                return InternalServerError();
            }

        }
        //Excel Tong Hop
        [Route("PostExcelReportNhapMua")]
        public HttpResponseMessage PostExcelReportNhapMua(BTS.API.SERVICE.NV.NvNhapHangMuaVm.ObjectReportNMUA para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _service.ExportExcelTongHop(para);
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);

                switch (para.GroupType)
                {
                    case "Nhà cung cấp":
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCaoNhapMuaTongHopTheoNhaCungCap.xlsx" };
                        break;
                    case "Loại hàng":
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCaoNhapMuaTongHopTheoLoaiHang.xlsx" };
                        break;
                    case "Nhóm hàng":
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCaoNhapMuaTongHopTheooNhomHang.xlsx" };
                        break;
                    case "Hàng hóa":
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCaoNhapMuaTongHopTheoHangHoa.xlsx" };
                        break;
                    case "Phiếu":
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCaoNhapMuaTongHopTheoPhieu.xlsx" };
                        break;


                }; response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;

            }
            catch (Exception ex)
            {
                WriteLogs.LogError(ex);
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;

            }

        }
        //DCN
        [Route("PostReportDieuChuyenNhan")]
        public async Task<IHttpActionResult> PostReportDieuChuyenNhan(NvNhapHangMuaVm.ParameterNMua para)
        {
            NvGiaoDichQuayVm.ReportGDQTongHop reporter = new NvGiaoDichQuayVm.ReportGDQTongHop();
            List<NvGiaoDichQuayVm.ObjectReport> result = new List<NvGiaoDichQuayVm.ObjectReport>();
            try
            {
                var unitCode = _service.GetCurrentUnitCode();
                reporter.CreateDateNow();
                reporter.FromDay = para.FromDate.Day;
                reporter.FromMonth = para.FromDate.Month;
                reporter.FromYear = para.FromDate.Year;
                reporter.ToDay = para.ToDate.Day;
                reporter.ToMonth = para.ToDate.Month;
                reporter.ToYear = para.ToDate.Year;
                reporter.ToDate = para.ToDate;
                reporter.FromDate = para.FromDate;
                reporter.TenDonVi = CurrentSetting.GetUnitName(unitCode);
                reporter.DiaChiDonVi = CurrentSetting.GetUnitAddress(unitCode);
                //GetNhanVien
                if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
                {
                    var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                    var name = currentUser.Identity.Name;
                    var nhanVien = _service.UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                    if (nhanVien != null)
                    {
                        reporter.Username = nhanVien.TenNhanVien;

                    }
                    else
                    {
                        reporter.Username = "Administrator";
                    }
                }
                switch (para.PhuongThucNhap)
                {
                    case PHUONGTHUCNHAP.NHANDIEUCHUYEN:
                        reporter.DataDetails.AddRange(_service.ReportDieuChuyenNhan(para));
                        break;
                    case PHUONGTHUCNHAP.NHAPKHAC:
                        reporter.DataDetails.AddRange(_service.ReportNhapKhac(para));
                        break;
                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                WriteLogs.LogError(ex);
            }
            return Ok(reporter);
        }
        [Route("PostExportExcelDCNChiTiet")]
        public HttpResponseMessage PostExportExcelDCNChiTiet(NvNhapHangMuaVm.ParameterNMua para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                switch (para.PhuongThucNhap)
                {
                    case PHUONGTHUCNHAP.NHANDIEUCHUYEN:
                        streamData = _service.ExportExcelDCNDetail(para);
                        break;
                    case PHUONGTHUCNHAP.NHAPKHAC:
                        streamData = _service.ExportExcelNKhacDetail(para);
                        break;
                    default:
                        break;
                }
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.PhuongThucNhap)
                {
                    case PHUONGTHUCNHAP.NHANDIEUCHUYEN:
                        switch (para.Option)
                        {
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_ChiTiet_TheoNhomVatTu.xlsx" };
                                break;
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_ChiTiet_TheoLoaiVatTu.xlsx" };
                                break;
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_ChiTiet_TheoNhaCungCap.xlsx" };
                                break;
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.hangHoa:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_ChiTiet_TheoVatTu.xlsx" };
                                break;
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_ChiTiet_TheoKho.xlsx" };
                                break;
                            default:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_ChiTiet.xlsx" };
                                break;
                        }
                        break;
                    case PHUONGTHUCNHAP.NHAPKHAC:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCNhapKhac_ChiTiet.xlsx" };
                        break;
                }
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception ex)
            {
                WriteLogs.LogError(ex);
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcelDCNTongHop")]
        public HttpResponseMessage PostExportExcelDCNTongHop(NvNhapHangMuaVm.ParameterNMua para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                switch (para.PhuongThucNhap)
                {
                    case PHUONGTHUCNHAP.NHANDIEUCHUYEN:
                        streamData = _service.ExportExcelDCNTongHop(para);
                        break;
                    case PHUONGTHUCNHAP.NHAPKHAC:
                        streamData = _service.ExportExcelNKhacTongHop(para);
                        break;
                    default:
                        break;
                }
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.PhuongThucNhap)
                {
                    case PHUONGTHUCNHAP.NHANDIEUCHUYEN:
                        switch (para.Option)
                        {
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhomHang:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_TongHop_TheoNhomVatTu.xlsx" };
                                break;
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_TongHop_TheoLoaiVatTu.xlsx" };
                                break;
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.nhaCungCap:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_TongHop_TheoNhaCungCap.xlsx" };
                                break;
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.hangHoa:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_TongHop_TheoVatTu.xlsx" };
                                break;
                            case NvNhapHangMuaVm.TypeGroupInventoryNMua.kho:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_TongHop_TheoKho.xlsx" };
                                break;
                            default:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenNhan_TongHop.xlsx" };
                                break;
                        }
                        break;
                    case PHUONGTHUCNHAP.NHAPKHAC:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCNKhac_TongHop.xlsx" };
                        break;
                }

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception ex)
            {
                WriteLogs.LogError(ex);
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("GetAll_NhapMua")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll_NhapMua()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            DateTime date = (DateTime.Now).AddYears(-1);
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha) && x.NgayCT > date && x.LoaiPhieu == "NMUA").Select(x => new ChoiceObj { Value = x.MaChungTu, Text = x.MaChungTu, Id = x.Id }).ToList();
            return Ok(result);
        }
        [Route("GetNewInstance")]
        public NvNhapHangMuaVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
        [CustomAuthorize(Method = "SUA", State = "phieuNhapHangMua")]
        public async Task<IHttpActionResult> Put(string id, NvNhapHangMuaVm.Dto instance)
        {
            string parentUnitCode = _service.GetParentUnitCode();
            TransferObj<NvVatTuChungTu> result = new TransferObj<NvVatTuChungTu>();
            NvVatTuChungTu check = _service.FindById(instance.Id);
            if (id != instance.Id || check.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            try
            {
                NvVatTuChungTu item = _service.UpdatePhieu(instance);
                if (item != null)
                {
                    if (await _service.UnitOfWork.SaveAsync() > 0)
                    {
                        result.Status = true;
                        result.Data = item;
                        result.Message = "Cập nhật thành công";

                        //update giá trong danh mục hàng hóa
                        if (instance.LstModifield.Count > 0)
                        {
                            foreach (var row in instance.LstModifield)
                            {
                                decimal giaMua, giaMuaCoVat, tyLeLaiLe, tyLeLaiBuon, tyLeVatRa = 0;
                                var price = _servicePrice.Repository.DbSet.FirstOrDefault((x => x.MaVatTu == row.MaHang && x.MaDonVi.StartsWith(parentUnitCode)));
                                if (price != null)
                                {
                                    decimal.TryParse(row.DonGia.ToString(), out giaMua);
                                    decimal.TryParse(row.GiaMuaCoVat.ToString(), out giaMuaCoVat);
                                    decimal.TryParse(row.TyLeLaiLe.ToString(), out tyLeLaiLe);
                                    decimal.TryParse(row.TyLeLaiBuon.ToString(), out tyLeLaiBuon);
                                    decimal.TryParse(row.TyLeVatRa.ToString(), out tyLeVatRa);

                                    price.GiaMua = giaMua;
                                    price.GiaMuaVat = giaMuaCoVat;
                                    price.TyLeLaiLe = 100 * (price.GiaBanLe - giaMua) / giaMua;
                                    price.TyLeLaiBuon = 100 * (price.GiaBanBuon - giaMua) / giaMua;
                                    //price.GiaBanLe = giaMua * tyLeLaiLe / 100 + giaMua;
                                    //price.GiaBanLeVat = price.GiaBanLe * tyLeVatRa / 100 + price.GiaBanLe;
                                    //price.GiaBanBuon = giaMua * tyLeLaiBuon / 100 + giaMua;
                                    //price.GiaBanBuonVat = price.GiaBanBuon * tyLeVatRa / 100 + price.GiaBanBuon;
                                    price.IUpdateBy = _service.GetClaimsPrincipal().Identity.Name;
                                    price.UnitCode = _service.GetCurrentUnitCode();
                                    price.IState = instance.MaChungTu;
                                    price.ObjectState = ObjectState.Modified;
                                    _servicePrice.UnitOfWork.Repository<MdMerchandisePrice>().Update(price);
                                }
                            }
                            if (await _servicePrice.UnitOfWork.SaveAsync() > 0)
                            {
                                result.Message = "Cập nhật thành công";
                            }
                        }
                        //end
                    }
                    else
                    {
                        result.Status = false;
                        result.Data = null;
                        result.Message = "Xảy ra lỗi khi lưu cập nhật";
                        return BadRequest(result.Message);
                    }
                }
                else
                {
                    result.Status = false;
                    result.Data = null;
                    result.Message = "Không tìm thấy dòng chi tiết phiếu";
                    return BadRequest(result.Message);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                return InternalServerError();
            }
        }
        [Route("PostApproval")]
        [CustomAuthorize(Method = "DUYET", State = "phieuNhapHangMua")]
        public async Task<IHttpActionResult> PostApproval(NvVatTuChungTu instance)
        {
            string unitCode = _service.GetCurrentUnitCode();
            NvVatTuChungTu chungTu = _service.FindById(instance.Id);

            if (chungTu == null || chungTu.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            chungTu.TrangThai = (int)ApprovalState.IsComplete;
            chungTu.NgayDuyetPhieu = CurrentSetting.GetNgayKhoaSo(unitCode);
            chungTu.ObjectState = ObjectState.Modified;
            #region For Only ILOVEVN - B&T
            //var _thanhTienCanTra = _serviceCongNo.GetAmmountSupplierLend(chungTu.MaKhachHang).ThanhTienCanTra;
            //string ammountMsg = "Nợ cũ " + Math.Round(_thanhTienCanTra.Value / 1000, 1, MidpointRounding.AwayFromZero) + "k, Thanh toán " + Math.Round(chungTu.TienThanhToan.Value / 1000, 1, MidpointRounding.AwayFromZero)
            //                    + "k, Tổng nợ " + Math.Round((_thanhTienCanTra.Value + chungTu.ThanhTienSauVat.Value - chungTu.TienThanhToan.Value) / 1000, 1, MidpointRounding.AwayFromZero) + "k";
            //chungTu.NoiDung = ammountMsg;
            //chungTu.TienNoCu = _thanhTienCanTra;
            _service.UnitOfWork.Save();
            //if (chungTu.TrangThaiThanhToan == (int)TrangThaiThanhToan.DaThanhToan)
            //{
            //    _InsertPhieuCongNo(chungTu);
            //}
            #endregion  

            switch (_service.Approval(chungTu.Id))
            {
                case StateProcessApproval.NoPeriod:

                    try
                    {
                        _service.UpdateAfterApproval(chungTu);
                        await _service.UnitOfWork.SaveAsync();
                        return Ok(true);
                    }
                    catch (Exception e)
                    {
                        return InternalServerError();
                    }
                case StateProcessApproval.Success:
                    try
                    {
                        _service.UpdateAfterApproval(chungTu);
                        await _service.UnitOfWork.SaveAsync();
                        return Ok(true);
                    }
                    catch (Exception e)
                    {
                        return InternalServerError();
                    }
                case StateProcessApproval.Failed:
                    break;
                default:
                    break;
            }

            return BadRequest("Không thể duyệt phiếu này");
        }
        [CustomAuthorize(Method = "XOA", State = "phieuNhapHangMua")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvVatTuChungTu instance = await _service.Repository.FindAsync(id);
            //var unitCode = _service.GetCurrentUnitCode();

            List<NvVatTuChungTuChiTiet> chitietinstance = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(o => o.MaChungTuPk == instance.MaChungTuPk).ToList();
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                if (_service.DeletePhieu(id))
                {
                    _service.Delete(instance.Id);
                    _service.UnitOfWork.Save();
                    return Ok(instance);
                }
                return InternalServerError();

            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                return InternalServerError();
            }
        }
        public async Task<IHttpActionResult> Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        [Route("GetReport/{id}")]
        public async Task<IHttpActionResult> GetReport(string id)
        {
            var result = new TransferObj<NvNhapHangMuaVm.ReportModel>();
            var data = _service.CreateReport(id);
            if (data != null)
            {
                result.Data = data;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }

        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "phieuNhapHangMua")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            TransferObj<NvNhapHangMuaVm.Dto> result = new TransferObj<NvNhapHangMuaVm.Dto>();
            NvNhapHangMuaVm.Dto temp = new NvNhapHangMuaVm.Dto();
            string _ParentUnitCode = _service.GetParentUnitCode();
            NvVatTuChungTu phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvVatTuChungTu, NvNhapHangMuaVm.Dto>(phieu);
                List<NvVatTuChungTuChiTiet> chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapHangMuaVm.DtoDetail>>(chiTietPhieu.OrderBy(x => x.Index).ToList());
                string unitcode = _service.GetCurrentUnitCode();
                foreach (NvNhapHangMuaVm.DtoDetail dt in temp.DataDetails)
                {
                    MdMerchandisePrice sp =
                        _service.UnitOfWork.Repository<MdMerchandisePrice>()
                            .DbSet.FirstOrDefault(
                                x => x.MaVatTu.Equals(dt.MaHang) && x.MaDonVi.StartsWith(_ParentUnitCode));
                    MdMerchandise item =
                        _service.UnitOfWork.Repository<MdMerchandise>()
                            .DbSet.FirstOrDefault(
                                x => x.MaVatTu.Equals(dt.MaHang) && x.UnitCode.StartsWith(_ParentUnitCode));
                    if (sp != null)
                    {
                        dt.TyLeVatVao = sp.TyLeVatVao;
                        dt.TyLeVatRa = sp.TyLeVatRa;
                        dt.TyLeLaiLe = sp.TyLeLaiLe;
                        dt.TyLeLaiBuon = sp.TyLeLaiBuon;
                        dt.GiaMuaCoVat = sp.GiaMuaVat;
                        dt.GiaBanLeVat = sp.GiaBanLeVat;
                        dt.ThanhTienVAT = dt.ThanhTien * (dt.TyLeVatVao / 100 + 1);
                    }
                    if (item != null)
                    {
                        dt.TenHang = item.TenHang;
                        dt.MaVatRa = item.MaVatRa;
                        dt.MaVatVao = item.MaVatVao;
                    }
                }
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        public static string UnicodetoTCVN_1(string strUnicode)
        {
            int i;
            StringBuilder stringBuilder = new StringBuilder(strUnicode);
            char[] chrArray = new char[] { 'ï', 'ü', 'û', 'þ', 'ó', 'ù', '÷', 'ö', 'õ', 'ø', 'ñ', 'ô', 'î', 'ì', 'ë', 'ê', '«', 'ç', 'æ', 'å', 'è', 'á', 'ä', 'Þ', 'Ø', 'Ö', 'Ô', 'Ó', 'Ò', 'Õ', 'Ï', 'Î', 'Ñ', 'Æ', '\u00BD', '\u00BC', '«', '\u00BE', 'Ë', 'É', 'È', 'Ç', 'Ê', '\u00B6', '\u00B9', '­', '\u00A6', '¬', '¥', 'ò', 'Ü', '\u00AE', '\u00A8', '¡', 'ó', 'ï', 'â', '»', 'ã', 'ß', 'Ý', '×', 'ª', 'Ð', 'Ì', '.', '\u00A9', '¸', 'µ', '¤', '\u00A7', 'Ê', '¢' };
            char[] chrArray1 = new char[] { 'ỹ', 'ỷ', 'ỵ', 'ỳ', 'ự', 'ữ', 'ử', 'ừ', 'ứ', 'ủ', 'ụ', 'ợ', 'ỡ', 'ở', 'ờ', 'ớ', 'ộ', 'ỗ', 'ổ', 'ồ', 'ố', 'ỏ', 'ọ', 'ị', 'ỉ', 'ệ', 'ễ', 'ể', 'ề', 'ế', 'ẽ', 'ẻ', 'ẹ', 'ặ', 'ẵ', 'ẳ', 'ô', 'ắ', 'ậ', 'ẫ', 'ẩ', 'ầ', 'ấ', 'ả', 'ạ', 'ư', 'Ư', 'ơ', 'Ơ', 'ũ', 'ĩ', 'đ', 'ă', 'Ă', 'ú', 'ù', 'õ', 'ằ', 'ó', 'ò', 'í', 'ì', 'ê', 'é', 'è', 'ã', 'â', 'á', 'à', 'Ô', 'Đ', 'Ê', 'Â' };
            for (i = 0; i < (int)chrArray.Length; i++)
            {
                stringBuilder.Replace(chrArray1[i], chrArray[i]);
            }
            string[] strArrays = new string[] { "Aà", "Aả", "Aã", "Aá", "Aạ", "Eè", "Eẻ", "Eẽ", "Eé", "Eẹ", "Iì", "Iỉ", "Iĩ", "Ií", "Iị", "Oò", "Oỏ", "Oõ", "Oó", "Oọ", "Uù", "Uủ", "Uũ", "Uú", "Uụ", "Yỳ", "Yỷ", "Yỹ", "Yý", "Yỵ", "Ăằ", "Ăẳ", "Ăẵ", "Ăắ", "Ăặ", "Âầ", "Âẩ", "Âẫ", "Âấ", "Âậ", "Êề", "Êể", "Êễ", "Êế", "Êệ", "Ôồ", "Ôổ", "Ôỗ", "Ôố", "Ôộ", "Ơờ", "Ơở", "Ơỡ", "Ơớ", "Ơợ", "Ưừ", "Ưử", "Ưữ", "Ưứ", "Ưự" };
            string[] strArrays1 = strArrays;
            strArrays = new string[] { "À", "Ả", "Ã", "Á", "Ạ", "È", "Ẻ", "Ẽ", "É", "Ẹ", "Ì", "Ỉ", "Ĩ", "Í", "Ị", "Ò", "Ỏ", "Õ", "Ó", "Ọ", "Ù", "Ủ", "Ũ", "Ú", "Ụ", "Ỳ", "Ỷ", "Ỹ", "Ý", "Ỵ", "Ằ", "Ẳ", "Ẵ", "Ắ", "Ặ", "Ầ", "Ẩ", "Ẫ", "Ấ", "Ậ", "Ề", "Ể", "Ễ", "Ế", "Ệ", "Ồ", "Ổ", "Ỗ", "Ố", "Ộ", "Ờ", "Ở", "Ỡ", "Ớ", "Ợ", "Ừ", "Ử", "Ữ", "Ứ", "Ự" };
            string[] strArrays2 = strArrays;
            for (i = 0; i < (int)strArrays1.Length; i++)
            {
                stringBuilder.Replace(strArrays2[i], strArrays1[i]);
            }
            return stringBuilder.ToString();
        }
        public String UnicodetoTCVN_2(String strUnicode)
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
        private bool IsExists(string id)
        {
            return _service.Repository.Find(id) != null;
        }
        private async void _InsertPhieuCongNo(NvVatTuChungTu chungTu)
        {
            //Cos nhieu phieu trong ngay phat sinh no.
            var unitCode = _service.GetCurrentUnitCode();
            var _thanhTienCanTra = _serviceCongNo.GetAmmountSupplierLend(chungTu.MaKhachHang).ThanhTienCanTra;
            _serviceCongNo.InsertPhieu(new NvCongNoVm.Dto()
            {
                Id = Guid.NewGuid().ToString(),
                LoaiChungTu = LoaiCongNo.CNNCC.ToString(),
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
                MaNhaCungCap = chungTu.MaKhachHang,
                GhiChu = "[" + chungTu.MaChungTu + "]",
                ThanhTien = chungTu.ThanhTienSauVat,
                ThanhTienCanTra = _thanhTienCanTra
            });
            _serviceCongNo.UnitOfWork.Save();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Repository.DataContext.Dispose();
            }
            base.Dispose(disposing);
        }
        [Route("GetInfoItemDetails/{id}")]
        public async Task<IHttpActionResult> GetInfoItemDetails(string id)
        {
            TransferObj<NvNhapHangMuaVm.Dto> result = new TransferObj<NvNhapHangMuaVm.Dto>();
            NvNhapHangMuaVm.Dto temp = new NvNhapHangMuaVm.Dto();
            string _ParentUnitCode = _service.GetParentUnitCode();
            NvVatTuChungTu phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvVatTuChungTu, NvNhapHangMuaVm.Dto>(phieu);
                List<NvVatTuChungTuChiTiet> chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapHangMuaVm.DtoDetail>>(chiTietPhieu.OrderBy(x => x.Index).ToList());
                string unitcode = _service.GetCurrentUnitCode();
                foreach (NvNhapHangMuaVm.DtoDetail dt in temp.DataDetails)
                {
                    MdMerchandisePrice sp = _service.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu.Equals(dt.MaHang) && x.MaDonVi.Equals(unitcode)).FirstOrDefault();
                    MdMerchandise item = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu.Equals(dt.MaHang) && x.UnitCode.StartsWith(_ParentUnitCode));

                    if (sp != null)
                    {
                        dt.GiaBanLe = sp.GiaBanLeVat;
                    }
                    if (item != null) dt.TenHang = item.TenHang;

                }
                temp.CalcResult();
                result.Data = temp;

                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            var unitCode = _service.GetCurrentUnitCode();
            var currentDate = DateTime.Now.Date;
            var datelock = CurrentSetting.GetNgayKhoaSo(unitCode);
            var result = new NvNhapHangMuaVm.ParameterNMua()
            {
                ToDate = datelock,
                FromDate = datelock,
                MaxDate = currentDate,
                UnitCode = unitCode,
                Option = NvNhapHangMuaVm.TypeGroupInventoryNMua.loaiHang,
                PhuongThucNhap = PHUONGTHUCNHAP.NHAPMUA
            };
            return Ok(result);
        }

    }
}
