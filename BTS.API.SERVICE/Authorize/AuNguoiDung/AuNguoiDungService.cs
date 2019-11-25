using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTS.API.ENTITY.Md;

namespace BTS.API.SERVICE.Authorize.AuNguoiDung
{
    public interface IAuNguoiDungService : IDataInfoService<AU_NGUOIDUNG>
    {
        AuNguoiDungVm.Dto CreateNewUser(AuNguoiDungVm.ModelRegister model);
        AuNguoiDungVm.CurrentUser Login(AuNguoiDungVm.ModelLogin model);
        AuNguoiDungVm.Dto FindUser(string username, string password);
        Client FindClient(string clientId);
        Task<bool> AddRefreshToken(RefreshToken token);
        Task<bool> RemoveRefreshToken(string refreshTokenId);
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> FindRefreshToken(string refreshTokenId);
        List<RefreshToken> GetAllRefreshTokens();
        string BuildCodeByParent(string unitCode);
        string SaveCodeByParent(string unitCode);
        bool DeleteUser(string id);
    }
    public class AuNguoiDungService : DataInfoServiceBase<AU_NGUOIDUNG>, IAuNguoiDungService
    {
        public AuNguoiDungService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override System.Linq.Expressions.Expression<Func<AU_NGUOIDUNG, bool>> GetKeyFilter(AU_NGUOIDUNG instance)
        {
            var _parent = GetParentUnitCode();
            return x => x.Username == instance.Username && x.UnitCode.StartsWith(_parent);
        }
        public AU_NGUOIDUNG FindByUsername(string username)
        {
            return Repository.DbSet.FirstOrDefault(x => x.Username == username);
        }
        public AuNguoiDungVm.Dto CreateNewInstance()
        {
            var unitCode = GetCurrentUnitCode();
            return new AuNguoiDungVm.Dto()
            {
                MaNhanVien = BuildCode_DM("NHANVIEN",unitCode,false)
            };
        }
        public string BuildCodeByParent(string unitCode)
        {
            var type = TypeMasterData.NV.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == unitCode).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    UnitCode = unitCode,
                    Current = "0",
                };
            }
            var soMa = config.GenerateNumber();
            config.Current = soMa;
            result = unitCode + "-" + config.Code + soMa;

            return result;
        }

        public string SaveCodeByParent(string unitCode)
        {
            var type = TypeMasterData.NV.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == unitCode).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    UnitCode = unitCode,
                    Current = "0",

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
            result = unitCode + "-" + config.Code + config.Current;
            return result;
        }
        public AuNguoiDungVm.CurrentUser Login(AuNguoiDungVm.ModelLogin model)
        {
            AuNguoiDungVm.CurrentUser result = null;
            var user = Repository.DbSet.FirstOrDefault(x => x.Username == model.Username);
            if(user !=null)
            {
                if(user.Password == MD5Encrypt.Encrypt(model.Password))
                {
                    result = Mapper.Map<AU_NGUOIDUNG, AuNguoiDungVm.CurrentUser>(user);
                }
            }
            return result;
        }
        public AuNguoiDungVm.Dto FindUser(string username, string password)
        {
            var result = new AuNguoiDungVm.Dto();
            using (var ctx = new ERPContext())
            {
                var user = ctx.AU_NGUOIDUNGs.FirstOrDefault(x => x.Username == username && x.TrangThai == 10);
                if (user != null)
                {
                    if (user.Password == MD5Encrypt.Encrypt(password))
                    {
                        result = Mapper.Map<AU_NGUOIDUNG, AuNguoiDungVm.Dto>(user);
                        return result;
                    }
                }
                else
                {
                    result =  null;
                }
            }
            return result;
        }
        public AuNguoiDungVm.Dto CreateNewUser(AuNguoiDungVm.ModelRegister model)
        {
            var entity = Mapper.Map<AuNguoiDungVm.ModelRegister, AU_NGUOIDUNG>(model);
            entity.Password = MD5Encrypt.Encrypt(entity.Password);
            entity.Id = Guid.NewGuid().ToString();
            Repository.Insert(entity);
            var result = Mapper.Map<AU_NGUOIDUNG, AuNguoiDungVm.Dto>(entity);
            return result;
        }
        public Client FindClient(string clientId)
        {
            var client = UnitOfWork.Repository<Client>().DbSet.Find(clientId);

            return client;
        }
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var repoRefresh = UnitOfWork.Repository<RefreshToken>().DbSet;
                var existingToken = repoRefresh.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();
                if (existingToken != null)
                {
                    var result = await RemoveRefreshToken(existingToken);
                }
            UnitOfWork.Repository<RefreshToken>().Insert(token);
            return  await UnitOfWork.SaveAsync() > 0;
        }
        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var repoRefresh = UnitOfWork.Repository<RefreshToken>().DbSet;
            var refreshToken = await repoRefresh.FindAsync(refreshTokenId);
            if (refreshToken != null)
            {
                UnitOfWork.Repository<RefreshToken>().Delete(refreshToken.Id);
                return await UnitOfWork.SaveAsync() > 0;
            }
            return false;
        }
        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            UnitOfWork.Repository<RefreshToken>().Delete(refreshToken.Id);
            return await UnitOfWork.SaveAsync() > 0;
        }
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await UnitOfWork.Repository<RefreshToken>().DbSet.FindAsync(refreshTokenId);
            return refreshToken;
        }
        public List<RefreshToken> GetAllRefreshTokens()
        {
            return UnitOfWork.Repository<RefreshToken>().DbSet.ToList();
        }

        public bool DeleteUser(string id)
        {
            var insatance = UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Id == id).FirstOrDefault();
            if (insatance == null)
            {
                return false;
            }

            var auNguoiDungQuyen = UnitOfWork.Repository<AU_NGUOIDUNG_QUYEN>().DbSet.Where(o => o.USERNAME == insatance.Username).ToList();
            foreach (AU_NGUOIDUNG_QUYEN andq in auNguoiDungQuyen)
            {
                andq.ObjectState = ObjectState.Deleted;
            }

            var auNguoiDungNhomQuyen = UnitOfWork.Repository<AU_NGUOIDUNG_NHOMQUYEN>().DbSet.Where(o => o.USERNAME == insatance.Username).ToList();
            foreach (AU_NGUOIDUNG_NHOMQUYEN andnq in auNguoiDungNhomQuyen)
            {
                andnq.ObjectState = ObjectState.Deleted;
            }

            return true;

        }
    }
}
