using AutoMapper;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.NV;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/GiaoDichQuay")]
    [Route("{id?}")]
    [Authorize]
    public class GiaoDichQuayController : ApiController
    {
        private readonly INvGiaoDichQuayService _service;
        private readonly IDclGeneralLedgerService _serviceGeneral;
        private readonly IMdMerchandiseService _merchandiseservice;
        private readonly IMdPeriodService _servicePeriod;
        private readonly INvRetailsService _serviceRetails;
        public GiaoDichQuayController(INvGiaoDichQuayService service, IDclGeneralLedgerService service2, IMdMerchandiseService service3, IMdPeriodService servicePeriod, INvRetailsService serviceRetails)
        {
            _service = service;
            _serviceGeneral = service2;
            _merchandiseservice = service3;
            _servicePeriod = servicePeriod;
            _serviceRetails = serviceRetails;
        }

        /// <summary>
        /// Query entity
        /// POST
        /// </summary>
        /// <param name="jsonData">complex data : jsonData.filtered & jsonData.paged</param>
        /// <returns></returns>
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "nvGiaoDichQuay")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            string _unitCode = _service.GetCurrentUnitCode();
            TransferObj<PagedObj<NvGiaoDichQuayVm.Dto>> result = new TransferObj<PagedObj<NvGiaoDichQuayVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvGiaoDichQuayVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvGiaoDichQuayVm.Search>>();
            filtered.OrderBy = "NgayPhatSinh";
            PagedObj<NvGiaoDichQuay> paged = ((JObject)postData.paged).ToObject<PagedObj<NvGiaoDichQuay>>();
            string unitCode = _service.GetCurrentUnitCode();
            QueryBuilder query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new NvGiaoDichQuay().MaDonVi),
                    Method = FilterMethod.StartsWith,
                    Value = _unitCode
                },
                Orders = new List<IQueryOrder>
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvGiaoDichQuay().NgayPhatSinh),
                        Method = OrderMethod.DESC
                    }
                }
            };
            try
            {
                ResultObj<PagedObj<NvGiaoDichQuay>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<NvGiaoDichQuay>, PagedObj<NvGiaoDichQuayVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("GetAllDataTrade/{codeTrade}")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "nvGiaoDichQuay")]
        public async Task<IHttpActionResult> GetAllDataTrade(string codeTrade)
        {
            TransferObj<NvGiaoDichQuayVm.DataDto> result = new TransferObj<NvGiaoDichQuayVm.DataDto>();
            NvGiaoDichQuayVm.DataDto instance = new NvGiaoDichQuayVm.DataDto();
            try
            {
                NvGiaoDichQuay data = _service.Repository.DbSet.FirstOrDefault(x => x.MaGiaoDich == codeTrade);
                if (data != null)
                {
                    instance = Mapper.Map<NvGiaoDichQuay, NvGiaoDichQuayVm.DataDto>(data);
                    instance = _serviceRetails.SetCustomer(instance);
                    instance.DataDetails = _serviceRetails.DataDetails(instance);
                    result.Data = instance;
                    result.Status = true;
                    return Ok(result);
                }
                else
                {
                    result.Data = new NvGiaoDichQuayVm.DataDto();
                    result.Status = false;
                    return Ok(result);
                }

            }
            catch (Exception)
            {
                result.Data = new NvGiaoDichQuayVm.DataDto();
                result.Status = false;
                return Ok(result);
            }
        }

        [Route("PostSelectDataQuery")]
        [CustomAuthorize(Method = "XEM", State = "nvGiaoDichQuay")]
        public async Task<IHttpActionResult> PostSelectDataQuery(JObject jsonData)
        {
            TransferObj<PagedObj<NvGiaoDichQuayVm.DataDto>> result = new TransferObj<PagedObj<NvGiaoDichQuayVm.DataDto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvGiaoDichQuayVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvGiaoDichQuayVm.Search>>();
            filtered.OrderBy = "NgayPhatSinh";
            PagedObj<NvGiaoDichQuay> paged = ((JObject)postData.paged).ToObject<PagedObj<NvGiaoDichQuay>>();
            string unitCode = _service.GetCurrentUnitCode();
            QueryBuilder query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new NvGiaoDichQuay().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = unitCode
                },
                Orders = new List<IQueryOrder>
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvGiaoDichQuay().NgayPhatSinh),
                        Method = OrderMethod.ASC
                    }
                }
            };
            try
            {
                ResultObj<PagedObj<NvGiaoDichQuay>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = Mapper.Map<PagedObj<NvGiaoDichQuay>, PagedObj<NvGiaoDichQuayVm.DataDto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                result.Status = false;
                result.Data = new PagedObj<NvGiaoDichQuayVm.DataDto>();
                return Ok(result);
            }
        }


        [Route("PostDetails")]
        [CustomAuthorize(Method = "XEM", State = "nvGiaoDichQuay")]
        public async Task<IHttpActionResult> PostDetails(NvGiaoDichQuayVm.Dto data)
        {
            TransferObj<NvGiaoDichQuayVm.Dto> result = new TransferObj<NvGiaoDichQuayVm.Dto>();
            NvGiaoDichQuayVm.Dto temp = new NvGiaoDichQuayVm.Dto();
            NvGiaoDichQuay phieu = _service.Repository.DbSet.FirstOrDefault(x => x.MaGiaoDichQuayPK == data.MaGiaoDichQuayPK);
            if (phieu != null)
            {
                temp = Mapper.Map<NvGiaoDichQuay, NvGiaoDichQuayVm.Dto>(phieu);
                List<NvGiaoDichQuayChiTiet> chiTietPhieu = _service.UnitOfWork.Repository<NvGiaoDichQuayChiTiet>().DbSet.Where(x => x.MaGDQuayPK == phieu.MaGiaoDichQuayPK).ToList();
                temp.DataDetails = Mapper.Map<List<NvGiaoDichQuayChiTiet>, List<NvGiaoDichQuayVm.DtoDetail>>(chiTietPhieu).ToList();
                string unitcode = _service.GetCurrentUnitCode();
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }

        [Route("PostPrintTranferCashieer")]
        public async Task<IHttpActionResult> PostPrintTranferCashieer(ParameterCashier para)
        {
            NvGiaoDichQuayVm.ReportGDQ reporter = new NvGiaoDichQuayVm.ReportGDQ();
            List<NvGiaoDichQuayVm.ReportGDQDetailLevel2> result = new List<NvGiaoDichQuayVm.ReportGDQDetailLevel2>();
            try
            {
                //for (int i = 0; i < nv.Count();i++ )
                //{
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
                reporter.DataDetails.AddRange(_service.CreatePrintTranferCashieer(para.FromDate, para.ToDate, para.UnitCode, para.SellingMachineCodes, para.CashieerCodes));
                //}
            }
            catch (Exception ex)
            {

            }
            return Ok(reporter);
        }

        [Route("PostReportGDQTongHop")]
        public async Task<IHttpActionResult> PostReportGDQTongHop(ParameterCashier para)
        {
            NvGiaoDichQuayVm.ReportGDQTongHopNew reporter = new NvGiaoDichQuayVm.ReportGDQTongHopNew();
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
                        reporter.Username = nhanVien.Username;
                    }
                    else
                    {
                        reporter.Username = "Administrator";
                    }
                }
                switch (para.LoaiGiaoDich)
                {
                    case TypeGiaoDich.NHAPBANLETRALAI:
                        reporter.DataDetails.AddRange(_service.ReportNhapBLeTraLai(para));
                        break;
                    default:
                        reporter.DataDetails.AddRange(_service.ReportGiaoDichQuay(para));
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(reporter);
        }
        private string _convertToArrayCondition(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            var subStrAray = str.Split(',');
            int length = subStrAray.Length;
            string[] resultArray = new string[length];
            for (int i = 0; i < length; i++)
            {
                resultArray[i] = "'" + subStrAray[i] + "'";
            }
            return String.Join(",", resultArray);
        }
        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            var unitCode = _service.GetCurrentUnitCode();
            var currentDate = DateTime.Now.Date;
            var datelock = CurrentSetting.GetNgayKhoaSo(unitCode);
            var result = new ParameterCashier()
            {
                ToDate = datelock,
                FromDate = datelock,
                MaxDate = currentDate,
                UnitCode = unitCode,
                GroupBy = TypeGroupInventoryCashier.MALOAIVATTU,
                LoaiGiaoDich = TypeGiaoDich.XUATBANLE
            };
            return Ok(result);
        }
        [Route("ExportExcel")]
        public HttpResponseMessage ExportExcel(ParameterCashier para)
        {
            var _ParentUnitCode = _service.GetParentUnitCode();
            NvGiaoDichQuayVm.ReportExcel result = null;
            var unitCode = _service.GetCurrentUnitCode();
            //var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaVatTu.ToUpper() == code.ToUpper());//&& x.UnitCode == unitCode);
            var service = new ProcedureService<NvGiaoDichQuayVm.ReportExcel>();
            var data = ProcedureCollection.GetReportGDQExcel(para.FromDate, para.ToDate, new BTS.API.ENTITY.ERPContext(), unitCode);
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int index = 0;
                worksheet.Cells[1, 1, 1, 5].Merge = true;
                worksheet.Cells[1, 1].Value = "SIÊU THỊ TỪ SƠN ";
                worksheet.Cells[2, 1, 2, 5].Merge = true;
                worksheet.Cells[2, 1].Value = "Đường Lý Thái Tổ, P. Đình Bảng, TX Từ Sơn, Bắc Ninh ";
                worksheet.Cells[4, 4, 4, 7].Merge = true;
                worksheet.Cells[4, 4].Value = "BÁO CÁO GIAO DỊCH QUẦY ";
                worksheet.Cells[5, 4, 5, 8].Merge = true;
                worksheet.Cells[5, 4].Value = "Từ ngày: " + para.FromDate.Day + "/" + para.FromDate.Month + "/" + para.FromDate.Year + "   Đến ngày:" + para.ToDate.Day + "/" + para.ToDate.Month + "/" + para.ToDate.Year;
                worksheet.Cells[7, 1].Value = "STT";
                worksheet.Cells[7, 2].Value = "Nhân viên";
                worksheet.Cells[7, 3].Value = "Máy bán";
                worksheet.Cells[7, 4].Value = "Ngày phát sinh";
                worksheet.Cells[7, 5].Value = "Loại giao dịch";
                worksheet.Cells[7, 6].Value = "Mã hàng";
                worksheet.Cells[7, 7].Value = "Tên hàng";
                worksheet.Cells[7, 8].Value = "Số lượng";
                worksheet.Cells[7, 9].Value = "Tổng tiền";
                index = 8;
                if (data != null && data.Count() >= 1)
                {
                    var item = data.ToList();
                    for (int i = 0; i < item.Count; i++)
                    {
                        var hanghoa = new MdMerchandiseVm.Dto();
                        var innerindex = item[i].MaVatTu;
                        var hanghoatg = _merchandiseservice.UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu == innerindex && x.UnitCode.StartsWith(_ParentUnitCode));
                        hanghoa = Mapper.Map<MdMerchandise, MdMerchandiseVm.Dto>(hanghoatg);
                        worksheet.Cells[index, 1].Value = index;
                        worksheet.Cells[index, 2].Value = item[i].MaNguoiTao + "-" + item[i].NguoiTao;
                        worksheet.Cells[index, 3].Value = item[i].MaQuayBan;
                        worksheet.Cells[index, 4].Value = item[i].NgayPhatSinh.Date + "/" + item[i].NgayPhatSinh.Month + "/" + item[i].NgayPhatSinh.Year;
                        worksheet.Cells[index, 6].Value = item[i].MaVatTu;
                        if (hanghoa == null) worksheet.Cells[index, 7].Value = " ";
                        worksheet.Cells[index, 7].Value = hanghoa.TenHang;
                        worksheet.Cells[index, 8].Value = item[i].SoLuong;
                        worksheet.Cells[index, 9].Value = CurrentSetting.FormatTienViet(item[i].TTienCoVat.ToString());
                        if (item[i].LoaiGiaoDich == 1)
                        {
                            worksheet.Cells[index, 5].Value = "xuất bán";
                        }
                        else if (item[i].LoaiGiaoDich == 2)
                        {
                            worksheet.Cells[index, 5].Value = "bán trả lại";
                        }
                        index++;
                    }

                }
                else
                {
                    //return null;
                }

                package.SaveAs(ms);
                HttpResponseMessage response = Request.CreateResponse();
                response.StatusCode = HttpStatusCode.OK;
                ms.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(ms);

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCaoGiaoDichQuay.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                return response;
            }
        }

        [Route("PostExportExcel")]
        public HttpResponseMessage PostExportExcel(ParameterCashier para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                switch (para.LoaiGiaoDich)
                {
                    case TypeGiaoDich.NHAPBANLETRALAI:
                        streamData = _service.ExportExcelNhapBLeTraLaiDetail(para);
                        break;
                    default:
                        streamData = _service.ExportExcelDetail(para);
                        break;
                }
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.GroupBy)
                {
                    case TypeGroupInventoryCashier.MANHOMVATTU:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_ChiTiet_TheoNhomVatTu.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MALOAIVATTU:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_ChiTiet_TheoLoaiVatTu.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MAKHACHHANG:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_ChiTiet_TheoNhaCungCap.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MAVATTU:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_ChiTiet_TheoVatTu.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MAGIAODICH:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_ChiTiet_TheoGiaoDich.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MAKHO:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_ChiTiet_TheoKho.xlsx" };
                        break;
                    default:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_ChiTiet.xlsx" };
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
        [Route("postExportExcelGDQTongHop")]
        public HttpResponseMessage PostExportExcelGDQTongHop(ParameterCashier para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                switch (para.LoaiGiaoDich)
                {
                    case TypeGiaoDich.NHAPBANLETRALAI:
                        streamData = _service.ExportExcelNhapBLeTraLaiTongHop(para);
                        //switch (para.GroupBy)
                        //{
                        //    case TypeGroupInventoryCashier.TYPE:
                        //    case TypeGroupInventoryCashier.GROUP:
                        //    case TypeGroupInventoryCashier.NHACUNGCAP:
                        //    case TypeGroupInventoryCashier.WAREHOUSE:
                        //    case TypeGroupInventoryCashier.MERCHANDISE:
                        //    case TypeGroupInventoryCashier.GIAODICH:
                        //        break;

                        //}
                        break;
                    default:
                        streamData = _service.ExportExcelGDQTongHop(para);
                        //switch (para.GroupBy)
                        //{
                        //    case TypeGroupInventoryCashier.TYPE:
                        //    case TypeGroupInventoryCashier.GROUP:
                        //    case TypeGroupInventoryCashier.NHACUNGCAP:
                        //    case TypeGroupInventoryCashier.WAREHOUSE:
                        //    case TypeGroupInventoryCashier.MERCHANDISE:
                        //    case TypeGroupInventoryCashier.GIAODICH:
                        //        break;
                        //}
                        break;
                }
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.GroupBy)
                {
                    case TypeGroupInventoryCashier.MANHOMVATTU:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_TongHop_TheoNhomVatTu.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MALOAIVATTU:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_TongHop_TheoLoaiVatTu.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MAKHACHHANG:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_TongHop_TheoNhaCungCap.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MAVATTU:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_TongHop_TheoVatTu.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MAGIAODICH:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_TongHop_TheoGiaoDich.xlsx" };
                        break;
                    case TypeGroupInventoryCashier.MAKHO:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_TongHop_TheoKho.xlsx" };
                        break;
                    default:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_GDQ_TongHop.xlsx" };
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
        [Route("UpdateGiaVon")]
        public async Task<IHttpActionResult> UpdateGiaVon(ParameterInventory para)
        {
            var unitCode = _servicePeriod.GetCurrentUnitCode();
            var period = _service.UnitOfWork.Repository<MdPeriod>().DbSet.Where(x => x.ToDate.Date == para.ToDate.Date).FirstOrDefault();
            if (period != null)
            {
                try
                {
                    _servicePeriod.UpDateGiaVon(period);
                    return Ok(true);
                }
                catch (Exception)
                {
                    return InternalServerError();
                }

            }
            return Ok(false);
        }


        [Route("GetDataReport/{id}")]
        [CustomAuthorize(Method = "XEM", State = "nvGiaoDichQuay")]
        public async Task<IHttpActionResult> GetDataReport(string id)
        {
            TransferObj<NvGiaoDichQuayVm.Dto> result = new TransferObj<NvGiaoDichQuayVm.Dto>();
            NvGiaoDichQuayVm.Dto temp = new NvGiaoDichQuayVm.Dto();
            string _unitCode = _service.GetCurrentUnitCode();
            string maGiaoDichQuay = id;
            NvGiaoDichQuay phieu = _service.Repository.DbSet.FirstOrDefault(x => x.MaGiaoDich == maGiaoDichQuay && x.MaDonVi == _unitCode);
            if (phieu != null)
            {
                temp = Mapper.Map<NvGiaoDichQuay, NvGiaoDichQuayVm.Dto>(phieu);
                List<NvGiaoDichQuayChiTiet> chiTietPhieu =
                    _service.UnitOfWork.Repository<NvGiaoDichQuayChiTiet>()
                        .DbSet.Where(x => x.MaGDQuayPK == phieu.MaGiaoDichQuayPK)
                        .ToList();
                temp.DataDetails =
                    Mapper.Map<List<NvGiaoDichQuayChiTiet>, List<NvGiaoDichQuayVm.DtoDetail>>(chiTietPhieu).ToList();
                temp.DataDetails.ForEach(x =>
                {
                    var obj = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(y => y.MaVatTu == x.MaVatTu && y.UnitCode == temp.MaDonVi);
                    if (obj != null) x.TyLeVatVao = obj.TyLeVatVao.ToString();
                });
                string unitcode = _service.GetCurrentUnitCode();
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            else
            {
                result.Data = new NvGiaoDichQuayVm.Dto();
                result.Message = "Không tìm thấy !";
                result.Status = false;
            }
            return Ok(result);
        }


        [Route("ExportExcelDetailsByCondition")]
        public HttpResponseMessage ExportExcelDetailsByCondition(ParameterExcelByCondition para)
        {
            var _ParentUnitCode = _service.GetParentUnitCode();
            NvGiaoDichQuayVm.ReportExcel result = null;
            var unitCode = _service.GetCurrentUnitCode();
            para.UnitCode = unitCode;
            var service = new ProcedureService<NvGiaoDichQuayVm.ReportExcel>();
            var data = ProcedureCollection.DuLieuGiaoDichQuayTheoDieuKien(para.TuNgay, para.DenNgay, para.UnitCode, new BTS.API.ENTITY.ERPContext());
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int index = 0;
                worksheet.Cells[1, 1, 1, 5].Merge = true;
                worksheet.Cells[1, 1].Value = "SIÊU THỊ TỪ SƠN ";
                worksheet.Cells[2, 1, 2, 5].Merge = true;
                worksheet.Cells[2, 1].Value = "Đường Lý Thái Tổ, P. Đình Bảng, TX Từ Sơn, Bắc Ninh ";
                worksheet.Cells[4, 4, 4, 7].Merge = true;
                worksheet.Cells[4, 4].Value = "BÁO CÁO GIAO DỊCH QUẦY ";
                worksheet.Cells[5, 4, 5, 8].Merge = true;
                worksheet.Cells[5, 4].Value = "Từ ngày: " + para.TuNgay.Day + "/" + para.TuNgay.Month + "/" + para.TuNgay.Year + "   Đến ngày:" + para.DenNgay.Day + "/" + para.DenNgay.Month + "/" + para.DenNgay.Year;
                worksheet.Cells[7, 1].Value = "STT";
                worksheet.Cells[7, 2].Value = "Nhân viên";
                worksheet.Cells[7, 3].Value = "Máy bán";
                worksheet.Cells[7, 4].Value = "Ngày phát sinh";
                worksheet.Cells[7, 5].Value = "Loại giao dịch";
                worksheet.Cells[7, 6].Value = "Mã hàng";
                worksheet.Cells[7, 7].Value = "Tên hàng";
                worksheet.Cells[7, 8].Value = "Số lượng";
                worksheet.Cells[7, 9].Value = "Tổng tiền";
                index = 8;
                int stt = 1;
                if (data != null && data.Count() >= 1)
                {
                    var item = data.ToList();
                    for (int i = 0; i < item.Count; i++)
                    {
                        var hanghoa = new MdMerchandiseVm.Dto();
                        var innerindex = item[i].MaVatTu;
                        var hanghoatg = _merchandiseservice.UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu == innerindex && x.UnitCode.StartsWith(_ParentUnitCode));
                        hanghoa = Mapper.Map<MdMerchandise, MdMerchandiseVm.Dto>(hanghoatg);
                        worksheet.Cells[index, 1].Value = stt;
                        worksheet.Cells[index, 2].Value = item[i].MaNguoiTao + "-" + item[i].NguoiTao;
                        worksheet.Cells[index, 3].Value = item[i].MaQuayBan;
                        worksheet.Cells[index, 4].Value = item[i].NgayPhatSinh.Date + "/" + item[i].NgayPhatSinh.Month + "/" + item[i].NgayPhatSinh.Year;
                        worksheet.Cells[index, 6].Value = item[i].MaVatTu;
                        if (hanghoa == null) worksheet.Cells[index, 7].Value = " ";
                        worksheet.Cells[index, 7].Value = hanghoa.TenHang;
                        worksheet.Cells[index, 8].Value = item[i].SoLuong;
                        worksheet.Cells[index, 9].Value = CurrentSetting.FormatTienViet(item[i].TTienCoVat.ToString());
                        if (item[i].LoaiGiaoDich == 1)
                        {
                            worksheet.Cells[index, 5].Value = "xuất bán";
                        }
                        else if (item[i].LoaiGiaoDich == 2)
                        {
                            worksheet.Cells[index, 5].Value = "bán trả lại";
                        }
                        index++;
                        stt++;
                    }

                }
                else
                {
                    //return null;
                }

                package.SaveAs(ms);
                HttpResponseMessage response = Request.CreateResponse();
                response.StatusCode = HttpStatusCode.OK;
                ms.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(ms);

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCaoGiaoDichQuay.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                return response;
            }
        }

        [Route("ExportExcelSyntheticByCondition")]
        public HttpResponseMessage ExportExcelSyntheticByCondition(ParameterExcelByCondition para)
        {
            var _ParentUnitCode = _service.GetParentUnitCode();
            NvGiaoDichQuayVm.ReportExcel result = null;
            var unitCode = _service.GetCurrentUnitCode();
            para.UnitCode = unitCode;
            var service = new ProcedureService<NvGiaoDichQuayVm.ReportExcel>();
            var data = ProcedureCollection.DuLieuTongHopGiaoDichQuayTheoDieuKien(para.TuNgay, para.DenNgay, para.UnitCode, new BTS.API.ENTITY.ERPContext());
            using (ExcelPackage package = new ExcelPackage())
            {
                var ms = new MemoryStream();
                package.Workbook.Worksheets.Add("Data");
                var worksheet = package.Workbook.Worksheets[1];
                int index = 0;
                worksheet.Cells[1, 1, 1, 5].Merge = true;
                worksheet.Cells[1, 1].Value = "SIÊU THỊ TỪ SƠN ";
                worksheet.Cells[2, 1, 2, 5].Merge = true;
                worksheet.Cells[2, 1].Value = "Đường Lý Thái Tổ, P. Đình Bảng, TX Từ Sơn, Bắc Ninh ";
                worksheet.Cells[4, 4, 4, 7].Merge = true;
                worksheet.Cells[4, 4].Value = "BÁO CÁO GIAO DỊCH QUẦY TỔNG HỢP ";
                worksheet.Cells[5, 4, 5, 8].Merge = true;
                worksheet.Cells[5, 4].Value = "Từ ngày: " + para.TuNgay.Day + "/" + para.TuNgay.Month + "/" + para.TuNgay.Year + "   Đến ngày:" + para.DenNgay.Day + "/" + para.DenNgay.Month + "/" + para.DenNgay.Year;
                worksheet.Cells[7, 1].Value = "STT";
                worksheet.Cells[7, 2].Value = "Mã giao dịch";
                worksheet.Cells[7, 3].Value = "Mã giao dịch PK";
                worksheet.Cells[7, 4].Value = "Loại giao dịch";
                worksheet.Cells[7, 5].Value = "Tổng tiền";
                int stt = 1;
                index = 8;
                if (data != null && data.Count() >= 1)
                {
                    var item = data.ToList();
                    for (int i = 0; i < item.Count; i++)
                    {
                        var hanghoa = new MdMerchandiseVm.Dto();
                        var innerindex = item[i].MaVatTu;
                        var hanghoatg = _merchandiseservice.UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu == innerindex && x.UnitCode.StartsWith(_ParentUnitCode));
                        hanghoa = Mapper.Map<MdMerchandise, MdMerchandiseVm.Dto>(hanghoatg);
                        worksheet.Cells[index, 1].Value = stt;
                        worksheet.Cells[index, 2].Value = item[i].MaGiaoDich;
                        worksheet.Cells[index, 3].Value = item[i].MaGiaoDichQuayPK;
                        if (item[i].LoaiGiaoDich == 1)
                        {
                            worksheet.Cells[index, 4].Value = "Bán lẻ";
                        }
                        else if (item[i].LoaiGiaoDich == 2)
                        {
                            worksheet.Cells[index, 4].Value = "Trả lại";
                        }
                        worksheet.Cells[index, 5].Value = CurrentSetting.FormatTienViet(item[i].TTienCoVat.ToString());
                        index++;
                        stt++;
                    }

                }
                else
                {
                    //return null;
                }

                package.SaveAs(ms);
                HttpResponseMessage response = Request.CreateResponse();
                response.StatusCode = HttpStatusCode.OK;
                ms.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(ms);

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCaoGiaoDichQuayTongHop.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                return response;
            }
        }

    }
}