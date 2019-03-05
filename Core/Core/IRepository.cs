using System;
using System.Collections.Generic;
using System.Linq.Expressions;


namespace Core
{
    public interface IRepository<TEntity>
    {
        IEnumerable<TEntity> All();

        bool Any(Expression<Func<TEntity, bool>> predicate);

        int Count { get; }

        TEntity Create(TEntity t);
        List<TEntity> Create(List<TEntity> t);

        int Delete(TEntity t);
        int Delete(Expression<Func<TEntity, bool>> predicate);

        TEntity Find(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

        int Update(TEntity t);
    }

}
