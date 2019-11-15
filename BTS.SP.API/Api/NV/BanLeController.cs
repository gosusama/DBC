using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE;
using BTS.API.SERVICE.NV;
using BTS.API.SERVICE.Authorize.AuNguoiDung;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/BanLe")]
    [Route("{id?}")]
    [Authorize]
    public class BanLeController : ApiController
    {
        private readonly IMdPeriodService _servicePeriod;
        private readonly INvRetailsService _serviceRetails;
        private readonly INvGiaoDichQuayService _serviceGdQuay;
        private readonly IAuNguoiDungService _serviceUser;
        private readonly IMdCustomerService _serviceCustomer;
        private readonly IMdMerchandiseService _serviceMerchandise;
        private readonly INvKhuyenMaiVoucherService _serviceKhuyenMaiVoucher;
        public BanLeController(INvRetailsService serviceRetails,IMdPeriodService service, INvGiaoDichQuayService serviceGdQuay, IAuNguoiDungService serviceUser, IMdCustomerService serviceCustomer, IMdMerchandiseService serviceMerchandise, INvKhuyenMaiVoucherService serviceKhuyenMaiVoucher)
        {
            _servicePeriod = service;
            _serviceRetails = serviceRetails;
            _serviceGdQuay = serviceGdQuay;
            _serviceUser = serviceUser;
            _serviceCustomer = serviceCustomer;
            _serviceMerchandise = serviceMerchandise;
            _serviceKhuyenMaiVoucher = serviceKhuyenMaiVoucher;
        }
        [Route("GetPeriodDate")]
        [HttpGet]
        public MdPeriod GetPeriodDate()
        {
            var unitCode = _servicePeriod.GetCurrentUnitCode();
            var period = new MdPeriod();
            period = CurrentSetting.GetKhoaSo(unitCode);
            if (period != null)
            {
                return period;
            }
            else
            {
                return null;
            }
        }
        
        [Route("WriteLog")]
        [HttpPost]
        public void WriteLog(AuNguoiDungVm.CurrentUser currentUser)
        {
            _servicePeriod.WiteLog(DateTime.Now, currentUser.MaMayBan, currentUser.MaNhanVien, "LOGOUT", currentUser.UnitCode, currentUser.UserName);
        }
        [Route("HistoryBuyOfCustomer/{para}")]
        [HttpGet]
        public async Task<IHttpActionResult> HistoryBuyOfCustomer(string para)
        {
            List<NvGiaoDichQuayVm.ReportHistoryBuyOfCustomer> data = new List<NvGiaoDichQuayVm.ReportHistoryBuyOfCustomer>();
            var result = new TransferObj<List<NvGiaoDichQuayVm.ReportHistoryBuyOfCustomer>>();
            try
            {
                data = _serviceRetails.ReportHistoryBuyOfCustomer(para);
                if (data != null)
                {
                    result.Status = true;
                    result.Data = data;
                }
                else
                {
                    result.Status = false;
                    result.Data = null;
                }
            }
            catch (Exception)
            {
                result.Status = false;
                result.Data = null;
            }
            return Ok(result);
        }
        [Route("CheckInventory/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> CheckInventory(string code)
        {
            List<string[]> lstData = new List<string[]>();
            var result = new TransferObj<List<string[]>>();
            try
            {
                lstData = _serviceRetails.GetInventory(code);
                if (lstData.Count > 0)
                {
                    result.Status = true;
                    result.Data = lstData;
                }
                else
                {
                    result.Status = false;
                    result.Data = null;
                }
            }
            catch (Exception)
            {
                result.Status = false;
                result.Data = null;
            }
            return Ok(result);
        }
        [Route("BuildCodeTrade")]
        [HttpPost]
        public string NewCodeExchange()
        {
            string curentUser = _serviceUser.GetClaimsPrincipal().Identity.Name;
            string maGiaoDich = "XBANBUON-" + curentUser + "-" + DateTime.Now.Hour +DateTime.Now.Minute + DateTime.Now.Second +DateTime.Now.Millisecond;
            return maGiaoDich;
        }
        [Route("KiemTraKhoaBanAm")]
        [HttpGet]
        public bool KiemTraKhoaBanAm()
        {
            bool flag = false;
            var check =
                _serviceRetails.UnitOfWork.Repository<AU_THAMSOHETHONG>()
                    .DbSet.FirstOrDefault(x => x.MaThamSo == "KHOABANAM");
            if (check != null && check.GiaTriThamSo == 1)
            {
                flag = true;
            }
            return flag;
        }
        [Route("GetUserByUnitCode")]
        [HttpPost]
        public async Task<IHttpActionResult> GetUserByUnitCode()
        {
            var result = new TransferObj<List<AU_NGUOIDUNG>>();
            var _unitCode = _serviceUser.GetCurrentUnitCode();
            var allUser = _serviceUser.Repository.DbSet.Where(x => x.UnitCode == _unitCode).ToList();
            if (allUser.Count > 0)
            {
                result.Data = allUser;
                result.Status = true;
            }
            else
            {
                result.Data = new List<AU_NGUOIDUNG>();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetKhuyenMaiCombo")]
        [HttpGet]
        public async Task<IHttpActionResult> GetKhuyenMaiCombo()
        {
            string unitCode = _serviceCustomer.GetCurrentUnitCode();
            var data = new NvKhuyenMaiComboVm.ListCombo();
            var result = new TransferObj<NvKhuyenMaiComboVm.ListCombo>();
            try
            {
                var returnData = ProcedureCollection.GetKhuyenMaiCombo(unitCode);
                if (returnData != null && returnData.Count > 0)
                {
                    foreach (var record in returnData)
                    {
                        data.ListComboLeft.Add(record);
                        data.ListComboRight.Add(record);
                    }
                    var groupByComboLeft = data.ListComboLeft.GroupBy(x => new { x.MaVatTuLeft,x.MaChuongTrinh,x.GiaTriKhuyenMai,x.SoLuongKhuyenMai}).Select(group => new NvKhuyenMaiComboVm.ListCombo()
                    {
                        MaVatTuLeft = group.Key.MaVatTuLeft,
                        MaChuongTrinh = group.Key.MaChuongTrinh,
                        GiaTriKhuyenMai = group.Key.GiaTriKhuyenMai,
                        SoLuongKhuyenMai = group.Key.SoLuongKhuyenMai,
                    }).ToList();
                    var groupByComboRight = data.ListComboRight.GroupBy(x => new { x.MaVatTuRight, x.MaChuongTrinh, x.GiaTriKhuyenMai, x.SoLuongKhuyenMai}).Select(group => new NvKhuyenMaiComboVm.ListCombo()
                    {
                        MaVatTuRight = group.Key.MaVatTuRight,
                        MaChuongTrinh = group.Key.MaChuongTrinh,
                        GiaTriKhuyenMai = group.Key.GiaTriKhuyenMai,
                        SoLuongKhuyenMai = group.Key.SoLuongKhuyenMai,
                    }).ToList();
                    if (groupByComboLeft.Count > 0 || groupByComboRight.Count > 0)
                    {
                        result.Data = new NvKhuyenMaiComboVm.ListCombo();
                        result.Data.ListComboLeft = groupByComboLeft;
                        result.Data.ListComboRight = groupByComboRight;
                    }
                    result.Status = true;
                }
                else
                {
                    result.Data = null;
                    result.Status = false;
                }
            }
            catch(Exception ex)
            {
                result.Data = null;
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetKhuyenMaiHangTangHang")]
        [HttpGet]
        public async Task<IHttpActionResult> GetKhuyenMaiHangTangHang()
        {
            string unitCode = _serviceCustomer.GetCurrentUnitCode();
            var result = new TransferObj<List<NvKhuyenMaiBuy1Get1Vm.ListHangTangHang>>();
            try
            {
                var returnData = ProcedureCollection.GetKhuyenMaiHangTangHang(unitCode);
                if (returnData != null && returnData.Count > 0)
                {
                        result.Data = returnData;
                        result.Status = true;
                }
                else
                {
                    result.Data = null;
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Status = false;
            }
            return Ok(result);
        }
        
        [Route("GetHangKhachHang/{hangkh}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetHangKhachHang(string hangkh)
        {
            string _parentUnicode = _serviceCustomer.GetParentUnitCode();
            var result = new TransferObj<MdHangKH>();
            try
            {
                if (!string.IsNullOrEmpty(hangkh))
                {
                    var data =
                        _serviceRetails.UnitOfWork.Repository<MdHangKH>()
                            .DbSet.FirstOrDefault(x => x.MaHangKh == hangkh && x.UnitCode.StartsWith(_parentUnicode));
                    if (data != null)
                    {
                        result.Data = data;
                        result.Message = "Hạng " + data.TenHangKh;
                        result.Status = true;
                    }
                    else
                    {
                        result.Data = data;
                        result.Message = "Không tồn tại hạng khách hàng";
                        result.Status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("FilterCustomerData/{strKey}")]
        [HttpGet]
        public async Task<IHttpActionResult> FilterCustomerData(string strKey)
        {
            var result = new TransferObj();
           var unitCode = _serviceCustomer.GetCurrentUnitCode();
            try
            {
                var serviceProcedure = new ProcedureService<MdCustomerVm.CustomerDto>();
                var data = ProcedureCollection.QueryCustomer(unitCode, strKey);
                if (data.Count > 0)
                {
                    result.Data = data;
                    result.Status = true;
                    result.Message = "Truy vấn thành công";
                }
                else
                {
                    result.Data = null;
                    result.Status = false;
                    result.Message = "Không tìm thấy";
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Status = false;
                result.Message = "Không tìm thấy";
            }
            return Ok(result);
        }
        //Procerdure Get Mã bó hàng
        [Route("GetDataPackageMerchandise/{maBoHang}")]
        public async Task<IHttpActionResult> GetDataPackageMerchandise(string maBoHang)
        {
            MdMerchandiseVm.DataBoHang result = null;
            //Tạm thời khóa mã đơn vị
            var unitCode = _serviceUser.GetCurrentUnitCode();
            var maKho = _serviceUser.GetCurrentUnitCode() + "-K2";
            var service = new ProcedureService<MdMerchandiseVm.DtoAndPromotion>();
            if (maBoHang.Substring(0, 2).Equals("BH"))
            {
                try
                {
                    var data = ProcedureCollection.GetDataBoHang(new BTS.API.ENTITY.ERPContext(), maBoHang,unitCode);
                    if (data != null)
                    {
                        foreach(var i in data.ListMaHang)
                        {
                            string codeMerchandise = i.MaVatTu;
                            //check tồn mã hàng này
                            var xntItem = ProcedureCollection.GetCostOfGoodsSoldByMerchandise(unitCode, maKho, codeMerchandise);
                            if (xntItem != null)
                            {
                                i.TonCuoiKySl = xntItem.ClosingQuantity == 0 ? 0 : xntItem.ClosingQuantity;
                                i.LogKhuyenMaiError = 1;
                                i.IsBanAm = false;
                                i.Status = true;
                                i.Message = "Bán mã bó hàng";
                                i.NoiDungKhuyenMai = "Khuyến mại bó hàng";

                                if (i.TonCuoiKySl <= 0)
                                {
                                    i.LogKhuyenMaiError = 1;
                                    i.IsBanAm = true;
                                    i.Status = true;
                                    i.Message = "Bán mã bó hàng";
                                    i.NoiDungKhuyenMai = "Khuyến mại bó hàng";
                                }
                            }
                            //end check tồn 
                        }
                    }
                    else
                    {
                    }
                    result = data;
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }
            return Ok(result);
        }
        public void SaveCustomer(string maKhachHang, NvGiaoDichQuayVm.DataDto instance)
        {
            bool flag = false;
            decimal tienNguyenGia = 0, sumTienNguyenGia = 0, tienSale = 0, sumTienSale = 0, tongTien = 0, soDiem = 0;
            int quenThe = 0;
            var _parentUnitCode = _serviceCustomer.GetParentUnitCode();
            var khachHang = _serviceCustomer.Repository.DbSet.FirstOrDefault(x => x.MaKH == maKhachHang && x.UnitCode.StartsWith(_parentUnitCode));
            if (khachHang != null)
            {
                
                foreach (var record in instance.DataDetails)
                {
                    if (instance.IsTichDiem) flag = true;
                    if (record != null && record.TienKhuyenMai > 0)
                    {
                        decimal.TryParse(record.ThanhTien.ToString(), out tienSale);
                        sumTienSale = sumTienSale + tienSale;
                    }
                    else
                    {
                        decimal.TryParse(record.ThanhTien.ToString(), out tienNguyenGia);
                        sumTienNguyenGia = sumTienNguyenGia + tienNguyenGia;
                    }
                }
                decimal.TryParse(instance.SumTienHang.ToString(), out soDiem);
                if (flag) soDiem = soDiem*2;
                    tongTien = sumTienNguyenGia + sumTienSale;
                //có rồi -- update
                int.TryParse(instance.QuenThe.ToString(), out quenThe);
                _serviceCustomer.UpdateCustomerAfterPurchase(sumTienNguyenGia, sumTienSale, tongTien, soDiem, quenThe,khachHang.Id);
            }
            else //chưa có khách hàng này
            {
                var kh = new MdCustomer();
                kh.MaKH = instance.Makh;
                kh.TenKH = instance.TenKH;
                kh.TenKhac = instance.TenKH;
                kh.DiaChi = instance.DiaChi;
                kh.DienThoai = instance.DienThoai;
                kh.NgaySinh = instance.NgaySinh;
                kh.NgayDacBiet = instance.NgayDacBiet;
                kh.Email = instance.Email;
                kh.MaThe = instance.MaThe;
                kh.SoDiem = instance.TTienCoVat;
                kh.QuenThe = 0;
                foreach (var record in instance.DataDetails)
                {
                    if (record != null && record.TienKhuyenMai > 0)
                    {
                        decimal.TryParse(record.ThanhTien.ToString(), out tienSale);
                        sumTienSale = sumTienSale + tienSale;
                    }
                    else
                    {
                        decimal.TryParse(record.ThanhTien.ToString(), out tienNguyenGia);
                        sumTienNguyenGia = sumTienNguyenGia + tienNguyenGia;
                    }
                }
                decimal.TryParse(instance.TTienCoVat.ToString(), out soDiem);
                tongTien = sumTienNguyenGia + sumTienSale;
                int quenTheNew = 0;
                _serviceCustomer.Insert(kh);
                _serviceCustomer.UpdateCustomerAfterPurchase(sumTienNguyenGia, sumTienSale, tongTien, soDiem, quenTheNew, kh.Id);

            }
        }
        [Route("Post")]
        public async Task<IHttpActionResult> Post(NvGiaoDichQuayVm.DataDto instance)
        {
            var result = new TransferObj<NvGiaoDichQuay>();
            var _parentUnitCode = _serviceGdQuay.GetParentUnitCode();
            try
            {
                //lưu giao dịch
                var item = _serviceRetails.InsertPhieu(instance);
                //run XNT tăng giảm phiếu           
                //update thông tin khách hàng
                await _serviceRetails.UnitOfWork.SaveAsync();
                if (item.MaGiaoDichQuayPK != "" && item.LoaiGiaoDich > 0)
                {
                    int loaiGiaoDich = 0;
                    Int32.TryParse(item.LoaiGiaoDich.ToString(), out loaiGiaoDich);
                    _serviceRetails.RUNSTORE_TANGGIAM_TON(item.MaGiaoDichQuayPK, loaiGiaoDich);
                }
                //update customer
                if (instance.Makh != "KHACHLE") SaveCustomer(instance.Makh, instance);
                await _serviceCustomer.UnitOfWork.SaveAsync();
                //update only one use voucher code 
                if (item.MaVoucher != null)
                {
                    //check code voucher in NVKhuyenMai
                    var itemVoucher =
                        _serviceGdQuay.UnitOfWork.Repository<NvChuongTrinhKhuyenMai>()
                            .DbSet.FirstOrDefault(x => x.LoaiKhuyenMai == 7 && x.MaGiamGia == item.MaVoucher && x.UnitCode.StartsWith(_parentUnitCode));
                    if (itemVoucher != null)
                    {
                        itemVoucher.TrangThaiSuDung = (int)ApprovalState.IsExpired;
                        _serviceKhuyenMaiVoucher.Update(itemVoucher);
                        await _serviceKhuyenMaiVoucher.UnitOfWork.SaveAsync();
                    }
                }
                result.Data = item;
                result.Status = true;
                return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.MaGiaoDichQuayPk }, result);
            }
            catch (Exception exception)
            {
                return InternalServerError();
            }
        }
        [Route("GetAllDataTrade/{codeTrade}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllDataTrade(string codeTrade)
        {
            var result = new TransferObj<NvGiaoDichQuayVm.DataDto>();
            NvGiaoDichQuayVm.DataDto instance = new NvGiaoDichQuayVm.DataDto();
            try
            {
                var data = _serviceGdQuay.Repository.DbSet.FirstOrDefault(x => x.MaGiaoDich == codeTrade);
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
        [HttpPost]
        [Route("GetDataDetailsGDQuay")]
        public async Task<IHttpActionResult> GetDataDetailsGDQuay(JObject jsonData)
        {
            var result = new TransferObj<NvGiaoDichQuayVm.Dto>();
            var data = new NvGiaoDichQuayVm.Dto();
            var MaGiaoDich = (string)jsonData.SelectToken("data");
            if (MaGiaoDich != null)
            {
                var giaoDich = _serviceGdQuay.Repository.DbSet.FirstOrDefault(x => x.MaGiaoDichQuayPK == MaGiaoDich);
                if (giaoDich != null)
                {
                    data = Mapper.Map<NvGiaoDichQuay, NvGiaoDichQuayVm.Dto>(giaoDich);
                    var chiTietPhieu = _serviceGdQuay.UnitOfWork.Repository<NvGiaoDichQuayChiTiet>().DbSet.Where(x => x.MaGDQuayPK == giaoDich.MaGiaoDichQuayPK).ToList();
                    data.DataDetails = Mapper.Map<List<NvGiaoDichQuayChiTiet>, List<NvGiaoDichQuayVm.DtoDetail>>(chiTietPhieu).ToList();
                    result.Data = data;
                    result.Status = true;
                    result.Message = "Tìm thấy giao dịch";
                    return Ok(result);
                }
            }
            else
            {
                result.Data = null;
                result.Status = false;
                result.Message = "Không tìm thấy mã giao dịch";
            }
            return Ok(result);
        }
        [Route("GetDetailByCode/{code}")]
        public async Task<IHttpActionResult> GetDetailByCode(string code)
        {
            var result = new MdMerchandiseVm.MasterDto();
            var unitCode = _serviceMerchandise.GetCurrentUnitCode();
            var instance = _serviceMerchandise.Repository.DbSet.FirstOrDefault(x => x.MaVatTu == code);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                var detail = _serviceMerchandise.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu == instance.MaVatTu && x.MaDonVi == unitCode);

                //context.Request.Url.Host
                result = Mapper.Map<MdMerchandise, MdMerchandiseVm.MasterDto>(instance);
                result.PathImage = WebConfigurationManager.AppSettings["rootUrl"] + "/" + result.PathImage;
                result.DataDetails = Mapper.Map<List<MdMerchandisePrice>, List<MdMerchandiseVm.DtoDetail>>(detail.ToList());
                return Ok(result);
            }
            catch (Exception e)
            {

                return InternalServerError();
            }

        }
        [Route("PostDataMerchandise")]
        [HttpPost]
        public async Task<IHttpActionResult> PostDataMerchandise(MdMerchandiseVm.Search dataSearch)
        {
            var result = new TransferObj<MdMerchandiseVm.DtoAndPromotion>();
            //Tạm thời khóa mã đơn vị
            var _unitCode = _serviceUser.GetCurrentUnitCode();
            var _parentUnitCode = _serviceUser.GetParentUnitCode();
            var _codeWareHouse = _unitCode + "-K2";
            //trường hợp bán mã cân
            if (dataSearch.MaVatTu.Length > 9 && dataSearch.MaVatTu.Substring(0, 2).Equals("20"))
            {
                try
                {
                    var data = ProcedureCollection.GetBalanceCode(new ERPContext(), dataSearch.MaVatTu);
                    if (data != null && data.Count() == 1)
                    {
                        var items = data.ToList();
                        string codeMerchandise = items[0].MaVatTu;
                        result.Data = _serviceRetails.GetDataPromotionByMerchandise(items[0]);
                        //check tồn mã hàng này
                        var xntItem = ProcedureCollection.GetCostOfGoodsSoldByMerchandise(_unitCode, _codeWareHouse,
                            items[0].MaVatTu);
                        if (xntItem != null)
                        {
                            result.Data.TonCuoiKySl = xntItem.ClosingQuantity == 0 ? 0 : xntItem.ClosingQuantity;
                            result.Data.GiaVon = xntItem.CostOfCapital == 0 ? 0 : xntItem.CostOfCapital;
                            if (result.Data.TonCuoiKySl <= 0)
                            {
                                result.Data.IsBanAm = true;
                            }
                        }
                        //end check tồn 
                        result.Status = true;
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Không tìm thấy sản phẩm này trong danh sách hàng hóa ";
                    }
                }
                catch (Exception ex)
                {
                    return NotFound();
                }
            }
            //trường hợp bán mã hàng 
            else
            {
                try
                {
                    var data = ProcedureCollection.GetMerchandiseAndPromotion(new BTS.API.ENTITY.ERPContext(),
                        dataSearch.MaVatTu, _parentUnitCode);
                    if (data != null && data.Count() == 1)
                    {
                        var items = data.ToList();
                        string codeMerchandise = items[0].MaVatTu;
                        result.Data = _serviceRetails.GetDataPromotionByMerchandise(items[0]);
                        //check tồn mã hàng này
                        var xntItem = ProcedureCollection.GetCostOfGoodsSoldByMerchandise(_unitCode, _codeWareHouse,items[0].MaVatTu);
                        if (xntItem != null)
                        {
                            result.Data.TonCuoiKySl = xntItem.ClosingQuantity == 0 ? 0 : xntItem.ClosingQuantity;
                            result.Data.GiaVon = xntItem.CostOfCapital == 0 ? 0 : xntItem.CostOfCapital;
                            if (result.Data.TonCuoiKySl <= 0)
                            {
                                result.Data.IsBanAm = true;
                            }
                        }
                        // end check tồn
                        result.Status = true;
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Không tìm thấy sản phẩm này !";
                    }
                }
                catch (Exception ex)
                {
                    result.Status = false;
                    result.Data = new MdMerchandiseVm.DtoAndPromotion();
                    return Ok(result);
                }
            }
            //}
            return Ok(result);
        }
    }
}