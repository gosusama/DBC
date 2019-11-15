using BTS.API.ENTITY;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.NV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.MD;
using System.Net.Http.Headers;
using AutoMapper;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.SP.API.Utils;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/KiemKe")]
    [Route("{id?}")]
    [Authorize]
    public class KiemKeController : ApiController
    {
        private readonly IMdMerchandiseService _service;
        private readonly INvKiemKeService _serviceKK;
        public KiemKeController(IMdMerchandiseService service, INvKiemKeService serviceKK)
        {
            _service = service;
            _serviceKK = serviceKK;
        }
        //lấy những mã chưa kiểm kê,là những mã có tồn nhưng không nằm trong phiếu kiểm kê nào
        [Route("GetExternalCode")]
        [HttpPost]
        public async Task<IHttpActionResult> GetExternalCode(ParameterKiemKe para)
        {
            string unitCode = _service.GetCurrentUnitCode();
            MdPeriod curentDate = CurrentSetting.GetKhoaSo(unitCode);
            if (para.WareHouseCodes == null) para.WareHouseCodes = unitCode + "-K2";
            string tableName = curentDate.GetTableName();
            List<NvKiemKeVm.ExternalCodeInInventory> result = new List<NvKiemKeVm.ExternalCodeInInventory>();
            result = ProcedureCollection.GetExternalCodeInventory(unitCode, tableName, para);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("GetDataKiemKe/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDataKiemKe(string id)
        {
            string para = id.Substring(0, 36);
            string typeKiemKe = id.Substring(37);
            var result = new TransferObj<NvKiemKeVm.Dto>();
            var temp = new NvKiemKeVm.Dto();
            var phieu = _serviceKK.FindById(para);
            var chiTietPhieu = new List<NvKiemKeChiTiet>();
            if (phieu != null)
            {
                temp = Mapper.Map<NvKiemKe, NvKiemKeVm.Dto>(phieu);
                //thua
                if (typeKiemKe == "1")
                {
                    chiTietPhieu = _service.UnitOfWork.Repository<NvKiemKeChiTiet>().DbSet.Where(x => x.MaPhieuKiemKe == phieu.MaPhieuKiemKe).Where(x => x.SoLuongKiemKe == x.SoLuongTonMay).ToList();
                    temp.SoPhieuKiemKe = "1";
                }
                else if (typeKiemKe == "2")
                {
                    chiTietPhieu = _service.UnitOfWork.Repository<NvKiemKeChiTiet>().DbSet.Where(x => x.MaPhieuKiemKe == phieu.MaPhieuKiemKe).Where(x => x.SoLuongKiemKe > x.SoLuongTonMay).ToList();
                    temp.SoPhieuKiemKe = "2";
                }
                else
                {
                    chiTietPhieu = _service.UnitOfWork.Repository<NvKiemKeChiTiet>().DbSet.Where(x => x.MaPhieuKiemKe == phieu.MaPhieuKiemKe).Where(x => x.SoLuongKiemKe < x.SoLuongTonMay).ToList();
                    temp.SoPhieuKiemKe = "3";
                }
                //thieu
                temp.DataDetails = Mapper.Map<List<NvKiemKeChiTiet>, List<NvKiemKeVm.DtoDetails>>(chiTietPhieu);
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }


        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "nvKiemKe")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            TransferObj<NvKiemKeVm.Dto> result = new TransferObj<NvKiemKeVm.Dto>();
            NvKiemKeVm.Dto temp = new NvKiemKeVm.Dto();
            NvKiemKe phieu = _serviceKK.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvKiemKe, NvKiemKeVm.Dto>(phieu);
                List<NvKiemKeChiTiet> chiTietPhieu = _service.UnitOfWork.Repository<NvKiemKeChiTiet>().DbSet.Where(x => x.MaPhieuKiemKe == phieu.MaPhieuKiemKe).ToList();
                temp.DataDetails = Mapper.Map<List<NvKiemKeChiTiet>, List<NvKiemKeVm.DtoDetails>>(chiTietPhieu);
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }

        [Route("GetInfoMerchandiseByCode/{code}")]
        public async Task<IHttpActionResult> GetInfoMerchandiseByCode(string code)
        {
            string id = code.Substring(0, 36);
            string maVatTu = code.Substring(37);
            var result = new TransferObj<NvKiemKeVm.Dto>();
            var temp = new NvKiemKeVm.Dto();
            var phieu = _serviceKK.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvKiemKe, NvKiemKeVm.Dto>(phieu);
                var chiTietPhieu = _service.UnitOfWork.Repository<NvKiemKeChiTiet>().DbSet.Where(x => x.MaPhieuKiemKe == phieu.MaPhieuKiemKe).Where(x => x.MaVatTu.Contains(maVatTu)).ToList();
                temp.DataDetails = Mapper.Map<List<NvKiemKeChiTiet>, List<NvKiemKeVm.DtoDetails>>(chiTietPhieu);
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "nvKiemKe")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            TransferObj<PagedObj<NvKiemKeVm.Dto>> result = new TransferObj<PagedObj<NvKiemKeVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvKiemKeVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKiemKeVm.Search>>();
            filtered.OrderType = "DESC";
            filtered.OrderBy = "NgayKiemKe";
            PagedObj<NvKiemKe> paged = ((JObject)postData.paged).ToObject<PagedObj<NvKiemKe>>();
            string unitCode = _serviceKK.GetCurrentUnitCode();
            QueryBuilder query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new NvKiemKe().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                },
            };
            try
            {
                ResultObj<PagedObj<NvKiemKe>> filterResult = _serviceKK.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = Mapper.Map<PagedObj<NvKiemKe>, PagedObj<NvKiemKeVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("Post")]
        [CustomAuthorize(Method = "THEM", State = "nvKiemKe")]
        public async Task<IHttpActionResult> Post(NvKiemKeVm.Dto instance)
        {
            TransferObj<NvKiemKe> result = new TransferObj<NvKiemKe>();
            try
            {
                NvKiemKe item = _serviceKK.InsertPhieu(instance);
                await _serviceKK.UnitOfWork.SaveAsync();
                result.Data = item;
                result.Status = true;
                return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.MaPhieuKiemKe }, result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [CustomAuthorize(Method = "SUA", State = "nvKiemKe")]
        public async Task<IHttpActionResult> Put(string id, NvKiemKeVm.Dto instance)
        {
            //Lấy danh sách các mã không thuộc kệ để update lại mã kệ cho các mã đó
            List<NvKiemKeVm.DtoDetails> hangKhongThuocKe = new List<NvKiemKeVm.DtoDetails>();
            foreach (NvKiemKeVm.DtoDetails data in instance.DataDetails)
            {
                if (data.KeKiemKe == instance.KeKiemKe)
                {

                }
                else
                {
                    hangKhongThuocKe.Add(data);
                }
            }

            //Update mã kệ trong danh mục hàng hóa
            foreach (NvKiemKeVm.DtoDetails value in hangKhongThuocKe)
            {
                MdMerchandise maHangHoa = _service.Repository.DbSet.FirstOrDefault(x => x.MaVatTu == value.MaVatTu);
                if (maHangHoa != null)
                {
                    maHangHoa.MaKeHang = instance.KeKiemKe;
                    _service.Update(maHangHoa);
                }
            }
            await _service.UnitOfWork.SaveAsync();

            //end cập nhật mã kệ

            TransferObj<NvKiemKe> result = new TransferObj<NvKiemKe>();
            NvKiemKe check = _serviceKK.FindById(instance.Id);
            if (id != instance.Id || check.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            try
            {
                NvKiemKe item = _serviceKK.UpdatePhieu(instance);
                await _serviceKK.UnitOfWork.SaveAsync();
                result.Status = true;
                result.Data = item;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("UploadFile")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> UploadFile()
        {
            var ctx = new ERPContext();
            var result = new List<MdMerchandise>();
            string path = HttpContext.Current.Server.MapPath("/Upload/KiemKe/");
            //string path = @"C:/inetpub/wwwroot/wss/PHAMTUANANH_API/Upload/KiemKe/";
            HttpRequest request = HttpContext.Current.Request;
            if (request.Files.Count > 0)
            {
                try
                {

                    HttpPostedFile file = request.Files[0];
                    file.SaveAs(path + file.FileName);
                    //kiem tra ton tai file
                    if (File.Exists(path + file.FileName))
                    {
                        if (file.FileName.Contains(".txt"))
                        {
                            //doc du lieu tu file text
                            string[] lines = System.IO.File.ReadAllLines(path + file.FileName);
                            foreach (string line in lines)
                            {
                                string[] data = line.Split(',');
                                string MaVatTu = data[0].ToString();
                                string MaKeKiemKem = data[1].ToString();
                                var vatTu = ctx.MdMerchandises.Where(x => x.MaVatTu == MaVatTu).FirstOrDefault();
                                //vatTu.KeKiemKe = MaKeKiemKem;
                                result.Add(vatTu);
                            }
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError();
                }
            }
            return Ok(result);
        }
        [ResponseType(typeof(NvKiemKe))]
        [CustomAuthorize(Method = "XOA", State = "nvKiemKe")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvKiemKe instance = await _serviceKK.Repository.FindAsync(id);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                _serviceKK.DeletePhieu(instance.Id);
                await _serviceKK.UnitOfWork.SaveAsync();
                return Ok(instance);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        
        [Route("ReceiveDataKiemKe/{makho}")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> ReceiveDataKiemKe(string MaKho)
        {
            string UNITCODE_GLOBAL = "";
            NvKiemKeVm.Dto returnList = new NvKiemKeVm.Dto();
            if (!string.IsNullOrEmpty(MaKho))
            {
                UNITCODE_GLOBAL = MaKho.Substring(0, MaKho.LastIndexOf('-'));
                NvKiemKeVm.Dto lst = new NvKiemKeVm.Dto();
                List<string> vatTu = new List<string>();
                string path = HttpContext.Current.Server.MapPath("/Upload/KiemKe/");
                HttpRequest request = HttpContext.Current.Request;
                if (request.Files.Count > 0)
                {
                    try
                    {
                        HttpPostedFile file = request.Files[0];
                        file.SaveAs(path + file.FileName);
                        //kiem tra ton tai file
                        if (File.Exists(path + file.FileName))
                        {
                            if (file.FileName.Contains(".txt"))
                            {
                                //doc du lieu tu file text
                                string[] lines = File.ReadAllLines(path + file.FileName);
                                foreach (string line in lines)
                                {
                                    string[] data = line.Split(',');
                                    string MaVatTu = data[0].ToString();
                                    string SoTonKiemKe = data[1].ToString();
                                    vatTu.Add(MaVatTu);
                                }
                                string[] arrayVatTu = vatTu.ToArray();
                                string paraVatTu = String.Join(",", arrayVatTu);
                                lst.DataDetails = ProcedureCollection.GetInventoryForActionInventoryByOne(UNITCODE_GLOBAL, MaKho,paraVatTu);
                                foreach (var value in lst.DataDetails)
                                {
                                    foreach (string line in lines)
                                    {
                                        string[] data = line.Split(',');
                                        if (data[0].ToString() == value.MaVatTu)
                                        {
                                            decimal SoLuongKiemKe = 0;
                                            decimal.TryParse(data[1].ToString(), out SoLuongKiemKe);
                                            value.SoLuongKiemKe = SoLuongKiemKe;
                                            returnList.DataDetails.Add(value);
                                        }

                                    }
                                }
                                returnList.KhoKiemKe = MaKho;
                                returnList.NgayKiemKe = DateTime.Now.Date;
                                returnList.NgayIn = DateTime.Now.Date;
                                int number = 0;
                                string maKe = "";
                                if (lst.DataDetails.Count > 10)
                                {

                                    for (int i = 0; i < 10; i++)
                                    {
                                        if (lst.DataDetails[i].KeKiemKe == lst.DataDetails[i + 1].KeKiemKe)
                                        {
                                            number++;
                                            maKe = lst.DataDetails[i].KeKiemKe;
                                        }
                                    }
                                }
                                if (number > 5)
                                {
                                    returnList.KeKiemKe = maKe;
                                }
                                else
                                {
                                    returnList.KeKiemKe = lst.DataDetails[0].KeKiemKe;
                                }
                                returnList.MaPhieuKiemKe = _serviceKK.BuildCode();
                            }

                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLogs.LogError(ex);
                        return InternalServerError();
                    }
                }
                //lấy những mã không kiểm kê nhưng nằm trong kệ 
                string maKeHangHoa = returnList.KeKiemKe;
                var hangTrongKe = ProcedureCollection.LayThongTinHangThuocKe(UNITCODE_GLOBAL, returnList.KhoKiemKe,maKeHangHoa);
                try
                {
                    if (hangTrongKe != null && hangTrongKe.Count > 0)
                    {
                        for (int i = 0; i < hangTrongKe.Count; i++)
                        {
                            var data = returnList.DataDetails.Find(x => x.MaVatTu == hangTrongKe[i].MaVatTu);
                            if (data == null)
                            {
                                hangTrongKe[i].SoLuongKiemKe = 0;
                                returnList.DataDetails.Add(hangTrongKe[i]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLogs.LogError(ex);
                }
            }
            else
            {
                return NotFound();
            }
            return Ok(returnList);
        }

        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            var unitCode = _serviceKK.GetCurrentUnitCode();
            var currentDate = DateTime.Now.Date;
            var datelock = CurrentSetting.GetNgayKhoaSo(unitCode);
            var result = new ParameterKiemKe()
            {
                ToDate = datelock,
                FromDate = datelock,
                MaxDate = currentDate,
                UnitCode = unitCode,
                GroupBy = TypeGroupKiemKe.WAREHOUSE,
                ReportType = TypeReportKiemKe.BAOCAODAYDU
            };
            return Ok(result);
        }

        [Route("PostReportTongHop")]
        public async Task<IHttpActionResult> PostReportTongHop(ParameterKiemKe para)
        {
            NvKiemKeVm.ReportTongHop reporter = new NvKiemKeVm.ReportTongHop();
            List<NvKiemKeVm.ObjectReport> result = new List<NvKiemKeVm.ObjectReport>();
            try
            {
                var unitCode = _serviceKK.GetCurrentUnitCode();
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
                switch (para.GroupBy)
                {
                    case TypeGroupKiemKe.WAREHOUSE:
                        reporter.NameGroupBy = "Kho hàng";
                        break;
                    case TypeGroupKiemKe.TYPE:
                        reporter.NameGroupBy = "Loại vật tư";
                        break;
                    case TypeGroupKiemKe.GROUP:
                        reporter.NameGroupBy = "Nhóm vật tư";
                        break;
                    case TypeGroupKiemKe.MERCHANDISE:
                        reporter.NameGroupBy = "Vật tư";
                        break;
                    case TypeGroupKiemKe.NHACUNGCAP:
                        reporter.NameGroupBy = "Nhà cung cấp";
                        break;
                    case TypeGroupKiemKe.KEHANG:
                        reporter.NameGroupBy = "Kệ hàng";
                        break;
                    default:
                        reporter.NameGroupBy = "Vật tư";
                        break;
                }
                //GetNhanVien
                if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
                {
                    var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                    var name = currentUser.Identity.Name;
                    var nhanVien = _serviceKK.UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                    if (nhanVien != null)
                    {
                        reporter.Username = nhanVien.TenNhanVien;
                    }
                    else
                    {
                        reporter.Username = "Administrator";
                    }
                }
                reporter.DataDetails.AddRange(_serviceKK.ReportKiemKe(para));
            }
            catch (Exception ex)
            {

            }
            return Ok(reporter);
        }
        [Route("PostExportExcelTongHop")]
        public HttpResponseMessage PostExportExcelTongHop(ParameterKiemKe para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _serviceKK.ExportExcelTongHop(para);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.GroupBy)
                {
                    case TypeGroupKiemKe.GROUP:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCTongHop_TheoNhomVatTu.xlsx" };
                        break;
                    case TypeGroupKiemKe.TYPE:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCTongHop_TheoLoaiVatTu.xlsx" };
                        break;
                    case TypeGroupKiemKe.NHACUNGCAP:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCTongHop_TheoNhaCungCap.xlsx" };
                        break;
                    case TypeGroupKiemKe.MERCHANDISE:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCTongHop_TheoVatTu.xlsx" };
                        break;
                    case TypeGroupKiemKe.WAREHOUSE:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCTongHop_TheoKho.xlsx" };
                        break;
                    case TypeGroupKiemKe.KEHANG:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCTongHop_TheoKe.xlsx" };
                        break;
                    default:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_TongHop.xlsx" };
                        break;
                }

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcelChiTiet")]
        public HttpResponseMessage PostExportExcelChiTiet(ParameterKiemKe para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _serviceKK.ExportExcelDetail(para);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                        switch (para.GroupBy)
                        {
                            case TypeGroupKiemKe.GROUP:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCChiTiet_TheoNhomVatTu.xlsx" };
                                break;
                            case TypeGroupKiemKe.TYPE:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCChiTiet_TheoLoaiVatTu.xlsx" };
                                break;
                            case TypeGroupKiemKe.NHACUNGCAP:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCChiTiet_TheoNhaCungCap.xlsx" };
                                break;
                            case TypeGroupKiemKe.MERCHANDISE:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCChiTiet_TheoVatTu.xlsx" };
                                break;
                            case TypeGroupKiemKe.WAREHOUSE:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCChiTiet_TheoKho.xlsx" };
                                break;
                           case TypeGroupKiemKe.KEHANG:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCChiTiet_TheoKe.xlsx" };
                                break;
                            default:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCChiTiet.xlsx" };
                                break;
                        }
                        
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("GetMerchandiseByCodeKK/{maHang}")]
        public async Task<IHttpActionResult> GetMerchandiseByCodeKK(string MaHang)
        {
            var unitCode = _service.GetCurrentUnitCode();
            string MaKho = unitCode +"-K2";
            NvKiemKeVm.Dto lst = new NvKiemKeVm.Dto();
            try
            {
                String[] strArray = new String[] { MaHang };
                string paraVatTu = String.Join(",", strArray);
                lst.DataDetails = ProcedureCollection.GetInventoryForActionInventoryByOne(unitCode, MaKho, paraVatTu);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return Ok(lst);
        }

        //        [Route("PostComplete")]
        //        [ResponseType(typeof(bool))]
        //        public async Task<IHttpActionResult> PostComplete(NvKiemKeVm.Dto instance)
        //        {
        //            var unitCode = _service.GetCurrentUnitCode();
        //            var curentDate = CurrentSetting.GetKhoaSo(unitCode);
        //            var tablleName = curentDate.GetTableName();
        //            var phieuKiemKe = _serviceKK.FindById(instance.Id);
        //            if (phieuKiemKe == null || phieuKiemKe.TrangThai == (int)OrderState.IsComplete)
        //            {
        //                return NotFound();
        //            }
        //            instance.TrangThai = 10;
        //            var details = _serviceKK.UnitOfWork.Repository<NvKiemKeChiTiet>()
        //                        .DbSet.Where(x => x.MaPhieuKiemKe == phieuKiemKe.MaPhieuKiemKe).ToList();
        //            string KhoKiemKe = phieuKiemKe.KhoKiemKe;
        //            using (var ctx = new ERPContext())
        //            {
        //                foreach (var item in instance.DataDetails)
        //                {
        //                    var itemNew = details.FirstOrDefault(u => u.Id == item.Id);
        //                    var getDataXNT = string.Format("SELECT TONCUOIKYSL as TonCuoiKySL, GIAVON as GiaVon FROM {0} WHERE MAVATTU = '{1}' AND MAKHO='{2}' AND NAM='{3}'", tablleName, itemNew.MaVatTu, KhoKiemKe, DateTime.Now.Year);
        //                    var dataXNT = ctx.Database.SqlQuery<MdMerchandiseVm.DataXNT>(getDataXNT).ToList();
        //                    decimal TonCuoiKySL_New, TonCuoiKySL, SoLuongChenhLech, GiaVon, TonCuoiKyGiaTri_New = 0;
        //                    decimal.TryParse(dataXNT[0].TonCuoiKySL.ToString(), out TonCuoiKySL);
        //                    decimal.TryParse(dataXNT[0].GiaVon.ToString(), out GiaVon);
        //                    decimal.TryParse(itemNew.SoLuongChenhLech.ToString(), out SoLuongChenhLech);
        //                    var queryStr = "";
        //                    if (itemNew.SoLuongChenhLech <= 0)
        //                    {
        //                        TonCuoiKySL_New = TonCuoiKySL - SoLuongChenhLech;
        //                        TonCuoiKyGiaTri_New = TonCuoiKySL_New * GiaVon;
        //                        queryStr = string.Format(@"UPDATE {0} SET GiaVon = {1},TONCUOIKYSL = {2}, TONCUOIKYGT = {3}    
        //                                        WHERE MAVATTU = '{4}' AND MAKHO='{5}' AND NAM='{6}'", tablleName, itemNew.GiaVon, TonCuoiKySL_New, TonCuoiKyGiaTri_New, itemNew.MaVatTu, KhoKiemKe, DateTime.Now.Year);
        //                    }
        //                    else
        //                    {
        //                        TonCuoiKySL_New = TonCuoiKySL - SoLuongChenhLech;
        //                        TonCuoiKyGiaTri_New = TonCuoiKySL_New * GiaVon;
        //                        queryStr = string.Format(@"UPDATE {0} SET GiaVon = {1},TONCUOIKYSL = {2}, TONCUOIKYGT = {3}    
        //                                        WHERE MAVATTU = '{4}' AND MAKHO='{5}' AND NAM='{6}'", tablleName, itemNew.GiaVon, TonCuoiKySL_New, TonCuoiKyGiaTri_New, itemNew.MaVatTu, KhoKiemKe, DateTime.Now.Year);
        //                    }
        //                    try
        //                    {
        //                        ctx.Database.ExecuteSqlCommand(queryStr);
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        return InternalServerError();
        //                    }
        //                }
        //            }
        //            try
        //            {
        //                _serviceKK.UpdateApproval(instance);
        //                _serviceKK.UnitOfWork.Save();
        //                return Ok(true);
        //            }
        //            catch (Exception)
        //            {
        //                return InternalServerError();
        //            }
        //        }


        [Route("PostComplete")]
        [ResponseType(typeof(bool))]
        [CustomAuthorize(Method = "DUYET", State = "nvKiemKe")]
        public async Task<IHttpActionResult> PostComplete(NvKiemKeVm.Dto instance)
        {
            string unitCode = _service.GetCurrentUnitCode();
            MdPeriod curentDate = CurrentSetting.GetKhoaSo(unitCode);
            int period = curentDate.Period;
            int year = curentDate.Year;
            string tablleName = curentDate.GetTableName();
            NvKiemKe phieuKiemKe = _serviceKK.FindById(instance.Id);
            instance.NgayDuyetPhieu = curentDate.ToDate;
            //thực hiện tạo phiếu kiểm kê nhập, kiểm kê xuất
            try
            {
                _serviceKK.Approval(instance, tablleName, year.ToString(), period);
                _serviceKK.UpdateApproval(instance);
                _serviceKK.UnitOfWork.Save();
                return Ok(true);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("PostExportExcel")]
        public HttpResponseMessage PostExportExcel(List<NvKiemKeVm.ExternalCodeInInventory> para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _serviceKK.ExportExcelExternalCodeInvertory(para);
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "HangChuaKiemKe.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
    }
}
