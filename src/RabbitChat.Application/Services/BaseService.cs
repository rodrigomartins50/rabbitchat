using RabbitChat.Data;
using RabbitChat.Domain;
using System;

namespace RabbitChat.Service
{
    public abstract class BaseService<TEntity> where TEntity : Entity
    {
        protected BaseRepository<TEntity> _repository;

        public BaseService(BaseRepository<TEntity> repository)
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
