using Microsoft.EntityFrameworkCore;
using RabbitChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RabbitChat.Data
{
    public class BaseRepository<T> where T : Entity
    {
        protected DbContext _context;
        protected DbSet<T> _dataSet;

        public BaseRepository(DbContext context)
        {
            _context = context;
            _dataSet = _context.Set<T>();

            context.Database.EnsureCreated();
        }

        public T Get(Guid id)
        {
            return _dataSet.Where(m => m.Id == id).Take(1).Single();
        }

        public PagedList<T> GetList(Expression<Func<T, bool>> where,
                                    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
                                    int pageNumber = 0, 
                                    int pageSize = 0)
        {
            PagedList<T> pagedList = new PagedList<T>();

            List<T> list = null;

            var query = _dataSet.Where(where).OrderBy(t => t.Id);

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (pageNumber > 0 || pageSize > 0)
            {
                list = query.AsNoTracking()
                            .Skip(pageNumber * pageSize)
                            .Take(pageSize)
                            .ToList();

                pagedList.Total = _dataSet.Count(where);
            }
            else
            {
                list = query.AsNoTracking().ToList();

                pagedList.Total = list.Count;
            }

            foreach (var item in list)
            {
                pagedList.Add(item);
            }

            return pagedList;
        }

        public T Insert(T entity)
        {
            _dataSet.Add(entity);
            _context.SaveChanges();

            return entity;
        }

        public T Update(T entity)
        {
            var result = _dataSet.SingleOrDefault(b => b.Id == entity.Id);

            _context.Entry(result).CurrentValues.SetValues(entity);
            _context.SaveChanges();

            return result;
        }
    }
}
