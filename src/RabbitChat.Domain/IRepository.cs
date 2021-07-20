using System;
using System.Linq;
using System.Linq.Expressions;

namespace RabbitChat.Domain
{
    public interface IRepository<T> where T : Entity
    {
        T Insert(T entity);

        T Update(T entity);

        T Get(Guid id);

        PagedList<TType> GetList<TType>(Expression<Func<T, bool>> where,
                                   Expression<Func<T, TType>> select,
                                   Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                   int pageNumber = 0,
                                   int pageSize = 0) where TType : class;
    }
}
