using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.MD
{
    public interface IMdMerchandiseTypeService : IDataInfoService<MdMerchandiseType>
    {
        MdMerchandiseType CreateNewInstance();
        List<ChoiceObj> GetSelectSort();
        string SaveCode();
    }
    public class MdMerchandiseTypeService : DataInfoServiceBase<MdMerchandiseType>, IMdMerchandiseTypeService
    {
        public MdMerchandiseTypeService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdMerchandiseType, bool>> GetKeyFilter(MdMerchandiseType instance)
        {
            var unitCode = GetCurrentUnitCode();
            var maDonViCha = GetParentUnitCode();
            return x => x.MaLoaiVatTu == instance.MaLoaiVatTu && x.UnitCode.StartsWith(maDonViCha);
        }

        public MdMerchandiseType CreateNewInstance()
        {
            return new MdMerchandiseType() {
                MaLoaiVatTu = BuildCode()
            };
        }

        public List<ChoiceObj> GetSelectSort()
        {
            var result = new List<ChoiceObj>();
            var merchandiseTypeCollection = Repository.DbSet.ToList();
            var unitCode = GetCurrentUnitCode();
            var firstLevelMerchanseType =
                merchandiseTypeCollection.Where(item => string.IsNullOrEmpty(item.MaCha)).OrderBy(x => x.TenLoaiVatTu).Where(x => x.UnitCode == unitCode).ToList();
            foreach (var merchandise in firstLevelMerchanseType)
            {
                AddData(result, merchandiseTypeCollection, merchandise);
            }
            return result;
        }
        private void AddData(List<ChoiceObj> result, List<MdMerchandiseType> data, MdMerchandiseType current, string prefix = "")
        {
            result.Add(new ChoiceObj
            {
                Id = current.Id,
                Value = current.MaLoaiVatTu,
                Text = string.Format("{1}{0}", current.TenLoaiVatTu, prefix)
            });
            var children = data.Where(item => item.MaCha == current.MaLoaiVatTu).ToList();
            foreach (var child in children)
            {
                AddData(result, data, child, prefix + "--");
            }
        }
        public string BuildCode()
        {
            var unitCode = GetCurrentUnitCode();
            var maDonViCha = GetParentUnitCode();
            var type = TypeMasterData.LOAIHANG.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var strType = type.ToString();
            var config = idRepo.DbSet.Where(x => x.Type == strType && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = strType,
                    Current = ((char)64).ToString(),
                    UnitCode  = maDonViCha
                };   
            }
                result = config.GenerateChar();
                config.Current = result;
            return result;
        }
        public string SaveCode()
        {
            var maDonViCha = GetParentUnitCode();
            var type = TypeMasterData.LOAIHANG.ToString();
            var result = "";
            var idRepo = UnitOfWork.Repository<MdIdBuilder>();
            var strType = type.ToString();
            var config = idRepo.DbSet.Where(x => x.Type == strType && x.UnitCode == maDonViCha).FirstOrDefault();
            if (config == null)
            {
                config = new MdIdBuilder
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = strType,
                    Current = ((char)64).ToString(),
                    UnitCode = maDonViCha
                };
                result = config.GenerateChar();
                config.Current = result;
                idRepo.Insert(config);
            }
            else
            {
                result = config.GenerateChar();
                config.Current = result;
                config.ObjectState = ObjectState.Modified;
            }
;
            return result;
        }
    }
}
