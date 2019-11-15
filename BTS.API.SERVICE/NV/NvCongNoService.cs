using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace BTS.API.SERVICE.NV
{
    public interface INvCongNoService : IDataInfoService<NvCongNo>
    {
        NvCongNoVm.Dto CreateNewInstance(string LoaiChungTu);
        NvCongNo InsertPhieu(NvCongNoVm.Dto instance);
        NvCongNo UpdatePhieu(NvCongNoVm.Dto instance);
        NvCongNoVm.Dto GetAmmountCustomerBorrowed(string code, DateTime ngayDuyetPhieu);
        NvCongNoVm.Dto GetAmmountSupplierLend(string code);
        decimal GetTienThanhToan(string code, DateTime ngayDuyetPhieu);
        //NvCongNoVm.Dto ThongTinKhachHangNo(string code, DateTime ngayCT);
        //NvCongNoVm.Dto ThongTinVayNhaCungCap(string code, DateTime ngayCT);
        //NvCongNoVm.Dto ThanhTienKhachHangNoTheoNgay(string code, DateTime ngayCT);
        //NvCongNoVm.Dto ThanhTienVayNhaCungCapTheoNgay(string code, DateTime ngayCT);
    }
    public class NvCongNoService : DataInfoServiceBase<NvCongNo>, INvCongNoService
    {
        private static readonly DateTime DATESTART_NHBANTL = new DateTime(2018, 5, 15);
        public NvCongNoService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        protected override Expression<Func<NvCongNo, bool>> GetKeyFilter(NvCongNo instance)
        {
            string _unitCode = GetCurrentUnitCode();
            return x => x.MaChungTuPk == instance.MaChungTuPk && x.UnitCode==_unitCode;
        }
        public NvCongNoVm.Dto CreateNewInstance(string _loaiChungTu)
        {
            var unitCode = GetCurrentUnitCode();
            var code = BuildCode_PTNX(_loaiChungTu, unitCode, false);
            return new NvCongNoVm.Dto()
            {
                LoaiChungTu = _loaiChungTu,
                MaChungTu = code,
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
            };
        }

        public NvCongNo InsertPhieu(NvCongNoVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var item = Mapper.Map<NvCongNoVm.Dto, NvCongNo>(instance);
            item.ThoiGianDuyetPhieu = int.Parse(DateTime.Now.ToString("HHmmssfff"));
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            string _unitCode = GetCurrentUnitCode();
            result.MaChungTu = BuildCode_PTNX(instance.LoaiChungTu, _unitCode, true);
            item.GenerateMaChungTuPk();
            result = Insert(result);
            return result;
        }
        public NvCongNo UpdatePhieu(NvCongNoVm.Dto instance)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvCongNoVm.Dto, NvCongNo>(instance);
            var result = Update(masterData);
            return result;
        }

        public NvCongNoVm.Dto GetAmmountCustomerBorrowed(string code, DateTime ngayDuyetPhieu)
        {
            var _unitCode = GetCurrentUnitCode();
            NvCongNoVm.Dto result = new NvCongNoVm.Dto();
            try
            {
                var customer = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == code && x.UnitCode == _unitCode);
                if(customer != null)
                {
                    result.MaKhachHang = customer.MaKH;
                    result.ThanhTienCanTra = 0;
                    result.TienThanhToan = 0;
                    var phieuXBs = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => x.MaKhachHang == customer.MaKH && DbFunctions.TruncateTime(x.NgayDuyetPhieu) <= DbFunctions.TruncateTime(ngayDuyetPhieu)
                                    && x.TrangThai == (int)OrderState.IsComplete  && x.LoaiPhieu == TypeVoucher.XBAN.ToString() && x.UnitCode == _unitCode ).ToList();
                    if(phieuXBs.Count > 0)
                        phieuXBs.ForEach(x => result.ThanhTienCanTra += (x.ThanhTienSauVat != null ? x.ThanhTienSauVat : 0));
                    var phieuTLs = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => DbFunctions.TruncateTime(x.NgayDuyetPhieu) > DbFunctions.TruncateTime(DATESTART_NHBANTL) && DbFunctions.TruncateTime(x.NgayDuyetPhieu) <= DbFunctions.TruncateTime(ngayDuyetPhieu)
                                                && x.MaKhachHang == customer.MaKH && x.LoaiPhieu == TypeVoucher.NHBANTL.ToString() && x.UnitCode == _unitCode && x.TrangThai == (int)OrderState.IsComplete).ToList();
                    if (phieuTLs.Count > 0) 
                        phieuTLs.ForEach(x => result.ThanhTienCanTra += (x.ThanhTienSauVat != null ? x.ThanhTienSauVat : 0));
                    var phieuCNs = UnitOfWork.Repository<NvCongNo>().DbSet.Where(x => x.MaKhachHang == customer.MaKH && DbFunctions.TruncateTime(x.NgayCT) <= DbFunctions.TruncateTime(ngayDuyetPhieu) && x.LoaiChungTu == LoaiCongNo.CNKH.ToString() && x.UnitCode == _unitCode).ToList();
                    if (phieuCNs.Count > 0)
                        phieuCNs.ForEach(x => result.ThanhTienCanTra -= (x.ThanhTien != null ? x.ThanhTien : 0));
                }
                return result;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public NvCongNoVm.Dto GetAmmountSupplierLend(string code)
        {
            var _unitCode = GetCurrentUnitCode();
            NvCongNoVm.Dto result = new NvCongNoVm.Dto();
            try
            {
                var supplier = UnitOfWork.Repository<MdSupplier>().DbSet.FirstOrDefault(x => x.MaNCC == code && x.UnitCode == _unitCode);
                if (supplier != null)
                {
                    result.MaKhachHang = supplier.MaNCC;
                    result.ThanhTienCanTra = 0;
                    var phieuXBs = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => x.MaKhachHang == supplier.MaNCC && x.LoaiPhieu == TypeVoucher.NMUA.ToString() && x.UnitCode == _unitCode && x.TrangThai == (int)OrderState.IsComplete).ToList();
                    if (phieuXBs.Count > 0)
                        phieuXBs.ForEach(x => result.ThanhTienCanTra += (x.ThanhTienSauVat != null ? x.ThanhTienSauVat : 0));
                    var phieuCNs = UnitOfWork.Repository<NvCongNo>().DbSet.Where(x => x.MaNhaCungCap == supplier.MaNCC && x.LoaiChungTu == LoaiCongNo.CNNCC.ToString() && x.UnitCode == _unitCode).ToList();
                    if (phieuCNs.Count > 0)
                        phieuCNs.ForEach(x => result.ThanhTienCanTra -= (x.ThanhTien != null ? x.ThanhTien : 0));

                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal GetTienThanhToan(string code, DateTime ngayDuyetPhieu)
        {
            decimal tienThanhToan = 0;
            var _unitCode = GetCurrentUnitCode();
            try
            {
                var customer = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == code && x.UnitCode == _unitCode);
                if (customer != null)
                {
                    int _lastTransaction = 0;
                    var phieuXBs = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => x.MaKhachHang == customer.MaKH && DbFunctions.TruncateTime(x.NgayDuyetPhieu) == DbFunctions.TruncateTime(ngayDuyetPhieu)
                                    && x.TrangThai == (int)OrderState.IsComplete && x.LoaiPhieu == TypeVoucher.XBAN.ToString() && x.UnitCode == _unitCode).OrderByDescending(x => x.ThoiGianDuyetPhieu).ToList();
                    if (phieuXBs.Count > 0 && phieuXBs[0].ThoiGianDuyetPhieu.Value >= _lastTransaction)
                    {
                        _lastTransaction = phieuXBs[0].ThoiGianDuyetPhieu.Value;
                    }
                    var phieuTLs = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => DbFunctions.TruncateTime(x.NgayDuyetPhieu) > DbFunctions.TruncateTime(DATESTART_NHBANTL) && DbFunctions.TruncateTime(x.NgayDuyetPhieu) == DbFunctions.TruncateTime(ngayDuyetPhieu)
                                                && x.MaKhachHang == customer.MaKH && x.LoaiPhieu == TypeVoucher.NHBANTL.ToString() && x.UnitCode == _unitCode && x.TrangThai == (int)OrderState.IsComplete).OrderByDescending(x => x.ThoiGianDuyetPhieu).ToList();
                    if (phieuTLs.Count > 0 && phieuTLs[0].ThoiGianDuyetPhieu.Value >= _lastTransaction)
                    {
                        _lastTransaction = phieuTLs[0].ThoiGianDuyetPhieu.Value;
                    }
                    var phieuCNs = UnitOfWork.Repository<NvCongNo>().DbSet.Where(x => x.MaKhachHang == customer.MaKH && DbFunctions.TruncateTime(x.NgayCT) == DbFunctions.TruncateTime(ngayDuyetPhieu) && x.LoaiChungTu == LoaiCongNo.CNKH.ToString() && x.UnitCode == _unitCode).OrderByDescending(x => x.ThoiGianDuyetPhieu).ToList();
                    if (phieuCNs.Count > 0 && phieuCNs[0].ThoiGianDuyetPhieu.Value >= _lastTransaction)
                    {
                        tienThanhToan = phieuCNs[0].ThanhTien.HasValue ? phieuCNs[0].ThanhTien.Value : 0;
                    }
                }
                return tienThanhToan;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
