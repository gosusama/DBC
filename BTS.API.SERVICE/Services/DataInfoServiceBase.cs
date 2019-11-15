using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using BTS.API.ENTITY;
using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.NV;
using System.Web;
using BTS.API.ENTITY.Authorize;
using System.Security.Claims;
using System.Web.Configuration;

namespace BTS.API.SERVICE.Services
{
    public class DataInfoServiceBase<TEntity> : EntityServiceBase<TEntity>, IDataInfoService<TEntity>
        where TEntity : DataInfoEntity
    {
        public DataInfoServiceBase(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public virtual IDataInfoService<TEntity> Include(Expression<Func<TEntity, object>> include)
        {
            Includes.Add(include);
            ((IQueryable<TEntity>) Repository.DbSet).Include(include);
            return this;
        }

        protected virtual Expression<Func<TEntity, bool>> GetIdFilter(string id)
        {
            return x => x.Id == id;
        }

        protected virtual Expression<Func<TEntity, bool>> GetKeyFilter(TEntity instance)
        {
            return x => x.Id == instance.Id;
        }
        public virtual bool ValidateNgayCT(DateTime ngayCt)
        {
            var currentUnitCode = GetCurrentUnitCode();
            var ngayKhoaSo = CurrentSetting.GetNgayKhoaSo(currentUnitCode);
            if (ngayCt.Date < ngayKhoaSo.Date) return false;
            return true;
        }
        public virtual TEntity Find(TEntity instance, bool notracking = false)
        {
            var result = (notracking ? Repository.DbSet.AsNoTracking() : Repository.DbSet)
                .FirstOrDefault(GetKeyFilter(instance));
            return result;
        }

        public virtual TEntity FindById(string id, bool notracking = false)
        {
            var result = (notracking ? Repository.DbSet.AsNoTracking() : Repository.DbSet)
                .FirstOrDefault(GetIdFilter(id));
            return result;
        }

        public string GetPhysicalPathImportFile()
        {
            return WebConfigurationManager.AppSettings["rootPhysical"] + "\\Upload\\ImportFile\\";
        }
        public virtual TEntity Insert(TEntity instance, bool withUnitCode = true)
        {
            var exist = Find(instance, true);
            if (exist != null)
            {
                throw new Exception("Tồn tại bản ghi có cùng mã!");
            }
            var newInstance = Mapper.DynamicMap<TEntity, TEntity>(instance);
            var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
            newInstance.ICreateDate = DateTime.Now;
            newInstance.ICreateBy = currentUser.Identity.Name;
            newInstance.Id = Guid.NewGuid().ToString();
            if (withUnitCode) { Repository.Insert(AddUnit(newInstance)); }
            else
            {
                newInstance.UnitCode = instance.UnitCode;
                Repository.Insert(newInstance);
            }
            return newInstance;
        }

        public virtual TEntity Update(TEntity instance,
           Action<TEntity, TEntity> updateAction = null,
           Func<TEntity, TEntity, bool> updateCondition = null)
        {
            Mapper.CreateMap<TEntity, TEntity>();
            var entity = Find(instance, false);
            if (entity == null || instance.Id != entity.Id)
            {
                throw new Exception("Bản ghi không tồn tại trong hệ thống");
            }
            var allowUpdate = updateCondition == null || updateCondition(
                instance, entity);
            if (allowUpdate)
            {
                if (updateAction == null)
                {
                    entity = Mapper.Map(instance, entity);
                    var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                    entity.IUpdateDate = DateTime.Now;
                    entity.IUpdateBy = currentUser.Identity.Name;
                }
                else
                {
                    updateAction(instance, entity);
                }
                entity.ObjectState = ObjectState.Modified;
            }
            return entity;
        }


        public virtual void WiteLog(DateTime ngay,string maMayBan,string maNhanVien,string trangThai,string unitCode,string userName)
        {
            using (var context = new ERPContext())
            {
                var checkExist = context.AU_LOGs.FirstOrDefault(x => x.NGAY == ngay && x.MaMayBan == maMayBan && x.MaNhanVien == maNhanVien && x.TrangThai == trangThai);
                if (checkExist != null)
                {
                    checkExist.NGAY = ngay;
                    checkExist.TrangThai = trangThai;
                    checkExist.MaNhanVien = maNhanVien;
                    checkExist.MaMayBan = maMayBan;
                    checkExist.ThoiGian = ngay.Hour.ToString() + ":" + ngay.Minute.ToString() + ":" + ngay.Second.ToString();
                    if (!string.IsNullOrEmpty(trangThai) && trangThai == "LOGIN") checkExist.TinhTrang = 1;
                    else if (!string.IsNullOrEmpty(trangThai) && trangThai == "LOGOUT") checkExist.TinhTrang = 0;
                    checkExist.UnitCode = unitCode;
                    checkExist.IUpdateBy = userName;
                    checkExist.IUpdateDate = DateTime.Now;
                    checkExist.ObjectState = ObjectState.Modified;
                    context.AU_LOGs.Attach(checkExist);
                    context.Entry(checkExist).State = EntityState.Modified;
                    var iUpdate = context.SaveChanges();
                }
                else
                {
                    AU_LOG instance = new AU_LOG();
                    instance.Id = Guid.NewGuid().ToString();
                    instance.NGAY = ngay;
                    instance.TrangThai = trangThai;
                    instance.MaNhanVien = maNhanVien;
                    instance.MaMayBan = maMayBan;
                    instance.ThoiGian = ngay.Hour.ToString() + ":" + ngay.Minute.ToString() + ":" + ngay.Second.ToString();
                    if (!string.IsNullOrEmpty(trangThai) && trangThai == "LOGIN") instance.TinhTrang = 1;
                    else if (!string.IsNullOrEmpty(trangThai) && trangThai == "LOGOUT") instance.TinhTrang = 0;
                    instance.UnitCode = unitCode;
                    instance.ICreateBy = userName;
                    instance.ICreateDate = DateTime.Now;
                    instance.ObjectState = ObjectState.Added;
                    context.AU_LOGs.Attach(instance);
                    context.Entry(instance).State = EntityState.Added;
                    var iInsert = context.SaveChanges();
                }
            }
        }

        public virtual TEntity AddUnit(TEntity instance)
        {
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                var unit = currentUser.Claims.FirstOrDefault(x => x.Type == "unitCode");
                if (unit != null)
                {
                    instance.UnitCode = unit.Value;
                    
                }
  
            }
            
            return instance;
        }

        public string BuildCode_DM(string MA_DM, string _unicode, bool _isSave)
        {
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            bool isInsert = false;
            var config = idRepo.DbSet.Where(x => x.Type == MA_DM && x.UnitCode.StartsWith(_unicode)).FirstOrDefault();
            if (config == null)
            {
                isInsert = true;
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = MA_DM,
                    Code = MA_DM,
                    UnitCode = _unicode,
                    Current = "0",
                };

            }
            var maChungTuGenerate = config.GenerateNumber();
            config.Current = maChungTuGenerate;
            result = string.Format("{0}{1}", config.Code, maChungTuGenerate);
            if(_isSave)
            {
                if (isInsert)
                {
                    idRepo.Insert(config);
                }
                else
                {
                    config.ObjectState = ObjectState.Modified;
                    UnitOfWork.Save();
                }
            }
            return result;
        }

