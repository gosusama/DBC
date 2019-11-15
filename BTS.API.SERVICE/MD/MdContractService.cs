using System;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using System.Security.Claims;
using System.Web;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Common;

namespace BTS.API.SERVICE.MD
{
    public interface IMdContractService : IDataInfoService<MdContract>
    {
        //Add function here
       MdContract InsertDto(MdContractVm.Dto instance);
        MdContract UpdateDto(MdContractVm.Dto instance);
        MdContractVm.ReportModel CreateReport(string id);
        UserInfo GetDoProfile();
    }
    public class MdContractService : DataInfoServiceBase<MdContract>, IMdContractService
    {
        public MdContractService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdContract, bool>> GetKeyFilter(MdContract instance)
        {
            var unitCode = GetCurrentUnitCode();
            return x => x.MaHd == instance.MaHd && x.UnitCode == unitCode;
        }

        public MdContract InsertDto(MdContractVm.Dto instance)
        {
            
            var result = Insert(Mapper.Map<MdContractVm.Dto, MdContract>(instance));
            var detail = Mapper.Map<List<MdContractVm.Detail>, List<MdDetailContract>>(instance.DataDetails);
            var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
            detail.ForEach(x => {
                var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang);
                x.TenHang = hang != null ? hang.TenHang : "";
                x.Id = Guid.NewGuid().ToString();
                x.MaHd = result.MaHd;
            });
            UnitOfWork.Repository<MdDetailContract>().InsertRange(detail);
            return result;
        }

        public MdContract UpdateDto(MdContractVm.Dto instance)
        {
            
            MdContract result = null;
            var item = FindById(instance.Id);
            if (item != null)
            {
                result = Update(Mapper.Map<MdContractVm.Dto, MdContract>(instance));
                {//delete
                    var deleteList = UnitOfWork.Repository<MdDetailContract>().DbSet.Where(x => x.MaHd == result.MaHd).ToList();
                    deleteList.ForEach(x => x.ObjectState = ObjectState.Deleted);
                }
                {//insert data details
                    var merchandiseCollection = UnitOfWork.Repository<MdMerchandise>().DbSet;
                    var detail = Mapper.Map<List<MdContractVm.Detail>, List<MdDetailContract>>(instance.DataDetails);
                    detail.ForEach(x => {
                        var hang = merchandiseCollection.FirstOrDefault(u => u.MaVatTu == x.MaHang);
                        x.Id = Guid.NewGuid().ToString();
                        x.MaHd = result.MaHd;
                    });
                    UnitOfWork.Repository<MdDetailContract>().InsertRange(detail);
                }
            }
            return result;
        }
        public MdContractVm.ReportModel CreateReport(string id)
        {
            var result = new MdContractVm.ReportModel();
            var phieu = FindById(id);
            if (phieu != null)
            {
                result = Mapper.Map<MdContract, MdContractVm.ReportModel>(phieu);
                var chiTietPhieu = UnitOfWork.Repository<MdDetailContract>().DbSet.Where(x => x.MaHd == phieu.MaHd).ToList();
                result.DataReportDetails = Mapper.Map<List<MdDetailContract>, List<MdContractVm.ReportDetailModel>>(chiTietPhieu);
                var customer = UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == result.MaKhachHang);
                if (customer != null)
                {
                    result.DienThoai = customer.DienThoai;
                    result.TenKhachHang = customer.TenKH;
                }
                return result;
            }
            return null;
        }

        public UserInfo GetDoProfile()
        {
            UserInfo info = new UserInfo();
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {              
                var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                var unit = currentUser.Claims.FirstOrDefault(x => x.Type == "unitCode");
                if (unit != null)
                {
                    info.MaDonVi = unit.Value;
                }
                var name = currentUser.Identity.Name;
                var userName = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                if (userName != null)
                {
                    info.Username = userName.TenNhanVien;
                }
                else
                {
                    info.Username = "Administrator";
                }
        }
            return info;
        }
    }
}
