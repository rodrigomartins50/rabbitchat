using RabbitChat.Data.Repositories;
using RabbitChat.Domain.Entities;
using System;

namespace RabbitChat.Service
{
    public class MessageService : BaseService<Message>
    {
        public MessageService(MessageRepository repository) : base(repository)
        {
        }

        public override Message Insert(Message entity)
        {
            entity.DateRegister = DateTime.Now;

            return base.Insert(entity);
        }

        internal void ReadMessage(Guid messageId)
        {
            var message = _repository.Get(messageId);

            message.Read = true;

            this.Update(message);
        }
    }
}
