using System;

namespace RabbitChat.Domain
{
    public interface IService<TEntity> where TEntity : Entity
    {
        TEntity Insert(TEntity entity);

        TEntity Update(TEntity entity);

        TEntity Get(Guid id);
    }
}
