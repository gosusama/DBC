using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Helper;
namespace BTS.API.SERVICE.Authorize.AuDonVi
{
    public interface IAuDonViService : IDataInfoService<AU_DONVI>
    {
        string BuildCode();
        string BuildCodeByParent(string parent);
        string SaveCodeByParent(string parent);
        string SaveCode();
        List<ChoiceObj> GetSelectSort();
    }
    public class AuDonViService : DataInfoServiceBase<AU_DONVI>, IAuDonViService
    {
        public AuDonViService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public List<ChoiceObj> GetSelectSort()
        {
            var result = new List<ChoiceObj>();
            var merchandiseTypeCollection = Repository.DbSet.ToList();
            var firstLevelMerchanseType =
                merchandiseTypeCollection.Where(item => string.IsNullOrEmpty(item.MaDonViCha)).OrderBy(x => x.TenDonVi).ToList();
            foreach (var merchandise in firstLevelMerchanseType)
            {
                AddData(result, merchandiseTypeCollection, merchandise);
            }
            return result;
        }
        private void AddData(List<ChoiceObj> result, List<AU_DONVI> data, AU_DONVI current, string prefix = "")
        {
            result.Add(new ChoiceObj
            {
                Id = current.Id,
                Value = current.MaDonVi,
                Text = string.Format("{1}{0}", current.TenDonVi, prefix)
            });
            var children = data.Where(item => item.MaDonViCha == current.MaDonVi).ToList();
            foreach (var child in children)
            {
                AddData(result, data, child, prefix + "--");
            }
        }
        public string BuildCodeByParent(string parent)
        {
            var type = TypeMasterData.CH.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == parent).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    UnitCode = parent,
                    Current = "0",
                };
            }
            var soMa = config.GenerateNumber();
            config.Current = soMa;
            result = parent + "-" + config.Type + soMa;

            return result;
        }

        public string SaveCodeByParent(string parent)
        {
            var type = TypeMasterData.CH.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type && x.UnitCode == parent).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    UnitCode = parent,
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
            result = parent + "-" + config.Code + config.Current;
            return result;
        }

        public string BuildCode()
        {
            var type = TypeMasterData.DV.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
                    Current = "0",
                };
            }
            var soMa = config.GenerateNumber();
            config.Current = soMa;
            result = config.Type + soMa;

            return result;
        }

        public string SaveCode()
        {
            var type = TypeMasterData.DV.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var config = idRepo.DbSet.Where(x => x.Type == type).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = type,
                    Code = type,
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
            result = config.Code + config.Current;
            return result;

        }
        protected override Expression<Func<AU_DONVI, bool>> GetKeyFilter(AU_DONVI instance)
        {
            return x => x.MaDonVi == instance.MaDonVi;
        }
    }
}
