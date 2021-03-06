﻿using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BTS.API.SERVICE.NV
{
    public interface INvNhapKhacService : IDataInfoService<NvVatTuChungTu>
    {
        NvVatTuChungTu InsertPhieu(NvNhapKhacVm.Dto instance);
        NvVatTuChungTu UpdatePhieu(NvNhapKhacVm.Dto instance);
        StateProcessApproval Approval(string id);
        NvNhapKhacVm.ReportModel CreateReport(string id);
        NvNhapKhacVm.Dto CreateNewInstance();
        NvVatTuChungTu InsertPhieuXuLyAm(NvNhapKhacVm.Dto instance);
        bool DeletePhieu(string id);
    }
    public class NvNhapKhacService : DataInfoServiceBase<NvVatTuChungTu>, INvNhapKhacService
    {
        public NvNhapKhacService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public bool DeletePhieu(string id)
        {
            var insatance = UnitOfWork.Repository<NvVatTuChungTu>().DbSet.Where(x => x.Id == id).FirstOrDefault();
            if (insatance == null)
            {
                return false;
            }
            var detailData = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(o => o.MaChungTuPk == insatance.MaChungTuPk).ToList();
            foreach (NvVatTuChungTuChiTiet dt in detailData)
            {
                dt.ObjectState = ObjectState.Deleted;
            }
            return true;

        }
        public NvNhapKhacVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            return new NvNhapKhacVm.Dto()
            {
                LoaiPhieu = TypeVoucher.NKHAC.ToString(),
                MaChungTu = BuildCode_PTNX(TypeVoucher.NKHAC.ToString(), unitCode, false),
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode)

            };
        }
        public NvVatTuChungTu InsertPhieu(NvNhapKhacVm.Dto instance)
        {
            var item = Mapper.Map<NvNhapKhacVm.Dto, NvVatTuChungTu>(instance);
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            string _unitCode = GetCurrentUnitCode();
            result.MaChungTu = BuildCode_PTNX(TypeVoucher.NKHAC.ToString(), _unitCode, true);
            item.GenerateMaChungTuPk();
            if(ValidateNgayCT(item.NgayCT.Value))
            result = Insert(result);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var detailData = Mapper.Map<List<NvNhapKhacVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            var khoNhap = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);     
            detailData.ForEach(x => {
                var merchandise = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang);
                x.TenHang = merchandise != null ? merchandise.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTu = result.MaChungTu;
                x.MaChungTuPk = result.MaChungTuPk;
            });
            InsertGeneralLedger(instance.DataClauseDetails, result);
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            return result;
        }
        public NvNhapKhacVm.ReportModel CreateReport(string id)
        {
            var result = new NvNhapKhacVm.ReportModel();
            var exsit = FindById(id);
            if (exsit != null)
            {
                result = Mapper.Map<NvVatTuChungTu, NvNhapKhacVm.ReportModel>(exsit);
                var nhanvien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == exsit.ICreateBy).FirstOrDefault();
                if (nhanvien != null)
                {
                    result.NameNhanVienCreate = nhanvien.TenNhanVien != null ? nhanvien.TenNhanVien : "";
                    result.Username = nhanvien.TenNhanVien != null ? nhanvien.TenNhanVien : "";
                }
                var detailData = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsit.MaChungTuPk).ToList();
                result.DataReportDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapKhacVm.ReportDetailModel>>(detailData);

                var customer = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == result.MaKhachHang);
                if (customer != null)
                {
                    result.TenKhachHang =  customer.TenKH;
                    result.DiaChiKhachHang = customer.DiaChi;
                    result.DienThoai = customer.DienThoai;
                }
                var kho = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);
                if (kho != null)
                {
                    result.TenKho = kho.TenKho;
                }
            }
            var unitCode = GetCurrentUnitCode();
            var createDate = DateTime.Now;
            result.CreateDay = createDate.Day;
            result.CreateMonth = createDate.Month;
            result.CreateYear = createDate.Year;
            result.TenDonVi = CurrentSetting.GetUnitName(unitCode);
            result.DiaChiDonVi = CurrentSetting.GetUnitAddress(unitCode);
            return result;
        }
        public void InsertGeneralLedger(List<NvNhapKhacVm.DtoClauseDetail> data, NvVatTuChungTu chungTu)
        {
            var generalLedgers = Mapper.Map<List<NvNhapKhacVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
            generalLedgers.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = chungTu.MaChungTuPk;
                x.MaChungTu = chungTu.MaChungTu;
                x.LoaiPhieu = chungTu.LoaiPhieu;
                x.TrangThai = chungTu.TrangThai; // Chưa duyệt
                x.NgayCT = chungTu.NgayCT;
                x.NoiDung = chungTu.NoiDung;
                x.UnitCode = x.UnitCode;
            });
            UnitOfWork.Repository<DclGeneralLedger>().InsertRange(generalLedgers);
        }
        public NvVatTuChungTu UpdatePhieu(NvNhapKhacVm.Dto instance)
        {
            instance.Calc();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)ApprovalState.IsComplete) return null;
            var masterData = Mapper.Map<NvNhapKhacVm.Dto, NvVatTuChungTu>(instance);
            var detailData = Mapper.Map<List<NvNhapKhacVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            {
                var detailCollection = UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == exsitItem.MaChungTuPk);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            detailData.ForEach(x =>
            {
                var merchandise = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang);
                x.TenHang = merchandise != null ? merchandise.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = exsitItem.MaChungTuPk;
                x.MaChungTu = exsitItem.MaChungTu;
            });
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            UpdateGeneralLedger(instance.DataClauseDetails, exsitItem);
            var result = Update(masterData);
            return result;
        }
        public void UpdateGeneralLedger(List<NvNhapKhacVm.DtoClauseDetail> data, NvVatTuChungTu exsitItem)
        {
            var generalLedgers = Mapper.Map<List<NvNhapKhacVm.DtoClauseDetail>, List<DclGeneralLedger>>(data);
            {
                var detailCollection = UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(x => x.MaChungTuPk == exsitItem.MaChungTuPk);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            generalLedgers.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTuPk = exsitItem.MaChungTuPk;
                x.MaChungTu = exsitItem.MaChungTu;
                x.LoaiPhieu = exsitItem.LoaiPhieu;
                x.TrangThai = exsitItem.TrangThai;
                x.NgayCT = exsitItem.NgayCT;
                x.NoiDung = exsitItem.NoiDung;
                x.UnitCode = exsitItem.UnitCode;
            });
            UnitOfWork.Repository<DclGeneralLedger>().InsertRange(generalLedgers);
        }
       

    
        public StateProcessApproval Approval(string id)
        {
            StateProcessApproval result;
            var unitCode = GetCurrentUnitCode();
            var periods = CurrentSetting.GetKhoaSo(unitCode);
            if (periods != null)
            {
                var tableName = ProcedureCollection.GetTableName(periods.Year, periods.Period);
                if (ProcedureCollection.IncreaseVoucher(tableName, periods.Year, periods.Period, id))
                {
                    result = StateProcessApproval.Success;
                }
                else
                {
                    result = StateProcessApproval.Failed;
                }
            }
            else
            {
                result = StateProcessApproval.NoPeriod;
            }

            return result;
        }

        public NvVatTuChungTu InsertPhieuXuLyAm(NvNhapKhacVm.Dto instance)
        {
            var item = AutoMapper.Mapper.Map<NvNhapKhacVm.Dto, NvVatTuChungTu>(instance);
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            string _unitCode = GetCurrentUnitCode();
            result.MaChungTu = BuildCode_PTNX(TypeVoucher.NKHAC.ToString(), _unitCode, true);
            item.GenerateMaChungTuPk();
            item.LoaiPhieu = "NKHAC";
            item.NgayDuyetPhieu = item.NgayCT;
            if (ValidateNgayCT(item.NgayCT.Value)) result = Insert(result);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var detailData = AutoMapper.Mapper.Map<List<NvNhapKhacVm.DtoDetail>, List<NvVatTuChungTuChiTiet>>(instance.DataDetails);
            var khoNhap = UnitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == result.MaKhoNhap);
            detailData.ForEach(x =>
            {
                var merchandise = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang);
                x.TenHang = merchandise != null ? merchandise.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaChungTu = result.MaChungTu;
                x.MaChungTuPk = result.MaChungTuPk;
                decimal soluong = (decimal)x.SoLuong;
                x.SoLuong = Math.Abs(soluong);
                x.SoLuongLe = x.SoLuong;
                x.ThanhTien = x.ThanhTien == 0 ? x.SoLuong * x.DonGia : x.ThanhTien;
                x.GiaVon = x.DonGia;
            });
            UnitOfWork.Repository<NvVatTuChungTuChiTiet>().InsertRange(detailData);
            return result;
        }

        protected override Expression<Func<NvVatTuChungTu, bool>> GetKeyFilter(NvVatTuChungTu instance)
        {
            string _unitCode = GetCurrentUnitCode();
            return x => x.MaChungTuPk == instance.MaChungTuPk && x.UnitCode==_unitCode;
        }
    }
}
