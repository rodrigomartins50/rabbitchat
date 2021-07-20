using RabbitChat.Domain.Entities;
using RabbitChat.Domain.Repositories;
using RabbitChat.Domain.Services;
using System;

namespace RabbitChat.Service
{
    public class MessageService : Service<Message>, IMessageService
    {
        public MessageService(IMessageRepository repository) : base(repository)
        {
        }

        public override Message Insert(Message entity)
        {
            entity.DateRegister = DateTime.Now;

            return base.Insert(entity);
        }
    }
}