        public string BuildCode_PTNX(string MPT_NHAP_XUAT, string _unicode, bool _isSave)
        {
            var result = "";
            bool isInsert = false;
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == MPT_NHAP_XUAT && x.UnitCode.StartsWith(_unicode)).FirstOrDefault();
            if (config == null)
            {
                isInsert = true;
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = MPT_NHAP_XUAT,
                    Code = MPT_NHAP_XUAT,
                    UnitCode = _unicode,
                    Current = "0",
                };
            }
            var maChungTuGenerate = config.GenerateNumber();
            config.Current = maChungTuGenerate;
            result = string.Format("{0}{1}", config.Code, maChungTuGenerate);
            if (_isSave)
            {
                if (isInsert)
                {
                    idRepo.Insert(config);
                }else
                {
                    config.ObjectState = ObjectState.Modified;
                    UnitOfWork.Save();
                }
            }
            return result;
        }

        public string BuildCode_BD(string BD, string _unicode, bool _isSave)
        {
            var result = "";
            bool isInsert = false;
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == BD && x.UnitCode.StartsWith(_unicode)).FirstOrDefault();
            if (config == null)
            {
                isInsert = true;
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = BD,
                    Code = BD,
                    UnitCode = _unicode,
                    Current = "0",
                };
            }
            var maBaoDateGenerate = config.GenerateNumber();
            config.Current = maBaoDateGenerate;
            result = string.Format("{0}{1}", config.Code, maBaoDateGenerate);
            if (_isSave)
            {
                if (isInsert)
                {
                    idRepo.Insert(config);
                }
                else
                {
                    config.ObjectState = ObjectState.Modified;
                    UnitOfWork.Save();
                }
            }
            return result;
        }

        public virtual string GetCurrentUnitCode()
        {
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                var unit = currentUser.Claims.FirstOrDefault(x => x.Type == "unitCode");
                if (unit != null) return unit.Value;
            }
            return "";
        }

        public virtual string GetParentUnitCode()
        {
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                var parentunit = currentUser.Claims.FirstOrDefault(x => x.Type == "parentUnitCode");
                if (parentunit != null) return parentunit.Value;
            }
            return "";
        }
        public virtual ClaimsPrincipal GetClaimsPrincipal()
        {
            var currentClaims = new ClaimsPrincipal();
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                currentClaims = (HttpContext.Current.User as ClaimsPrincipal);
            }
            return currentClaims;
        }
       
        public virtual TEntity Delete(string id)
        {
            var entity = FindById(id, false);
            if (entity == null)
            {
                throw new Exception("Bản ghi không tồn tại trong hệ thống");
            }
            entity.ObjectState = ObjectState.Deleted;
            return entity;
        }


    }
}