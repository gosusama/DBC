using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace BTS.API.SERVICE.NV
{
    public interface INvNgayHetHanHangHoaService : IDataInfoService<NvNgayHetHanHangHoa>
    {
        NvNgayHetHanHangHoa InsertPhieu(NvNgayHetHanHangHoaVm.Dto instance);
        NvNgayHetHanHangHoa UpdatePhieu(NvNgayHetHanHangHoaVm.Dto instance);
        NvNgayHetHanHangHoaVm.ReportModel CreateReport(string id);
        NvNgayHetHanHangHoaVm.Dto CreateNewInstance();
        bool DeletePhieu(string id);
    }
    public class NvNgayHetHanHangHoaService : DataInfoServiceBase<NvNgayHetHanHangHoa>, INvNgayHetHanHangHoaService
    {
        public NvNgayHetHanHangHoaService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public NvNgayHetHanHangHoaVm.Dto CreateNewInstance()
        {
            string unitCode = GetCurrentUnitCode();
            string code = BuildCode_BD("BAODATE", unitCode, false);
            return new NvNgayHetHanHangHoaVm.Dto()
            {
                MaPhieu = code,
            };
        }
        public NvNgayHetHanHangHoa InsertPhieu(NvNgayHetHanHangHoaVm.Dto instance)
        {
            string _ParentUnitCode = GetParentUnitCode();
            string _unitCode = GetCurrentUnitCode();
            NvNgayHetHanHangHoa item = Mapper.Map<NvNgayHetHanHangHoaVm.Dto, NvNgayHetHanHangHoa>(instance);
            item.Id = Guid.NewGuid().ToString();
            NvNgayHetHanHangHoa result = AddUnit(item);
            result.MaPhieu = BuildCode_BD("BAODATE", _unitCode, true);
            result.GenerateMaPhieuPk();
            result.TrangThai = 10;
            result = Insert(result);
            List<NvNgayHetHanHangHoaChiTiet> detailData = Mapper.Map<List<NvNgayHetHanHangHoaVm.DtoDetail>, List<NvNgayHetHanHangHoaChiTiet>>(instance.DataDetails);
            detailData.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaPhieuPk = result.MaPhieuPk;
                x.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
                x.ICreateBy = GetClaimsPrincipal().Identity.Name;
                x.ICreateDate = DateTime.Now;
            });
            item.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
            item.ICreateBy = GetClaimsPrincipal().Identity.Name;
            item.ICreateDate = DateTime.Now;
            UnitOfWork.Repository<NvNgayHetHanHangHoaChiTiet>().InsertRange(detailData);
            return result;
        }
        public NvNgayHetHanHangHoa UpdatePhieu(NvNgayHetHanHangHoaVm.Dto instance)
        {
            string _ParentUnitCode = GetParentUnitCode();
            string _unitCode = GetCurrentUnitCode();
            NvNgayHetHanHangHoa exsitItem = FindById(instance.Id);
            NvNgayHetHanHangHoa masterData = Mapper.Map<NvNgayHetHanHangHoaVm.Dto, NvNgayHetHanHangHoa>(instance);
            List<NvNgayHetHanHangHoaChiTiet> detailData = Mapper.Map<List<NvNgayHetHanHangHoaVm.DtoDetail>, List<NvNgayHetHanHangHoaChiTiet>>(instance.DataDetails);
            IQueryable<NvNgayHetHanHangHoaChiTiet> detailCollection = UnitOfWork.Repository<NvNgayHetHanHangHoaChiTiet>().DbSet.Where(x => x.MaPhieuPk == exsitItem.MaPhieuPk);
            detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            detailData.ForEach(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.MaPhieuPk = exsitItem.MaPhieuPk;
                x.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
                x.IUpdateBy = GetClaimsPrincipal().Identity.Name;
                x.IUpdateDate = DateTime.Now;
                x.UnitCode = _unitCode;
            });
            masterData.IState = string.Format("{0}", (int)SyncEntityState.IsWaitingForSync);
            masterData.IUpdateBy = GetClaimsPrincipal().Identity.Name;
            masterData.IUpdateDate = DateTime.Now;
            masterData.UnitCode = _unitCode;
            UnitOfWork.Repository<NvNgayHetHanHangHoaChiTiet>().InsertRange(detailData);
            NvNgayHetHanHangHoa result = Update(masterData);
            return result;
        }

        public NvNgayHetHanHangHoaVm.ReportModel CreateReport(string id)
        {
            string _ParentUnitCode = GetParentUnitCode();
            NvNgayHetHanHangHoaVm.ReportModel result = new NvNgayHetHanHangHoaVm.ReportModel();
            NvNgayHetHanHangHoa exsit = FindById(id);
            if (exsit != null)
            {
                result = Mapper.Map<NvNgayHetHanHangHoa, NvNgayHetHanHangHoaVm.ReportModel>(exsit);
                AU_NGUOIDUNG nhanvien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == exsit.ICreateBy).FirstOrDefault();
                if (nhanvien != null)
                {

                    result.NameNhanVienCreate = nhanvien.TenNhanVien != null ? nhanvien.TenNhanVien : "";
                }
                var detailData = UnitOfWork.Repository<NvNgayHetHanHangHoaChiTiet>().DbSet.Where(x => x.MaPhieuPk == exsit.MaPhieuPk).ToList();
                result.DataReportDetails = Mapper.Map<List<NvNgayHetHanHangHoaChiTiet>, List<NvNgayHetHanHangHoaVm.ReportDetailModel>>(detailData);
                List<NvNgayHetHanHangHoaVm.DtoDetail> listDetails = new List<NvNgayHetHanHangHoaVm.DtoDetail>();
            }
            string unitCode = GetCurrentUnitCode();
            DateTime createDate = DateTime.Now;
            result.CreateDay = createDate.Day;
            result.CreateMonth = createDate.Month;
            result.CreateYear = createDate.Year;
            //GetNhanVien
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                ClaimsPrincipal currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                string name = currentUser.Identity.Name;
                AU_NGUOIDUNG nhanVien = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
            }

            return result;
        }

        public bool DeletePhieu(string id)
        {
            NvNgayHetHanHangHoa insatance = UnitOfWork.Repository<NvNgayHetHanHangHoa>().DbSet.Where(x => x.Id == id).FirstOrDefault();
            if (insatance == null)
            {
                return false;
            }
            List<NvNgayHetHanHangHoaChiTiet> detailData = UnitOfWork.Repository<NvNgayHetHanHangHoaChiTiet>().DbSet.Where(o => o.MaPhieuPk == insatance.MaPhieuPk).ToList();
            foreach (NvNgayHetHanHangHoaChiTiet dt in detailData)
            {
                dt.ObjectState = ObjectState.Deleted;
            }
            return true;
        }
    }
}
