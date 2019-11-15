using BTS.API.ENTITY;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BTS.API.ENTITY.Authorize;

namespace BTS.API.SERVICE.Services
{
    public interface IDataInfoService<TEntity> : IEntityService<TEntity>
        where TEntity : DataInfoEntity
    {
        new IDataInfoService<TEntity> Include(Expression<Func<TEntity, object>> include);
        TEntity Find(TEntity instance, bool notracking = false);
        TEntity FindById(string id, bool notracking = false);

        TEntity Insert(TEntity instance, bool withUnitCode = true);

        TEntity Update(TEntity instance,
            Action<TEntity, TEntity> updateAction = null,
            Func<TEntity, TEntity, bool> updateCondition = null);

        TEntity Delete(string id);
        TEntity AddUnit(TEntity instance);
        string GetCurrentUnitCode();
        string GetParentUnitCode();
        //string GetCurrentUsername();
        string BuildCode_PTNX(string MPT_NHAP_XUAT, string _unicode, bool _isSave);
        string BuildCode_DM(string MA_DM, string _unicode, bool _isSave);
        string GetPhysicalPathImportFile();
        ClaimsPrincipal GetClaimsPrincipal();
        bool ValidateNgayCT(DateTime ngayCt);
        void WiteLog(DateTime ngay, string maMayBan, string maNhanVien, string trangThai, string unitCode, string userName);
    }
}