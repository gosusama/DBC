﻿using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.NV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize.Utils;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/KhuyenMaiBuy1Get1")]
    [Route("{id?}")]
    [Authorize]
    public class KhuyenMaiBuy1Get1Controller : ApiController
    {
        private readonly INvKhuyenMaiBuy1Get1Service _service;

        public KhuyenMaiBuy1Get1Controller(INvKhuyenMaiBuy1Get1Service service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            return data.Where(x => x.UnitCode == unitCode).Select(x => new ChoiceObj() { Value = x.MaChuongTrinh, Text = x.NoiDung, Id = x.Id }).ToList();
        }


        [Route("TemplateExcel_KhuyenMaiMua1Tang1")]
        public HttpResponseMessage TemplateExcel_KhuyenMaiMua1Tang1()
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _service.TemplateExcelKhuyenMaiMua1Tang1();
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "TemplateExcel-KhuyenMaiChietKhau-HangHoa.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("ImportExcelMua1Tang1/{unitCode}")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ImportExcelMua1Tang1(string unitCode)
        {
            string parent = "";
            var result = new TransferObj();
            if (!string.IsNullOrEmpty(unitCode))
            {
                var parentUnitCode = _service.UnitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == unitCode);
                if (parentUnitCode != null) parent = parentUnitCode.MaDonViCha;
            }
            string path = _service.GetPhysicalPathImportFile();
            //string path = @"C:/inetpub/wwwroot/wss/VirtualDirectories/BANHANG/Upload/DataHangHoa/";
            HttpRequest request = HttpContext.Current.Request;
            try
            {
                if (request.Files.Count > 0)
                {
                    List<NvChuongTrinhKhuyenMaiVm.ObjImportExcel> lstImportExcel = new List<NvChuongTrinhKhuyenMaiVm.ObjImportExcel>();
                    HttpPostedFile file = request.Files[0];
                    file.SaveAs(path + file.FileName);
                    if (File.Exists(path + file.FileName))
                    {
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
                            decimal soThuTu = 0;
                            NvChuongTrinhKhuyenMaiVm.ObjImportExcel record = new NvChuongTrinhKhuyenMaiVm.ObjImportExcel();
                            int cCnt = 1; //bỏ qua ô số thứ tự
                            string SoThuTu = (range.Cells[rCnt, cCnt] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                            decimal.TryParse(SoThuTu, out soThuTu);
                            record.SoThuTu = soThuTu;
                            record.MaHang = string.IsNullOrEmpty((string)(range.Cells[rCnt, cCnt + 1] as Microsoft.Office.Interop.Excel.Range).Value2) ? "" : (string)(range.Cells[rCnt, cCnt + 1] as Microsoft.Office.Interop.Excel.Range).Value2;
                            var mMerchandise = _service.UnitOfWork.Repository<MdMerchandise>()
                                    .DbSet.FirstOrDefault(
                                        x => x.MaVatTu == record.MaHang && x.UnitCode.StartsWith(parent));
                            string nameMerchandise = (string)(range.Cells[rCnt, cCnt + 2] as Microsoft.Office.Interop.Excel.Range).Value2;
                            if (string.IsNullOrEmpty(nameMerchandise))
                            {
                                if (mMerchandise != null) record.TenHang = mMerchandise.TenHang;
                            }
                            else
                            {
                                record.TenHang = nameMerchandise;
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
                result.Message = "Xảy ra lỗi !";
                result.Data = null;
                result.Status = false;
                return Ok(result);
            }
            return Ok(result);
        }
        [Route("PostSelectData")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<ChoiceObj>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKhuyenMaiBuy1Get1Vm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvChuongTrinhKhuyenMai>>();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<NvChuongTrinhKhuyenMai>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "nvKMBuy1Get1")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            TransferObj<PagedObj<NvKhuyenMaiBuy1Get1Vm.Dto>> result = new TransferObj<PagedObj<NvKhuyenMaiBuy1Get1Vm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvKhuyenMaiBuy1Get1Vm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKhuyenMaiBuy1Get1Vm.Search>>();
            PagedObj<NvChuongTrinhKhuyenMai> paged = ((JObject)postData.paged).ToObject<PagedObj<NvChuongTrinhKhuyenMai>>();
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
                            Property = ClassHelper.GetProperty(() => new NvChuongTrinhKhuyenMai().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvChuongTrinhKhuyenMai().LoaiKhuyenMai),
                            Method = FilterMethod.EqualTo,
                            Value = LoaiKhuyenMai.Buy1Get1
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvChuongTrinhKhuyenMai().ICreateDate),
                        Method = OrderMethod.DESC

                    }
                }
            };
            try
            {
                ResultObj<PagedObj<NvChuongTrinhKhuyenMai>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<NvChuongTrinhKhuyenMai>, PagedObj<NvKhuyenMaiBuy1Get1Vm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [ResponseType(typeof(NvChuongTrinhKhuyenMai))]
        [CustomAuthorize(Method = "THEM", State = "nvKMBuy1Get1")]
        public async Task<IHttpActionResult> Post(NvKhuyenMaiBuy1Get1Vm.Dto instance)
        {
            TransferObj<NvChuongTrinhKhuyenMai> result = new TransferObj<NvChuongTrinhKhuyenMai>();
            try
            {
                NvChuongTrinhKhuyenMai item = _service.InsertPhieu(instance);
                _service.UnitOfWork.Save();
                result.Data = item;
                result.Status = true;
                return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }

        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "nvKMBuy1Get1")]
        public async Task<IHttpActionResult> Put(string id, NvKhuyenMaiBuy1Get1Vm.Dto instance)
        {
            TransferObj<NvChuongTrinhKhuyenMai> result = new TransferObj<NvChuongTrinhKhuyenMai>();
            NvChuongTrinhKhuyenMai check = _service.FindById(instance.Id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (check == null)
            {
                return NotFound();
            }
            try
            {
                NvChuongTrinhKhuyenMai item = _service.UpdatePhieu(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Data = item;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [ResponseType(typeof(NvChuongTrinhKhuyenMai))]
        [CustomAuthorize(Method = "XOA", State = "nvKMBuy1Get1")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvChuongTrinhKhuyenMai instance = await _service.Repository.FindAsync(id);
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
        public async Task<IHttpActionResult> Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        [Route("PostPrintDetail")]
        public async Task<IHttpActionResult> PostPrintDetail(JObject jsonData)
        {
            var result = new List<NvKhuyenMaiBuy1Get1Vm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKhuyenMaiBuy1Get1Vm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvChuongTrinhKhuyenMai>>();
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
                            Method = FilterMethod.EqualTo,
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvChuongTrinhKhuyenMai().UnitCode),
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
                        Field = ClassHelper.GetPropertyName(() => new NvChuongTrinhKhuyenMai().ICreateDate),
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

                result = Mapper.Map<List<NvChuongTrinhKhuyenMai>, List<NvKhuyenMaiBuy1Get1Vm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().DbSet.Where(u => u.MaChuongTrinh == x.MaChuongTrinh);
                        x.DataDetails = Mapper.Map<List<NvChuongTrinhKhuyenMaiChiTiet>, List<NvKhuyenMaiBuy1Get1Vm.DtoDetail>>(details.ToList());
                    }
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [Route("PostApproval/{id}")]
        [CustomAuthorize(Method = "DUYET", State = "nvKMBuy1Get1")]
        public async Task<IHttpActionResult> PostApproval(string id)
        {
            string unitCode = _service.GetCurrentUnitCode();
            NvChuongTrinhKhuyenMai chuongTrinh = _service.FindById(id);

            if (chuongTrinh == null || chuongTrinh.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            chuongTrinh.TrangThai = (int)ApprovalState.IsComplete;
            chuongTrinh.ObjectState = ObjectState.Modified;
            await _service.UnitOfWork.SaveAsync();
            return Ok(true);
        }
        [Route("PostUnApprove/{id}")]
        [CustomAuthorize(Method = "DUYET", State = "nvKMBuy1Get1")]
        public async Task<IHttpActionResult> PostUnApprove(string id)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var chuongTrinh = _service.FindById(id);

            if (chuongTrinh == null || chuongTrinh.TrangThai == (int)ApprovalState.IsNotApproval)
            {
                return NotFound();
            }
            chuongTrinh.TrangThai = (int)ApprovalState.IsNotApproval;
            chuongTrinh.ObjectState = ObjectState.Modified;
            await _service.UnitOfWork.SaveAsync();
            return Ok(true);
        }
        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "nvKMBuy1Get1")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            var result = new TransferObj<NvKhuyenMaiBuy1Get1Vm.Dto>();
            var temp = new NvKhuyenMaiBuy1Get1Vm.Dto();

            var phieu = _service.FindById(id);

            if (phieu != null)
            {
                temp = Mapper.Map<NvChuongTrinhKhuyenMai, NvKhuyenMaiBuy1Get1Vm.Dto>(phieu);
                var tb_ChuongTrinhKhuyenMaiChiTiet = _service.UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().DbSet;

                var chiTietChuongTrinhKhuyenMai = tb_ChuongTrinhKhuyenMaiChiTiet.Where(x => x.MaChuongTrinh == phieu.MaChuongTrinh).ToList();
                temp.DataDetails = Mapper.Map<List<NvChuongTrinhKhuyenMaiChiTiet>, List<NvKhuyenMaiBuy1Get1Vm.DtoDetail>>(chiTietChuongTrinhKhuyenMai);
                //var dataGift = _service.UnitOfWork.Repository<NvChuongTrinhKhuyenMaiHangKM>().DbSet.Where(x => x.MaChuongTrinh == phieu.MaChuongTrinh).ToList();
                //temp.DataGifts = Mapper.Map<List<NvChuongTrinhKhuyenMaiHangKM>, List<NvKhuyenMaiBuy1Get1Vm.DtoDetail>>(dataGift);
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("GetNewInstance")]
        public NvKhuyenMaiBuy1Get1Vm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();

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
