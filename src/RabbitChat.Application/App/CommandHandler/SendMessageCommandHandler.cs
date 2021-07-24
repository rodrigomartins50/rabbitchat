using MediatR;
using Microsoft.AspNetCore.SignalR;
using RabbitChat.Application.App.Command;
using RabbitChat.Application.SignalR;
using RabbitChat.Data.Repositories;
using RabbitChat.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitChat.Application.App.CommandHandler
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, bool>
    {
        private MessageRepository _messageRepository;
        private IHubContext<RabbitChatHub> _hub;

        public SendMessageCommandHandler(MessageRepository messageRepository, IHubContext<RabbitChatHub> hub)
        {
            _messageRepository = messageRepository;
            _hub = hub;
        }

        public Task<bool> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new Message();

            message.Text = request.Text;
            message.FromUser = request.FromUser;
            message.DateRegister = request.DateRegister;

            message = this._messageRepository.Insert(message);

            if (_hub != null)
            {
                _hub.Clients.All.SendAsync("ReceiveNewMessage", message);
            }

            return Task.FromResult(true);
        }
    }
}
