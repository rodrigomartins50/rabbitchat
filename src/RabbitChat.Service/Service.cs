using RabbitChat.Domain;
using System;

namespace RabbitChat.Service
{
    public abstract class Service<TEntity> where TEntity : Entity
    {
        protected IRepository<TEntity> _repository;

        public Service(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual TEntity Insert(TEntity entity)
        {
            return _repository.Insert(entity);
        }

        public virtual TEntity Update(TEntity entity)
        {
            return _repository.Update(entity);
        }

        public TEntity Get(Guid id)
        {
            return _repository.Get(id);
        }
    }
}
