using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using System.Data.Entity;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BTS.API.SERVICE.NV
{
    public interface INvPhieuDatHangNCCService : IDataInfoService<NvDatHang>
    {
        NvDatHang InsertPhieu(NvPhieuDatHangNCCVm.Dto instance);
        NvDatHang InsertSummary(NvPhieuDatHangNCCVm.Dto instance);
        NvDatHang UpdatePhieu(NvPhieuDatHangNCCVm.Dto instance);
        NvDatHang ReceiveSummary(NvPhieuDatHangNCCVm.Dto instance);
        List<NvPhieuDatHangNCCVm.DtoDetail> MergePhieu(List<string> soPhieu);
        NvPhieuDatHangNCCVm.ReportModel CreateReport(string id);
        NvPhieuDatHangNCCVm.Dto CreateNewInstance();
        void DeleteSummary(NvDatHang instance);
        bool DeletePhieu(string id);
    }
    public class NvPhieuDatHangNCCService : DataInfoServiceBase<NvDatHang>, INvPhieuDatHangNCCService
    {
        public NvPhieuDatHangNCCService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public bool DeletePhieu(string id)
        {
            var insatance = UnitOfWork.Repository<NvDatHang>().DbSet.Where(x => x.Id == id).FirstOrDefault();
            if (insatance == null)
            {
                return false;
            }
            var detailData = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(o => o.SoPhieuPk == insatance.SoPhieuPk).ToList();
            foreach (NvDatHangChiTiet dt in detailData)
            {
                dt.ObjectState = ObjectState.Deleted;
            }
            return true;

        }
        public NvPhieuDatHangNCCVm.Dto CreateNewInstance()
        {
            var userName = GetClaimsPrincipal().Identity.Name;
            var currentUser = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.FirstOrDefault(x => x.Username == userName);
            var _maNguoiLap = currentUser == null ? "" : currentUser.MaNhanVien;

            return new NvPhieuDatHangNCCVm.Dto()
            {
                Loai = (int)LoaiDatHang.NHACUNGCAP,
                SoPhieu = BuildCode(),
                Ngay = DateTime.Now,
                NguoiLap = _maNguoiLap,
                MaDonViDat = GetCurrentUnitCode(),
            };
        }

        public List<NvPhieuDatHangNCCVm.DtoDetail> MergePhieu(List<string> soPhieu)
        {
            List<NvPhieuDatHangNCCVm.DtoDetail> tempResult = new List<NvPhieuDatHangNCCVm.DtoDetail>();
            foreach (var sp in soPhieu)
            {
                NvDatHang phieu = Repository.DbSet.FirstOrDefault(x => x.SoPhieuPk == sp && x.TrangThai == (int)OrderState.IsApproval);
                if (phieu != null)
                {
                    IQueryable<NvDatHangChiTiet> chiTietPhieu = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieuPk == phieu.SoPhieuPk);
                    List<NvPhieuDatHangNCCVm.DtoDetail> data = Mapper.Map<List<NvDatHangChiTiet>, List<NvPhieuDatHangNCCVm.DtoDetail>>(chiTietPhieu.ToList());
                    tempResult.AddRange(data);
                }
            }
            IEnumerable<NvPhieuDatHangNCCVm.DtoDetail> result = tempResult.GroupBy(x => x.MaHang).Select(u => new NvPhieuDatHangNCCVm.DtoDetail()
            {
                SoLuong = u.Sum(x => x.SoLuong),
                SoLuongBao = u.Sum(x => x.SoLuongBao),
                SoLuongBaoDuyet = u.Sum(x => x.SoLuongBaoDuyet),
                MaBaoBi = u.First().MaBaoBi,
                SoLuongDuyet = u.Sum(x => x.SoLuongDuyet),
                SoLuongLe = u.Sum(x => x.SoLuongLe),
                SoLuongLeDuyet = u.Sum(x => x.SoLuongLeDuyet),
                LuongBao = u.Sum(x => x.LuongBao),
                DonGia = u.First().DonGia,
                DonGiaDuyet = u.First().DonGiaDuyet,
                DonViTinh = u.First().DonViTinh,
                TenHang = u.First().TenHang,
                Barcode = u.First().Barcode,
                MaHd = u.First().MaHd,
                MaHang = u.First().MaHang,
                ThanhTien = u.Sum(x => x.SoLuong) * u.First().DonGia,

            });
            return result.ToList();
        }

        public NvDatHang InsertPhieu(NvPhieuDatHangNCCVm.Dto instance)
        {
            var item = Mapper.Map<NvPhieuDatHangNCCVm.Dto, NvDatHang>(instance);
            item.Id = Guid.NewGuid().ToString();
            var result = AddUnit(item);
            result.GenerateMaChungTuPk();
            result.SoPhieu = SaveCode();
            result = Insert(result);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var dataDetails = Mapper.Map<List<NvPhieuDatHangNCCVm.DtoDetail>, List<NvDatHangChiTiet>>(instance.ListAdd);
            dataDetails.ForEach(x =>
            {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang);
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.SoPhieu = result.SoPhieu;
                x.SoPhieuPk = result.SoPhieuPk;
                x.MaHd = result.MaHd;
            });
            UnitOfWork.Repository<NvDatHangChiTiet>().InsertRange(dataDetails);
            return result;
        }
        public NvDatHang InsertSummary(NvPhieuDatHangNCCVm.Dto instance)
        {
            instance.Calc(); //Tinh lại cac thuộc tính thứ sinh

            NvDatHang item = AutoMapper.Mapper.Map<NvPhieuDatHangNCCVm.Dto, NvDatHang>(instance);
            if (!string.IsNullOrEmpty(instance.SoPhieuCon))
            {
                string[] phieus = instance.SoPhieuCon.Split(',');
                if (phieus.Length > 0)
                {
                    item.SoPhieuCon = string.Empty;
                    foreach (var str in phieus)
                    {
                        NvDatHang obj = Repository.DbSet.FirstOrDefault(x => x.SoPhieuPk == str);
                        if (obj != null)
                        {
                            item.SoPhieuCon += obj.SoPhieuPk + ",";
                            obj.TrangThai = (int)OrderState.IsComplete;
                            obj.ObjectState = ObjectState.Modified;
                        }
                    }
                }
            }
            item.Id = Guid.NewGuid().ToString();
            NvDatHang result = AddUnit(item);
            result.GenerateMaChungTuPk();
            result.SoPhieu = SaveCode();
            result = Insert(result);

            DbSet<MdMerchandise> merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            List<NvPhieuDatHangNCCVm.DtoDetail> dataFilter = instance.DataDetails.Where(x => x.SoLuong > 0).ToList();
            List<NvDatHangChiTiet> dataDetails = AutoMapper.Mapper.Map<List<NvPhieuDatHangNCCVm.DtoDetail>, List<NvDatHangChiTiet>>(dataFilter);
            dataDetails.ForEach(x =>
            {
                MdMerchandise hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang);
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.SoPhieu = result.SoPhieu;
                x.SoPhieuPk = result.SoPhieuPk;
                x.MaHd = result.MaHd;
            });
            UnitOfWork.Repository<NvDatHangChiTiet>().InsertRange(dataDetails);
            return result;
        }
        public NvDatHang UpdatePhieu(NvPhieuDatHangNCCVm.Dto instance)
        {
            instance.Calc();
            var exsitItem = FindById(instance.Id);
            if (exsitItem.TrangThai == (int)OrderState.IsComplete || exsitItem.TrangThai == (int)OrderState.IsRecieved) return null;
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            var masterData = Mapper.Map<NvPhieuDatHangNCCVm.Dto, NvDatHang>(instance);
            var detailData = Mapper.Map<List<NvPhieuDatHangNCCVm.DtoDetail>, List<NvDatHangChiTiet>>(instance.ListEdit);
            {
                var detailCollection = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieu == exsitItem.SoPhieu);
                detailCollection.ToList().ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            var result = Update(masterData);
            detailData.ForEach(x =>
            {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang);
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.SoPhieu = exsitItem.SoPhieu;
                x.SoPhieuPk = exsitItem.SoPhieuPk;
                x.MaHd = masterData.MaHd;
            });
            UnitOfWork.Repository<NvDatHangChiTiet>().InsertRange(detailData);
            return result;
        }
        public NvPhieuDatHangNCCVm.ReportModel CreateReport(string id)
        {
            var result = new NvPhieuDatHangNCCVm.ReportModel();
            var exitItem = FindById(id);
            if (exitItem != null)
            {
                result = Mapper.Map<NvDatHang, NvPhieuDatHangNCCVm.ReportModel>(exitItem);
                var detailData = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieu == exitItem.SoPhieu).ToList();
                result.DataReportDetails = Mapper.Map<List<NvDatHangChiTiet>, List<NvPhieuDatHangNCCVm.ReportDetailModel>>(detailData);
                var _supp = UnitOfWork.Repository<MdSupplier>().DbSet.FirstOrDefault(x => x.MaNCC == exitItem.MaNhaCungCap);
                if (_supp != null)
                {
                    result.NhaCungCap = _supp;
                }
                var unitCode = GetCurrentUnitCode();
                var _donVi = UnitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == unitCode);
                if (_supp != null)
                {
                    result.DonVi = _donVi;
                }
                var createDate = DateTime.Now;
                result.CreateDay = createDate.Day;
                result.CreateMonth = createDate.Month;
                result.CreateYear = createDate.Year;
                //GetNhanVien
                if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
                {
                    var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                    var name = currentUser.Identity.Name;
                    var idProfile = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                    if (idProfile != null)
                    {
                        result.NhanVien = idProfile;
                    }
                }
            }
            return result;
        }
        public string BuildCode(string type = "DHNCC")
        {
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type.ToString(),
                    Code = type.ToString(),
                    Current = "0",
                };
            }
            var soPhieu = config.GenerateNumber();
            config.Current = soPhieu;
            result = string.Format("{0}{1}", config.Code, soPhieu);
            return result;
        }

        public string SaveCode(string type = "DHNCC")
        {
            var result = "";
            var strType = type.ToString();
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == strType).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = strType,
                    Code = strType,
                    Current = "0",
                };
                config.Current = config.GenerateNumber();
                idRepo.Insert(config);
            }
            else
            {
                var newNumber = config.GenerateNumber();
                config.Current = newNumber;
                config.ObjectState = ObjectState.Modified;
            }
            result = string.Format("{0}{1}", config.Code, config.Current);
            return result;
        }

        protected override Expression<Func<NvDatHang, bool>> GetKeyFilter(NvDatHang instance)
        {
            return x => x.SoPhieu == instance.SoPhieu;
        }

        public void DeleteSummary(NvDatHang instance)
        {
            var detailCollection = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieuPk == instance.SoPhieuPk).ToList();
            if (detailCollection.Count > 0)
            {
                detailCollection.ForEach(x => x.ObjectState = ObjectState.Deleted);
            }
            if (!string.IsNullOrEmpty(instance.SoPhieuCon))
            {
                string[] phieus = instance.SoPhieuCon.Split(',');
                if (phieus.Length > 0)
                {
                    foreach (var str in phieus)
                    {
                        var obj = Repository.DbSet.FirstOrDefault(x => x.SoPhieuPk == str);
                        if (obj != null)
                        {
                            obj.TrangThai = (int)OrderState.IsApproval;
                            obj.ObjectState = ObjectState.Modified;
                        }
                    }
                }
            }
            Delete(instance.Id);
        }

        public NvDatHang ReceiveSummary(NvPhieuDatHangNCCVm.Dto instance)
        {
            try
            {
                NvDatHang existtItem = FindById(instance.Id);
                if (existtItem.TrangThai == (int)OrderState.IsRecieved) return null;
                existtItem.TrangThai = (int)OrderState.IsRecieved;
                var result = Update(existtItem);
                instance.DataDetails.ForEach(item =>
                {
                    NvDatHangChiTiet obj = UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.FirstOrDefault(x => x.SoPhieuPk == item.SoPhieuPk && x.MaHang == item.MaHang);
                    if (obj != null)
                    {
                        obj.SoLuongThucTe = item.SoLuongThucTe;
                        obj.ObjectState = ObjectState.Modified;
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
