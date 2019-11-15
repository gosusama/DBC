using AutoMapper;
using BTS.API.ASYNC.DatabaseContext;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BTS.API.SERVICE.MD
{

    public interface IMdSupplierService : IDataInfoService<MdSupplier>
    {
        MdSupplier SyncSQLToOracle(TDS_Dmkhachhang instance);
        string BuildCode();
        string SaveCode();
        MdSupplier CreateNewInstance();
        //Add function here
    }
    public class MdSupplierService : DataInfoServiceBase<MdSupplier>, IMdSupplierService
    {
        public MdSupplierService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdSupplier, bool>> GetKeyFilter(MdSupplier instance)
        {
            var maDonViCha = GetParentUnitCode();
            var unitCode = GetCurrentUnitCode();
            return x => x.MaNCC == instance.MaNCC && x.UnitCode.StartsWith(maDonViCha);
        }
        public MdSupplier SyncSQLToOracle(TDS_Dmkhachhang instance)
        {
            using (var ctx = new DBCSQL())
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TDS_Dmkhachhang, MdSupplier>()
                       .ForMember(dest => dest.MaNCC, opt => opt.MapFrom(u => u.Makhachhang))
                       .ForMember(dest => dest.TenNCC, opt => opt.MapFrom(u => u.Tenkhachhang))
                       .ForMember(dest => dest.DiaChi, opt => opt.MapFrom(u => u.Diachi))
                       .ForMember(dest => dest.DienThoai, opt => opt.MapFrom(u => u.Dienthoai))
                       .ForMember(dest => dest.Fax, opt => opt.MapFrom(u => u.Fax))
                       .ForMember(dest => dest.Email, opt => opt.MapFrom(u => u.Email))
                       .ForMember(dest => dest.TrangThai, opt => opt.UseValue(10))
                       .ForMember(dest => dest.IState, opt => opt.UseValue(10));
                });
                try
                {
                    var mapper = config.CreateMapper();
                    var customer = mapper.Map<TDS_Dmkhachhang, MdSupplier>(instance);
                    customer.ICreateBy = GetClaimsPrincipal().Identity.Name;
                    customer.ICreateDate = DateTime.Now;
                    var result = Insert(customer);
                    return result;
                }
                catch
                {
                    return null;
                }
            }
        }
        public MdSupplier CreateNewInstance()
        {
            return new MdSupplier()
            {
                MaNCC = BuildCode()
            };
        }
        public string BuildCode()
        {
            var type = TypeMasterData.NCC.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var maDonViCha = GetParentUnitCode();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    Current = "0000",
                    UnitCode = maDonViCha,
                };
            }
            var soMa = config.GenerateNumber();
            config.Current = soMa;
            result = string.Format("{0}",soMa);

            return result;
        }

        public string SaveCode()
        {
            var type = TypeMasterData.NCC.ToString();
            var result = "";


            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var maDonViCha = GetParentUnitCode();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    Current = "0000",
                    UnitCode = maDonViCha,
                };
                result = config.GenerateNumber();
                config.Current = result;
                idRepo.Insert(config);
            }
            else
            {
                result = config.GenerateNumber();
                config.Current = result;
                config.ObjectState = ObjectState.Modified;
            }
            result = string.Format("{0}", config.Current);
            return result;

        }

    }
}
