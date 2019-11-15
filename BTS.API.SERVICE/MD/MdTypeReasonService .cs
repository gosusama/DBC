using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System.Linq.Expressions;
using BTS.API.SERVICE.Helper;

namespace BTS.API.SERVICE.MD
{
    public interface IMdTypeReasonService : IDataInfoService<MdTypeReason>
    {
        List<ChoiceObj> GetSelectSort(TypeXN type);
        List<ChoiceObj> GetSelectSort();
        //Add function here
    }
    public class MdTypeReasonService : DataInfoServiceBase<MdTypeReason>, IMdTypeReasonService
    {
        public MdTypeReasonService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdTypeReason, bool>> GetKeyFilter(MdTypeReason instance)
        {
            var unitCode = GetCurrentUnitCode();
            return x => x.MaLyDo == instance.MaLyDo && x.UnitCode == unitCode;
        }

        public List<ChoiceObj> GetSelectSort(TypeXN type)
        {
            var unitCode = GetCurrentUnitCode();
            var result = new List<ChoiceObj>();
            var typeReasonCollection = Repository.DbSet.ToList();
            var firstLevelMerchanseType =
                typeReasonCollection.Where(item => string.IsNullOrEmpty(item.MaCha) && item.Loai == type && item.UnitCode==unitCode).OrderBy(x => x.TenLyDo).ToList();
            foreach (var reason in firstLevelMerchanseType)
            {
                AddData(result, typeReasonCollection, reason);
            }
            return result;
        }
        public List<ChoiceObj> GetSelectSort()
        {
            var result = new List<ChoiceObj>();
            var typeReasonCollection = Repository.DbSet.ToList();
            var firstLevelMerchanseType =
                typeReasonCollection.Where(item => string.IsNullOrEmpty(item.MaCha)).OrderBy(x => x.TenLyDo).ToList();
            foreach (var reason in firstLevelMerchanseType)
            {
                AddData(result, typeReasonCollection, reason);
            }
            return result;
        }
        private void AddData(List<ChoiceObj> result, List<MdTypeReason> data, MdTypeReason current, string prefix = "")
        {
            var unitCode = GetCurrentUnitCode();
            result.Add(new ChoiceObj
            {
                Id = current.Id,
                Value = current.MaLyDo,
                Text = string.Format("{1}{0}", current.TenLyDo, prefix)
            });
            var children = data.Where(item => item.MaCha == current.MaLyDo && item.UnitCode==unitCode).ToList();
            foreach (var child in children)
            {
                AddData(result, data, child, prefix + "--");
            }
        }
    }
}
