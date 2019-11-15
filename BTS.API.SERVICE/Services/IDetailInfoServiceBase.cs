using BTS.API.ENTITY;
using System;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.Services
{
    public interface IDetailInfoServiceBase<TEntity> : IEntityService<TEntity>
        where TEntity : DetailInfoEntity
    {
        new IDetailInfoServiceBase<TEntity> Include(Expression<Func<TEntity, object>> include);
        TEntity Find(TEntity instance, bool notracking = false);
        TEntity FindById(string id, bool notracking = false);

        TEntity Insert(TEntity instance);

        TEntity Update(TEntity instance,
            Action<TEntity, TEntity> updateAction = null,
            Func<TEntity, TEntity, bool> updateCondition = null);

        TEntity Delete(string id);
    }
}
