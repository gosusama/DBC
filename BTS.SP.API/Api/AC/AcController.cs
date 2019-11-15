using BTS.API.SERVICE.DCL;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
namespace BTS.SP.API.Api.AC
{

    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string OrderNumber { get; set; }
    }
    [RoutePrefix("api/Ac/Ac")]
    [Route("{id?}")]
    [Authorize]
    public class AcController : ApiController
    {
        private readonly IDclGeneralLedgerService _service;
        public AcController(IDclGeneralLedgerService service)
        {
            _service = service;
        }

        [Route("GenerealJournalReport")]
        public async Task<IHttpActionResult> GenerealJournalReport(DclGeneralLedgerVm.ParameterGeneralJournal instance)
        {
            var result = new DclGeneralLedgerVm.BanBaoCaoNKChung();
            var listItem = new List<DclGeneralLedgerVm.ChiTietChungTu>();
            if (instance.TuNgay == null || instance.DenNgay == null || string.IsNullOrEmpty(instance.UnitCode)) return null;
            var data = _service.ProcedureSoNhatKyChung(instance.TuNgay, instance.DenNgay, instance.UnitCode);
            try
            {

                var headListItem = data.Where(x => x.SOCT != null);
                var detailListItem = data.Where(x => x.SOCT == null);
                headListItem.ToList().ForEach(x =>
                {
                    var tempDetail = new List<DclGeneralLedgerVm.ThongTinChungTu>();
                    var detail = detailListItem.Where(u => u.STT.Contains(x.SOCT));
                    var total = new DclGeneralLedgerVm.ThongTinChungTu()
                    {
                        NoiDung = "Cộng",
                        No = 0,
                        Co = 0
                    };
                    detail.ToList().ForEach(y =>
                    {
                        tempDetail.Add(new DclGeneralLedgerVm.ThongTinChungTu()
                        {
                            Co = y.CO ?? 0,
                            No = y.NO ?? 0,
                            NoiDung = y.NOIDUNG,
                            TaiKhoan = y.TAIKHOAN
                        });
                        total.No += y.CO ?? 0;
                        total.Co += y.NO ?? 0;
                    });
                    var item = new DclGeneralLedgerVm.ChiTietChungTu()
                    {
                        ChungTu = new DclGeneralLedgerVm.ThongTinChungTu()
                        {
                            SoCT = x.SOCT,
                            NgayCT = x.NGAYCT,
                            NoiDung = x.NOIDUNG
                        },
                        ChiTiet = tempDetail,
                        Total = total
                    };
                    listItem.Add(item);
                });

                var date = DateTime.Now;
                result.ThangBC = date.Month.ToString();
                result.NamBC = date.Year.ToString();
                result.Data = listItem;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }

        // GET: api/Orders
        [HttpGet]
        public HttpResponseMessage Download()
        {
            MediaTypeHeaderValue mediaType =
                MediaTypeHeaderValue.Parse("application/octet-stream");
            byte[] excelFile = ExcelSheet();
            string fileName = "Orders.xlsx";
            MemoryStream memoryStream = new MemoryStream(excelFile);
            HttpResponseMessage response =
                response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(memoryStream);
            response.Content.Headers.ContentType = mediaType;
            response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("fileName") { FileName = fileName };
            return response;
        }

        public List<Order> Orders()
        {
            List<Order> orders = new List<Order>();
            orders.Add(new Order { Id = 1, OrderNumber = "123456789" });
            orders.Add(new Order { Id = 2, OrderNumber = "987654321" });
            return orders;
        }
        // this doesn't work
        //public List<Order> Orders()
        //{
        //    List<Order> colOrders = new List<Order>();
        //    using (ServerApplicationContext ctx = ServerApplicationContext.Current ??
        //        ServerApplicationContext.CreateContext())
        //    {
        //        colOrders = ctx.DataWorkspace.Data.Orders.GetQuery().Execute().ToList();
        //    }
        //    return colOrders;
        //}

        public byte[] ExcelSheet()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");
                worksheet.Cells["A1"].LoadFromCollection(Orders(), false);
                byte[] bytes = package.GetAsByteArray();
                return bytes;
            }
        }
    }
}
