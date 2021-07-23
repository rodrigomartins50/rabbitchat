using Microsoft.AspNetCore.SignalR;
using RabbitChat.Application.SignalR;
using RabbitChat.Data.Repositories;
using RabbitChat.Domain.Entities;
using System;

namespace RabbitChat.Service
{
    public class MessageService : BaseService<Message>
    {
        private IHubContext<RabbitChatHub> _hub;

        public MessageService(MessageRepository repository, IHubContext<RabbitChatHub> hub) : base(repository)
        {
            _hub = hub;
        }

        public override Message Insert(Message entity)
        {
            entity.DateRegister = DateTime.Now;

            var message = base.Insert(entity);


            _hub.Clients.All.SendAsync("ReceiveNewMessage", entity.Text);

            return message;
        }

        internal void ReadMessage(Guid messageId)
        {
            var message = _repository.Get(messageId);

            message.Read = true;

            this.Update(message);
        }
    }
}
